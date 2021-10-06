using KTX2021.GUI.Bill;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.HoaDon
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
                using (var entity = new QuanLyKTX2021Entities())
                {
                    dgv.DataSource = entity.LayDanhSachHoaDon();
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addBill = new AddBill();
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
            string toanha = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string tenphong = dgv.SelectedRows[0].Cells[2].Value.ToString();
            string sonuoc = dgv.SelectedRows[0].Cells[3].Value.ToString();
            string sodien = dgv.SelectedRows[0].Cells[4].Value.ToString();
            string thang = dgv.SelectedRows[0].Cells[5].Value.ToString();
            string trangthai = dgv.SelectedRows[0].Cells[6].Value.ToString();
            string tongtien = dgv.SelectedRows[0].Cells[7].Value.ToString();
            string nhanvienlap = dgv.SelectedRows[0].Cells[8].Value.ToString();

            var editBill = new EditBill();
            editBill.LoadEditBill(mahoadon, toanha, tenphong, sonuoc, sodien, thang, trangthai, tongtien, nhanvienlap);
            editBill.ShowDialog();
            UCHoaDon_Load(sender, e);
        }
    }
}
