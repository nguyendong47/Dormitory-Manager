using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.NhanVien
{
    public partial class UCNhanVien : UserControl
    {
        public UCNhanVien()
        {
            InitializeComponent();
        }

        private void UC_NhanVien_Load(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entities.usp_Get_Employee();
                    cb_Work_Employee.DataSource = entities.usp_Get_Work_Employee();
                    cb_Workplace_Employee.DataSource = entities.usp_Get_Workplace_Employee();
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ThemNhanVien themNhanVien = new ThemNhanVien();
            themNhanVien.ShowDialog();
            UC_NhanVien_Load(sender, e);
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            string hoTen = dgv.SelectedRows[0].Cells[0].Value.ToString();
            string cmnd = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string queQuan = dgv.SelectedRows[0].Cells[2].Value.ToString();
            string congViec = dgv.SelectedRows[0].Cells[3].Value.ToString();
            string luong = dgv.SelectedRows[0].Cells[4].Value.ToString();
            string diaDiem = dgv.SelectedRows[0].Cells[5].Value.ToString();
            string sdt = dgv.SelectedRows[0].Cells[6].Value.ToString();

            SuaNhanVien suaNhanVien = new SuaNhanVien();
            suaNhanVien.LoadSuaNhanVien(hoTen, cmnd, queQuan, congViec, luong, diaDiem, sdt);
            suaNhanVien.ShowDialog();
            UC_NhanVien_Load(sender, e);
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            UC_NhanVien_Load(sender, e);
        }

        private void btn_Home_Town_Employee_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string Home_Town_Employee = txt_Home_Town_Employee.Text;
                    dgv.DataSource = entities.usp_Get_Employee_by_Home_Town_Employee(Home_Town_Employee);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_Work_Employee_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string Work_Employee = cb_Work_Employee.Text;
                    dgv.DataSource = entities.usp_Get_Employee_by_Work_Employee(Work_Employee);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_Workplace_Employee_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string Workplace_Employee = cb_Workplace_Employee.Text;
                    dgv.DataSource = entities.usp_Get_Employee_by_Workplace_Employee(Workplace_Employee);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_Name_Employee_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string Name_Employee = txt_Name_Employee.Text;
                    dgv.DataSource = entities.usp_Get_Employee_by_Name_Employee(Name_Employee);
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
