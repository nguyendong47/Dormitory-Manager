
namespace Dormitory_Management_2021.GUI.Phong
{
    partial class TaoPhong
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
            this.guna2Elipse = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.tb_TenPhong = new Guna.UI2.WinForms.Guna2TextBox();
            this.tb_SoNguoiToiDa = new Guna.UI2.WinForms.Guna2TextBox();
            this.tb_ToaNha = new Guna.UI2.WinForms.Guna2TextBox();
            this.btn_Cancel = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Create = new Guna.UI2.WinForms.Guna2Button();
            this.cb_LoaiPhong = new Guna.UI2.WinForms.Guna2ComboBox();
            this.tb_LoaiPhong = new Guna.UI2.WinForms.Guna2TextBox();
            this.nud_SoNguoiToiDa = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.nud_SoNguoiToiDa)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Elipse
            // 
            this.guna2Elipse.BorderRadius = 20;
            this.guna2Elipse.TargetControl = this;
            // 
            // tb_TenPhong
            // 
            this.tb_TenPhong.AutoRoundedCorners = true;
            this.tb_TenPhong.BorderRadius = 29;
            this.tb_TenPhong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_TenPhong.DefaultText = "";
            this.tb_TenPhong.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tb_TenPhong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tb_TenPhong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_TenPhong.DisabledState.Parent = this.tb_TenPhong;
            this.tb_TenPhong.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_TenPhong.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_TenPhong.FocusedState.Parent = this.tb_TenPhong;
            this.tb_TenPhong.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_TenPhong.ForeColor = System.Drawing.Color.Black;
            this.tb_TenPhong.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_TenPhong.HoverState.Parent = this.tb_TenPhong;
            this.tb_TenPhong.IconLeft = global::Dormitory_Management_2021.Properties.Resources.hotel_room_key_500px;
            this.tb_TenPhong.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tb_TenPhong.Location = new System.Drawing.Point(40, 117);
            this.tb_TenPhong.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_TenPhong.Name = "tb_TenPhong";
            this.tb_TenPhong.PasswordChar = '\0';
            this.tb_TenPhong.PlaceholderText = "Tên phòng";
            this.tb_TenPhong.SelectedText = "";
            this.tb_TenPhong.ShadowDecoration.Parent = this.tb_TenPhong;
            this.tb_TenPhong.Size = new System.Drawing.Size(320, 60);
            this.tb_TenPhong.TabIndex = 61;
            this.tb_TenPhong.MouseHover += new System.EventHandler(this.tb_TenPhong_MouseHover);
            // 
            // tb_SoNguoiToiDa
            // 
            this.tb_SoNguoiToiDa.AutoRoundedCorners = true;
            this.tb_SoNguoiToiDa.BorderRadius = 29;
            this.tb_SoNguoiToiDa.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_SoNguoiToiDa.DefaultText = "";
            this.tb_SoNguoiToiDa.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tb_SoNguoiToiDa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tb_SoNguoiToiDa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_SoNguoiToiDa.DisabledState.Parent = this.tb_SoNguoiToiDa;
            this.tb_SoNguoiToiDa.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_SoNguoiToiDa.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_SoNguoiToiDa.FocusedState.Parent = this.tb_SoNguoiToiDa;
            this.tb_SoNguoiToiDa.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_SoNguoiToiDa.ForeColor = System.Drawing.Color.Black;
            this.tb_SoNguoiToiDa.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_SoNguoiToiDa.HoverState.Parent = this.tb_SoNguoiToiDa;
            this.tb_SoNguoiToiDa.IconLeft = global::Dormitory_Management_2021.Properties.Resources.add_user_group_man_man_500px;
            this.tb_SoNguoiToiDa.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tb_SoNguoiToiDa.Location = new System.Drawing.Point(40, 261);
            this.tb_SoNguoiToiDa.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_SoNguoiToiDa.Name = "tb_SoNguoiToiDa";
            this.tb_SoNguoiToiDa.PasswordChar = '\0';
            this.tb_SoNguoiToiDa.PlaceholderText = "Số người tối đa";
            this.tb_SoNguoiToiDa.SelectedText = "";
            this.tb_SoNguoiToiDa.ShadowDecoration.Parent = this.tb_SoNguoiToiDa;
            this.tb_SoNguoiToiDa.Size = new System.Drawing.Size(320, 60);
            this.tb_SoNguoiToiDa.TabIndex = 59;
            this.tb_SoNguoiToiDa.MouseHover += new System.EventHandler(this.tb_SoNguoiToiDa_MouseHover);
            // 
            // tb_ToaNha
            // 
            this.tb_ToaNha.AutoRoundedCorners = true;
            this.tb_ToaNha.BorderRadius = 29;
            this.tb_ToaNha.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_ToaNha.DefaultText = "";
            this.tb_ToaNha.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tb_ToaNha.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tb_ToaNha.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_ToaNha.DisabledState.Parent = this.tb_ToaNha;
            this.tb_ToaNha.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_ToaNha.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_ToaNha.FocusedState.Parent = this.tb_ToaNha;
            this.tb_ToaNha.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_ToaNha.ForeColor = System.Drawing.Color.Black;
            this.tb_ToaNha.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_ToaNha.HoverState.Parent = this.tb_ToaNha;
            this.tb_ToaNha.IconLeft = global::Dormitory_Management_2021.Properties.Resources.building_500px;
            this.tb_ToaNha.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tb_ToaNha.Location = new System.Drawing.Point(40, 45);
            this.tb_ToaNha.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_ToaNha.Name = "tb_ToaNha";
            this.tb_ToaNha.PasswordChar = '\0';
            this.tb_ToaNha.PlaceholderText = "Toà nhà";
            this.tb_ToaNha.SelectedText = "";
            this.tb_ToaNha.ShadowDecoration.Parent = this.tb_ToaNha;
            this.tb_ToaNha.Size = new System.Drawing.Size(320, 60);
            this.tb_ToaNha.TabIndex = 58;
            this.tb_ToaNha.MouseHover += new System.EventHandler(this.tb_TenKhu_MouseHover);
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
            this.btn_Cancel.Location = new System.Drawing.Point(82, 396);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.ShadowDecoration.Parent = this.btn_Cancel;
            this.btn_Cancel.Size = new System.Drawing.Size(230, 60);
            this.btn_Cancel.TabIndex = 53;
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
            this.btn_Create.Location = new System.Drawing.Point(82, 330);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.ShadowDecoration.Parent = this.btn_Create;
            this.btn_Create.Size = new System.Drawing.Size(230, 60);
            this.btn_Create.TabIndex = 52;
            this.btn_Create.Text = "Thêm mới";
            this.btn_Create.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // cb_LoaiPhong
            // 
            this.cb_LoaiPhong.AutoRoundedCorners = true;
            this.cb_LoaiPhong.BackColor = System.Drawing.Color.White;
            this.cb_LoaiPhong.BorderRadius = 17;
            this.cb_LoaiPhong.BorderThickness = 0;
            this.cb_LoaiPhong.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_LoaiPhong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_LoaiPhong.DropDownWidth = 236;
            this.cb_LoaiPhong.FocusedColor = System.Drawing.Color.Empty;
            this.cb_LoaiPhong.FocusedState.Parent = this.cb_LoaiPhong;
            this.cb_LoaiPhong.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_LoaiPhong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cb_LoaiPhong.FormattingEnabled = true;
            this.cb_LoaiPhong.HoverState.Parent = this.cb_LoaiPhong;
            this.cb_LoaiPhong.ItemHeight = 30;
            this.cb_LoaiPhong.Items.AddRange(new object[] {
            "Nam",
            "Nữ"});
            this.cb_LoaiPhong.ItemsAppearance.Parent = this.cb_LoaiPhong;
            this.cb_LoaiPhong.Location = new System.Drawing.Point(106, 202);
            this.cb_LoaiPhong.Name = "cb_LoaiPhong";
            this.cb_LoaiPhong.ShadowDecoration.Parent = this.cb_LoaiPhong;
            this.cb_LoaiPhong.Size = new System.Drawing.Size(245, 36);
            this.cb_LoaiPhong.TabIndex = 62;
            this.cb_LoaiPhong.MouseHover += new System.EventHandler(this.tb_LoaiPhong_MouseHover);
            // 
            // tb_LoaiPhong
            // 
            this.tb_LoaiPhong.AutoRoundedCorners = true;
            this.tb_LoaiPhong.BorderRadius = 29;
            this.tb_LoaiPhong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_LoaiPhong.DefaultText = "";
            this.tb_LoaiPhong.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tb_LoaiPhong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tb_LoaiPhong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_LoaiPhong.DisabledState.Parent = this.tb_LoaiPhong;
            this.tb_LoaiPhong.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tb_LoaiPhong.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_LoaiPhong.FocusedState.Parent = this.tb_LoaiPhong;
            this.tb_LoaiPhong.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_LoaiPhong.ForeColor = System.Drawing.Color.Black;
            this.tb_LoaiPhong.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tb_LoaiPhong.HoverState.Parent = this.tb_LoaiPhong;
            this.tb_LoaiPhong.IconLeft = global::Dormitory_Management_2021.Properties.Resources.hotel_bed_500px;
            this.tb_LoaiPhong.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tb_LoaiPhong.Location = new System.Drawing.Point(40, 189);
            this.tb_LoaiPhong.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_LoaiPhong.Name = "tb_LoaiPhong";
            this.tb_LoaiPhong.PasswordChar = '\0';
            this.tb_LoaiPhong.PlaceholderText = "Tên phòng";
            this.tb_LoaiPhong.SelectedText = "";
            this.tb_LoaiPhong.ShadowDecoration.Parent = this.tb_LoaiPhong;
            this.tb_LoaiPhong.Size = new System.Drawing.Size(320, 60);
            this.tb_LoaiPhong.TabIndex = 64;
            this.tb_LoaiPhong.MouseHover += new System.EventHandler(this.tb_LoaiPhong_MouseHover);
            // 
            // nud_SoNguoiToiDa
            // 
            this.nud_SoNguoiToiDa.AutoRoundedCorners = true;
            this.nud_SoNguoiToiDa.BackColor = System.Drawing.Color.White;
            this.nud_SoNguoiToiDa.BorderRadius = 26;
            this.nud_SoNguoiToiDa.BorderThickness = 0;
            this.nud_SoNguoiToiDa.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.nud_SoNguoiToiDa.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.nud_SoNguoiToiDa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.nud_SoNguoiToiDa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.nud_SoNguoiToiDa.DisabledState.Parent = this.nud_SoNguoiToiDa;
            this.nud_SoNguoiToiDa.DisabledState.UpDownButtonFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(177)))), ((int)(((byte)(177)))));
            this.nud_SoNguoiToiDa.DisabledState.UpDownButtonForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.nud_SoNguoiToiDa.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.nud_SoNguoiToiDa.FocusedState.Parent = this.nud_SoNguoiToiDa;
            this.nud_SoNguoiToiDa.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nud_SoNguoiToiDa.ForeColor = System.Drawing.Color.Black;
            this.nud_SoNguoiToiDa.Location = new System.Drawing.Point(106, 263);
            this.nud_SoNguoiToiDa.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.nud_SoNguoiToiDa.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_SoNguoiToiDa.Name = "nud_SoNguoiToiDa";
            this.nud_SoNguoiToiDa.ShadowDecoration.Parent = this.nud_SoNguoiToiDa;
            this.nud_SoNguoiToiDa.Size = new System.Drawing.Size(233, 55);
            this.nud_SoNguoiToiDa.TabIndex = 65;
            this.nud_SoNguoiToiDa.MouseHover += new System.EventHandler(this.tb_SoNguoiToiDa_MouseHover);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 0;
            this.toolTip.InitialDelay = 0;
            this.toolTip.ReshowDelay = 0;
            this.toolTip.ToolTipTitle = "Ghi chú:";
            // 
            // TaoPhong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.ClientSize = new System.Drawing.Size(400, 500);
            this.Controls.Add(this.nud_SoNguoiToiDa);
            this.Controls.Add(this.cb_LoaiPhong);
            this.Controls.Add(this.tb_TenPhong);
            this.Controls.Add(this.tb_SoNguoiToiDa);
            this.Controls.Add(this.tb_ToaNha);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Create);
            this.Controls.Add(this.tb_LoaiPhong);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TaoPhong";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TaoPhong";
            ((System.ComponentModel.ISupportInitialize)(this.nud_SoNguoiToiDa)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btn_Cancel;
        private Guna.UI2.WinForms.Guna2Button btn_Create;
        private Guna.UI2.WinForms.Guna2TextBox tb_ToaNha;
        private Guna.UI2.WinForms.Guna2TextBox tb_SoNguoiToiDa;
        private Guna.UI2.WinForms.Guna2TextBox tb_TenPhong;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse;
        private Guna.UI2.WinForms.Guna2ComboBox cb_LoaiPhong;
        private Guna.UI2.WinForms.Guna2TextBox tb_LoaiPhong;
        private Guna.UI2.WinForms.Guna2NumericUpDown nud_SoNguoiToiDa;
        private System.Windows.Forms.ToolTip toolTip;
    }
}