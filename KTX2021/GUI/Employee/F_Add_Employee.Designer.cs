
namespace Dormitory_Management_2021.GUI.NhanVien
{
    partial class ThemNhanVien
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
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.cb_DiaDiem = new Guna.UI2.WinForms.Guna2ComboBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tbSDT = new Guna.UI2.WinForms.Guna2TextBox();
            this.tbDiaDiem = new Guna.UI2.WinForms.Guna2TextBox();
            this.tbLuong = new Guna.UI2.WinForms.Guna2TextBox();
            this.tbCMND = new Guna.UI2.WinForms.Guna2TextBox();
            this.tbCongViec = new Guna.UI2.WinForms.Guna2TextBox();
            this.tbHoTen = new Guna.UI2.WinForms.Guna2TextBox();
            this.btn_Cancel = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Create = new Guna.UI2.WinForms.Guna2Button();
            this.tbQueQuan = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this;
            // 
            // cb_DiaDiem
            // 
            this.cb_DiaDiem.AutoRoundedCorners = true;
            this.cb_DiaDiem.BackColor = System.Drawing.Color.White;
            this.cb_DiaDiem.BorderRadius = 17;
            this.cb_DiaDiem.BorderThickness = 0;
            this.cb_DiaDiem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_DiaDiem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_DiaDiem.DropDownWidth = 236;
            this.cb_DiaDiem.FocusedColor = System.Drawing.Color.Empty;
            this.cb_DiaDiem.FocusedState.Parent = this.cb_DiaDiem;
            this.cb_DiaDiem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_DiaDiem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cb_DiaDiem.FormattingEnabled = true;
            this.cb_DiaDiem.HoverState.Parent = this.cb_DiaDiem;
            this.cb_DiaDiem.ItemHeight = 30;
            this.cb_DiaDiem.Items.AddRange(new object[] {
            "Nam",
            "Nữ"});
            this.cb_DiaDiem.ItemsAppearance.Parent = this.cb_DiaDiem;
            this.cb_DiaDiem.Location = new System.Drawing.Point(106, 397);
            this.cb_DiaDiem.Name = "cb_DiaDiem";
            this.cb_DiaDiem.ShadowDecoration.Parent = this.cb_DiaDiem;
            this.cb_DiaDiem.Size = new System.Drawing.Size(245, 36);
            this.cb_DiaDiem.TabIndex = 71;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 0;
            this.toolTip.InitialDelay = 0;
            this.toolTip.ReshowDelay = 0;
            this.toolTip.ToolTipTitle = "Ghi chú:";
            // 
            // tbSDT
            // 
            this.tbSDT.AutoRoundedCorners = true;
            this.tbSDT.BorderRadius = 29;
            this.tbSDT.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbSDT.DefaultText = "";
            this.tbSDT.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbSDT.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbSDT.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbSDT.DisabledState.Parent = this.tbSDT;
            this.tbSDT.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbSDT.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbSDT.FocusedState.Parent = this.tbSDT;
            this.tbSDT.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSDT.ForeColor = System.Drawing.Color.Black;
            this.tbSDT.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbSDT.HoverState.Parent = this.tbSDT;
            this.tbSDT.IconLeft = global::Dormitory_Management_2021.Properties.Resources.phone_500px;
            this.tbSDT.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tbSDT.Location = new System.Drawing.Point(40, 458);
            this.tbSDT.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tbSDT.Name = "tbSDT";
            this.tbSDT.PasswordChar = '\0';
            this.tbSDT.PlaceholderText = "Số điện thoại";
            this.tbSDT.SelectedText = "";
            this.tbSDT.ShadowDecoration.Parent = this.tbSDT;
            this.tbSDT.Size = new System.Drawing.Size(320, 60);
            this.tbSDT.TabIndex = 75;
            this.tbSDT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCMND_KeyPress);
            // 
            // tbDiaDiem
            // 
            this.tbDiaDiem.AutoRoundedCorners = true;
            this.tbDiaDiem.BorderRadius = 29;
            this.tbDiaDiem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbDiaDiem.DefaultText = "";
            this.tbDiaDiem.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbDiaDiem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbDiaDiem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbDiaDiem.DisabledState.Parent = this.tbDiaDiem;
            this.tbDiaDiem.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbDiaDiem.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbDiaDiem.FocusedState.Parent = this.tbDiaDiem;
            this.tbDiaDiem.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDiaDiem.ForeColor = System.Drawing.Color.Black;
            this.tbDiaDiem.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbDiaDiem.HoverState.Parent = this.tbDiaDiem;
            this.tbDiaDiem.IconLeft = global::Dormitory_Management_2021.Properties.Resources.place_marker_500px;
            this.tbDiaDiem.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tbDiaDiem.Location = new System.Drawing.Point(40, 386);
            this.tbDiaDiem.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tbDiaDiem.Name = "tbDiaDiem";
            this.tbDiaDiem.PasswordChar = '\0';
            this.tbDiaDiem.PlaceholderText = "Địa điểm";
            this.tbDiaDiem.SelectedText = "";
            this.tbDiaDiem.ShadowDecoration.Parent = this.tbDiaDiem;
            this.tbDiaDiem.Size = new System.Drawing.Size(320, 60);
            this.tbDiaDiem.TabIndex = 74;
            // 
            // tbLuong
            // 
            this.tbLuong.AutoRoundedCorners = true;
            this.tbLuong.BorderRadius = 29;
            this.tbLuong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbLuong.DefaultText = "";
            this.tbLuong.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbLuong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbLuong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbLuong.DisabledState.Parent = this.tbLuong;
            this.tbLuong.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbLuong.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbLuong.FocusedState.Parent = this.tbLuong;
            this.tbLuong.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLuong.ForeColor = System.Drawing.Color.Black;
            this.tbLuong.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbLuong.HoverState.Parent = this.tbLuong;
            this.tbLuong.IconLeft = global::Dormitory_Management_2021.Properties.Resources.cash_in_hand_500px;
            this.tbLuong.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tbLuong.Location = new System.Drawing.Point(40, 314);
            this.tbLuong.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tbLuong.Name = "tbLuong";
            this.tbLuong.PasswordChar = '\0';
            this.tbLuong.PlaceholderText = "Lương";
            this.tbLuong.SelectedText = "";
            this.tbLuong.ShadowDecoration.Parent = this.tbLuong;
            this.tbLuong.Size = new System.Drawing.Size(320, 60);
            this.tbLuong.TabIndex = 73;
            this.tbLuong.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCMND_KeyPress);
            // 
            // tbCMND
            // 
            this.tbCMND.AutoRoundedCorners = true;
            this.tbCMND.BorderRadius = 29;
            this.tbCMND.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbCMND.DefaultText = "";
            this.tbCMND.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbCMND.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbCMND.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbCMND.DisabledState.Parent = this.tbCMND;
            this.tbCMND.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbCMND.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbCMND.FocusedState.Parent = this.tbCMND;
            this.tbCMND.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCMND.ForeColor = System.Drawing.Color.Black;
            this.tbCMND.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbCMND.HoverState.Parent = this.tbCMND;
            this.tbCMND.IconLeft = global::Dormitory_Management_2021.Properties.Resources.contact_500px;
            this.tbCMND.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tbCMND.Location = new System.Drawing.Point(40, 98);
            this.tbCMND.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tbCMND.Name = "tbCMND";
            this.tbCMND.PasswordChar = '\0';
            this.tbCMND.PlaceholderText = "CMND";
            this.tbCMND.SelectedText = "";
            this.tbCMND.ShadowDecoration.Parent = this.tbCMND;
            this.tbCMND.Size = new System.Drawing.Size(320, 60);
            this.tbCMND.TabIndex = 70;
            this.tbCMND.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCMND_KeyPress);
            // 
            // tbCongViec
            // 
            this.tbCongViec.AutoRoundedCorners = true;
            this.tbCongViec.BorderRadius = 29;
            this.tbCongViec.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbCongViec.DefaultText = "";
            this.tbCongViec.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbCongViec.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbCongViec.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbCongViec.DisabledState.Parent = this.tbCongViec;
            this.tbCongViec.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbCongViec.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbCongViec.FocusedState.Parent = this.tbCongViec;
            this.tbCongViec.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCongViec.ForeColor = System.Drawing.Color.Black;
            this.tbCongViec.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbCongViec.HoverState.Parent = this.tbCongViec;
            this.tbCongViec.IconLeft = global::Dormitory_Management_2021.Properties.Resources.work_500px;
            this.tbCongViec.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tbCongViec.Location = new System.Drawing.Point(40, 242);
            this.tbCongViec.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tbCongViec.Name = "tbCongViec";
            this.tbCongViec.PasswordChar = '\0';
            this.tbCongViec.PlaceholderText = "Công việc";
            this.tbCongViec.SelectedText = "";
            this.tbCongViec.ShadowDecoration.Parent = this.tbCongViec;
            this.tbCongViec.Size = new System.Drawing.Size(320, 60);
            this.tbCongViec.TabIndex = 69;
            // 
            // tbHoTen
            // 
            this.tbHoTen.AutoRoundedCorners = true;
            this.tbHoTen.BorderRadius = 29;
            this.tbHoTen.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbHoTen.DefaultText = "";
            this.tbHoTen.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbHoTen.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbHoTen.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbHoTen.DisabledState.Parent = this.tbHoTen;
            this.tbHoTen.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbHoTen.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbHoTen.FocusedState.Parent = this.tbHoTen;
            this.tbHoTen.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbHoTen.ForeColor = System.Drawing.Color.Black;
            this.tbHoTen.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbHoTen.HoverState.Parent = this.tbHoTen;
            this.tbHoTen.IconLeft = global::Dormitory_Management_2021.Properties.Resources.name_tag_500px;
            this.tbHoTen.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tbHoTen.Location = new System.Drawing.Point(40, 26);
            this.tbHoTen.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tbHoTen.Name = "tbHoTen";
            this.tbHoTen.PasswordChar = '\0';
            this.tbHoTen.PlaceholderText = "Họ tên";
            this.tbHoTen.SelectedText = "";
            this.tbHoTen.ShadowDecoration.Parent = this.tbHoTen;
            this.tbHoTen.Size = new System.Drawing.Size(320, 60);
            this.tbHoTen.TabIndex = 68;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.AutoRoundedCorners = true;
            this.btn_Cancel.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Cancel.BorderRadius = 29;
            this.btn_Cancel.CheckedState.Parent = this.btn_Cancel;
            this.btn_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Cancel.CustomImages.Parent = this.btn_Cancel;
            this.btn_Cancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_Cancel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.ForeColor = System.Drawing.Color.White;
            this.btn_Cancel.HoverState.Parent = this.btn_Cancel;
            this.btn_Cancel.Image = global::Dormitory_Management_2021.Properties.Resources.back_500px;
            this.btn_Cancel.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Cancel.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Cancel.Location = new System.Drawing.Point(77, 615);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.ShadowDecoration.Parent = this.btn_Cancel;
            this.btn_Cancel.Size = new System.Drawing.Size(230, 60);
            this.btn_Cancel.TabIndex = 67;
            this.btn_Cancel.Text = "Trở lại";
            this.btn_Cancel.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Create
            // 
            this.btn_Create.AutoRoundedCorners = true;
            this.btn_Create.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Create.BorderRadius = 29;
            this.btn_Create.CheckedState.Parent = this.btn_Create;
            this.btn_Create.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Create.CustomImages.Parent = this.btn_Create;
            this.btn_Create.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btn_Create.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Create.ForeColor = System.Drawing.Color.White;
            this.btn_Create.HoverState.Parent = this.btn_Create;
            this.btn_Create.Image = global::Dormitory_Management_2021.Properties.Resources.add_new_500px;
            this.btn_Create.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Create.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Create.Location = new System.Drawing.Point(77, 549);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.ShadowDecoration.Parent = this.btn_Create;
            this.btn_Create.Size = new System.Drawing.Size(230, 60);
            this.btn_Create.TabIndex = 66;
            this.btn_Create.Text = "Thêm mới";
            this.btn_Create.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // tbQueQuan
            // 
            this.tbQueQuan.AutoRoundedCorners = true;
            this.tbQueQuan.BorderRadius = 29;
            this.tbQueQuan.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbQueQuan.DefaultText = "";
            this.tbQueQuan.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbQueQuan.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbQueQuan.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbQueQuan.DisabledState.Parent = this.tbQueQuan;
            this.tbQueQuan.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbQueQuan.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbQueQuan.FocusedState.Parent = this.tbQueQuan;
            this.tbQueQuan.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbQueQuan.ForeColor = System.Drawing.Color.Black;
            this.tbQueQuan.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbQueQuan.HoverState.Parent = this.tbQueQuan;
            this.tbQueQuan.IconLeft = global::Dormitory_Management_2021.Properties.Resources.location_500px;
            this.tbQueQuan.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tbQueQuan.Location = new System.Drawing.Point(40, 170);
            this.tbQueQuan.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tbQueQuan.Name = "tbQueQuan";
            this.tbQueQuan.PasswordChar = '\0';
            this.tbQueQuan.PlaceholderText = "Quê quán";
            this.tbQueQuan.SelectedText = "";
            this.tbQueQuan.ShadowDecoration.Parent = this.tbQueQuan;
            this.tbQueQuan.Size = new System.Drawing.Size(320, 60);
            this.tbQueQuan.TabIndex = 72;
            // 
            // ThemNhanVien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.ClientSize = new System.Drawing.Size(400, 700);
            this.Controls.Add(this.tbSDT);
            this.Controls.Add(this.tbLuong);
            this.Controls.Add(this.cb_DiaDiem);
            this.Controls.Add(this.tbCMND);
            this.Controls.Add(this.tbCongViec);
            this.Controls.Add(this.tbHoTen);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Create);
            this.Controls.Add(this.tbQueQuan);
            this.Controls.Add(this.tbDiaDiem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ThemNhanVien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ThemNhanVien";
            this.Load += new System.EventHandler(this.ThemNhanVien_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2ComboBox cb_DiaDiem;
        private Guna.UI2.WinForms.Guna2TextBox tbCMND;
        private Guna.UI2.WinForms.Guna2TextBox tbCongViec;
        private Guna.UI2.WinForms.Guna2TextBox tbHoTen;
        private Guna.UI2.WinForms.Guna2Button btn_Cancel;
        private Guna.UI2.WinForms.Guna2Button btn_Create;
        private Guna.UI2.WinForms.Guna2TextBox tbQueQuan;
        private System.Windows.Forms.ToolTip toolTip;
        private Guna.UI2.WinForms.Guna2TextBox tbSDT;
        private Guna.UI2.WinForms.Guna2TextBox tbDiaDiem;
        private Guna.UI2.WinForms.Guna2TextBox tbLuong;
    }
}