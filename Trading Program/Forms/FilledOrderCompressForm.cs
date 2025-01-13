using CiriData.Manage;
using CommonLib.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Ciri.Forms
{
    public partial class FilledOrderCompressForm : DockContent, IObserver<NotifyCollectionChangedEventArgs>, IObserver<ItemSelectNotifyInfo>
    {
        public class FilledOrderCompressFormInitData
        {
            public string filterText;
            public Dictionary<string, int> widthDict;
            public string serverFilter;

            public FilledOrderCompressFormInitData(string filterText, Dictionary<string, int> widthDict, string serverFilter)
            {
                this.filterText = filterText;
                this.widthDict = widthDict;
                this.serverFilter = serverFilter;
            }
        }
        enum CalcTypeEnum { Sum = 0, Product, AvgPrice };
        CalcTypeEnum calcType = CalcTypeEnum.Sum;
        bool scrollFixFlag = false;
        IDisposable observableDIsposer, itemSelectNotifierUnsubscriber, serverIdUnsubscriber;
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        // key: serverId, value: orderId_price/pos
        ConcurrentDictionary<string, ConcurrentDictionary<String, List<JToken>>> FilledOrderDic = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<JToken>>>();

        string serverId = "전체";
        public FilledOrderCompressForm()
        {
            InitializeComponent();
        }

        private void FilledOrdersForm_Load(object sender, EventArgs e)
        {
            observableDIsposer = FilledOrderManager.SubscribeToDailyFilledOrderDictionary(this);
            itemSelectNotifierUnsubscriber = ItemSelectNotifier.Instance.Subscribe(this);
            serverIdUnsubscriber = ServerManager.Instance.SubscribeToServerIdCollection(this);

            FilledOrderManager.dgvSinglePosition = dgvFilledOrder;
            dgvFilledOrder.InitLater();
            dgvFilledOrder.UpdateView(String.Empty);
            dgvFilledOrder.VirtualMode = true;
            timer.Interval = 1000;
            timer.Tick += new EventHandler(gridUpdater);
            timer.Start();
            comboBoxServerUpdater();
        }
        public void comboBoxServerUpdater()
        {
            if(serverId == null)
                serverId = "전체";
            comboBoxServer.Items.Clear();
            comboBoxServer.Items.Add("전체");
            foreach (string sId in ServerManager.Instance.GetSeverIdList())
            {
                comboBoxServer.Items.Add(sId);
            }
            comboBoxServer.SelectedIndex = comboBoxServer.Items.IndexOf(serverId);
            if(comboBoxServer.SelectedIndex < 0)
            {
                comboBoxServer.Text = serverId;
            }
            MakeFilter();
        }
        private void comboBoxServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            serverId = comboBoxServer.SelectedItem.ToString();
            MakeFilter();
        }

        public void gridUpdater(object sender, EventArgs eArgs)
        {
            dgvFilledOrder.updateViewInUIThread();
        }
        private void dgvFilledOrder_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            // 서버, 시간, 개수, bookCode, isinCode, 종목명, purpose, 수량, 체결가, 주문번호, 체결번호, contractType
            string selectedIsinCode = (string)dgvFilledOrder.Rows[e.RowIndex].Cells[4].Value;
            if (selectedIsinCode == null)
            {
                return;
            }
            int count = int.Parse((string)dgvFilledOrder.Rows[e.RowIndex].Cells[2].Value);
            if (e.Button == MouseButtons.Left)
            {
                if (IsinCreator.IsMini(selectedIsinCode))
                {
                    selectedIsinCode = IsinCreator.getK200ProductOf(selectedIsinCode);
                }

                ItemSelectNotifier.Instance.SelectItem(selectedIsinCode, this.GetType().Name, new string[] { "StockAndSFQuoter", "StockOptionsQuoter", "StockFuturesQuoter", "ManualOrder", "DailyPositionForm" }, this, "");
            }


            if (count > 1)
            {
                string orderId = (string)dgvFilledOrder.Rows[e.RowIndex].Cells[9].Value;
                string price = dgvFilledOrder.Rows[e.RowIndex].Cells[8].Value.ToString();
                string key = orderId + "_" + price;

                string serverId = (string)dgvFilledOrder.Rows[e.RowIndex].Cells[0].Value;

                ConcurrentDictionary<string, List<JToken>> innerDic;
                List<JToken> list = null;
                if (FilledOrderDic.TryGetValue(serverId, out innerDic)) innerDic.TryGetValue(key, out list);

                if (list != null)
                {
                    FilledOrderCompressMiniForm form = FilledOrderCompressMiniForm.Instance;
                    form.LoadData(serverId, list);
                }
            }
        }

        private void dgvFilledOrder_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            // 서버, 시간, 개수, bookCode, isinCode, 종목명, purpose, 수량, 체결가, 주문번호, 체결번호, contractType
            int count = int.Parse((string)dgvFilledOrder.Rows[e.RowIndex].Cells[2].Value);

            if (count > 1)
            {
                string orderId = (string)dgvFilledOrder.Rows[e.RowIndex].Cells[9].Value;
                string price = dgvFilledOrder.Rows[e.RowIndex].Cells[8].Value.ToString();
                string key = orderId + "_" + price;

                string serverId = (string)dgvFilledOrder.Rows[e.RowIndex].Cells[0].Value;

                ConcurrentDictionary<string, List<JToken>> innerDic;
                List<JToken> list = null;
                if (FilledOrderDic.TryGetValue(serverId, out innerDic)) innerDic.TryGetValue(key, out list);

                if (list != null)
                {
                    FilledOrderCompressMiniForm form = FilledOrderCompressMiniForm.Instance;
                    form.LoadData(serverId, list);
                }
            }
        }

        private void dgvFilledOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (dgvFilledOrder.CurrentCell == null)
                return;

            int currentRowIndex = dgvFilledOrder.CurrentCell.OwningRow.Index;
            int currentColIndex = dgvFilledOrder.CurrentCell.OwningColumn.Index;
            //선택된 셀이 없는 경우 이벤트 발생 X
            if (currentRowIndex < 0)
                return;

            if (e.KeyCode == Keys.F)
            {
                String selectedStr = (string)dgvFilledOrder.Rows[currentRowIndex].Cells[currentColIndex].Value;
                if (selectedStr == null)
                {
                    return;
                }

                if (!String.IsNullOrEmpty(textBoxCodeFilter.Text))
                    textBoxCodeFilter.AppendText(",");

                textBoxCodeFilter.AppendText(selectedStr);
            }
        }

        private void MakeFilter()
        {
            string serverFilter = GetServerFilter();
            string codeFilter = GetCodeFilter();
            string filter = string.IsNullOrEmpty(serverFilter) ? codeFilter : (string.IsNullOrEmpty(codeFilter) ? serverFilter : $"{serverFilter} AND {codeFilter}");
            dgvFilledOrder.ApplyFilter(filter);

        }

        private string GetServerFilter()
        {
            string filter = comboBoxServer.Text;
            if(comboBoxServer.SelectedIndex != -1)
                filter = comboBoxServer.SelectedItem.ToString();
            if (filter == "전체")
                filter = "";
            else
            {
                string filterString;
                filterString = string.Format("(서버 like '%{0}%')", filter);
                filter = filterString;
            }
            return filter;
        }

        private string GetCodeFilter()
        {
            string filter = textBoxCodeFilter.Text;
            if (filter.Trim() == "")
                filter = "";
            else
            {
                string filterString;
                if (!filter.Contains(","))
                {
                    if (filter[0].Equals('~'))
                    {
                        filterString = string.Format("NOT (isinCode like '%{0}%' or 종목명 like '%{0}%' or 북코드 like '%{0}%')", filter.Substring(1));
                    }
                    else
                    {
                        filterString = string.Format("(isinCode like '%{0}%' or 종목명 like '%{0}%' or 북코드 like '%{0}%')", filter);
                    }
                }
                else
                {
                    string[] split = filter.Split(',');

                    if (split[0].Length != 0)
                    {
                        if (split[0][0].Equals('~'))
                        {
                            filterString = string.Format("(NOT (isinCode like '%{0}%' or 종목명 like '%{0}%' or 북코드 like '%{0}%'))", split[0].Substring(1));
                        }
                        else
                        {
                            filterString = string.Format("(isinCode like '%{0}%' or 종목명 like '%{0}%' or 북코드 like '%{0}%')", split[0]);
                        }
                    }
                    else
                    {
                        filterString = "";
                    }

                    for (int i = 1; i < split.Length; i++)
                    {
                        string filterText = split[i];

                        if (filterText.Length == 0)
                            continue;

                        if (filterText[0].Equals('~'))
                        {
                            filterString = filterString + string.Format("AND (NOT (isinCode like '%{0}%' or 종목명 like '%{0}%' or 북코드 like '%{0}%'))", filterText.Substring(1));
                        }
                        else
                        {
                            filterString = filterString + string.Format("AND (isinCode like '%{0}%' or 종목명 like '%{0}%' or 북코드 like '%{0}%')", filterText);
                        }
                    }
                }
                filter = filterString;
            }
            return filter;
        }
        private void textBoxCodeFilter_TextChanged(object sender, EventArgs e)
        {
            MakeFilter();
        }
        private void UpdateCalcResult()
        {
            double nextNumber = 0;
            double sumNumbers = 0;
            List<Tuple<int, int, string>> selectedCellList = new List<Tuple<int, int, string>>();
            int tableWidth = dgvFilledOrder.Columns.Count;
            int tableHeight = dgvFilledOrder.Rows.Count;
            labelCalc.Text = calcType.ToString() + "(로드중)";
            System.Diagnostics.Debug.WriteLine(string.Format("[{0}] UI->local 시작", DateTime.Now.ToString()));
            //UI data -> local data
            if (tableWidth * tableHeight != dgvFilledOrder.SelectedCells.Count)
            {
                for (int i = 0; i < dgvFilledOrder.SelectedCells.Count; i++)
                {
                    Tuple<int, int, string> o = new Tuple<int, int, string>(dgvFilledOrder.SelectedCells[i].RowIndex, dgvFilledOrder.SelectedCells[i].ColumnIndex, dgvFilledOrder.SelectedCells[i].Value.ToString());
                    selectedCellList.Add(o);
                }
            }
            else
            {
                for (int i = 0; i < tableHeight; ++i)
                {
                    for (int j = 0; j < tableWidth; ++j)
                    {
                        Tuple<int, int, string> o = new Tuple<int, int, string>(i, j, dgvFilledOrder[j, i].Value.ToString());
                        selectedCellList.Add(o);
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine(string.Format("[{0}] UI->local 끝", DateTime.Now.ToString()));
            labelCalc.Text = calcType.ToString() + "(계산중)";
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler((obj, arg) =>
            {
                string resultString = "";
                if (calcType == CalcTypeEnum.Sum)
                {
                    //calc sum
                    for (int i = 0; i < selectedCellList.Count; i++)
                    {
                        if (double.TryParse(selectedCellList[i].Item3, out nextNumber))
                            sumNumbers += nextNumber;
                    }
                    resultString = sumNumbers.ToString("#,#.####");
                }
                else if (calcType == CalcTypeEnum.Product)
                {
                    //calc product
                    Dictionary<int, double> productResult = new Dictionary<int, double>();
                    for (int i = 0; i < selectedCellList.Count; i++)
                    {
                        int rowIdx = selectedCellList[i].Item1;
                        double value;
                        double.TryParse(selectedCellList[i].Item3, out value);
                        if (productResult.ContainsKey(rowIdx))
                        {
                            productResult[rowIdx] *= value;
                        }
                        else
                        {
                            productResult.Add(rowIdx, value);
                        }
                    }
                    resultString = productResult.Values.Sum().ToString("#,#.####");
                }
                else if (calcType == CalcTypeEnum.AvgPrice)
                {
                    //2xN 배열이 선택되었을 때에만 계산함.
                    Dictionary<int, double> productResult = new Dictionary<int, double>();
                    SortedSet<int> indexSet = new SortedSet<int>();
                    for (int i = 0; i < selectedCellList.Count; i++)
                    {
                        int rowIdx = selectedCellList[i].Item1;
                        int colIdx = selectedCellList[i].Item2;
                        //1x1 array idx
                        int flattenedIdx = rowIdx * tableWidth + colIdx;
                        indexSet.Add(flattenedIdx);
                        double value;
                        double.TryParse(selectedCellList[i].Item3, out value);
                        productResult.Add(flattenedIdx, value);
                    }
                    int firstIdx = indexSet.First();
                    double sumAmtXPrice = 0, sumAmt = 0;
                    bool errorFlag = false;
                    for (int i = 0; i < selectedCellList.Count / 2; ++i)
                    {
                        try
                        {
                            double amt = productResult[firstIdx + tableWidth * i];
                            double price = productResult[firstIdx + tableWidth * i + 1];
                            sumAmtXPrice += amt * price;
                            sumAmt += amt;
                        }
                        catch (KeyNotFoundException)
                        {
                            //선택된 셀이 2xN 배열이 아니라면 에러
                            errorFlag = true;
                            break;
                        }
                    }
                    if (!errorFlag)
                        resultString = (sumAmtXPrice / sumAmt).ToString("#,#.####");
                    else
                        resultString = "error";
                }


                textBoxCalcResult.InvokeIfRequired(control => { control.Text = resultString; labelCalc.Text = calcType.ToString(); });
            }
            );
            bw.RunWorkerAsync();
        }
        private void dgvFilledOrder_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCalcResult();
        }


        private void labelCalc_Click(object sender, EventArgs e)
        {
            calcType = (CalcTypeEnum)((int)(calcType + 1) % 3);
            labelCalc.Text = calcType.ToString();
            dgvFilledOrder_SelectionChanged(null, null);
        }
        DateTime lastDgvFilledOrderTime = DateTime.MinValue;

        private void dgvFilledOrder_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (scrollFixFlag == false)
            {
                dgvFilledOrder.FirstDisplayedScrollingRowIndex = 0;
            }
            //너무 잦은 업데이트로 인한 부하 방지
            if ((DateTime.Now - lastDgvFilledOrderTime).Seconds > 4)
            {
                labelFillCount.Text = "체결 : " + dgvFilledOrder.GetTotalFillCount().ToString();
                //update TradingPnL
                UpdateTradingPnL();
                lastDgvFilledOrderTime = DateTime.Now;
            }
        }
        private void UpdateTradingPnL()
        {
            //update TradingPnL
            double netAmt = dgvFilledOrder.GetNetAmount();
            double arbNetAmt = 0;
            double.TryParse(textBoxArbNetAmount.Text, out arbNetAmt);
            double pnl = arbNetAmt - netAmt;
            textBoxTradingPnL.Text = pnl.ToString("#,#");
        }
        private void checkBoxScrollFix_CheckedChanged(object sender, EventArgs e)
        {
            scrollFixFlag = checkBoxScrollFix.Checked;
        }
        private void UpdateFilledOrderData(string serverId, JToken pos)
        {
            string orderId = pos["orderId"].ToString();
            string price = pos["price"].ToString();
            string key = orderId + "_" + price;

            var innerDic = FilledOrderDic.GetOrAdd(serverId, new ConcurrentDictionary<string, List<JToken>>());
            var list = innerDic.GetOrAdd(key, new List<JToken>());

            if (list.Count == 0)
            {
                list.Add(pos);
                dgvFilledOrder.UpdateFilledOrderData(serverId, pos);
            }
            else
            {
                list.Add(pos);
                dgvFilledOrder.UpdateExistFilledOrderData(serverId, pos);
            }
        }
        public void OnNext(NotifyCollectionChangedEventArgs value)
        {
            if (value.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in value.NewItems)
                {
                    if (item is KeyValuePair<string, JToken> kv)
                    {
                        if (kv.Value != null)
                            UpdateFilledOrderData(kv.Key, kv.Value);
                    }
                    if (item is string serverId)
                    {
                        if (!string.IsNullOrEmpty(serverId))
                            comboBoxServerUpdater();
                    }
                }
            }
            else if (value.Action == NotifyCollectionChangedAction.Remove)
            {
                if (value.OldItems[0] is KeyValuePair<string, JToken> kv)
                {
                    if (kv.Key != null)
                    {
                        dgvFilledOrder.ClearGridView(kv.Key);
                        var innerDic = FilledOrderDic.GetOrAdd(kv.Key, new ConcurrentDictionary<string, List<JToken>>());
                        innerDic.Clear();
                    }

                }

            }
        }

        public void OnNext(ItemSelectNotifyInfo value)
        {
            if (checkBoxFilterFix.Checked)
                return;
            string isinCode = value.itemIsinCode;
            string underlyingIsinCode = ItemMaster.Instance.GetUnderlyingIsinCode(isinCode);
            string itemName = ItemMaster.Instance.GetProdNameWithIsinCode(isinCode);
            if (value.senderName.Contains("Option"))
            {
                string underlyingName = itemName.Split(' ')[0];
                if (underlyingName.Contains("코스피"))
                    underlyingName = "KOSPI_OPT";
                else if (underlyingName.Contains("코스닥150"))
                    underlyingName = "KOSDAQ_OPT";
                else
                {
                    itemName = ItemMaster.Instance.GetProdNameWithIsinCode(underlyingIsinCode);
                    underlyingName = itemName.Split(' ')[0];
                    underlyingName += "_OPT";

                }
                textBoxCodeFilter.Text = underlyingName;
            }
            else
                textBoxCodeFilter.Text = itemName;
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            //throw new NotImplementedException();
            //do nothing
        }

        private void FilledOrderCompressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            observableDIsposer.Dispose();
            itemSelectNotifierUnsubscriber.Dispose();
            serverIdUnsubscriber.Dispose();
            timer.Stop();
        }

        private void textBoxArbNetAmount_TextChanged(object sender, EventArgs e)
        {
            UpdateTradingPnL();
        }

        private FilledOrderCompressFormInitData GetInitData()
        {
            var widthDict = new Dictionary<string, int>();

            foreach (DataGridViewColumn col in dgvFilledOrder.Columns)
                widthDict.Add(col.Name, col.Width);

            return new FilledOrderCompressFormInitData(textBoxCodeFilter.Text, widthDict, comboBoxServer.Text);
        }

        protected override string GetPersistString()
        {
            return this.GetType().ToString() + "|" + JsonConvert.SerializeObject(GetInitData());
        }


        public static IDockContent createFromJson(string json)
        {
            FilledOrderCompressFormInitData data = JsonConvert.DeserializeObject<FilledOrderCompressFormInitData>(json);
            var q = new FilledOrderCompressForm();
            q.textBoxCodeFilter.Text = data.filterText;
            if(data.serverFilter != null)
                q.serverId = data.serverFilter;
            if (data.widthDict != null)
            {
                var cols = q.dgvFilledOrder.Columns;
                foreach (var key in data.widthDict.Keys)
                {
                    if (cols.Contains(key))
                        cols[key].Width = data.widthDict[key];
                }
            }
            return q;
        }



        private void buttonClear_Click(object sender, EventArgs e)
        {
            string textBoxStr = textBoxCodeFilter.Text;
            int index = textBoxStr.LastIndexOf(",");
            if (index < 0)
            {
                textBoxCodeFilter.Clear();
                return;
            }

            textBoxStr = textBoxStr.Substring(0, index);
            textBoxCodeFilter.Text = textBoxStr;
        }
    }
}
