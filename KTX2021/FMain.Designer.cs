
namespace KTX2021
{
    partial class FMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pl_Top = new System.Windows.Forms.Panel();
            this.pictureBox_Logo = new System.Windows.Forms.PictureBox();
            this.label_KTXDHHP = new System.Windows.Forms.Label();
            this.panel_Left = new System.Windows.Forms.Panel();
            this.btnCaiDat = new Guna.UI.WinForms.GunaButton();
            this.btnHoSo = new Guna.UI.WinForms.GunaButton();
            this.btnDangXuat = new Guna.UI.WinForms.GunaButton();
            this.btnLienHe = new Guna.UI.WinForms.GunaButton();
            this.btnHuongDan = new Guna.UI.WinForms.GunaButton();
            this.btnThongKe = new Guna.UI.WinForms.GunaButton();
            this.btnHoaDon = new Guna.UI.WinForms.GunaButton();
            this.btnPhong = new Guna.UI.WinForms.GunaButton();
            this.btnNhanVien = new Guna.UI.WinForms.GunaButton();
            this.btnTrangChu = new Guna.UI.WinForms.GunaButton();
            this.btnHopDong = new Guna.UI.WinForms.GunaButton();
            this.btnSinhVien = new Guna.UI.WinForms.GunaButton();
            this.btnGiaHan = new Guna.UI.WinForms.GunaButton();
            this.btnTraPhong = new Guna.UI.WinForms.GunaButton();
            this.btnChuyenPhong = new Guna.UI.WinForms.GunaButton();
            this.borderRadiusFormMain = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.UC_NhanVien = new KTX2021.GUI.NhanVien.UCNhanVien();
            this.UC_Phong = new KTX2021.GUI.Phong.UCPhong();
            this.UC_HopDong = new KTX2021.GUI.HopDong.UCHopDong();
            this.UC_SinhVien = new KTX2021.GUI.SinhVien.UC_SinhVien();
            this.UC_HoaDon = new KTX2021.GUI.HoaDon.UCHoaDon();
            this.pl_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Logo)).BeginInit();
            this.panel_Left.SuspendLayout();
            this.SuspendLayout();
            // 
            // pl_Top
            // 
            this.pl_Top.BackColor = System.Drawing.Color.White;
            this.pl_Top.Controls.Add(this.pictureBox_Logo);
            this.pl_Top.Controls.Add(this.label_KTXDHHP);
            this.pl_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pl_Top.Location = new System.Drawing.Point(0, 0);
            this.pl_Top.Name = "pl_Top";
            this.pl_Top.Size = new System.Drawing.Size(1598, 55);
            this.pl_Top.TabIndex = 34;
            this.pl_Top.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pl_Top_MouseDown);
            this.pl_Top.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pl_Top_MouseMove);
            this.pl_Top.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pl_Top_MouseUp);
            // 
            // pictureBox_Logo
            // 
            this.pictureBox_Logo.Image = global::KTX2021.Properties.Resources.DHHP;
            this.pictureBox_Logo.Location = new System.Drawing.Point(12, 4);
            this.pictureBox_Logo.Name = "pictureBox_Logo";
            this.pictureBox_Logo.Size = new System.Drawing.Size(45, 45);
            this.pictureBox_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Logo.TabIndex = 0;
            this.pictureBox_Logo.TabStop = false;
            // 
            // label_KTXDHHP
            // 
            this.label_KTXDHHP.AutoSize = true;
            this.label_KTXDHHP.Font = new System.Drawing.Font("Open Sans SemiBold", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_KTXDHHP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(151)))), ((int)(((byte)(153)))));
            this.label_KTXDHHP.Location = new System.Drawing.Point(69, 12);
            this.label_KTXDHHP.Name = "label_KTXDHHP";
            this.label_KTXDHHP.Size = new System.Drawing.Size(115, 28);
            this.label_KTXDHHP.TabIndex = 36;
            this.label_KTXDHHP.Text = "KTX DHHP";
            // 
            // panel_Left
            // 
            this.panel_Left.BackColor = System.Drawing.Color.White;
            this.panel_Left.Controls.Add(this.btnCaiDat);
            this.panel_Left.Controls.Add(this.btnHoSo);
            this.panel_Left.Controls.Add(this.btnDangXuat);
            this.panel_Left.Controls.Add(this.btnLienHe);
            this.panel_Left.Controls.Add(this.btnHuongDan);
            this.panel_Left.Controls.Add(this.btnThongKe);
            this.panel_Left.Controls.Add(this.btnHoaDon);
            this.panel_Left.Controls.Add(this.btnPhong);
            this.panel_Left.Controls.Add(this.btnNhanVien);
            this.panel_Left.Controls.Add(this.btnTrangChu);
            this.panel_Left.Controls.Add(this.btnHopDong);
            this.panel_Left.Controls.Add(this.btnSinhVien);
            this.panel_Left.Controls.Add(this.btnGiaHan);
            this.panel_Left.Controls.Add(this.btnTraPhong);
            this.panel_Left.Controls.Add(this.btnChuyenPhong);
            this.panel_Left.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_Left.Location = new System.Drawing.Point(0, 55);
            this.panel_Left.Name = "panel_Left";
            this.panel_Left.Size = new System.Drawing.Size(193, 883);
            this.panel_Left.TabIndex = 35;
            // 
            // btnCaiDat
            // 
            this.btnCaiDat.AnimationHoverSpeed = 0.07F;
            this.btnCaiDat.AnimationSpeed = 0.03F;
            this.btnCaiDat.BackColor = System.Drawing.Color.Transparent;
            this.btnCaiDat.BaseColor = System.Drawing.Color.White;
            this.btnCaiDat.BorderColor = System.Drawing.Color.Black;
            this.btnCaiDat.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnCaiDat.FocusedColor = System.Drawing.Color.Empty;
            this.btnCaiDat.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCaiDat.ForeColor = System.Drawing.Color.Black;
            this.btnCaiDat.Image = global::KTX2021.Properties.Resources.setting;
            this.btnCaiDat.ImageSize = new System.Drawing.Size(20, 20);
            this.btnCaiDat.Location = new System.Drawing.Point(13, 687);
            this.btnCaiDat.Name = "btnCaiDat";
            this.btnCaiDat.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnCaiDat.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnCaiDat.OnHoverForeColor = System.Drawing.Color.White;
            this.btnCaiDat.OnHoverImage = null;
            this.btnCaiDat.OnPressedColor = System.Drawing.Color.Black;
            this.btnCaiDat.Radius = 15;
            this.btnCaiDat.Size = new System.Drawing.Size(171, 42);
            this.btnCaiDat.TabIndex = 37;
            this.btnCaiDat.Text = "Cài đặt";
            // 
            // btnHoSo
            // 
            this.btnHoSo.AnimationHoverSpeed = 0.07F;
            this.btnHoSo.AnimationSpeed = 0.03F;
            this.btnHoSo.BackColor = System.Drawing.Color.Transparent;
            this.btnHoSo.BaseColor = System.Drawing.Color.White;
            this.btnHoSo.BorderColor = System.Drawing.Color.Black;
            this.btnHoSo.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnHoSo.FocusedColor = System.Drawing.Color.Empty;
            this.btnHoSo.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoSo.ForeColor = System.Drawing.Color.Black;
            this.btnHoSo.Image = global::KTX2021.Properties.Resources.profile;
            this.btnHoSo.ImageSize = new System.Drawing.Size(20, 20);
            this.btnHoSo.Location = new System.Drawing.Point(13, 549);
            this.btnHoSo.Name = "btnHoSo";
            this.btnHoSo.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnHoSo.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnHoSo.OnHoverForeColor = System.Drawing.Color.White;
            this.btnHoSo.OnHoverImage = null;
            this.btnHoSo.OnPressedColor = System.Drawing.Color.Black;
            this.btnHoSo.Radius = 15;
            this.btnHoSo.Size = new System.Drawing.Size(171, 42);
            this.btnHoSo.TabIndex = 40;
            this.btnHoSo.Text = "Hồ sơ";
            // 
            // btnDangXuat
            // 
            this.btnDangXuat.AnimationHoverSpeed = 0.07F;
            this.btnDangXuat.AnimationSpeed = 0.03F;
            this.btnDangXuat.BackColor = System.Drawing.Color.Transparent;
            this.btnDangXuat.BaseColor = System.Drawing.Color.White;
            this.btnDangXuat.BorderColor = System.Drawing.Color.Black;
            this.btnDangXuat.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnDangXuat.FocusedColor = System.Drawing.Color.Empty;
            this.btnDangXuat.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDangXuat.ForeColor = System.Drawing.Color.Black;
            this.btnDangXuat.Image = global::KTX2021.Properties.Resources.logout;
            this.btnDangXuat.ImageSize = new System.Drawing.Size(20, 20);
            this.btnDangXuat.Location = new System.Drawing.Point(13, 790);
            this.btnDangXuat.Name = "btnDangXuat";
            this.btnDangXuat.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDangXuat.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnDangXuat.OnHoverForeColor = System.Drawing.Color.White;
            this.btnDangXuat.OnHoverImage = null;
            this.btnDangXuat.OnPressedColor = System.Drawing.Color.Black;
            this.btnDangXuat.Radius = 15;
            this.btnDangXuat.Size = new System.Drawing.Size(171, 42);
            this.btnDangXuat.TabIndex = 50;
            this.btnDangXuat.Text = "Đăng xuất";
            // 
            // btnLienHe
            // 
            this.btnLienHe.AnimationHoverSpeed = 0.07F;
            this.btnLienHe.AnimationSpeed = 0.03F;
            this.btnLienHe.BackColor = System.Drawing.Color.Transparent;
            this.btnLienHe.BaseColor = System.Drawing.Color.White;
            this.btnLienHe.BorderColor = System.Drawing.Color.Black;
            this.btnLienHe.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnLienHe.FocusedColor = System.Drawing.Color.Empty;
            this.btnLienHe.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLienHe.ForeColor = System.Drawing.Color.Black;
            this.btnLienHe.Image = global::KTX2021.Properties.Resources.contact;
            this.btnLienHe.ImageSize = new System.Drawing.Size(20, 20);
            this.btnLienHe.Location = new System.Drawing.Point(13, 595);
            this.btnLienHe.Name = "btnLienHe";
            this.btnLienHe.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnLienHe.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnLienHe.OnHoverForeColor = System.Drawing.Color.White;
            this.btnLienHe.OnHoverImage = null;
            this.btnLienHe.OnPressedColor = System.Drawing.Color.Black;
            this.btnLienHe.Radius = 15;
            this.btnLienHe.Size = new System.Drawing.Size(171, 42);
            this.btnLienHe.TabIndex = 39;
            this.btnLienHe.Text = "Liên hệ";
            // 
            // btnHuongDan
            // 
            this.btnHuongDan.AnimationHoverSpeed = 0.07F;
            this.btnHuongDan.AnimationSpeed = 0.03F;
            this.btnHuongDan.BackColor = System.Drawing.Color.Transparent;
            this.btnHuongDan.BaseColor = System.Drawing.Color.White;
            this.btnHuongDan.BorderColor = System.Drawing.Color.Black;
            this.btnHuongDan.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnHuongDan.FocusedColor = System.Drawing.Color.Empty;
            this.btnHuongDan.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHuongDan.ForeColor = System.Drawing.Color.Black;
            this.btnHuongDan.Image = global::KTX2021.Properties.Resources.search;
            this.btnHuongDan.ImageSize = new System.Drawing.Size(20, 20);
            this.btnHuongDan.Location = new System.Drawing.Point(13, 641);
            this.btnHuongDan.Name = "btnHuongDan";
            this.btnHuongDan.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnHuongDan.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnHuongDan.OnHoverForeColor = System.Drawing.Color.White;
            this.btnHuongDan.OnHoverImage = null;
            this.btnHuongDan.OnPressedColor = System.Drawing.Color.Black;
            this.btnHuongDan.Radius = 15;
            this.btnHuongDan.Size = new System.Drawing.Size(171, 42);
            this.btnHuongDan.TabIndex = 38;
            this.btnHuongDan.Text = "Hướng dẫn";
            // 
            // btnThongKe
            // 
            this.btnThongKe.AnimationHoverSpeed = 0.07F;
            this.btnThongKe.AnimationSpeed = 0.03F;
            this.btnThongKe.BackColor = System.Drawing.Color.Transparent;
            this.btnThongKe.BaseColor = System.Drawing.Color.White;
            this.btnThongKe.BorderColor = System.Drawing.Color.Black;
            this.btnThongKe.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnThongKe.FocusedColor = System.Drawing.Color.Empty;
            this.btnThongKe.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThongKe.ForeColor = System.Drawing.Color.Black;
            this.btnThongKe.Image = global::KTX2021.Properties.Resources.bar_chart;
            this.btnThongKe.ImageSize = new System.Drawing.Size(20, 20);
            this.btnThongKe.Location = new System.Drawing.Point(13, 426);
            this.btnThongKe.Name = "btnThongKe";
            this.btnThongKe.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnThongKe.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnThongKe.OnHoverForeColor = System.Drawing.Color.White;
            this.btnThongKe.OnHoverImage = null;
            this.btnThongKe.OnPressedColor = System.Drawing.Color.Black;
            this.btnThongKe.Radius = 15;
            this.btnThongKe.Size = new System.Drawing.Size(171, 42);
            this.btnThongKe.TabIndex = 41;
            this.btnThongKe.Text = "Thống kê";
            // 
            // btnHoaDon
            // 
            this.btnHoaDon.AnimationHoverSpeed = 0.07F;
            this.btnHoaDon.AnimationSpeed = 0.03F;
            this.btnHoaDon.BackColor = System.Drawing.Color.Transparent;
            this.btnHoaDon.BaseColor = System.Drawing.Color.White;
            this.btnHoaDon.BorderColor = System.Drawing.Color.Black;
            this.btnHoaDon.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnHoaDon.FocusedColor = System.Drawing.Color.Empty;
            this.btnHoaDon.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoaDon.ForeColor = System.Drawing.Color.Black;
            this.btnHoaDon.Image = global::KTX2021.Properties.Resources.bill;
            this.btnHoaDon.ImageSize = new System.Drawing.Size(20, 20);
            this.btnHoaDon.Location = new System.Drawing.Point(13, 380);
            this.btnHoaDon.Name = "btnHoaDon";
            this.btnHoaDon.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnHoaDon.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnHoaDon.OnHoverForeColor = System.Drawing.Color.White;
            this.btnHoaDon.OnHoverImage = null;
            this.btnHoaDon.OnPressedColor = System.Drawing.Color.Black;
            this.btnHoaDon.Radius = 15;
            this.btnHoaDon.Size = new System.Drawing.Size(171, 42);
            this.btnHoaDon.TabIndex = 42;
            this.btnHoaDon.Text = "Hoá đơn";
            this.btnHoaDon.Click += new System.EventHandler(this.btnHoaDon_Click);
            // 
            // btnPhong
            // 
            this.btnPhong.AnimationHoverSpeed = 0.07F;
            this.btnPhong.AnimationSpeed = 0.03F;
            this.btnPhong.BackColor = System.Drawing.Color.Transparent;
            this.btnPhong.BaseColor = System.Drawing.Color.White;
            this.btnPhong.BorderColor = System.Drawing.Color.Black;
            this.btnPhong.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnPhong.FocusedColor = System.Drawing.Color.Empty;
            this.btnPhong.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPhong.ForeColor = System.Drawing.Color.Black;
            this.btnPhong.Image = global::KTX2021.Properties.Resources.home;
            this.btnPhong.ImageSize = new System.Drawing.Size(20, 20);
            this.btnPhong.Location = new System.Drawing.Point(13, 58);
            this.btnPhong.Name = "btnPhong";
            this.btnPhong.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnPhong.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnPhong.OnHoverForeColor = System.Drawing.Color.White;
            this.btnPhong.OnHoverImage = null;
            this.btnPhong.OnPressedColor = System.Drawing.Color.Black;
            this.btnPhong.Radius = 15;
            this.btnPhong.Size = new System.Drawing.Size(171, 42);
            this.btnPhong.TabIndex = 49;
            this.btnPhong.Text = "Phòng";
            this.btnPhong.Click += new System.EventHandler(this.btnPhong_Click);
            // 
            // btnNhanVien
            // 
            this.btnNhanVien.AnimationHoverSpeed = 0.07F;
            this.btnNhanVien.AnimationSpeed = 0.03F;
            this.btnNhanVien.BackColor = System.Drawing.Color.Transparent;
            this.btnNhanVien.BaseColor = System.Drawing.Color.White;
            this.btnNhanVien.BorderColor = System.Drawing.Color.Black;
            this.btnNhanVien.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnNhanVien.FocusedColor = System.Drawing.Color.Empty;
            this.btnNhanVien.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNhanVien.ForeColor = System.Drawing.Color.Black;
            this.btnNhanVien.Image = global::KTX2021.Properties.Resources.employee;
            this.btnNhanVien.ImageSize = new System.Drawing.Size(20, 20);
            this.btnNhanVien.Location = new System.Drawing.Point(13, 104);
            this.btnNhanVien.Name = "btnNhanVien";
            this.btnNhanVien.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnNhanVien.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnNhanVien.OnHoverForeColor = System.Drawing.Color.White;
            this.btnNhanVien.OnHoverImage = null;
            this.btnNhanVien.OnPressedColor = System.Drawing.Color.Black;
            this.btnNhanVien.Radius = 15;
            this.btnNhanVien.Size = new System.Drawing.Size(171, 42);
            this.btnNhanVien.TabIndex = 48;
            this.btnNhanVien.Text = "Nhân viên";
            this.btnNhanVien.Click += new System.EventHandler(this.btnNhanVien_Click);
            // 
            // btnTrangChu
            // 
            this.btnTrangChu.AnimationHoverSpeed = 0.07F;
            this.btnTrangChu.AnimationSpeed = 0.03F;
            this.btnTrangChu.BackColor = System.Drawing.Color.Transparent;
            this.btnTrangChu.BaseColor = System.Drawing.Color.White;
            this.btnTrangChu.BorderColor = System.Drawing.Color.Black;
            this.btnTrangChu.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnTrangChu.FocusedColor = System.Drawing.Color.Empty;
            this.btnTrangChu.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrangChu.ForeColor = System.Drawing.Color.Black;
            this.btnTrangChu.Image = global::KTX2021.Properties.Resources.dashboard;
            this.btnTrangChu.ImageSize = new System.Drawing.Size(20, 20);
            this.btnTrangChu.Location = new System.Drawing.Point(13, 12);
            this.btnTrangChu.Name = "btnTrangChu";
            this.btnTrangChu.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTrangChu.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnTrangChu.OnHoverForeColor = System.Drawing.Color.White;
            this.btnTrangChu.OnHoverImage = null;
            this.btnTrangChu.OnPressedColor = System.Drawing.Color.Black;
            this.btnTrangChu.Radius = 15;
            this.btnTrangChu.Size = new System.Drawing.Size(171, 42);
            this.btnTrangChu.TabIndex = 36;
            this.btnTrangChu.Text = "Trang chủ";
            // 
            // btnHopDong
            // 
            this.btnHopDong.AnimationHoverSpeed = 0.07F;
            this.btnHopDong.AnimationSpeed = 0.03F;
            this.btnHopDong.BackColor = System.Drawing.Color.Transparent;
            this.btnHopDong.BaseColor = System.Drawing.Color.White;
            this.btnHopDong.BorderColor = System.Drawing.Color.Black;
            this.btnHopDong.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnHopDong.FocusedColor = System.Drawing.Color.Empty;
            this.btnHopDong.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHopDong.ForeColor = System.Drawing.Color.Black;
            this.btnHopDong.Image = global::KTX2021.Properties.Resources.contract;
            this.btnHopDong.ImageSize = new System.Drawing.Size(20, 20);
            this.btnHopDong.Location = new System.Drawing.Point(13, 150);
            this.btnHopDong.Name = "btnHopDong";
            this.btnHopDong.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnHopDong.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnHopDong.OnHoverForeColor = System.Drawing.Color.White;
            this.btnHopDong.OnHoverImage = null;
            this.btnHopDong.OnPressedColor = System.Drawing.Color.Black;
            this.btnHopDong.Radius = 15;
            this.btnHopDong.Size = new System.Drawing.Size(171, 42);
            this.btnHopDong.TabIndex = 47;
            this.btnHopDong.Text = "Hợp đồng";
            this.btnHopDong.Click += new System.EventHandler(this.btnHopDong_Click);
            // 
            // btnSinhVien
            // 
            this.btnSinhVien.AnimationHoverSpeed = 0.07F;
            this.btnSinhVien.AnimationSpeed = 0.03F;
            this.btnSinhVien.BackColor = System.Drawing.Color.Transparent;
            this.btnSinhVien.BaseColor = System.Drawing.Color.White;
            this.btnSinhVien.BorderColor = System.Drawing.Color.Black;
            this.btnSinhVien.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnSinhVien.FocusedColor = System.Drawing.Color.Empty;
            this.btnSinhVien.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSinhVien.ForeColor = System.Drawing.Color.Black;
            this.btnSinhVien.Image = global::KTX2021.Properties.Resources.graduated;
            this.btnSinhVien.ImageSize = new System.Drawing.Size(20, 20);
            this.btnSinhVien.Location = new System.Drawing.Point(13, 334);
            this.btnSinhVien.Name = "btnSinhVien";
            this.btnSinhVien.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnSinhVien.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnSinhVien.OnHoverForeColor = System.Drawing.Color.White;
            this.btnSinhVien.OnHoverImage = null;
            this.btnSinhVien.OnPressedColor = System.Drawing.Color.Black;
            this.btnSinhVien.Radius = 15;
            this.btnSinhVien.Size = new System.Drawing.Size(171, 42);
            this.btnSinhVien.TabIndex = 43;
            this.btnSinhVien.Text = "Sinh viên";
            this.btnSinhVien.Click += new System.EventHandler(this.btnSinhVien_Click);
            // 
            // btnGiaHan
            // 
            this.btnGiaHan.AnimationHoverSpeed = 0.07F;
            this.btnGiaHan.AnimationSpeed = 0.03F;
            this.btnGiaHan.BackColor = System.Drawing.Color.Transparent;
            this.btnGiaHan.BaseColor = System.Drawing.Color.White;
            this.btnGiaHan.BorderColor = System.Drawing.Color.Black;
            this.btnGiaHan.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnGiaHan.FocusedColor = System.Drawing.Color.Empty;
            this.btnGiaHan.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGiaHan.ForeColor = System.Drawing.Color.Black;
            this.btnGiaHan.Image = global::KTX2021.Properties.Resources.extension;
            this.btnGiaHan.ImageSize = new System.Drawing.Size(20, 20);
            this.btnGiaHan.Location = new System.Drawing.Point(34, 196);
            this.btnGiaHan.Name = "btnGiaHan";
            this.btnGiaHan.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnGiaHan.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnGiaHan.OnHoverForeColor = System.Drawing.Color.White;
            this.btnGiaHan.OnHoverImage = null;
            this.btnGiaHan.OnPressedColor = System.Drawing.Color.Black;
            this.btnGiaHan.Radius = 15;
            this.btnGiaHan.Size = new System.Drawing.Size(150, 42);
            this.btnGiaHan.TabIndex = 46;
            this.btnGiaHan.Text = "Gia hạn";
            // 
            // btnTraPhong
            // 
            this.btnTraPhong.AnimationHoverSpeed = 0.07F;
            this.btnTraPhong.AnimationSpeed = 0.03F;
            this.btnTraPhong.BackColor = System.Drawing.Color.Transparent;
            this.btnTraPhong.BaseColor = System.Drawing.Color.White;
            this.btnTraPhong.BorderColor = System.Drawing.Color.Black;
            this.btnTraPhong.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnTraPhong.FocusedColor = System.Drawing.Color.Empty;
            this.btnTraPhong.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTraPhong.ForeColor = System.Drawing.Color.Black;
            this.btnTraPhong.Image = global::KTX2021.Properties.Resources.check_out;
            this.btnTraPhong.ImageSize = new System.Drawing.Size(20, 20);
            this.btnTraPhong.Location = new System.Drawing.Point(34, 288);
            this.btnTraPhong.Name = "btnTraPhong";
            this.btnTraPhong.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnTraPhong.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnTraPhong.OnHoverForeColor = System.Drawing.Color.White;
            this.btnTraPhong.OnHoverImage = null;
            this.btnTraPhong.OnPressedColor = System.Drawing.Color.Black;
            this.btnTraPhong.Radius = 15;
            this.btnTraPhong.Size = new System.Drawing.Size(150, 42);
            this.btnTraPhong.TabIndex = 44;
            this.btnTraPhong.Text = "Trả phòng";
            // 
            // btnChuyenPhong
            // 
            this.btnChuyenPhong.AnimationHoverSpeed = 0.07F;
            this.btnChuyenPhong.AnimationSpeed = 0.03F;
            this.btnChuyenPhong.BackColor = System.Drawing.Color.Transparent;
            this.btnChuyenPhong.BaseColor = System.Drawing.Color.White;
            this.btnChuyenPhong.BorderColor = System.Drawing.Color.Black;
            this.btnChuyenPhong.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnChuyenPhong.FocusedColor = System.Drawing.Color.Empty;
            this.btnChuyenPhong.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChuyenPhong.ForeColor = System.Drawing.Color.Black;
            this.btnChuyenPhong.Image = global::KTX2021.Properties.Resources.agreement;
            this.btnChuyenPhong.ImageSize = new System.Drawing.Size(20, 20);
            this.btnChuyenPhong.Location = new System.Drawing.Point(34, 242);
            this.btnChuyenPhong.Name = "btnChuyenPhong";
            this.btnChuyenPhong.OnHoverBaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnChuyenPhong.OnHoverBorderColor = System.Drawing.Color.Black;
            this.btnChuyenPhong.OnHoverForeColor = System.Drawing.Color.White;
            this.btnChuyenPhong.OnHoverImage = null;
            this.btnChuyenPhong.OnPressedColor = System.Drawing.Color.Black;
            this.btnChuyenPhong.Radius = 15;
            this.btnChuyenPhong.Size = new System.Drawing.Size(150, 42);
            this.btnChuyenPhong.TabIndex = 45;
            this.btnChuyenPhong.Text = "Chuyển phòng";
            // 
            // borderRadiusFormMain
            // 
            this.borderRadiusFormMain.ElipseRadius = 20;
            this.borderRadiusFormMain.TargetControl = this;
            // 
            // UC_NhanVien
            // 
            this.UC_NhanVien.BackColor = System.Drawing.Color.WhiteSmoke;
            this.UC_NhanVien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UC_NhanVien.Location = new System.Drawing.Point(193, 55);
            this.UC_NhanVien.Name = "UC_NhanVien";
            this.UC_NhanVien.Size = new System.Drawing.Size(1405, 883);
            this.UC_NhanVien.TabIndex = 37;
            // 
            // UC_Phong
            // 
            this.UC_Phong.BackColor = System.Drawing.Color.WhiteSmoke;
            this.UC_Phong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UC_Phong.Location = new System.Drawing.Point(193, 55);
            this.UC_Phong.Name = "UC_Phong";
            this.UC_Phong.Size = new System.Drawing.Size(1405, 883);
            this.UC_Phong.TabIndex = 36;
            // 
            // UC_HopDong
            // 
            this.UC_HopDong.BackColor = System.Drawing.Color.WhiteSmoke;
            this.UC_HopDong.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UC_HopDong.Location = new System.Drawing.Point(0, 0);
            this.UC_HopDong.Name = "UC_HopDong";
            this.UC_HopDong.Size = new System.Drawing.Size(1598, 938);
            this.UC_HopDong.TabIndex = 38;
            // 
            // UC_SinhVien
            // 
            this.UC_SinhVien.BackColor = System.Drawing.Color.WhiteSmoke;
            this.UC_SinhVien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UC_SinhVien.Location = new System.Drawing.Point(0, 0);
            this.UC_SinhVien.Name = "UC_SinhVien";
            this.UC_SinhVien.Size = new System.Drawing.Size(1598, 938);
            this.UC_SinhVien.TabIndex = 39;
            // 
            // UC_HoaDon
            // 
            this.UC_HoaDon.BackColor = System.Drawing.Color.Gainsboro;
            this.UC_HoaDon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UC_HoaDon.Location = new System.Drawing.Point(193, 55);
            this.UC_HoaDon.Name = "UC_HoaDon";
            this.UC_HoaDon.Size = new System.Drawing.Size(1405, 883);
            this.UC_HoaDon.TabIndex = 40;
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1598, 938);
            this.Controls.Add(this.UC_HoaDon);
            this.Controls.Add(this.UC_NhanVien);
            this.Controls.Add(this.UC_Phong);
            this.Controls.Add(this.panel_Left);
            this.Controls.Add(this.pl_Top);
            this.Controls.Add(this.UC_HopDong);
            this.Controls.Add(this.UC_SinhVien);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FMain";
            this.pl_Top.ResumeLayout(false);
            this.pl_Top.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Logo)).EndInit();
            this.panel_Left.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pl_Top;
        private System.Windows.Forms.Panel panel_Left;
        private System.Windows.Forms.PictureBox pictureBox_Logo;
        private System.Windows.Forms.Label label_KTXDHHP;
        private Bunifu.Framework.UI.BunifuElipse borderRadiusFormMain;
        private Guna.UI.WinForms.GunaButton btnTrangChu;
        private Guna.UI.WinForms.GunaButton btnPhong;
        private Guna.UI.WinForms.GunaButton btnNhanVien;
        private Guna.UI.WinForms.GunaButton btnHopDong;
        private Guna.UI.WinForms.GunaButton btnGiaHan;
        private Guna.UI.WinForms.GunaButton btnChuyenPhong;
        private Guna.UI.WinForms.GunaButton btnTraPhong;
        private Guna.UI.WinForms.GunaButton btnSinhVien;
        private Guna.UI.WinForms.GunaButton btnHoaDon;
        private Guna.UI.WinForms.GunaButton btnThongKe;
        private Guna.UI.WinForms.GunaButton btnHoSo;
        private Guna.UI.WinForms.GunaButton btnLienHe;
        private Guna.UI.WinForms.GunaButton btnHuongDan;
        private Guna.UI.WinForms.GunaButton btnCaiDat;
        private Guna.UI.WinForms.GunaButton btnDangXuat;
        private GUI.Phong.UCPhong UC_Phong;
        private GUI.NhanVien.UCNhanVien UC_NhanVien;
        private GUI.HopDong.UCHopDong UC_HopDong;
        private GUI.SinhVien.UC_SinhVien UC_SinhVien;
        private GUI.HoaDon.UCHoaDon UC_HoaDon;
    }
}