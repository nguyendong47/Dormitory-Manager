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
    public partial class SuaPhong : Form
    {
        QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
        public SuaPhong()
        {
            InitializeComponent();
        }

        public void LoadSuaPhong(string tenKhu, string tenPhong, string loaiPhong, int soNguoiToiDa)
        {
            tb_TenKhu.Text = tenKhu;
            tb_TenPhong.Text = tenPhong;
            tb_LoaiPhong.Text = loaiPhong;
            tb_SoNguoiToiDa.Text = soNguoiToiDa.ToString();
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            try
            {
                string tenKhu = tb_TenKhu.Text;
                string tenPhong = tb_TenPhong.Text;
                string loaiPhong = tb_LoaiPhong.Text;
                string soNguoiToiDa = tb_SoNguoiToiDa.Text;

                entities.SuaPhong(tenKhu, tenPhong, loaiPhong, Convert.ToInt32(soNguoiToiDa));

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

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                string tenKhu = tb_TenKhu.Text;
                string tenPhong = tb_TenPhong.Text;
                entities.XoaPhong(tenKhu, tenPhong);
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
