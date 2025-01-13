using Ciri.Properties;
using CiriData.Manage;
using CommonLib.Interface;
using CommonLib.Util;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Ciri.Util
{
    sealed class OrderStatusChecker
    {
        #region SingletonPattern
        private static readonly Lazy<OrderStatusChecker> instance = new Lazy<OrderStatusChecker>(() => new OrderStatusChecker());

        public static OrderStatusChecker Instance { get { return instance.Value; } }

        #endregion SingletonPattern

        private OrderStatusChecker()
        {
            DateTime marketStartTime = DateTimeCenter.Instance.GetMarketStartTime();
            DateTime marketEndTime = DateTimeCenter.Instance.GetMarketEndTime();

            DateTime beforeMarketOpen = marketStartTime.AddMinutes(-5);
            DateTime beforeEquityMarketEveningAuction = marketEndTime.AddMinutes(-1);
            DateTime beforeEquityMarketClose = marketEndTime.AddMinutes(9);
            DateTime beforeDerivMarketEveningAuction = marketEndTime.AddMinutes(14);
            DateTime beforeDerivMarketClose = marketEndTime.AddMinutes(24);

            //// 테스트용
            //DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 26, 0);

            //beforeMarketOpen = dateTime;
            //beforeEquityMarketEveningAuction = dateTime;
            //beforeEquityMarketClose = dateTime;
            //beforeDerivMarketEveningAuction = dateTime;
            //beforeDerivMarketClose = dateTime;

            marketEventList.Add(new MarketOpen(beforeMarketOpen));
            marketEventList.Add(new MarketEveningAuction(beforeEquityMarketEveningAuction, "Equity"));
            marketEventList.Add(new MarketEveningAuction(beforeDerivMarketEveningAuction, "Deriv"));
            marketEventList.Add(new MarketClose(beforeEquityMarketClose, "Equity"));
            marketEventList.Add(new MarketClose(beforeDerivMarketClose, "Deriv"));


            t = new Thread(() => run());
        }

        Thread t;
        bool isFinished = false;
        bool isMutexAcquired = false;
        List<IMarketEvent> marketEventList = new List<IMarketEvent>();

        private void run()
        {
            Mutex mut = null;
            try
            {
                mut = new Mutex(true, Settings.Default.IsDev ? "Dev_OrderStatusChecker" : "Prod_OrderStatusChecker");
                bool retry = true;
                while (retry)
                {
                    try
                    {
                        isMutexAcquired = mut.WaitOne(1000);
                        if (isFinished)
                            return;
                        if (isMutexAcquired)
                        {
                            Console.WriteLine("Mutex acquired");
                            break;
                        }
                    }
                    catch (AbandonedMutexException e)
                    {
                        Console.WriteLine("Ignoring exception " + e.ToString());
                        retry = false;
                    }
                }

                //string HHmm = DateTime.Now.ToString("HHmm");;
                //bool beforeMarketOpenChecked = false, beforeEquityMarketEveningAuctionChecked = false, beforeEquityMarketCloseChecked = false, beforeDerivMarketEveningAuctionChecked = false, beforeDerivMarketCloseChecked = false;
                while (!isFinished)
                {
                    if (ServerManager.Instance.GetServerInfoManagerList().Count != 0)
                    {
                        foreach(IMarketEvent marketEvent in marketEventList)
                        {
                            if(marketEvent.IsTime(DateTime.Now)) marketEvent.Execute();
                        }
                    }
                    #region 예전 코드

                    //if ((HHmm == beforeMarketOpen.ToString("HHmm") && !beforeMarketOpenChecked))
                    //{
                    //    //quoterDic이 0인지 검사
                    //    int quoterCnt = ControlUpdater.GetQuotingInfoCount();
                    //    if (quoterCnt == 0)
                    //    {
                    //        MessageBox.Show(string.Format("장 시작 5분전입니다. 제출된 호가가 없습니다. quoterCnt={0}", quoterCnt), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeMarketOpenChecked = true;
                    //    }
                    //}
                    //else if (!beforeEquityMarketEveningAuctionChecked && HHmm == beforeEquityMarketEveningAuction.ToString("HHmm"))
                    //{
                    //    //모든 quoter 껐는지 검사 후 경고
                    //    int quoterCnt = ControlUpdater.GetWorkingQuoterCount();
                    //    if (quoterCnt > 0)
                    //    {
                    //        MessageBox.Show(string.Format("현물 동시호가 1분 전입니다. quoterCnt={0} 개의 Working 호가가 있습니다. 확인후 취소해주세요.", quoterCnt), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeEquityMarketEveningAuctionChecked = true;
                    //    }
                    //}
                    //else if (!beforeDerivMarketEveningAuctionChecked && HHmm == beforeDerivMarketEveningAuction.ToString("HHmm"))
                    //{
                    //    //모든 quoter 껐는지 검사 후 경고
                    //    int quoterCnt = ControlUpdater.GetWorkingQuoterCount();
                    //    if (quoterCnt > 0)
                    //    {
                    //        MessageBox.Show(string.Format("파생 동시호가 1분 전입니다. quoterCnt={0} 개의 Working 호가가 있습니다. 확인후 취소해주세요.", quoterCnt), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeDerivMarketEveningAuctionChecked = true;
                    //    }
                    //}
                    //else if (!beforeEquityMarketCloseChecked && HHmm == beforeEquityMarketClose.ToString("HHmm"))
                    //{
                    //    //모든 WORKING ORDER 시장가로 바꿨는지 확인
                    //    int cnt = 0;
                    //    foreach (var doc in DBUtil.Instance.GetAllHedgeServerWorkingOrder())
                    //    {
                    //        //시장가 여부를 가격0으로 체크하는게 정확하진 않지만 임시로 사용
                    //        //M:STOCKLP는 종가에 BF로 청산하므로 제외
                    //        if (doc.price != 0 && doc.bookCode != "M:STOCKLP")
                    //        {
                    //            cnt++;
                    //        }
                    //    }
                    //    if (cnt > 0)
                    //    {
                    //        MessageBox.Show(string.Format("현물 시장 종료 1분 전입니다. 시장가가 아닌 주문이 {0}개 존재합니다. 확인하세요.", cnt), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeEquityMarketCloseChecked = true;
                    //    }
                    //}
                    //else if (!beforeDerivMarketCloseChecked && HHmm == beforeDerivMarketClose.ToString("HHmm"))
                    //{
                    //    //모든 WORKING ORDER 시장가로 바꿨는지 확인
                    //    int cnt = 0;
                    //    foreach (var doc in DBUtil.Instance.GetAllHedgeServerWorkingOrder())
                    //    {
                    //        //시장가 여부를 가격0으로 체크하는게 정확하진 않지만 임시로 사용
                    //        if (doc.price != 0)
                    //        {
                    //            cnt++;
                    //        }
                    //    }
                    //    if (cnt > 0)
                    //    {
                    //        MessageBox.Show(string.Format("파생 시장 종료 1분 전입니다. 시장가가 아닌 주문이 {0}개 존재합니다. 확인하세요.", cnt), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeDerivMarketCloseChecked = true;
                    //    }
                    //}
                    #endregion
                    #region 단순 구현
                    //if ((HHmm == beforeMarketOpen.ToString("HHmm") && !beforeMarketOpenChecked))
                    //{
                    //    foreach (ServerInfoManager serverInfoManager in simList)
                    //    {
                    //        //quoterDic이 0인지 검사
                    //        int quoterCnt = serverInfoManager.GetQuotingInfoCount();
                    //        if (quoterCnt == 0)
                    //        {
                    //            serverIdList += serverInfoManager.serverId + " ";
                    //        }
                    //    }
                    //    if (!string.IsNullOrEmpty(serverIdList))
                    //    {
                    //        MessageBox.Show(string.Format("장 시작 5분전입니다. 다음의 서버에 제출된 호가가 없습니다. 서버: {0}", serverIdList), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeMarketOpenChecked = true;
                    //    }
                    //}
                    //else if (!beforeEquityMarketEveningAuctionChecked && HHmm == beforeEquityMarketEveningAuction.ToString("HHmm"))
                    //{
                    //    foreach (ServerInfoManager serverInfoManager in simList)
                    //    {
                    //        //모든 quoter 껐는지 검사 후 경고
                    //        int quoterCnt = serverInfoManager.GetWorkingQuoterCount();
                    //        if (quoterCnt > 0)
                    //        {
                    //            serverIdList += serverInfoManager.serverId + " ";
                    //        }
                    //    }
                    //    if (!string.IsNullOrEmpty(serverIdList))
                    //    {
                    //        MessageBox.Show(string.Format("현물 동시호가 1분 전입니다. 다음의 서버에 Working 호가가 있습니다. 확인후 취소해주세요. 서버: {0}", serverIdList), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeEquityMarketEveningAuctionChecked = true;
                    //    }
                    //}
                    //else if (!beforeDerivMarketEveningAuctionChecked && HHmm == beforeDerivMarketEveningAuction.ToString("HHmm"))
                    //{
                    //    foreach (ServerInfoManager serverInfoManager in simList)
                    //    {
                    //        //모든 quoter 껐는지 검사 후 경고
                    //        int quoterCnt = serverInfoManager.GetWorkingQuoterCount();
                    //        if (quoterCnt > 0)
                    //        {
                    //            serverIdList += serverInfoManager.serverId + " ";
                    //        }
                    //    }
                    //    if (!string.IsNullOrEmpty(serverIdList))
                    //    {
                    //        MessageBox.Show(string.Format("파생 동시호가 1분 전입니다. 다음의 서버에 Working 호가가 있습니다. 확인후 취소해주세요. 서버: {0}", serverIdList), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeDerivMarketEveningAuctionChecked = true;
                    //    }
                    //}
                    //else if (!beforeEquityMarketCloseChecked && HHmm == beforeEquityMarketClose.ToString("HHmm"))
                    //{
                    //    //모든 WORKING ORDER 시장가로 바꿨는지 확인
                    //    int cnt = 0;
                    //    foreach (var doc in DBUtil.Instance.GetAllHedgeServerWorkingOrder())
                    //    {
                    //        //시장가 여부를 가격0으로 체크하는게 정확하진 않지만 임시로 사용
                    //        //M:STOCKLP는 종가에 BF로 청산하므로 제외
                    //        if (doc.price != 0 && doc.bookCode != "M:STOCKLP")
                    //        {
                    //            cnt++;
                    //        }
                    //    }
                    //    if (cnt > 0)
                    //    {
                    //        MessageBox.Show(string.Format("현물 시장 종료 1분 전입니다. 시장가가 아닌 주문이 {0}개 존재합니다. 확인하세요.", cnt), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeEquityMarketCloseChecked = true;
                    //    }
                    //}
                    //else if (!beforeDerivMarketCloseChecked && HHmm == beforeDerivMarketClose.ToString("HHmm"))
                    //{
                    //    //모든 WORKING ORDER 시장가로 바꿨는지 확인
                    //    int cnt = 0;
                    //    foreach (var doc in DBUtil.Instance.GetAllHedgeServerWorkingOrder())
                    //    {
                    //        //시장가 여부를 가격0으로 체크하는게 정확하진 않지만 임시로 사용
                    //        if (doc.price != 0)
                    //        {
                    //            cnt++;
                    //        }
                    //    }
                    //    if (cnt > 0)
                    //    {
                    //        MessageBox.Show(string.Format("파생 시장 종료 1분 전입니다. 시장가가 아닌 주문이 {0}개 존재합니다. 확인하세요.", cnt), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    //        beforeDerivMarketCloseChecked = true;
                    //    }
                    //}
                    #endregion
                    Thread.Sleep(1000);
                }
            }
            finally
            {
                if (mut != null)
                {
                    if (isMutexAcquired)
                        mut.ReleaseMutex();
                    mut.Close();
                    mut.Dispose();
                    Console.WriteLine("Disposing mutex finished");
                }
            }
        }

        public void StopAndWait()
        {
            isFinished = true;
            if (t != null)
                t.Join();
        }
        public void Start()
        {
            t.Start();
            isFinished = false;
        }
    }
}