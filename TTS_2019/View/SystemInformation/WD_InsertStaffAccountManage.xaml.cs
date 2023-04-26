using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_InsertStaffAccountManage.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertStaffAccountManage : Window
    {
        public WD_InsertStaffAccountManage()
        {
            InitializeComponent();
        }
        //1.0 实例化服务
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        BLL.UC_StaffAccountManage.UC_StaffAccountManageClient myClient = new BLL.UC_StaffAccountManage.UC_StaffAccountManageClient();
        //1.1 页面加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 下拉框绑定
            DataTable dt = myClient.Window_Loaded_SelectStaffManage().Tables[0];
            cbo_Name.ItemsSource = dt.DefaultView;
            cbo_Name.SelectedValuePath = "staff_id";//id
            cbo_Name.DisplayMemberPath = "staff_name";//name

            DataTable dtGroup = myClient.Window_Loaded_SelectUserGroup().Tables[0];
            cbo_Group.ItemsSource = dtGroup.DefaultView;
            cbo_Group.SelectedValuePath = "group_id";//id
            cbo_Group.DisplayMemberPath = "group_name";//name
            #endregion
        }
        //1.3 保存新增
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //获取页面数据
                int intID = Convert.ToInt32(cbo_Name.SelectedValue);
                string strAccounts = txt_Account.Text.Trim();
                int intgroup_id = Convert.ToInt32(cbo_Group.SelectedValue);
                string strPassword = PB_Password.Password.Trim();
                bool blEffective = (bool)chk_Effect.IsChecked;
                string strNote = txt_Note.Text.Trim();
                //判断页面数据不为空
                if (intID > 0 && strAccounts != "" && strPassword != "")
                {
                    //执行服务方法
                    int count = myClient.btn_Affirm_Click_InsertStaffAccountManage(intID,intgroup_id, strAccounts, strPassword, blEffective, strNote);
                    if (count > 0)
                    {
                        MessageBoxResult dr = MessageBox.Show("您新注册了一个账号！", "系统提示", MessageBoxButton.OKCancel,
                            MessageBoxImage.Asterisk); //弹出确定对话框
                        if (dr == MessageBoxResult.OK) //如果点了确定按钮
                        {
                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63,
                             "注册【" + strAccounts + "】账号", DateTime.Now);
                            this.Close();
                        }
                    }
                    else if (count == -1)
                    {
                        MessageBox.Show("账号重复！", "系统提示", MessageBoxButton.OKCancel,
                           MessageBoxImage.Exclamation); //弹出确定对话框
                    }
                }
                else
                {
                    MessageBox.Show("请把页面数据填写完整！", "系统提示", MessageBoxButton.OK,
                           MessageBoxImage.Warning); //弹出确定对话框
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        //1.4 取消
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                //关闭窗口
                this.Close();
            }
        }

      
    }
}
