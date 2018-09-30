using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NorthOps.Internals.Helpers;

namespace Northops.AD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        public ActiveDirectoryHelpers activeDirectoryHelpers;
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnListofDomains_Click(object sender, EventArgs e)
        {
            activeDirectoryHelpers = new ActiveDirectoryHelpers() { Container = txtContainer.Text };
            frmListofEntries frm = new frmListofEntries(activeDirectoryHelpers, this, ActiveDirectoryListof.ListofDomainNames);
            frm.ShowDialog();
        }

        private void btnListofEntries_Click(object sender, EventArgs e)
        {
            frmListofEntries frm = new frmListofEntries(activeDirectoryHelpers, this, ActiveDirectoryListof.ListofContainers);
            frm.ShowDialog();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            activeDirectoryHelpers.Container = txtContainer.Text;
            activeDirectoryHelpers.CreateUser(new Users() { UserName = txtUsername.Text, Password = txtPassword.Text, FirstName = txtFirstname.Text, LastName = txtSurName.Text }, comboBox1.SelectedItem);
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
