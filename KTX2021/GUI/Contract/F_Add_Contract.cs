using Dormitory_Management_2021.GUI.Contract;
using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.HopDong
{
    public partial class ThemHopDong : Form
    {
        public ThemHopDong()
        {
            InitializeComponent();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    try
                    {
                        var Name_Student_Contract = txt_Name_Student_Contract.Text;
                        var Date_of_Birth_Student_Contract = dtp_Date_of_Birth_Student_Contract.Value;
                        var Sex_Student_Contract = cb_Sex_Student_Contract.Text;
                        var Identity_Card_Student_Contract = Convert.ToInt64(txt_Identity_Card_Student_Contract.Text);
                        var Home_Town_Student_Contract = txt_Home_Town_Student_Contract.Text;
                        var Nation_Student_Contract = txt_Nation_Student_Contract.Text;
                        var Class_Student_Contract = txt_Class_Student_Contract.Text;
                        var Name_Room = txt_Name_Room.Text;
                        var Phone_Number_Student_Contract = Convert.ToInt64(txt_Phone_Number_Student_Contract.Text);
                        var Parents_Name_Student_Contract = txt_Parents_Name_Student_Contract.Text;
                        var Phone_Number_Parents_Student_Contract = Convert.ToInt64(txt_Phone_Number_Parents_Student_Contract.Text);
                        var Start_Day_Contract = dtp_Start_Day_Contract.Value;
                        var Expiration_Date_Contract = dtp_Expiration_Date_Contract.Value;
                        var Total_Money_Contract = Convert.ToDecimal(txt_Total_Money_Contract.Text);
                        entity.usp_Add_Contract(Name_Student_Contract, Date_of_Birth_Student_Contract, Sex_Student_Contract, Identity_Card_Student_Contract, Home_Town_Student_Contract, Nation_Student_Contract, Class_Student_Contract, Name_Room,
                            Phone_Number_Student_Contract, Parents_Name_Student_Contract, Phone_Number_Parents_Student_Contract, Start_Day_Contract, Expiration_Date_Contract,
                            Total_Money_Contract);
                        MessageBox.Show("Thành công!", "Thông báo!", MessageBoxButtons.OK);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Search_Room_Click(object sender, EventArgs e)
        {
            using (var search_Room = new F_Search_Room())
            {
                var result = search_Room.ShowDialog();
                string name_room = search_Room.Name_Room;
                txt_Name_Room.Text = name_room;

            }
        }
    }
}
