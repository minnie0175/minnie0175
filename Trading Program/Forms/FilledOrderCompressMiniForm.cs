using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using CommonLib.Util;
using MongoDB.Driver.Core.Servers;

namespace Ciri.Forms
{
    public partial class FilledOrderCompressMiniForm : DockContent
    {
        #region SingletonPattern
        private static readonly Lazy<FilledOrderCompressMiniForm> instance = new Lazy<FilledOrderCompressMiniForm>(() => new FilledOrderCompressMiniForm());

        public static FilledOrderCompressMiniForm Instance {  get { return instance.Value; } }

        #endregion SingletonPattern

        private FilledOrderCompressMiniForm()
        {
            InitializeComponent();
        }

        public void LoadData(string serverId, List<JToken> posList)
        {
            dgvFilledOrder.ClearGridView();

            foreach (JToken token in posList)
            {
                dgvFilledOrder.UpdateFilledOrderData(serverId, token);
            }

            dgvFilledOrder.updateViewInUIThread();
        }

        private void FilledOrdersForm_Load(object sender, EventArgs e)
        {           
            dgvFilledOrder.InitLater();
            dgvFilledOrder.UpdateView(String.Empty);
            dgvFilledOrder.VirtualMode = true;
            //timer.Interval = 1000;
            //timer.Tick += new EventHandler(gridUpdater);
            //timer.Start();
        }

        private void dgvFilledOrder_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            // 서버, 시간, 개수, bookCode, isinCode, 종목명, purpose, 수량, 체결가, 주문번호, 체결번호, contractType
            String selectedIsinCode = (string)dgvFilledOrder.Rows[e.RowIndex].Cells[4].Value;
            if (selectedIsinCode == null)
            {
                return;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (IsinCreator.IsMini(selectedIsinCode))
                {
                    selectedIsinCode = IsinCreator.getK200ProductOf(selectedIsinCode);
                }
                
                ItemSelectNotifier.Instance.SelectItem(selectedIsinCode, this.GetType().Name, new string[] { "StockOptionsQuoter", "StockFuturesQuoter", "ManualOrder", "DailyPositionForm"}, this, "");
            }
        }

        private void FilledOrderCompressMiniForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dgvFilledOrder.ClearGridView();
        }
    }
}
