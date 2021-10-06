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
    public partial class ThemHopDong : Form
    {
        public ThemHopDong()
        {
            InitializeComponent();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            using (var entity = new QuanLyKTX2021Entities())
            {
                try
                {
                    string maSV = tbMaSV.Text;
                    string tenSV = tbTenSV.Text;
                    DateTime ngaySinh = Convert.ToDateTime(tbNgaySinh.Text);
                    Boolean gioiTinh = Convert.ToBoolean(tbGioiTinh.Text);
                    long cmnd = Convert.ToInt64(tbCMND.Text);
                    var queQuan = tbQueQuan.Text;
                    var danToc = tbDanToc.Text;
                    var lop = tbLop.Text;
                    var toaNha = tbToaNha.Text;
                    var tenPhong = tbTenPhong.Text;
                    long sdt = Convert.ToInt64(tbSDT.Text);
                    var tenPhuHuynh = tbTenPhuHuynh.Text;
                    var sdtPhuHuynh = tbSDTPhuHuynh.Text;
                    DateTime ngayLap = Convert.ToDateTime(tbNgayLap.Text);
                    DateTime ngayHetHan = Convert.ToDateTime(tbNgayHetHan.Text);
                    decimal tienPhong = Convert.ToDecimal(tbTienPhong.Text);
                    long nhanVienLap = Convert.ToInt64(tbNhanVienLap.Text);
                    entity.ThemHopDong(maSV, tenSV, ngaySinh, gioiTinh, cmnd, queQuan, danToc, lop, toaNha, tenPhong,
                        sdt, "1", tenPhuHuynh, sdtPhuHuynh, ngayLap, ngayHetHan,
                        tienPhong, nhanVienLap);
                    MessageBox.Show("Thành công!", "Thông báo!", MessageBoxButtons.OK);
                }
                catch (Exception)
                {
                    MessageBox.Show("Thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
