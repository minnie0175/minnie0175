using Ciri.COms;
using Ciri.Properties;
using Ciri.Util;
using CiriData.Manage;
using CommonLib.Connection;
using CommonLib.Manage;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using WeifenLuo.WinFormsUI.Docking;

namespace Ciri.Forms
{
    public partial class ConnectionSelect2 : DockContent
    {

        #region SingletonPattern

        private static readonly Lazy<ConnectionSelect2> instance = new Lazy<ConnectionSelect2>(() => new ConnectionSelect2());

        public static ConnectionSelect2 Instance { get { return instance.Value; } }

        #endregion SingletonPattern

        private System.Windows.Forms.Timer serverStateCheckTimer = new System.Windows.Forms.Timer();

        private bool isUpdatingUI = false;
        private int idx = 2;
        private ConnectionSelect2()
        {
            InitializeComponent();
            this.HideOnClose = true;
            serverStateCheckTimer.Tick += T_Tick;
            serverStateCheckTimer.Interval = 5000;
            serverStateCheckTimer.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            int cnt = 0;
            //check server state
            foreach(ServerInfoManager serverInfoManager in ServerManager.Instance.GetServerInfoManagerList())
            {
                if (serverInfoManager.IsActive())
                {
                    cnt++;
                }
            }
            if(cnt != 0 && cnt == ServerManager.Instance.GetServerInfoManagerList().Count) serverStateCheckTimer.Stop();
        }

        public void StartServerStateChecker()
        {
            serverStateCheckTimer.Start();
        }

        private void SetSiseIpAndPort()
        {
            if (MarketDataCrawler.Instance.IsInitialized())
            {
                string fullUri = MarketDataCrawler.Instance.GetUri().Substring(5);
                string uri = fullUri.Substring(0, fullUri.IndexOf(":"));
                string port = fullUri.Substring(fullUri.IndexOf(":")+1, fullUri.IndexOf("/")-fullUri.IndexOf(":")-1);
                if (MarketDataCrawler.Instance.GetWebSocketState() == WebSocketSharp.WebSocketState.Open)
                {
                    comboBoxSiseServerIP.Text = uri;
                    comboBoxSisePort.Text = port;
                }
            }

            return;
        }

        private void ConnectSise(string siseIP, string sisePort)
        {
            int sisePortInt = int.Parse(sisePort);
            MarketDataCrawler.Instance.Init(siseIP, sisePortInt, "LiveSiseMsg", new KrxTcpHandler());
            if (!MarketDataCrawler.Instance.Connect())
            {
                MessageBox.Show(this, "시세 서버 연결 실패", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(this, "시세 서버 연결 성공", "Connection Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private bool CheckSiseConnection()
        {
            bool isConnected = false;
            if (MarketDataCrawler.Instance.IsInitialized())
            {
                if (MarketDataCrawler.Instance.GetWebSocketState() == WebSocketSharp.WebSocketState.Open)
                    isConnected = true;
            }
            return isConnected;
        }

        public void ChangeOAColor(string serverId, string state)
        {
            foreach(Control control in OAPanel.Controls)
            {
                if(control is ServerPanel panel)
                {
                    if(panel.ServerId == serverId)
                    {
                        Color color = Color.White;
                        switch (state)
                        {
                            case "ACTIVE":
                                color = Color.YellowGreen;
                                break;
                            case "INACTIVE":
                                color = Color.Yellow;
                                break;
                            case "DISCONNECTED":
                                color = Color.Tomato;
                                break;
                        }
                        panel.UpdateColors(color);
                        break;
                    }
                }
            }
        }

        public void ConfirmServerPanel(string serverId)
        {
            foreach (Control control in OAPanel.Controls)
            {
                if (control is ServerPanel panel)
                {
                    if (panel.ServerId == serverId)
                    {
                        panel.ConfirmServer();
                        break;
                    }
                }
            }
        }

        public void ChangeSiseColor()
        {
            Panel panel = OASisePanel;

            Color newColor = Color.White;
            if (MarketDataCrawler.Instance.IsInitialized())
            {
                if(CheckSiseConnection()) newColor = Color.YellowGreen;
                else newColor = Color.Tomato;
            }

            panel.BackColor = newColor;
            foreach (Control control in panel.Controls)
            {
                control.BackColor = newColor;
            }
        }


        private void ConnectionSelect2_Load(object sender, EventArgs e)
        {
            //comboBoxCOmsSiseServerIP.SelectedIndex = 0;
            comboBoxSiseServerIP.SelectedIndex = 0;
            comboBoxSisePort.SelectedIndex = 0;
        }


        private void buttonConnectSise_Click(object sender, EventArgs e)
        {
            string siseIP = comboBoxSiseServerIP.Text;
            string sisePort = comboBoxSisePort.Text;
            if (siseIP == null || sisePort == null) return;
            ConnectSise(siseIP, sisePort);
            ChangeSiseColor();
        }


        private void buttonOA_SiseReconnect_Click(object sender, EventArgs e)
        {
            if (MarketDataCrawler.Instance.Reconnect())
            {
                ChangeSiseColor();
                MessageBox.Show("시세 서버 재연결 성공");
            }
        }

        private void buttonCOms_SiseReconnect_Click(object sender, EventArgs e)
        {
            if (MarketDataCrawler.Instance.Reconnect())
            {
                ChangeSiseColor();
                MessageBox.Show("시세 서버 재연결 성공");
            }
        }

        private void buttonConnectComs_Click(object sender, EventArgs e)
        {
            COmsManager.Instance.Init();
            //ChangeColor(COmsSisePanel, isConnected);
        }


        private void buttonAdd_Click(object sender, EventArgs e)
        {

            if (isUpdatingUI) return;
            try
            {
                isUpdatingUI = true;

                string serverId = "CUSTOM" + idx;
                ServerPanel panel = new ServerPanel
                {
                    ServerId = serverId
                };
                OAPanel.RowCount += 1;
                OAPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                OAPanel.Controls.Add(panel, 0, OAPanel.RowCount - 1);

            }
            finally
            {
                isUpdatingUI = false;
                idx++;
            }

        }
    }
}
