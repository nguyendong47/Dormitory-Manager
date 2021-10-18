using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.SinhVien
{
    public partial class SuaSinhVien : Form
    {
        public SuaSinhVien()
        {
            InitializeComponent();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
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
                    var Phone_Number_Student_Contract = Convert.ToInt64(txt_Phone_Number_Student_Contract.Text);
                    var Parents_Name_Student_Contract = txt_Parents_Name_Student_Contract.Text;
                    var Phone_Number_Parents_Student_Contract = Convert.ToInt64(txt_Phone_Number_Parents_Student_Contract.Text);
                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Edit_Student(Name_Student_Contract, Date_of_Birth_Student_Contract, Sex_Student_Contract, Identity_Card_Student_Contract,
                            Home_Town_Student_Contract, Nation_Student_Contract, Class_Student_Contract, Phone_Number_Student_Contract, Parents_Name_Student_Contract, Phone_Number_Parents_Student_Contract);
                    }
                    MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
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

        public void LoadSuaSinhVien(string tenSV, string ngaySinh, string gioiTinh, string cmnd, string queQuan, string danToc, string lop, string sdt, string tenPhuHuynh, string sdtPhuHuynh)
        {
            txt_Name_Student_Contract.Text = tenSV;
            txt_Date_of_Birth_Student_Contract.Text = ngaySinh;
            cb_Sex_Student_Contract.Text = gioiTinh;
            txt_Identity_Card_Student_Contract.Text = cmnd;
            txt_Home_Town_Student_Contract.Text = queQuan;
            txt_Nation_Student_Contract.Text = danToc;
            txt_Class_Student_Contract.Text = lop;
            txt_Phone_Number_Student_Contract.Text = sdt;
            txt_Parents_Name_Student_Contract.Text = tenPhuHuynh;
            txt_Phone_Number_Parents_Student_Contract.Text = sdtPhuHuynh;
        }
    }
}
