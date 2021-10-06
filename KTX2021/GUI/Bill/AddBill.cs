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
    public partial class AddBill : Form
    {
        public AddBill()
        {
            InitializeComponent();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entity = new QuanLyKTX2021Entities())
                {
                    string toanha = tb_TenKhu.Text;
                    string tenphong = tb_TenPhong.Text;
                    int sonuoc = Convert.ToInt32(tb_SoNuoc.Text);
                    int sodien = Convert.ToInt32(tb_SoDien.Text);
                    DateTime thang = Convert.ToDateTime(tb_Thang.Text);
                    bool trangthai = Convert.ToBoolean(tb_TrangThai.Text);
                    decimal tongtien = Convert.ToDecimal(tb_TongTien.Text);
                    long nhanvienlap = Convert.ToInt64(tb_NhanVienLap.Text);

                    entity.ThemHoaDon(toanha, tenphong, sonuoc, sodien, thang, trangthai, tongtien, nhanvienlap);
                    MessageBox.Show("Thêm thành công!", "Thông báo!", MessageBoxButtons.OK);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Thêm thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
