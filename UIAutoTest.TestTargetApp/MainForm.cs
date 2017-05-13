using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UIAutoTest.TestTargetApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoginForm Login = new LoginForm();
            if (Login.ShowDialog() != DialogResult.OK)
            {
                // The try / finally clause is necessary in case that somebody adds some code afterwards that throws an exception.
                // It is essential that the Close() function is reached in order for the function to stop if the password is incorrect.
                try { MessageBox.Show("Impossible de se logger"); }
                finally { Close(); }
            }
            else
            {
                lblLogUser.Text = "User72";
                this.Focus();
            }

        }
    }
}
