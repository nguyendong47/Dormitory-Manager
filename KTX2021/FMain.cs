using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTX2021
{
    public partial class FMain : Form
    {
        public FMain()
        {
            InitializeComponent();
            UC_Phong.Hide();
            UC_NhanVien.Hide();
            UC_HopDong.Hide();
            UC_SinhVien.Hide();
            UC_HoaDon.Hide();
        }

        #region Moveable no-border
        int mouseDownX, mouseDownY;
        bool mouseDown;
        private void pl_Top_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            mouseDownX = e.X;
            mouseDownY = e.Y;
        }

        private void pl_Top_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.SetDesktopLocation(MousePosition.X - mouseDownX, MousePosition.Y - mouseDownY);
            }
        }

        private void pl_Top_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }
        #endregion

        private void btnNhanVien_Click(object sender, EventArgs e)
        {
            UC_NhanVien.Show();
            UC_NhanVien.BringToFront();
        }

        private void btnHopDong_Click(object sender, EventArgs e)
        {
            UC_HopDong.Show();
            UC_HopDong.BringToFront();
        }

        private void btnSinhVien_Click(object sender, EventArgs e)
        {
            UC_SinhVien.Show();
            UC_SinhVien.BringToFront();
        }

        private void btnHoaDon_Click(object sender, EventArgs e)
        {
            UC_HoaDon.Show();
            UC_HoaDon.BringToFront();
        }

        private void btnPhong_Click(object sender, EventArgs e)
        {
            UC_Phong.Show();
            UC_Phong.BringToFront();
        }
    }
}
