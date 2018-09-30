using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NorthOps.Internals.Helpers;

namespace Northops.AD
{
    public partial class frmListofEntries : Form
    {
        private ActiveDirectoryHelpers activeDirectoryHelpers;
        Form1 frm;
        private ActiveDirectoryListof activeDirectoryListof;

        public frmListofEntries()
        {
            InitializeComponent();
        }
        public frmListofEntries(ActiveDirectoryHelpers activeDirectoryHelpers, Form1 frm,
            ActiveDirectoryListof activeDirectoryListof)
        {
            this.activeDirectoryHelpers = activeDirectoryHelpers;
            this.frm = frm;
            this.activeDirectoryListof = activeDirectoryListof;
            InitializeComponent();
        }
        private void frmListofEntries_Load(object sender, EventArgs e)
        {
            try
            {
                switch (activeDirectoryListof)
                {
                    case ActiveDirectoryListof.ListofDomainNames:
                        foreach (Domain i in this.activeDirectoryHelpers.DomainNames())
                        {

                            listBox1.Items.Add(i);
                        }
                        break;
                    case ActiveDirectoryListof.ListofContainers:
                        foreach (SearchResult i in this.activeDirectoryHelpers.Containers())
                        {
                            listBox1.Items.Add(i.Path);
                        }


                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(activeDirectoryHelpers.DomainName + Environment.NewLine + exception, "Error!");
            }



        }

        private void listBox1_Click(object sender, EventArgs e)
        {

            try
            {
                var lstbox = sender as ListBox;
                switch (activeDirectoryListof)
                {
                    case ActiveDirectoryListof.ListofDomainNames:
                        this.frm.activeDirectoryHelpers = new ActiveDirectoryHelpers()
                        {
                            DomainName = lstbox.SelectedItem as Domain,
                            Container = this.activeDirectoryHelpers.Container
                        };

                        this.frm.btnListofEntries.Enabled = true;
                        break;
                    case ActiveDirectoryListof.ListofContainers:

                        var select = lstbox.SelectedItem.ToString().Replace("LDAP://", "");
                        this.frm.txtContainer.Text = select;
                        this.frm.btnAddUser.Enabled = true;
                        foreach (var i in this.activeDirectoryHelpers.Groups())
                        {
                            this.frm.comboBox1.Items.Add(i);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();


                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception + Environment.NewLine + activeDirectoryHelpers.DomainName);
            }

            this.Close();
        }
    }
}
