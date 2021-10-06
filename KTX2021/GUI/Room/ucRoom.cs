using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.Phong
{
    public partial class UCPhong : UserControl
    {
        public UCPhong()
        {
            InitializeComponent();
        }

        private void UC_Phong_Load(object sender, EventArgs e)
        {
            QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
            try
            {
                dgv.DataSource = entities.LayDanhSachPhong();
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
                using (QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities())
                {
                    string tenphong = txtSearch.Text;
                    dgv.DataSource = entities.TimPhong(tenphong);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
