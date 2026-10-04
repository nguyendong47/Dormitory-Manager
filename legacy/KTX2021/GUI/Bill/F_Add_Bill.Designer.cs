
namespace Dormitory_Management_2021.GUI.Bill
{
    partial class F_Add_Bill
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
            this.cb_Status_Bill = new Guna.UI2.WinForms.Guna2ComboBox();
            this.dtp_Month_Bill = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.btn_Cancel = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Create = new Guna.UI2.WinForms.Guna2Button();
            this.txt_Total_Money_Bill = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Number_Electricty_Bill = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Number_Water_Bill = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Status_Bill = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Month_Bill = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Name_Room = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // cb_Status_Bill
            // 
            this.cb_Status_Bill.AutoRoundedCorners = true;
            this.cb_Status_Bill.BackColor = System.Drawing.Color.White;
            this.cb_Status_Bill.BorderColor = System.Drawing.Color.White;
            this.cb_Status_Bill.BorderRadius = 17;
            this.cb_Status_Bill.BorderThickness = 0;
            this.cb_Status_Bill.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cb_Status_Bill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Status_Bill.DropDownWidth = 236;
            this.cb_Status_Bill.FocusedColor = System.Drawing.Color.Empty;
            this.cb_Status_Bill.FocusedState.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_Status_Bill.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cb_Status_Bill.FormattingEnabled = true;
            this.cb_Status_Bill.HoverState.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.ItemHeight = 30;
            this.cb_Status_Bill.Items.AddRange(new object[] {
            "Chưa Thanh Toán",
            "Đã Thanh Toán"});
            this.cb_Status_Bill.ItemsAppearance.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.Location = new System.Drawing.Point(159, 322);
            this.cb_Status_Bill.Name = "cb_Status_Bill";
            this.cb_Status_Bill.ShadowDecoration.Parent = this.cb_Status_Bill;
            this.cb_Status_Bill.Size = new System.Drawing.Size(225, 36);
            this.cb_Status_Bill.TabIndex = 243;
            // 
            // dtp_Month_Bill
            // 
            this.dtp_Month_Bill.CheckedState.Parent = this.dtp_Month_Bill;
            this.dtp_Month_Bill.FillColor = System.Drawing.Color.White;
            this.dtp_Month_Bill.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_Month_Bill.HoverState.Parent = this.dtp_Month_Bill;
            this.dtp_Month_Bill.Location = new System.Drawing.Point(159, 240);
            this.dtp_Month_Bill.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtp_Month_Bill.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtp_Month_Bill.Name = "dtp_Month_Bill";
            this.dtp_Month_Bill.ShadowDecoration.Parent = this.dtp_Month_Bill;
            this.dtp_Month_Bill.Size = new System.Drawing.Size(225, 58);
            this.dtp_Month_Bill.TabIndex = 242;
            this.dtp_Month_Bill.Value = new System.DateTime(2021, 10, 16, 10, 29, 52, 905);
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
            this.btn_Cancel.Location = new System.Drawing.Point(135, 518);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.ShadowDecoration.Parent = this.btn_Cancel;
            this.btn_Cancel.Size = new System.Drawing.Size(230, 60);
            this.btn_Cancel.TabIndex = 246;
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
            this.btn_Create.Location = new System.Drawing.Point(135, 452);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.ShadowDecoration.Parent = this.btn_Create;
            this.btn_Create.Size = new System.Drawing.Size(230, 60);
            this.btn_Create.TabIndex = 245;
            this.btn_Create.Text = "Thêm mới";
            this.btn_Create.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // txt_Total_Money_Bill
            // 
            this.txt_Total_Money_Bill.AutoRoundedCorners = true;
            this.txt_Total_Money_Bill.BorderRadius = 29;
            this.txt_Total_Money_Bill.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Total_Money_Bill.DefaultText = "";
            this.txt_Total_Money_Bill.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Total_Money_Bill.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Total_Money_Bill.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Total_Money_Bill.DisabledState.Parent = this.txt_Total_Money_Bill;
            this.txt_Total_Money_Bill.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Total_Money_Bill.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Total_Money_Bill.FocusedState.Parent = this.txt_Total_Money_Bill;
            this.txt_Total_Money_Bill.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Total_Money_Bill.ForeColor = System.Drawing.Color.Black;
            this.txt_Total_Money_Bill.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Total_Money_Bill.HoverState.Parent = this.txt_Total_Money_Bill;
            this.txt_Total_Money_Bill.IconLeft = global::Dormitory_Management_2021.Properties.Resources.money_500px;
            this.txt_Total_Money_Bill.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Total_Money_Bill.Location = new System.Drawing.Point(91, 383);
            this.txt_Total_Money_Bill.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Total_Money_Bill.Name = "txt_Total_Money_Bill";
            this.txt_Total_Money_Bill.PasswordChar = '\0';
            this.txt_Total_Money_Bill.PlaceholderText = "Tổng tiền";
            this.txt_Total_Money_Bill.SelectedText = "";
            this.txt_Total_Money_Bill.ShadowDecoration.Parent = this.txt_Total_Money_Bill;
            this.txt_Total_Money_Bill.Size = new System.Drawing.Size(320, 60);
            this.txt_Total_Money_Bill.TabIndex = 244;
            // 
            // txt_Number_Electricty_Bill
            // 
            this.txt_Number_Electricty_Bill.AutoRoundedCorners = true;
            this.txt_Number_Electricty_Bill.BorderRadius = 29;
            this.txt_Number_Electricty_Bill.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Number_Electricty_Bill.DefaultText = "";
            this.txt_Number_Electricty_Bill.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Number_Electricty_Bill.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Number_Electricty_Bill.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Number_Electricty_Bill.DisabledState.Parent = this.txt_Number_Electricty_Bill;
            this.txt_Number_Electricty_Bill.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Number_Electricty_Bill.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Number_Electricty_Bill.FocusedState.Parent = this.txt_Number_Electricty_Bill;
            this.txt_Number_Electricty_Bill.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Number_Electricty_Bill.ForeColor = System.Drawing.Color.Black;
            this.txt_Number_Electricty_Bill.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Number_Electricty_Bill.HoverState.Parent = this.txt_Number_Electricty_Bill;
            this.txt_Number_Electricty_Bill.IconLeft = global::Dormitory_Management_2021.Properties.Resources.light_on_500px;
            this.txt_Number_Electricty_Bill.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Number_Electricty_Bill.Location = new System.Drawing.Point(91, 167);
            this.txt_Number_Electricty_Bill.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Number_Electricty_Bill.Name = "txt_Number_Electricty_Bill";
            this.txt_Number_Electricty_Bill.PasswordChar = '\0';
            this.txt_Number_Electricty_Bill.PlaceholderText = "Số điện";
            this.txt_Number_Electricty_Bill.SelectedText = "";
            this.txt_Number_Electricty_Bill.ShadowDecoration.Parent = this.txt_Number_Electricty_Bill;
            this.txt_Number_Electricty_Bill.Size = new System.Drawing.Size(320, 60);
            this.txt_Number_Electricty_Bill.TabIndex = 244;
            // 
            // txt_Number_Water_Bill
            // 
            this.txt_Number_Water_Bill.AutoRoundedCorners = true;
            this.txt_Number_Water_Bill.BorderRadius = 29;
            this.txt_Number_Water_Bill.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Number_Water_Bill.DefaultText = "";
            this.txt_Number_Water_Bill.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Number_Water_Bill.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Number_Water_Bill.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Number_Water_Bill.DisabledState.Parent = this.txt_Number_Water_Bill;
            this.txt_Number_Water_Bill.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Number_Water_Bill.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Number_Water_Bill.FocusedState.Parent = this.txt_Number_Water_Bill;
            this.txt_Number_Water_Bill.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Number_Water_Bill.ForeColor = System.Drawing.Color.Black;
            this.txt_Number_Water_Bill.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Number_Water_Bill.HoverState.Parent = this.txt_Number_Water_Bill;
            this.txt_Number_Water_Bill.IconLeft = global::Dormitory_Management_2021.Properties.Resources.water_500px;
            this.txt_Number_Water_Bill.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Number_Water_Bill.Location = new System.Drawing.Point(89, 95);
            this.txt_Number_Water_Bill.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Number_Water_Bill.Name = "txt_Number_Water_Bill";
            this.txt_Number_Water_Bill.PasswordChar = '\0';
            this.txt_Number_Water_Bill.PlaceholderText = "Số nước";
            this.txt_Number_Water_Bill.SelectedText = "";
            this.txt_Number_Water_Bill.ShadowDecoration.Parent = this.txt_Number_Water_Bill;
            this.txt_Number_Water_Bill.Size = new System.Drawing.Size(320, 60);
            this.txt_Number_Water_Bill.TabIndex = 244;
            // 
            // txt_Status_Bill
            // 
            this.txt_Status_Bill.AutoRoundedCorners = true;
            this.txt_Status_Bill.BorderRadius = 29;
            this.txt_Status_Bill.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Status_Bill.DefaultText = "";
            this.txt_Status_Bill.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Status_Bill.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Status_Bill.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Status_Bill.DisabledState.Parent = this.txt_Status_Bill;
            this.txt_Status_Bill.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Status_Bill.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Status_Bill.FocusedState.Parent = this.txt_Status_Bill;
            this.txt_Status_Bill.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Status_Bill.ForeColor = System.Drawing.Color.Black;
            this.txt_Status_Bill.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Status_Bill.HoverState.Parent = this.txt_Status_Bill;
            this.txt_Status_Bill.IconLeft = global::Dormitory_Management_2021.Properties.Resources.cash_in_hand_500px;
            this.txt_Status_Bill.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Status_Bill.Location = new System.Drawing.Point(91, 311);
            this.txt_Status_Bill.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Status_Bill.Name = "txt_Status_Bill";
            this.txt_Status_Bill.PasswordChar = '\0';
            this.txt_Status_Bill.PlaceholderText = "Họ tên";
            this.txt_Status_Bill.SelectedText = "";
            this.txt_Status_Bill.ShadowDecoration.Parent = this.txt_Status_Bill;
            this.txt_Status_Bill.Size = new System.Drawing.Size(320, 60);
            this.txt_Status_Bill.TabIndex = 241;
            // 
            // txt_Month_Bill
            // 
            this.txt_Month_Bill.AutoRoundedCorners = true;
            this.txt_Month_Bill.BorderRadius = 29;
            this.txt_Month_Bill.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Month_Bill.DefaultText = "";
            this.txt_Month_Bill.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Month_Bill.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Month_Bill.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Month_Bill.DisabledState.Parent = this.txt_Month_Bill;
            this.txt_Month_Bill.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Month_Bill.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Month_Bill.FocusedState.Parent = this.txt_Month_Bill;
            this.txt_Month_Bill.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Month_Bill.ForeColor = System.Drawing.Color.Black;
            this.txt_Month_Bill.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Month_Bill.HoverState.Parent = this.txt_Month_Bill;
            this.txt_Month_Bill.IconLeft = global::Dormitory_Management_2021.Properties.Resources.planner_500px;
            this.txt_Month_Bill.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Month_Bill.Location = new System.Drawing.Point(91, 239);
            this.txt_Month_Bill.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Month_Bill.Name = "txt_Month_Bill";
            this.txt_Month_Bill.PasswordChar = '\0';
            this.txt_Month_Bill.PlaceholderText = "Họ tên";
            this.txt_Month_Bill.SelectedText = "";
            this.txt_Month_Bill.ShadowDecoration.Parent = this.txt_Month_Bill;
            this.txt_Month_Bill.Size = new System.Drawing.Size(320, 60);
            this.txt_Month_Bill.TabIndex = 240;
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
            this.txt_Name_Room.Location = new System.Drawing.Point(89, 23);
            this.txt_Name_Room.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Name_Room.Name = "txt_Name_Room";
            this.txt_Name_Room.PasswordChar = '\0';
            this.txt_Name_Room.PlaceholderText = "Tên phòng";
            this.txt_Name_Room.SelectedText = "";
            this.txt_Name_Room.ShadowDecoration.Parent = this.txt_Name_Room;
            this.txt_Name_Room.Size = new System.Drawing.Size(320, 60);
            this.txt_Name_Room.TabIndex = 239;
            // 
            // F_Add_Bill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Create);
            this.Controls.Add(this.txt_Total_Money_Bill);
            this.Controls.Add(this.txt_Number_Electricty_Bill);
            this.Controls.Add(this.txt_Number_Water_Bill);
            this.Controls.Add(this.cb_Status_Bill);
            this.Controls.Add(this.dtp_Month_Bill);
            this.Controls.Add(this.txt_Status_Bill);
            this.Controls.Add(this.txt_Month_Bill);
            this.Controls.Add(this.txt_Name_Room);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_Add_Bill";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddBill";
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2ComboBox cb_Status_Bill;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtp_Month_Bill;
        private Guna.UI2.WinForms.Guna2TextBox txt_Status_Bill;
        private Guna.UI2.WinForms.Guna2TextBox txt_Month_Bill;
        private Guna.UI2.WinForms.Guna2TextBox txt_Name_Room;
        private Guna.UI2.WinForms.Guna2TextBox txt_Number_Water_Bill;
        private Guna.UI2.WinForms.Guna2TextBox txt_Number_Electricty_Bill;
        private Guna.UI2.WinForms.Guna2TextBox txt_Total_Money_Bill;
        private Guna.UI2.WinForms.Guna2Button btn_Cancel;
        private Guna.UI2.WinForms.Guna2Button btn_Create;
    }
}