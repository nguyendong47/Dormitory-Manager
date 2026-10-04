
namespace Dormitory_Management_2021.GUI.Contract
{
    partial class F_Search_Room
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.btnTinhTrang = new Guna.UI2.WinForms.Guna2Button();
            this.cbTinhTrang = new Guna.UI2.WinForms.Guna2ComboBox();
            this.btnLoaiPhong = new Guna.UI2.WinForms.Guna2Button();
            this.cbLoaiPhong = new Guna.UI2.WinForms.Guna2ComboBox();
            this.btnLayPhongTheoToaNha = new Guna.UI2.WinForms.Guna2Button();
            this.cbToaNha = new Guna.UI2.WinForms.Guna2ComboBox();
            this.tbTimTenPhong = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnTimTenPhong = new Guna.UI2.WinForms.Guna2Button();
            this.btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(209)))), ((int)(((byte)(161)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(209)))), ((int)(((byte)(161)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv.ColumnHeadersHeight = 60;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgv.DoubleBuffered = true;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.HeaderBgColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(209)))), ((int)(((byte)(161)))));
            this.dgv.HeaderForeColor = System.Drawing.Color.White;
            this.dgv.Location = new System.Drawing.Point(16, 117);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgv.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dgv.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgv.RowTemplate.Height = 50;
            this.dgv.RowTemplate.ReadOnly = true;
            this.dgv.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(1379, 733);
            this.dgv.TabIndex = 43;
            this.dgv.DoubleClick += new System.EventHandler(this.dgv_DoubleClick);
            // 
            // btnTinhTrang
            // 
            this.btnTinhTrang.AutoRoundedCorners = true;
            this.btnTinhTrang.BorderColor = System.Drawing.Color.Transparent;
            this.btnTinhTrang.BorderRadius = 27;
            this.btnTinhTrang.CheckedState.Parent = this.btnTinhTrang;
            this.btnTinhTrang.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTinhTrang.CustomImages.Parent = this.btnTinhTrang;
            this.btnTinhTrang.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btnTinhTrang.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTinhTrang.ForeColor = System.Drawing.Color.White;
            this.btnTinhTrang.HoverState.Parent = this.btnTinhTrang;
            this.btnTinhTrang.Image = global::Dormitory_Management_2021.Properties.Resources.family_500px;
            this.btnTinhTrang.ImageSize = new System.Drawing.Size(60, 60);
            this.btnTinhTrang.Location = new System.Drawing.Point(757, 12);
            this.btnTinhTrang.Name = "btnTinhTrang";
            this.btnTinhTrang.ShadowDecoration.Parent = this.btnTinhTrang;
            this.btnTinhTrang.Size = new System.Drawing.Size(203, 57);
            this.btnTinhTrang.TabIndex = 61;
            this.btnTinhTrang.Text = "Tình Trạng";
            this.btnTinhTrang.Click += new System.EventHandler(this.btnTinhTrang_Click);
            // 
            // cbTinhTrang
            // 
            this.cbTinhTrang.AutoRoundedCorners = true;
            this.cbTinhTrang.BackColor = System.Drawing.Color.Transparent;
            this.cbTinhTrang.BorderRadius = 17;
            this.cbTinhTrang.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbTinhTrang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTinhTrang.FocusedColor = System.Drawing.Color.Empty;
            this.cbTinhTrang.FocusedState.Parent = this.cbTinhTrang;
            this.cbTinhTrang.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbTinhTrang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbTinhTrang.FormattingEnabled = true;
            this.cbTinhTrang.HoverState.Parent = this.cbTinhTrang;
            this.cbTinhTrang.ItemHeight = 30;
            this.cbTinhTrang.Items.AddRange(new object[] {
            "Trống",
            "Còn chỗ",
            "Đầy"});
            this.cbTinhTrang.ItemsAppearance.Parent = this.cbTinhTrang;
            this.cbTinhTrang.Location = new System.Drawing.Point(757, 73);
            this.cbTinhTrang.Name = "cbTinhTrang";
            this.cbTinhTrang.ShadowDecoration.Parent = this.cbTinhTrang;
            this.cbTinhTrang.Size = new System.Drawing.Size(203, 36);
            this.cbTinhTrang.TabIndex = 60;
            // 
            // btnLoaiPhong
            // 
            this.btnLoaiPhong.AutoRoundedCorners = true;
            this.btnLoaiPhong.BorderColor = System.Drawing.Color.Transparent;
            this.btnLoaiPhong.BorderRadius = 27;
            this.btnLoaiPhong.CheckedState.Parent = this.btnLoaiPhong;
            this.btnLoaiPhong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoaiPhong.CustomImages.Parent = this.btnLoaiPhong;
            this.btnLoaiPhong.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btnLoaiPhong.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoaiPhong.ForeColor = System.Drawing.Color.White;
            this.btnLoaiPhong.HoverState.Parent = this.btnLoaiPhong;
            this.btnLoaiPhong.Image = global::Dormitory_Management_2021.Properties.Resources.hotel_bed_500px;
            this.btnLoaiPhong.ImageSize = new System.Drawing.Size(60, 60);
            this.btnLoaiPhong.Location = new System.Drawing.Point(510, 12);
            this.btnLoaiPhong.Name = "btnLoaiPhong";
            this.btnLoaiPhong.ShadowDecoration.Parent = this.btnLoaiPhong;
            this.btnLoaiPhong.Size = new System.Drawing.Size(203, 57);
            this.btnLoaiPhong.TabIndex = 59;
            this.btnLoaiPhong.Text = "Loại Phòng";
            this.btnLoaiPhong.Click += new System.EventHandler(this.btnLoaiPhong_Click);
            // 
            // cbLoaiPhong
            // 
            this.cbLoaiPhong.AutoRoundedCorners = true;
            this.cbLoaiPhong.BackColor = System.Drawing.Color.Transparent;
            this.cbLoaiPhong.BorderRadius = 17;
            this.cbLoaiPhong.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbLoaiPhong.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoaiPhong.FocusedColor = System.Drawing.Color.Empty;
            this.cbLoaiPhong.FocusedState.Parent = this.cbLoaiPhong;
            this.cbLoaiPhong.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbLoaiPhong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbLoaiPhong.FormattingEnabled = true;
            this.cbLoaiPhong.HoverState.Parent = this.cbLoaiPhong;
            this.cbLoaiPhong.ItemHeight = 30;
            this.cbLoaiPhong.Items.AddRange(new object[] {
            "Nam",
            "Nữ"});
            this.cbLoaiPhong.ItemsAppearance.Parent = this.cbLoaiPhong;
            this.cbLoaiPhong.Location = new System.Drawing.Point(510, 73);
            this.cbLoaiPhong.Name = "cbLoaiPhong";
            this.cbLoaiPhong.ShadowDecoration.Parent = this.cbLoaiPhong;
            this.cbLoaiPhong.Size = new System.Drawing.Size(203, 36);
            this.cbLoaiPhong.TabIndex = 58;
            // 
            // btnLayPhongTheoToaNha
            // 
            this.btnLayPhongTheoToaNha.AutoRoundedCorners = true;
            this.btnLayPhongTheoToaNha.BorderColor = System.Drawing.Color.Transparent;
            this.btnLayPhongTheoToaNha.BorderRadius = 27;
            this.btnLayPhongTheoToaNha.CheckedState.Parent = this.btnLayPhongTheoToaNha;
            this.btnLayPhongTheoToaNha.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLayPhongTheoToaNha.CustomImages.Parent = this.btnLayPhongTheoToaNha;
            this.btnLayPhongTheoToaNha.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btnLayPhongTheoToaNha.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLayPhongTheoToaNha.ForeColor = System.Drawing.Color.White;
            this.btnLayPhongTheoToaNha.HoverState.Parent = this.btnLayPhongTheoToaNha;
            this.btnLayPhongTheoToaNha.Image = global::Dormitory_Management_2021.Properties.Resources.building_500px;
            this.btnLayPhongTheoToaNha.ImageSize = new System.Drawing.Size(60, 60);
            this.btnLayPhongTheoToaNha.Location = new System.Drawing.Point(16, 12);
            this.btnLayPhongTheoToaNha.Name = "btnLayPhongTheoToaNha";
            this.btnLayPhongTheoToaNha.ShadowDecoration.Parent = this.btnLayPhongTheoToaNha;
            this.btnLayPhongTheoToaNha.Size = new System.Drawing.Size(203, 57);
            this.btnLayPhongTheoToaNha.TabIndex = 57;
            this.btnLayPhongTheoToaNha.Text = "Toà Nhà";
            this.btnLayPhongTheoToaNha.Click += new System.EventHandler(this.btnLayPhongTheoToaNha_Click);
            // 
            // cbToaNha
            // 
            this.cbToaNha.AutoRoundedCorners = true;
            this.cbToaNha.BackColor = System.Drawing.Color.Transparent;
            this.cbToaNha.BorderRadius = 17;
            this.cbToaNha.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbToaNha.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbToaNha.FocusedColor = System.Drawing.Color.Empty;
            this.cbToaNha.FocusedState.Parent = this.cbToaNha;
            this.cbToaNha.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbToaNha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbToaNha.FormattingEnabled = true;
            this.cbToaNha.HoverState.Parent = this.cbToaNha;
            this.cbToaNha.ItemHeight = 30;
            this.cbToaNha.ItemsAppearance.Parent = this.cbToaNha;
            this.cbToaNha.Location = new System.Drawing.Point(16, 73);
            this.cbToaNha.Name = "cbToaNha";
            this.cbToaNha.ShadowDecoration.Parent = this.cbToaNha;
            this.cbToaNha.Size = new System.Drawing.Size(203, 36);
            this.cbToaNha.TabIndex = 56;
            // 
            // tbTimTenPhong
            // 
            this.tbTimTenPhong.AutoRoundedCorners = true;
            this.tbTimTenPhong.BorderRadius = 17;
            this.tbTimTenPhong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbTimTenPhong.DefaultText = "";
            this.tbTimTenPhong.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.tbTimTenPhong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.tbTimTenPhong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbTimTenPhong.DisabledState.Parent = this.tbTimTenPhong;
            this.tbTimTenPhong.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.tbTimTenPhong.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbTimTenPhong.FocusedState.Parent = this.tbTimTenPhong;
            this.tbTimTenPhong.Font = new System.Drawing.Font("Open Sans", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTimTenPhong.ForeColor = System.Drawing.Color.Black;
            this.tbTimTenPhong.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tbTimTenPhong.HoverState.Parent = this.tbTimTenPhong;
            this.tbTimTenPhong.Location = new System.Drawing.Point(263, 73);
            this.tbTimTenPhong.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.tbTimTenPhong.Name = "tbTimTenPhong";
            this.tbTimTenPhong.PasswordChar = '\0';
            this.tbTimTenPhong.PlaceholderText = "A101";
            this.tbTimTenPhong.SelectedText = "";
            this.tbTimTenPhong.ShadowDecoration.Parent = this.tbTimTenPhong;
            this.tbTimTenPhong.Size = new System.Drawing.Size(203, 36);
            this.tbTimTenPhong.TabIndex = 55;
            this.tbTimTenPhong.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbTimTenPhong.TextChanged += new System.EventHandler(this.btnTimTenPhong_Click);
            // 
            // btnTimTenPhong
            // 
            this.btnTimTenPhong.AutoRoundedCorners = true;
            this.btnTimTenPhong.BorderColor = System.Drawing.Color.Transparent;
            this.btnTimTenPhong.BorderRadius = 27;
            this.btnTimTenPhong.CheckedState.Parent = this.btnTimTenPhong;
            this.btnTimTenPhong.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTimTenPhong.CustomImages.Parent = this.btnTimTenPhong;
            this.btnTimTenPhong.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btnTimTenPhong.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnTimTenPhong.ForeColor = System.Drawing.Color.White;
            this.btnTimTenPhong.HoverState.Parent = this.btnTimTenPhong;
            this.btnTimTenPhong.Image = global::Dormitory_Management_2021.Properties.Resources.hotel_room_key_500px;
            this.btnTimTenPhong.ImageSize = new System.Drawing.Size(40, 40);
            this.btnTimTenPhong.Location = new System.Drawing.Point(263, 12);
            this.btnTimTenPhong.Name = "btnTimTenPhong";
            this.btnTimTenPhong.ShadowDecoration.Parent = this.btnTimTenPhong;
            this.btnTimTenPhong.Size = new System.Drawing.Size(203, 57);
            this.btnTimTenPhong.TabIndex = 54;
            this.btnTimTenPhong.Text = "Tên Phòng";
            this.btnTimTenPhong.Click += new System.EventHandler(this.btnTimTenPhong_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.AutoRoundedCorners = true;
            this.btnRefresh.BorderColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BorderRadius = 47;
            this.btnRefresh.CheckedState.Parent = this.btnRefresh;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.CustomImages.Parent = this.btnRefresh;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.HoverState.Parent = this.btnRefresh;
            this.btnRefresh.Image = global::Dormitory_Management_2021.Properties.Resources.refresh_500px;
            this.btnRefresh.ImageSize = new System.Drawing.Size(60, 60);
            this.btnRefresh.Location = new System.Drawing.Point(1189, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.ShadowDecoration.Parent = this.btnRefresh;
            this.btnRefresh.Size = new System.Drawing.Size(206, 97);
            this.btnRefresh.TabIndex = 62;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // F_Search_Room
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1411, 862);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnTinhTrang);
            this.Controls.Add(this.cbTinhTrang);
            this.Controls.Add(this.btnLoaiPhong);
            this.Controls.Add(this.cbLoaiPhong);
            this.Controls.Add(this.btnLayPhongTheoToaNha);
            this.Controls.Add(this.cbToaNha);
            this.Controls.Add(this.tbTimTenPhong);
            this.Controls.Add(this.btnTimTenPhong);
            this.Controls.Add(this.dgv);
            this.Name = "F_Search_Room";
            this.Text = "F_Search_Room";
            this.Load += new System.EventHandler(this.F_Search_Room_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Bunifu.Framework.UI.BunifuCustomDataGrid dgv;
        private Guna.UI2.WinForms.Guna2Button btnTinhTrang;
        private Guna.UI2.WinForms.Guna2ComboBox cbTinhTrang;
        private Guna.UI2.WinForms.Guna2Button btnLoaiPhong;
        private Guna.UI2.WinForms.Guna2ComboBox cbLoaiPhong;
        private Guna.UI2.WinForms.Guna2Button btnLayPhongTheoToaNha;
        private Guna.UI2.WinForms.Guna2ComboBox cbToaNha;
        private Guna.UI2.WinForms.Guna2TextBox tbTimTenPhong;
        private Guna.UI2.WinForms.Guna2Button btnTimTenPhong;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;
    }
}