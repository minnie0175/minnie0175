using Ciri.Interface;
using Ciri.Properties;
using Ciri.Util;
using CiriData.Data;
using CiriData.Enums;
using CiriData.Manage;
using CiriData.StaticData;
using CommonLib.Data;
using CommonLib.DBDataType;
using CommonLib.Enums;
using CommonLib.Manage;
using CommonLib.Util;
using MongoDB.Driver.Core.Servers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TenTec.Windows.iGridLib;
using WeifenLuo.WinFormsUI.Docking;

namespace Ciri.Forms
{
    public partial class ETFQuoter : DockContent, IObserver<LPItemStatusSnapshot>, IObserver<ItemSelectNotifyInfo>, IQuoterForm
    {
        public class ETFQuoterInitData
        {
            public string group;
            public bool lpOnly;
            public Dictionary<string, int> widthDict;
            public string serverId;

            public ETFQuoterInitData(string group, bool lpOnly, Dictionary<string, int> widthDict, string serverId)
            {
                this.group = group;
                this.lpOnly = lpOnly;
                this.widthDict = widthDict;
                this.serverId = serverId;
            }
        }

        public string serverId { get; private set; }
        string group = "";
        ServerInfoManager serverInfoManager;
        QuoterInitiator quoterInitiator;

        const string defaultBookCode = "M:ETF_ETC";

        public int currentMonth;
        IDisposable itemSelectNotifierUnsubscriber;
        readonly Timer timer = new Timer();
        Dictionary<string, ETFLPDoc> etfLPDic;

        Dictionary<string, int> neutralPositionDic;

        int onUpdate = 0;
        string defaultPurpose = null;

        string filterGroup = "";
        bool filterLPDutyOnly = true;
        SortedSet<string> etfGroupSet = new SortedSet<string>();

        string etfSiseViewMode = "A";

        public ETFQuoter(string serverId)
        {
            this.serverId = serverId;
            InitializeComponent();
            IGridUtil.UnlockIGrid(iGridETF);
            this.Text += "/" + serverId;

            DateTime dateTime = DateTime.Now;
            int nearMonth = DateTimeUtil.GetNearbyMonth(dateTime);
            currentMonth = nearMonth;

            underlyingGridView.initLater();
            etfSiseGridView.initLater();

            InitNeutralPositionDic();

            //InitETFTable();
            //SelectETF(iGridETF.Rows[0].Cells["종목코드"].Text, iGridETF.Rows[0].Cells["기초자산"].Text, "L");
            quoteControl.init(QuotingProduct.ETF, "", true);

            //iGridETF.DoAutoResizeCols();
            quoterInitiator = new QuoterInitiator(this);
            quoterInitiator.InitOrRegisterQuoter();
        }
        public void InitQuoter(ServerInfoManager serverInfoManager)
        {
            this.serverInfoManager = serverInfoManager;
            quoteControl.GetCTMList += new QuotingInfoControl.CTMListGetter(serverInfoManager.GetCTMList);
            quoteControl.GetDBInfoManager += new QuotingInfoControl.DBQuotingInfoManagerGetter(serverInfoManager.GetDBInfoManager);
            quoteControl.GetCiriAndOAInfoManager += new QuotingInfoControl.CiriAndOAQuotingInfoManagerGetter(serverInfoManager.GetCiriAndOAQuotingInfoManager);



            timer.Interval = 1000;
            timer.Tick += new EventHandler(dgv_Updater);
            timer.Start();

            InitETFTable();
            SetCodeToMonitorList();
            //quoteControl.init(QuotingProduct.ETF, iGridETF.Rows[0].Cells["종목코드"].Text, true);
            SelectETF(iGridETF.Rows[0].Cells["종목코드"].Text, iGridETF.Rows[0].Cells["기초자산"].Text, "L");

            int idx;

            idx = comboBoxETFGroup.Items.IndexOf(group);
            if (idx >= 0)
                comboBoxETFGroup.SelectedIndex = idx;
        }

        private void InitETFTable()
        {
            etfLPDic = DBUtil.Instance.GetETFLPList();
            List<ETFLPDoc> docList = etfLPDic.Values.ToList();
            docList.Sort(ETFLPDoc.CompareIdx);
            foreach (var data in docList)
            {
                iGRow row = iGridETF.Rows.Add();
                row.Cells["종목명"].Value = data.korean;
                row.Cells["종목코드"].Value = data.isinCode;
                row.Cells["Info#"].Value = 0;
                row.Cells["Purpose"].Value = "";
                row.Cells["Style"].Value = "";
                row.Cells["운용사"].Value = data.운용사;
                row.Cells["기초자산"].Value = data.underlying;
                row.Cells["LP"].Value = data.LP계약여부;
                row.Cells["만족"].Value = "";
                row.Cells["주문수"].Value = "";
                row.Cells["이론Bid"].Value = "";
                row.Cells["이론Ask"].Value = "";
                row.Cells["매수"].Value = "";
                row.Cells["매도"].Value = "";
                row.Cells["매도가능"].Value = "";
                row.Cells["설정수량"].Value = "";
                row.Cells["순포지션"].Value = "";
                row.Cells["D.Ask"].Value = "";
                row.Cells["D.Bid"].Value = "";
                row.Cells["AL"].Value = "";
                row.Cells["BL"].Value = "";
                row.Cells["그룹"].Value = data.group;
                if (data.group.Trim() != "")
                    etfGroupSet.Add(data.group);

                iGRow subRow = iGridETF.Rows.Add();
                subRow.Cells["종목명"].Value = data.korean;
                subRow.Cells["종목코드"].Value = data.isinCode;
                subRow.Cells["Info#"].Value = 0;
                subRow.Cells["Purpose"].Value = "S";
                subRow.Cells["Style"].Value = "";
                subRow.Cells["운용사"].Value = data.운용사;
                subRow.Cells["기초자산"].Value = data.underlying;
                subRow.Cells["LP"].Value = data.LP계약여부;
                subRow.Cells["만족"].Value = "";
                subRow.Cells["주문수"].Value = "";
                subRow.Cells["이론Bid"].Value = "";
                subRow.Cells["이론Ask"].Value = "";
                subRow.Cells["매수"].Value = "";
                subRow.Cells["매도"].Value = "";
                subRow.Cells["매도가능"].Value = "";
                subRow.Cells["설정수량"].Value = "";
                subRow.Cells["순포지션"].Value = "";
                subRow.Cells["D.Ask"].Value = "";
                subRow.Cells["D.Bid"].Value = "";
                subRow.Cells["AL"].Value = "";
                subRow.Cells["AL"].Value = "";
                subRow.Cells["그룹"].Value = data.group;
                subRow.CellStyle.BackColor = Color.Gainsboro;
            }

            comboBoxETFGroup.Items.Clear();
            comboBoxETFGroup.Items.Add("전체");
            comboBoxETFGroup.SelectedIndex = 0;
            foreach (string group in etfGroupSet)
            {
                comboBoxETFGroup.Items.Add(group);
            }
        }

        private void SetCodeToMonitorList()
        {
            for (int i = 0; i < iGridETF.Rows.Count; i++)
            {
                string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                SynchronizedCollection<string> CTMList;
                serverInfoManager.codeToMonitorTable.TryGetValue(isinCode, out CTMList);

                if (CTMList == null)
                {
                    CTMList = new SynchronizedCollection<string>();
                    serverInfoManager.codeToMonitorTable.TryAdd(isinCode, CTMList);

                    CTMList.Add(isinCode);
                }
            }
        }

        private void ApplyFilter()
        {
            bool checkGroup = true;
            if (filterGroup == "" || filterGroup == "전체" || !etfGroupSet.Contains(filterGroup))
                checkGroup = false;

            iGridSortHelper helper = iGridSortHelper.CreateAndBeginUpdate(iGridETF);
            try
            {
                for (int i = iGridETF.Rows.Count - 1; i >= 0; i--)
                {
                    bool visible = true;
                    string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();

                    if (filterLPDutyOnly && (bool)iGridETF.Rows[i].Cells["LP"].Value == false)
                        visible = visible && false;

                    if (checkGroup)
                    {
                        string group = iGridETF.Rows[i].Cells["그룹"].Value.ToString();
                        if (group != filterGroup)
                        {
                            visible = visible && false;
                        }
                    }

                    iGridETF.Rows[i].Visible = visible;

                    if (visible)
                    {
                        MarketDataCrawler.Instance.addCodeToMonitorBidAsk(isinCode);
                    }
                }
            }
            finally
            {
                helper.EndUpdate();
            }
        }

        private void InitNeutralPositionDic()
        {
            neutralPositionDic = DBUtil.Instance.GetETFNeutralPositionDic();
        }

        public Color GetBackColorConsideredSubPurpose(Color color, string purpose)
        {
            if (color == Color.White && purpose == Purpose.SUB.purpose)
                return Color.Gainsboro;

            return color;
        }

        private void dgv_Updater(object sender, EventArgs eArgs)
        {
            if (0 != System.Threading.Interlocked.Exchange(ref onUpdate, 1))
                return;

            bool defaultPurposeMode = false;
            string defaultPurposeTemp = "";

            if (defaultPurpose != null)
            {
                defaultPurposeMode = true;
                defaultPurposeTemp = defaultPurpose;
            }

            try
            {
                QuotingInfo info = quoteControl.getQuotingInfo();
                if (info == null)
                    return;

                if (!this.IsFloat && !(this.DockHandler.Pane != null && this.DockHandler.Pane.ActiveContent != null && this.DockHandler.Pane.ActiveContent.DockHandler == this.DockHandler))
                    return;

                string etfIsinCode = info.isinCode;
                serverInfoManager = ServerManager.Instance.GetServerInfoManagerFromServerId(serverId);
                //쿼팅 인포 데이터로 제출 호가 표시
                QuotingInfo etfInfo, woInfo = null;
                if (serverInfoManager.GetQuotingInfo(info.isinCode, info.purpose, out etfInfo))
                {
                    if (etfSiseViewMode == "2")
                    {
                        if (Purpose.mainSubStringSet.Contains(info.purpose))
                            woInfo = serverInfoManager.GetWOUpdatedQuotingInfo(etfInfo, Purpose.mainSubStringSet);
                    }
                    else if (etfSiseViewMode == "A")
                    {
                        woInfo = serverInfoManager.GetWOUpdatedQuotingInfo(etfInfo);
                    }

                    if (isWO.Checked)
                        etfSiseGridView.UpdateBidAskFromWO(etfInfo);
                    else
                        etfSiseGridView.UpdateMarketBidAsk();
                    etfSiseGridView.UpdateWithQuotingInfo(etfInfo, woInfo);
                }
                else
                {
                    etfSiseGridView.UpdateMarketBidAsk();
                }

                BidAskData ba = MarketDataCenter.Instance.GetBidAskData(etfIsinCode);
                var wolist = DBUtil.Instance.GetHedgeServerWorkingOrder(etfIsinCode);
                if (wolist == null)
                {
                    MessageBox.Show("DB오류로 인해 데이터를 가져오지 못했습니다.");
                }
                else
                {
                    foreach (var doc in wolist)
                    {
                        int price = doc.price;
                        int amt = doc.remainingAmount;

                        if (doc.isLong == "LONG")
                        {
                            int idx = Array.FindIndex<int>(ba.bidPrice, (a) => { return a == price; });
                            if (idx >= 0)
                            {
                                ba.bidAmount[idx] += amt;
                            }
                        }
                        else if (doc.isLong == "SHORT")
                        {
                            int idx = Array.FindIndex<int>(ba.askPrice, (a) => { return a == price; });
                            if (idx >= 0)
                            {
                                ba.askAmount[idx] += amt;
                            }
                        }
                    }

                }

                underlyingGridView.UpdateMarketBidAsk();

                var code = etfSiseGridView.getIsinCode();
                if (code != null)
                {
                    BidAskData bad = MarketDataCenter.Instance.GetBidAskData(code);
                    if (bad != null)
                        label16.Text = bad.announcedNav == 0 ? $"ETF호가" : $"ETF호가 (NAV: {bad.announcedNav.ToString("F2")})";
                }

                iGridETF.BeginUpdate();
                QuotingInfo quoteInfo;
                for (int i = 0; i < iGridETF.Rows.Count; ++i)
                {
                    iGRow row = iGridETF.Rows[i];
                    string isinCode = row.Cells["종목코드"].Value.ToString();
                    string purpose = row.Cells["Purpose"].Value.ToString();

                    // QuotingInfo가 없더라도 등락률은 업데이트. Sub Purpose에는 등락률 표시 X
                    BidAskData bad = MarketDataCenter.Instance.GetBidAskData(isinCode);
                    if (bad != null && purpose != "S")
                    {
                        row.Cells["전일%"].Value = bad.prevDiffPcnt.ToString("+0.00;-0.00;0"); ;
                    }

                    quoteInfo = null;

                    // Sub purpose는 무조건 Sub만 업데이트
                    if (purpose == Purpose.SUB.purpose)
                    {
                        serverInfoManager.GetQuotingInfo(isinCode, purpose, out quoteInfo);
                    }
                    // 나머지 Purpose는 Sub 빼고 업데이트
                    else
                    {
                        // 현재 선택한 Purpose가 Sub이면 그냥 해당 행의 Purpose를 사용해야 함
                        if (info.purpose != Purpose.SUB.purpose && isinCode == info.isinCode)
                            purpose = info.purpose;
                        else if (defaultPurposeMode)
                            serverInfoManager.GetQuotingInfoWithoutSub(isinCode, defaultPurpose, out quoteInfo);

                        if (quoteInfo == null)
                            serverInfoManager.GetQuotingInfoWithoutSub(isinCode, purpose, out quoteInfo);
                    }


                    if (quoteInfo != null)
                    {
                        row.Cells["상태"].Value = quoteInfo.stateCode;
                        var multiQuoterState = serverInfoManager.GetMultiQuoterState(quoteInfo.isinCode);
                        row.Cells["Info#"].BackColor = multiQuoterState.GetColor(GetDefaultBackColor(quoteInfo.purpose));
                        row.Cells["Info#"].Value = serverInfoManager.GetWorkingInfoCount(quoteInfo.isinCode);
                        row.Cells["Purpose"].Value = quoteInfo.purpose;
                        row.Cells["Style"].Value = quoteInfo.style;
                        row.Cells["주문수"].Value = quoteInfo.orderSubmitted;
                        row.Cells["이론Bid"].Value = quoteInfo.expectedBid;
                        row.Cells["이론Ask"].Value = quoteInfo.expectedAsk;
                        row.Cells["매도가능"].Value = quoteInfo.equityShortAvailable;

                        int creationAmount = 0;
                        int netPosition = quoteInfo.equityShortAvailable;
                        if (neutralPositionDic.ContainsKey(quoteInfo.isinCode))
                        {
                            creationAmount = neutralPositionDic[quoteInfo.isinCode];
                            netPosition = quoteInfo.equityShortAvailable - creationAmount;
                        }

                        row.Cells["설정수량"].Value = creationAmount;
                        row.Cells["순포지션"].Value = netPosition;
                        row.Cells["매수"].Value = quoteInfo.longContracted;
                        row.Cells["매도"].Value = quoteInfo.shortContracted;
                        row.Cells["D.Ask"].Value = quoteInfo.decorationAskSkew;
                        row.Cells["D.Bid"].Value = quoteInfo.decorationBidSkew;
                        int maxSkew = 5 * quoteInfo.GetDecorationMainTick() / 100;
                        row.Cells["D.Ask"].BackColor = GetBackColorConsideredSubPurpose(IGridUtil.GetColorForDecoration(row.Cells["D.Ask"], maxSkew), quoteInfo.purpose);
                        row.Cells["D.Bid"].BackColor = GetBackColorConsideredSubPurpose(IGridUtil.GetColorForDecoration(row.Cells["D.Ask"], maxSkew), quoteInfo.purpose);

                        row.Cells["AL"].Value = quoteInfo.askVol.ToString("##,###");
                        row.Cells["BL"].Value = quoteInfo.bidVol.ToString("##,###");

                        double bidRatio = 0, askRatio = 0;

                        if (quoteInfo.woLongPriceAmt == null || quoteInfo.woShortPriceAmt == null)
                            return;
                        int shortWoAmt = quoteInfo.woShortPriceAmt.Select(entry => entry.Value).Aggregate(0, (sum, amt) => sum + amt);
                        int longWoAmt = quoteInfo.woLongPriceAmt.Select(entry => entry.Value).Aggregate(0, (sum, amt) => sum + amt);

                        if (quoteInfo.marketBidTotal > 0)
                            bidRatio = (double)longWoAmt / quoteInfo.marketBidTotal;
                        if (quoteInfo.marketAskTotal > 0)
                            askRatio = (double)shortWoAmt / quoteInfo.marketAskTotal;

                        Color bidColor = GetDefaultBackColor(quoteInfo.purpose);
                        Color askColor = GetDefaultBackColor(quoteInfo.purpose);
                        // 호가수량 10% 초과시 Grid에서 색깔 표시할 곳
                        if (quoteInfo.stateCode == "OFF")
                        {
                            bidColor = askColor = GetDefaultBackColor(quoteInfo.purpose);
                        }
                        else
                        {
                            if (bidRatio >= 0.1)
                                bidColor = Color.Orange;
                            if (askRatio >= 0.1)
                                askColor = Color.Orange;
                        }

                        row.Cells["종목명"].BackColor = quoteInfo.stateCode == "OFF" ? GetDefaultBackColor(quoteInfo.purpose) : GetBackColorConsideredSubPurpose(OPKUtil.GetColor(quoteInfo.opk), quoteInfo.purpose);
                        row.Cells["매수"].BackColor = GetBackColorConsideredSubPurpose(bidColor, quoteInfo.purpose);
                        row.Cells["매도"].BackColor = GetBackColorConsideredSubPurpose(askColor, quoteInfo.purpose);
                    }
                }
                iGridETF.EndUpdate();
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                System.Threading.Interlocked.Exchange(ref onUpdate, 0);
            }
        }

        private Color GetDefaultBackColor(string purpose)
        {
            return purpose == "S" ? Color.Gainsboro : Color.White;
        }

        private bool checkAccountValidity(string account, string isinCode)
        {
            if (!AccountMaster.Instance.IsAllowedItem(account, isinCode) || AccountMaster.Instance.IsDisllowedItem(account, isinCode))
            {
                MessageBox.Show(string.Format("{1} 종목은 {0} 계좌에서 주문이 불가능합니다. 주문을 원할 경우 AccountMaster를 수정해주세요.", account, isinCode));
                return false;
            }
            else
            {
                return true;
            }
        }
        private void ETFQuoter_Load(object sender, EventArgs e)
        {
            itemSelectNotifierUnsubscriber = ItemSelectNotifier.Instance.Subscribe(this);
        }

        private void StartOrUpdateQuote()
        {
            QuotingInfo info = quoteControl.getQuotingInfo();

            if (info == null)
                return;

            if (info.isinCode.Equals(String.Empty))
            {
                CiriForm.Instance.SafeInvoke(form => MessageBox.Show(form, "상품코드 입력오류", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return;
            }

            // 늘어날땐 2배씩만..
            //numericAmount.Maximum = (info.amountLong == 0)?10:info.amountLong * 2;

            //if (radioHedgeAble.Checked)
            //    refillHedge(info.amountLong * info.depth * 2);

            info.depth = 1;
            info.isLp = StaticValues.LP;
            info.netContracted = 0;
            info.autoDutySkew = false;

            info.SetTrCodeQuotingType();
            info.SetCommandQuoteStart();

            checkAndShootInfo(info);
        }

        // 211104 ETFQuoter는 만족 표시 X
        #region LPItemStatus Observer 관련
        public void OnNext(LPItemStatusSnapshot value)
        {
            // do nothing
        }

        public void OnError(Exception error)
        {
            //throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            //throw new NotImplementedException();
        }
        #endregion

        private void iGridStock_CellClick(object sender, iGCellClickEventArgs e)
        {
            string etf = iGridETF.Cells[e.RowIndex, "종목코드"].Value.ToString();
            string under = iGridETF.Cells[e.RowIndex, "기초자산"].Value.ToString();
            string purpose = iGridETF.Cells[e.RowIndex, "Purpose"].Value.ToString();
            SelectETF(etf, under, purpose);
        }

        private void SelectETF(string isinCode, string uid, string purpose)
        {
            if (quoteControl.CheckMultiMode(iGridETF))
                return;

            StockInstrument si = ItemMaster.Instance.GetStockItemWithIsinCode(isinCode);
            if (si == null)
                return;
            string underFutIsinCode = GetUnderlyingFuturesIsinCode(uid);

            MarketDataCrawler.Instance.addCodeToMonitorBidAsk(isinCode);
            if (underFutIsinCode != null)
                MarketDataCrawler.Instance.addCodeToMonitorBidAsk(underFutIsinCode);

            stockName.Text = si.prodName;

            underlyingGridView.setIsinCode(underFutIsinCode);
            etfSiseGridView.setIsinCode(isinCode);

            QuotingInfo info;
            if(serverInfoManager == null)
            {
                return;
            }
            serverInfoManager.GetQuotingInfo(isinCode, purpose, out info);

            if (info == null)
            {
                info = new QuotingInfo();
                info.isinCode = isinCode;
                info.account = AccountMap.ACCOUNT_NH_EQUITY_REAL1;
                info.bookCode = defaultBookCode;
                if (underFutIsinCode != null)
                    info.codeToMonitor = underFutIsinCode;
                else
                    info.codeToMonitor = isinCode;
                info.purpose = "L";
                if (purpose == "S")
                    info.purpose = purpose;
                info.style = "기본";

                //LPItemRule rule = LPDutyManager.Instance.GetLPItemRule(isinCode);
                //if (rule != null)
                //{
                //    info.SetDutyAmount(rule.DutyAmount);
                //}
                //else
                //{
                //    info.SetDutyAmount(1000);
                //}

                info.skewUnit = Math.Max(1, (int)(si.DownTickSize / 2));
                info.askSkew = info.skewUnit * 1;

                info.logic = "ETF";

                info.weight = 0;
                info.weight2 = 0;

                info.bullets = 10000;
                info.depth = 5;

                info.isLp = StaticValues.LP;
            }

            quoteControl.setQuotingInfo(info);
            quoteControl.refreshSelectPurpose();
            quoteControl.setComboboxStyle(info.style);

            ItemSelectNotifier.Instance.SelectItem(isinCode, this.GetType().Name, new string[] { "DailyPositionForm", "ManualOrder" }, this, "");
        }

        private void ETFQuoter_FormClosing(object sender, FormClosingEventArgs e)
        {
            itemSelectNotifierUnsubscriber.Dispose();
            timer.Stop();
            quoterInitiator.UnregisterQuoter();
        }


        private void buttonSummitAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < iGridETF.Rows.Count; i++)
            {
                if (!iGridETF.Rows[i].Visible)
                    continue;

                String isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                string purpose = iGridETF.Rows[i].Cells["Purpose"].Value.ToString();

                QuotingInfo info;
                serverInfoManager.GetQuotingInfo(isinCode, purpose, out info);

                // QuoterInfo
                if (info == null)
                {
                    continue;
                }

                info.SetTrCodeQuotingType();
                info.SetCommandQuoteStart();

                info.SetFitToMarket(Control.ModifierKeys);

                if (!checkAccountValidity(info.account, info.isinCode))
                    return;
                JToken token = JToken.FromObject(info);
                String packet = token.ToString();
                if (serverInfoManager.clientConn == null) MessageBox.Show("서버에 연결되지 않았습니다");
                else
                {
                    serverInfoManager.clientConn.writeWithLength(packet);
                    //System.Threading.Thread.Sleep(100);
                }
            }
        }

        private void iGridStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            QuotingInfo info = quoteControl.getQuotingInfo();
            if (info == null)
                return;
            if (e.KeyChar == '\r')
            {
                if (info.isinCode == "")
                    MessageBox.Show("종목을 먼저 선택해주세요.");
                else
                {
                    DialogResult dr = MessageBox.Show(string.Format("{0} 종목의 호가를 다시 제출하시겠습니까?", info.isinCode), "", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        StartOrUpdateQuote();
                    }
                }
            }
        }

        private void iGridStock_CurRowChanged(object sender, EventArgs e)
        {
            string etf = iGridETF.CurRow.Cells["종목코드"].Value.ToString();
            string uid = iGridETF.CurRow.Cells["기초자산"].Value.ToString();
            string purpose = iGridETF.CurRow.Cells["Purpose"].Value.ToString();
            if (etf != "")
            {
                SelectETF(etf, uid, purpose);
            }
        }

        private void buttonCancelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < iGridETF.Rows.Count; i++)
            {
                if (!iGridETF.Rows[i].Visible)
                    continue;

                string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                if (isinCode == null)
                    continue;

                foreach (string purpose in serverInfoManager.GetQuoterPurposeList(isinCode))
                {
                    CancelQuote(isinCode, purpose);
                }
            }
        }
        private void CancelQuote(string isinCode, string purpose)
        {
            QuotingInfo info;
            serverInfoManager.GetQuotingInfo(isinCode, purpose, out info);
            if (info == null)
            {
                return;
            }

            info.SetCommandQuoteStop(Control.ModifierKeys);
            JToken token = JToken.FromObject(info);
            String packet = token.ToString();
            if (serverInfoManager.clientConn == null) MessageBox.Show("서버에 연결되지 않았습니다"); else serverInfoManager.clientConn.writeWithLength(packet);
        }
        public void OnNext(ItemSelectNotifyInfo value)
        {
        }
        public static string GetUnderlyingFuturesIsinCode(string uid)
        {
            string underFutIsinCode = null;
            DateTime today = DateTimeCenter.Instance.GetToday();
            int todayInt = today.Year * 10000 + today.Month * 100 + today.Day;
            foreach (var fi in ItemMaster.Instance.GetFuturesInstrumentListWithUnderlyingID(uid))
            {
                if (fi.matDate >= todayInt)
                {
                    underFutIsinCode = fi.isinCode;
                    break;
                }
            }
            return underFutIsinCode;
        }

        private void quoteControl_StartButtonClick(object sender, QuoteEventArgs<QuotingInfo> e)
        {
            QuotingInfo info = e.GetQuotingInfo();
            checkAndShootInfo(info);
        }

        private void quoteControl_CancelButtonClick(object sender, QuoteEventArgs<QuotingInfo> e)
        {
            QuotingInfo info = e.GetQuotingInfo();
            info.SetCommandQuoteStop(Control.ModifierKeys);
            checkAndShootInfo(info);
        }

        private void comboBoxTargetPurpose_DropDown(object sender, EventArgs e)
        {
            SetComboBoxQuoterPurpose(comboBoxTargetPurpose);
        }

        private void comboBoxLoadStyle_DropDown(object sender, EventArgs e)
        {
            SetComboBoxStyle(comboBoxLoadStyle);
        }

        private void comboBoxLoadPurpose_DropDown(object sender, EventArgs e)
        {
            SetComboBoxPurpose(comboBoxLoadPurpose);
        }

        private void SetComboBoxPurpose(ComboBox comboBox)
        {
            // 아직 초기화 전
            if (serverInfoManager.KrxdbInfoManager.isEmpty())
            {
                return;
            }

            SortedSet<string> purposeSet = new SortedSet<string>();

            for (int i = 0; i < iGridETF.Rows.Count; i++)
            {
                if (!iGridETF.Rows[i].Visible)
                    continue;

                string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                foreach (string purpose in serverInfoManager.KrxdbInfoManager.GetPurposeList(isinCode))
                {
                    purposeSet.Add(purpose);
                }
            }

            if (purposeSet.Count == 0)
            {
                comboBox.Items.Clear();
            }
            else
            {
                string selectedItem = null;

                if (comboBox.SelectedItem != null)
                    selectedItem = comboBox.SelectedItem.ToString();

                comboBox.Items.Clear();

                foreach (string purpose in purposeSet)
                    comboBox.Items.Add(purpose);

                if (selectedItem != null && purposeSet.Contains(selectedItem))
                {
                    comboBox.SelectedItem = selectedItem;
                }
                else
                {
                    comboBox.SelectedIndex = 0;
                }

                comboBox.DropDownWidth = UIUtil.DropDownWidth(comboBox);
            }
        }

        private void SetComboBoxStyle(ComboBox comboBox)
        {
            // 아직 초기화 전
            if (serverInfoManager.KrxdbInfoManager.isEmpty())
            {
                return;
            }

            SortedSet<string> styleSet = new SortedSet<string>();

            for (int i = 0; i < iGridETF.Rows.Count; i++)
            {
                if (!iGridETF.Rows[i].Visible)
                    continue;

                string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                foreach (string purpose in serverInfoManager.KrxdbInfoManager.GetPurposeList(isinCode))
                {
                    foreach (string style in serverInfoManager.KrxdbInfoManager.GetStyleList(isinCode, purpose))
                    {
                        styleSet.Add(style);
                    }
                }
            }

            if (styleSet.Count == 0)
            {
                comboBox.Items.Clear();
            }
            else
            {
                string selectedItem = null;

                if (comboBox.SelectedItem != null)
                    selectedItem = comboBox.SelectedItem.ToString();

                comboBox.Items.Clear();

                foreach (string style in styleSet)
                    comboBox.Items.Add(style);

                if (selectedItem != null && styleSet.Contains(selectedItem))
                {
                    comboBox.SelectedItem = selectedItem;
                }
                else
                {
                    comboBox.SelectedIndex = 0;
                }

                comboBox.DropDownWidth = UIUtil.DropDownWidth(comboBox);
            }
        }

        private void SetComboBoxQuoterPurpose(ComboBox comboBox)
        {
            SortedSet<string> purposeSet = new SortedSet<string>();

            for (int i = 0; i < iGridETF.Rows.Count; i++)
            {
                if (!iGridETF.Rows[i].Visible)
                    continue;

                string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                foreach (string purpose in serverInfoManager.GetQuoterPurposeList(isinCode))
                {
                    purposeSet.Add(purpose);
                }
            }

            if (purposeSet.Count == 0)
            {
                comboBox.Items.Clear();
            }
            else
            {
                string selectedItem = null;

                if (comboBox.SelectedItem != null)
                    selectedItem = comboBox.SelectedItem.ToString();

                comboBox.Items.Clear();

                foreach (string purpose in purposeSet)
                    comboBox.Items.Add(purpose);

                if (selectedItem != null && purposeSet.Contains(selectedItem))
                {
                    comboBox.SelectedItem = selectedItem;
                }
                else
                {
                    comboBox.SelectedIndex = 0;
                }

                comboBox.DropDownWidth = UIUtil.DropDownWidth(comboBox);
            }
        }

        private void buttonLoadPurpose_Click(object sender, EventArgs e)
        {
            string loadPurpose = comboBoxLoadPurpose.Text;
            string style = comboBoxLoadStyle.Text;

            if (style.Equals("") || loadPurpose.Equals(""))
            {
                MessageBox.Show("Purpose나 Style이 잘못되었습니다.", "오류");
                return;
            }

            if (!serverInfoManager.KrxdbInfoManager.isEmpty())
            {
                int counter = 0;

                // Sub purpose를 불러오는 경우 이미 Sub인 곳만 불러오기
                if (loadPurpose == Purpose.SUB.purpose)
                {
                    for (int i = 0; i < iGridETF.Rows.Count; i++)
                    {
                        if (!iGridETF.Rows[i].Visible)
                            continue;

                        string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                        string purpose = iGridETF.Rows[i].Cells["Purpose"].Value.ToString();

                        if (purpose != Purpose.SUB.purpose)
                            continue;

                        QuotingInfo info;
                        if (!serverInfoManager.KrxdbInfoManager.GetQuotingInfo(isinCode, loadPurpose, style, out info))
                            continue;

                        serverInfoManager.UpdateQuotingInfo(isinCode, loadPurpose, info);
                        counter++;
                    }
                }
                // 나머지 Purpose를 불러오는 경우 Sub로 지정된 곳은 Skip하기
                else
                {
                    for (int i = 0; i < iGridETF.Rows.Count; i++)
                    {
                        if (!iGridETF.Rows[i].Visible)
                            continue;

                        string isinCode = iGridETF.Rows[i].Cells["종목코드"].Value.ToString();
                        string purpose = iGridETF.Rows[i].Cells["Purpose"].Value.ToString();

                        if (purpose == Purpose.SUB.purpose)
                            continue;

                        QuotingInfo info;
                        if (!serverInfoManager.KrxdbInfoManager.GetQuotingInfo(isinCode, loadPurpose, style, out info))
                            continue;

                        serverInfoManager.UpdateQuotingInfo(isinCode, loadPurpose, info);
                        counter++;
                    }
                }

                MessageBox.Show(string.Format("{0} - {1} : {2}개의 QuotingInfo를 QuoterDic에 불러왔습니다.", loadPurpose, style, counter), "알림");
            }
            else
            {
                MessageBox.Show("quotingPurposeDic이 비어있습니다.", "오류");
            }
        }

        private bool checkAndShootInfo(QuotingInfo info)
        {
            if (!checkAccountValidity(info.account, info.isinCode))
                return false;

            if (!IsQuotingInfoValid(info))
                return false;

            JToken token = JToken.FromObject(info);
            String packet = token.ToString();
            if (serverInfoManager.clientConn == null)
            {
                MessageBox.Show("서버에 연결되지 않았습니다");
                return false;
            }
            else
            {
                serverInfoManager.clientConn.writeWithLength(packet);
                return true;
            }
        }

        private bool IsQuotingInfoValid(QuotingInfo q)
        {
            if (q.bookCode == null || q.bookCode.Trim() == "")
            {
                return false;
            }
            return true;
        }

        private void buttonSaveServerQuoterDic_Click(object sender, EventArgs e)
        {
            serverInfoManager.SaveServerQuoterDic(iGridETF);
        }

        private void buttonChangeViewPurpose_Click(object sender, EventArgs e)
        {
            string purpose = comboBoxTargetPurpose.Text;

            if (purpose == "")
            {
                MessageBox.Show("purpose가 선택되지 않았습니다.");
                return;
            }

            defaultPurpose = purpose;
        }

        private void checkBoxFilterLPOnly_CheckedChanged(object sender, EventArgs e)
        {
            filterLPDutyOnly = checkBoxFilterLPOnly.Checked;
            ApplyFilter();
        }

        private void comboBoxETFGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterGroup = comboBoxETFGroup.Text;
            ApplyFilter();
        }

        private void iGridETF_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                quoteControl.ComboBoxSelectStyleNavigator(ArrowType.Up);
            }
            else if (e.KeyCode == Keys.D)
            {
                quoteControl.ComboBoxSelectStyleNavigator(ArrowType.Down);
            }
            else if (e.KeyCode == Keys.Q)
            {
                quoteControl.ComboBoxSelectStyleNavigator(ArrowType.Up, true);
            }
            else if (e.KeyCode == Keys.E)
            {
                quoteControl.ComboBoxSelectStyleNavigator(ArrowType.Down, true);
            }
            else if (e.KeyCode == Keys.Z)
            {
                var currentInfo = quoteControl.getQuotingInfo();
                QuotingInfo info;
                if (serverInfoManager.GetNextQuotingInfo(currentInfo.isinCode, currentInfo.purpose, false, true, out info))
                {
                    quoteControl.setQuotingInfo(info);
                    quoteControl.refreshSelectPurpose();
                    quoteControl.setComboboxStyle(info.style);
                    dgv_Updater(null, null);
                }
            }
            else if (e.KeyCode == Keys.C)
            {
                var currentInfo = quoteControl.getQuotingInfo();
                QuotingInfo info;
                if (serverInfoManager.GetNextQuotingInfo(currentInfo.isinCode, currentInfo.purpose, true, true, out info))
                {
                    quoteControl.setQuotingInfo(info);
                    quoteControl.refreshSelectPurpose();
                    quoteControl.setComboboxStyle(info.style);
                    dgv_Updater(null, null);
                }
            }
        }

        private ETFQuoterInitData GetInitData()
        {
            string group = comboBoxETFGroup.Text;
            bool lpOnly = checkBoxFilterLPOnly.Checked;
            var widthDict = new Dictionary<string, int>();
            string serverId = this.serverId;

            foreach (iGCol col in iGridETF.Cols)
                widthDict.Add(col.Key, col.Width);

            return new ETFQuoterInitData(group, lpOnly, widthDict, serverId);
        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "|" + JsonConvert.SerializeObject(GetInitData());
        }

        private void isWO_CheckedChanged(object sender, EventArgs e)
        {
            dgv_Updater(null, null);
        }

        private void buttonEtfSiseViewOption_Click(object sender, EventArgs e)
        {
            if (!(sender is Button eventButton))
                return;

            string text = eventButton.Text.ToString();
            if (text == etfSiseViewMode)
                return;

            foreach (var control in etfSiseTogglePanel.Controls)
            {
                if (!(control is Button button))
                    continue;

                if (button == eventButton)
                    button.BackColor = Color.LightSkyBlue;
                else
                    button.BackColor = Color.White;
            }

            etfSiseViewMode = text;
            dgv_Updater(null, null);
        }

        public static IDockContent createFromJson(string json)
        {
            ETFQuoterInitData data = JsonConvert.DeserializeObject<ETFQuoterInitData>(json);
            var q = new ETFQuoter(data.serverId);
            q.group = data.group;

            //int idx;

            //idx = q.comboBoxETFGroup.Items.IndexOf(data.group);
            //if (idx >= 0)
            //    q.comboBoxETFGroup.SelectedIndex = idx;

            q.checkBoxFilterLPOnly.Checked = data.lpOnly;

            if (data.widthDict != null)
            {
                var cols = q.iGridETF.Cols;
                foreach (var key in data.widthDict.Keys)
                {
                    if (cols.KeyExists(key))
                        cols[key].Width = data.widthDict[key];
                }
            }
            return q;
        }

        private void LoadQuotingInfoHistoryViewer(QuotingInfo info)
        {
            if (info == null)
            {
                MessageBox.Show("종목이 선택되지 않았습니다.");
                return;
            }
            QuotingInfoHistoryViewer f = new QuotingInfoHistoryViewer(serverId, info);
            f.FormClosing += new FormClosingEventHandler(LoadQuotingInfo);
            f.Show();
        }

        private void quoteControl_HistoryButtonClick(object sender, QuoteEventArgs<QuotingInfo> e)
        {
            QuotingInfo info = e.GetQuotingInfo();
            LoadQuotingInfoHistoryViewer(info);
        }

        private void LoadQuotingInfo(object sender, FormClosingEventArgs e)
        {
            QuotingInfoHistoryViewer f = sender as QuotingInfoHistoryViewer;
            if (f != null)
            {
                QuotingInfo info = f.GetQuotingInfo();
                quoteControl.setQuotingInfo(info);
                quoteControl.refreshSelectPurpose();
                quoteControl.setComboboxStyle(info.style);
            }
        }

    }
}