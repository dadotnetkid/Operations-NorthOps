using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthOps.Services.SmsService
{
    public sealed class MsisdnChangedEventArgs : EventArgs
    {
        internal MsisdnChangedEventArgs(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public string NewValue { get; }
        public string OldValue { get; }
    }
}
