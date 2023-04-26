using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_ImportStaffAccount.xaml 的交互逻辑
    /// </summary>
    public partial class WD_ImportStaffAccount : System.Windows.Window
    {
        public WD_ImportStaffAccount()
        {
            InitializeComponent();
        }
        BLL.UC_StaffAccountManage.UC_StaffAccountManageClient myClient = new BLL.UC_StaffAccountManage.UC_StaffAccountManageClient();
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        //1.1 下载模板
        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //（1）、获取项目中文件
                string WantedPath = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.LastIndexOf(@"\"));
                string path2 = System.IO.Path.GetDirectoryName(WantedPath);
                path2 = path2 + @"\Excel\账号信息Excel文件.xls";
                //（2）、用户选择目录
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.ShowDialog();
                var s = fbd.SelectedPath;
                if (fbd.SelectedPath != string.Empty)
                {
                    File.Copy(path2, fbd.SelectedPath + "\\账号信息Excel文件.xls", true);
                    MessageBox.Show("下载完毕!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("文件不存在!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //1.2 导入Excel
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //（1）、选中文件
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "选择数据源文件";
                openFileDialog.Filter = "Excel文件|*.xls|所有文件|*.*";
                openFileDialog.FileName = string.Empty;
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.DefaultExt = "xls";
                if (openFileDialog.ShowDialog() == false)
                {
                    return;
                }
                //文件对话框中选定的文件的完整路径
                string pPath = openFileDialog.FileName;
                //（2）、通过路径获取到的数据
                DataTable dt = ImportToExcel.ChangeExcelToDateTable(pPath);
                if (dt.Rows.Count>0)
                {
                    //此时我们就可以用这数据进行处理了，比如绑定到显示数据的控件当中去
                    dgAccountManage.ItemsSource = dt.DefaultView;                  
                    MessageBox.Show("导入成功!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("表格无数据!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }              
            }
            catch (Exception)
            {
                MessageBox.Show("数据异常!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //1.3 保存数据
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            /*
               保存数据逻辑思路：
               （1）判断数据准确性：是否准确(判断员工，判断权限)，是否存在重复数据。
               （2）保存数据：批量保存数据。
            */
            try
            {
                //获取表格ItemsSource并转化为DataTable
                DataTable dt = (dgAccountManage.ItemsSource as DataView).ToTable();
                //初始化两个变量：分别记录保存成功条数、失败条数（找不到）和存在条数。
                int intSuccess=0, intNull = 0, intRepeat = 0;
                #region 1、处理dt表格数据并绑定到dgAccountManage   
                //实例化表格（接收需要新增的数据）     
                DataTable dtSaveData = new DataTable();
                //添加列(通过列架构添加列)
                dtSaveData.Columns.Add("staff_id", typeof(int));
                dtSaveData.Columns.Add("p_group_id", typeof(int));
                dtSaveData.Columns.Add("accounts", typeof(string));
                dtSaveData.Columns.Add("password", typeof(string));
                dtSaveData.Columns.Add("note", typeof(string));
                //（2）DataTable增加数据，先请空行的信息
                dtSaveData.Rows.Clear();//清空数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //数据库查询数据:根据“姓名”获取staff_id
                    string strName = dt.Rows[i]["姓名"].ToString().Trim();                   
                    DataTable dtStaffId=myClient.btnSave_Click_SeleteStaffId(strName).Tables[0];
                    //根据“权限组”获取 permission_group_id     
                    string strPGroup = dt.Rows[i]["权限组"].ToString().Trim();
                    DataTable dtPGroup = myClient.btnSave_Click_SeletePGroupId(strPGroup).Tables[0];
                    if (dtStaffId.Rows.Count>0 && dtPGroup.Rows.Count>0)
                    {
                        //添加数据行
                        DataRow dr = dtSaveData.NewRow();
                        dr[0] = dtStaffId.Rows[0]["staff_id"];
                        dr[1] = dtPGroup.Rows[0]["p_group_id"];
                        dr[2] = dt.Rows[i]["员工账号"];
                        dr[3] = dt.Rows[i]["密码"];                       
                        dr[4] = dt.Rows[i]["备注"];
                        //添加数据给表格                    
                        dtSaveData.Rows.Add(dr);                        
                    }
                    else
                    {
                        //找不到用户和权限组。
                        intNull = intNull + 1;
                    }
                }
                #endregion
                #region 2、批量新增数据
                //批量新增数据
                if (dtSaveData.Rows.Count>0)
                {
                    for (int i = 0; i < dtSaveData.Rows.Count; i++)
                    {
                        //（1）获取表格单元格数据
                      
                        int intID = Convert.ToInt32(dtSaveData.Rows[i]["staff_id"]);
                        int intgroup_id = Convert.ToInt32(dtSaveData.Rows[i]["p_group_id"]);
                        string strAccounts = dtSaveData.Rows[i]["accounts"].ToString().Trim();
                        string strPassword = dtSaveData.Rows[i]["password"].ToString().Trim();
                        bool blEffective = true;
                        string strNote = dtSaveData.Rows[i]["note"].ToString().Trim();
                        //（2）执行新增操作                      
                        int count = myClient.btn_Affirm_Click_InsertStaffAccountManage(intID, intgroup_id, strAccounts, strPassword, blEffective, strNote);
                        if (count > 0)
                        {
                            //成功条数
                            intSuccess = intSuccess + 1;
                        }
                        else if (count == -1)
                        {
                            //重复条数
                            intRepeat = intRepeat + 1;
                        }
                    }
                    MessageBoxResult dr = MessageBox.Show("导入成功" + intSuccess + "条数\n已存在" + intRepeat + "条数\n不存在用户或者权限" + intNull + "条数", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk); //弹出确定对话框
                    if (dr == MessageBoxResult.OK) //如果点了确定按钮
                    {
                        myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63,"导入账号数据", DateTime.Now);
                        this.Close();
                    }                   
                }
                else
                {
                    MessageBox.Show("没有满足条件的数据："+ intNull + "条", "提示",MessageBoxButton.OK,MessageBoxImage.Error);
                }
                #endregion
            }
            catch (Exception)
            {
                MessageBox.Show("没有满足条件的数据，请重新填写数据。", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
