using Ciri.Properties;
using CiriData.Data;
using CiriData.Enums;
using CiriData.Manage;
using CommonLib.Util;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Ciri.Forms
{
    public partial class QuotingInfoHistoryViewer : Form
    {
        private JsonDiffPatch jdp = new JsonDiffPatch();
        private string serverId;
        private string userId;
        private string isinCode;
        private DateTime curDateTime;
        private QuotingInfo liveInfo;
        private QuotingInfo selectedInfo;
        // TO-DO: 종목에 대해 빈 QuotingInfo 만들기

        private List<(DateTime date, QuotingInfo qinfo)> history;
        private List<(DateTime date, QuotingInfo qinfo)> filteredHistory;

        private int pageSize = 11;  // 한 페이지에 표시할 항목 수
        private int currentPage = 1;  // 현재 페이지
        private int totalPage = 1;  // 총 페이지 수

        private Color color1 = Color.FromArgb(255, 153, 139);
        private Color color2 = Color.FromArgb(170, 210, 137);

        private JObject jObjectqinfo1;
        private JObject jObjectqinfo2;
        private bool quotingInfoChanged = false;

        public QuotingInfoHistoryViewer(string serverId, QuotingInfo info)
        {
            InitializeComponent();
            curDateTime = DateTime.Now;
            liveInfo = info;
            isinCode = info.isinCode;
            this.serverId = serverId;
            selectedInfo = info;

            jObjectqinfo1 = new JObject();
            jObjectqinfo2 = new JObject();
            QuotingProduct pType = QuotingProductFunctions.GetTypeFrom(isinCode);

            // TO-DO: 각 종목이 LP인지 아닌지 체크하는 방법
            quotingInfoControl1.init(pType, isinCode, true);
            quotingInfoControl2.init(pType, isinCode, true);

            string nameAndCode = ItemMaster.Instance.GetProdNameWithIsinCode(isinCode) + "(" + isinCode + ")";
            labelCode.Text = nameAndCode;

            comboBoxServer.Items.Clear();
            foreach (string sId in ServerManager.Instance.GetSeverIdList())
            {
                comboBoxServer.Items.Add(sId);
            }
            comboBoxServer.SelectedIndex = comboBoxServer.Items.IndexOf(serverId);
            textBoxUserId.Text = Settings.Default.UserId;

            labelQuotingInfo1.Text = "";
            labelQuotingInfo1.ForeColor = Color.Red;
            labelQuotingInfo2.Text = "";
            labelQuotingInfo2.ForeColor = Color.Green;

            quotingInfoControl1.BackColor = color1;
            quotingInfoControl2.BackColor = color2;
            buttonLoadQuotingInfo1.BackColor = color1;
            buttonLoadQuotingInfo2.BackColor = color2;

            LoadData();
        }

        private string DateTimeToString(DateTime dateTime)
        {
            if (dateTime == curDateTime)
            {
                return "Live";
            }
            else
            {
                return dateTime.ToString("yyyy-MM-dd");
            }
        }

        public QuotingInfo GetQuotingInfo()
        {
            return selectedInfo;
        }

        private void SetComboBox()
        {
            comboBoxPurpose.Items.Clear();
            comboBoxStyle.Items.Clear();
            comboBoxPurpose.Items.Add("전체");
            comboBoxStyle.Items.Add("전체");
            foreach (var (_, qinfo) in history)
            {
                if (!comboBoxPurpose.Items.Contains(qinfo.purpose))
                    comboBoxPurpose.Items.Add(qinfo.purpose);
                if (!comboBoxStyle.Items.Contains(qinfo.style))
                    comboBoxStyle.Items.Add(qinfo.style);

            }
            comboBoxPurpose.SelectedIndex = 0;
            comboBoxStyle.SelectedIndex = 0;
        }

        private void comboBoxPurpose_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void comboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (comboBoxPurpose.SelectedItem != null && comboBoxStyle.SelectedItem != null)
            {
                string purpose = comboBoxPurpose.SelectedItem.ToString();
                string style = comboBoxStyle.SelectedItem.ToString();
                filteredHistory = history
                    .Where(item => (item.qinfo.purpose == purpose || purpose == "전체") &&
                                   (item.qinfo.style == style || style == "전체"))
                    .OrderByDescending(item => item.date)    // item.date를 내림차순으로 정렬
                    .ThenBy(item => item.qinfo.purpose)
                    .ThenBy(item => item.qinfo.style)
                    .ToList();

                totalPage = (int)Math.Ceiling(filteredHistory.Count / (double)pageSize);
                currentPage = 1;
                dgv_Update();
            }
        }

        private void LoadData()
        {
            serverId = comboBoxServer.Text;
            string serverIp = ServerManager.Instance.GetIpFromServerId(serverId);
            // id가 아니라면 ip가 들어온 것임
            if (string.IsNullOrEmpty(serverIp))
            {
                serverIp = serverId;
            }

            userId = textBoxUserId.Text;
            DBUtil.Instance.GetQuotingPurposeDicHistory(serverIp, userId, isinCode, out List<string> rawHistory);

            history = new List<(DateTime, QuotingInfo)>
            {
                (curDateTime, liveInfo)
            };
            foreach (var data in rawHistory)
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, QuotingInfo>>(data);
                var dateQuotingInfo = dictionary.First();
                DateTime date = DateTime.Parse(dateQuotingInfo.Key);
                history.Add((date, dateQuotingInfo.Value));
            }
            SetComboBox();
            SetQuotingInfo1(liveInfo, DateTimeToString(curDateTime));
            SetQuotingInfo2(liveInfo, DateTimeToString(curDateTime));
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dgv_Update()
        {
            var pageData = GetPageData(currentPage);

            dataGridView1.Rows.Clear();
            foreach (var entry in pageData)
            {
                AddDataGridViewRow(entry);
            }

            if (currentPage == 1)
            {
                buttonPrevious.ForeColor = Color.Gray;
            }
            else
            {
                buttonPrevious.ForeColor = Color.Black;
            }
            if (currentPage == totalPage)
            {
                buttonNext.ForeColor = Color.Gray;
            }
            else
            {
                buttonNext.ForeColor = Color.Black;
            }
        }

        private List<(DateTime date, QuotingInfo qinfo)> GetPageData(int pageNumber)
        {
            return filteredHistory
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private void AddDataGridViewRow((DateTime date, QuotingInfo qinfo) dateQuotingInfo)
        {
            string date = DateTimeToString(dateQuotingInfo.date);

            int rowIndex = dataGridView1.Rows.Add();
            var row = dataGridView1.Rows[rowIndex];

            row.Cells["DateColumn"].Value = date;
            row.Cells["PurposeColumn"].Value = dateQuotingInfo.qinfo.purpose;
            row.Cells["StyleColumn"].Value = dateQuotingInfo.qinfo.style;
            row.Tag = dateQuotingInfo.qinfo;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                QuotingInfo quotingInfo = (QuotingInfo)selectedRow.Tag;
                string date = selectedRow.Cells["DateColumn"].Value.ToString();

                if (e.ColumnIndex == dataGridView1.Columns["buttonSetQuoter1"].Index)
                {
                    SetQuotingInfo1(quotingInfo, date);
                }

                if (e.ColumnIndex == dataGridView1.Columns["buttonSetQuoter2"].Index)
                {
                    SetQuotingInfo2(quotingInfo, date);
                }
            }
        }

        private void SetQuotingInfo1(QuotingInfo qi, string date = null)
        {
            quotingInfoControl1.setQuotingInfo(qi);

            string key = date != null ? $"{date} _ {qi.purpose} _ {qi.style}" : "";
            labelQuotingInfo1.Text = key;
            string qinfo = JsonConvert.SerializeObject(qi);
            jObjectqinfo1 = JObject.Parse(qinfo);
            UpdateTreeView();
        }

        private void SetQuotingInfo2(QuotingInfo qi, string date = null)
        {
            quotingInfoControl2.setQuotingInfo(qi);

            string key = date != null ? $"{date} _ {qi.purpose} _ {qi.style}" : "";
            labelQuotingInfo2.Text = key;
            string qinfo = JsonConvert.SerializeObject(qi);
            jObjectqinfo2 = JObject.Parse(qinfo);
            UpdateTreeView();
        }
        private void LoadDiffTreeView(TreeView treeView, JToken diff)
        {
            treeView.Nodes.Clear();
            DisplayDiff(diff, treeView.Nodes);
            treeView.ExpandAll();
        }

        private void DisplayDiff(JToken diff, TreeNodeCollection nodes)
        {
            if (diff == null)
                return;

            foreach (var property in diff.Children<JProperty>())
            {
                TreeNode node = new TreeNode(property.Name);

                // diff에서 변경된 부분을 색상으로 강조
                if (property.Value.Type == JTokenType.Array && property.Value.Count() == 2)
                {
                    string oldValue = property.Value[0].ToString();
                    if (property.Name == "codeToMonitor") oldValue = ItemMaster.Instance.GetProdNameWithIsinCode(oldValue);
                    if (!string.IsNullOrEmpty(oldValue) && oldValue != "null")
                    {
                        if (!IsJson(oldValue))
                        {
                            // Old 값이 단순 문자열이면 바로 추가
                            node.Nodes.Add(new TreeNode(oldValue)
                            {
                                ForeColor = Color.Red
                            });
                        }
                        else
                        {
                            continue;
                        }
                    }

                    string newValue = property.Value[1].ToString();
                    if (property.Name == "codeToMonitor") newValue = ItemMaster.Instance.GetProdNameWithIsinCode(newValue);
                    if (!string.IsNullOrEmpty(newValue) && newValue != "null")
                    {
                        if (!IsJson(newValue))
                        {
                            // New 값이 단순 문자열이면 바로 추가
                            node.Nodes.Add(new TreeNode(newValue)
                            {
                                ForeColor = Color.Green
                            });
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else if (property.Value.Type == JTokenType.Object)
                {
                    //// TO-DO: array는 어떤식으로 표시하여 비교할지 고민
                    //// 하위 객체가 있을 경우 재귀적으로 처리
                    //// 여기서는 pinfo만!
                    //DisplayDiff(property.Value, node.Nodes);
                    continue;
                }
                else
                {
                    //// TO-DO: array는 어떤식으로 표시하여 비교할지 고민
                    //// 기타 추가된 항목은 기본적으로 초록색으로 표시
                    //// pinfo의 각 항목들 _t: a, _0: [{}, {}]
                    //node.ForeColor = Color.Green;
                    //node.Text += ": " + property.Value.ToString();
                    continue;
                }

                nodes.Add(node);
            }
        }
        private bool IsJson(string input)
        {
            input = input.Trim();
            if ((input.StartsWith("{") && input.EndsWith("}")) || // JSON Object
                (input.StartsWith("[") && input.EndsWith("]")))   // JSON Array
            {
                try
                {
                    var obj = JToken.Parse(input);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        #region originalcode
        //private void DisplayTreeView(TreeView tv, JToken root, string rootName)
        //{
        //    tv.BeginUpdate();
        //    try
        //    {
        //        tv.Nodes.Clear();
        //        var tNode = tv.Nodes[tv.Nodes.Add(new TreeNode(rootName))];
        //        tNode.Tag = root;
        //        AddNode(root, tNode);
        //        // 모든 TreeNode를 확장시
        //        tv.ExpandAll();                 
        //        // 매 4번째 TreeNode를 확장시
        //        //ExpandToLevel(tv.Nodes, 2);
        //        // TreeView의 맨 위쪽으로 포커스 이동(주석 처리시 TreeView의 맨 아래쪽에 포커스가 됨)
        //        tv.Nodes[0].EnsureVisible();
        //    }
        //    finally
        //    {
        //        tv.EndUpdate();
        //    }
        //}

        //private void ExpandToLevel(TreeNodeCollection nodes, int level)
        //{
        //    if (level > 0)
        //    {
        //        foreach (TreeNode node in nodes)
        //        {
        //            node.Expand();
        //            ExpandToLevel(node.Nodes, level - 1);
        //        }
        //    }
        //}

        //private void AddNode(JToken token, TreeNode inTreeNode)
        //{
        //    if (token == null)
        //        return;
        //    if (token is JValue)
        //    {
        //        var value = token.ToString();
        //        if (IsJson(value))
        //        {
        //            var parsedJson = JToken.Parse(value);
        //            AddNode(parsedJson, inTreeNode);
        //        }
        //        else
        //        {
        //            var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(value))];
        //            childNode.Tag = token;
        //        }
        //    }
        //    else if (token is JObject)
        //    {
        //        var obj = (JObject)token;
        //        foreach (var property in obj.Properties())
        //        {
        //            var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
        //            childNode.Tag = property;
        //            AddNode(property.Value, childNode);
        //        }
        //    }
        //    else if (token is JArray)
        //    {
        //        var array = (JArray)token;
        //        for (int i = 0; i < array.Count; i++)
        //        {
        //            var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
        //            childNode.Tag = array[i];
        //            AddNode(array[i], childNode);
        //        }
        //    }
        //    else
        //    {
        //        Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
        //    }
        //}
        #endregion

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPage)
            {
                currentPage++;
                dgv_Update();
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                dgv_Update();
            }
        }

        private JToken GetDiff(JObject leftJson, JObject rightJson)
        {

            // 두 JSON에서 동일한 키 집합을 가지도록 설정 (누락된 키는 빈 문자열로 추가)
            foreach (var key in rightJson.Properties().Select(p => p.Name).ToList())
            {
                if (!leftJson.ContainsKey(key))
                {
                    leftJson[key] = "";
                }
            }

            foreach (var key in leftJson.Properties().Select(p => p.Name).ToList())
            {
                if (!rightJson.ContainsKey(key))
                {
                    rightJson[key] = "";
                }
            }
            return jdp.Diff(leftJson, rightJson);
        }

        private void UpdateTreeView()
        {

            LoadDiffTreeView(treeView1, GetDiff(jObjectqinfo1, jObjectqinfo2));

            string jsonString1 = (string)jObjectqinfo1.SelectToken("jsonString");
            if (string.IsNullOrEmpty(jsonString1)) jsonString1 = "{}";
            string jsonString2 = (string)jObjectqinfo2.SelectToken("jsonString");
            if (string.IsNullOrEmpty(jsonString2)) jsonString2 = "{}";

            JObject leftJson = JObject.Parse(jsonString1);
            JObject rightJson = JObject.Parse(jsonString2);
            LoadDiffTreeView(treeView2, GetDiff(leftJson, rightJson));
        }

        private void buttonLoadQuotingInfo1_Click(object sender, EventArgs e)
        {
            selectedInfo = quotingInfoControl1.getQuotingInfo();
            this.Close();
        }

        private void buttonLoadQuotingInfo2_Click(object sender, EventArgs e)
        {
            selectedInfo = quotingInfoControl2.getQuotingInfo();
            this.Close();
        }

        private void QuotingInfoHistoryViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (selectedInfo != liveInfo)
            {
                DialogResult exitResult = MessageBox.Show("선택한 Quoting Info를 불러오시겠습니까?", "확인", MessageBoxButtons.YesNo);
                if (exitResult == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

            }

        }
    }
}
