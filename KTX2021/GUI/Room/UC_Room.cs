using Dormitary_Management_2021;
using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Phong
{
    public partial class UCPhong : UserControl
    {
        public UCPhong()
        {
            InitializeComponent();
        }

        private void UC_Phong_Load(object sender, EventArgs e)
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

        private void btnThemPhong_Click(object sender, EventArgs e)
        {
            TaoPhong taoPhong = new TaoPhong();
            taoPhong.ShowDialog();
            UC_Phong_Load(sender, e);
        }

        private void dgv_Phong_DoubleClick(object sender, EventArgs e)
        {
            string tenKhu = dgv.SelectedRows[0].Cells[0].Value.ToString();
            string tenPhong = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string loaiPhong = dgv.SelectedRows[0].Cells[2].Value.ToString();
            int soNguoiToiDa = Convert.ToInt32(dgv.SelectedRows[0].Cells[3].Value.ToString());

            SuaPhong suaPhong = new SuaPhong();

            try
            {
                suaPhong.LoadSuaPhong(tenKhu, tenPhong, loaiPhong, soNguoiToiDa);
            }
            catch (Exception)
            {

            }

            suaPhong.ShowDialog();
            UC_Phong_Load(sender, e);
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            UC_Phong_Load(sender, e);
        }

        private void btnSearch_Click(object sender, EventArgs e)
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

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            if (dgv.Rows.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application xcelApp = new Microsoft.Office.Interop.Excel.Application();
                xcelApp.Application.Workbooks.Add(Type.Missing);

                for (int i = 1; i < dgv.Columns.Count + 1; i++)
                {
                    xcelApp.Cells[1, i] = dgv.Columns[i - 1].HeaderText;
                }

                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        xcelApp.Cells[i + 2, j + 1] = dgv.Rows[i].Cells[j].Value.ToString();
                    }
                }
                xcelApp.Columns.AutoFit();
                xcelApp.Visible = true;
            }
        }
    }
}