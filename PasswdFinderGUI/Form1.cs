﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PasswdFinderGUI
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private List<Input> passwds = new List<Input>();
        private string ActualPass = string.Empty;
        private string path = string.Empty;
        private void Frm_Load(object sender, EventArgs e)
        {
            bttnDroptxt.Enabled = false;
            bttnSearch.Enabled = false;
            txtBxForQuery.Enabled = false;
            bttnCopyLoginCred.Enabled = false;
            bttnCopyPasswd.Enabled = false;
            bttnOnlyAccount.Enabled = false;
            bttnChanger.Enabled = false;
            bttnListAllPrittyPrint.Enabled = false;
            lstBx.Enabled = false;
            lstBx.Size = new Size(768, 511);
            lstVw.Size = new Size(0, 0);
        }

        private void LoadList_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                path = openFileDialog1.FileName;
            foreach (var row in File.ReadAllLines(path).Skip(1))
                passwds.Add(new Input(row));
            bttnDroptxt.Enabled = true;
            bttnSearch.Enabled = true;
            txtBxForQuery.Enabled = true;
            bttnOnlyAccount.Enabled = true;
            bttnListAllPrittyPrint.Enabled = true;
            lstBx.Enabled = true;
            bttnLoad.Enabled = false;
            lstBx.Size = new Size(768, 511);
            lstVw.Size = new Size(0, 0);
        }

        private void AccountList_Click(object sender, EventArgs e)
        {
            bttnChanger.Enabled = false;
            bttnCopyLoginCred.Enabled = false;
            bttnCopyPasswd.Enabled = false;
            bttnListAllPrittyPrint.Enabled = true;
            lstVw.Items.Clear();
            lstBx.Items.Clear();
            lstBx.Size = new Size(768, 511);
            lstVw.Size = new Size(0, 0);
            foreach (var item in passwds)
                lstBx.Items.Add(item.AppName);
        }

        private void bttnListAllPrittyPrint_Click(object sender, EventArgs e)
        {
            bttnChanger.Enabled = false;
            bttnCopyLoginCred.Enabled = false;
            bttnCopyPasswd.Enabled = false;
            lstVw.Items.Clear();
            lstBx.Items.Clear();
            lstBx.Size = new Size(0, 0);
            lstVw.Size = new Size(768, 511);
            lstVw.View = View.Details;
            lstVw.Columns.Add("AppName");
            lstVw.Columns.Add("LoginNameSlashEmail");
            lstVw.Columns.Add("Password");
            lstVw.Columns.Add("RecoverySlash2FA");
            foreach (var item in passwds)
                lstVw.Items.Add(new ListViewItem(new string[] { item.AppName, item.LoginNameSlashEmail, item.Password, item.RecoverySlash2FA}));
            lstVw.GridLines = true;
            lstVw.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            
        }

        public void Search_Click(object sender, EventArgs e)
        {
            bttnListAllPrittyPrint.Enabled = true;
            lstVw.Items.Clear();
            lstBx.Items.Clear();
            lstBx.Size = new Size(768, 511);
            lstVw.Size = new Size(0, 0);
            foreach (var item in passwds) //AppName's all letter to lower case
                item.AppName = item.AppName.ToLower();
            string appName = txtBxForQuery.Text.ToLower().ToString();
            var query = from q in passwds where q.AppName.Contains(appName) select q;
            if (appName == "")
            {
                lstBx.Items.Add($"There is nothing in the search bar!");
                bttnCopyLoginCred.Enabled = false;
                bttnCopyPasswd.Enabled = false;
            }
            else
            {
                if (query.Count() > 1)
                {
                    foreach (var item in query)
                    {
                        if (item.RecoverySlash2FA != "-")
                        {
                            lstBx.Items.Add($"Account:\t\t{item.AppName}");
                            lstBx.Items.Add($"\tEmail/LoginName:\t{item.LoginNameSlashEmail}");
                            lstBx.Items.Add($"\tPassword:\t{item.Password}");
                            lstBx.Items.Add($"\tRecovery:\t{item.RecoverySlash2FA}");
                        }
                        else
                        {
                            lstBx.Items.Add($"Account:\t\t{item.AppName}");
                            lstBx.Items.Add($"\tEmail/LoginName:\t{item.LoginNameSlashEmail}");
                            lstBx.Items.Add($"\tPassword:\t{item.Password}");
                        }
                    }
                    lstBx.Items.Add($"You can't copy your password, because there are multiple accounts for the keyword you searched for ({appName})!");
                    bttnCopyLoginCred.Enabled = false;
                    bttnCopyPasswd.Enabled = false;
                }
                else if (query.Count() == 1)
                {
                    bttnCopyLoginCred.Enabled = true;
                    bttnCopyPasswd.Enabled = true;
                    bttnChanger.Enabled = true;
                    ActualPass = query.FirstOrDefault().Password;
                    foreach (var item in query)
                    {
                        if (item.RecoverySlash2FA != "-")
                        {
                            lstBx.Items.Add($"Account:\t\t{item.AppName}");
                            lstBx.Items.Add($"\tEmail/LoginName:\t{item.LoginNameSlashEmail}");
                            lstBx.Items.Add($"\tPassword:\t{item.Password}");
                            lstBx.Items.Add($"\tRecovery:\t{item.RecoverySlash2FA}");
                        }
                        else
                        {
                            lstBx.Items.Add($"Account:\t\t{item.AppName}");
                            lstBx.Items.Add($"\tEmail/LoginName:\t{item.LoginNameSlashEmail}");
                            lstBx.Items.Add($"\tPassword:\t{item.Password}");
                        }
                    }
                }
                else
                {
                    lstBx.Items.Add($"There is nothing for the keyword: {appName}");
                    bttnCopyLoginCred.Enabled = false;
                    bttnCopyPasswd.Enabled = false;
                }
            }
        }

        private void bttnCopyLoginCred_Click(object sender, EventArgs e)
        {
            lstVw.Items.Clear();
            lstBx.Size = new Size(768, 511);
            lstVw.Size = new Size(0, 0);
            foreach (var item in passwds) //AppName's all letter to lower case
                item.AppName = item.AppName.ToLower();
            string appName = txtBxForQuery.Text.ToLower().ToString();
            var query = from q in passwds where q.AppName.Contains(appName) select q;
            Clipboard.SetText(query.FirstOrDefault().LoginNameSlashEmail.ToString());
            lstBx.Items.Add("");
            lstBx.Items.Add("Your login credential has been copied to the clipboard!");
        }

        private void bttnCopyPasswd_Click(object sender, EventArgs e)
        {
            lstVw.Items.Clear();
            lstBx.Size = new Size(768, 511);
            lstVw.Size = new Size(0, 0);
            foreach (var item in passwds) //AppName's all letter to lower case
                item.AppName = item.AppName.ToLower();
            string appName = txtBxForQuery.Text.ToLower().ToString();
            var query = from q in passwds where q.AppName.Contains(appName) select q;
            Clipboard.SetText(query.FirstOrDefault().Password.ToString());
            lstBx.Items.Add("");
            lstBx.Items.Add("Your password has been copied to the clipboard!");
        }

        private void bttnDroptxt_Click(object sender, EventArgs e)
        {
            lstBx.Size = new Size(768, 511);
            lstVw.Size = new Size(0, 0);
            passwds.Clear();
            bttnDroptxt.Enabled = false;
            bttnSearch.Enabled = false;
            txtBxForQuery.Enabled = false;
            bttnCopyLoginCred.Enabled = false;
            bttnCopyPasswd.Enabled = false;
            bttnOnlyAccount.Enabled = false;
            bttnChanger.Enabled = false;
            
            lstBx.Enabled = false;
            lstBx.Items.Clear();
            lstVw.Items.Clear();
            txtBxForQuery.Text = "";
            bttnLoad.Enabled = true;
        }

        private void bttnChanger_Click(object sender, EventArgs e)
        {
            var PwdChanger = new PsswdChngrFrm();
            PwdChanger.ShowDialog();
            if (PwdChanger.wasChanged)
            {
                DialogResult dialogResult = MessageBox.Show($"Do you really want to change your password\nFrom: {ActualPass}\nTo: {PwdChanger.Password}", "Confirmation of changes", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string strFile = File.ReadAllText(path);
                    strFile = Regex.Replace(strFile, ActualPass, PwdChanger.Password);
                    File.WriteAllText(path, strFile);
                }
                else if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("Password change failed");
                    PwdChanger.Password = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("Password change failed");
                PwdChanger.Password = string.Empty;
            }

        }
        private void txtBxForQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                bttnSearch.PerformClick();
        }
    }
}
