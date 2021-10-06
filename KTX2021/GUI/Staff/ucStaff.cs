using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.NhanVien
{
    public partial class UCNhanVien : UserControl
    {
        public UCNhanVien()
        {
            InitializeComponent();
        }

        private void UC_NhanVien_Load(object sender, EventArgs e)
        {
            QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
            try
            {
                dgv.DataSource = entities.LayDanhSachNhanVien();
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

        private void btn_Refresh_Click_1(object sender, EventArgs e)
        {
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

            try
            {

                suaNhanVien.LoadSuaNhanVien(hoTen, cmnd, queQuan, congViec, luong, diaDiem, sdt);
            }
            catch (Exception)
            {

            }

            suaNhanVien.ShowDialog();
            UC_NhanVien_Load(sender, e);
        }
    }
}
