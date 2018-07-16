using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.Model
{
    public partial class Transactions
    {
        // Properties
        public string Cardno { get; set; }

        public string DoorID { get; set; }

        public EventType EventType { get; set; }

        public InOutState InOutState { get; set; }

        public string Pin { get; set; }

        public DateTime LogDateTime { get; set; }

        public VerifiedType Verified { get; set; }
        
    }
    public enum InOutState
    {
        CheckIn=10,
        CheckOut=11
    }



}
