
namespace Dormitory_Management_2021.GUI.HopDong
{
    partial class SuaHopDong
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
            this.dtp_Expiration_Date_Contract = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.dtp_Start_Day_Contract = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.btn_Cancel = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Delete = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Update = new Guna.UI2.WinForms.Guna2Button();
            this.txt_ID_Contract = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Total_Money_Contract = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Expiration_Date_Contract = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Start_Day_Contract = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Name_Room = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Name_Student_Contract = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // dtp_Expiration_Date_Contract
            // 
            this.dtp_Expiration_Date_Contract.CheckedState.Parent = this.dtp_Expiration_Date_Contract;
            this.dtp_Expiration_Date_Contract.FillColor = System.Drawing.Color.White;
            this.dtp_Expiration_Date_Contract.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_Expiration_Date_Contract.HoverState.Parent = this.dtp_Expiration_Date_Contract;
            this.dtp_Expiration_Date_Contract.Location = new System.Drawing.Point(159, 330);
            this.dtp_Expiration_Date_Contract.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtp_Expiration_Date_Contract.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtp_Expiration_Date_Contract.Name = "dtp_Expiration_Date_Contract";
            this.dtp_Expiration_Date_Contract.ShadowDecoration.Parent = this.dtp_Expiration_Date_Contract;
            this.dtp_Expiration_Date_Contract.Size = new System.Drawing.Size(225, 58);
            this.dtp_Expiration_Date_Contract.TabIndex = 245;
            this.dtp_Expiration_Date_Contract.Value = new System.DateTime(2021, 10, 16, 10, 29, 52, 905);
            // 
            // dtp_Start_Day_Contract
            // 
            this.dtp_Start_Day_Contract.CheckedState.Parent = this.dtp_Start_Day_Contract;
            this.dtp_Start_Day_Contract.FillColor = System.Drawing.Color.White;
            this.dtp_Start_Day_Contract.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_Start_Day_Contract.HoverState.Parent = this.dtp_Start_Day_Contract;
            this.dtp_Start_Day_Contract.Location = new System.Drawing.Point(159, 257);
            this.dtp_Start_Day_Contract.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtp_Start_Day_Contract.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtp_Start_Day_Contract.Name = "dtp_Start_Day_Contract";
            this.dtp_Start_Day_Contract.ShadowDecoration.Parent = this.dtp_Start_Day_Contract;
            this.dtp_Start_Day_Contract.Size = new System.Drawing.Size(225, 58);
            this.dtp_Start_Day_Contract.TabIndex = 244;
            this.dtp_Start_Day_Contract.Value = new System.DateTime(2021, 10, 16, 10, 29, 52, 905);
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
            this.btn_Cancel.Location = new System.Drawing.Point(133, 616);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.ShadowDecoration.Parent = this.btn_Cancel;
            this.btn_Cancel.Size = new System.Drawing.Size(230, 60);
            this.btn_Cancel.TabIndex = 249;
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
            this.btn_Delete.Location = new System.Drawing.Point(133, 550);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.ShadowDecoration.Parent = this.btn_Delete;
            this.btn_Delete.Size = new System.Drawing.Size(230, 60);
            this.btn_Delete.TabIndex = 247;
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
            this.btn_Update.Location = new System.Drawing.Point(133, 484);
            this.btn_Update.Name = "btn_Update";
            this.btn_Update.ShadowDecoration.Parent = this.btn_Update;
            this.btn_Update.Size = new System.Drawing.Size(230, 60);
            this.btn_Update.TabIndex = 248;
            this.btn_Update.Text = "Cập nhật";
            this.btn_Update.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
            // 
            // txt_ID_Contract
            // 
            this.txt_ID_Contract.AutoRoundedCorners = true;
            this.txt_ID_Contract.BorderRadius = 29;
            this.txt_ID_Contract.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_ID_Contract.DefaultText = "";
            this.txt_ID_Contract.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_ID_Contract.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_ID_Contract.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_ID_Contract.DisabledState.Parent = this.txt_ID_Contract;
            this.txt_ID_Contract.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_ID_Contract.Enabled = false;
            this.txt_ID_Contract.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_ID_Contract.FocusedState.Parent = this.txt_ID_Contract;
            this.txt_ID_Contract.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_ID_Contract.ForeColor = System.Drawing.Color.Black;
            this.txt_ID_Contract.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_ID_Contract.HoverState.Parent = this.txt_ID_Contract;
            this.txt_ID_Contract.IconLeft = global::Dormitory_Management_2021.Properties.Resources.barcode_500px;
            this.txt_ID_Contract.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_ID_Contract.Location = new System.Drawing.Point(90, 37);
            this.txt_ID_Contract.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_ID_Contract.Name = "txt_ID_Contract";
            this.txt_ID_Contract.PasswordChar = '\0';
            this.txt_ID_Contract.PlaceholderText = "Mã hợp đồng";
            this.txt_ID_Contract.SelectedText = "";
            this.txt_ID_Contract.ShadowDecoration.Parent = this.txt_ID_Contract;
            this.txt_ID_Contract.Size = new System.Drawing.Size(320, 60);
            this.txt_ID_Contract.TabIndex = 246;
            // 
            // txt_Total_Money_Contract
            // 
            this.txt_Total_Money_Contract.AutoRoundedCorners = true;
            this.txt_Total_Money_Contract.BorderRadius = 29;
            this.txt_Total_Money_Contract.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Total_Money_Contract.DefaultText = "";
            this.txt_Total_Money_Contract.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Total_Money_Contract.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Total_Money_Contract.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Total_Money_Contract.DisabledState.Parent = this.txt_Total_Money_Contract;
            this.txt_Total_Money_Contract.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Total_Money_Contract.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Total_Money_Contract.FocusedState.Parent = this.txt_Total_Money_Contract;
            this.txt_Total_Money_Contract.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Total_Money_Contract.ForeColor = System.Drawing.Color.Black;
            this.txt_Total_Money_Contract.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Total_Money_Contract.HoverState.Parent = this.txt_Total_Money_Contract;
            this.txt_Total_Money_Contract.IconLeft = global::Dormitory_Management_2021.Properties.Resources.money_500px;
            this.txt_Total_Money_Contract.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Total_Money_Contract.Location = new System.Drawing.Point(90, 402);
            this.txt_Total_Money_Contract.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Total_Money_Contract.Name = "txt_Total_Money_Contract";
            this.txt_Total_Money_Contract.PasswordChar = '\0';
            this.txt_Total_Money_Contract.PlaceholderText = "Tổng tiền";
            this.txt_Total_Money_Contract.SelectedText = "";
            this.txt_Total_Money_Contract.ShadowDecoration.Parent = this.txt_Total_Money_Contract;
            this.txt_Total_Money_Contract.Size = new System.Drawing.Size(320, 60);
            this.txt_Total_Money_Contract.TabIndex = 243;
            // 
            // txt_Expiration_Date_Contract
            // 
            this.txt_Expiration_Date_Contract.AutoRoundedCorners = true;
            this.txt_Expiration_Date_Contract.BorderRadius = 29;
            this.txt_Expiration_Date_Contract.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Expiration_Date_Contract.DefaultText = "";
            this.txt_Expiration_Date_Contract.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Expiration_Date_Contract.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Expiration_Date_Contract.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Expiration_Date_Contract.DisabledState.Parent = this.txt_Expiration_Date_Contract;
            this.txt_Expiration_Date_Contract.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Expiration_Date_Contract.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Expiration_Date_Contract.FocusedState.Parent = this.txt_Expiration_Date_Contract;
            this.txt_Expiration_Date_Contract.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Expiration_Date_Contract.ForeColor = System.Drawing.Color.Black;
            this.txt_Expiration_Date_Contract.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Expiration_Date_Contract.HoverState.Parent = this.txt_Expiration_Date_Contract;
            this.txt_Expiration_Date_Contract.IconLeft = global::Dormitory_Management_2021.Properties.Resources.important_month_500px;
            this.txt_Expiration_Date_Contract.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Expiration_Date_Contract.Location = new System.Drawing.Point(90, 329);
            this.txt_Expiration_Date_Contract.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Expiration_Date_Contract.Name = "txt_Expiration_Date_Contract";
            this.txt_Expiration_Date_Contract.PasswordChar = '\0';
            this.txt_Expiration_Date_Contract.PlaceholderText = "Họ tên";
            this.txt_Expiration_Date_Contract.SelectedText = "";
            this.txt_Expiration_Date_Contract.ShadowDecoration.Parent = this.txt_Expiration_Date_Contract;
            this.txt_Expiration_Date_Contract.Size = new System.Drawing.Size(320, 60);
            this.txt_Expiration_Date_Contract.TabIndex = 242;
            // 
            // txt_Start_Day_Contract
            // 
            this.txt_Start_Day_Contract.AutoRoundedCorners = true;
            this.txt_Start_Day_Contract.BorderRadius = 29;
            this.txt_Start_Day_Contract.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Start_Day_Contract.DefaultText = "";
            this.txt_Start_Day_Contract.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Start_Day_Contract.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Start_Day_Contract.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Start_Day_Contract.DisabledState.Parent = this.txt_Start_Day_Contract;
            this.txt_Start_Day_Contract.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Start_Day_Contract.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Start_Day_Contract.FocusedState.Parent = this.txt_Start_Day_Contract;
            this.txt_Start_Day_Contract.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Start_Day_Contract.ForeColor = System.Drawing.Color.Black;
            this.txt_Start_Day_Contract.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Start_Day_Contract.HoverState.Parent = this.txt_Start_Day_Contract;
            this.txt_Start_Day_Contract.IconLeft = global::Dormitory_Management_2021.Properties.Resources.edit_calendar_500px;
            this.txt_Start_Day_Contract.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Start_Day_Contract.Location = new System.Drawing.Point(90, 256);
            this.txt_Start_Day_Contract.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Start_Day_Contract.Name = "txt_Start_Day_Contract";
            this.txt_Start_Day_Contract.PasswordChar = '\0';
            this.txt_Start_Day_Contract.PlaceholderText = "Họ tên";
            this.txt_Start_Day_Contract.SelectedText = "";
            this.txt_Start_Day_Contract.ShadowDecoration.Parent = this.txt_Start_Day_Contract;
            this.txt_Start_Day_Contract.Size = new System.Drawing.Size(320, 60);
            this.txt_Start_Day_Contract.TabIndex = 241;
            // 
            // txt_Name_Room
            // 
            this.txt_Name_Room.AutoRoundedCorners = true;
            this.txt_Name_Room.BorderRadius = 29;
            this.txt_Name_Room.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Name_Room.DefaultText = "";
            this.txt_Name_Room.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Name_Room.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Name_Room.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Room.DisabledState.Parent = this.txt_Name_Room;
            this.txt_Name_Room.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Room.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Room.FocusedState.Parent = this.txt_Name_Room;
            this.txt_Name_Room.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Name_Room.ForeColor = System.Drawing.Color.Black;
            this.txt_Name_Room.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Room.HoverState.Parent = this.txt_Name_Room;
            this.txt_Name_Room.IconLeft = global::Dormitory_Management_2021.Properties.Resources.hotel_room_key_500px;
            this.txt_Name_Room.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Name_Room.Location = new System.Drawing.Point(90, 110);
            this.txt_Name_Room.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Name_Room.Name = "txt_Name_Room";
            this.txt_Name_Room.PasswordChar = '\0';
            this.txt_Name_Room.PlaceholderText = "Tên phòng";
            this.txt_Name_Room.SelectedText = "";
            this.txt_Name_Room.ShadowDecoration.Parent = this.txt_Name_Room;
            this.txt_Name_Room.Size = new System.Drawing.Size(320, 60);
            this.txt_Name_Room.TabIndex = 231;
            // 
            // txt_Name_Student_Contract
            // 
            this.txt_Name_Student_Contract.AutoRoundedCorners = true;
            this.txt_Name_Student_Contract.BorderRadius = 29;
            this.txt_Name_Student_Contract.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Name_Student_Contract.DefaultText = "";
            this.txt_Name_Student_Contract.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Name_Student_Contract.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Name_Student_Contract.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Student_Contract.DisabledState.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Name_Student_Contract.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Student_Contract.FocusedState.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Name_Student_Contract.ForeColor = System.Drawing.Color.Black;
            this.txt_Name_Student_Contract.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Name_Student_Contract.HoverState.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.IconLeft = global::Dormitory_Management_2021.Properties.Resources.name_tag_500px;
            this.txt_Name_Student_Contract.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Name_Student_Contract.Location = new System.Drawing.Point(90, 183);
            this.txt_Name_Student_Contract.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Name_Student_Contract.Name = "txt_Name_Student_Contract";
            this.txt_Name_Student_Contract.PasswordChar = '\0';
            this.txt_Name_Student_Contract.PlaceholderText = "Họ tên";
            this.txt_Name_Student_Contract.SelectedText = "";
            this.txt_Name_Student_Contract.ShadowDecoration.Parent = this.txt_Name_Student_Contract;
            this.txt_Name_Student_Contract.Size = new System.Drawing.Size(320, 60);
            this.txt_Name_Student_Contract.TabIndex = 222;
            // 
            // SuaHopDong
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.ClientSize = new System.Drawing.Size(500, 700);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Update);
            this.Controls.Add(this.txt_ID_Contract);
            this.Controls.Add(this.dtp_Expiration_Date_Contract);
            this.Controls.Add(this.dtp_Start_Day_Contract);
            this.Controls.Add(this.txt_Total_Money_Contract);
            this.Controls.Add(this.txt_Expiration_Date_Contract);
            this.Controls.Add(this.txt_Start_Day_Contract);
            this.Controls.Add(this.txt_Name_Room);
            this.Controls.Add(this.txt_Name_Student_Contract);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SuaHopDong";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SuaHopDong";
            this.ResumeLayout(false);

        }

        #endregion
        private Guna.UI2.WinForms.Guna2TextBox txt_Name_Student_Contract;
        private Guna.UI2.WinForms.Guna2TextBox txt_Name_Room;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtp_Expiration_Date_Contract;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtp_Start_Day_Contract;
        private Guna.UI2.WinForms.Guna2TextBox txt_Total_Money_Contract;
        private Guna.UI2.WinForms.Guna2TextBox txt_Expiration_Date_Contract;
        private Guna.UI2.WinForms.Guna2TextBox txt_Start_Day_Contract;
        private Guna.UI2.WinForms.Guna2TextBox txt_ID_Contract;
        private Guna.UI2.WinForms.Guna2Button btn_Cancel;
        private Guna.UI2.WinForms.Guna2Button btn_Delete;
        private Guna.UI2.WinForms.Guna2Button btn_Update;
    }
}