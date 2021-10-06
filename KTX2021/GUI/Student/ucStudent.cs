using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.SinhVien
{
    public partial class UC_SinhVien : UserControl
    {
        public UC_SinhVien()
        {
            InitializeComponent();
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            string maSV = dgv.SelectedRows[0].Cells[0].Value.ToString();
            string tenSV = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string ngaySinh = dgv.SelectedRows[0].Cells[2].Value.ToString();
            string gioiTinh = dgv.SelectedRows[0].Cells[3].Value.ToString();
            string cmnd = dgv.SelectedRows[0].Cells[4].Value.ToString();
            string queQuan = dgv.SelectedRows[0].Cells[5].Value.ToString();
            string danToc = dgv.SelectedRows[0].Cells[6].Value.ToString();
            string lop = dgv.SelectedRows[0].Cells[7].Value.ToString();
            string sdt = dgv.SelectedRows[0].Cells[8].Value.ToString();
            string tenPhuHuynh = dgv.SelectedRows[0].Cells[9].Value.ToString();
            string sdtPhuHuynh = dgv.SelectedRows[0].Cells[10].Value.ToString();

            SuaSinhVien suaSinhVien = new SuaSinhVien();

            try
            {

                suaSinhVien.LoadSuaSinhVien(maSV, tenSV, ngaySinh, gioiTinh, cmnd, queQuan, danToc, lop, sdt, tenPhuHuynh, sdtPhuHuynh);
            }
            catch (Exception)
            {

            }

            suaSinhVien.ShowDialog();
            UC_SinhVien_Load(sender, e);
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            UC_SinhVien_Load(sender, e);
        }

        private void UC_SinhVien_Load(object sender, EventArgs e)
        {
            QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
            try
            {
                dgv.DataSource = entities.LayDanhSachSinhVien();
            }
            catch (Exception)
            {
            }
        }
    }
}
