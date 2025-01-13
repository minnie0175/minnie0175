using Ciri;
using Ciri.Interface;
using Ciri.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CiriData.Manage
{
    public class ServerManager
    {

        #region SingletonPattern
        private static readonly Lazy<ServerManager> instance = new Lazy<ServerManager>(() => new ServerManager());

        public static ServerManager Instance { get { return instance.Value; } }

        #endregion SingletonPattern
        // key: serverId, value: ServerInfoManager. 언제나 valid한(연결된 적 있는 서버의) ServerInfoManager만 들어감
        private ConcurrentDictionary<string, ServerInfoManager> ServerIdToServerInfoManager = new ConcurrentDictionary<string, ServerInfoManager>();
        // key: serverId, value: List<IQuoter>
        private ConcurrentDictionary<string, List<IQuoterForm>> ServerIdToQuoterList = new ConcurrentDictionary<string, List<IQuoterForm>>();
        private static ServerInfoManager emptyServerInfoManager = new ServerInfoManager();

        private Dictionary<string, string> IpToServerId = new Dictionary<string, string>()
        {
            {  "192.168.245.117" ,"SEOUL1"},
            { "172.20.131.236" ,"BUSAN1"},
            {  "172.20.131.197" ,"BUSAN2"},
            {  "localhost", "LOCAL" },
        };

        private Dictionary<string, string> ServerIdToIp = new Dictionary<string, string>()
        {
            {"SEOUL1", "192.168.245.117"},
            {"BUSAN1", "172.20.131.236"},
            {"BUSAN2", "172.20.131.197" },
            {"LOCAL", "localhost" },
        };

        private ServerManager()
        {
        }

        public string GetIpFromServerId(string serverId)
        {
            if (ServerIdToIp.TryGetValue(serverId, out string ip))
                return ip;
            else
                return "";
        }

        public string GetServerIdFromIp(string ip)
        {
            if (IpToServerId.TryGetValue(ip, out string serverId))
                return serverId;
            else
                return "";
        }

        // 등록된 모든 서버
        public List<string> GetSeverIdList()
        {
            List<string> serverIdList = new List<string>(IpToServerId.Values);
            return serverIdList;
        }

        // 연결된 적 있는 서버
        public List<ServerInfoManager> GetServerInfoManagerList()
        {
            List<ServerInfoManager> serverInfoManagerList = new List<ServerInfoManager>(ServerIdToServerInfoManager.Values);
            return serverInfoManagerList;
        }

        public string GetFirstConnectedServerId()
        {
            if (ServerIdToServerInfoManager.Count > 0) return ServerIdToServerInfoManager.First().Key;
            return null;
        }

        public ServerInfoManager GetServerInfoManagerFromServerId(string serverId)
        {
            // 없으면 빈 ServerInfoManager 반환
            if (ServerIdToServerInfoManager.TryGetValue(serverId, out ServerInfoManager serverInfoManager))
            {
                return serverInfoManager;
            }
            return emptyServerInfoManager;
        }

        public bool isEmptyServerInfoManager(ServerInfoManager serverInfoManager)
        {
            if (serverInfoManager == emptyServerInfoManager) return true;
            return false;
        }

        public void TryAddNewServer(string ip, string serverId)
        {
            string oldIp = GetIpFromServerId(serverId);

            // 이 id가 등록되어 있음
            if (!string.IsNullOrEmpty(oldIp))
            {
                ServerIdToIp.Remove(serverId);
                IpToServerId.Remove(oldIp);
                ServerIdToServerInfoManager.TryRemove(serverId, out _);
                RemoveServerId(serverId);
            }
            IpToServerId.Add(ip, serverId);
            ServerIdToIp.Add(serverId, ip);
            CiriForm.Instance.UpdateServerList();
            AddServerIdCollection(serverId);
            return;
        }

        public void ConnectOrReconnect(string serverId, string port)
        {
            ServerInfoManager serverInfoManager = GetServerInfoManagerFromServerId(serverId);
            // ServerInfoManager가 깡통이면
            if (isEmptyServerInfoManager(serverInfoManager))
            {
                //생성하고 Connect, QuoterForm에게 알림
                serverInfoManager = new ServerInfoManager(serverId);
                ServerIdToServerInfoManager.TryAdd(serverId, serverInfoManager);
                serverInfoManager.Connect(port);
                ActivateQuoterForm(serverId, serverInfoManager);

            }
            // 유효한 ServerInfoManager면
            else
            {
                // Reconnect
                serverInfoManager.Reconnect();

            }
        }

        public void RegisterQuoter(string serverId, IQuoterForm quoter)
        {
            ServerIdToQuoterList.AddOrUpdate(serverId, new List<IQuoterForm> { quoter },
                (key, existingList) =>
                {
                    existingList.Add(quoter);
                    return existingList;
                });
        }

        public void ActivateQuoterForm(string serverId, ServerInfoManager sim)
        {
            if (ServerIdToQuoterList.TryRemove(serverId, out var quoterList))
            {
                foreach (var quoter in quoterList)
                {
                    quoter.InitQuoter(sim);
                }
            }
        }

        public void UnregisterQuoter(string serverId, IQuoterForm quoter)
        {
            if (ServerIdToQuoterList.TryGetValue(serverId, out var quoterList))
            {
                quoterList.Remove(quoter);
            }
        }

        readonly Swordfish.NET.Collections.ConcurrentObservableCollection<string> serverIdCollection = new Swordfish.NET.Collections.ConcurrentObservableCollection<string>();
        public IDisposable SubscribeToServerIdCollection(IObserver<NotifyCollectionChangedEventArgs> observer)
        {
            return serverIdCollection.Subscribe(observer);
        }
        public void AddServerIdCollection(string serverId)
        {
            serverIdCollection.Add(serverId);
        }
        public void RemoveServerId(string serverId)
        {
            serverIdCollection.Remove(serverId);
        }
    }
}
