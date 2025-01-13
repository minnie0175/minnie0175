using CommonLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Ciri.Util
{
    public class MarketClose: MarketAlarmEvent<int>
    {
        private int workingOrderCnt;
        private Dictionary<string, string> marketMsg = new Dictionary<string, string>()
        {
            {"Equity", "현물 시장 종료 1분 전입니다. 시장가가 아닌 주문이 {0}개 존재합니다. 확인하세요." },
            { "Deriv", "파생 시장 종료 1분 전입니다. 시장가가 아닌 주문이 {0}개 존재합니다. 확인하세요." }
        };

        public MarketClose(DateTime marketCloseTime, string marketType)
            : base(marketCloseTime)
        {
            msg = marketMsg[marketType];
        }

        public override int MakeData()
        {
            workingOrderCnt = 0;
            foreach (var doc in DBUtil.Instance.GetAllHedgeServerWorkingOrder())
            {
                if (doc.price != 0 && doc.bookCode != "M:STOCKLP")
                {
                    workingOrderCnt++;
                }
            }
            return workingOrderCnt;
        }

        public override int GetDataCount()
        {
            return workingOrderCnt;
        }

        public override string MakeInfo(object info)
        {
            return info.ToString();
        }
    }
}
