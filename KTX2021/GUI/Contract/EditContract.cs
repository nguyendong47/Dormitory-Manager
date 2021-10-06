using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.HopDong
{
    public partial class SuaHopDong : Form
    {
        QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
        public SuaHopDong()
        {
            InitializeComponent();
        }

        public void LoadSuaHopDong(string mahd, string masv, string tensv, string toanha, string tenphong, string ngaylap, string ngayhethan, string tienphong, string nhanvienlap)
        {
            try
            {
                using (var entity = new QuanLyKTX2021Entities())
                {
                    tbMaHD.Text = mahd;
                    tbMaSV.Text = masv;
                    tbTenSV.Text = tensv;
                    tbToaNha.Text = toanha;
                    tbTenPhong.Text = tenphong;
                    tbNgayLap.Text = ngaylap;
                    tbNgayHetHan.Text = ngayhethan;
                    tbTienPhong.Text = tienphong;
                    tbNhanVienLap.Text = nhanvienlap;
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

        private void btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                long mahd = Convert.ToInt64(tbMaHD.Text);
                string maSV = tbMaSV.Text;
                string tenSV = tbTenSV.Text;
                var toaNha = tbToaNha.Text;
                var tenPhong = tbTenPhong.Text;
                DateTime ngayLap = Convert.ToDateTime(tbNgayLap.Text);
                DateTime ngayHetHan = Convert.ToDateTime(tbNgayHetHan.Text);
                decimal tienPhong = Convert.ToDecimal(tbTienPhong.Text);
                long nhanVienLap = Convert.ToInt64(tbNhanVienLap.Text);

                entities.SuaHopDong(mahd, maSV, tenSV, toaNha, tenPhong, ngayLap, ngayHetHan, tienPhong, nhanVienLap);

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
                long mahd = Convert.ToInt64(tbMaHD.Text);
                entities.XoaHopDong(mahd);
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
