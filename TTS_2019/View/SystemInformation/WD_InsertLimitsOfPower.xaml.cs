using System;
using System.Data;
using System.Windows;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_InsertLimitsOfPower.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertLimitsOfPower : Window
    {
        public WD_InsertLimitsOfPower()
        {
            InitializeComponent();
        }
        BLL.UC_PowerManage.UC_PowerManageClient myClient = new BLL.UC_PowerManage.UC_PowerManageClient();
        DataTable dtModel = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            dtModel = myClient.limitsOfPower_Loaded_SelectAllModular().Tables[0];
            dgModel.ItemsSource = dtModel.DefaultView;
        }
        //1.1 保存按钮（单击事件）
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //---1、新增权限组
                //(1)获取页面数据判断
                string strPname = txt_Name.Text.Trim();
                string strRemarks = txt_Remark.Text.Trim();
                if (strPname != string.Empty && strRemarks != string.Empty)
                {
                    //(2) 循环勾选的表格信息 --获取模块ID
                    string strPgroup = string.Empty;
                    for (int i = 0; i < dgModel.Items.Count; i++)
                    {
                        if (Convert.ToBoolean(dtModel.Rows[i]["chked"]) == true)
                        {
                            // 获取模块ID
                            int intFid = Convert.ToInt32(((DataRowView)dgModel.Items[i]).Row["modular_id"]);
                            string strAsOperationId = string.Empty;
                            //(3) --获取操作ID
                            if (Convert.ToBoolean(dtModel.Rows[i]["SelectID"]) == true)
                            {
                                strAsOperationId += "62,";
                            }
                            if (Convert.ToBoolean(dtModel.Rows[i]["InsertID"]) == true)
                            {
                                strAsOperationId += "63,";
                            }
                            if (Convert.ToBoolean(dtModel.Rows[i]["UpdateID"]) == true)
                            {
                                strAsOperationId += "64,";
                            }
                            if (Convert.ToBoolean(dtModel.Rows[i]["DeleteID"]) == true)
                            {
                                strAsOperationId += "65,";
                            }
                            //(4) --新增模块操作
                            DataTable dtModularDetailId = myClient.Window_Loaded_InsertModularOperation(intFid, strAsOperationId, strPname).Tables[0];
                            if (dtModularDetailId.Rows.Count > 0)
                            {
                                //(5) --提取操作模块ID 拼接
                                //获取单元格（站点ID）                           
                                strPgroup += dtModularDetailId.Rows[0][0].ToString().Trim() + ',';
                            }
                            else {
                                MessageBox.Show("已存在当前权限组，请重新输入！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Error);
                            }                           
                        }
                    }
                    int intCount = Convert.ToInt32(myClient.Window_Loaded_InsertPermissionGroup(strPname, strPgroup, strRemarks));
                    if (intCount == -1)
                    {
                        MessageBox.Show("已存在当前权限组，请重新输入！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (intCount > 0)
                    {                       
                        MessageBoxResult dr = MessageBox.Show("您新增了一个权限组：" + strPname, "系统提示,是否继续新增？", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        //弹出确定对话框
                        if (dr == MessageBoxResult.Yes) //如果点了确定按钮
                        {
                            txt_Name.Text = string.Empty;
                            txt_Remark.Text = string.Empty;
                            dgModel.ItemsSource =  myClient.limitsOfPower_Loaded_SelectAllModular().Tables[0].DefaultView;
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请把页面数据填写完成！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (System.Exception)
            {
                return;
            }
        }
        //1.2 取消按钮（单击事件）
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel,
                MessageBoxImage.Information); //弹出确定对话框
            if (dr == MessageBoxResult.OK) //如果点了确定按钮
            {
                //关闭窗口
                this.Close();
            }
        }
    }
}
