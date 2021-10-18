using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Bill
{
    public partial class F_Add_Bill : Form
    {
        public F_Add_Bill()
        {
            InitializeComponent();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    using (var entity = new db_Dormitory_Management_2021Entities())
                    {
                        var Name_Room = txt_Name_Room.Text;
                        var Number_Water_Bill = Convert.ToInt32(txt_Number_Water_Bill.Text);
                        var Number_Electricty_Bill = Convert.ToInt32(txt_Number_Electricty_Bill.Text);
                        var Month_Bill = dtp_Month_Bill.Value;
                        var Status_Bill = cb_Status_Bill.SelectedIndex == 0 ? false : true;
                        var Total_Money_Bill = Convert.ToDecimal(txt_Total_Money_Bill.Text);

                        entity.usp_Add_Bill(Name_Room, Number_Water_Bill, Number_Electricty_Bill, Month_Bill, Status_Bill, Total_Money_Bill);
                        MessageBox.Show("Thêm thành công!", "Thông báo!", MessageBoxButtons.OK);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Thêm thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
