using Amazon.Runtime.Internal.Transform;
using CiriData.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ciri.Util
{
    public class MarketEveningAuction: MarketAlarmEvent<Dictionary<string, int>>
    {
        private Dictionary<string, int> quoterCntDic = new Dictionary<string, int>();
        private Dictionary<string, string> marketMsg = new Dictionary<string, string>()
        {
            {"Equity", "현물 동시호가 1분 전입니다. 다음의 서버에 Working 호가가 있습니다. 확인후 취소해주세요\n{0}" },
            { "Deriv", "파생 동시호가 1분 전입니다. 다음의 서버에 Working 호가가 있습니다. 확인후 취소해주세요\n{0}" }
        };

        public MarketEveningAuction(DateTime marketEveningAuctionTime, string marketType)
            : base(marketEveningAuctionTime)
        {
            msg = marketMsg[marketType];
        }

        public override Dictionary<string, int> MakeData()
        {
            foreach (ServerInfoManager sim in ServerManager.Instance.GetServerInfoManagerList())
            {
                int quoterCnt = sim.GetWorkingQuoterCount();
                if (quoterCnt > 0)
                {
                    quoterCntDic.Add(sim.serverId, quoterCnt);
                }
            }
            return quoterCntDic;
        }

        public override int GetDataCount()
        {
            return quoterCntDic.Count;
        }

        public override string MakeInfo(object info)
        {
            var pair = (KeyValuePair<string, int>)info;
            string serverAndQuoterCnt = "서버 {0}: {1}개";
            string serverId = pair.Key;
            int quoterCnt = pair.Value;
            return string.Format(serverAndQuoterCnt, serverId, quoterCnt);
        }
    }
}
