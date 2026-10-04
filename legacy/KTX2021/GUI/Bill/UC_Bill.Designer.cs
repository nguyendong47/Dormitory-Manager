
namespace Dormitory_Management_2021.GUI.HoaDon
{
    partial class UCHoaDon
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.txt_Name_Room = new Guna.UI2.WinForms.Guna2TextBox();
            this.cb_Status_Bill = new Guna.UI2.WinForms.Guna2ComboBox();
            this.dtp_Month_Bill = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.btn_Excel = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Status_Bill = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Month_Bill = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Name_Room = new Guna.UI2.WinForms.Guna2Button();
            this.btnRefresh = new Guna.UI2.WinForms.Guna2Button();
            this.btnAdd = new Guna.UI2.WinForms.Guna2Button();
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
            this.dgv.Location = new System.Drawing.Point(13, 119);
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
            this.dgv.Size = new System.Drawing.Size(1379, 650);
            this.dgv.TabIndex = 34;
            this.dgv.DoubleClick += new System.EventHandler(this.dgv_DoubleClick);
            // 
            // txt_Name_Room
            // 
            this.txt_Name_Room.AutoRoundedCorners = true;
            this.txt_Name_Room.BorderRadius = 17;
            this.txt_Name_Room.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Name_Room.DefaultText = "";
            this.txt_Name_Room.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Name_Room.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Name_Room.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Room.DisabledState.Parent = this.txt_Name_Room;
            this.txt_Name_Room.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Room.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Room.FocusedState.Parent = this.txt_Name_Room;
            this.txt_Name_Room.Font = new System.Drawing.Font("Open Sans", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Name_Room.ForeColor = System.Drawing.Color.Black;
            this.txt_Name_Room.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Room.HoverState.Parent = this.txt_Name_Room;
            this.txt_Name_Room.Location = new System.Drawing.Point(13, 74);
            this.txt_Name_Room.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txt_Name_Room.Name = "txt_Name_Room";
            this.txt_Name_Room.PasswordChar = '\0';
            this.txt_Name_Room.PlaceholderText = "A101";
            this.txt_Name_Room.SelectedText = "";
            this.txt_Name_Room.ShadowDecoration.Parent = this.txt_Name_Room;
            this.txt_Name_Room.Size = new System.Drawing.Size(203, 36);
            this.txt_Name_Room.TabIndex = 53;
            this.txt_Name_Room.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_Name_Room.TextChanged += new System.EventHandler(this.btn_Name_Room_Click);
            // 
            // cb_Status_Bill
            // 
            this.cb_Status_Bill.AutoRoundedCorners = true;
            this.cb_Status_Bill.BackColor = System.Drawing.Color.Transparent;
            this.cb_Status_Bill.BorderRadius = 17;
            this.cb_Status_Bill.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_Status_Bill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Status_Bill.FocusedColor = System.Drawing.Color.Empty;
            this.cb_Status_Bill.FocusedState.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cb_Status_Bill.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cb_Status_Bill.FormattingEnabled = true;
            this.cb_Status_Bill.HoverState.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.ItemHeight = 30;
            this.cb_Status_Bill.Items.AddRange(new object[] {
            "Chưa Thanh Toán",
            "Đã Thanh Toán"});
            this.cb_Status_Bill.ItemsAppearance.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.Location = new System.Drawing.Point(526, 74);
            this.cb_Status_Bill.Name = "cb_Status_Bill";
            this.cb_Status_Bill.ShadowDecoration.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.Size = new System.Drawing.Size(203, 36);
            this.cb_Status_Bill.TabIndex = 54;
            // 
            // dtp_Month_Bill
            // 
            this.dtp_Month_Bill.AutoRoundedCorners = true;
            this.dtp_Month_Bill.BorderRadius = 17;
            this.dtp_Month_Bill.CheckedState.Parent = this.dtp_Month_Bill;
            this.dtp_Month_Bill.FillColor = System.Drawing.Color.White;
            this.dtp_Month_Bill.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_Month_Bill.HoverState.Parent = this.dtp_Month_Bill;
            this.dtp_Month_Bill.Location = new System.Drawing.Point(260, 74);
            this.dtp_Month_Bill.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtp_Month_Bill.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtp_Month_Bill.Name = "dtp_Month_Bill";
            this.dtp_Month_Bill.ShadowDecoration.Parent = this.dtp_Month_Bill;
            this.dtp_Month_Bill.Size = new System.Drawing.Size(203, 36);
            this.dtp_Month_Bill.TabIndex = 56;
            this.dtp_Month_Bill.Value = new System.DateTime(2021, 10, 16, 20, 34, 4, 437);
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this.dgv;
            // 
            // btn_Excel
            // 
            this.btn_Excel.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Excel.BorderRadius = 20;
            this.btn_Excel.CheckedState.Parent = this.btn_Excel;
            this.btn_Excel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Excel.CustomImages.Parent = this.btn_Excel;
            this.btn_Excel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(172)))), ((int)(((byte)(132)))));
            this.btn_Excel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Excel.ForeColor = System.Drawing.Color.White;
            this.btn_Excel.HoverState.Parent = this.btn_Excel;
            this.btn_Excel.Image = global::Dormitory_Management_2021.Properties.Resources.microsoft_excel_500px;
            this.btn_Excel.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Excel.Location = new System.Drawing.Point(1186, 780);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.ShadowDecoration.Parent = this.btn_Excel;
            this.btn_Excel.Size = new System.Drawing.Size(206, 74);
            this.btn_Excel.TabIndex = 57;
            this.btn_Excel.Text = "Excel";
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // btn_Status_Bill
            // 
            this.btn_Status_Bill.AutoRoundedCorners = true;
            this.btn_Status_Bill.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Status_Bill.BorderRadius = 27;
            this.btn_Status_Bill.CheckedState.Parent = this.btn_Status_Bill;
            this.btn_Status_Bill.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Status_Bill.CustomImages.Parent = this.btn_Status_Bill;
            this.btn_Status_Bill.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btn_Status_Bill.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Status_Bill.ForeColor = System.Drawing.Color.White;
            this.btn_Status_Bill.HoverState.Parent = this.btn_Status_Bill;
            this.btn_Status_Bill.Image = global::Dormitory_Management_2021.Properties.Resources.hotel_bed_500px;
            this.btn_Status_Bill.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Status_Bill.Location = new System.Drawing.Point(526, 13);
            this.btn_Status_Bill.Name = "btn_Status_Bill";
            this.btn_Status_Bill.ShadowDecoration.Parent = this.btn_Status_Bill;
            this.btn_Status_Bill.Size = new System.Drawing.Size(203, 57);
            this.btn_Status_Bill.TabIndex = 55;
            this.btn_Status_Bill.Text = "Trạng Thái";
            this.btn_Status_Bill.Click += new System.EventHandler(this.btn_Status_Bill_Click);
            // 
            // btn_Month_Bill
            // 
            this.btn_Month_Bill.AutoRoundedCorners = true;
            this.btn_Month_Bill.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Month_Bill.BorderRadius = 27;
            this.btn_Month_Bill.CheckedState.Parent = this.btn_Month_Bill;
            this.btn_Month_Bill.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Month_Bill.CustomImages.Parent = this.btn_Month_Bill;
            this.btn_Month_Bill.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btn_Month_Bill.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Month_Bill.ForeColor = System.Drawing.Color.White;
            this.btn_Month_Bill.HoverState.Parent = this.btn_Month_Bill;
            this.btn_Month_Bill.Image = global::Dormitory_Management_2021.Properties.Resources.hotel_bed_500px;
            this.btn_Month_Bill.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Month_Bill.Location = new System.Drawing.Point(260, 13);
            this.btn_Month_Bill.Name = "btn_Month_Bill";
            this.btn_Month_Bill.ShadowDecoration.Parent = this.btn_Month_Bill;
            this.btn_Month_Bill.Size = new System.Drawing.Size(203, 57);
            this.btn_Month_Bill.TabIndex = 55;
            this.btn_Month_Bill.Text = "Tháng";
            this.btn_Month_Bill.Click += new System.EventHandler(this.btn_Month_Bill_Click);
            // 
            // btn_Name_Room
            // 
            this.btn_Name_Room.AutoRoundedCorners = true;
            this.btn_Name_Room.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Name_Room.BorderRadius = 27;
            this.btn_Name_Room.CheckedState.Parent = this.btn_Name_Room;
            this.btn_Name_Room.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Name_Room.CustomImages.Parent = this.btn_Name_Room;
            this.btn_Name_Room.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btn_Name_Room.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btn_Name_Room.ForeColor = System.Drawing.Color.White;
            this.btn_Name_Room.HoverState.Parent = this.btn_Name_Room;
            this.btn_Name_Room.Image = global::Dormitory_Management_2021.Properties.Resources.hotel_room_key_500px;
            this.btn_Name_Room.ImageSize = new System.Drawing.Size(40, 40);
            this.btn_Name_Room.Location = new System.Drawing.Point(13, 13);
            this.btn_Name_Room.Name = "btn_Name_Room";
            this.btn_Name_Room.ShadowDecoration.Parent = this.btn_Name_Room;
            this.btn_Name_Room.Size = new System.Drawing.Size(203, 57);
            this.btn_Name_Room.TabIndex = 52;
            this.btn_Name_Room.Text = "Tên Phòng";
            this.btn_Name_Room.Click += new System.EventHandler(this.btn_Name_Room_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BorderColor = System.Drawing.Color.Transparent;
            this.btnRefresh.BorderRadius = 20;
            this.btnRefresh.CheckedState.Parent = this.btnRefresh;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.CustomImages.Parent = this.btnRefresh;
            this.btnRefresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.HoverState.Parent = this.btnRefresh;
            this.btnRefresh.Image = global::Dormitory_Management_2021.Properties.Resources.refresh_500px;
            this.btnRefresh.ImageSize = new System.Drawing.Size(60, 60);
            this.btnRefresh.Location = new System.Drawing.Point(974, 13);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.ShadowDecoration.Parent = this.btnRefresh;
            this.btnRefresh.Size = new System.Drawing.Size(206, 97);
            this.btnRefresh.TabIndex = 46;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btn_Refresh_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.BorderColor = System.Drawing.Color.Transparent;
            this.btnAdd.BorderRadius = 20;
            this.btnAdd.CheckedState.Parent = this.btnAdd;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.CustomImages.Parent = this.btnAdd;
            this.btnAdd.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.HoverState.Parent = this.btnAdd;
            this.btnAdd.Image = global::Dormitory_Management_2021.Properties.Resources.add_500px;
            this.btnAdd.ImageSize = new System.Drawing.Size(60, 60);
            this.btnAdd.Location = new System.Drawing.Point(1186, 13);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.ShadowDecoration.Parent = this.btnAdd;
            this.btnAdd.Size = new System.Drawing.Size(206, 97);
            this.btnAdd.TabIndex = 45;
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // UCHoaDon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.btn_Excel);
            this.Controls.Add(this.dtp_Month_Bill);
            this.Controls.Add(this.btn_Status_Bill);
            this.Controls.Add(this.cb_Status_Bill);
            this.Controls.Add(this.btn_Month_Bill);
            this.Controls.Add(this.txt_Name_Room);
            this.Controls.Add(this.btn_Name_Room);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgv);
            this.Name = "UCHoaDon";
            this.Size = new System.Drawing.Size(1405, 864);
            this.Load += new System.EventHandler(this.UCHoaDon_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Bunifu.Framework.UI.BunifuCustomDataGrid dgv;
        private Guna.UI2.WinForms.Guna2Button btnRefresh;
        private Guna.UI2.WinForms.Guna2Button btnAdd;
        private Guna.UI2.WinForms.Guna2Button btn_Month_Bill;
        private Guna.UI2.WinForms.Guna2TextBox txt_Name_Room;
        private Guna.UI2.WinForms.Guna2Button btn_Name_Room;
        private Guna.UI2.WinForms.Guna2ComboBox cb_Status_Bill;
        private Guna.UI2.WinForms.Guna2Button btn_Status_Bill;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtp_Month_Bill;
        private Guna.UI2.WinForms.Guna2Button btn_Excel;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
    }
}
