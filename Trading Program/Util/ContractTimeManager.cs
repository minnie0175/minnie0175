using CommonLib.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Ciri.Util
{
    public class ContractTimeManager : IObserver<NotifyCollectionChangedEventArgs>
    {
        #region SingletonPattern
        private static readonly Lazy<ContractTimeManager> instance = new Lazy<ContractTimeManager>(() => new ContractTimeManager());

        public static ContractTimeManager Instance { get { return instance.Value; } }

        #endregion SingletonPattern

        private readonly ConcurrentDictionary<string, DateTime> lastContractTimeDic =
            new ConcurrentDictionary<string, DateTime>();

        private ContractTimeManager()
        {
            // 체결 내역 수신하도록 옵저버 등록
            CiriData.Manage.FilledOrderManager.SubscribeToDailyFilledOrderDictionary(this);
        }

        public DateTime? GetLastContractedTime(string bookCode, string isinCode)
        {
            var key = GetDictionaryKey(bookCode, isinCode);
            if (!lastContractTimeDic.TryGetValue(key, out var dateTime))
                return null;

            return dateTime;
        }

        public TimeSpan GetTimeSpan(string bookCode, string isinCode)
        {
            var dateTime = GetLastContractedTime(bookCode, isinCode);
            if (dateTime == null)
                return TimeSpan.Zero;

            return (TimeSpan)(DateTime.Now - dateTime);
        }

        // mm:ss
        public string GetTimeSpanString(string bookCode, string isinCode)
        {
            var span = GetTimeSpan(bookCode, isinCode);
            return $"{(span.Hours * 60 + span.Minutes):00}:{span.Seconds:00}";
        }

        private void UpdateContract(JToken obj)
        {
            var key = GetDictionaryKey(obj);
            var dateTime = MiscUtil.GetDateTime(obj["contractedTime"].Value<String>());
            lastContractTimeDic.AddOrUpdate(key, dateTime, (a, b) => dateTime);
            Console.WriteLine(obj);
        }

        private string GetDictionaryKey(JToken obj)
        {
            string bookCode = obj["bookCode"].Value<string>();
            string isinCode = obj["isinCode"].Value<string>();

            return GetDictionaryKey(bookCode, isinCode);
        }

        private string GetDictionaryKey(string bookCode, string isinCode)
        {
            return bookCode + "_" + isinCode;
        }

        public void OnNext(NotifyCollectionChangedEventArgs value)
        {
            if (value.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (KeyValuePair<string, JToken> kv in value.NewItems)
                {
                    if (kv.Value != null)
                        UpdateContract(kv.Value);
                }
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            //throw new NotImplementedException();
        }

        public void Touch()
        {
            // do nothing
        }
    }
}
