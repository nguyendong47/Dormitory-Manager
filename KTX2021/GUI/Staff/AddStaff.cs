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
    public partial class ThemNhanVien : Form
    {
        QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();

        public ThemNhanVien()
        {
            InitializeComponent();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Create_Click(object sender, EventArgs e)
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

                entities.ThemNhanVien(hoTen, Convert.ToInt64(cmnd), queQuan, congViec, Convert.ToInt64(luong), diaDiem, Convert.ToInt64(sdt), "1");

                MessageBox.Show("Thành công!", "Thông báo!", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
