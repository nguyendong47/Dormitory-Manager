using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dormitory_Management_2021
{
    public partial class F_Login : Form
    {
        public F_Login()
        {
            InitializeComponent();
        }

        #region Moveable no-border
        int mouseDownX, mouseDownY;
        bool mouseDown;
        private void pl_Top_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseDownX = e.X;
            mouseDownY = e.Y;
        }

        private void pl_Top_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.SetDesktopLocation(MousePosition.X - mouseDownX, MousePosition.Y - mouseDownY);
            }
        }

        private void pl_Top_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        #endregion

        private void bn_Login_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Username.Text) || string.IsNullOrEmpty(txt_Password.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Username.Focus();
            }
            if (string.IsNullOrEmpty(txt_Password.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt_Password.Focus();
            }
            try
            {
                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    List<int> lst = entity.usp_Login(txt_Username.Text, txt_Password.Text).Select(i => i.GetValueOrDefault(0)).ToList();
                  
                    foreach (var item in lst)
                    {
                        if (item == 1)
                        {
                            var f_Main = new F_Main();
                            Hide();
                            f_Main.ShowDialog();
                            Show();
                            if (chk_Save_Account.Checked == false)
                            {
                                txt_Username.Text = "Tên tài khoản";
                                txt_Username.ForeColor = Color.Silver;
                                txt_Password.Text = "12345";
                                txt_Password.ForeColor = Color.Silver;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
