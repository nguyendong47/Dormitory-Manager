using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Bill
{
    public partial class F_Edit_Bill : Form
    {
        public F_Edit_Bill()
        {
            InitializeComponent();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var ID_Bill = Convert.ToInt32(txt_ID_Bill.Text);
                var Name_Room = txt_Name_Room.Text;
                var Number_Water_Bill = Convert.ToInt32(txt_Number_Water_Bill.Text);
                var Number_Electricty_Bill = Convert.ToInt32(txt_Number_Electricty_Bill.Text);
                var Month_Bill = dtp_Month_Bill.Value;
                var Status_Bill = cb_Status_Bill.SelectedIndex == 0 ? false : true;
                var Total_Money_Bill = Convert.ToDecimal(txt_Total_Money_Bill.Text);
                try
                {
                    using (var entity = new db_Dormitory_Management_2021Entities())
                    {
                        entity.usp_Edit_Bill(ID_Bill, Name_Room, Number_Water_Bill, Number_Electricty_Bill, Month_Bill, Status_Bill, Total_Money_Bill);
                        MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
                        Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Cập nhật thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                try
                {
                    using (var entity = new db_Dormitory_Management_2021Entities())
                    {
                        var ID_Bill = Convert.ToInt32(txt_ID_Bill.Text);
                        entity.usp_Delete_Bill(ID_Bill);
                        MessageBox.Show("Xoá thành công!", "Thông báo!", MessageBoxButtons.OK);
                        Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Xoá thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void LoadEditBill(string mahoadong, string tenphong, string sonuoc, string sodien, string thang, string trangthai, string tongtien)
        {
            txt_ID_Bill.Text = mahoadong;
            txt_Name_Room.Text = tenphong;
            txt_Number_Water_Bill.Text = sonuoc;
            txt_Number_Electricty_Bill.Text = sodien;
            dtp_Month_Bill.Value = Convert.ToDateTime(thang);
            cb_Status_Bill.Text = trangthai;
            txt_Total_Money_Bill.Text = tongtien;
        }
    }
}
