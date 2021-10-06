using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.Phong
{
    public partial class TaoPhong : Form
    {
        QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
        public TaoPhong()
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
                string tenKhu = tb_TenKhu.Text;
                string tenPhong = tb_TenPhong.Text;
                string loaiPhong = tb_LoaiPhong.Text;
                string soNguoiToiDa = tb_SoNguoiToiDa.Text;

                entities.ThemPhong(tenKhu, tenPhong, loaiPhong, Convert.ToInt32(soNguoiToiDa));

                MessageBox.Show("Thêm thành công!", "Thông báo!", MessageBoxButtons.OK);
            }
            catch (Exception)
            {
                MessageBox.Show("Thêm thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
