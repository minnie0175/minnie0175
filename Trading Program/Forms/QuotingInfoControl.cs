using CiriData.Data;
using CiriData.Enums;
using CommonLib.Data;
using CommonLib.DBDataType;
using CommonLib.Enums;
using CommonLib.Forms;
using CommonLib.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TenTec.Windows.iGridLib;

namespace CiriData.Manage
{
    public partial class QuotingInfoControl : UserControl
    {
        public event StartClickHandler StartButtonClick;
        public delegate void StartClickHandler(object sender, QuoteEventArgs<QuotingInfo> e);
        public event CancelClickHandler CancelButtonClick;
        public delegate void CancelClickHandler(object sender, QuoteEventArgs<QuotingInfo> e);
        public event HistoryClickHandler HistoryButtonClick;
        public delegate void HistoryClickHandler(object sender, QuoteEventArgs<QuotingInfo> e);

        public event CiriAndOAQuotingInfoManagerGetter GetCiriAndOAInfoManager;
        public delegate CiriAndOAQuotingInfoManager CiriAndOAQuotingInfoManagerGetter();
        public event DBQuotingInfoManagerGetter GetDBInfoManager;
        public delegate KRXDBQuotingInfoManager DBQuotingInfoManagerGetter();

        public event CTMListGetter GetCTMList;
        public delegate SynchronizedCollection<string> CTMListGetter(string isinCode);

        public event CTMUpdateEvent CTMUpdate;
        public delegate void CTMUpdateEvent(string codeToMonitor);

        // Parent 로부터 받은 QuotingInfo, QuoteController 에서 수정중인 값이 아닌 ControlUpdater 에 있는 값( SetQuotingInfo 호출시 ref 변경 )
        // parent 에서 가져가는 값은 이 property 의 Get 이 아니라 GetQuotingInfo 를 통해(현재 콘트롤에 셋팅 된) 데이터를 가져가야한다.
        private QuotingInfo qi;

        //comboBoxSelectStyle 이벤트 핸들러의 작동 여부 변수
        private bool _noise = false;

        bool sendQuoteWhenAdjustSkew = true;
        bool remainSkew = false;

        bool IFCTMComboBoxMode = false;

        public List<DecorationParameterInfo> pInfoList;
        public string jsonString;

        int lastResolution = 1;

        private bool internalUpdate = false;

        #region multiModeVariables        
        bool multiMode = false;
        readonly HashSet<Type> controllableControlTypeSet = new HashSet<Type>
        {
            typeof(Button),
            typeof(ComboBox),
            typeof(TextBox),
            typeof(RadioButton),
            typeof(CheckBox),
            typeof(NumericUpDown)
        };

        HashSet<Control> multiModeAvailableControlSet = new HashSet<Control>();
        Dictionary<Control, bool> initStatusDic = new Dictionary<Control, bool>();
        List<QuotingInfo> selectedQuotingInfoList = new List<QuotingInfo>();
        readonly string NotChangedString = "그대로 유지";
        #endregion

        public QuotingInfoControl()
        {
            InitializeComponent();
            //init();

            numericAskSkew.Maximum = int.MaxValue;
            numericAskSkew.Minimum = int.MinValue;
            numericBidSkew.Maximum = int.MaxValue;
            numericBidSkew.Minimum = int.MinValue;

            numericHitRatio.Maximum = 100;
            numericHitRatio.Minimum = 0;
            numericHitRatio.Increment = 1;
        }

        public void init(QuotingProduct pType, string representingIsinCode, bool isLP)
        {
            initComboBoxFrom(pType);

            foreach (var info in BookMaster.Instance.GetAll())
            {
                comboBoxBook.Items.Add(new { dispName = info.bookName, value = info.bookCode });
                comboBoxBook.DisplayMember = "dispName";
                comboBoxBook.ValueMember = "value";
            }

            setAccount(pType);

            foreach (var comboBox in HelperUtil.GetAllControls(this, typeof(ComboBox)))
            {
                if (comboBox is ComboBox)
                {
                    var cb = comboBox as ComboBox;
                    cb.DropDownWidth = UIUtil.DropDownWidth(cb);
                }
            }

            if (pType == QuotingProduct.SO || pType == QuotingProduct.IO)
            {
                comboBoxSelectStyle.Enabled = false;
                buttonSaveStyle.Enabled = false;
                buttonSaveStyle.BackColor = Color.LightGray;
                buttonRemoveStyle.Enabled = false;
                buttonRemoveStyle.BackColor = Color.LightGray;
                textBoxCloneStyleName.Enabled = false;
                buttonCloneStyle.Enabled = false;
                buttonCloneStyle.BackColor = Color.LightGray;
                buttonApplyAndSave.Enabled = false;
                buttonApplyAndSave.BackColor = Color.LightGray;

                comboBoxPurpose.Enabled = false;
            }
            else if (pType == QuotingProduct.IF)
            {
                InitCodeToMonitorForIndexFutures();
            }

            foreach (Control control in GetControllableControls(this))
            {
                initStatusDic.Add(control, control.Enabled);
            }

            multiModeAvailableControlSet.Add(comboBoxQuotingType);
            multiModeAvailableControlSet.Add(comboBoxBook);
            multiModeAvailableControlSet.Add(comboBoxAccount);
            multiModeAvailableControlSet.Add(comboBoxAmtStrategy);
            multiModeAvailableControlSet.Add(comboBoxHitAuthority);
            multiModeAvailableControlSet.Add(comboBoxPriceLogic);
            multiModeAvailableControlSet.Add(comboBoxOPK);
            multiModeAvailableControlSet.Add(buttonApply);
            multiModeAvailableControlSet.Add(buttonCancel);
        }

        // return: true on setting success otherwise, false
        public bool setQuotingInfo(QuotingInfo info, bool remainSkew = false)
        {
            QuotingProduct pType = QuotingProductFunctions.GetTypeFrom(info.isinCode);
            if (pType != (QuotingProduct)comboBoxProdType.SelectedItem)
            {
                return false;
            }

            internalUpdate = true;

            this.qi = info;

            labelStyle.Text = info.style;
            textBoxCode.Text = info.isinCode;

            if (info.purpose != null)
                comboBoxPurpose.SelectedIndex = comboBoxPurpose.Items.IndexOf(info.purpose);
            else
                comboBoxPurpose.SelectedIndex = 0;


            if (!IFCTMComboBoxMode)
                comboBoxCodeToMonitor.Items.Clear();

            UpdateCodeToMonitor(info.isinCode, info.codeToMonitor);

            bool isLP = true;
            Boolean.TryParse(info.isLp, out isLP);

            setAccount(pType);

            comboBoxAccount.SelectedIndex = -1;
            for (int itemIdx = 0; itemIdx < comboBoxAccount.Items.Count; ++itemIdx)
                if (((AccountMasterData)((ComboBoxItem)comboBoxAccount.Items[itemIdx]).Value).accountNumber == info.account)
                    comboBoxAccount.SelectedIndex = itemIdx;

            if (info.hitAuthority != null)
                comboBoxHitAuthority.SelectedIndex = comboBoxHitAuthority.Items.IndexOf(info.hitAuthority);
            else
                comboBoxHitAuthority.SelectedIndex = 0;

            if (info.quotingType != null)
                comboBoxQuotingType.SelectedIndex = comboBoxQuotingType.Items.IndexOf(info.quotingType);
            else
                comboBoxQuotingType.SelectedIndex = 0;

            if (info.logic != null)
                comboBoxPriceLogic.SelectedIndex = comboBoxPriceLogic.Items.IndexOf(info.logic);
            else
                comboBoxPriceLogic.SelectedIndex = 0;

            if (info.amtStrategy != null)
                comboBoxAmtStrategy.SelectedIndex = comboBoxAmtStrategy.Items.IndexOf(info.amtStrategy);
            else
                comboBoxAmtStrategy.SelectedIndex = 0;

            if (info.skewStrategy != null)
                comboBoxSkewStrategy.SelectedIndex = comboBoxSkewStrategy.Items.IndexOf(info.skewStrategy);
            else
                comboBoxSkewStrategy.SelectedIndex = 0;

            if (info.opk != null)
                comboBoxOPK.SelectedIndex = comboBoxOPK.Items.IndexOf(info.opk);
            else
                comboBoxOPK.SelectedIndex = 0;

            if (info.hittingStyle != null)
                comboBoxHittingStyle.SelectedIndex = comboBoxHittingStyle.Items.IndexOf(info.hittingStyle);
            else
                comboBoxHittingStyle.SelectedIndex = 0;

            if (info.quotingStyle != null)
                comboBoxQuotingStyle.SelectedIndex = comboBoxQuotingStyle.Items.IndexOf(info.quotingStyle);
            else
                comboBoxQuotingStyle.SelectedIndex = 0;

            if (info.biasedQuotingStyle != null)
                comboBoxBiasedQuotingStyle.SelectedIndex = comboBoxBiasedQuotingStyle.Items.IndexOf(info.biasedQuotingStyle);
            else
                comboBoxBiasedQuotingStyle.SelectedIndex = 0;

            numericOPKTick.Value = info.opkTick;
            numericOPKAmtDenominator.Value = info.opkAmtDenominator;
            numericOPKReduceTill.Value = info.opkReduceTill;

            checkBoxOPKTodayOnly.Checked = info.opkTodayOnly;
            checkBoxOPKHitAllowed.Checked = info.opkHitAllowed;
            checkBoxOPKShortOnly.Checked = info.opkShortOnly;


            numericAmountLong.Value = info.amountLong;
            numericAmountShort.Value = info.amountShort;
            numericAmountIOCLong.Value = info.amountLongHit;
            numericAmountIOCShort.Value = info.amountShortHit;

            numericUpDownSkewUnit.Value = info.skewUnit;


            // Skew Part

            numericAskSkew.Increment = info.skewUnit;
            numericBidSkew.Increment = info.skewUnit;


            if (!remainSkew)
            {
                numericAskSkew.Value = info.askSkew;
                numericBidSkew.Value = info.bidSkew;

                checkBoxAutoSkew.Checked = info.autoDutySkew;
                checkBoxAggressiveSkew.Checked = info.useAggressiveDuty;

                if (info.hitAuthority != null)
                    comboBoxHitAuthority.SelectedIndex = comboBoxHitAuthority.Items.IndexOf(info.hitAuthority);
                else
                    comboBoxHitAuthority.SelectedIndex = 0;

                // Decoration Part

                pInfoList = info.pInfoList;
                if (pInfoList == null)
                    pInfoList = new List<DecorationParameterInfo>();

                RefreshDecoratorView();

                //labeldecorationAskSkew.Text = info.decorationAskSkew.ToString();
                //labeldecorationBidSkew.Text = info.decorationBidSkew.ToString();
            }

            numericLongLimit.Value = info.longLimit;
            numericShortLimit.Value = info.shortLimit;

            numericUpDownFilCanBullet.Value = info.bullets;
            numericUpDownFilCanDepth.Value = info.depth;

            numericHitRatio.Value = info.hitRatio;

            textWeight.Text = info.weight.ToString();
            textWeight2.Text = info.weight2.ToString();
            textBoxAmtParam.Text = info.amtParam.ToString();

            if (info.bullets > 0)
                numericUpDownFilCanBullet.Value = info.bullets;
            if (info.depth > 0)
                numericUpDownFilCanDepth.Value = info.depth;

            setComboBoxString(comboBoxBook, info.bookCode);

            if (pType == QuotingProduct.SF)
            {
                BidAskData ba = MarketDataCenter.Instance.GetBidAskData(info.codeToMonitor);
                if (ba == null)
                {
                    internalUpdate = false;
                    return true;
                }

                int price = ba.bidPrice[0];

                TickCalculator stockFuturesTickCalculator = new StockTickCalculator();
                double tickSize = stockFuturesTickCalculator.AddTick(price, 1) - price;

                int bidBuffer = (int)(info.bidSkew / tickSize);
                int askBuffer = (int)(info.askSkew / tickSize);
                updateSFExpectedValues(info.codeToMonitor, info.isinCode, bidBuffer, askBuffer);
            }

            if (info.skewTriggerAmt == 0)
            {
                numericUpDownTriggerAmt.Value = Math.Min(info.amountShort, info.amountLong);
            }
            else
            {
                numericUpDownTriggerAmt.Value = info.skewTriggerAmt;
            }

            textBoxPricingLogicParam.Text = info.pricingLogicParam;
            checkBoxDecoratedCTM.Checked = info.decoratedCTM;
            jsonString = info.jsonString;
            ApplyJsonStringCount();

            comboBoxQII.Text = info.qii;
            //refreshSelectPurpose();

            int resolutionIdx = comboBoxResolution.Items.IndexOf(info.resolution.ToString());
            if (resolutionIdx < 0)
                resolutionIdx = 0;

            comboBoxResolution.SelectedIndex = resolutionIdx;

            internalUpdate = false;

            return true;
        }


        public QuotingInfo getQuotingInfo()
        {
            QuotingInfo info = new QuotingInfo();
            if (labelStyle.Text.Equals(""))
                info.style = "기본";
            else
                info.style = labelStyle.Text;

            if (comboBoxPurpose.Text.Equals(""))
                info.purpose = "L";
            else
                info.purpose = comboBoxPurpose.Text;

            info.isinCode = textBoxCode.Text;
            info.SetAmountStrategy(comboBoxAmtStrategy.SelectedItem?.ToString());
            info.SetQuotingInfoInterpreter(comboBoxQII.SelectedItem?.ToString());
            info.amountLong = (int)numericAmountLong.Value;
            info.amountShort = (int)numericAmountShort.Value;
            info.amountLongHit = (int)numericAmountIOCLong.Value;
            info.amountShortHit = (int)numericAmountIOCShort.Value;

            info.amtParam = Int32.Parse(textBoxAmtParam.Text);

            info.SetHittingStyle(comboBoxHittingStyle.SelectedItem?.ToString());
            info.SetQuotingStyle(comboBoxQuotingStyle.SelectedItem?.ToString());
            info.SetBiasedQuotingStyle(comboBoxBiasedQuotingStyle.SelectedItem?.ToString());

            info.depth = 1;
            info.longLimit = (int)numericLongLimit.Value;
            info.shortLimit = (int)numericShortLimit.Value;
            info.codeToMonitor = getComboBoxString(comboBoxCodeToMonitor);
            info.bookCode = getComboBoxString(comboBoxBook);

            info.askSkew = (int)numericAskSkew.Value;
            info.bidSkew = (int)numericBidSkew.Value;

            // skew 전략에 따라 ask/bid 초기값 변경이 있을수 있으므로 bid/ask 값 설정 이후에 설정한다.
            info.SetSkewStrategy(comboBoxSkewStrategy.SelectedItem?.ToString());
            info.skewUnit = (int)numericUpDownSkewUnit.Value;
            info.skewTriggerAmt = (int)numericUpDownTriggerAmt.Value;

            if (comboBoxPriceLogic.SelectedIndex < 0)
                return null;
            info.logic = (String)comboBoxPriceLogic.Items[comboBoxPriceLogic.SelectedIndex];

            if (comboBoxAccount.SelectedIndex < 0)
                return null;
            info.account = ((AccountMasterData)((ComboBoxItem)comboBoxAccount.SelectedItem).Value).accountNumber;

            info.isLp = StaticValues.LP;
            info.netContracted = 0;
            info.autoDutySkew = checkBoxAutoSkew.Checked;
            info.useAggressiveDuty = checkBoxAggressiveSkew.Checked;
            info.hitRatio = (int)numericHitRatio.Value;
            info.weight = Int32.TryParse(textWeight.Text, out info.weight) ? info.weight : 0;
            info.weight2 = Int32.TryParse(textWeight2.Text, out info.weight2) ? info.weight2 : 0;
            info.quotingType = (string)comboBoxQuotingType.Items[comboBoxQuotingType.SelectedIndex];
            info.SetTrCodeQuotingType();
            info.SetCommandQuoteStart();
            info.hitAuthority = comboBoxHitAuthority.SelectedItem?.ToString();
            info.bullets = (int)numericUpDownFilCanBullet.Value;
            info.depth = (int)numericUpDownFilCanDepth.Value;
            info.pricingLogicParam = textBoxPricingLogicParam.Text;

            info.pInfoList = pInfoList;

            info.decoratedCTM = checkBoxDecoratedCTM.Checked;

            info.jsonString = jsonString;

            info.opk = comboBoxOPK.SelectedItem?.ToString();

            info.opkTick = (int)numericOPKTick.Value;
            info.opkAmtDenominator = (int)numericOPKAmtDenominator.Value;
            info.opkReduceTill = (int)numericOPKReduceTill.Value;
            info.opkTodayOnly = checkBoxOPKTodayOnly.Checked;
            info.opkShortOnly = checkBoxOPKShortOnly.Checked;
            info.opkHitAllowed = checkBoxOPKHitAllowed.Checked;


            info.resolution = Int32.TryParse(comboBoxResolution.Text, out info.resolution) ? info.resolution : 1;

            if (info.isinCode == null || info.isinCode.Trim() == "")
                return null;

            if (info.account == null || info.account.Trim() == "")
                return null;

            if (!checkAccountValidity(info.account, info.isinCode))
                return null;

            return info;
        }

        private void setAccount(QuotingProduct pType)
        {
            List<string> accList = pType.getAccount();
            List<AccountMasterData> amdList = new List<AccountMasterData>();
            accList.ForEach(accNum => amdList.Add(AccountMaster.Instance.get(accNum)));

            comboBoxAccount.Items.Clear();

            if (amdList.Count == 0)
                return;

            foreach (AccountMasterData amd in amdList)
            {
                comboBoxAccount.Items.Add(new ComboBoxItem() { Text = amd.accountName + "_" + amd.accountNumber, Value = amd });
                comboBoxAccount.DisplayMember = "Text";
                comboBoxAccount.ValueMember = "Value";
            }
            comboBoxAccount.SelectedIndex = 0;

        }


        private void setComboBoxString(ComboBox cbBox, string value)
        {
            cbBox.SelectedIndex = -1;
            for (int itemIdx = 0; itemIdx < cbBox.Items.Count; ++itemIdx)
                if (cbBox.Items[itemIdx]?.GetType().GetProperty("value")?.GetValue(cbBox.Items[itemIdx], null).ToString() == value)
                    cbBox.SelectedIndex = itemIdx;
        }

        private string getComboBoxString(ComboBox cbBox)
        {
            string result = cbBox.SelectedItem?.GetType().GetProperty("value")?.GetValue(cbBox.SelectedItem, null).ToString();
            if (result == null)
                return cbBox.Text;
            return result;
        }


        private bool checkAccountValidity(string account, string isinCode)
        {
            if (!AccountMaster.Instance.IsAllowedItem(account, isinCode) || AccountMaster.Instance.IsDisllowedItem(account, isinCode))
            {
                //MessageBox.Show(string.Format("{1} 종목은 {0} 계좌에서 주문이 불가능합니다. 주문을 원할 경우 AccountMaster를 수정해주세요.", account, isinCode));
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool getValidTriggerAmt(int amtShort, int amtLong, int askSkew, int bidSkew, int oldSkewTriggerAmt, int skewUnit, out int result)
        {

            int skewRange = askSkew - bidSkew;

            //제출 수량이 매수,매도 모두 0인 경우 관계X
            if (amtShort == 0 && amtLong == 0)
            {
                result = oldSkewTriggerAmt;
                return true;
            }


            if (oldSkewTriggerAmt == 0)
            {
                result = oldSkewTriggerAmt;
                return true;
            }

            int candidateAmt = Math.Max(amtShort, amtLong);
            int skewOnComplete = candidateAmt / oldSkewTriggerAmt;
            int newSkewTriggerAmt = oldSkewTriggerAmt;

            if (skewRange == 0)
            {
                //askSkew와 bidSkew가 동일한 상태에서 체결완료로 인한 skew 변화가 2회 초과일 때 가장 보수적으로 세팅
                if (skewOnComplete > 2)
                {
                    newSkewTriggerAmt = candidateAmt;
                    DialogResult dr = MessageBox.Show(string.Format("skewTriggerAmt를 {0} -> {1}로 수정하여 발송하시겠습니까?.", oldSkewTriggerAmt, newSkewTriggerAmt), "확인", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.No)
                    {
                        result = oldSkewTriggerAmt;
                        return false;
                    }
                    //MessageBox.Show(string.Format("skewTriggerAmt를 {0} -> {1}로 수정하여 발송하시겠습니까?.", oldSkewTriggerAmt, newSkewTriggerAmt), "알림");                    
                }

            }
            else if (skewRange < skewOnComplete * skewUnit)
            {
                double amtPerUnit = (double)candidateAmt / (double)skewRange;
                newSkewTriggerAmt = (int)Math.Ceiling(amtPerUnit * skewUnit);
                DialogResult dr = MessageBox.Show(string.Format("skewTriggerAmt를 {0} -> {1}로 수정하여 발송하시겠습니까?.", oldSkewTriggerAmt, newSkewTriggerAmt), "확인", MessageBoxButtons.YesNo);
                if (dr == DialogResult.No)
                {
                    result = oldSkewTriggerAmt;
                    return false;
                }
                //MessageBox.Show(string.Format("skewTriggerAmt를 {0} -> {1}로 수정하여 발송하시겠습니까?.", oldSkewTriggerAmt, newSkewTriggerAmt), "알림");
            }

            result = newSkewTriggerAmt;
            return true;
        }

        private bool IsQuotingInfoValid(QuotingInfo q)
        {
            if (q == null)
                return false;

            if (q.isinCode == null || q.isinCode.Trim() == "")
            {
                MessageBox.Show("isinCode가 없습니다.");
                return false;
            }

            if (q.account == null || q.isinCode.Trim() == "")
            {
                MessageBox.Show("account가 없습니다.");
                return false;
            }

            if (q.bookCode == null || q.bookCode.Trim() == "")
            {
                MessageBox.Show("bookCode가 없습니다.");
                return false;
            }

            return true;
        }

        public void initComboBoxFrom(QuotingProduct pType)
        {
            comboBoxProdType.DataSource = System.Enum.GetValues(typeof(QuotingProduct));
            comboBoxProdType.SelectedItem = pType;

            comboBoxHitAuthority.DataSource = System.Enum.GetNames(typeof(HitAuthority)).ToList();
            comboBoxHitAuthority.SelectedItem = HitAuthority.ALLOW_ALL;

            // PricingLogic 업데이트
            comboBoxPriceLogic.DataSource = PricingLogic.availablePricingLogics(pType);
            comboBoxPriceLogic.SelectedIndex = 0;

            // QtType
            comboBoxQuotingType.DataSource = System.Enum.GetNames(typeof(QuotingType)).ToList();
            comboBoxQuotingType.SelectedItem = 0;

            // SkewStrategy
            comboBoxSkewStrategy.DataSource = System.Enum.GetNames(typeof(SkewStrategy)).ToList();
            comboBoxSkewStrategy.SelectedItem = 0;

            // Amtstrategy
            comboBoxAmtStrategy.DataSource = System.Enum.GetNames(typeof(AmtStrategy)).ToList();
            comboBoxAmtStrategy.SelectedItem = 0;

            comboBoxOPK.DataSource = System.Enum.GetNames(typeof(OPK)).ToList();
            comboBoxOPK.SelectedItem = 0;

            comboBoxHittingStyle.DataSource = System.Enum.GetNames(typeof(HittingStyle)).ToList();
            comboBoxHittingStyle.SelectedItem = 0;

            comboBoxQuotingStyle.DataSource = System.Enum.GetNames(typeof(QuotingStyle)).ToList();
            comboBoxQuotingStyle.SelectedItem = 0;

            comboBoxBiasedQuotingStyle.DataSource = System.Enum.GetNames(typeof(QuotingStyle)).ToList();
            comboBoxBiasedQuotingStyle.SelectedItem = 0;
        }


        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (multiMode)
            {
                foreach (QuotingInfo info in selectedQuotingInfoList)
                {
                    foreach (Control control in multiModeAvailableControlSet)
                    {
                        ApplyControlData(info, control);
                    }

                    CheckAndShootOrder(info);
                }
            }
            else
            {
                QuotingInfo info = getQuotingInfo();
                CheckAndShootOrder(info);
            }

            refreshSelectPurpose();
        }

        private void CheckAndShootOrder(QuotingInfo info)
        {
            QuoteEventArgs<QuotingInfo> qe = QuoteEventArgs<QuotingInfo>.Create(info);

            if (!IsQuotingInfoValid(info))
                return;

            if (!checkAccountValidity(info.account, info.isinCode))
                return;

            //SkewStrategy skewStrat;
            //Enum.TryParse(info.skewStrategy, out skewStrat);
            //if (skewStrat == SkewStrategy.SLIDE_ON_CONTRACT)
            //{
            //    bool result = getValidTriggerAmt(info.amountShort, info.amountLong, info.askSkew, info.bidSkew, info.skewTriggerAmt, info.skewUnit, out info.skewTriggerAmt);
            //    if (!result)
            //        return;
            //}

            info.SetFitToMarket(Control.ModifierKeys);
            info.SetCommandQuoteStart();
            StartButtonClick?.Invoke(null, qe);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (multiMode)
            {
                foreach (QuotingInfo info in selectedQuotingInfoList)
                {
                    CheckAndCancelOrder(info);
                }
            }
            else
            {
                QuotingInfo info = getQuotingInfo();
                CheckAndCancelOrder(info);
            }
        }

        private void CheckAndCancelOrder(QuotingInfo info)
        {
            QuoteEventArgs<QuotingInfo> qe = QuoteEventArgs<QuotingInfo>.Create(info);

            if (!IsQuotingInfoValid(info))
                return;

            if (!checkAccountValidity(info.account, info.isinCode))
                return;

            CancelButtonClick?.Invoke(null, qe);
        }

        private void comboBoxQuotingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxQuotingType.SelectedIndex == -1)
                return;

            string selectedStr = comboBoxQuotingType.SelectedItem as string;
            QuotingType qt;
            System.Enum.TryParse(selectedStr, out qt);

            if (qt == QuotingType.NORMAL || qt == QuotingType.SHADOW)
            {
                numericUpDownFilCanBullet.Enabled = false;
                numericUpDownFilCanDepth.Enabled = false;
            }
            else
            {
                numericUpDownFilCanBullet.Enabled = true;
                numericUpDownFilCanDepth.Enabled = true;
            }
        }

        private void comboBoxSkewStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSkewStrategy.SelectedIndex == -1)
                return;

            string selectedStr = comboBoxSkewStrategy.SelectedItem as string;
            SkewStrategy skewStrat;
            System.Enum.TryParse(selectedStr, out skewStrat);

            if (skewStrat == SkewStrategy.BY_TICK)
            {
                // "BY_TICK skew strategy를 선택할 경우 변경해주는 skew 정보들.
                numericUpDownSkewUnit.Value = 1;

                numericAskSkew.Value = 5;
                numericBidSkew.Value = -5;
            }
        }

        private void updateSFExpectedValues(String stockCode, String futuresCode, int bidBuffer, int askBuffer, bool updateSkew = false)
        {
            FuturesInstrument fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(futuresCode);

            if (fi == null)
            {
                MessageBox.Show("해당 기능은 현재 주식선물에 대해서만 사용 가능합니다.");
                return;
            }

            BidAskData ba = MarketDataCenter.Instance.GetBidAskData(stockCode);
            if (ba == null)
            {
                MessageBox.Show("해당 기능은 실시간 시세 수신이 필요합니다.");
                return;
            }
            int price = ba.bidPrice[0];

            double theoDrvPrice = fi.GetTheoFutPrice(price, DateTime.Today, ItemMaster.Instance.rfr);
            double timeValue = theoDrvPrice - price;

            double div = 0;
            PricingFactorDoc pfd = DBUtil.Instance.GetPricingFactor(stockCode);
            if (pfd == null)
            {
                div = 0;
            }
            else if (fi.isUnderDivEffect(DateTime.Today, pfd.divDate))
            {
                div = pfd.div;
            }

            double diff = timeValue + div;

            TickCalculator stockFuturesTickCalculator = new StockTickCalculator();
            double tickSize = stockFuturesTickCalculator.AddTick(price, 1) - price;

            double bidMinMargin = (int)(diff % tickSize);
            double askMinMargin = tickSize - bidMinMargin;

            if (updateSkew)
            {
                numericBidSkew.Value = (int)(bidBuffer * tickSize);
                numericAskSkew.Value = (int)(askBuffer * tickSize);
                numericUpDownSkewUnit.Value = (int)(tickSize);
            }
        }

        private void adjustSkew(object sender, int adjAsk, int adjBid)
        {
            String isinCode = textBoxCode.Text;
            if (isinCode.Equals(String.Empty))
            {
                MessageBox.Show("상품코드 입력 오류");
                return;
            }

            numericAskSkew.Value = (int)numericAskSkew.Value + adjAsk;
            numericBidSkew.Value = (int)numericBidSkew.Value + adjBid;


            QuotingInfo info = getQuotingInfo();
            QuoteEventArgs<QuotingInfo> qe = QuoteEventArgs<QuotingInfo>.Create(info);

            if (!IsQuotingInfoValid(info))
                return;

            if (!checkAccountValidity(info.account, info.isinCode))
                return;

            if (this.StartButtonClick != null && sendQuoteWhenAdjustSkew)
            {
                this.StartButtonClick(sender, qe);
            }
        }

        private void comboBoxPriceLogic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPriceLogic.SelectedIndex == -1)
                return;

            string selectedStr = comboBoxPriceLogic.SelectedItem as string;
        }

        private void buttonItemSelect_Click(object sender, EventArgs e)
        {
            ItemSelectionForm itemSelectionForm = new ItemSelectionForm(true, false);
            itemSelectionForm.ShowDialog();

            UpdateCodeToMonitor(textBoxCode.Text, itemSelectionForm.SelectedCode);
        }

        private void InitCodeToMonitorForIndexFutures()
        {
            IFCTMComboBoxMode = true;
            string[] kospiIndexList = { "CustomIndexMidProvider_KRD020020016_C", "SC30IndexProvider_KRD020020016_C", "CustomIndexMidProvider_KRD020021378_C", "SC30IndexProvider_KRD020021378_C", "customSFIndex" };

            comboBoxCodeToMonitor.Items.Clear();

            foreach (string code in kospiIndexList)
            {
                comboBoxCodeToMonitor.Items.Add(new { dispName = code, value = code });
            }

            HashSet<string> underlyingIsinCodeSet = new HashSet<string>();

            foreach (var uid in UnderlyingID.GetIndexUnderlyingIDList())
            {
                if (underlyingIsinCodeSet.Contains(uid.underlyingIsinCode))
                    continue;

                underlyingIsinCodeSet.Add(uid.underlyingIsinCode);

                var underlyingIsinCode = uid.underlyingIsinCode;
                var alias = "Basket_" + uid.korean;

                comboBoxCodeToMonitor.Items.Add(new { dispName = alias, value = underlyingIsinCode });
                comboBoxCodeToMonitor.DisplayMember = "dispName";
                comboBoxCodeToMonitor.ValueMember = "value";
            }

        }

        private void UpdateCodeToMonitor(string isinCode, string newCTMisinCode = null)
        {
            if (GetCTMList == null)
            {
                if (newCTMisinCode == null)
                    return;
                if (newCTMisinCode.Trim() == "")
                    return;

                if (!IFCTMComboBoxMode)
                {
                    comboBoxCodeToMonitor.Items.Clear();

                    var korean = ItemMaster.Instance.GetProdNameWithIsinCode(newCTMisinCode);
                    comboBoxCodeToMonitor.Items.Add(new { dispName = korean, value = newCTMisinCode });
                    comboBoxCodeToMonitor.DisplayMember = "dispName";
                    comboBoxCodeToMonitor.ValueMember = "value";

                    setComboBoxString(comboBoxCodeToMonitor, newCTMisinCode);
                    comboBoxCodeToMonitor.SelectedIndex = comboBoxCodeToMonitor.Items.IndexOf(newCTMisinCode);
                }
                else
                {
                    setComboBoxString(comboBoxCodeToMonitor, newCTMisinCode);
                    if (comboBoxCodeToMonitor.SelectedIndex == -1)
                    {
                        var korean = ItemMaster.Instance.GetProdNameWithIsinCode(newCTMisinCode);
                        comboBoxCodeToMonitor.Items.Add(new { dispName = korean, value = newCTMisinCode });
                        setComboBoxString(comboBoxCodeToMonitor, newCTMisinCode);
                    }
                }
            }
            else
            {
                if (newCTMisinCode != null && newCTMisinCode.Trim() == "")
                    return;

                SynchronizedCollection<string> CTMList = GetCTMList(isinCode);

                if (newCTMisinCode != null && !CTMList.Contains(newCTMisinCode))
                {
                    CTMList.Insert(0, newCTMisinCode);
                }

                if (!IFCTMComboBoxMode)
                {
                    comboBoxCodeToMonitor.Items.Clear();

                    foreach (string codeToMonitor in CTMList)
                    {
                        var korean = ItemMaster.Instance.GetProdNameWithIsinCode(codeToMonitor);
                        comboBoxCodeToMonitor.Items.Add(new { dispName = korean, value = codeToMonitor });
                        comboBoxCodeToMonitor.DisplayMember = "dispName";
                        comboBoxCodeToMonitor.ValueMember = "value";
                    }

                    if (newCTMisinCode == null)
                    {
                        comboBoxCodeToMonitor.SelectedIndex = 0;
                    }
                    else
                    {
                        setComboBoxString(comboBoxCodeToMonitor, newCTMisinCode);
                        //comboBoxCodeToMonitor.SelectedIndex = comboBoxCodeToMonitor.Items.IndexOf(newCTMisinCode);
                    }
                }
                else
                {
                    if (newCTMisinCode == null)
                    {
                        comboBoxCodeToMonitor.SelectedIndex = 0;
                    }
                    else
                    {
                        setComboBoxString(comboBoxCodeToMonitor, newCTMisinCode);
                        if (comboBoxCodeToMonitor.SelectedIndex == -1)
                        {
                            var korean = ItemMaster.Instance.GetProdNameWithIsinCode(newCTMisinCode);
                            comboBoxCodeToMonitor.Items.Add(new { dispName = korean, value = newCTMisinCode });
                            setComboBoxString(comboBoxCodeToMonitor, newCTMisinCode);
                        }
                    }
                }
            }

            comboBoxCodeToMonitor.DropDownWidth = UIUtil.DropDownWidth(comboBoxCodeToMonitor);
        }

        private void buttonAskUpBidStay_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, unit, 0);
        }

        private void buttonAskDownBidStay_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, -unit, 0);
        }

        private void buttonAskStayBidDown_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, 0, -unit);
        }

        private void buttonAskStayBidUp_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, 0, unit);
        }

        private void buttonAskUpBidUp_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, unit, unit);
        }

        private void buttonAskDownBidDown_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, -unit, -unit);
        }

        private void buttonAskUpBidDown_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, unit, -unit);
        }

        private void buttonAskDownBidUp_Click(object sender, EventArgs e)
        {
            int unit = (int)numericUpDownSkewUnit.Value;
            adjustSkew(sender, -unit, unit);
        }

        private void numericUpDownSkewUnit_ValueChanged(object sender, EventArgs e)
        {
            numericAskSkew.Increment = numericUpDownSkewUnit.Value;
            numericBidSkew.Increment = numericUpDownSkewUnit.Value;
        }

        private void numericAskSkew_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown c = (NumericUpDown)sender;
            if (c.Value < 0)
                c.BackColor = Color.OrangeRed;
            else
                c.BackColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void numericBidSkew_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown c = (NumericUpDown)sender;
            if (c.Value > 0)
                c.BackColor = Color.OrangeRed;
            else
                c.BackColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void radioAutoSkewAble_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoSkew.Checked)
            {
                numericAskSkew.Enabled = false;
            }
            else
            {
                numericAskSkew.Enabled = true;
            }
        }

        private void buttonSavePurpose_Click(object sender, EventArgs e)
        {
            QuotingInfo info = getQuotingInfo();

            if (info == null)
                return;

            if (!CheckValidity(info))
                return;

            if (this.GetDBInfoManager != null)
            {
                var infoManager = GetDBInfoManager();
                infoManager.UpdateQuotingInfo(info);
                refreshSelectPurpose();
            }
            else
            {
                MessageBox.Show("GetQuotingPurposeDic, UpdateQuotingPurposeNameTable이 설정되지 않았습니다.", "오류");
            }
        }

        private void buttonRemoveStyle_Click(object sender, EventArgs e)
        {
            QuotingInfo info = getQuotingInfo();

            if (info == null)
                return;

            if (GetDBInfoManager != null)
            {
                var infoManager = GetDBInfoManager();
                var quotingStyleList = infoManager.GetStyleList(info.isinCode, info.purpose);
                string warningMsg = "";

                if (quotingStyleList.Count == 0)
                {
                    MessageBox.Show("삭제할 QuotingPurpose가 없습니다.", "알림");
                    return;
                }
                else if (quotingStyleList.Count == 1)
                {
                    warningMsg = "\n주의 : 현재 해당 종목의 QuotingPurpose가 1개밖에 없습니다.";
                }

                DialogResult result = MessageBox.Show(string.Format("{0} 종목의 {1}을 정말 삭제하시겠습니까?{2}", ItemMaster.Instance.GetProdNameWithIsinCode(info.isinCode), info.purpose, warningMsg), "알림", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }

                infoManager.RemoveQuotingInfo(info);
                labelStyle.Text = "";
                refreshSelectPurpose();
            }
            else
            {
                MessageBox.Show("GetQuotingPurposeDic, GetQuotingPurposeList가 설정되지 않았습니다.", "오류");
            }
        }

        private void buttonClonePurpose_Click(object sender, EventArgs e)
        {
            QuotingInfo info = getQuotingInfo();

            if (info == null)
                return;

            info.style = textBoxCloneStyleName.Text;

            if (!CheckValidity(info))
                return;

            if (info.style.Trim().Equals(""))
            {
                MessageBox.Show("Style을 입력해주세요.", "오류");
                return;
            }

            if (GetDBInfoManager != null)
            {
                var infoManager = GetDBInfoManager();
                var styleList = infoManager.GetStyleList(info.isinCode, info.purpose);

                if (styleList.Contains(info.style))
                {
                    DialogResult result = MessageBox.Show(info.style + "이(가) 이미 존재합니다. 데이터를 덮어씌우시겠습니까?", "알림", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                infoManager.UpdateQuotingInfo(info);
                setQuotingInfo(info);
                refreshSelectPurpose();
            }
            else
            {
                MessageBox.Show("GetQuotingInfoManager가 설정되지 않았습니다.", "오류");
            }
        }

        public void refreshSelectPurpose()
        {
            string isinCode = textBoxCode.Text;
            string purpose = comboBoxPurpose.Text;

            if (this.GetDBInfoManager != null)
            {
                var infoManager = GetDBInfoManager();

                comboBoxSelectStyle.Items.Clear();

                foreach (string style in infoManager.GetStyleList(isinCode, purpose).OrderBy(str => str))
                {
                    comboBoxSelectStyle.Items.Add(style);
                }

                comboBoxSelectStyle.Text = "";
                comboBoxSelectStyle.SelectedItem = null;
                comboBoxSelectStyle.DropDownWidth = UIUtil.DropDownWidth(comboBoxSelectStyle);
            }
            else
            {
                MessageBox.Show("GetQuotingInfoManager가 연결되지 않았습니다.", "오류");
            }
        }

        private void comboBoxSelectStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_noise) return;

            QuotingInfo info = getQuotingInfo();
            string isinCode = textBoxCode.Text;
            string purpose = comboBoxPurpose.SelectedItem as string;
            string style = comboBoxSelectStyle.SelectedItem as string;

            if (style.Equals(""))
                return;

            if (GetDBInfoManager != null)
            {
                var infoManager = GetDBInfoManager();
                if (infoManager.GetQuotingInfo(isinCode, purpose, style, out info))
                {
                    if (this.remainSkew)
                    {
                        this.remainSkew = false;
                        setQuotingInfo(info, true);
                    }
                    else
                    {
                        setQuotingInfo(info);
                    }
                }
            }
            else
            {
                MessageBox.Show("GetQuotingInfoManager가 연결되지 않았습니다.", "오류");
            }
        }

        private int GetNextIndex(ArrowType arrow, int size, int idx)
        {
            int nextIndex = idx;
            if (arrow == ArrowType.Up)
            {
                if (nextIndex == -1 || nextIndex == 0)
                    nextIndex = size - 1;
                else
                    nextIndex--;
            }
            else if (arrow == ArrowType.Down)
            {
                if (nextIndex == -1 || nextIndex == (size - 1))
                    nextIndex = 0;
                else
                    nextIndex++;
            }

            return nextIndex;
        }

        public void ComboBoxSelectStyleNavigator(ArrowType arrow, bool remainSkew = false)
        {
            if (multiMode)
                return;

            string purpose = labelStyle.Text;
            int realIndex = comboBoxSelectStyle.Items.IndexOf(purpose);
            int size = comboBoxSelectStyle.Items.Count;

            if (size == 0)
                return;
            else if (size == 1)
                realIndex = 0;
            else
            {
                realIndex = GetNextIndex(arrow, size, realIndex);
            }

            this.remainSkew = remainSkew;
            comboBoxSelectStyle.SelectedIndex = realIndex;
        }

        private bool CheckValidity(QuotingInfo info)
        {
            if (info.codeToMonitor == null || info.codeToMonitor.Trim().Equals(""))
            {
                MessageBox.Show("codeToMoniter 값이 잘못되었습니다.", "오류");
                return false;
            }

            return true;
        }

        private void buttonApplyAndSave_Click(object sender, EventArgs e)
        {
            QuotingInfo info = getQuotingInfo();
            QuoteEventArgs<QuotingInfo> qe = QuoteEventArgs<QuotingInfo>.Create(info);

            if (!IsQuotingInfoValid(info))
                return;

            if (!checkAccountValidity(info.account, info.isinCode))
                return;

            DialogResult result = MessageBox.Show("정말 호가를 제출하고 설정한 QuotingInfo를 컬렉션에 반영하시겠습니까?", "알림", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            info.SetFitToMarket(Control.ModifierKeys);

            if (this.StartButtonClick != null && GetDBInfoManager != null)
            {
                this.StartButtonClick(sender, qe);
                var infoManager = GetDBInfoManager();
                infoManager.UpdateQuotingInfo(info);
                //UpdateQuotingPurposeNameTable(info.isinCode, info.purpose, QuotingInfoControl.UpdateType.Add);
                refreshSelectPurpose();
            }
        }

        private void checkBoxSendQuoteWhenAdjustSkew_CheckedChanged(object sender, EventArgs e)
        {
            sendQuoteWhenAdjustSkew = checkBoxSendQuoteWhenAdjustSkew.Checked;
        }

        private void textBoxCode_TextChanged(object sender, EventArgs e)
        {
            string isinCode = textBoxCode.Text;

            if (IsinCreator.IsCallOption(isinCode))
                textBoxCode.BackColor = Color.Red;
            else if (IsinCreator.IsPutOption(isinCode))
                textBoxCode.BackColor = Color.DeepSkyBlue;
            else
                textBoxCode.BackColor = Color.Gray;
        }

        private void comboBoxCodeToMonitor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CTMUpdate != null)
            {
                string codeToMonitor = comboBoxCodeToMonitor.SelectedItem?.ToString();

                if (codeToMonitor != null && textBoxCode.Text != codeToMonitor)
                    this.CTMUpdate(codeToMonitor);
                else
                    this.CTMUpdate("");
            }
        }

        private void comboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelStyle.Text = "기본";
        }

        public void setComboboxStyle(string style)
        {
            if (style == null)
                return;
            this._noise = true;

            int realIndex = comboBoxSelectStyle.Items.IndexOf(style);
            if (realIndex < 0)
                return;

            comboBoxSelectStyle.SelectedIndex = realIndex;
            this._noise = false;
        }

        private bool GetDecoratorTextBox(int index, out TextBox textBox)
        {
            string textBoxName = "textBoxDecorator" + index;
            if (groupBoxDecoration.Controls.ContainsKey(textBoxName))
            {
                textBox = (TextBox)groupBoxDecoration.Controls[textBoxName];
                return true;
            }

            textBox = null;
            return false;
        }

        private bool GetDecoratorTextBox(string controlName, out TextBox textBox)
        {
            string textBoxName = "textBoxDecorator" + controlName[controlName.Length - 1];
            if (groupBoxDecoration.Controls.ContainsKey(textBoxName))
            {
                textBox = (TextBox)groupBoxDecoration.Controls[textBoxName];
                return true;
            }

            textBox = null;
            return false;
        }

        private void SetDecorator(string buttonName, int idx)
        {
            TextBox textBox;
            if (!GetDecoratorTextBox(buttonName, out textBox))
                return;

            DecoratorSelectionForm<DecorationParameterInfo, ItemSelectionForm> form
                = new DecoratorSelectionForm<DecorationParameterInfo, ItemSelectionForm>();
            QuotingInfo info = getQuotingInfo();

            if (info == null)
            {
                MessageBox.Show("QuotingInfo 정보가 불완전해 QuotingInfo를 생성할 수 없습니다.");
                return;
            }

            form.SetData(info, pInfoList);
            if (textBox.Text != "")
                form.SetDefaultStrategy(idx);

            form.ShowDialog();

            if (form.removeCurrentDecorator)
            {
                if (pInfoList.Count < idx)
                    return;

                pInfoList.RemoveAt(idx);
                RefreshDecoratorView();
            }
            else if (form.defaultStrategy != null)
                RefreshDecoratorView();

        }

        private void buttonSetDecorator1_Click(object sender, EventArgs e)
        {
            SetDecorator(((Button)sender).Name, 0);
        }

        private void buttonSetDecorator2_Click(object sender, EventArgs e)
        {
            SetDecorator(((Button)sender).Name, 1);
        }

        private void buttonSetDecorator3_Click(object sender, EventArgs e)
        {
            SetDecorator(((Button)sender).Name, 2);
        }

        private void buttonSetDecorator4_Click(object sender, EventArgs e)
        {
            SetDecorator(((Button)sender).Name, 3);
        }

        private void SwapDecorator(int firstIdx, int secondIdx)
        {
            DecorationParameterInfo pInfoTemp;

            pInfoTemp = pInfoList[firstIdx];
            pInfoList[firstIdx] = pInfoList[secondIdx];
            pInfoList[secondIdx] = pInfoTemp;
        }

        private void MoveDecorator(string buttonName, int idx, bool isUp)
        {
            int targetIdx;
            if (isUp)
                targetIdx = idx - 1;
            else
                targetIdx = idx + 1;

            if (idx < 0 || targetIdx < 0 || idx >= pInfoList.Count || targetIdx >= pInfoList.Count)
            {
                return;
            }

            SwapDecorator(idx, targetIdx);
            RefreshDecoratorView();
        }

        private void RefreshDecoratorView()
        {
            int idx = 0;
            TextBox textBox;
            foreach (var pInfo in pInfoList)
            {
                if (!GetDecoratorTextBox(++idx, out textBox))
                    break;

                textBox.Text = pInfo.strategy;
            }

            while (true)
            {
                if (!GetDecoratorTextBox(++idx, out textBox))
                    break;

                textBox.Text = "";
            }
        }

        private void buttonParameterSetting_Click(object sender, EventArgs e)
        {
            JsonViewer f = new JsonViewer();
            f.SetJsonString(jsonString);
            f.ShowDialog();

            if (f.CheckApplyClose())
            {
                jsonString = f.GetJsonString();
                ApplyJsonStringCount();
            }
        }

        private void ApplyJsonStringCount()
        {
            if (jsonString == null || jsonString.Trim() == "")
            {
                buttonParameterSetting.Text = "P";
                return;
            }

            try
            {
                JObject root = JObject.Parse(jsonString);
                buttonParameterSetting.Text = root.Count + "";
            }
            catch (JsonReaderException)
            {
                buttonParameterSetting.Text = "P";
            }
        }

        #region multiModeMethods

        public List<Control> GetControllableControls(Control parent)
        {
            List<Control> controlList = new List<Control>();

            if (controllableControlTypeSet.Contains(parent.GetType()))
                controlList.Add(parent);

            foreach (Control control in parent.Controls)
            {
                controlList.AddRange(GetControllableControls(control));
            }

            return controlList;
        }

        public bool CheckMultiMode(iGrid grid)
        {
            List<int> selectedRowsIndex = new List<int>();

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (!grid.Rows[i].Visible)
                    continue;

                for (int j = 0; j < grid.Cols.Count; j++)
                {
                    if (grid.Rows[i].Cells[j].Selected)
                    {
                        selectedRowsIndex.Add(i);
                        break;
                    }
                }
            }

            if (selectedRowsIndex.Count <= 1)
            {
                SetMode(false);
                return false;
            }

            List<QuotingInfo> infoList = new List<QuotingInfo>();

            foreach (int index in selectedRowsIndex)
            {
                string isinCode = grid.Rows[index].Cells["종목코드"].Value.ToString();
                string purpose = grid.Rows[index].Cells["Purpose"].Value.ToString();
                string style = grid.Rows[index].Cells["Style"].Value.ToString();

                QuotingInfo info;

                if (GetCiriAndOAInfoManager != null && GetDBInfoManager != null)
                {
                    if (GetCiriAndOAInfoManager().GetQuotingInfo(isinCode, purpose, false, out info))
                    {
                        infoList.Add(info);
                    }
                    else if (GetDBInfoManager().GetQuotingInfo(isinCode, purpose, style, out info))
                    {
                        infoList.Add(info);
                    }
                }
            }

            selectedQuotingInfoList = infoList;
            SetMode(true);
            return true;
        }

        /// <summary>
        /// Tuple => isinCode, purpose, style
        /// </summary>
        /// <param name="rows"></param>
        public bool CheckMultiMode(List<Tuple<string, string, string>> rows)
        {
            if (rows.Count <= 1)
            {
                SetMode(false);
                return false;
            }

            List<QuotingInfo> infoList = new List<QuotingInfo>();

            foreach (var item in rows)
            {
                string isinCode = item.Item1;
                string purpose = item.Item2;
                string style = item.Item3;

                QuotingInfo info;

                if (GetCiriAndOAInfoManager != null && GetDBInfoManager != null)
                {
                    if (GetCiriAndOAInfoManager().GetQuotingInfo(isinCode, purpose, false, out info))
                    {
                        infoList.Add(info);
                    }
                    else if (GetDBInfoManager().GetQuotingInfo(isinCode, purpose, style, out info))
                    {
                        infoList.Add(info);
                    }
                }
            }

            selectedQuotingInfoList = infoList;
            SetMode(true);
            return true;
        }

        public void SetMode(bool multiMode)
        {
            if (this.multiMode != multiMode)
            {
                ToggleMode();
            }
        }

        private void ToggleMode()
        {
            // Single -> Multi
            if (this.multiMode == false)
            {
                foreach (var entry in initStatusDic)
                {
                    Control control = entry.Key;

                    if (multiModeAvailableControlSet.Contains(control))
                        continue;

                    control.Enabled = false;
                }

                foreach (Control control in multiModeAvailableControlSet)
                {
                    if (control is ComboBox)
                    {
                        ComboBox cb = (ComboBox)control;
                        if (cb.DataSource != null)
                        {
                            var ds = cb.DataSource;

                            if (ds is List<string>)
                            {
                                List<string> list = (List<string>)ds;
                                list.Insert(0, NotChangedString);
                            }

                            cb.SelectedIndex = 0;

                            // 콤보박스 목록 갱신을 위해서 DataSource를 갱신해주어야 함
                            cb.DataSource = null;
                            cb.DataSource = ds;
                        }
                        else
                        {
                            if (cb == comboBoxAccount)
                            {
                                cb.Items.Insert(0, new ComboBoxItem()
                                {
                                    Text = NotChangedString,
                                    Value = new AccountMasterData()
                                    {
                                        accountName = NotChangedString,
                                        accountNumber = NotChangedString
                                    }
                                });
                            }
                            else if (cb == comboBoxBook || cb == comboBoxAccount)
                            {
                                cb.Items.Insert(0, new { dispName = NotChangedString, value = NotChangedString });
                            }
                            else
                            {
                                cb.Items.Insert(0, NotChangedString);
                            }

                            cb.SelectedIndex = 0;
                        }
                    }
                }
            }
            else // Multi -> Single
            {
                foreach (Control control in multiModeAvailableControlSet)
                {
                    if (control is ComboBox)
                    {
                        ComboBox cb = (ComboBox)control;
                        if (cb.DataSource != null)
                        {
                            var ds = cb.DataSource;

                            if (ds is List<string>)
                            {
                                List<string> list = (List<string>)ds;
                                list.RemoveAt(0);
                            }

                            // 콤보박스 목록 갱신을 위해서 DataSource를 갱신해주어야 함
                            cb.DataSource = null;
                            cb.DataSource = ds;
                        }
                        else
                        {
                            cb.Items.RemoveAt(0);
                        }

                        cb.SelectedIndex = 0;
                    }
                }

                foreach (var entry in initStatusDic)
                {
                    Control control = entry.Key;
                    bool enabled = entry.Value;

                    control.Enabled = enabled;
                }
            }

            this.multiMode = !this.multiMode;
        }

        private bool ApplyControlData(QuotingInfo info, Control control)
        {
            string val;
            switch (control.Name)
            {
                case "comboBoxQuotingType":
                    val = getComboBoxString((ComboBox)control);
                    if (val == NotChangedString)
                        return false;
                    info.quotingType = getComboBoxString((ComboBox)control);
                    return true;
                case "comboBoxBook":
                    val = getComboBoxString((ComboBox)control);
                    if (val == NotChangedString)
                        return false;
                    info.bookCode = getComboBoxString((ComboBox)control);
                    return true;
                case "comboBoxAccount":
                    val = ((AccountMasterData)((ComboBoxItem)((ComboBox)control).SelectedItem).Value).accountNumber;
                    if (val == NotChangedString)
                        return false;
                    info.account = val;
                    return true;
                case "comboBoxAmtStrategy":
                    val = getComboBoxString((ComboBox)control);
                    if (val == NotChangedString)
                        return false;
                    info.SetAmountStrategy(((ComboBox)control).SelectedItem?.ToString());
                    return true;
                case "comboBoxHitAuthority":
                    val = getComboBoxString((ComboBox)control);
                    if (val == NotChangedString)
                        return false;
                    info.hitAuthority = ((ComboBox)control).SelectedItem?.ToString();
                    return true;
                case "comboBoxPriceLogic":
                    val = getComboBoxString((ComboBox)control);
                    if (val == NotChangedString)
                        return false;
                    info.logic = ((ComboBox)control).Items[((ComboBox)control).SelectedIndex].ToString();
                    return true;
                case "comboBoxOPK":
                    val = getComboBoxString((ComboBox)control);
                    if (val == NotChangedString)
                        return false;
                    info.opk = ((ComboBox)control).SelectedItem?.ToString();
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        private void comboBoxOPK_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            string text = ((ComboBox)sender).Items[e.Index].ToString();
            Brush brush = OPKUtil.GetBrush(text);
            //if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            //    brush = Brushes.Yellow;

            e.Graphics.FillRectangle(brush, e.Bounds);

            e.Graphics.DrawString(text, ((Control)sender).Font, Brushes.Black, e.Bounds.X, e.Bounds.Y);
        }

        private void comboBoxResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            int lastResolution, currentResolution;
            lastResolution = this.lastResolution;
            if (!Int32.TryParse(comboBoxResolution.Text, out currentResolution))
            {
                return;
            }

            this.lastResolution = currentResolution;

            if (internalUpdate)
            {
                return;
            }

            DialogResult result = MessageBox.Show(string.Format("Skew에 Resolution 변화를 적용하시겠습니까? ({0} -> {1})", lastResolution, currentResolution), "확인", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            double ratio = (double)currentResolution / lastResolution;

            int ask, bid, unit;
            ask = (int)((int)numericAskSkew.Value * ratio);
            bid = (int)((int)numericBidSkew.Value * ratio);
            unit = (int)((int)numericUpDownSkewUnit.Value * ratio);

            if (unit <= 0)
                unit = 1;

            numericAskSkew.Value = ask;
            numericBidSkew.Value = bid;
            numericUpDownSkewUnit.Value = unit;
        }

        private void buttonMoveUpDecorator1_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 0, true);
        }

        private void buttonMoveUpDecorator2_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 1, true);
        }

        private void buttonMoveUpDecorator3_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 2, true);
        }

        private void buttonMoveUpDecorator4_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 3, true);
        }

        private void buttonMoveDownDecorator1_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 0, false);
        }

        private void buttonMoveDownDecorator2_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 1, false);
        }

        private void buttonMoveDownDecorator3_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 2, false);
        }

        private void buttonMoveDownDecorator4_Click(object sender, EventArgs e)
        {
            MoveDecorator(((Button)sender).Name, 3, false);
        }

        private void checkBoxAutoSkew_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoSkew.Checked)
            {
                numericAskSkew.Enabled = false;
            }
            else
            {
                numericAskSkew.Enabled = true;
            }
        }

        private void buttonHistory_Click(object sender, EventArgs e)
        {
            QuotingInfo info = getQuotingInfo();
            ViewQuotingInfoHistory(info);
        }

        private void ViewQuotingInfoHistory(QuotingInfo info)
        {
            QuoteEventArgs<QuotingInfo> qe = QuoteEventArgs<QuotingInfo>.Create(info);
            HistoryButtonClick?.Invoke(null, qe);
        }
    }
}
