using System;
using System.Collections;
using System.Data;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TTS_2019.Tools.Controls;
using TTS_2019.View;
using TTS_2019.View.LineManage;
using TTS_2019.View.SystemInformation;
using TTS_2019.View.TicketTask;
using TTS_2019.View.TrainOrder;

namespace TTS_2019
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer showtimer;  
        static TabControl TC;// 静态选项卡
        string strModularDetailIds;
        //实例化服务
        BLL.Login.LoginClient myLoginClient = new BLL.Login.LoginClient();
        public MainWindow(string strDepartment, string strStaffName, string strMDI)
        {
            InitializeComponent();
            strModularDetailIds = strMDI.ToString().Trim();//获取菜单拼接的ID(权限获取)
            TbDepartment.Text = strDepartment + ":";
            TbEmployee.Text = strStaffName;
            #region 显示时间
            showtimer = new DispatcherTimer();//实例化
            showtimer.Tick += new EventHandler(ShowCurTimer);
            showtimer.Interval = new TimeSpan(0, 0, 0, 1, 0);//控制时间在一秒钟跳动一次
            showtimer.Start();//开启时间
            #endregion           
        }
        #region 系统时间
        public void ShowCurTimer(object sender, EventArgs e)
        {
            txt_Time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//确定时间格式
        }
        #endregion
        #region 选项卡      

        public static void AddItem(object sender, string trname, UserControl uc)
        {
            bool bolWhetherBe = false;//是否存在当前选项
            //1、判断当前选项卡的个数是否大于0
            if (TC.Items.Count > 0)
            {
                for (int i = 0; i < TC.Items.Count; i++)
                {
                    if (((UCTabItemWithClose)TC.Items[i]).Name == trname)//判断是否存在
                    {
                        TC.SelectedItem = ((UCTabItemWithClose)TC.Items[i]);//选中子选项
                        bolWhetherBe = true;
                        break;
                    }
                }
                //2、是否存在当前选项
                if (bolWhetherBe == false)
                {
                    //3、创建子选项
                    UCTabItemWithClose item = new UCTabItemWithClose();//创建子选项
                    item.Name = trname;//名称
                    item.Header = string.Format(trname);//标头名称
                    item.Content = uc;//子选择里面的内容                  
                    item.Margin = new Thickness(0, 0, 1, 0);
                    item.Height = 28;
                    TC.Items.Add(item);//添加子选项
                    TC.SelectedItem = item;//选中子选项

                }
            }
            else
            {
                UCTabItemWithClose item = new UCTabItemWithClose();//创建子选项
                item.Name = trname;//名称
                item.Header = string.Format(trname);//标头名称
                item.Content = uc;//子选择里面的内容           
                item.Margin = new Thickness(0, 0, 1, 0);
                item.Height = 28;
                TC.Items.Add(item);//添加子选项
                TC.SelectedItem = item;//选中子选项
            }

        }
        #endregion
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {          

            TC = tab_Main;
            //实例化用户控件
            UC_FirstPage myFirstPage = new UC_FirstPage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "数据统计", myFirstPage);
            //权限应用
            LimitsOfPower();

            //#region 语音提示
            //string str = TbEmployee.Text;
            ////SpeechSynthesizer文字转音频
            ////项目添加引用：System.Speech
            //SpeechSynthesizer speech = new SpeechSynthesizer();
            //speech.Rate = 0;  //语速:对象的语速，在 -10 到 10 之间。
            //speech.Volume = 100;  //音量:（0 到 100）。
            //speech.Speak(str+"欢迎使用火车后台管理系统");            
            //#endregion
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_UpdatePassword myUpdatePassword = new UC_UpdatePassword();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender,"修改密码", myUpdatePassword);
        }
        /// <summary>
        /// 员工管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStaffInformation_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StaffInformation myStaffInformation = new UC_StaffInformation();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "员工管理", myStaffInformation);
        }
        /// <summary>
        /// 账户管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStaffAccountManage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StaffAccountManage myAccountManage = new UC_StaffAccountManage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "账户管理", myAccountManage);
        }
        /// <summary>
        /// 权限模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPowerManage_Click(object sender, RoutedEventArgs e)
        {
            UC_PowerManage myPowerManage = new UC_PowerManage();
            AddItem(sender, "权限管理", myPowerManage);
        }
        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSystemOperateLog_Click(object sender, RoutedEventArgs e)
        {
            UC_SystemOperateLog mySystemOperateLog = new UC_SystemOperateLog();
            AddItem(sender, "系统日志", mySystemOperateLog);
        }
       /// <summary>
       /// 数据统计
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_FirstPage myFirstPage = new UC_FirstPage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "数据统计", myFirstPage);
        }
        //按钮（旅客管理）
        private void btnTravellerInformation_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TravellerInformation myTravellerInformation = new UC_TravellerInformation();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "旅客管理", myTravellerInformation);
        }
        //按钮（站点管理）
        private void btnStationManage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StationManage myStationManage = new UC_StationManage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "站点管理", myStationManage);
        }
        //按钮（生成线路）
        private void btnCreateLine_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_CreateLine myCreateLine = new UC_CreateLine();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "生成线路", myCreateLine);
        }
        //按钮（车辆管理）
        private void btnCompartment_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_Compartment myCompartment = new UC_Compartment();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车辆管理", myCompartment);
        }
        //按钮（车次管理）
        private void btnCarOrder_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_CarOrder myCarOrder = new UC_CarOrder();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车次管理", myCarOrder);
        }
        //按钮（车票生成）
        private void btnTicket_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_Ticket myTicket = new UC_Ticket();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车票生成", myTicket);
        }
        //按钮（车票分配）
        private void btnTicketDetails_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TicketDetails myTicketDetails = new UC_TicketDetails();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车票分配", myTicketDetails);
        }
        //按钮（票价规则）
        private void btnTicketPrice_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TicketPrice myTicketPrice = new UC_TicketPrice();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "票价规则", myTicketPrice);
        }
        #region 权限应用(菜单的显示和隐藏)
        private void LimitsOfPower()
        {
            try
            {
                //显示的菜单
                DataTable dtYes = myLoginClient.frmLogin_btn_Login_Click_SelectModular(strModularDetailIds).Tables[0];
                //不能显示的菜单
                DataTable dtNo = myLoginClient.frmLogin_btn_Login_Click_Select_NotInModular(strModularDetailIds).Tables[0];

                for (int i = 0; i < dtNo.Rows.Count; i++)
                {
                    //菜单隐藏 
                    Button btn = SecondButton(dtNo.Rows[i]["code"].ToString().Trim());
                    if (btn != null)
                    {
                        btn.Visibility = Visibility.Collapsed;
                    }

                }
                for (int i = 0; i < dtYes.Rows.Count; i++)
                {
                    //菜单显示
                    Button btn = SecondButton(dtYes.Rows[i]["code"].ToString().Trim());
                    if (btn != null)
                    {
                        btn.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception)
            {

                return;
            }

        }
        #endregion
        #region 权限封装
       
        private Hashtable _Sfz = new Hashtable();        
        /// <summary>
        /// 二级菜单
        /// </summary>
        private void SecondMenu()
        {
            _Sfz.Add("btnUpdatePassword", btnUpdatePassword);
            _Sfz.Add("btnStaffInformation", btnStaffInformation);
            _Sfz.Add("btnTravellerInformation", btnTravellerInformation);
            _Sfz.Add("btnStaffAccountManage", btnStaffAccountManage);
            _Sfz.Add("btnPowerManage", btnPowerManage);
            _Sfz.Add("btnSystemOperateLog", btnSystemOperateLog);
            _Sfz.Add("btnCreateLine", btnCreateLine);
            _Sfz.Add("btnStationManage", btnStationManage);
            _Sfz.Add("btnCompartment", btnCompartment);
            _Sfz.Add("btnCarOrder", btnCarOrder);
            _Sfz.Add("btnTicket", btnTicket);
            _Sfz.Add("btnTicketDetails", btnTicketDetails);
            _Sfz.Add("btnTicketPrice", btnTicketPrice);            
        }
        /// <summary>
        /// 根据编号获得二级菜单按钮
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public Button SecondButton(string strName)
        {
            if (strName.Length != 0)
            {
                if (_Sfz.Count == 0) SecondMenu();
                Button btn = _Sfz[strName] as Button;//根据键提取对应的值转化为矢量按钮
                if (btn == null) return null;
                return btn;//返回按钮
            }
            return null;
        }
        #endregion

        #region 发送邮件
        /// <summary>  
        /// 发送邮件,成功返回true,否则false  
        /// </summary>  
        /// <param name="to">收件人</param>  
        /// <param name="body">内容</param>  
        /// <param name="title">标题</param>  
        /// <param name="whichEmail">是否join</param>  
        /// <param name="path">附件</param>  
        /// <param name="Fname">姓名</param>  
        /// <returns>结果</returns>  
        public static bool SentMailHXD(string to, string body, string title, string whichEmail, string path, string Fname)
        {
            bool retrunBool = false;
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            string strFromEmail = "";//你的邮箱  
            string strEmailPassword = "";//QQPOP3/SMTP服务码  
            try
            {
                mail.From = new MailAddress("" + Fname + "<" + strFromEmail + ">");
                mail.To.Add(new MailAddress(to));
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Priority = MailPriority.Normal;
                mail.Body = body;
                mail.Subject = title;
                smtp.Host = "smtp.qq.com";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new System.Net.NetworkCredential(strFromEmail, strEmailPassword);
                //发送邮件  
                smtp.Send(mail);   //同步发送  
                retrunBool = true;
            }
            catch (Exception ex)
            {
                retrunBool = false;
            }
            // smtp.SendAsync(mail, mail.To); //异步发送 （异步发送时页面上要加上Async="true" ）  
            return retrunBool;
        }
        //方法二
        public static void QQfs()
        {
            try
            {
                //发送人邮箱
                MailAddress from = new MailAddress("crazysoftwaree@163.com");//crazysoftwaree@163.com
                //实例化MailMessage
                MailMessage message = new MailMessage();
                //内容
                message.Body = "1234567890";
                //是否html
                message.IsBodyHtml = true;
                //内容UTF-8编码
                message.BodyEncoding = System.Text.Encoding.UTF8;
                //收件人邮箱
                message.To.Add("1234567@qq.com");
                //标题
                message.Subject = "广信IT项目";
                //标题UTF-8编码
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                //获取或设置来自此电子邮件地址
                message.From = from;
                SmtpClient client = new SmtpClient();
                //使用安全套接字层 (SSL) 加密的连接
                client.EnableSsl = true;
                //邮箱-  SMTP解释：邮箱协议  SSL SMTP解释：安全邮箱协议
                client.Host = "smtp.163.com";
                //端口
                client.Port = 25;
                //邮箱用户名和邮箱授权码 -- POP3/SMTP服务
                client.Credentials = new System.Net.NetworkCredential("crazysoftwaree", "chenhuazhou123");               
                //消息发送到 SMTP 服务器以进行传递
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            //SentMailHXD("收件人", "内容, "标题", "抄送", "附件（附件方法我移除了）", "你的姓名");
            QQfs();
        }

       
    }
}
