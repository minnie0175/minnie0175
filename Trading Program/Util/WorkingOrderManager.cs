using Ciri.Properties;
using CommonLib.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ciri.Util
{
    public class WorkingOrderManager
    {
        // ManualOrder 관련 데이터
        public static Object workingOrderDataSource = new Object();
        public static ConcurrentDictionary<long, JToken> workingOrderDic = new ConcurrentDictionary<long, JToken>();

        public static void RemoveWorkingOrder(JToken order)
        {
            long orderID = order["orderId"].Value<long>();
            JToken tok;
            workingOrderDic.TryRemove(orderID, out tok);
            UpdateWorkingOrderSource();
        }
        public static void ResetPreviousData(JToken reset)
        {
            //System.Diagnostics.Debug.WriteLine("Properties.Settings.Default.Location=" + Properties.Settings.Default.Location.ToString());
            if (Settings.Default.ResetWorkingOrder == true)
            {
                if (reset["trToReset"].Value<String>() == "WorkingOrder")
                {
                    workingOrderDic.Clear();
                    UpdateWorkingOrderSource();
                }
            }
        }
        public static BidAskData GetBidAskDataFromWorkingOrder(string isinCode)
        {
            BidAskData ba = new BidAskData();
            ba.isinCode = isinCode;
            ba.bidPrice = new int[5];
            ba.bidAmount = new int[5];
            ba.askPrice = new int[5];
            ba.askAmount = new int[5];

            SortedDictionary<int, int> bidDic = new SortedDictionary<int, int>(), askDic = new SortedDictionary<int, int>();
            foreach (JToken token in workingOrderDic.Where((pair) => { return pair.Value.Value<string>("isinCode") == isinCode; }).Select(a => a.Value))
            {
                int price = token.Value<int>("price");
                int remainingAmount = token.Value<int>("remainingAmount");
                string isLong = token.Value<string>("isLong");
                SortedDictionary<int, int> tmpDic;
                if (isLong == "LONG")
                    tmpDic = bidDic;
                else
                    tmpDic = askDic;
                if (tmpDic.ContainsKey(price))
                    tmpDic[price] += remainingAmount;
                else
                {
                    tmpDic.Add(price, remainingAmount);
                }
            }

            BidAskData mktBA = MarketDataCenter.Instance.GetBidAskData(isinCode);
            if (mktBA == null)
                return ba;
            for (int idx = 0; idx < 5; ++idx)
            {
                ba.bidPrice[idx] = mktBA.bidPrice[idx];
                if (mktBA.bidPrice[idx] != 0 || idx == 0 && mktBA.bidPrice[idx] == 0)
                {
                    int price = ba.bidPrice[idx];
                    if (bidDic.ContainsKey(price))
                        ba.bidAmount[idx] = bidDic[price];
                    else
                        ba.bidAmount[idx] = 0;
                }

                ba.askPrice[idx] = mktBA.askPrice[idx];
                if (mktBA.askPrice[idx] != 0 || idx == 0 && mktBA.askPrice[idx] == 0)
                {
                    int price = ba.askPrice[idx];
                    if (askDic.ContainsKey(price))
                        ba.askAmount[idx] = askDic[price];
                    else
                        ba.askAmount[idx] = 0;
                }
            }
            return ba;
        }


        public static void UpdateWorkingOrderGridView(JToken workingOrder)
        {
            // 수동 주문창 호가 설정
            //Console.WriteLine("wo:" + workingOrder.ToString());
            long orderId = workingOrder.Value<long>("orderId");
            workingOrderDic.AddOrUpdate(orderId, workingOrder, (id, oldOrder) => workingOrder);

            if (workingOrder["orderState"].Value<String>() != "WorkingOrderState")
            {
                JToken removedOrder;
                bool isRemoved = workingOrderDic.TryRemove(orderId, out removedOrder);
                if (isRemoved == false)
                    Console.WriteLine(" Could not erase workingOrder !!!");
            }

            UpdateWorkingOrderSource();


        }

        public static void UpdateWorkingOrderSource()
        {
            var woArray = from row in workingOrderDic
                          select new
                          {
                              주문번호 = row.Value["orderId"].Value<String>(),
                              //접수시간 = MiscUtil.getPrettyDateStr(row.Value["confirmTime"].Value<String>()),
                              종목 = ItemMaster.Instance.GetProdNameWithIsinCode(row.Value["isinCode"].Value<String>()),
                              구분 = row.Value["isLong"].Value<String>(),
                              주문수량 = String.Format("{0:n0}", row.Value["amount"].Value<String>()),
                              잔여수량 = String.Format("{0:n0}", row.Value["remainingAmount"].Value<String>()),
                              주문가 = (row.Value["price"].Value<int>() / 100.0).ToString("0.00")
                          };

            workingOrderDataSource = woArray.OrderByDescending(item => item.주문번호).ToArray();
        }
    }
}
