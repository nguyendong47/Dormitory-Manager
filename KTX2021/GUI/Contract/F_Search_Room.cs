using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Contract
{
    public partial class F_Search_Room : Form
    {
        public string Name_Room { get; set; }
        public F_Search_Room()
        {
            InitializeComponent();
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            var name_room_selected = dgv.SelectedRows[0].Cells[1].Value.ToString();
            if (MessageBox.Show("Bạn chắc chắn chọn phòng " + name_room_selected + " chứ ?", "Lưu ý", MessageBoxButtons.OKCancel, MessageBoxIcon.Question ) == DialogResult.OK)
            {
                this.Name_Room = name_room_selected;
                Close();
            }
        }

        private void F_Search_Room_Load(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entities.usp_Get_Room();
                    cbToaNha.DataSource = entities.usp_Get_The_Building_Room();
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnLayPhongTheoToaNha_Click(object sender, EventArgs e)
        {
            string toaNha = "A";
            try
            {
                toaNha = cbToaNha.Text;
            }
            catch (Exception)
            {

            }
            finally
            {
                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entity.usp_Get_Room_by_The_Building_Room(toaNha);
                }
            }
        }

        private void btnTimTenPhong_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string tenphong = tbTimTenPhong.Text;
                    dgv.DataSource = entities.usp_Get_Room_by_Name_Room(tenphong);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnLoaiPhong_Click(object sender, EventArgs e)
        {
            string loaiPhong = "Nam";
            try
            {
                loaiPhong = cbLoaiPhong.Text;
            }
            catch (Exception)
            {

            }
            finally
            {
                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entity.usp_Get_Room_by_Type_Room(loaiPhong);
                }
            }
        }

        private void btnTinhTrang_Click(object sender, EventArgs e)
        {
            try
            {
                int tinhTrang = cbTinhTrang.SelectedIndex;

                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entity.usp_Get_Room_By_Status(tinhTrang); // 0 empty, 1 available, 2 fulled
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            F_Search_Room_Load(sender, e);
        }
    }
}
