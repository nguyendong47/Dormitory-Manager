using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.SinhVien
{
    public partial class UC_SinhVien : UserControl
    {
        public UC_SinhVien()
        {
            InitializeComponent();
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            string tenSV = dgv.SelectedRows[0].Cells[0].Value.ToString();
            string ngaySinh = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string gioiTinh = dgv.SelectedRows[0].Cells[2].Value.ToString();
            string cmnd = dgv.SelectedRows[0].Cells[3].Value.ToString();
            string queQuan = dgv.SelectedRows[0].Cells[4].Value.ToString();
            string danToc = dgv.SelectedRows[0].Cells[5].Value.ToString();
            string lop = dgv.SelectedRows[0].Cells[6].Value.ToString();
            string sdt = dgv.SelectedRows[0].Cells[7].Value.ToString();
            string tenPhuHuynh = dgv.SelectedRows[0].Cells[8].Value.ToString();
            string sdtPhuHuynh = dgv.SelectedRows[0].Cells[9].Value.ToString();

            SuaSinhVien suaSinhVien = new SuaSinhVien();
            suaSinhVien.LoadSuaSinhVien(tenSV, ngaySinh, gioiTinh, cmnd, queQuan, danToc, lop, sdt, tenPhuHuynh, sdtPhuHuynh);
            suaSinhVien.ShowDialog();
            UC_SinhVien_Load(sender, e);
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            UC_SinhVien_Load(sender, e);
        }

        private void UC_SinhVien_Load(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entities.usp_Get_Student();
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_Name_Student_Contract_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    string Name_Student_Contract = txt_Name_Student_Contract.Text;
                    dgv.DataSource = entities.usp_Get_Student_by_Name_Student_Contract(Name_Student_Contract);
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

        private void btn_Identity_Card_Student_Contract_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    var Identity_Card_Student_Contract = Convert.ToInt64(txt_Identity_Card_Student_Contract.Text);
                    dgv.DataSource = entities.usp_Get_Student_by_Identity_Card_Student_Contract(Identity_Card_Student_Contract);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btn_Home_Town_Student_Contract_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    var Home_Town_Student_Contract = txt_Home_Town_Student_Contract.Text;
                    dgv.DataSource = entities.usp_Get_Student_by_Home_Town_Student_Contract(Home_Town_Student_Contract);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btn_Class_Student_Contract_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    var Class_Student_Contract = txt_Class_Student_Contract.Text;
                    dgv.DataSource = entities.usp_Get_Student_by_Class_Student_Contract(Class_Student_Contract);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btn_Parents_Name_Student_Contract_Click(object sender, EventArgs e)
        {
            try
            {
                using (var entities = new db_Dormitory_Management_2021Entities())
                {
                    var Parents_Name_Student_Contract = txt_Parents_Name_Student_Contract.Text;
                    dgv.DataSource = entities.usp_Get_Student_by_Parents_Name_Student_Contract(Parents_Name_Student_Contract);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
