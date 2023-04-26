using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// UC_UpdatePassword.xaml 的交互逻辑
    /// </summary>
    public partial class UC_UpdatePassword : UserControl
    {
        public UC_UpdatePassword()
        {
            InitializeComponent();
        }
        BLL.UC_UpdatePassword.UC_UpdatePasswordClient myUS_UpdatePasswordClient = new BLL.UC_UpdatePassword.UC_UpdatePasswordClient();
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //回填账号信息
            tbAccount.Text = LoginWindow.strAccounts;
            DataTable dt = myUS_UpdatePasswordClient.UpdatePassword_Loaded_Selectpassword(LoginWindow.intOperatorID).Tables[0];
            txt_OldPassword.Text = (dt.Rows[0]["operator_password"]).ToString();
            #region 初始化页面
            //隐藏提示
            txtMB.Visibility = Visibility.Collapsed;
            txtAs.Visibility = Visibility.Collapsed;            
            #endregion
        }
        
       
        //1.4新密码失去焦点
        private void PB_NewPassword_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //判断密码长度是否小于六位
            if (PB_NewPassword.Password.Length<6)
            {               
                txtMB.Visibility = Visibility.Visible;
                PB_NewPassword.Password = "";             
            }
            else
            {
                txtMB.Visibility = Visibility.Collapsed;
                ////使用正则表达式判断是否匹配 
                string str = PB_NewPassword.Password.Trim();
                //只有数字
                if (Regex.IsMatch(str, @"^[0-9]+$"))
                {
                    bd_Low.Background = Brushes.Orange;
                    bd_Centre.Background = Brushes.Gray;
                    bd_High.Background = Brushes.Gray;
                }
                //只有字母
                else if (Regex.IsMatch(str, @"^[a-zA-Z]+$"))
                {
                    bd_Low.Background = Brushes.Orange;
                    bd_Centre.Background = Brushes.Gray;
                    bd_High.Background = Brushes.Gray;
                }
                //必须包含数字和字母  ^(?![0-9]+$)(?![a-zA-Z]+$)[0-9a-zA-Z]+$
                else if (Regex.IsMatch(str, @"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9a-zA-Z]+$"))
                {
                    bd_Low.Background = Brushes.Gray;
                    bd_Centre.Background = Brushes.Orange;
                    bd_High.Background = Brushes.Gray;
                }
                //包含数字、字母、符号3项组合
                else 
                {
                    bd_Low.Background = Brushes.Gray;
                    bd_Centre.Background = Brushes.Gray;
                    bd_High.Background = Brushes.Orange;
                }
            }
        }
        //1.5确认密码失去焦点
        private void PB_SurePassword_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //确认密码与新密码不等。
            if (PB_SurePassword.Password != PB_NewPassword.Password)
            {     
                //提示，并清空确认密码           
                txtAs.Visibility = Visibility.Visible;
                PB_SurePassword.Password = "";
            }
            else
            {
                //关闭提示
                txtAs.Visibility = Visibility.Collapsed;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PB_NewPassword.Password == PB_SurePassword.Password)
                {
                    string stroperator_password = PB_NewPassword.Password;
                    int intoperator_id = LoginWindow.intOperatorID;
                    int count = myUS_UpdatePasswordClient.UpdatePassword_Loaded_UpdatePassword(stroperator_password, intoperator_id);
                    if (count > 0)
                    {
                        myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 64, "修改【" + tbAccount.Text + "】账号密码", DateTime.Now);

                        MessageBoxResult dr = MessageBox.Show("密码修改成功,是否登录？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
                        if (dr == MessageBoxResult.OK)//如果点了确定按钮
                        {
                            //登录窗口
                            LoginWindow myLoginWindow = new LoginWindow();
                            myLoginWindow.Show();
                            //通过当前控件获取父级窗体并关闭=
                            Window parentWindow = Window.GetWindow(this);
                            parentWindow.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("修改密码失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                string ex1 = string.Empty;
                // 获取描述当前异常的消息。
                if (ex.InnerException != null) ex1 = ex.InnerException.Message;
                else
                {
                    ex1 = ex.Message;
                    ex1 = ex1.Split('。')[0];
                }
                MessageBox.Show(ex1, "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
