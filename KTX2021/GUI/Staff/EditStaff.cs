using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.NhanVien
{
    public partial class SuaNhanVien : Form
    {
        QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();

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
            tbDiaDiem.Text = diaDiem;
            tbSDT.Text = sdt;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                var hoTen = tbHoTen.Text;
                var cmnd = tbCMND.Text;
                var queQuan = tbQueQuan.Text;
                var congViec = tbCongViec.Text;
                var luong = tbLuong.Text;
                var diaDiem = tbDiaDiem.Text;
                var sdt = tbSDT.Text;

                entities.SuaNhanVien(hoTen, Convert.ToInt64(cmnd), queQuan, congViec, Convert.ToInt64(luong), diaDiem, Convert.ToInt64(sdt));

                MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Cập nhật thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                var cmnd = tbCMND.Text;
                entities.XoaNhanVien(Convert.ToInt64(cmnd));
                MessageBox.Show("Xoá thành công!", "Thông báo!", MessageBoxButtons.OK);
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Xoá thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
