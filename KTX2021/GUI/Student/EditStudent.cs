using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.SinhVien
{
    public partial class SuaSinhVien : Form
    {
        QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
        public SuaSinhVien()
        {
            InitializeComponent();
        }

        private void btn_Update_Click(object sender, EventArgs e)
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
                long sdt = Convert.ToInt64(tbSDT.Text);
                var tenPhuHuynh = tbTenPhuHuynh.Text;
                var sdtPhuHuynh = tbSDTPhuHuynh.Text;

                entities.SuaSinhVien(maSV, tenSV, ngaySinh, gioiTinh, cmnd, queQuan, danToc, lop, sdt, tenPhuHuynh, sdtPhuHuynh);

                MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Cập nhật thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void LoadSuaSinhVien(string maSV, string tenSV, string ngaySinh, string gioiTinh, string cmnd, string queQuan, string danToc, string lop, string sdt, string tenPhuHuynh, string sdtPhuHuynh)
        {
            tbMaSV.Text = maSV;
            tbTenSV.Text = tenSV;
            tbNgaySinh.Text = ngaySinh;
            tbGioiTinh.Text = gioiTinh;
            tbCMND.Text = cmnd;
            tbQueQuan.Text = queQuan;
            tbDanToc.Text = danToc;
            tbLop.Text = lop;
            tbSDT.Text = sdt;
            tbTenPhuHuynh.Text = tenPhuHuynh;
            tbSDTPhuHuynh.Text = sdtPhuHuynh;
        }
    }
}
