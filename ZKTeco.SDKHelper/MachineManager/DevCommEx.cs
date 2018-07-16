using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKTeco.SDK.Model;

namespace ZKTeco.SDK.MachineManager
{
    public class DevCommEx : DevComm
    {
        // Fields
        private Machines dev = new Machines();
        private IntPtr h = IntPtr.Zero;

        // Methods
        public DevCommEx(Machines device)
        {
            this.dev = device;
        }
    }


}
