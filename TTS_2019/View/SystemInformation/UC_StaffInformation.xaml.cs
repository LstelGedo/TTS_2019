using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// UC_StaffInformation.xaml 的交互逻辑
    /// </summary>
    public partial class UC_StaffInformation : UserControl
    {
        public UC_StaffInformation()
        {
            InitializeComponent();
        }
        #region 全局变量
        BLL.UC_StaffInformation.UC_StaffInformationClient myClient = new BLL.UC_StaffInformation.UC_StaffInformationClient();     
        private string myPictureByte; //图片      
       System.Data.DataTable dt;
        #endregion
        //1.0 绑定表格
        public void SelectDataGrid()
        {
            //执行服务端的方法获取表格 数据
            dt= myClient.UserControl_Loaded_SelectStaff().Tables[0];              
            //绑定表格
            dgvStaff.ItemsSource = dt.DefaultView;
        }
        //1.1 页面加载
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //初始化操作按钮
            btnUpdate.IsEnabled = false;//禁用停用按钮
            btnUpdate.Opacity = 0.5;
            btnDelete.IsEnabled = false;//禁用停用按钮
            btnDelete.Opacity = 0.5;
            btnPrint.IsEnabled = false;//禁用停用按钮
            btnPrint.Opacity = 0.5;

          
            //调用绑定表格数据
            SelectDataGrid();
        }
        // 弹出新增窗口按钮事件
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            //取出第一条数据的编号
            string strNumber = dt.Rows[0]["staff_number"].ToString();
            //从右边开始取4个字符
            int str =Convert.ToInt32(strNumber.Substring(strNumber.Length - 4));

            //实例化窗口
            WD_InsertStaffInformation myWD_InsertStaffInformation = new WD_InsertStaffInformation(str);
            //打开窗口
            myWD_InsertStaffInformation.ShowDialog();
            //调用绑定表格数据
            SelectDataGrid();

        }
        // 弹出修改窗口
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //1.选中数据                
                //获取行
                DataRowView dv = (DataRowView)dgvStaff.CurrentItem;
                //获取单元格
                int staffid = Convert.ToInt32(dv.Row["staff_id"]);
                if (staffid != 0)
                {
                    //2.弹出窗口
                    WD_UpdateStaffInformation myUpdate = new WD_UpdateStaffInformation(dv);
                    myUpdate.ShowDialog();
                    //调用绑定表格数据
                    SelectDataGrid();
                }
                else
                {
                    MessageBox.Show("请选择要删除的行!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        // 删除按钮
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //弹出确定对话框
                if (dr == MessageBoxResult.OK) //如果点了确定按钮
                {
                    //1、获取选中行的数据
                    int staffid = Convert.ToInt32(((DataRowView)dgvStaff.CurrentItem).Row["staff_id"]); // 获取所点击的会员类别ID               
                    //提取图片名字   
                    string strpicture = ((DataRowView)dgvStaff.CurrentItem).Row["picture"].ToString().Trim();
                    if (staffid != 0 && strpicture != string.Empty) //如果会员类别ID不为0
                    {
                        //2、执行删除操作     
                        myClient.btn_Delete_Click_DelectStaff(staffid, strpicture); //执行删除事件                     
                        SelectDataGrid();//刷新页面表格数据
                        MessageBox.Show("删除成功!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        MessageBox.Show("请选择要删除的行!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        // 打印按钮
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if ((DataRowView)dgvStaff.CurrentItem != null)
            {
                int staffid = Convert.ToInt32(((DataRowView)dgvStaff.CurrentItem).Row["staff_id"]);
                if (staffid != 0)
                {
                    //实例化窗口
                    ReportForms.WD_Staff myWD_Staff = new ReportForms.WD_Staff((DataRowView)dgvStaff.CurrentItem);
                    myWD_Staff.ShowDialog();//弹出窗口

                }
            }
            else
            {
                MessageBox.Show("请选择要打印工作证的员工信息！");
            }

        }
        // 表格选择单元格改变事件
        private void dgvStaff_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //获取图片
            try
            {
                #region 图片显示
                if ((DataRowView)dgvStaff.CurrentItem != null)
                {
                    //激活修改与删除按钮
                    btnUpdate.IsEnabled = true;
                    btnUpdate.Opacity = 1;
                    btnDelete.IsEnabled = true;
                    btnDelete.Opacity = 1;
                    btnPrint.IsEnabled = true;
                    btnPrint.Opacity = 1;

                    //查询数据
                    SelectDataGrid();
                    //获取图片
                    string strLuJing = (((DataRowView)dgvStaff.CurrentItem).Row["picture"]).ToString();
                    txtbContent.Text=(((DataRowView)dgvStaff.CurrentItem).Row["note"]).ToString();
                    myPictureByte = myClient.UserControl_Loaded_SelectPhoro(strLuJing);
                    if (myPictureByte != null)
                    {
                        //BitmapImage images = new BitmapImage(new Uri((myPictureByte).ToString()));
                        ////绑定图片
                        //img_photo.Source = images;
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        //增加这一行（指定位图图像如何利用内存缓存=在加载时将整个图像缓存到内存中。对图像数据的所有请求将通过内存存储区进行填充。）                
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(myPictureByte);
                        bi.EndInit();
                        //获取内存图片
                        img_photo.Source = bi;                        
                    }
                    else
                    {
                        img_photo.Source = null;
                    }
                }
                #endregion
            }
            catch (Exception)
            {
                return;
            }
        }
        //回车键
        private void txt_Select_KeyDown(object sender, KeyEventArgs e)
        {
            string select = "";
            string strS = txt_Select.Text.Trim();
            if (strS != "")
            {
                //模糊查询内容
                select += " staff_name like '%" + strS + "%'" +
                          " or staff_number like '%" + strS + "%'" +
                          " or gender like '%" + strS + "%'" +
                          " or branch like '%" + strS + "%'";
            }
            System.Data.DataTable dtselect = myClient.UserControl_Loaded_SelectStaff().Tables[0];
            DataView dv = new DataView(dtselect);
            System.Data.DataTable dt = new System.Data.DataTable();

            if (select != "")
            {
                //筛选数据
                dv.RowFilter = select;
                dt = dv.ToTable();
            }
            if (select == "")
            {
                //查询全部数据
                dt = dv.ToTable();
            }
            dgvStaff.ItemsSource = dt.DefaultView;
        }
        //导出数据
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            //ExportExcel.ExportDataGridSaveAs(true, dgvStaff, dt);
            #region 第一种：原始导出                    
            //（1）创建Excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            //（2）创建工作簿（WorkBook：即Excel文件主体本身）
            Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);
            //（3）创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出 
            Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];
            //如果数据中存在数字类型 可以让它变文本格式显示
            excelWS.Cells.NumberFormat = "@";

            //（5）将数据导入到工作表的单元格
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    //Excel单元格第一个从索引1开始
                    excelWS.Cells[i + 1, j + 1] = dt.Rows[i][j].ToString();
                }
            }
            //将其进行保存到指定的路径
            excelWB.SaveAs("D:\\员工信息Excel文件.xlsx");
            excelWB.Close();
            excelApp.Quit();
            //KillAllExcel(excelApp);释放可能还没释放的进程                    
           #endregion
        }
    }
}
