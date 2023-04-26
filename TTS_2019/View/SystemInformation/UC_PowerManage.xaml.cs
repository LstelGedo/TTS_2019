using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// UC_PowerManage.xaml 的交互逻辑
    /// </summary>
    public partial class UC_PowerManage : UserControl
    {
        public UC_PowerManage()
        {
            InitializeComponent();
        }
        //实例化命名空间
        BLL.UC_PowerManage.UC_PowerManageClient myClient = new BLL.UC_PowerManage.UC_PowerManageClient();
        //绑定查询表格方法
        private void SelectDataGridLimits()
        {
            DataTable dtLimits = myClient.limitsOfPower_Loaded_SelectPermissionGroup().Tables[0];
            dgLimits.ItemsSource = dtLimits.DefaultView;
        }

        //1.0 页面加载事件
        private void limitsOfPower_Loaded(object sender, RoutedEventArgs e)
        {
            #region 页面内容初始化
            //停用按钮           
            btnUpdate.IsEnabled = true;
            btnUpdate.Opacity = 1;          
            #endregion
            //刷新表格数据
            SelectDataGridLimits();
        }

        //1.2 选中行改变事件
        private void dgLimits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //判断选中的行是否为空
                if (((DataRowView)dgLimits.SelectedItem) != null)
                {
                    //激活停用按钮                    
                    btnUpdate.IsEnabled = true;
                    btnUpdate.Opacity = 1;
                }
                else
                {
                    //禁用停用按钮                   
                    btnUpdate.IsEnabled = false;
                    btnUpdate.Opacity = 0.5;
                }
                //获取权限组模块id
                string strPermissionGroup = ((DataRowView)dgLimits.CurrentItem).Row["p_group"].ToString().Trim();
                if (strPermissionGroup != string.Empty)
                {
                    //绑定右边表格
                    DataTable dtModular = myClient.limitsOfPower_Loaded_SelectModular(strPermissionGroup).Tables[0];
                    //取出操作id
                    for (int i = 0; i < dtModular.Rows.Count; i++)
                    {
                        DataTable dtOperation = myClient.limitsOfPower_Loaded_SelectOperation(dtModular.Rows[i]["as_operation_id"].ToString().Trim()).Tables[0];
                        for (int j = 0; j < dtOperation.Rows.Count; j++)
                        {
                            int intID = Convert.ToInt32(dtOperation.Rows[j]["Id"].ToString());
                            if (intID == 62)
                            {
                                dtModular.Rows[i]["SelectID"] = true;
                            }
                            else if (intID == 63)
                            {
                                dtModular.Rows[i]["InsertID"] = true;
                            }
                            else if (intID == 64)
                            {
                                dtModular.Rows[i]["UpdateID"] = true;
                            }
                            else if (intID == 65)
                            {
                                dtModular.Rows[i]["DeleteID"] = true;
                            }
                        }
                    }
                    dgModel.ItemsSource = dtModular.DefaultView;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        //1.3 新增按钮（单击事件）
        private void btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            WD_InsertLimitsOfPower myWD_InsertLimitsOfPower = new WD_InsertLimitsOfPower();
            myWD_InsertLimitsOfPower.ShowDialog();
            //刷新表格数据
            SelectDataGridLimits();
        }

        //1.4 修改按钮（单击事件）
        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((DataRowView)dgLimits.SelectedItem) != null)
                {
                    WD_UpdateLimitsOfPower myWD_UpdateLimitsOfPower = new WD_UpdateLimitsOfPower((DataRowView)dgLimits.SelectedItem);
                    myWD_UpdateLimitsOfPower.ShowDialog();
                    //刷新表格数据
                    SelectDataGridLimits();
                }
                else
                {
                    MessageBox.Show("请选择一行数据进行操作！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        //1.5 删除按钮（单击事件）
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //记录删除成功条数
                int intDSum = 0;
                //判断是否选中数据
                MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //弹出确定对话框
                if (dr == MessageBoxResult.OK) 
                {
                    int intPGroupId = Convert.ToInt32(((DataRowView)dgLimits.SelectedItem).Row["p_group_id"]); //权限组ID
                    //1.删除单条数据(权限组信息)
                    int intPGroupCount = myClient.Window_Loaded_DeletePermissionGroup(intPGroupId);
                    //（站点表）删除成功
                    if (intPGroupCount > 0)
                    {

                        //2、批量删除模块操作明细表
                        for (int i = 0; i < dgModel.Items.Count; i++)
                        {
                            intDSum++;
                            //获取主键ID
                            int intModularDetailId = Convert.ToInt32(((DataRowView)dgModel.Items[i]).Row["modular_detail_id"]);
                            int intNeighborCount = myClient.Window_Loaded_DeleteModularOperation(intModularDetailId);
                        }
                        //删除行数相同
                        if (intDSum == dgModel.Items.Count)
                        {
                            MessageBox.Show("数据删除成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else if(intPGroupCount == -1)
                    {
                        MessageBox.Show("用户正在使用该权限组，不能执行删除！", "系统提示", MessageBoxButton. OK, MessageBoxImage.Error);
                    }
                   
                }
                //调用绑定表格方法
                SelectDataGridLimits();
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
