
namespace KTX2021
{
    partial class FLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FLogin));
            this.borderRadiusFLogin = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pl_Top = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxRight = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.bn_Login = new Guna.UI2.WinForms.Guna2GradientButton();
            this.tb_Password = new System.Windows.Forms.TextBox();
            this.tb_Username = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pb_Password = new System.Windows.Forms.PictureBox();
            this.pb_Username = new System.Windows.Forms.PictureBox();
            this.lb_ForgotPassword = new System.Windows.Forms.Label();
            this.chk_SaveAccount = new Bunifu.Framework.UI.BunifuCheckbox();
            this.lb_SaveAccount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Password)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Username)).BeginInit();
            this.SuspendLayout();
            // 
            // borderRadiusFLogin
            // 
            this.borderRadiusFLogin.ElipseRadius = 60;
            this.borderRadiusFLogin.TargetControl = this;
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
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::KTX2021.Properties.Resources.DHHP;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(74, 67);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(79, 79);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBoxRight
            // 
            this.pictureBoxRight.BackgroundImage = global::KTX2021.Properties.Resources.img;
            this.pictureBoxRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBoxRight.Location = new System.Drawing.Point(480, 0);
            this.pictureBoxRight.Name = "pictureBoxRight";
            this.pictureBoxRight.Size = new System.Drawing.Size(501, 718);
            this.pictureBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxRight.TabIndex = 32;
            this.pictureBoxRight.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Open Sans", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(0)))), ((int)(((byte)(19)))));
            this.label1.Location = new System.Drawing.Point(69, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 33);
            this.label1.TabIndex = 35;
            this.label1.Text = "Đăng Nhập";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(116)))), ((int)(((byte)(142)))));
            this.label2.Location = new System.Drawing.Point(69, 224);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(336, 22);
            this.label2.TabIndex = 36;
            this.label2.Text = "Hệ thống quản lý Ký túc xá Đại Học Hải Phòng!";
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(73, 550);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(304, 65);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox7.TabIndex = 39;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(73, 431);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(304, 69);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 37;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(73, 346);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(304, 70);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 38;
            this.pictureBox3.TabStop = false;
            // 
            // bn_Login
            // 
            this.bn_Login.BackColor = System.Drawing.Color.Transparent;
            this.bn_Login.BorderRadius = 25;
            this.bn_Login.CheckedState.Parent = this.bn_Login;
            this.bn_Login.CustomImages.Parent = this.bn_Login;
            this.bn_Login.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.bn_Login.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.bn_Login.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bn_Login.ForeColor = System.Drawing.Color.White;
            this.bn_Login.HoverState.Parent = this.bn_Login;
            this.bn_Login.Location = new System.Drawing.Point(73, 559);
            this.bn_Login.Name = "bn_Login";
            this.bn_Login.ShadowDecoration.Parent = this.bn_Login;
            this.bn_Login.Size = new System.Drawing.Size(304, 47);
            this.bn_Login.TabIndex = 42;
            this.bn_Login.Text = "Đăng Nhập";
            this.bn_Login.Click += new System.EventHandler(this.bn_Login_Click);
            // 
            // tb_Password
            // 
            this.tb_Password.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(220)))), ((int)(((byte)(229)))));
            this.tb_Password.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_Password.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Password.ForeColor = System.Drawing.Color.Silver;
            this.tb_Password.Location = new System.Drawing.Point(132, 453);
            this.tb_Password.Multiline = true;
            this.tb_Password.Name = "tb_Password";
            this.tb_Password.PasswordChar = '*';
            this.tb_Password.Size = new System.Drawing.Size(230, 28);
            this.tb_Password.TabIndex = 40;
            this.tb_Password.TabStop = false;
            this.tb_Password.Text = "Password";
            this.tb_Password.Enter += new System.EventHandler(this.tb_Password_Enter);
            this.tb_Password.Leave += new System.EventHandler(this.tb_Password_Leave);
            // 
            // tb_Username
            // 
            this.tb_Username.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(220)))), ((int)(((byte)(229)))));
            this.tb_Username.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tb_Username.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Username.ForeColor = System.Drawing.Color.Silver;
            this.tb_Username.Location = new System.Drawing.Point(132, 367);
            this.tb_Username.Multiline = true;
            this.tb_Username.Name = "tb_Username";
            this.tb_Username.Size = new System.Drawing.Size(230, 30);
            this.tb_Username.TabIndex = 41;
            this.tb_Username.TabStop = false;
            this.tb_Username.Text = "123456789";
            this.tb_Username.Enter += new System.EventHandler(this.tb_Username_Enter);
            this.tb_Username.Leave += new System.EventHandler(this.tb_Username_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(116)))), ((int)(((byte)(142)))));
            this.label3.Location = new System.Drawing.Point(69, 332);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 22);
            this.label3.TabIndex = 43;
            this.label3.Text = "Tên tài khoản*";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(116)))), ((int)(((byte)(142)))));
            this.label4.Location = new System.Drawing.Point(71, 416);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 22);
            this.label4.TabIndex = 44;
            this.label4.Text = "Mật khẩu*";
            // 
            // pb_Password
            // 
            this.pb_Password.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(220)))), ((int)(((byte)(229)))));
            this.pb_Password.Image = ((System.Drawing.Image)(resources.GetObject("pb_Password.Image")));
            this.pb_Password.Location = new System.Drawing.Point(89, 449);
            this.pb_Password.Name = "pb_Password";
            this.pb_Password.Size = new System.Drawing.Size(37, 33);
            this.pb_Password.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_Password.TabIndex = 45;
            this.pb_Password.TabStop = false;
            // 
            // pb_Username
            // 
            this.pb_Username.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(220)))), ((int)(((byte)(229)))));
            this.pb_Username.Image = ((System.Drawing.Image)(resources.GetObject("pb_Username.Image")));
            this.pb_Username.Location = new System.Drawing.Point(89, 365);
            this.pb_Username.Name = "pb_Username";
            this.pb_Username.Size = new System.Drawing.Size(37, 33);
            this.pb_Username.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_Username.TabIndex = 46;
            this.pb_Username.TabStop = false;
            // 
            // lb_ForgotPassword
            // 
            this.lb_ForgotPassword.AutoSize = true;
            this.lb_ForgotPassword.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_ForgotPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.lb_ForgotPassword.Location = new System.Drawing.Point(249, 509);
            this.lb_ForgotPassword.Name = "lb_ForgotPassword";
            this.lb_ForgotPassword.Size = new System.Drawing.Size(129, 22);
            this.lb_ForgotPassword.TabIndex = 49;
            this.lb_ForgotPassword.Text = "Quên mật khẩu ?";
            this.lb_ForgotPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chk_SaveAccount
            // 
            this.chk_SaveAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.chk_SaveAccount.ChechedOffColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.chk_SaveAccount.Checked = true;
            this.chk_SaveAccount.CheckedOnColor = System.Drawing.Color.FromArgb(((int)(((byte)(81)))), ((int)(((byte)(56)))), ((int)(((byte)(238)))));
            this.chk_SaveAccount.ForeColor = System.Drawing.Color.White;
            this.chk_SaveAccount.Location = new System.Drawing.Point(74, 512);
            this.chk_SaveAccount.Name = "chk_SaveAccount";
            this.chk_SaveAccount.Size = new System.Drawing.Size(20, 20);
            this.chk_SaveAccount.TabIndex = 48;
            // 
            // lb_SaveAccount
            // 
            this.lb_SaveAccount.AutoSize = true;
            this.lb_SaveAccount.Font = new System.Drawing.Font("Open Sans Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_SaveAccount.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lb_SaveAccount.Location = new System.Drawing.Point(95, 510);
            this.lb_SaveAccount.Name = "lb_SaveAccount";
            this.lb_SaveAccount.Size = new System.Drawing.Size(105, 22);
            this.lb_SaveAccount.TabIndex = 47;
            this.lb_SaveAccount.Text = "Lưu tài khoản";
            this.lb_SaveAccount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(981, 718);
            this.Controls.Add(this.lb_ForgotPassword);
            this.Controls.Add(this.chk_SaveAccount);
            this.Controls.Add(this.lb_SaveAccount);
            this.Controls.Add(this.pb_Password);
            this.Controls.Add(this.pb_Username);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bn_Login);
            this.Controls.Add(this.tb_Password);
            this.Controls.Add(this.tb_Username);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pl_Top);
            this.Controls.Add(this.pictureBoxRight);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Password)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Username)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBoxRight;
        private Bunifu.Framework.UI.BunifuElipse borderRadiusFLogin;
        private System.Windows.Forms.Panel pl_Top;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private Guna.UI2.WinForms.Guna2GradientButton bn_Login;
        private System.Windows.Forms.TextBox tb_Password;
        private System.Windows.Forms.TextBox tb_Username;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pb_Password;
        private System.Windows.Forms.PictureBox pb_Username;
        private System.Windows.Forms.Label lb_ForgotPassword;
        private Bunifu.Framework.UI.BunifuCheckbox chk_SaveAccount;
        private System.Windows.Forms.Label lb_SaveAccount;
    }
}

