using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.HopDong
{
    public partial class SuaHopDong : Form
    {
        public SuaHopDong()
        {
            InitializeComponent();
        }

        public void LoadSuaHopDong(string ID_Contract, string Name_Student_Contract, string Name_Room, string Start_Day_Contract, string Expiration_Date_Contract, string Total_Money_Contract)
        {
            try
            {
                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    txt_ID_Contract.Text = ID_Contract;
                    txt_Name_Student_Contract.Text = Name_Student_Contract;
                    txt_Name_Room.Text = Name_Room;
                    txt_Start_Day_Contract.Text = Start_Day_Contract;
                    txt_Expiration_Date_Contract.Text = Expiration_Date_Contract;
                    txt_Total_Money_Contract.Text = Total_Money_Contract;
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                try
                {
                    var ID_Contract = Convert.ToInt64(txt_ID_Contract.Text);
                    var Name_Student_Contract = txt_Name_Student_Contract.Text;
                    var Name_Room = txt_Name_Room.Text;
                    var Start_Day_Contract = dtp_Start_Day_Contract.Value;
                    var Expiration_Date_Contract = dtp_Expiration_Date_Contract.Value;
                    var Total_Money_Contract = Convert.ToDecimal(txt_Total_Money_Contract.Text);
                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Edit_Contract(ID_Contract, Name_Student_Contract, Name_Room, Start_Day_Contract, Expiration_Date_Contract, Total_Money_Contract);
                    }
                    MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
                }
                catch (Exception)
                {
                    MessageBox.Show("Cập nhật thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                try
                {
                    var ID_Contract = Convert.ToInt64(txt_ID_Contract.Text);
                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Delete_Contract(ID_Contract);
                    }
                    MessageBox.Show("Xoá thành công!", "Thông báo!", MessageBoxButtons.OK);
                    Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Xoá thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
