using Ciri.Interface;
using CiriData.Manage;
using MongoDB.Driver.Core.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace Ciri.Util
{
    public class QuoterInitiator
    {
        private IQuoterForm quoterForm;
        private string serverId;

        public QuoterInitiator(IQuoterForm quoterForm)
        {
            this.quoterForm = quoterForm;
            this.serverId = quoterForm.serverId;
        }

        public void InitOrRegisterQuoter()
        {
            ServerInfoManager serverInfoManager = ServerManager.Instance.GetServerInfoManagerFromServerId(serverId);

            if (ServerManager.Instance.isEmptyServerInfoManager(serverInfoManager))
            {
                RegisterQuoter();
            }
            else
            {
                quoterForm.InitQuoter(serverInfoManager);
            }
        }

        public void RegisterQuoter()
        {
            ServerManager.Instance.RegisterQuoter(serverId, quoterForm);
        }

        public void UnregisterQuoter()
        {
            ServerManager.Instance.UnregisterQuoter(serverId, quoterForm);
        }
    }
}
