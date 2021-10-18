using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Profile
{
    public partial class F_Profile : Form
    {
        public F_Profile()
        {
            InitializeComponent();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    var Username_Account = txt_Username_Account.Text;
                    var Old_Password_Account = txt_Old_Password_Account.Text;
                    var New_Password_Account = txt_New_Password_Account.Text;

                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Edit_Account(Username_Account, Old_Password_Account, New_Password_Account);
                    }

                    MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
                }
                catch (Exception)
                {
                    MessageBox.Show("Cập nhật thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
