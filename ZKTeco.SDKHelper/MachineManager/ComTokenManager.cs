using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTeco.SDK.MachineManager
{
    public class ComTokenManager
    {
        // Fields
        protected static Dictionary<int, ComToken> dicComPort_Token = new Dictionary<int, ComToken>();

        // Methods
        public static ComToken GetComToken(int ComPort)
        {
            ComToken token;
            lock (dicComPort_Token)
            {
                if (dicComPort_Token.ContainsKey(ComPort))
                {
                    return dicComPort_Token[ComPort];
                }
                token = new ComToken(ComPort);
                dicComPort_Token.Add(ComPort, token);
            }
            return token;
        }
      


    }
    public class ComToken
    {
        // Fields
        private int _ComPort;
        private uint _SN = TokenCount;
        protected static uint TokenCount = 0;

        // Methods
        public ComToken(int Port)
        {
            this._ComPort = Port;
            if (uint.MaxValue == TokenCount)
            {
                TokenCount = 0;
            }
            else
            {
                TokenCount++;
            }
        }

        // Properties
        public int ComPort
        {
            get
            {
                return this._ComPort;
            }
        }

        public uint TokenSN
        {
            get
            {
                return this._SN;
            }
        }
    }


}
