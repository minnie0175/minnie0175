using Ciri.Interface;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TenTec.Windows.iGridLib;
using WeifenLuo.WinFormsUI.Docking;

namespace Ciri.Forms
{
    public partial class IndexFuturesQuoter : DockContent, IObserver<LPItemStatusSnapshot>, IObserver<ItemSelectNotifyInfo>, IQuoterForm
    {
        public class IndexFuturesQuoterInitData
        {
            public string serverId;
            public string category;
            public string style = null;
            public HashSet<string> uidPurposeSet;
            public Dictionary<string, int> widthDict;
            public bool spreadMode = false;

            public IndexFuturesQuoterInitData(string serverId, string category, string style, HashSet<string> uidPurposeSet, Dictionary<string, int> widthDict, bool spreadMode)
            {
                this.serverId = serverId;
                this.category = category;
                this.style = style;
                this.uidPurposeSet = uidPurposeSet;
                this.widthDict = widthDict;
                this.spreadMode = spreadMode;
            }
        }

        IDisposable lpDutyManagerUnsubscriber, itemSelectNotifierUnsubscriber;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        HashSet<string> dutySet;
        HashSet<string> uidPurposeSet;

        string prevIsinCode;
        string prevPurpose;
        string category;
        string style;
        bool spreadMode;

        bool hasWidthDic = false;
        public string serverId { get; private set; }
        ServerInfoManager serverInfoManager;
        QuoterInitiator quoterInitiator;

        private iGProgressBarCellManager m_PBCMgr = new iGProgressBarCellManager();

        private static Dictionary<UnderlyingID, int> tickByUIDDic = new Dictionary<UnderlyingID, int> {
            {UnderlyingID.K2I, 5},
            {UnderlyingID.MKI, 2},
            {UnderlyingID.KQI, 10},
            {UnderlyingID.QGI, 10},
            {UnderlyingID.EST, 100},
            {UnderlyingID.VKI, 5},
            {UnderlyingID.XA0, 20},
            {UnderlyingID.XA1, 20},
            {UnderlyingID.XA2, 20},
            {UnderlyingID.XA3, 20},
            {UnderlyingID.XA4, 50},
            {UnderlyingID.XA5, 50},
            {UnderlyingID.XA6, 20},
            {UnderlyingID.XA7, 20},
            {UnderlyingID.XA8, 20},
            {UnderlyingID.XA9, 20},
            {UnderlyingID.XAA, 20},
            {UnderlyingID.XAB, 20},
            {UnderlyingID.XAC, 50},
            {UnderlyingID.XAD, 50},
            {UnderlyingID.XAE, 50},
            {UnderlyingID.XI3, 20},
            {UnderlyingID.BM3, 10},
            {UnderlyingID.BMA, 10},
            {UnderlyingID.USD, 100},
        };


        int onUpdate = 0;

        ConcurrentDictionary<string, LPItemStatusSnapshot> lpStatusDic = new ConcurrentDictionary<string, LPItemStatusSnapshot>();
        string[] lpDutyColorColumnList = { "만족", "종목명" };

        private static readonly TimeSpan warningSpan = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan dangerSpan = TimeSpan.FromMinutes(20);

        public IndexFuturesQuoter(string serverId, string category = "없음", string style = null, HashSet<string> uidPurposeSet = null, bool spreadMode = false)
        {
            this.serverId = serverId;
            this.category = category;
            this.style = style;
            this.uidPurposeSet = uidPurposeSet;
            this.spreadMode = spreadMode;

            InitializeComponent();
            IGridUtil.UnlockIGrid(iGridIndexFutures);

            string prefix = "IF";
            if (spreadMode)
                prefix = "IF_SP";

            if (category != "없음")
                this.Text = prefix + " (" + category + ")";
            this.Text += "/" + serverId;

            DateTime dateTime = DateTime.Now;
            int nearMonth = DateTimeUtil.GetNearbyMonth(dateTime);

            indexFuturesGridView.initLater();
            indexSiseGridView.initLater();

            //ToggleSFArbColumns(false);

            SetSelectStyle(style);

            QuotingProduct pType;
            if (spreadMode)
                pType = QuotingProduct.SPREAD;
            else
                pType = QuotingProduct.IF;

            quoteControl.init(pType, "", true);
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

            if (spreadMode)
                InitIndexFuturesSpreadTable(uidPurposeSet);
            else
                InitIndexFuturesTable(uidPurposeSet);

            SelectIndexFutures(iGridIndexFutures.Rows[0].Cells["종목코드"].Text, "L");

        }

        private void InitIndexFuturesSpreadTable(HashSet<string> uidPurposeSet = null)
        {
            // 의무 확인할 필요 없이 uidPurposeSet에 해당하는 종목 중 스프레드가 있으면 모두 표시
            var AllDerivsList = DBUtil.Instance.GetDerivativesItemList();
            dutySet = new HashSet<string>();
            var notDutySet = new HashSet<string>();

            Dictionary<UnderlyingID, HashSet<string>> uidIsinCodeDic = new Dictionary<UnderlyingID, HashSet<string>>();

            // 의무인 선물 정보 가져오기
            foreach (var derivs in AllDerivsList)
            {
                FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(derivs.isinCode);
                if (fi == null)
                    continue;

                var uid = (UnderlyingID)fi.underlyingID;
                if (uid == UnderlyingID.UNKNOWN)
                    continue;

                if (fi.prodType != ProdTypeEnum.IndexFuturesSpread &&
                    fi.prodType != ProdTypeEnum.BondFuturesSpread &&
                    fi.prodType != ProdTypeEnum.CommodityFuturesSpread &&
                    fi.prodType != ProdTypeEnum.CurrencyFuturesSpread &&
                    fi.prodType != ProdTypeEnum.ForeignIndexFuturesSpread)
                    continue;

                // 만기가 먼 선물과의 스프레드는 필요없으므로 근월물 - 다음 월물 선물 스프레드만 표시한다.
                FuturesInstrument nextFi = ItemMaster.Instance.GetFuturesInstrumentByOrderWithUID(fi.underlyingID, 1);
                if (nextFi == null)
                    continue;

                if (nextFi.isinCode != fi.nextSpreadIsinCode)
                    continue;

                HashSet<string> isinCodeSet;
                if (!uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                {
                    isinCodeSet = new HashSet<string>();
                    uidIsinCodeDic[uid] = isinCodeSet;
                }

                isinCodeSet.Add(fi.isinCode);
            }

            if (uidPurposeSet == null)
            {
                // QuotingInfo 보고 목록 추가
                foreach (var isinCodeSet in uidIsinCodeDic.Values)
                {
                    foreach (var isinCode in isinCodeSet)
                    {
                        // 일단 추가할 종목들의 LP는 전부 추가한다
                        var fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(isinCode);
                        AddRow(iGridIndexFutures, fi);

                        foreach (var purpose in serverInfoManager.KrxdbInfoManager.GetPurposeList(fi.isinCode))
                        {
                            if (purpose == "L")
                                continue;

                            AddRow(iGridIndexFutures, fi, purpose);
                        }
                    }
                }
            }
            else
            {
                foreach (var data in uidPurposeSet)
                {
                    string[] split = data.Split('_');

                    if (split.Length != 2)
                    {
                        Console.WriteLine("uidPurposeSet Parsing Error : " + data);
                        continue;
                    }

                    string uidString = split[0];
                    string purpose = split[1];

                    var uid = (UnderlyingID)uidString;
                    if (uid == UnderlyingID.UNKNOWN)
                    {
                        Console.WriteLine("UID Parsing Error : " + uidString);
                        continue;
                    }

                    HashSet<string> isinCodeSet;

                    if (uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                    {
                        foreach (string isinCode in isinCodeSet)
                        {
                            FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(isinCode);
                            if (fi == null)
                                continue;

                            AddRow(iGridIndexFutures, fi, purpose);
                        }
                    }
                }
            }

            //iGridIndexFutures.Cols["종목"].AutoWidth();
            //iGridIndexFutures.Cols["종목코드"].AutoWidth();
            if (!hasWidthDic)
                iGridIndexFutures.Cols["종목코드"].AutoWidth();

            iGridIndexFutures.SortObject.Clear();
            iGridIndexFutures.SortObject.Add("의무", iGSortOrder.Descending);
            iGridIndexFutures.SortObject.Add("종목", iGSortOrder.Ascending);
            iGridIndexFutures.Sort();
        }

        private void InitIndexFuturesTable(HashSet<string> uidPurposeSet = null)
        {
            var AllDerivsList = DBUtil.Instance.GetDerivativesItemList();
            dutySet = new HashSet<string>();
            var notDutySet = new HashSet<string>();

            Dictionary<UnderlyingID, HashSet<string>> uidIsinCodeDic = new Dictionary<UnderlyingID, HashSet<string>>();
            bool dutyWeek = false;

            // 의무인 선물 정보 가져오기
            foreach (var derivs in AllDerivsList)
            {
                FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(derivs.isinCode);
                if (fi == null)
                    continue;

                var uid = (UnderlyingID)fi.underlyingID;
                if (uid == UnderlyingID.UNKNOWN)
                    continue;

                if (fi.prodType == ProdTypeEnum.StockFutureSpread || fi.prodType == ProdTypeEnum.IndexFuturesSpread || fi.prodType == ProdTypeEnum.StockFutures)
                    continue;

                if (fi.prodType == ProdTypeEnum.BondFutures || fi.prodType == ProdTypeEnum.CommodityFutures || fi.prodType == ProdTypeEnum.CurrencyFutures)
                {
                    var nextFi = ItemMaster.Instance.GetFuturesInstrumentByOrderWithUID(derivs.underlyingID, 1);
                    if (nextFi != null && nextFi.isinCode != fi.isinCode)
                    {
                        HashSet<string> isinCodeSet;
                        if (!uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                        {
                            isinCodeSet = new HashSet<string>();
                            uidIsinCodeDic[uid] = isinCodeSet;
                        }

                        isinCodeSet.Add(nextFi.isinCode);
                    }
                }

                if (LPDutyManager.Instance.IsCurrentLPItem(fi.isinCode))
                {
                    dutySet.Add(fi.isinCode);

                    HashSet<string> isinCodeSet;
                    if (!uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                    {
                        isinCodeSet = new HashSet<string>();
                        uidIsinCodeDic[uid] = isinCodeSet;
                    }

                    isinCodeSet.Add(fi.isinCode);
                }
                else
                {
                    var recentFi = ItemMaster.Instance.GetFuturesInstrumentByOrderWithUID(derivs.underlyingID, 0);
                    if (recentFi != null)
                    {
                        HashSet<string> isinCodeSet;
                        if (!uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                        {
                            isinCodeSet = new HashSet<string>();
                            uidIsinCodeDic[uid] = isinCodeSet;
                        }

                        isinCodeSet.Add(recentFi.isinCode);
                        notDutySet.Add(recentFi.isinCode);
                    }
                }
            }

            // 의무 이전 월물이 있으면 가져오기
            foreach (var isinCode in dutySet)
            {
                FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(isinCode);
                string uidString = fi.underlyingID;

                FuturesInstrument prevFi = ItemMaster.Instance.GetFuturesInstrumentByOrderWithUID(uidString, 0);
                var uid = (UnderlyingID)uidString;

                if (uid == UnderlyingID.UNKNOWN)
                    continue;

                if (!dutySet.Contains(prevFi.isinCode))
                {
                    HashSet<string> isinCodeSet;
                    if (!uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                    {
                        isinCodeSet = new HashSet<string>();
                        uidIsinCodeDic[uid] = isinCodeSet;
                    }

                    isinCodeSet.Add(prevFi.isinCode);
                    dutyWeek = true;
                }
            }

            // 만기 4일 이내이면 차월물 추가

            foreach (var isinCode in notDutySet)
            {
                FuturesInstrument recentFi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(isinCode);
                if ((recentFi.만기 - DateTime.Today).TotalDays > 4)
                {
                    continue;
                }

                string uidString = recentFi.underlyingID;
                FuturesInstrument nextFi = ItemMaster.Instance.GetFuturesInstrumentByOrderWithUID(uidString, 1);

                var uid = (UnderlyingID)uidString;

                if (uid == UnderlyingID.UNKNOWN)
                    continue;

                if (nextFi != null)
                {
                    HashSet<string> isinCodeSet;
                    if (!uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                    {
                        isinCodeSet = new HashSet<string>();
                        uidIsinCodeDic[uid] = isinCodeSet;
                    }

                    isinCodeSet.Add(nextFi.isinCode);
                }
            }

            if (uidPurposeSet == null)
            {
                // QuotingInfo 보고 목록 추가
                foreach (var isinCodeSet in uidIsinCodeDic.Values)
                {
                    foreach (var isinCode in isinCodeSet)
                    {
                        // 일단 추가할 종목들의 LP는 전부 추가한다
                        var fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(isinCode);
                        AddRow(iGridIndexFutures, fi);

                        foreach (var purpose in serverInfoManager.KrxdbInfoManager.GetPurposeList(fi.isinCode))
                        {
                            if (purpose == "L")
                                continue;

                            AddRow(iGridIndexFutures, fi, purpose);
                        }
                    }
                }
            }
            else
            {
                foreach (var data in uidPurposeSet)
                {
                    string[] split = data.Split('_');

                    if (split.Length != 2)
                    {
                        Console.WriteLine("uidPurposeSet Parsing Error : " + data);
                        continue;
                    }

                    string uidString = split[0];
                    string purpose = split[1];

                    var uid = (UnderlyingID)uidString;
                    if (uid == UnderlyingID.UNKNOWN)
                    {
                        Console.WriteLine("UID Parsing Error : " + uidString);
                        continue;
                    }

                    HashSet<string> isinCodeSet;

                    if (uidIsinCodeDic.TryGetValue(uid, out isinCodeSet))
                    {
                        foreach (string isinCode in isinCodeSet)
                        {
                            FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(isinCode);
                            if (fi == null)
                                continue;

                            AddRow(iGridIndexFutures, fi, purpose);
                        }
                    }
                }
            }

            //iGridIndexFutures.Cols["종목"].AutoWidth();
            //iGridIndexFutures.Cols["종목코드"].AutoWidth();
            if (!hasWidthDic)
                iGridIndexFutures.Cols["종목코드"].AutoWidth();

            iGridIndexFutures.SortObject.Clear();
            iGridIndexFutures.SortObject.Add("의무", iGSortOrder.Descending);
            iGridIndexFutures.SortObject.Add("종목", iGSortOrder.Ascending);
            iGridIndexFutures.Sort();
        }

        private void AddRow(iGrid grid, FuturesInstrument fi, string purpose = "L")
        {
            bool isCurrentLP = dutySet.Contains(fi.isinCode);

            iGRow row = grid.Rows.Add();
            row.Cells["월물"].Value = fi.maturityYearMonth;
            row.Cells["종목"].Value = fi.prodName + "_" + purpose;
            row.Cells["종목명"].Value = fi.prodName;
            row.Cells["종목코드"].Value = fi.isinCode;
            row.Cells["Time"].Value = "";
            row.Cells["Purpose"].Value = purpose;
            row.Cells["Style"].Value = "";
            row.Cells["의무"].Value = isCurrentLP ? "True" : "False";
            row.Cells["만족"].Value = "";
            row.Cells["LP종목"].Value = isCurrentLP ? "LP" : "일반";
            row.Cells["주문수"].Value = "";
            row.Cells["매수체결"].Value = "";
            row.Cells["매도체결"].Value = "";
            row.Cells["D.Ask"].Value = "";
            row.Cells["D.Bid"].Value = "";
        }

        private void dgv_Updater(object sender, EventArgs eArgs)
        {
            if (0 != System.Threading.Interlocked.Exchange(ref onUpdate, 1))
                return;

            try
            {
                QuotingInfo info = quoteControl.getQuotingInfo();
                if (info == null)
                    return;

                if (!this.IsFloat && !(this.DockHandler.Pane != null && this.DockHandler.Pane.ActiveContent != null && this.DockHandler.Pane.ActiveContent.DockHandler == this.DockHandler))
                    return;
                string stockFUcode = info.isinCode;
                string stockIsinCode = info.codeToMonitor;

                Dictionary<string, LiveDerivBalanceDoc> balanceDic = new Dictionary<string, LiveDerivBalanceDoc>();
                foreach (var item in DBUtil.Instance.GetLiveDerivAccountBalance(null, null))
                {
                    balanceDic[item.isinCode + "_" + item.account] = item;
                }

                indexSiseGridView.UpdateMarketBidAsk();

                // WO Part
                QuotingInfo indexFuturesInfo, woInfo = null;
                serverInfoManager.GetQuotingInfo(info, out indexFuturesInfo);
                if (checkBoxAllWO.Checked)
                    woInfo = serverInfoManager.GetWOUpdatedQuotingInfo(indexFuturesInfo);

                if (checkBoxOnlyWO.Checked)
                    indexFuturesGridView.UpdateBidAskFromWO(indexFuturesInfo);
                else
                    indexFuturesGridView.UpdateMarketBidAsk();

                indexFuturesGridView.UpdateWithQuotingInfo(indexFuturesInfo, woInfo);


                iGridSortHelper helper = iGridSortHelper.CreateAndBeginUpdate(iGridIndexFutures);
                QuotingInfo quoteInfo;

                for (int i = 0; i < iGridIndexFutures.Rows.Count; ++i)
                {
                    iGRow row = iGridIndexFutures.Rows[i];
                    string indexFuturesCode = row.Cells["종목코드"].Value.ToString();
                    string purpose = row.Cells["Purpose"].Value.ToString();

                    quoteInfo = null;

                    if (serverInfoManager.GetQuotingInfo(indexFuturesCode, purpose, out quoteInfo))
                    {
                        long qty = 0;
                        LiveDerivBalanceDoc derivBalance;
                        if (balanceDic.TryGetValue(quoteInfo.isinCode + "_" + quoteInfo.account, out derivBalance))
                            qty = derivBalance.일반잔고_실제;
                        row.Cells["SFQ(F)"].Value = qty;

                        int initialSpread;
                        var key = serverInfoManager.GetQuotingInfoKey(quoteInfo);
                        serverInfoManager.initialSpreadDic.TryGetValue(key, out initialSpread);
                        if (serverInfoManager.initialSpreadDic.ContainsKey(key))
                        {
                            int currentSpread = ServerInfoManager.GetSpreadTicksFromQuotingInfo(quoteInfo);
                            if (currentSpread <= initialSpread)
                            {
                                row.Cells["종목코드"].BackColor = Color.White;
                            }
                            else
                            {
                                row.Cells["종목코드"].BackColor = Color.Orange;
                            }
                        }

                        if (quoteInfo.bookCode == null)
                        {
                            Console.WriteLine("bookCode null : " + JsonConvert.SerializeObject(quoteInfo));
                        }
                        if (quoteInfo.quotingType == "FILCAN" || quoteInfo.quotingType == "AMENDER")
                        {
                            if (quoteInfo.orderSubmitted >= quoteInfo.depth * 2)
                            {
                                row.Cells["주문수"].BackColor = Color.LightGreen;
                            }
                            else if (quoteInfo.orderSubmitted == 0)
                            {
                                row.Cells["주문수"].BackColor = Color.Red;
                            }
                            else
                            {
                                row.Cells["주문수"].BackColor = Color.LightYellow;
                            }
                        }
                        else
                        {
                            if (quoteInfo.orderSubmitted > 1)
                            {
                                row.Cells["주문수"].BackColor = Color.LightGreen;
                            }
                            else if (quoteInfo.orderSubmitted == 1)
                            {
                                row.Cells["주문수"].BackColor = Color.LightYellow;
                            }
                            else
                            {
                                row.Cells["주문수"].BackColor = Color.Red;
                            }
                        }
                        row.Cells["상태"].Value = quoteInfo.stateCode;
                        row.Cells["Purpose"].Value = quoteInfo.purpose;
                        row.Cells["Style"].Value = quoteInfo.style;
                        row.Cells["주문수"].Value = quoteInfo.orderSubmitted;
                        row.Cells["매수체결"].Value = quoteInfo.longContracted;
                        row.Cells["매도체결"].Value = quoteInfo.shortContracted;
                        row.Cells["Bid"].Value = quoteInfo.expectedBid;
                        row.Cells["Ask"].Value = quoteInfo.expectedAsk;
                        row.Cells["D.Ask"].Value = quoteInfo.decorationAskSkew;
                        row.Cells["D.Bid"].Value = quoteInfo.decorationBidSkew;

                        if (quoteInfo.stateCode == "OFF")
                        {
                            row.Cells["Time"].Value = "";
                            row.Cells["Time"].BackColor = Color.White;
                        }
                        else
                        {
                            var span = ContractTimeManager.Instance.GetTimeSpan(quoteInfo.bookCode, quoteInfo.isinCode);
                            row.Cells["Time"].Value = $"{(span.Hours * 60 + span.Minutes):00}:{span.Seconds:00}";
                            if (span >= dangerSpan)
                            {
                                row.Cells["Time"].BackColor = Color.Red;
                            }
                            else if (span >= warningSpan)
                            {
                                row.Cells["Time"].BackColor = Color.Orange;
                            }
                            else
                            {
                                row.Cells["Time"].BackColor = Color.White;
                            }
                        }

                        var futuresBA = MarketDataCenter.Instance.GetBidAskData(quoteInfo.isinCode);
                        var quoteFi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(quoteInfo.isinCode);
                        var uid = (UnderlyingID)quoteFi?.underlyingID;
                        if (uid != UnderlyingID.UNKNOWN)
                        {
                            int tick = 10;
                            if (tickByUIDDic.ContainsKey(uid))
                                tick = tickByUIDDic[uid];

                            int maxSkew = tick * quoteInfo.GetDecorationMainTick() / 100;
                            IGridUtil.ApplyColorForDecoration(row.Cells["D.Ask"], maxSkew);
                            IGridUtil.ApplyColorForDecoration(row.Cells["D.Bid"], maxSkew);
                        }
                    }

                    LPItemStatusSnapshot snapshot;
                    if (!lpStatusDic.TryGetValue(indexFuturesCode, out snapshot))
                        continue;

                    row.Cells["의무비율"].Value = snapshot.dutySatisfiedSpreadRatio;
                    IGridUtil.FillLPStatusColor(row, snapshot, "만족", lpDutyColorColumnList);
                }

                helper.EndUpdate();
                //Console.WriteLine("Finished Update UI in DGVUpdater");
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
        private void IndexFuturesQuoter_Load(object sender, EventArgs e)
        {
            lpDutyManagerUnsubscriber = LPDutyManager.Instance.Subscribe(this);
            itemSelectNotifierUnsubscriber = ItemSelectNotifier.Instance.Subscribe(this);
            m_PBCMgr.Attach(this.iGridIndexFutures, iGridIndexFutures.Cols["의무비율"]);
        }

        private void CancelQuote(string isinCode, string purpose)
        {
            QuotingInfo info;
            serverInfoManager.GetQuotingInfo(isinCode, purpose, out info);

            if (info == null)
                return;

            info.SetCommandQuoteStop(Control.ModifierKeys);

            checkAndShootInfo(info);
        }

        bool filterLPDutyOnly = false;

        private void ApplyFilter()
        {

            iGridSortHelper helper = iGridSortHelper.CreateAndBeginUpdate(iGridIndexFutures);
            try
            {
                for (int i = iGridIndexFutures.Rows.Count - 1; i >= 0; i--)
                {
                    bool visible = true;
                    string isinCode = iGridIndexFutures.Rows[i].Cells["종목코드"].Value.ToString();
                    if (filterLPDutyOnly && ((string)iGridIndexFutures.Rows[i].Cells["의무"].Value) == "False")
                        visible = visible && false;
                    else
                        visible = visible && true;

                    iGridIndexFutures.Rows[i].Visible = visible;
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

        #region LPItemStatus Observer 관련
        public void OnNext(LPItemStatusSnapshot snapshot)
        {
            lpStatusDic.AddOrUpdate(snapshot.ItemIsinCode, snapshot, (s1, s2) => snapshot);
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

        private void checkBoxFilterLPOnly_CheckedChanged(object sender, EventArgs e)
        {
            filterLPDutyOnly = checkBoxFilterLPOnly.Checked;
            ApplyFilter();
        }


        private void iGridIndexFutures_CellClick(object sender, iGCellClickEventArgs e)
        {
            string stockFuturesIsinCode = iGridIndexFutures.Cells[e.RowIndex, "종목코드"].Value.ToString();
            string purpose = iGridIndexFutures.Cells[e.RowIndex, "Purpose"].Value.ToString();
            // 미니 코스피 의무가 월물 2개이므로 월물을 강제로 고정시킬 수 없음

            FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(stockFuturesIsinCode);
            if (fi == null)
                return;

            SelectIndexFutures(fi.isinCode, purpose);
        }

        private void SelectIndexFutures(string isinCode, string purpose, bool forceUpdate = false)
        {
            FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(isinCode);
            if (fi == null)
                return;

            if (isinCode == prevIsinCode && purpose == prevPurpose && !forceUpdate)
                return;

            prevIsinCode = isinCode;
            prevPurpose = purpose;

            string underlyingIsinCode = fi.underlyingIsinCode;
            MarketDataCrawler.Instance.addCodeToMonitorBidAsk(isinCode);
            MarketDataCrawler.Instance.addCodeToMonitorBidAsk(underlyingIsinCode);

            stockFuturesName.Text = fi.prodName;

            indexFuturesGridView.setIsinCode(isinCode);
            indexSiseGridView.setIsinCode(underlyingIsinCode);

            QuotingInfo info;
            serverInfoManager.GetQuotingInfo(isinCode, purpose, out info);

            if (info == null)
            {
                info = new QuotingInfo();

                LPItemRule rule = LPDutyManager.Instance.GetLPItemRule(isinCode);
                // 안전성을 위해 QuotingInfo가 없는 경우에는 매매 수량 0으로 설정
                if (rule != null)
                {
                    int dutyAmount = rule.DutyAmount;
                    //info.amountLong = dutyAmount;
                    //info.amountShort = dutyAmount;
                }
                else
                {
                    //info.amountLong = 1;
                    //info.amountShort = 1;
                }

                info.amountLong = info.amountShort = 0;

                info.isinCode = isinCode;
                info.account = AccountMap.GetApptAccount(isinCode);
                info.codeToMonitor = underlyingIsinCode;
                info.purpose = purpose;
                info.style = "기본";

                info.skewUnit = 20;
                info.askSkew = info.skewUnit * 1;
                info.bidSkew = info.skewUnit * -1;

                info.longLimit = 50;
                info.shortLimit = -50;

                info.logic = "PlainFollow";

                info.weight = 0;
                info.weight2 = 0;
                info.amtParam = 0;

                if (dutySet.Contains(isinCode))
                    info.bookCode = BookMaster.Instance.GetHedgeBookCodeWithPrimaryIsinCode(underlyingIsinCode);
                else
                    info.bookCode = "";

                info.bullets = 1000;
                info.depth = 5;
            }

            quoteControl.setQuotingInfo(info);
            quoteControl.refreshSelectPurpose();
            quoteControl.setComboboxStyle(info.style);

            ItemSelectNotifier.Instance.SelectItem(isinCode, this.GetType().Name, new string[] { "DailyPositionForm", "ManualOrder" }, this, "");
        }

        private void IndexFuturesQuoter_FormClosing(object sender, FormClosingEventArgs e)
        {
            lpDutyManagerUnsubscriber.Dispose();
            itemSelectNotifierUnsubscriber.Dispose();
            timer.Stop();
            quoterInitiator.UnregisterQuoter();
        }

        private bool IsQuotingInfoValid(QuotingInfo q)
        {
            if (q.bookCode == null || q.bookCode.Trim() == "")
            {
                return false;
            }
            return true;
        }
        private void buttonSummitAll_Click(object sender, EventArgs e)
        {
            string purpose = comboBoxTargetPurpose.Text;

            for (int i = 0; i < iGridIndexFutures.Rows.Count; i++)
            {
                if (!iGridIndexFutures.Rows[i].Visible)
                    continue;

                string isinCode = iGridIndexFutures.Rows[i].Cells["종목코드"].Value.ToString();
                string rowPurpose = iGridIndexFutures.Rows[i].Cells["Purpose"].Value.ToString();

                QuotingInfo info;
                serverInfoManager.GetQuotingInfo(isinCode, rowPurpose, out info);

                // QuoterInfo
                if (info == null)
                {
                    continue;
                }
                if (info.bookCode == null || info.bookCode.Trim() == "")
                {
                    MessageBox.Show(string.Format("{0} {1} 종목의 quotingInfo에 북코드가 비어있습니다.", info.isinCode, ItemMaster.Instance.GetProdNameWithIsinCode(info.isinCode)));
                }

                info.SetTrCodeQuotingType();
                info.SetCommandQuoteStart();

                info.SetFitToMarket(Control.ModifierKeys);

                checkAndShootInfo(info);
            }

        }

        private void iGridStockFutures_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                QuotingInfo info = quoteControl.getQuotingInfo();
                if (info == null)
                    MessageBox.Show("QuotingInfo 셋팅 오류");
                else
                {
                    DialogResult dr = MessageBox.Show(string.Format("{0} 종목의 호가를 다시 제출하시겠습니까?", info.isinCode), "", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        checkAndShootInfo(info);
                    }
                }
            }
        }

        private void iGridStockFutures_CurRowChanged(object sender, EventArgs e)
        {
            string stockFuturesIsinCode = iGridIndexFutures.CurRow.Cells["종목코드"].Value.ToString();
            string purpose = iGridIndexFutures.CurRow.Cells["Purpose"].Value.ToString();
            if (stockFuturesIsinCode != "")
            {
                SelectIndexFutures(stockFuturesIsinCode, purpose);
            }
        }

        private void buttonCancelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < iGridIndexFutures.Rows.Count; i++)
            {
                if (!iGridIndexFutures.Rows[i].Visible)
                    continue;

                string isinCode = iGridIndexFutures.Rows[i].Cells["종목코드"].Value.ToString();
                if (isinCode == null)
                    continue;

                foreach (string purpose in serverInfoManager.GetQuoterPurposeList(isinCode))
                {
                    CancelQuote(isinCode, purpose);
                }
            }
        }

        private void IndexFuturesQuoter_Resize(object sender, EventArgs e)
        {
            // 230517 IndexFuturesQuoter Resize 중지
            //int height = this.Height;
            //iGridIndexFutures.Height = (height - 1100) + 805 + 28;
            //quoteControl.Location = new Point(quoteControl.Location.X, 858 + (height - 1100) + 28);
        }

        public void OnNext(ItemSelectNotifyInfo value)
        {
            // do nothing
        }
        private void quoteControl_StartButtonClick(object sender, QuoteEventArgs<QuotingInfo> e)
        {
            QuotingInfo info = e.GetQuotingInfo();
            checkAndShootInfo(info);
        }

        private void buttonLoadPurpose_Click(object sender, EventArgs e)
        {
            string style = comboBoxLoadStyle.Text;

            LoadStyle(style);
        }

        private void LoadStyle(string style, bool printMsg = true)
        {
            if (style.Equals(""))
            {
                if (printMsg)
                    MessageBox.Show("Style이 잘못되었습니다.", "오류");

                return;
            }

            if (!serverInfoManager.KrxdbInfoManager.isEmpty())
            {
                int counter = 0;

                for (int i = 0; i < iGridIndexFutures.Rows.Count; i++)
                {
                    if (!iGridIndexFutures.Rows[i].Visible)
                        continue;

                    string isinCode = iGridIndexFutures.Rows[i].Cells["종목코드"].Value.ToString();
                    string purpose = iGridIndexFutures.Rows[i].Cells["Purpose"].Value.ToString();

                    QuotingInfo info;
                    if (!serverInfoManager.KrxdbInfoManager.GetQuotingInfo(isinCode, purpose, style, out info))
                        continue;

                    serverInfoManager.UpdateQuotingInfo(isinCode, purpose, info);
                    counter++;
                }

                if (printMsg)
                    MessageBox.Show(string.Format("{0} : {1}개의 QuotingInfo를 QuoterDic에 불러왔습니다.", style, counter), "알림");
            }
            else
            {
                if (printMsg)
                    MessageBox.Show("quotingPurposeDic이 비어있습니다.", "오류");
            }
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

        private void SetComboBoxStyle(ComboBox comboBox)
        {
            // 아직 초기화 전
            if (serverInfoManager.KrxdbInfoManager.isEmpty())
            {
                return;
            }

            SortedSet<string> styleSet = new SortedSet<string>();

            for (int i = 0; i < iGridIndexFutures.Rows.Count; i++)
            {
                if (!iGridIndexFutures.Rows[i].Visible)
                    continue;

                string isinCode = iGridIndexFutures.Rows[i].Cells["종목코드"].Value.ToString();
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

            for (int i = 0; i < iGridIndexFutures.Rows.Count; i++)
            {
                if (!iGridIndexFutures.Rows[i].Visible)
                    continue;

                string isinCode = iGridIndexFutures.Rows[i].Cells["종목코드"].Value.ToString();
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

        private void iGridStockFutures_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
            {
                quoteControl.ComboBoxSelectStyleNavigator(ArrowType.Up);
            }
            else if (e.KeyCode == Keys.D)
            {
                quoteControl.ComboBoxSelectStyleNavigator(ArrowType.Down);
            }
        }

        private void buttonSaveServerQuoterDic_Click(object sender, EventArgs e)
        {
            serverInfoManager.SaveServerQuoterDic(iGridIndexFutures);
        }

        private bool checkAndShootInfo(QuotingInfo info)
        {
            if (!checkAccountValidity(info.account, info.isinCode))
                return false;

            if (!IsQuotingInfoValid(info))
            {
                string json = JsonConvert.SerializeObject(info);
                MessageBox.Show("QuotingInfo is not valid : " + json);
                return false;
            }

            serverInfoManager.SetInitialQuotingInfo(info);
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

        private void SetSelectStyle(string style)
        {
            if (style != null)
                LoadStyle(style, false);
        }

        private IndexFuturesQuoterInitData GetInitData()
        {
            string serverId = this.serverId;
            string category = this.category;
            string style = this.style;
            HashSet<string> uidPurposeSet = this.uidPurposeSet;
            var widthDict = new Dictionary<string, int>();
            bool spreadMode = this.spreadMode;

            foreach (iGCol col in iGridIndexFutures.Cols)
                widthDict.Add(col.Key, col.Width);

            return new IndexFuturesQuoterInitData(serverId, category, style, uidPurposeSet, widthDict, spreadMode);
        }

        protected override string GetPersistString()
        {
            return this.GetType().ToString() + "|" + JsonConvert.SerializeObject(GetInitData());
        }

        private void IndexFuturesQuoter_Activated(object sender, EventArgs e)
        {
            var info = quoteControl.getQuotingInfo();
            if (info == null)
                return;

            string isinCode = info.isinCode;
            string purpose = info.purpose;
            if (isinCode == "")
                return;

            SelectIndexFutures(isinCode, purpose, true);
        }

        private void quoteControl_HistoryButtonClick(object sender, QuoteEventArgs<QuotingInfo> e)
        {
            QuotingInfo info = e.GetQuotingInfo();
            LoadQuotingInfoHistoryViewer(info);
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

        public static IDockContent createFromJson(string json)
        {
            IndexFuturesQuoterInitData data = JsonConvert.DeserializeObject<IndexFuturesQuoterInitData>(json);
            var q = new IndexFuturesQuoter(data.serverId, data.category, data.style, data.uidPurposeSet, data.spreadMode);

            if (data.widthDict != null)
            {
                var cols = q.iGridIndexFutures.Cols;
                foreach (var key in data.widthDict.Keys)
                {
                    if (cols.KeyExists(key))
                        cols[key].Width = data.widthDict[key];
                }
                q.hasWidthDic = true;
            }
            return q;
        }
    }
}