using Ciri.COms;
using Ciri.Forms;
using Ciri.Properties;
using CiriData.Data;
using CiriData.Manage;
using CommonLib.Connection;
using CommonLib.Enums;
using CommonLib.Forms;
using CommonLib.GridView;
using CommonLib.Interface;
using CommonLib.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Windows.Threading;
using TenTec.Windows.iGridLib;

/**
 * grid 만들고, 여기 static 으로 추가하고, container 폼에서 assign 해주고, update 매세지 구현해주고, LpPacketHandler에서 trCode 분기
 */
namespace Ciri.Util
{
    public class ServerInfoManager : IServerInfoManager
    {
        public string serverState = "INACTIVE";
        public string mainFormTitle = "";
        public TcpClientWithHB clientConn;

        public ConcurrentDictionary<string, int> initialSpreadDic = new ConcurrentDictionary<string, int>();
        //public ConcurrentDictionary<string, LinkerInfo> linkerDic = new ConcurrentDictionary<string, LinkerInfo>();

        public KRXDBQuotingInfoManager KrxdbInfoManager = new KRXDBQuotingInfoManager();
        public CiriAndOAQuotingInfoManager ciriAndOAInfoManager = new CiriAndOAQuotingInfoManager();

        public ConcurrentDictionary<string, SynchronizedCollection<string>> codeToMonitorTable = new ConcurrentDictionary<string, SynchronizedCollection<string>>();

        public DateTime quotingPurposeDicUpdateDate { get; set; }
        public DateTime quotingPurposeDicUpdateTime { get; set; }

        public string serverId { get; private set; }

        public string serverIp;

        static bool updateAvailable = true;

        public ServerInfoManager() { }

        public ServerInfoManager(string serverId)
        {
            this.serverId = serverId;
            this.serverIp = ServerManager.Instance.GetIpFromServerId(serverId);
        }

        public void Connect(string port)
        {
            int portInt = int.Parse(port);

            KrxTcpHandler handler = new KrxTcpHandler();
            if (!Settings.Default.IsHBEnabled)
                MessageBox.Show("TCP 연결에 대한 Heartbeat를 체크하지 않습니다. 디버깅 용도로만 사용하세요.");

            clientConn = new TcpClientWithHB(serverIp, portInt, handler, Dispatcher.CurrentDispatcher, OnServerDisconnect, Settings.Default.IsHBEnabled || !Settings.Default.IsDev);
            if (!clientConn.connect())
            {
                MessageBox.Show("LP 서버 연결 실패", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            OnServerConnect();
            ConnectionSelect2.Instance.InvokeIfRequired((c) =>
            {
                ConnectionSelect2.Instance.ConfirmServerPanel(serverId);
            });
            clientConn.startListen();
            LoadQuotingPurposeDic();
            return;
        }

        public void Reconnect()
        {
            if (clientConn.tryReconnect())
            {
                int cnt = FilledOrderManager.DailyFilledOrderCount(serverId);
                if (cnt != 0)
                {
                    string msg = serverId + "서버에 "+ cnt + "개의 체결 내역이 존재합니다. 체결 내역을 초기화하시겠습니까?";
                    DialogResult dr = MessageBox.Show(msg, "경고", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                        FilledOrderManager.ClearDailyFilledOrder(serverId);
                    
                }
                clientConn.startListen();
                OnServerConnect();
            }
        }

        public void Activate()
        {
            string json = JsonConvert.SerializeObject(new { trCode = "ServerState", action = "SetActive" });
            SendToServer(json);
        }

        public bool IsActive()
        {
            if (serverState == "ACTIVE")
            {
                return true;
            }
            string json = JsonConvert.SerializeObject(new { trCode = "ServerState", action = "GetServerState" });
            SendToServer(json);
            return false;
        }

        public void SendToServer(string packet)
        {
            clientConn.writeWithLength(packet);
        }


        public void UpdateServerState(string state)
        {
            // GetServerState할때마다 서버에서 정보를 보내주고, 서버로부터 state 정보를 받을 때 호출됨
            serverState = state;
            ConnectionSelect2.Instance.InvokeIfRequired((c) =>
            {
                ConnectionSelect2.Instance.ChangeOAColor(serverId, serverState);
            });
            //mainForm.UpdateTitle("OAState", serverState);
        }

        public void OnServerConnect()
        {
            CiriForm.Instance.OANum++;
            int connectedServers = CiriForm.Instance.OANum;
            CiriForm.Instance.InvokeIfRequired((c) =>
            {
                CiriForm.Instance.UpdateTitle("OAs", connectedServers.ToString());
            });
            ConnectionSelect2.Instance.InvokeIfRequired((c) =>
            {
                ConnectionSelect2.Instance.ChangeOAColor(serverId, serverState);
                ConnectionSelect2.Instance.StartServerStateChecker();
            });
        }

        public void OnServerDisconnect()
        {
            Debug.WriteLine("Trading Server disconnected");
            serverState = "DISCONNECTED";
            CiriForm.Instance.OANum--;
            int connectedServers = CiriForm.Instance.OANum;
            CiriForm.Instance.InvokeIfRequired((c) =>
            {
                CiriForm.Instance.UpdateTitle("OAs", connectedServers.ToString());
                //mainForm.UpdateTitle("OAState", serverState);
                //mainForm.StartServerStateChecker();
            });

            ConnectionSelect2.Instance.InvokeIfRequired((c) =>
            {
                ConnectionSelect2.Instance.ChangeOAColor(serverId, serverState);
                ConnectionSelect2.Instance.StartServerStateChecker();
            });

        }

        /*
        public void UpdateTitle(JToken connToken)
        {
            string connectionMsg = connToken.Value<string>("Fep");

            String title = serverIp + "  " + connectionMsg;
            mainFormTitle = title;
            //mainForm.SafeInvoke(form => form.Text = title);
            mainForm.UpdateTitle("OA", title);
            mainForm.UpdateTitle("OAState", serverState);
            PopupAlarmForm.Instance.InvokeIfRequired((c) =>
            {
                c.Text = "경고 - " + title;
            });
        }
        */



        public static int GetSpreadTicksFromQuotingInfo(QuotingInfo info)
        {
            int spread = info.askSkew - info.bidSkew;
            return spread;

        }

        public void SetInitialQuotingInfo(QuotingInfo info)
        {
            int spreadTick = GetSpreadTicksFromQuotingInfo(info);
            var key = ciriAndOAInfoManager.GetQuotingInfoKey(info);
            initialSpreadDic.AddOrUpdate(key, spreadTick, (k, oldSpread) => spreadTick);
        }

        /*
        public static void UpdateLinkerInfo(LinkerInfo info)
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("[{0}] {2}/{1}", info.code, info.ba.bidPrice[0], info.ba.askPrice[0]));
            linkerDic.AddOrUpdate(info.codeList[0], info, (id, oldInfo) => info);
        }
        */

        public void SaveQuotingPurposeDic()  // 저장할때 차월물에 대한 QuotingInfo를 추가한다.
        {
            if (KrxdbInfoManager.Save(serverIp, Settings.Default.UserId))
            {
                quotingPurposeDicUpdateDate = DateTimeCenter.Instance.GetToday();
                quotingPurposeDicUpdateTime = DateTimeCenter.Instance.GetNow();
            }
        }

        public void LoadQuotingPurposeDic(string ip)
        {
            KrxdbInfoManager.Load(ip, Settings.Default.UserId, out var updateDate, out var updateTime);
            this.quotingPurposeDicUpdateDate = updateDate;
            this.quotingPurposeDicUpdateTime = updateTime;
        }

        public void LoadQuotingPurposeDic()
        {
            LoadQuotingPurposeDic(serverIp);
        }

        public void CopyQuotingPurposeDic()
        {
            KrxdbInfoManager.UpdateNextFuturesQuotingInfo(serverId);
        }

        public CiriAndOAQuotingInfoManager GetCiriAndOAQuotingInfoManager()
        {
            return ciriAndOAInfoManager;
        }

        public KRXDBQuotingInfoManager GetDBInfoManager()
        {
            return KrxdbInfoManager;
        }

        public SynchronizedCollection<string> GetCTMList(string isinCode)
        {
            SynchronizedCollection<string> CTMList;
            if (codeToMonitorTable.TryGetValue(isinCode, out CTMList))
            {
                return CTMList;
            }
            else
            {
                CTMList = new SynchronizedCollection<string>();
                codeToMonitorTable.TryAdd(isinCode, CTMList);
                return CTMList;
            }
        }

        public void SaveServerQuoterDic(iGrid grid)
        {
            List<int> selectedRowsIndex = IGridUtil.GetSelectedRowIndexList(grid);

            if (selectedRowsIndex.Count == 0)
            {
                MessageBox.Show("선택된 종목이 없습니다.", "오류");
                return;
            }

            DialogResult result = MessageBox.Show(string.Format("서버의 QuoterDic {0}개를 QuotingPurposeDic으로 옮기시겠습니까?", selectedRowsIndex.Count), "알림", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            int skipCount = 0;

            foreach (int index in selectedRowsIndex)
            {
                string isinCode = grid.Rows[index].Cells["종목코드"].Value.ToString();
                string purpose = grid.Rows[index].Cells["Purpose"].Value.ToString();
                QuotingInfo info;

                if (!ciriAndOAInfoManager.GetQuotingInfo(isinCode, purpose, false, out info))
                {
                    skipCount++;
                    continue;
                }

                KrxdbInfoManager.UpdateQuotingInfo(info);
            }

            string tailMsg = "";

            if (skipCount > 0)
            {
                tailMsg = string.Format("\n{0}개를 옮기는데 실패했습니다.", skipCount);
            }

            MessageBox.Show(string.Format("서버의 QuoterDic {0}개가 옮겨졌습니다." + tailMsg, (selectedRowsIndex.Count - skipCount)));
        }
        public ICollection<QuotingInfo> GetAllQuotingInfo()
        {
            return ciriAndOAInfoManager.GetAllQuotingInfo();
        }
        public bool GetQuotingInfoWithoutPurpose(string isinCode, out QuotingInfo outInfo)
        {
            return ciriAndOAInfoManager.GetQuotingInfoWithoutPurpose(isinCode, out outInfo);
        }
        public bool GetNextQuotingInfo(string isinCode, string purpose, bool isAfter, bool passSubPurpose, out QuotingInfo outInfo)
        {
            return ciriAndOAInfoManager.GetNextQuotingInfo(isinCode, purpose, isAfter, passSubPurpose, out outInfo);
        }

        public void UpdateQuotingInfo(QuotingInfo info)
        {
            ////System.Diagnostics.Debug.WriteLine(string.Format("[{0}] {2}/{1}", info.code, info.ba.bidPrice[0], info.ba.askPrice[0]));   
            //var key = ciriAndOAInfoManager.GetQuotingInfoKey(info);
            //if (!initialSpreadDic.ContainsKey(key))
            //{
            //    setInitialQuotingInfo(info);
            //}
            //ciriAndOAInfoManager.UpdateQuotingInfo(info.isinCode, info.purpose, info);
            if (updateAvailable)
                UpdateQuotingInfo(info.isinCode, info.purpose, info);
        }

        public void UpdateQuotingInfo(string isinCode, string purpose, QuotingInfo qi)
        {
            var comsQi = COmsManager.Instance.GetQuotingInfoByIsinCodeAndPurpose(qi.isinCode, qi.purpose);
            if (comsQi != null && (comsQi.serverId != qi.serverId || comsQi.processName != qi.processName))
            {
                //COms에서 제출중인 isincode,purpose가 있는데 동일한 서버/프로세스가 아니라면 무시
                Debug.WriteLine("{0},{1} quotinginfo가 COms에 이미 존재하고 서버/프로세스가 아니므로 업데이트하지 않습니다.");
                return;
            }
            //COms에서 제출중인 isinCode,purpose 호가가 없다면 반영
            var key = ciriAndOAInfoManager.GetQuotingInfoKey(qi);
            if (!initialSpreadDic.ContainsKey(key))
            {
                SetInitialQuotingInfo(qi);
            }
            ciriAndOAInfoManager.UpdateQuotingInfo(isinCode, purpose, qi);
            return;
        }


        public void CreateDefaultStockLPQIs()
        {
            KrxdbInfoManager.CreateDefaultStockLPQIs();
        }

        public bool GetQuotingInfo(string isinCode, string purpose, out QuotingInfo qi)
        {
            return ciriAndOAInfoManager.GetQuotingInfo(isinCode, purpose, false, out qi);
        }

        public bool GetQuotingInfoWithoutSub(string isinCode, string purpose, out QuotingInfo qi)
        {
            return ciriAndOAInfoManager.GetQuotingInfo(isinCode, purpose, true, out qi);
        }

        public MultiQuoterState GetMultiQuoterState(string isinCode)
        {
            return ciriAndOAInfoManager.GetMultiQuoterState(isinCode);
        }


        public object GetWorkingInfoCount(string isinCode)
        {
            return ciriAndOAInfoManager.GetWorkingInfoCount(isinCode);
        }

        public string GetQuotingInfoKey(QuotingInfo quoteInfo)
        {
            return ciriAndOAInfoManager.GetQuotingInfoKey(quoteInfo);
        }

        public IEnumerable<string> GetQuoterPurposeList(string isinCode)
        {
            return ciriAndOAInfoManager.GetQuoterPurposeList(isinCode);
        }

        public int GetWorkingQuoterCount()
        {
            return ciriAndOAInfoManager.GetWorkingQuoterCount();
        }

        public int GetQuotingInfoCount()
        {
            return ciriAndOAInfoManager.GetQuotingInfoCount();
        }

        public void GetQuotingInfo(QuotingInfo qi1, out QuotingInfo qi2)
        {
            ciriAndOAInfoManager.GetQuotingInfo(qi1, out qi2);
        }

        public QuotingInfo GetLPCheckInfo(QuotingInfo info)
        {
            return ciriAndOAInfoManager.GetLPCheckInfo(info);
        }

        public QuotingInfo GetWOUpdatedQuotingInfo(QuotingInfo info, HashSet<string> purposeSet = null)
        {
            return ciriAndOAInfoManager.GetWOUpdatedQuotingInfo(info, purposeSet);
        }

        public bool RemoveQuotingInfo(string isinCode, string purpose)
        {
            return ciriAndOAInfoManager.RemoveQuotingInfo(isinCode, purpose);
        }

        public static bool ToggleUpdateStatus()
        {
            return updateAvailable = !updateAvailable;
        }

    }
}