using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021
{
    public partial class FLogin : Form
    {
        public FLogin()
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

        #region Placeholder text
        private void tb_Username_Enter(object sender, EventArgs e)
        {
            if (tb_Username.Text == "123456789")
            {
                tb_Username.Text = "";
                tb_Username.ForeColor = Color.Black;
            }
        }

        private void tb_Username_Leave(object sender, EventArgs e)
        {
            if (tb_Username.Text == "")
            {
                tb_Username.Text = "123456789";
                tb_Username.ForeColor = Color.Silver;
            }
        }

        private void tb_Password_Enter(object sender, EventArgs e)
        {
            if (tb_Password.Text == "Password")
            {
                tb_Password.Text = "";
                tb_Password.ForeColor = Color.Black;
            }
        }

        private void tb_Password_Leave(object sender, EventArgs e)
        {
            if (tb_Password.Text == "")
            {
                tb_Password.Text = "Password";
                tb_Password.ForeColor = Color.Silver;
            }
        }
        #endregion

        public static int CurrentUser = 0; // 1: quản lý, 0: nhân viên, sinh viên
        private void bn_Login_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_Username.Text) || string.IsNullOrEmpty(tb_Password.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_Username.Focus();
            }
            if (string.IsNullOrEmpty(tb_Password.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_Password.Focus();
            }
            try
            {
                using (var entity = new QuanLyKTX2021Entities())
                {
                    List<int> lst = entity.DangNhap(Convert.ToInt64(tb_Username.Text), tb_Password.Text).Select(i => i.GetValueOrDefault(0)).ToList();
                  
                    foreach (var item in lst)
                    {
                        if (item == 2)
                        {
                            CurrentUser = 1;
                            var formMain = new FMain();
                            Hide();
                            formMain.ShowDialog();
                            Show();
                            if (chk_SaveAccount.Checked == false)
                            {
                                tb_Username.Text = "Tên tài khoản";
                                tb_Username.ForeColor = Color.Silver;
                                tb_Password.Text = "Password";
                                tb_Password.ForeColor = Color.Silver;
                            }
                        }
                        else if (item == 1)
                        {
                            var formMain = new FMain();
                            Hide();
                            formMain.ShowDialog();
                            Show();
                            if (chk_SaveAccount.Checked == false)
                            {
                                tb_Username.Text = "Tên tài khoản";
                                tb_Username.ForeColor = Color.Silver;
                                tb_Password.Text = "Password";
                                tb_Password.ForeColor = Color.Silver;
                            }
                            CurrentUser = 0;
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
