
namespace Dormitory_Management_2021
{
    partial class F_Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_Login));
            this.elip_F_Login = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pl_Top = new System.Windows.Forms.Panel();
            this.lb_Title_Login = new System.Windows.Forms.Label();
            this.lb_Title_Desc = new System.Windows.Forms.Label();
            this.lb_Username = new System.Windows.Forms.Label();
            this.lb_Password = new System.Windows.Forms.Label();
            this.lb_Forgot_Password = new System.Windows.Forms.Label();
            this.chk_Save_Account = new Bunifu.Framework.UI.BunifuCheckbox();
            this.lb_Save_Account = new System.Windows.Forms.Label();
            this.pic_Logo = new System.Windows.Forms.PictureBox();
            this.pic_Right = new System.Windows.Forms.PictureBox();
            this.txt_Password = new Guna.UI2.WinForms.Guna2TextBox();
            this.txt_Username = new Guna.UI2.WinForms.Guna2TextBox();
            this.btn_Login = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Right)).BeginInit();
            this.SuspendLayout();
            // 
            // elip_F_Login
            // 
            this.elip_F_Login.ElipseRadius = 60;
            this.elip_F_Login.TargetControl = this;
            // 
            // pl_Top
            // 
            this.pl_Top.BackColor = System.Drawing.Color.Transparent;
            this.pl_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pl_Top.Location = new System.Drawing.Point(0, 0);
            this.pl_Top.Name = "pl_Top";
            this.pl_Top.Size = new System.Drawing.Size(480, 37);
            this.pl_Top.TabIndex = 33;
            this.pl_Top.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pl_Top_MouseDown);
            this.pl_Top.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pl_Top_MouseMove);
            this.pl_Top.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pl_Top_MouseUp);
            // 
            // lb_Title_Login
            // 
            this.lb_Title_Login.AutoSize = true;
            this.lb_Title_Login.Font = new System.Drawing.Font("Open Sans", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Title_Login.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(0)))), ((int)(((byte)(19)))));
            this.lb_Title_Login.Location = new System.Drawing.Point(69, 182);
            this.lb_Title_Login.Name = "lb_Title_Login";
            this.lb_Title_Login.Size = new System.Drawing.Size(148, 33);
            this.lb_Title_Login.TabIndex = 35;
            this.lb_Title_Login.Text = "Đăng Nhập";
            // 
            // lb_Title_Desc
            // 
            this.lb_Title_Desc.AutoSize = true;
            this.lb_Title_Desc.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Title_Desc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(116)))), ((int)(((byte)(142)))));
            this.lb_Title_Desc.Location = new System.Drawing.Point(69, 224);
            this.lb_Title_Desc.Name = "lb_Title_Desc";
            this.lb_Title_Desc.Size = new System.Drawing.Size(336, 22);
            this.lb_Title_Desc.TabIndex = 36;
            this.lb_Title_Desc.Text = "Hệ thống quản lý Ký túc xá Đại Học Hải Phòng!";
            // 
            // lb_Username
            // 
            this.lb_Username.AutoSize = true;
            this.lb_Username.BackColor = System.Drawing.Color.Transparent;
            this.lb_Username.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Username.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(116)))), ((int)(((byte)(142)))));
            this.lb_Username.Location = new System.Drawing.Point(71, 317);
            this.lb_Username.Name = "lb_Username";
            this.lb_Username.Size = new System.Drawing.Size(113, 22);
            this.lb_Username.TabIndex = 43;
            this.lb_Username.Text = "Tên tài khoản*";
            // 
            // lb_Password
            // 
            this.lb_Password.AutoSize = true;
            this.lb_Password.BackColor = System.Drawing.Color.Transparent;
            this.lb_Password.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(116)))), ((int)(((byte)(142)))));
            this.lb_Password.Location = new System.Drawing.Point(71, 408);
            this.lb_Password.Name = "lb_Password";
            this.lb_Password.Size = new System.Drawing.Size(84, 22);
            this.lb_Password.TabIndex = 44;
            this.lb_Password.Text = "Mật khẩu*";
            // 
            // lb_Forgot_Password
            // 
            this.lb_Forgot_Password.AutoSize = true;
            this.lb_Forgot_Password.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Forgot_Password.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.lb_Forgot_Password.Location = new System.Drawing.Point(249, 509);
            this.lb_Forgot_Password.Name = "lb_Forgot_Password";
            this.lb_Forgot_Password.Size = new System.Drawing.Size(129, 22);
            this.lb_Forgot_Password.TabIndex = 49;
            this.lb_Forgot_Password.Text = "Quên mật khẩu ?";
            this.lb_Forgot_Password.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chk_Save_Account
            // 
            this.chk_Save_Account.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.chk_Save_Account.ChechedOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.chk_Save_Account.Checked = true;
            this.chk_Save_Account.CheckedOnColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.chk_Save_Account.ForeColor = System.Drawing.Color.White;
            this.chk_Save_Account.Location = new System.Drawing.Point(74, 512);
            this.chk_Save_Account.Name = "chk_Save_Account";
            this.chk_Save_Account.Size = new System.Drawing.Size(20, 20);
            this.chk_Save_Account.TabIndex = 48;
            // 
            // lb_Save_Account
            // 
            this.lb_Save_Account.AutoSize = true;
            this.lb_Save_Account.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Save_Account.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lb_Save_Account.Location = new System.Drawing.Point(95, 510);
            this.lb_Save_Account.Name = "lb_Save_Account";
            this.lb_Save_Account.Size = new System.Drawing.Size(105, 22);
            this.lb_Save_Account.TabIndex = 47;
            this.lb_Save_Account.Text = "Lưu tài khoản";
            this.lb_Save_Account.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pic_Logo
            // 
            this.pic_Logo.BackgroundImage = global::Dormitory_Management_2021.Properties.Resources.DHHP;
            this.pic_Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Logo.Location = new System.Drawing.Point(74, 67);
            this.pic_Logo.Name = "pic_Logo";
            this.pic_Logo.Size = new System.Drawing.Size(79, 79);
            this.pic_Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_Logo.TabIndex = 34;
            this.pic_Logo.TabStop = false;
            // 
            // pic_Right
            // 
            this.pic_Right.BackgroundImage = global::Dormitory_Management_2021.Properties.Resources.img;
            this.pic_Right.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pic_Right.Dock = System.Windows.Forms.DockStyle.Right;
            this.pic_Right.Location = new System.Drawing.Point(480, 0);
            this.pic_Right.Name = "pic_Right";
            this.pic_Right.Size = new System.Drawing.Size(501, 718);
            this.pic_Right.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pic_Right.TabIndex = 32;
            this.pic_Right.TabStop = false;
            // 
            // txt_Password
            // 
            this.txt_Password.AutoRoundedCorners = true;
            this.txt_Password.BorderRadius = 29;
            this.txt_Password.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Password.DefaultText = "";
            this.txt_Password.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Password.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Password.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Password.DisabledState.Parent = this.txt_Password;
            this.txt_Password.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Password.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Password.FocusedState.Parent = this.txt_Password;
            this.txt_Password.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Password.ForeColor = System.Drawing.Color.Black;
            this.txt_Password.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Password.HoverState.Parent = this.txt_Password;
            this.txt_Password.IconLeft = ((System.Drawing.Image)(resources.GetObject("txt_Password.IconLeft")));
            this.txt_Password.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Password.Location = new System.Drawing.Point(73, 433);
            this.txt_Password.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Password.Name = "txt_Password";
            this.txt_Password.PasswordChar = '*';
            this.txt_Password.PlaceholderText = "12345";
            this.txt_Password.SelectedText = "";
            this.txt_Password.ShadowDecoration.Parent = this.txt_Password;
            this.txt_Password.Size = new System.Drawing.Size(320, 60);
            this.txt_Password.TabIndex = 74;
            // 
            // txt_Username
            // 
            this.txt_Username.AutoRoundedCorners = true;
            this.txt_Username.BorderRadius = 29;
            this.txt_Username.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txt_Username.DefaultText = "";
            this.txt_Username.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txt_Username.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txt_Username.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Username.DisabledState.Parent = this.txt_Username;
            this.txt_Username.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txt_Username.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Username.FocusedState.Parent = this.txt_Username;
            this.txt_Username.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Username.ForeColor = System.Drawing.Color.Black;
            this.txt_Username.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txt_Username.HoverState.Parent = this.txt_Username;
            this.txt_Username.IconLeft = global::Dormitory_Management_2021.Properties.Resources.name_tag_500px;
            this.txt_Username.IconLeftSize = new System.Drawing.Size(60, 60);
            this.txt_Username.Location = new System.Drawing.Point(75, 342);
            this.txt_Username.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.txt_Username.Name = "txt_Username";
            this.txt_Username.PasswordChar = '\0';
            this.txt_Username.PlaceholderText = "Tên tài khoản";
            this.txt_Username.SelectedText = "";
            this.txt_Username.ShadowDecoration.Parent = this.txt_Username;
            this.txt_Username.Size = new System.Drawing.Size(320, 60);
            this.txt_Username.TabIndex = 73;
            // 
            // btn_Login
            // 
            this.btn_Login.AutoRoundedCorners = true;
            this.btn_Login.BorderRadius = 29;
            this.btn_Login.CheckedState.Parent = this.btn_Login;
            this.btn_Login.CustomImages.Parent = this.btn_Login;
            this.btn_Login.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Login.ForeColor = System.Drawing.Color.White;
            this.btn_Login.HoverState.Parent = this.btn_Login;
            this.btn_Login.Location = new System.Drawing.Point(75, 559);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.ShadowDecoration.Parent = this.btn_Login;
            this.btn_Login.Size = new System.Drawing.Size(320, 60);
            this.btn_Login.TabIndex = 75;
            this.btn_Login.Text = "Đăng Nhập";
            this.btn_Login.Click += new System.EventHandler(this.bn_Login_Click);
            // 
            // F_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(981, 718);
            this.Controls.Add(this.btn_Login);
            this.Controls.Add(this.txt_Password);
            this.Controls.Add(this.txt_Username);
            this.Controls.Add(this.lb_Forgot_Password);
            this.Controls.Add(this.chk_Save_Account);
            this.Controls.Add(this.lb_Save_Account);
            this.Controls.Add(this.lb_Password);
            this.Controls.Add(this.lb_Username);
            this.Controls.Add(this.lb_Title_Desc);
            this.Controls.Add(this.lb_Title_Login);
            this.Controls.Add(this.pic_Logo);
            this.Controls.Add(this.pl_Top);
            this.Controls.Add(this.pic_Right);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "F_Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            ((System.ComponentModel.ISupportInitialize)(this.pic_Logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_Right)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pic_Right;
        private Bunifu.Framework.UI.BunifuElipse elip_F_Login;
        private System.Windows.Forms.Panel pl_Top;
        private System.Windows.Forms.PictureBox pic_Logo;
        private System.Windows.Forms.Label lb_Title_Login;
        private System.Windows.Forms.Label lb_Title_Desc;
        private System.Windows.Forms.Label lb_Username;
        private System.Windows.Forms.Label lb_Password;
        private System.Windows.Forms.Label lb_Forgot_Password;
        private Bunifu.Framework.UI.BunifuCheckbox chk_Save_Account;
        private System.Windows.Forms.Label lb_Save_Account;
        private Guna.UI2.WinForms.Guna2TextBox txt_Password;
        private Guna.UI2.WinForms.Guna2TextBox txt_Username;
        private Guna.UI2.WinForms.Guna2Button btn_Login;
    }
}

