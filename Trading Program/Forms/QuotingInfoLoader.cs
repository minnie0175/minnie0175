using CiriData.Data;
using CiriData.Enums;
using CiriData.Manage;
using CommonLib.Util;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ciri.Forms
{
    public partial class QuotingInfoLoader : Form
    {
        readonly Dictionary<string, QuotingProduct> CategoryMap = new Dictionary<string, QuotingProduct>()
        {
            { "주식", QuotingProduct.STOCK },
            { "주식선물", QuotingProduct.SF },
            { "ETF", QuotingProduct.ETF },
            { "지수선물", QuotingProduct.IF },
        };


        public QuotingInfoLoader()
        {
            InitializeComponent();

            checkedListBox.Items.Clear();
            foreach (var entry in CategoryMap)
            {
                checkedListBox.Items.Add(entry.Key);
            }

            comboBoxServer.Items.Clear();
            #region modified
            //foreach (var entry in ServerMap)
            //{
            //    comboBoxServer.Items.Add(entry.Key);
            //}
            foreach (string serverId in ServerManager.Instance.GetSeverIdList())
            {
                comboBoxServer.Items.Add(serverId);
            }
            #endregion
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            string userId = textBoxUserId.Text;
            if (userId == null || userId.Trim().Equals(""))
            {
                MessageBox.Show("QuotingInfo를 불러올 사용자의 UserId를 입력하세요.");
                return;
            }

            if (comboBoxServer.SelectedItem == null || comboBoxServer.SelectedIndex == -1)
            {
                MessageBox.Show("QuotingInfo를 불러올 서버를 선택하세요.");
                return;
            }

            string serverId = comboBoxServer.SelectedItem.ToString();
            string serverIp = ServerManager.Instance.GetIpFromServerId(serverId);
            #region modified
            //if (!ServerMap.TryGetValue(serverId, out serverIp))
            //{
            //    MessageBox.Show("등록되지 않은 서버가 선택되어 있습니다.");
            //    return;
            //}

            if (string.IsNullOrEmpty(serverIp))
            {
                MessageBox.Show("등록되지 않은 서버가 선택되어 있습니다.");
                return;
            }
            #endregion
            HashSet<QuotingProduct> productSet = new HashSet<QuotingProduct>();
            foreach (var item in checkedListBox.CheckedItems)
            {
                string value = item.ToString();
                QuotingProduct product;

                if (CategoryMap.TryGetValue(value, out product))
                {
                    productSet.Add(product);
                }
            }

            if (productSet.Count == 0)
            {
                MessageBox.Show("불러올 데이터 유형을 선택해주세요.");
                return;
            }

            string quotingPurposeDicJson;
            DateTime updateDate, updateTime;
            DBUtil.Instance.GetQuotingPurposeDic(serverIp, userId, out updateDate, out updateTime, out quotingPurposeDicJson);
            List<QuotingInfo> quotingInfoList = new List<QuotingInfo>();

            if (quotingPurposeDicJson.Equals(""))
            {
                MessageBox.Show(string.Format("서버 : {0}, UserId : {1}에 해당하는 QuotingInfo가 DB에 없습니다.", serverIp, userId));
                return;
            }
            else
            {
                var dic = JsonConvert.DeserializeObject<ConcurrentDictionary<string, QuotingInfo>>(quotingPurposeDicJson);
                foreach (string key in dic.Keys)
                {
                    string isinCode = dic[key].isinCode;
                    if (productSet.Contains(QuotingProductFunctions.GetTypeFrom(isinCode)))
                    {
                        dic[key].netContracted = 0;
                        dic[key].orderSubmitted = 0;
                        dic[key].ba = null;
                        dic[key].woLongPriceAmt = null;
                        dic[key].woShortPriceAmt = null;

                        if (dic[key].style == null || dic[key].style == "")
                        {
                            dic[key].style = dic[key].purpose;
                            dic[key].purpose = "L";
                        }

                        quotingInfoList.Add(dic[key]);
                    }
                }

                if (quotingInfoList.Count == 0)
                {
                    MessageBox.Show(string.Format("서버 : {0}, UserId : {1}에 해당하는 유형의 QuotingInfo가 없습니다.", serverIp, userId));
                    return;
                }

                QuotingInfoMultiLoaderListForm listForm = new QuotingInfoMultiLoaderListForm(serverId);
                listForm.SetData(quotingInfoList);
                listForm.SetParentForm(this);
                listForm.Show();
            }
        }
    }
}
