using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace TTS_2019
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        //实例化服务
        BLL.Login.LoginClient myLoginClient = new BLL.Login.LoginClient();
        public static int intStaffID = 0;
        public static string strAccounts;
        public static int intOperatorID;
        public static string StaticStrModularDetailIds;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 最大化 
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rc.Width;
            this.Height = rc.Height;
            #endregion



            //记住账号密码
            if (GetSettingString("isRemember") == "true")
            {
                loginCheckBoxUne.IsChecked = true;
                txtAccount.Text = GetSettingString("UserName");
                pwdPassword.Password = GetSettingString("UserPassword");
            }
            else
            {
                loginCheckBoxUne.IsChecked = false;
            }
            //自动登录
            if (GetSettingString("isLogin") == "true")
            {
                btnLogin_Click(null, null);
            }
            else
            {
                loginCheckBoxIs.IsChecked = false;
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //获取控件数值
                string strAccount = txtAccount.Text.ToString().Trim();
                string strPassword = pwdPassword.Password.ToString().Trim();
                DataTable dt = myLoginClient.frmLogin_btn_Login_Click_SelectLogin(strAccount, strPassword).Tables[0];
                //判断表格有没有数据
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(dt.Rows[0]["effective"]) == true)
                    {
                        

                        //从返回的表格里面取出部门和员工姓名
                        string strDepartment = dt.Rows[0]["branch"].ToString();
                        string strStaffName = dt.Rows[0]["staff_name"].ToString();
                        //根据拼接的ID获取操作菜单(权限应用)
                        string strModularDetailIds = dt.Rows[0]["modular_detail_id"].ToString();

                        #region 公共静态变量赋值
                        StaticStrModularDetailIds = strModularDetailIds;
                        intStaffID = Convert.ToInt32(dt.Rows[0]["staff_id"]);
                        //记录信息（修改密码的时候调用）
                        strAccounts = dt.Rows[0]["operator_accounts"].ToString().Trim();
                        intOperatorID = Convert.ToInt32(dt.Rows[0]["operator_id"]);
                        #endregion

                        //跳转到Main窗体
                        //MainWindow myMain = new MainWindow(strDepartment, strStaffName, strModularDetailIds);
                        //myMain.Show();

                        MainWindow1 myMain1 = new MainWindow1(strDepartment, strStaffName, strModularDetailIds);
                        myMain1.Show();

                        //MainWindow2 myMain2 = new MainWindow2(strDepartment, strStaffName, strModularDetailIds);
                        //myMain2.Show();

                        //MainWindow3 myMain3 = new MainWindow3();
                        //myMain3.Show();

                        //MainWindow4 myMain4 = new MainWindow4(strDepartment, strStaffName, strModularDetailIds);
                        //myMain4.Show();
                        //关闭当前窗体
                        this.Close();                        
                    }
                    else if (Convert.ToBoolean(dt.Rows[0]["effective"]) == false)
                    {
                        MessageBox.Show("该账号已被停用！", "提示！", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("请检查账号和密码是否输入错误！", "提示！", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                //设置页面透明
                dpLogin.Opacity = 0;
                string ex1 = string.Empty;
                // 获取描述当前异常的消息。
                if (ex.InnerException != null)
                {
                    ex1 = ex.InnerException.Message;
                }
                else
                {
                    ex1 = ex.Message;
                    ex1 = ex1.Split('。')[0];
                }
                if (ex1 == "无法连接到远程服务器")
                {
                    MessageBox.Show("服务器处于关闭状态，请启动！", "提 示",MessageBoxButton.OK,MessageBoxImage.Error);
                }
                else if (ex1 == "在与 SQL Server 建立连接时出现与网络相关的或特定于实例的错误")
                {
                    MessageBox.Show("服务端SQL服务处于关闭状态！", "提 示", MessageBoxButton.OK, MessageBoxImage.Error);                   
                }
                else
                {
                    MessageBox.Show("您断网了，或服务器处于关闭状态！", "提 示", MessageBoxButton.OK, MessageBoxImage.Error);                    
                }
                Environment.Exit(0);  //中断程序
            }
            //记住用户名和密码
            if (Convert.ToBoolean(loginCheckBoxUne.IsChecked))
            {
                UpdateSettingString("UserName", txtAccount.Text);
                UpdateSettingString("UserPassword", pwdPassword.Password);
                UpdateSettingString("isRemember", "true");
                UpdateSettingString("isLogin", "");
                if (Convert.ToBoolean(loginCheckBoxIs.IsChecked))
                {
                    UpdateSettingString("isLogin", "true");
                }                
                
            }           
            else
            {
                UpdateSettingString("UserName", "");
                UpdateSettingString("UserPassword", "");
                UpdateSettingString("isRemember", "");              
                UpdateSettingString("isLogin", "");                
            }

        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("是否要退出系统？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            //弹出确定对话框
            if (dr == MessageBoxResult.OK) //如果点了确定按钮
            {
                //关闭应用程序
                Application.Current.Shutdown();
            }               
        }


        /// <summary>
        /// 读取客户设置
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static string GetSettingString(string settingName)
        {
            try
            {
                string settingString = ConfigurationManager.AppSettings[settingName].ToString();
                return settingString;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 更新设置
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="valueName"></param>
        public static void UpdateSettingString(string settingName, string valueName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (ConfigurationManager.AppSettings[settingName] != null)
            {
                config.AppSettings.Settings.Remove(settingName);
            }
            config.AppSettings.Settings.Add(settingName, valueName);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}
