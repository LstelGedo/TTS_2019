using Microsoft.Win32;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// UC_StaffAccountManage.xaml 的交互逻辑
    /// </summary>
    public partial class UC_StaffAccountManage : UserControl
    {
        public UC_StaffAccountManage()
        {
            InitializeComponent();
        }
        #region 全局变量
        //实例化服务
        BLL.UC_StaffAccountManage.UC_StaffAccountManageClient myClient = new BLL.UC_StaffAccountManage.UC_StaffAccountManageClient();
        BLL.PublicFunction.PublicFunctionClient MYPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        #endregion
        //1.0 定义查询数据方法
        public void SelectDataGrid()
        {
            System.Data.DataTable dt = myClient.UserControl_Loaded_SelectStaffAccountManage().Tables[0];
            DataRow dr = dt.NewRow();
            dr["staff_name"] = "合 计：";
            dr["operator_id"] = dt.Compute("SUM(operator_id)", null);//退货数量合计
            dr["staff_id"] = dt.Compute("SUM(staff_id)", null);//退货金额合计
            dt.Rows.Add(dr);
            dgAccountManage.ItemsSource = dt.DefaultView;
        }
        //1.1 页面加载事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.IsEnabled = false;//禁用修改按钮
            btnUpdate.Opacity = 0.5;
            btnDelete.IsEnabled = false;//禁用删除按钮
            btnDelete.Opacity = 0.5;
                      
            //调用绑定表格数据
            SelectDataGrid();
        }
        //1.2 文本框改变事件（模糊查询）
        private void txt_Select_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string select = "";
            string strS = txt_Select.Text.Trim();
            if (strS != "")
            {
                //模糊查询内容
                select += " staff_name like '%" + strS + "%'" +
                          " or operator_accounts like '%" + strS + "%'" +
                          " or operator_password like '%" + strS + "%'" +
                          " or note like '%" + strS + "%'";
            }
            System.Data.DataTable dtselect = myClient.UserControl_Loaded_SelectStaffAccountManage().Tables[0];
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
            dgAccountManage.ItemsSource = dt.DefaultView;
        }
        //1.3 新增（弹出窗口）
        private void btnInert_Click(object sender, RoutedEventArgs e)
        {
            //弹出新增窗口
            WD_InsertStaffAccountManage myInsert = new WD_InsertStaffAccountManage();
            myInsert.ShowDialog();
            //刷新表格
            SelectDataGrid();
        }
        //1.4 修改（弹出窗口）
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //1.0 判断选中数据
                //获取选中行的ID
                int intStaffid = Convert.ToInt32(((DataRowView)dgAccountManage.SelectedItem).Row["staff_id"]);

                if (intStaffid != 0)
                {
                    //1.1 数据回填（选中行数据）
                    WD_UpdateStaffAccountManage myWD_UpdateStaffAccountManage =
                        new WD_UpdateStaffAccountManage((DataRowView)dgAccountManage.SelectedItem);
                    myWD_UpdateStaffAccountManage.ShowDialog();
                    SelectDataGrid(); //绑定账号数据:刷新表格
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        //1.5 表格改变事件
        private void dgAccountManage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((DataRowView)dgAccountManage.CurrentItem != null)
            {
                //激活修改与删除按钮
                btnUpdate.IsEnabled = true;
                btnUpdate.Opacity = 1;
                btnDelete.IsEnabled = true;
                btnDelete.Opacity = 1;
            }
            else
            {
                //禁用修改与删除按钮
                btnUpdate.IsEnabled = false;
                btnUpdate.Opacity = 0.5;
                btnDelete.IsEnabled = false;
                btnDelete.Opacity = 0.5;
            }
        }
        //1.6 删除
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //弹出确定对话框
                if (dr == MessageBoxResult.OK) //如果点了确定按钮
                {
                    //获取选中行的ID
                    int intID = Convert.ToInt32(((DataRowView)dgAccountManage.SelectedItem).Row["operator_id"]);
                    string Accounts = (((DataRowView)dgAccountManage.SelectedItem).Row["operator_accounts"]).ToString();
                    if (intID != 0) //如果会员类别ID不为0
                    {
                        int count = myClient.btn_Affirm_Click_DeleteStaffAccountManage(intID); //执行删除事件 
                        if (count > 0)
                        {
                            //新增操作日志
                            MYPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 65, "删除【" + Accounts + "】账户信息",
                       DateTime.Now);
                            MessageBox.Show("删除数据成功！");
                            SelectDataGrid(); //绑定账号数据:刷新表格                            
                        }
                        else
                        {
                            MessageBox.Show("删除数据失败！");
                        }
                    }
                }


            }
            catch (Exception)
            {
                return;
            }
        }
        #region 导入导出        
        //1.7 导入Excel（弹出页面）
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            //实例化窗口
            WD_ImportStaffAccount myImport = new WD_ImportStaffAccount();
            myImport.ShowDialog();
            SelectDataGrid(); //绑定账号数据:刷新表格              
        }
        //1.8 导出Excel
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //获取将要导出的数据，这些数据都存于DataTable中
                System.Data.DataTable dt = ((DataView)dgAccountManage.ItemsSource).ToTable();
                if (dt.Rows.Count > 0)
                {
                    //#region 第一种：原始导出                    
                    ////（1）创建Excel
                    //Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                    ////（2）创建工作簿（WorkBook：即Excel文件主体本身）
                    //Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);
                    ////（3）创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出 
                    //Worksheet excelWS = (Worksheet)excelWB.Worksheets[1];

                    ////如果数据中存在数字类型 可以让它变文本格式显示
                    //excelWS.Cells.NumberFormat = "@";                  

                    ////（5）将数据导入到工作表的单元格
                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    for (int j = 0; j < dt.Columns.Count; j++)
                    //    {
                    //        //Excel单元格第一个从索引1开始
                    //        excelWS.Cells[i + 1, j + 1] = dt.Rows[i][j].ToString();
                    //    }
                    //}
                    ////将其进行保存到指定的路径
                    //excelWB.SaveAs("D:\\员工账户信息Excel文件.xlsx");
                    //excelWB.Close();
                    //excelApp.Quit();
                    ////KillAllExcel(excelApp);释放可能还没释放的进程                    
                    //#endregion

                    //第二种方法：调用方法导出   
                    if (ExportToExcel.Export(dgAccountManage, "员工账号")== true)
                    {
                        MessageBox.Show("数据导出成功!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("数据导出失败!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                }
                else
                {
                    MessageBox.Show("无数据!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
    }
}
