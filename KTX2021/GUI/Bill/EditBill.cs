using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.Bill
{
    public partial class EditBill : Form
    {
        public EditBill()
        {
            InitializeComponent();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            long mahoadon = Convert.ToInt64(tb_MaHoaDon.Text);
            string toanha = tb_TenKhu.Text;
            string tenphong = tb_TenPhong.Text;
            int sonuoc = Convert.ToInt32(tb_SoNuoc.Text);
            int sodien = Convert.ToInt32(tb_SoDien.Text);
            DateTime thang = Convert.ToDateTime(tb_Thang.Text);
            bool trangthai = Convert.ToBoolean(tb_TrangThai.Text);
            decimal tongtien = Convert.ToDecimal(tb_TongTien.Text);
            long nhanvienlap = Convert.ToInt64(tb_NhanVienLap.Text);
            try
            {
                using (var entity = new QuanLyKTX2021Entities())
                {
                    entity.SuaHoaDon(mahoadon, toanha, tenphong, sonuoc, sodien, thang, trangthai, tongtien, nhanvienlap);
                    MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
                    Close();
                }
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

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entity = new QuanLyKTX2021Entities())
                {
                    long mahoadon = Convert.ToInt64(tb_MaHoaDon.Text);
                    entity.XoaHoaDon(mahoadon);
                    MessageBox.Show("Xoá thành công!", "Thông báo!", MessageBoxButtons.OK);
                    Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Xoá thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadEditBill(string mahoadong, string toanha, string tenphong, string sonuoc, string sodien, string thang, string trangthai, string tongtien, string nhanvienlap)
        {
            tb_MaHoaDon.Text = mahoadong;
            tb_TenKhu.Text = toanha;
            tb_TenPhong.Text = tenphong;
            tb_SoNuoc.Text = sonuoc;
            tb_SoDien.Text = sodien;
            tb_Thang.Text = thang;
            tb_TrangThai.Text = trangthai;
            tb_TongTien.Text = tongtien;
            tb_NhanVienLap.Text = nhanvienlap;
        }
    }
}
