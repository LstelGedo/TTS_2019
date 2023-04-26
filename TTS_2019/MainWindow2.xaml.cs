using System;
using System.Collections;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TTS_2019.Tools.Controls;
using TTS_2019.View;
using TTS_2019.View.DataStatistics;
using TTS_2019.View.LineManage;
using TTS_2019.View.SystemInformation;
using TTS_2019.View.TicketTask;
using TTS_2019.View.TrainOrder;

namespace TTS_2019
{
    /// <summary>
    /// MainWindow2.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow2 : Window
    {
        private DispatcherTimer showtimer;
        static TabControl TC;// 静态选项卡
        string strModularDetailIds;
        UserControl UC = null;//容器要嵌套的界面
        int intDownTag = 0;// 当前选择的按钮的tag         
        BLL.Login.LoginClient myLoginClient = new BLL.Login.LoginClient();//实例化服务
        public MainWindow2(string strDepartment, string strStaffName, string strMDI)
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
        //系统时间
        public void ShowCurTimer(object sender, EventArgs e)
        {
            txt_Time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//确定时间格式
        }
        //*加载 *
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //隐藏还原按钮
            this.btn_Normal.Visibility = Visibility.Collapsed;
            //最大化
            btn_Max_Click(null, null);

            TC = tab_Main;
            //实例化用户控件
            UC_FirstPage myFirstPage = new UC_FirstPage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "数据统计", myFirstPage);
            //权限应用
            LimitsOfPower();
            //绑定头像
            GetImage();
        }        
        #region 绑定头像
        private void GetImage()
        {        

            DataTable dt = myLoginClient.frmLogin_btn_Login_Click_Select_Picture(LoginWindow.intStaffID).Tables[0];
            //（1）获取图片
            string strLuJing = (dt.Rows[0]["picture"]).ToString();
            //（2）查询图片路径
            BLL.UC_StaffInformation.UC_StaffInformationClient myStaff = new BLL.UC_StaffInformation.UC_StaffInformationClient();
            string  myPictureByte = myStaff.UserControl_Loaded_SelectPhoro(strLuJing);
            if (myPictureByte != null)
            {
                //提供使用 可扩展应用程序标记语言 (XAML)，用于加载的图像优化的专用
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                //增加这一行（指定位图图像如何利用内存缓存=在加载时将整个图像缓存到内存中。对图像数据的所有请求将通过内存存储区进行填充。）                
                bi.CacheOption = BitmapCacheOption.OnLoad;
                //获取或设置 System.Windows.Media.Imaging.BitmapImage的 System.Uri 源。默认值为 null。
                bi.UriSource = new Uri(myPictureByte);
                bi.EndInit();
                //获取内存图片
                MainTopImg_png.Source = bi;
            }
            else
            {
                MainTopImg_png.Source = null;
            }

        }
        #endregion
        #region 添加选项卡        
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
        #region 左边一级菜单按妞（控制二级菜单栏的显示）
        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            //嵌套菜单
            //判断按钮是不是重复选择           
            if (intDownTag == Convert.ToInt32(((Button)sender).Tag))
            {

                if (((Button)sender) == btnSystemInformation)
                {
                    //控制【系统管理】菜单组件隐藏
                    btnSystemInformationBorder.Visibility = Visibility.Collapsed;
                }
                else if (((Button)sender) == btnTrainOrder)
                {
                    //控制【车次管理】菜单组件隐藏
                    btnTrainOrderBorder.Visibility = Visibility.Collapsed;
                }
                else if (((Button)sender) == btnLineManage)
                {
                    //控制【线路管理】菜单组件隐藏
                    btnLineManageBorder.Visibility = Visibility.Collapsed;
                }
                else if (((Button)sender) == btnTicketTask)
                {
                    //控制【票务管理】菜单组件隐藏
                    btnTicketTaskBorder.Visibility = Visibility.Collapsed;
                }
                else if (((Button)sender) == btnDataStatistics)
                {
                    //控制【数据统计】菜单组件隐藏
                    btnDataStatisticsBorder.Visibility = Visibility.Collapsed;
                }
                //调整左边菜单栏的显示和隐藏
                intDownTag = 0;
                return;
            }
            else if (((Button)sender) == btnSystemInformation)
            {
                //控制【系统管理】菜单组件显示
                btnSystemInformationBorder.Visibility = Visibility.Visible;
                //控制【车次管理】菜单组件隐藏
                btnTrainOrderBorder.Visibility = Visibility.Collapsed;
                //控制【线路管理】菜单组件隐藏
                btnLineManageBorder.Visibility = Visibility.Collapsed;
                //控制【票务管理】菜单组件隐藏
                btnTicketTaskBorder.Visibility = Visibility.Collapsed;
                //控制【数据统计】菜单组件隐藏
                btnDataStatisticsBorder.Visibility = Visibility.Collapsed;
            }

            else if (((Button)sender) == btnTrainOrder)
            {

                //控制【系统管理】菜单组件隐藏
                btnSystemInformationBorder.Visibility = Visibility.Collapsed;
                //控制【车次管理】菜单组件显示
                btnTrainOrderBorder.Visibility = Visibility.Visible;
                //控制【线路管理】菜单组件隐藏
                btnLineManageBorder.Visibility = Visibility.Collapsed;
                //控制【票务管理】菜单组件隐藏
                btnTicketTaskBorder.Visibility = Visibility.Collapsed;
                //控制【数据统计】菜单组件隐藏
                btnDataStatisticsBorder.Visibility = Visibility.Collapsed;
            }
            else if (((Button)sender) == btnLineManage)
            {
                //控制【系统管理】菜单组件隐藏
                btnSystemInformationBorder.Visibility = Visibility.Collapsed;
                //控制【车次管理】菜单组件隐藏
                btnTrainOrderBorder.Visibility = Visibility.Collapsed;
                //控制【线路管理】菜单组件显示
                btnLineManageBorder.Visibility = Visibility.Visible;
                //控制【票务管理】菜单组件隐藏
                btnTicketTaskBorder.Visibility = Visibility.Collapsed;
                //控制【数据统计】菜单组件隐藏
                btnDataStatisticsBorder.Visibility = Visibility.Collapsed;
            }
            else if (((Button)sender) == btnTicketTask)
            {
                //控制【系统管理】菜单组件隐藏
                btnSystemInformationBorder.Visibility = Visibility.Collapsed;
                //控制【车次管理】菜单组件隐藏
                btnTrainOrderBorder.Visibility = Visibility.Collapsed;
                //控制【线路管理】菜单组件隐藏
                btnLineManageBorder.Visibility = Visibility.Collapsed;
                //控制【票务管理】菜单组件显示
                btnTicketTaskBorder.Visibility = Visibility.Visible;
                //控制【数据统计】菜单组件隐藏
                btnDataStatisticsBorder.Visibility = Visibility.Collapsed;
            }
            else if (((Button)sender) == btnDataStatistics)
            {
                //控制【系统管理】菜单组件隐藏
                btnSystemInformationBorder.Visibility = Visibility.Collapsed;
                //控制【车次管理】菜单组件隐藏
                btnTrainOrderBorder.Visibility = Visibility.Collapsed;
                //控制【线路管理】菜单组件隐藏
                btnLineManageBorder.Visibility = Visibility.Collapsed;
                //控制【票务管理】菜单组件隐藏
                btnTicketTaskBorder.Visibility = Visibility.Collapsed;
                //控制【数据统计】菜单组件显示
                btnDataStatisticsBorder.Visibility = Visibility.Visible;
            }
            intDownTag = Convert.ToInt32(((Button)sender).Tag);//当前选择按钮的tag

        }
        #endregion
        #region 菜单按妞（点击嵌套页面）
        //按钮（修改密码）
        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_UpdatePassword myUpdatePassword = new UC_UpdatePassword();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnUpdatePassword.Content.ToString(), myUpdatePassword);
        }
        //按钮（员工管理）
        private void btnStaffInformation_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StaffInformation myStaffInformation = new UC_StaffInformation();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnStaffInformation.Content.ToString(), myStaffInformation);
        }
        //按钮（旅客管理）
        private void btnTravellerInformation_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
             UC_TravellerInformation myTravellerInformation = new UC_TravellerInformation();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnTravellerInformation.Content.ToString(), myTravellerInformation);
        }
        //按钮（员工账户管理）
        private void btnStaffAccountManage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StaffAccountManage myStaffAccountManage = new UC_StaffAccountManage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnStaffAccountManage.Content.ToString(), myStaffAccountManage);
        }
        //按钮（权限管理）
        private void btnPowerManage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_PowerManage myPowerManage = new UC_PowerManage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnPowerManage.Content.ToString(), myPowerManage);
        }
        //按钮（系统操作日志）
        private void btnSystemOperateLog_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_SystemOperateLog mySystemOperateLog = new UC_SystemOperateLog();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnSystemOperateLog.Content.ToString(), mySystemOperateLog);
        }
        //按钮（生成线路）
        private void btnCreateLine_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_CreateLine myCreateLine = new UC_CreateLine();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnCreateLine.Content.ToString(), myCreateLine);
        }
        //按钮（站点管理）
        private void btnStationManage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StationManage myStationManage = new UC_StationManage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnStationManage.Content.ToString(), myStationManage);
        }
        //按钮（车辆管理）
        private void btnCompartment_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_Compartment myCompartment = new UC_Compartment();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnCompartment.Content.ToString(), myCompartment);
        }
        //按钮（车次管理）
        private void btnCarOrder_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_CarOrder myCarOrder = new UC_CarOrder();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnCarOrder.Content.ToString(), myCarOrder);


        }
        //按钮（车票生成）
        private void btnTicket_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_Ticket myTicket = new UC_Ticket();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnTicket.Content.ToString(), myTicket);
        }
        //按钮（车票分配)
        private void btnTicketDetails_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TicketDetails myTicketDetails = new UC_TicketDetails();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnTicketDetails.Content.ToString(), myTicketDetails);
        }
        //按钮（票价规则)
        private void btnTicketPrice_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TicketPrice myTicketPrice = new UC_TicketPrice();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnTicketPrice.Content.ToString(), myTicketPrice);
        }
        //销售金额统计
        private void btnSaleStatistics_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_SaleStatistics mySaleStatistics = new UC_SaleStatistics();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnSaleStatistics.Content.ToString(), mySaleStatistics);
        }
        //按钮（各站旅客流量)
        private void btnSitePeopleStream_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_SitePeopleStream mySitePeopleStream = new UC_SitePeopleStream();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnSitePeopleStream.Content.ToString(), mySitePeopleStream);
        }
        //按钮（网上账户购票次数)
        private void btnWebShopTicketNumber_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_WebShopTicketNumber myWebShopTicketNumber = new UC_WebShopTicketNumber();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, btnWebShopTicketNumber.Content.ToString(), myWebShopTicketNumber);
        }
        #endregion
        #region 权限应用(菜单的显示和隐藏)
        private void LimitsOfPower()
        {
            //显示的菜单
            DataTable dtYes = myLoginClient.frmLogin_btn_Login_Click_SelectModular(strModularDetailIds).Tables[0];
            //不能显示的菜单
            DataTable dtNo = myLoginClient.frmLogin_btn_Login_Click_Select_NotInModular(strModularDetailIds).Tables[0];

            for (int i = 0; i < dtNo.Rows.Count; i++)
            {
                //一级菜单隐藏
                Button dtnFirst = FirstButton(dtNo.Rows[i]["f_id"].ToString().Trim());
                dtnFirst.Visibility = Visibility.Collapsed;
                //二级菜单隐藏 
                CalendarButton dtnSecond = SecondButton(dtNo.Rows[i]["code"].ToString().Trim());
                dtnSecond.Visibility = Visibility.Collapsed;
            }
            for (int i = 0; i < dtYes.Rows.Count; i++)
            {
                //一级菜单显示
                Button dtnFirst = FirstButton(dtYes.Rows[i]["f_id"].ToString().Trim());
                dtnFirst.Visibility = Visibility.Visible;
                //二级菜单显示
                CalendarButton dtnSecond = SecondButton(dtYes.Rows[i]["code"].ToString().Trim());
                dtnSecond.Visibility = Visibility.Visible;
            }

        }
        #endregion
        #region 权限封装
        private Hashtable _SfzFisrt = new Hashtable();
        private Hashtable _Sfz = new Hashtable();
        /// <summary>
        /// 一级菜单
        /// </summary>
        private void FirstMenu()
        {
            _SfzFisrt.Add("14", btnSystemInformation);
            _SfzFisrt.Add("15", btnLineManage);
            _SfzFisrt.Add("16", btnTrainOrder);
            _SfzFisrt.Add("17", btnTicketTask);
            _SfzFisrt.Add("18", btnDataStatistics);
        }
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
            _Sfz.Add("btnSaleStatistics", btnSaleStatistics);
            _Sfz.Add("btnSitePeopleStream", btnSitePeopleStream);
            _Sfz.Add("btnWebShopTicketNumber", btnWebShopTicketNumber);
            _Sfz.Add("btnFirstPage", btnFirstPage);
            _Sfz.Add("btnEmail", btnEmail);
            _Sfz.Add("btnMenuManagement", btnMenuManagement);
        }
        /// <summary>
        /// 根据编号获得二级菜单按钮
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public Button FirstButton(string strFid)
        {
            if (strFid.Length > 0)
            {
                if (_SfzFisrt.Count == 0) FirstMenu();
                Button btn = _SfzFisrt[strFid] as Button;//根据键提取对应的值转化为矢量按钮
                if (btn == null) return null;
                return btn;//返回按钮
            }
            return null;
        }
        /// <summary>
        /// 根据编号获得二级菜单按钮
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public CalendarButton SecondButton(string strName)
        {
            if (strName.Length != 0)
            {
                if (_Sfz.Count == 0) SecondMenu();
                CalendarButton btn = _Sfz[strName] as CalendarButton;//根据键提取对应的值转化为矢量按钮
                if (btn == null) return null;
                return btn;//返回按钮
            }
            return null;
        }


        #endregion
        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Min_Click(object sender, RoutedEventArgs e)
        {
            wd_Main.WindowState = WindowState.Minimized;
        }
        Rect rcnormal;//定义一个全局rect记录还原状态下窗口的位置和大小。
        /// <summary>
        /// 缩小全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Normal_Click(object sender, RoutedEventArgs e)
        {
            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;
            this.btn_Max.Visibility = Visibility.Visible;
            this.btn_Normal.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Max_Click(object sender, RoutedEventArgs e)
        {
            this.btn_Max.Visibility = Visibility.Collapsed;
            this.btn_Normal.Visibility = Visibility.Visible;
            rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rc = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rc.Width;
            this.Height = rc.Height;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("你确定要退出吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
