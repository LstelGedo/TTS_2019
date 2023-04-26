using System;
using System.Data;
using System.Windows;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_UpdateStaffAccountManage.xaml 的交互逻辑
    /// </summary>
    public partial class WD_UpdateStaffAccountManage : Window
    {
        #region  全局变量
        DataRowView dgAccountManage;
        //实例化服务
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        BLL.UC_StaffAccountManage.UC_StaffAccountManageClient myClient = new BLL.UC_StaffAccountManage.UC_StaffAccountManageClient();
        #endregion
        public WD_UpdateStaffAccountManage(DataRowView Drv)
        {
            InitializeComponent();
            //获取页面传递过来的那一条数据
            dgAccountManage = Drv;
        }
        //1.0 页面加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 下拉框绑定          
            //权限组
            DataTable dtGroup = myClient.Window_Loaded_SelectUserGroup().Tables[0];
            cbo_Group.ItemsSource = dtGroup.DefaultView;
            cbo_Group.SelectedValuePath = "group_id";//id
            cbo_Group.DisplayMemberPath = "group_name";//name
            #endregion
            #region 页面数据回填（回显）
            txt_Account.Text = (dgAccountManage.Row["operator_accounts"]).ToString();
            txt_Note.Text = (dgAccountManage.Row["note"]).ToString();
            PB_Password.Password = (dgAccountManage.Row["operator_password"]).ToString();
            txt_Name.Text = dgAccountManage.Row["staff_name"].ToString().Trim();
            cbo_Group.SelectedValue = Convert.ToInt32(dgAccountManage.Row["group_id"]);
            if (dgAccountManage.Row["effective"].ToString() == "启用")
            {
                chk_Effect.IsChecked = true;
            }
            else
            {
                chk_Effect.IsChecked = false;

            }
            #endregion
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(cbo_Group.SelectedValue) != 0 && txt_Account.Text!="" && txt_Note.Text!="")
                {
                    //获取页面数据
                    int intID = Convert.ToInt32(dgAccountManage.Row["staff_id"]);
                    int intGroupID = Convert.ToInt32(cbo_Group.SelectedValue);
                    string strAccounts = txt_Account.Text.Trim();
                    string strPassword = PB_Password.Password.Trim();
                    bool blEffective = (bool)chk_Effect.IsChecked;
                    string strNote = txt_Note.Text.Trim();
                    int intOperatorID = Convert.ToInt32(dgAccountManage.Row["operator_id"]);
                    //执行服务方法
                    int count = myClient.btn_Affirm_Click_UpdateStaffAccountManage(intID, intGroupID, strAccounts, strPassword, blEffective, strNote, intOperatorID);
                    if (count > 0)
                    {
                        MessageBoxResult dr = MessageBox.Show("账号修改成功！", "系统提示", MessageBoxButton.OKCancel,
                            MessageBoxImage.Information); //弹出确定对话框
                        if (dr == MessageBoxResult.OK) //如果点了确定按钮
                        {
                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 64, "修改【" + strAccounts + "】账号信息", DateTime.Now);
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBoxResult dr = MessageBox.Show("账号修改失败，请重新操作！", "系统提示", MessageBoxButton.OKCancel,
                            MessageBoxImage.Exclamation); //弹出确定对话框
                    }
                }
                else {
                    MessageBoxResult dr = MessageBox.Show("请把页面数据填写完整！", "系统提示", MessageBoxButton.OKCancel,
                          MessageBoxImage.Error); //弹出确定对话框
                }
                
            }
            catch (Exception)
            {
                return;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
    }
}
