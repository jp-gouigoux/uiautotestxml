using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace UIAutoTest.TestTargetApp
{
    public partial class LoginForm : Form
    {
        private SecureString SecuredPassword = new SecureString();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Note to readers : the following code should not be used as is in a secured environment. Incrementing a SecureString key by key
            // instead of trusting the PasswordChar of the TextBox improves the security by hiding the content of the password from the memory.
            // Also, using a SHA1 hash is a good practice to encrypt the password. The problem lies inbetween : when calculating the hash,
            // it is necessary to put the content of the SecureString inside an array of bytes, since no method takes a SecureString as an input.
            // Thus, the password is visible in memory, which is what we tried to prevent by using a SecureString. If someone has a completely
            // secured method to calculate a hash from a SecureString, I would be very happy to know (jp.gouigoux.AT.free.fr).

            SecuredPassword.MakeReadOnly();
            byte[] Table = null;
            IntPtr Pointer = IntPtr.Zero;
            try
            {
                Table = new byte[SecuredPassword.Length * 2];
                Pointer = Marshal.SecureStringToBSTR(SecuredPassword);
                Marshal.Copy(Pointer, Table, 0, Table.Length);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(Pointer);
                SecuredPassword.Dispose();
                SecuredPassword = null;
            }

            SHA1 HashEngine = SHA1Managed.Create();
            byte[] HashedPassword = HashEngine.ComputeHash(Table);
            HashEngine.Clear();

            if (string.Compare(Convert.ToBase64String(HashedPassword), "FlF9ga5MRiKN1GB0FtOPrw8Q//w=") == 0)
                DialogResult = DialogResult.OK;
            Close();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                SecuredPassword.Dispose();
                SecuredPassword = new SecureString();
                txtPassword.Clear();
            }
            else if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar))
            {
                SecuredPassword.AppendChar(e.KeyChar);
                txtPassword.Text += "*";
                e.Handled = true;
            }
        }
    }
}
