using Ciri.Util;
using CiriData.Manage;
using CommonLib.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Ciri.Forms
{
    public partial class IndexFuturesQuoterStarter : Form
    {
        DockPanel panel;
        Dictionary<string, string> categoryDic;
        string serverId;
        ServerInfoManager serverInfoManager;

        JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
        };
        public IndexFuturesQuoterStarter(string serverId)
        {
            this.serverId = serverId;
            this.Text += "/" + serverId;
            InitializeComponent();
            serverInfoManager = ServerManager.Instance.GetServerInfoManagerFromServerId(serverId);
            InitCategoryList();
            InitComboBoxStyle(comboBoxSelectStyle);

            datalabel.Text = "";
        }

        public void SetDockPanel(DockPanel panel)
        {
            this.panel = panel;
        }
        private void InitCategoryList()
        {
            categoryDic = DBUtil.Instance.GetIFCategoryDic();

            comboBoxCategory.Items.Clear();
            comboBoxCategory.Items.Add("없음");

            foreach (string categoryName in categoryDic.Keys)
            {
                comboBoxCategory.Items.Add(categoryName);
            }

            comboBoxCategory.SelectedIndex = 0;
        }

        private void InitComboBoxStyle(ComboBox comboBox)
        {
            SortedSet<string> styleSet = new SortedSet<string>();

            foreach (var uid in UnderlyingID.GetIndexUnderlyingIDList())
            {
                var fi = ItemMaster.Instance.GetFuturesInstrumentByOrderWithUID(uid.uid, 0);
                if (fi == null)
                    return;

                string isinCode = fi.isinCode;

                foreach (string purpose in serverInfoManager.KrxdbInfoManager.GetPurposeList(isinCode))
                {
                    foreach (string style in serverInfoManager.KrxdbInfoManager.GetStyleList(isinCode, purpose))
                    {
                        styleSet.Add(style);
                    }
                }
            }

            comboBox.Items.Clear();
            comboBox.Items.Add("설정 안 함");
            comboBox.SelectedIndex = 0;

            if (styleSet.Count == 0)
            {
                return;
            }
            else
            {
                foreach (string purpose in styleSet)
                    comboBox.Items.Add(purpose);

                comboBox.DropDownWidth = UIUtil.DropDownWidth(comboBox);
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            string selectCategory = comboBoxCategory.Items[comboBoxCategory.SelectedIndex].ToString();

            if (panel == null)
            {
                MessageBox.Show("이 객체를 초기화할 때 Panel을 설정하지 않았습니다. StockQuoter2는 실행되지 않습니다.");
                this.Close();
            }

            bool spreadMode = checkBoxSpreadMode.Checked;

            if (selectCategory == "없음")
            {
                IndexFuturesQuoter f = new IndexFuturesQuoter(serverId);
                f.Show(panel);
                f.DockState = DockState.Document;
            }
            else
            {
                string json;

                if (!categoryDic.TryGetValue(selectCategory, out json))
                {
                    MessageBox.Show("해당하는 분류가 없습니다.", "오류");
                    this.Close();
                }

                HashSet<string> uidPurposeSet = JsonConvert.DeserializeObject<HashSet<string>>(json, serializerSettings);

                string style = comboBoxSelectStyle.Text;
                if (style == "설정 안 함")
                    style = null;

                IndexFuturesQuoter f = new IndexFuturesQuoter(serverId, selectCategory, style, uidPurposeSet, spreadMode);

                f.Show(panel);
                f.DockState = DockState.Document;
            }

            this.Close();
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectCategory = comboBoxCategory.Items[comboBoxCategory.SelectedIndex].ToString();

            if (selectCategory == "없음")
            {
                datalabel.Text = "";
            }

            string json;

            if (!categoryDic.TryGetValue(selectCategory, out json))
            {
                return;
            }

            HashSet<string> uidPurposeSet = JsonConvert.DeserializeObject<HashSet<string>>(json, serializerSettings);
            int count = uidPurposeSet.Count;

            datalabel.Text = "종목 개수 : " + count + "개";
        }

        private void StockQuoter2Starter_Load(object sender, EventArgs e)
        {

        }
    }
}
