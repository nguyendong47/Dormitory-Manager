using System;
using System.Linq;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.NhanVien
{
    public partial class SuaNhanVien : Form
    {
        public SuaNhanVien()
        {
            InitializeComponent();
        }

        public void LoadSuaNhanVien(string hoTen, string cmnd, string queQuan, string congViec, string luong, string diaDiem, string sdt)
        {
            tbHoTen.Text = hoTen;
            tbCMND.Text = cmnd;
            tbQueQuan.Text = queQuan;
            tbCongViec.Text = congViec;
            tbLuong.Text = luong;
            try
            {
                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    var listToaNha = entity.usp_Get_The_Building_Room().ToList();
                    listToaNha.Add("Ký Túc Xá");
                    cb_DiaDiem.DataSource = listToaNha;
                }
            }
            catch (Exception)
            {

            }
            cb_DiaDiem.Text = diaDiem;
            tbSDT.Text = sdt;
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    var hoTen = tbHoTen.Text;
                    var cmnd = tbCMND.Text;
                    var queQuan = tbQueQuan.Text;
                    var congViec = tbCongViec.Text;
                    var luong = tbLuong.Text;
                    var diaDiem = cb_DiaDiem.Text;
                    var sdt = tbSDT.Text;

                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Edit_Employee(hoTen, Convert.ToInt64(cmnd), queQuan, congViec, Convert.ToInt64(luong), diaDiem, Convert.ToInt64(sdt));
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
                    var cmnd = tbCMND.Text;
                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Delete_Employee(Convert.ToInt64(cmnd));
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
