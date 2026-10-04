using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Phong
{
    public partial class SuaPhong : Form
    {
        public SuaPhong()
        {
            InitializeComponent();
        }

        public void LoadSuaPhong(string toaNha, string tenPhong, string loaiPhong, int soNguoiToiDa)
        {
            tb_ToaNha.Text = toaNha;
            tb_TenPhong.Text = tenPhong;
            cb_LoaiPhong.Text = loaiPhong;
            nud_SoNguoiToiDa.Value = soNguoiToiDa;
        }

        private void btn_Update_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    string toaNha = tb_ToaNha.Text;
                    string tenPhong = tb_TenPhong.Text;
                    string loaiPhong = cb_LoaiPhong.Text;
                    int soNguoiToiDa = (int)nud_SoNguoiToiDa.Value;

                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Edit_Room(toaNha, tenPhong, loaiPhong, soNguoiToiDa);
                    }

                    MessageBox.Show("Cập nhật thành công!", "Thông báo!", MessageBoxButtons.OK);
                }
                catch (Exception)
                {
                    MessageBox.Show("Cập nhật thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    string toaNha = tb_ToaNha.Text;
                    string tenPhong = tb_TenPhong.Text;

                    using (var entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Delete_Room(tenPhong);
                    }

                    MessageBox.Show("Xoá thành công!", "Thông báo!", MessageBoxButtons.OK);
                    Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Xoá thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
