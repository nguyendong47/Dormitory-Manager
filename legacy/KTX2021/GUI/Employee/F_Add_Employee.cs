using System;
using System.Linq;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.NhanVien
{
    public partial class ThemNhanVien : Form
    {
        public ThemNhanVien()
        {
            InitializeComponent();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    var hoTen = tbHoTen.Text;
                    var cmnd = Convert.ToInt64(tbCMND.Text);
                    var queQuan = tbQueQuan.Text;
                    var congViec = tbCongViec.Text;
                    var luong = Convert.ToInt64(tbLuong.Text);
                    var diaDiem = cb_DiaDiem.Text;
                    var sdt = Convert.ToInt64(tbSDT.Text);

                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Add_Employee(hoTen, cmnd, queQuan, congViec, luong, diaDiem, sdt);
                    }

                    MessageBox.Show("Thành công!", "Thông báo!", MessageBoxButtons.OK);
                }
                catch (Exception)
                {
                    MessageBox.Show("Thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }                
        }

        private void tbCMND_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ThemNhanVien_Load(object sender, EventArgs e)
        {
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
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
