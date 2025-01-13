using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Ciri.Util
{
    public abstract class MarketAlarmEvent<T>: SingleExecutionMarketEvent
    {
        private T Data;
        protected string msg;
        public MarketAlarmEvent(DateTime marketCheckTime)
            : base(marketCheckTime)
        {
        }

        public override void InternalExecute()
        {
            Data = MakeData();
            int cnt = GetDataCount();
            if (cnt > 0)
            {
                ActivateAlarm();
            }
        }

        public abstract T MakeData();
        public abstract int GetDataCount();


        public string MakeInfoText()
        {
            string infoText = "";
            if (Data is IEnumerable enumerableData)
            {
                foreach (var info in enumerableData)
                {
                    infoText += MakeInfo(info) + ", ";
                }
                infoText = infoText.TrimEnd(',', ' ');
            }
            else
            {
                infoText = MakeInfo(Data);
            }
            return infoText;
        }
        public abstract string MakeInfo(object info);


        public void ActivateAlarm()
        {
            string infoText = MakeInfoText();
            MessageBox.Show(string.Format(msg, infoText), "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
