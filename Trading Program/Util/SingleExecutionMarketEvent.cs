using CiriData.Manage;
using CommonLib.Interface;
using CommonLib.Util;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ciri.Util
{
    public abstract class SingleExecutionMarketEvent: IMarketEvent
    {
        private DateTime checkTime;
        private bool isChecked = false;

        public SingleExecutionMarketEvent(DateTime checkTime)
        {
            this.checkTime = checkTime;
        }

        public bool IsTime(DateTime currentTime)
        {
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0); 
            return currentTime.Equals(checkTime);
        }

        public void Execute()
        {
            if (isChecked) return;
            InternalExecute();
            isChecked = true;
        }

        public abstract void InternalExecute();

    }
}
