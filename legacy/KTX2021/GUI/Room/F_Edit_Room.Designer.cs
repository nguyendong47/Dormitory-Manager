
namespace Dormitory_Management_2021.GUI.Phong
{
    partial class SuaPhong
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
            this.btn_Cancel = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Delete = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Update = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.nud_SoNguoiToiDa = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.cb_LoaiPhong = new Guna.UI2.WinForms.Guna2ComboBox();
            this.tb_TenPhong = new Guna.UI2.WinForms.Guna2TextBox();
            this.tb_SoNguoiToiDa = new Guna.UI2.WinForms.Guna2TextBox();
            this.tb_ToaNha = new Guna.UI2.WinForms.Guna2TextBox();
            this.tb_LoaiPhong = new Guna.UI2.WinForms.Guna2TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SoNguoiToiDa)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.AutoRoundedCorners = true;
            this.btn_Cancel.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Cancel.BorderRadius = 29;
            this.btn_Cancel.CheckedState.Parent = this.btn_Cancel;
            this.btn_Cancel.CustomImages.Parent = this.btn_Cancel;
            this.btn_Cancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_Cancel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.ForeColor = System.Drawing.Color.White;
            this.btn_Cancel.HoverState.Parent = this.btn_Cancel;
            this.btn_Cancel.Image = global::Dormitory_Management_2021.Properties.Resources.back_500px;
            this.btn_Cancel.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Cancel.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Cancel.Location = new System.Drawing.Point(76, 479);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.ShadowDecoration.Parent = this.btn_Cancel;
            this.btn_Cancel.Size = new System.Drawing.Size(230, 60);
            this.btn_Cancel.TabIndex = 66;
            this.btn_Cancel.Text = "Trở lại";
            this.btn_Cancel.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.AutoRoundedCorners = true;
            this.btn_Delete.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Delete.BorderRadius = 29;
            this.btn_Delete.CheckedState.Parent = this.btn_Delete;
            this.btn_Delete.CustomImages.Parent = this.btn_Delete;
            this.btn_Delete.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_Delete.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Delete.ForeColor = System.Drawing.Color.White;
            this.btn_Delete.HoverState.Parent = this.btn_Delete;
            this.btn_Delete.Image = global::Dormitory_Management_2021.Properties.Resources.delete_500px;
            this.btn_Delete.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Delete.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Delete.Location = new System.Drawing.Point(76, 413);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.ShadowDecoration.Parent = this.btn_Delete;
            this.btn_Delete.Size = new System.Drawing.Size(230, 60);
            this.btn_Delete.TabIndex = 64;
            this.btn_Delete.Text = "Xoá";
            this.btn_Delete.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_Update
            // 
            this.btn_Update.AutoRoundedCorners = true;
            this.btn_Update.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Update.BorderRadius = 29;
            this.btn_Update.CheckedState.Parent = this.btn_Update;
            this.btn_Update.CustomImages.Parent = this.btn_Update;
            this.btn_Update.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btn_Update.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Update.ForeColor = System.Drawing.Color.White;
            this.btn_Update.HoverState.Parent = this.btn_Update;
            this.btn_Update.Image = global::Dormitory_Management_2021.Properties.Resources.restart_500px;
            this.btn_Update.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Update.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Update.Location = new System.Drawing.Point(76, 347);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.ShadowDecoration.Parent = this.btn_Update;
            this.btn_Update.Size = new System.Drawing.Size(230, 60);
            this.btn_Update.TabIndex = 65;
            this.btn_Update.Text = "Cập nhật";
            this.btn_Update.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this;
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
            this.nud_SoNguoiToiDa.Location = new System.Drawing.Point(100, 278);
            this.nud_SoNguoiToiDa.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.nud_SoNguoiToiDa.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nud_SoNguoiToiDa.Name = "nud_SoNguoiToiDa";
            this.nud_SoNguoiToiDa.ShadowDecoration.Parent = this.nud_SoNguoiToiDa;
            this.nud_SoNguoiToiDa.Size = new System.Drawing.Size(233, 55);
            this.nud_SoNguoiToiDa.TabIndex = 72;
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
            this.cb_LoaiPhong.Location = new System.Drawing.Point(100, 217);
            this.cb_LoaiPhong.Name = "cb_LoaiPhong";
            this.cb_LoaiPhong.ShadowDecoration.Parent = this.cb_LoaiPhong;
            this.cb_LoaiPhong.Size = new System.Drawing.Size(245, 36);
            this.cb_LoaiPhong.TabIndex = 70;
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
            this.tb_TenPhong.Location = new System.Drawing.Point(34, 132);
            this.tb_TenPhong.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_TenPhong.Name = "tb_TenPhong";
            this.tb_TenPhong.PasswordChar = '\0';
            this.tb_TenPhong.PlaceholderText = "Tên phòng";
            this.tb_TenPhong.SelectedText = "";
            this.tb_TenPhong.ShadowDecoration.Parent = this.tb_TenPhong;
            this.tb_TenPhong.Size = new System.Drawing.Size(320, 60);
            this.tb_TenPhong.TabIndex = 69;
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
            this.tb_SoNguoiToiDa.IconLeft = global::Dormitory_Management_2021.Properties.Resources.family_500px;
            this.tb_SoNguoiToiDa.IconLeftSize = new System.Drawing.Size(60, 60);
            this.tb_SoNguoiToiDa.Location = new System.Drawing.Point(34, 276);
            this.tb_SoNguoiToiDa.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_SoNguoiToiDa.Name = "tb_SoNguoiToiDa";
            this.tb_SoNguoiToiDa.PasswordChar = '\0';
            this.tb_SoNguoiToiDa.PlaceholderText = "Số người tối đa";
            this.tb_SoNguoiToiDa.SelectedText = "";
            this.tb_SoNguoiToiDa.ShadowDecoration.Parent = this.tb_SoNguoiToiDa;
            this.tb_SoNguoiToiDa.Size = new System.Drawing.Size(320, 60);
            this.tb_SoNguoiToiDa.TabIndex = 68;
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
            this.tb_ToaNha.Location = new System.Drawing.Point(34, 60);
            this.tb_ToaNha.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_ToaNha.Name = "tb_ToaNha";
            this.tb_ToaNha.PasswordChar = '\0';
            this.tb_ToaNha.PlaceholderText = "Toà nhà";
            this.tb_ToaNha.SelectedText = "";
            this.tb_ToaNha.ShadowDecoration.Parent = this.tb_ToaNha;
            this.tb_ToaNha.Size = new System.Drawing.Size(320, 60);
            this.tb_ToaNha.TabIndex = 67;
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
            this.tb_LoaiPhong.Location = new System.Drawing.Point(34, 204);
            this.tb_LoaiPhong.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.tb_LoaiPhong.Name = "tb_LoaiPhong";
            this.tb_LoaiPhong.PasswordChar = '\0';
            this.tb_LoaiPhong.PlaceholderText = "Tên phòng";
            this.tb_LoaiPhong.SelectedText = "";
            this.tb_LoaiPhong.ShadowDecoration.Parent = this.tb_LoaiPhong;
            this.tb_LoaiPhong.Size = new System.Drawing.Size(320, 60);
            this.tb_LoaiPhong.TabIndex = 71;
            // 
            // SuaPhong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.ClientSize = new System.Drawing.Size(400, 600);
            this.Controls.Add(this.nud_SoNguoiToiDa);
            this.Controls.Add(this.cb_LoaiPhong);
            this.Controls.Add(this.tb_TenPhong);
            this.Controls.Add(this.tb_SoNguoiToiDa);
            this.Controls.Add(this.tb_ToaNha);
            this.Controls.Add(this.tb_LoaiPhong);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Update);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SuaPhong";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SuaPhong";
            ((System.ComponentModel.ISupportInitialize)(this.nud_SoNguoiToiDa)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btn_Cancel;
        private Guna.UI2.WinForms.Guna2Button btn_Delete;
        private Guna.UI2.WinForms.Guna2Button btn_Update;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2NumericUpDown nud_SoNguoiToiDa;
        private Guna.UI2.WinForms.Guna2ComboBox cb_LoaiPhong;
        private Guna.UI2.WinForms.Guna2TextBox tb_TenPhong;
        private Guna.UI2.WinForms.Guna2TextBox tb_SoNguoiToiDa;
        private Guna.UI2.WinForms.Guna2TextBox tb_ToaNha;
        private Guna.UI2.WinForms.Guna2TextBox tb_LoaiPhong;
    }
}