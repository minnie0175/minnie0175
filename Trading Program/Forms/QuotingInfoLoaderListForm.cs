using Ciri.Util;
using CiriData.Data;
using CiriData.Enums;
using CiriData.Manage;
using CommonLib.DBDataType;
using CommonLib.Util;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TenTec.Windows.iGridLib;

namespace Ciri.Forms
{
    public partial class QuotingInfoLoaderListForm : Form
    {
        private KRXDBQuotingInfoManager infoManager = new KRXDBQuotingInfoManager();
        private Form parentForm = null;

        public QuotingInfoLoaderListForm()
        {
            InitializeComponent();
            IGridUtil.UnlockIGrid(iGridQuotingInfo);
        }

        public void SetParentForm(Form form)
        {
            this.parentForm = form;
        }

        public void SetData(List<QuotingInfo> quotingInfoList)
        {
            foreach (QuotingInfo info in quotingInfoList)
            {
                infoManager.UpdateQuotingInfo(info);

                QuotingProduct pType = QuotingProductFunctions.GetTypeFrom(info.isinCode);
                string type1 = "";
                string type2 = "";
                string type3 = "";
                string type4 = "";

                if (pType == QuotingProduct.STOCK)
                {
                    StockLPDoc doc;
                    DBUtil.Instance.GetStockLPList().TryGetValue(info.isinCode, out doc);

                    if (doc != null)
                    {
                        type1 = doc.isDuty.ToString();
                        type2 = doc.dutyAmount.ToString();
                        type3 = doc.tradeType;
                        type4 = doc.type;
                    }
                }
                else if (pType == QuotingProduct.SF)
                {
                    var fi = ItemMaster.Instance.GetFuturesInstrumentWithIsinCode(info.isinCode);

                    if (fi != null)
                    {
                        type1 = LPDutyManager.Instance.IsCurrentLPItem(fi.isinCode) == true ? "True" : "False";
                        type2 = fi.maturityYearMonth.ToString();
                    }
                }
                else if (pType == QuotingProduct.ETF)
                {
                    ETFLPDoc doc;
                    DBUtil.Instance.GetETFLPList().TryGetValue(info.isinCode, out doc);

                    if (doc != null)
                    {
                        type1 = doc.LP계약여부.ToString();
                        type2 = doc.group;
                        type3 = doc.underlying;
                        type4 = doc.idx.ToString();
                    }
                }

                iGRow row = iGridQuotingInfo.Rows.Add();
                row.Cells["종류"].Value = pType.ToString();
                row.Cells["유형1"].Value = type1;
                row.Cells["유형2"].Value = type2;
                row.Cells["유형3"].Value = type3;
                row.Cells["유형4"].Value = type4;
                row.Cells["종목명"].Value = ItemMaster.Instance.GetProdNameWithIsinCode(info.isinCode);
                row.Cells["종목코드"].Value = info.isinCode;
                row.Cells["Purpose"].Value = info.purpose;
                row.Cells["Style"].Value = info.style;
            }

            iGridQuotingInfo.Cols["종목명"].AutoWidth();
            iGridQuotingInfo.SortObject.Clear();
            iGridQuotingInfo.SortObject.Add("종류", iGSortOrder.Descending);
            iGridQuotingInfo.SortObject.Add("종목명", iGSortOrder.Ascending);
            iGridQuotingInfo.SortObject.Add("Purpose", iGSortOrder.Ascending);
            iGridQuotingInfo.SortObject.Add("Style", iGSortOrder.Ascending);
            iGridQuotingInfo.Sort();
        }

        private void saveSelectedQuotingInfo()
        {
            int count = 0;

            for (int i = 0; i < iGridQuotingInfo.Rows.Count; i++)
            {
                var row = iGridQuotingInfo.Rows[i];

                if (row.Selected)
                {
                    count++;
                    string isinCode = row.Cells["종목코드"].Value.ToString();
                    string purpose = row.Cells["Purpose"].Value.ToString();
                    string style = row.Cells["Style"].Value.ToString();
                    QuotingInfo info;
                    if (infoManager.GetQuotingInfo(isinCode, purpose, style, out info))
                    {
                        ControlUpdater.KrxdbInfoManager.UpdateQuotingInfo(info, true);
                    }
                }
            }

            if (count == 0)
            {
                MessageBox.Show("선택된 종목이 없습니다.");
            }
            else
            {
                MessageBox.Show(string.Format("{0}개의 QuotingInfo를 불러왔습니다.", count));
                if (parentForm != null)
                {
                    parentForm.Close();
                }
                this.Close();
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            saveSelectedQuotingInfo();
        }
    }
}
