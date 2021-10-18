using Dormitary_Management_2021;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Dormitory_Management_2021.GUI.Statistic
{
    public partial class UC_Statistic : UserControl
    {
        public UC_Statistic()
        {
            InitializeComponent();
        }

        private void btn_PDF_Click(object sender, EventArgs e)
        {
            //DGVPrinter printer = new DGVPrinter();
            //printer.Title = $"THỐNG KÊ CHI TIẾT";
            //string temp = $"{lb_Total_Buiding_Room.Text}\r\n" +
            //                $"{lb_Total_Room.Text}\r\n" +
            //                $"{lb_Total_Room_Empty.Text}\r\n" +
            //                $"{lb_Total_Employee.Text}\r\n" +
            //                $"{lb_Total_Contract.Text}\r\n" +
            //                $"{lb_Total_Student.Text}\r\n" +
            //                $"{lb_Total_Bill.Text}\r\n" +
            //                $"{lb_Total_Bill_Unpaid.Text}\r\n";
            //printer.SubTitle = temp + $"Ngày in: {DateTime.Now.Date.ToString("dd/MM/yyyy")}";
            //printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            //printer.PageNumbers = true;
            //printer.PageNumberInHeader = false;
            //printer.PorportionalColumns = true;
            //printer.HeaderCellAlignment = StringAlignment.Near;
            //printer.Footer = "Quản Lý Ký Túc Xá Đại Học Hải Phòng 2021";
            //printer.FooterSpacing = 15;

            //using (var entity = new db_Dormitory_Management_2021Entities())
            //{
            //    //var AllTable = new List<DataGridView>();

            //    //var tbl_Room = new DataGridView();
            //    //tbl_Room.DataSource = entity.usp_Get_Room();

            //    //var tbl_Employee = new DataGridView();
            //    //tbl_Employee.DataSource = entity.usp_Get_Employee();

            //    //var tbl_Contract = new DataGridView();
            //    //tbl_Contract.DataSource = entity.usp_Get_Contract();

            //    //var tbl_Student = new DataGridView();
            //    //tbl_Student.DataSource = entity.usp_Get_Student();

            //    //var tbl_Bill = new DataGridView();
            //    //tbl_Bill.DataSource = entity.usp_Get_Bill();

            //    //AllTable.Add(tbl_Room);
            //    //AllTable.Add(tbl_Employee);
            //    //AllTable.Add(tbl_Contract);
            //    //AllTable.Add(tbl_Student);
            //    //AllTable.Add(tbl_Bill);

            //    //foreach (var item in AllTable)
            //    //{
                    
            //    //}
            //}
            //printer.Print();
        }

        private void btn_Refresh_Click(object sender, EventArgs e)
        {
            UC_Statistic_Load(sender, e);
        }

        string Total_Buiding_Room, Total_Room, Total_Room_Empty, Total_Employee, Total_Contract, Total_Student, Total_Bill, Total_Bill_Unpaid;

        private void UC_Statistic_Load(object sender, EventArgs e)
        {
            try
            {
                using (var entity = new db_Dormitory_Management_2021Entities())
                {
                    Total_Buiding_Room = entity.usp_Total_Buiding_Room().ToList().Count.ToString();
                    Total_Room = entity.usp_Total_Room().ToList()[0].ToString();
                    Total_Room_Empty = entity.usp_Total_Room_Empty().ToList()[0].ToString();
                    Total_Employee = entity.usp_Total_Employee().ToList()[0].ToString();
                    Total_Contract = entity.usp_Total_Contract().ToList()[0].ToString();
                    Total_Student = Total_Contract;
                    Total_Bill = entity.usp_Total_Bill().ToList()[0].ToString();
                    Total_Bill_Unpaid = entity.usp_Total_Bill_Unpaid().ToList()[0].ToString();


                    lb_Total_Buiding_Room.Text = "Toà Nhà: " + Total_Buiding_Room;
                    lb_Total_Room.Text = "Số Phòng: " + Total_Room;
                    lb_Total_Room_Empty.Text = "Phòng Trống: " + Total_Room_Empty;
                    lb_Total_Employee.Text = "Nhân Viên: " + Total_Employee;
                    lb_Total_Contract.Text = "Hợp Đồng: " + Total_Contract;
                    lb_Total_Student.Text = "Sinh Viên: " + Total_Student;
                    lb_Total_Bill.Text = "Hoá Đơn: " + Total_Bill;
                    lb_Total_Bill_Unpaid.Text = "Chưa Thanh Toán: " + Total_Bill_Unpaid;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
