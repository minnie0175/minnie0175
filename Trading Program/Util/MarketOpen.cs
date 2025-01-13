using CiriData.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ciri.Util
{
    public class MarketOpen: MarketAlarmEvent<List<string>>
    {
        private List<string> serverIdList = new List<string>();
        private string marketMsg = "장 시작 5분전입니다. 다음의 서버에서 제출된 호가가 없습니다.\n 서버: {0}";

        public MarketOpen(DateTime marketOpenTime)
            : base(marketOpenTime)
        {
            msg = marketMsg;
        }

        public override List<string> MakeData()
        {
            foreach(ServerInfoManager sim in ServerManager.Instance.GetServerInfoManagerList())
            {
                int quoterCnt = sim.GetQuotingInfoCount();
                if(quoterCnt == 0)
                {
                    serverIdList.Add(sim.serverId);
                }
            }
            return serverIdList;
        }

        public override int GetDataCount()
        {
            return serverIdList.Count;
        }

        public override string MakeInfo(object info)
        {
            return info.ToString();
        }
    }
}
