using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Phong
{
    public partial class TaoPhong : Form
    {
        public TaoPhong()
        {
            InitializeComponent();
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn chứ?", "Lưu ý!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    string toaNha = tb_ToaNha.Text;
                    string tenPhong = tb_TenPhong.Text;
                    string loaiPhong = cb_LoaiPhong.Text;
                    int soNguoiToiDa = (int)nud_SoNguoiToiDa.Value;

                    using (db_Dormitory_Management_2021Entities entities = new db_Dormitory_Management_2021Entities())
                    {
                        entities.usp_Add_Room(toaNha, tenPhong, loaiPhong, soNguoiToiDa);
                    }

                    MessageBox.Show("Thêm thành công!", "Thông báo!", MessageBoxButtons.OK);
                }
                catch (Exception)
                {
                    MessageBox.Show("Thêm thất bại!", "Thông báo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tb_TenKhu_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Điền tên toà nhà!");
        }

        private void tb_TenPhong_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Điền tên phòng!");
        }

        private void tb_LoaiPhong_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Chọn loại phòng (Nam/Nữ)!");
        }

        private void tb_SoNguoiToiDa_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(sender as Control, "Thiết lập số người tối đa!");
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
