using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021.GUI.HopDong
{
    public partial class UCHopDong : UserControl
    {
        public UCHopDong()
        {
            InitializeComponent();
        }

        private void UC_HopDong_Load(object sender, EventArgs e)
        {
            QuanLyKTX2021Entities entities = new QuanLyKTX2021Entities();
            try
            {
                dgv.DataSource = entities.LayDanhSachHopDong();
            }
            catch (Exception)
            {
            }
        }

        private void dgv_DoubleClick(object sender, EventArgs e)
        {
            string mahd = dgv.SelectedRows[0].Cells[0].Value.ToString();
            string masv = dgv.SelectedRows[0].Cells[1].Value.ToString();
            string tensv = dgv.SelectedRows[0].Cells[2].Value.ToString();
            string toanha = dgv.SelectedRows[0].Cells[3].Value.ToString();
            string tenphong = dgv.SelectedRows[0].Cells[4].Value.ToString();
            string ngaylap = dgv.SelectedRows[0].Cells[5].Value.ToString();
            string ngayhethan = dgv.SelectedRows[0].Cells[6].Value.ToString();
            string tienphong = dgv.SelectedRows[0].Cells[7].Value.ToString();
            string nhanvienlap = dgv.SelectedRows[0].Cells[8].Value.ToString();

            SuaHopDong suaHopDong = new SuaHopDong();
            suaHopDong.LoadSuaHopDong(mahd, masv, tensv, toanha, tenphong, ngaylap, ngayhethan, tienphong, nhanvienlap);

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
    }
}
