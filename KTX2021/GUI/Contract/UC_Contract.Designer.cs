
namespace Dormitory_Management_2021.GUI.HopDong
{
    partial class UCHopDong
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.btn_Refresh = new Guna.UI2.WinForms.Guna2Button();
            this.btnAdd = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Name_Student_Contract = new Guna.UI2.WinForms.Guna2Button();
            this.txt_Name_Room = new Guna.UI2.WinForms.Guna2TextBox();
            this.btn_Name_Room = new Guna.UI2.WinForms.Guna2Button();
            this.txt_Name_Student_Contract = new Guna.UI2.WinForms.Guna2TextBox();
            this.btn_Excel = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeColumns = false;
            this.dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle21.BackColor = System.Drawing.Color.White;
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle21;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgv.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle22.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(209)))), ((int)(((byte)(161)))));
            dataGridViewCellStyle22.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle22.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle22.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(209)))), ((int)(((byte)(161)))));
            dataGridViewCellStyle22.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle22.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle22;
            this.dgv.ColumnHeadersHeight = 60;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle23.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle23;
            this.dgv.DoubleBuffered = true;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.HeaderBgColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(209)))), ((int)(((byte)(161)))));
            this.dgv.HeaderForeColor = System.Drawing.Color.White;
            this.dgv.Location = new System.Drawing.Point(13, 119);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle24.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle24;
            this.dgv.RowHeadersVisible = false;
            dataGridViewCellStyle25.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle25.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle25.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.Color.Black;
            this.dgv.RowsDefaultCellStyle = dataGridViewCellStyle25;
            this.dgv.RowTemplate.Height = 50;
            this.dgv.RowTemplate.ReadOnly = true;
            this.dgv.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(1379, 650);
            this.dgv.TabIndex = 42;
            this.dgv.DoubleClick += new System.EventHandler(this.dgv_DoubleClick);
            // 
            // btn_Refresh
            // 
            this.btn_Refresh.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Refresh.BorderRadius = 20;
            this.btn_Refresh.CheckedState.Parent = this.btn_Refresh;
            this.btn_Refresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Refresh.CustomImages.Parent = this.btn_Refresh;
            this.btn_Refresh.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.btn_Refresh.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Refresh.ForeColor = System.Drawing.Color.White;
            this.btn_Refresh.HoverState.Parent = this.btn_Refresh;
            this.btn_Refresh.Image = global::Dormitory_Management_2021.Properties.Resources.refresh_500px;
            this.btn_Refresh.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Refresh.Location = new System.Drawing.Point(974, 13);
            this.btn_Refresh.Name = "btn_Refresh";
            this.btn_Refresh.ShadowDecoration.Parent = this.btn_Refresh;
            this.btn_Refresh.Size = new System.Drawing.Size(206, 90);
            this.btn_Refresh.TabIndex = 48;
            this.btn_Refresh.Text = "Làm mới";
            this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
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
            this.btnAdd.Size = new System.Drawing.Size(206, 90);
            this.btnAdd.TabIndex = 47;
            this.btnAdd.Text = "Thêm mới";
            this.btnAdd.Click += new System.EventHandler(this.btnThem_Click);
            // 
            // btn_Name_Student_Contract
            // 
            this.btn_Name_Student_Contract.AutoRoundedCorners = true;
            this.btn_Name_Student_Contract.BorderColor = System.Drawing.Color.Transparent;
            this.btn_Name_Student_Contract.BorderRadius = 27;
            this.btn_Name_Student_Contract.CheckedState.Parent = this.btn_Name_Student_Contract;
            this.btn_Name_Student_Contract.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Name_Student_Contract.CustomImages.Parent = this.btn_Name_Student_Contract;
            this.btn_Name_Student_Contract.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(188)))), ((int)(((byte)(156)))));
            this.btn_Name_Student_Contract.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Name_Student_Contract.ForeColor = System.Drawing.Color.White;
            this.btn_Name_Student_Contract.HoverState.Parent = this.btn_Name_Student_Contract;
            this.btn_Name_Student_Contract.Image = global::Dormitory_Management_2021.Properties.Resources.name_tag_500px;
            this.btn_Name_Student_Contract.ImageSize = new System.Drawing.Size(60, 60);
            this.btn_Name_Student_Contract.Location = new System.Drawing.Point(13, 13);
            this.btn_Name_Student_Contract.Name = "btn_Name_Student_Contract";
            this.btn_Name_Student_Contract.ShadowDecoration.Parent = this.btn_Name_Student_Contract;
            this.btn_Name_Student_Contract.Size = new System.Drawing.Size(203, 57);
            this.btn_Name_Student_Contract.TabIndex = 53;
            this.btn_Name_Student_Contract.Text = "Tên";
            this.btn_Name_Student_Contract.Click += new System.EventHandler(this.btn_Name_Student_Contract_Click);
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
            this.txt_Name_Room.Location = new System.Drawing.Point(260, 74);
            this.txt_Name_Room.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txt_Name_Room.Name = "txt_Name_Room";
            this.txt_Name_Room.PasswordChar = '\0';
            this.txt_Name_Room.PlaceholderText = "A101";
            this.txt_Name_Room.SelectedText = "";
            this.txt_Name_Room.ShadowDecoration.Parent = this.txt_Name_Room;
            this.txt_Name_Room.Size = new System.Drawing.Size(203, 36);
            this.txt_Name_Room.TabIndex = 51;
            this.txt_Name_Room.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_Name_Room.TextChanged += new System.EventHandler(this.btn_Name_Room_Click);
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
            this.btn_Name_Room.Location = new System.Drawing.Point(260, 13);
            this.btn_Name_Room.Name = "btn_Name_Room";
            this.btn_Name_Room.ShadowDecoration.Parent = this.btn_Name_Room;
            this.btn_Name_Room.Size = new System.Drawing.Size(203, 57);
            this.btn_Name_Room.TabIndex = 50;
            this.btn_Name_Room.Text = "Tên Phòng";
            this.btn_Name_Room.Click += new System.EventHandler(this.btn_Name_Room_Click);
            // 
            // txt_Name_Student_Contract
            // 
            this.txt_Name_Student_Contract.AutoRoundedCorners = true;
            this.txt_Name_Student_Contract.BorderRadius = 17;
            this.txt_Name_Student_Contract.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Name_Student_Contract.DefaultText = "";
            this.txt_Name_Student_Contract.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Name_Student_Contract.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Name_Student_Contract.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Student_Contract.DisabledState.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Student_Contract.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Student_Contract.FocusedState.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.Font = new System.Drawing.Font("Open Sans", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Name_Student_Contract.ForeColor = System.Drawing.Color.Black;
            this.txt_Name_Student_Contract.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Student_Contract.HoverState.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.Location = new System.Drawing.Point(13, 74);
            this.txt_Name_Student_Contract.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txt_Name_Student_Contract.Name = "txt_Name_Student_Contract";
            this.txt_Name_Student_Contract.PasswordChar = '\0';
            this.txt_Name_Student_Contract.PlaceholderText = "dong";
            this.txt_Name_Student_Contract.SelectedText = "";
            this.txt_Name_Student_Contract.ShadowDecoration.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.Size = new System.Drawing.Size(203, 36);
            this.txt_Name_Student_Contract.TabIndex = 54;
            this.txt_Name_Student_Contract.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_Name_Student_Contract.TextChanged += new System.EventHandler(this.btn_Name_Student_Contract_Click);
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
            this.btn_Excel.Location = new System.Drawing.Point(1186, 779);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.ShadowDecoration.Parent = this.btn_Excel;
            this.btn_Excel.Size = new System.Drawing.Size(206, 74);
            this.btn_Excel.TabIndex = 58;
            this.btn_Excel.Text = "Excel";
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this.dgv;
            // 
            // UCHopDong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Controls.Add(this.btn_Excel);
            this.Controls.Add(this.txt_Name_Student_Contract);
            this.Controls.Add(this.btn_Name_Student_Contract);
            this.Controls.Add(this.txt_Name_Room);
            this.Controls.Add(this.btn_Name_Room);
            this.Controls.Add(this.btn_Refresh);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dgv);
            this.Name = "UCHopDong";
            this.Size = new System.Drawing.Size(1405, 864);
            this.Load += new System.EventHandler(this.UC_HopDong_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Bunifu.Framework.UI.BunifuCustomDataGrid dgv;
        private Guna.UI2.WinForms.Guna2Button btn_Refresh;
        private Guna.UI2.WinForms.Guna2Button btnAdd;
        private Guna.UI2.WinForms.Guna2Button btn_Name_Student_Contract;
        private Guna.UI2.WinForms.Guna2TextBox txt_Name_Room;
        private Guna.UI2.WinForms.Guna2Button btn_Name_Room;
        private Guna.UI2.WinForms.Guna2TextBox txt_Name_Student_Contract;
        private Guna.UI2.WinForms.Guna2Button btn_Excel;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
    }
}
