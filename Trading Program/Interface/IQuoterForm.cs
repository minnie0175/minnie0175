using Ciri.Util;
using CiriData.Manage;
using CommonLib.Interface;
using MongoDB.Driver.Core.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ciri.Interface
{
    public interface IQuoterForm
    {
        string serverId { get;}
        void InitQuoter(ServerInfoManager serverInfoManager);
    }
}
