using Dormitory_Management_2021.GUI.Bill;
using System;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.HoaDon
{
    public partial class UCHoaDon : UserControl
    {
        public UCHoaDon()
        {
            InitializeComponent();
        }

        private void UCHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entity.usp_Get_Bill();
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addBill = new F_Add_Bill();
            addBill.ShowDialog();
            UCHoaDon_Load(sender, e);
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            UCHoaDon_Load(sender, e);
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            string mahoadon = dgv.SelectedRows[0].Cells[0].Value.ToString();
            string tenphong = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string sonuoc = dgv.SelectedRows[0].Cells[2].Value.ToString();
            string sodien = dgv.SelectedRows[0].Cells[3].Value.ToString();
            string thang = dgv.SelectedRows[0].Cells[4].Value.ToString();
            string trangthai = dgv.SelectedRows[0].Cells[5].Value.ToString();
            string tongtien = dgv.SelectedRows[0].Cells[6].Value.ToString();

            var editBill = new F_Edit_Bill();
            editBill.LoadEditBill(mahoadon, tenphong, sonuoc, sodien, thang, trangthai, tongtien);
            editBill.ShowDialog();
            UCHoaDon_Load(sender, e);
        }

        private void btn_Month_Bill_Click(object sender, EventArgs e)
        {
            try
            {
                var month_bill = dtp_Month_Bill.Value;

                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entity.usp_Get_Bill_by_Month_Bill(month_bill);
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
                var name_room = txt_Name_Room.Text;

                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entity.usp_Get_Bill_by_Name_Room(name_room);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btn_Status_Bill_Click(object sender, EventArgs e)
        {
            try
            {
                var status_bill = cb_Status_Bill.SelectedIndex == 0 ? false : true;

                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    dgv.DataSource = entity.usp_Get_Bill_by_Status_Bill(status_bill);
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
