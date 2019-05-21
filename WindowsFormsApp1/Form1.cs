using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZKTeco.SDK.MachineManager;
using ZKTeco.SDK.Model;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            STDDevComm devComm = new STDDevComm(new Machines(ip: "10.10.20.50"));
            devComm.IsConnected = true;
            devComm.Unlock();
        }
    }
}
