using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.HopDong
{
    public partial class UCHopDong : UserControl
    {
        public UCHopDong()
        {
            InitializeComponent();
        }

        private void UC_HopDong_Load(object sender, EventArgs e)
        {

            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entities.usp_Get_Contract();
                }
            }
            catch (Exception)
            {
            }
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            string mahd = dgv.SelectedRows[0].Cells[0].Value.ToString();
            string tensv = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string tenphong = dgv.SelectedRows[0].Cells[2].Value.ToString();
            string ngaylap = dgv.SelectedRows[0].Cells[3].Value.ToString();
            string ngayhethan = dgv.SelectedRows[0].Cells[4].Value.ToString();
            string tienphong = dgv.SelectedRows[0].Cells[5].Value.ToString();

            SuaHopDong suaHopDong = new SuaHopDong();
            suaHopDong.LoadSuaHopDong(mahd, tensv, tenphong, ngaylap, ngayhethan, tienphong);
            suaHopDong.ShowDialog();
            UC_HopDong_Load(sender, e);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ThemHopDong themHopDong = new ThemHopDong();
            themHopDong.ShowDialog();
            UC_HopDong_Load(sender, e);
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            UC_HopDong_Load(sender, e);
        }

        private void btn_Name_Student_Contract_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string Name_Student_Contract = txt_Name_Student_Contract.Text;
                    dgv.DataSource = entities.usp_Get_Contract_by_Name_Student_Contract(Name_Student_Contract);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btn_Name_Room_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string Name_Room = txt_Name_Room.Text;
                    dgv.DataSource = entities.usp_Get_Contract_by_Name_Room(Name_Room);
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
