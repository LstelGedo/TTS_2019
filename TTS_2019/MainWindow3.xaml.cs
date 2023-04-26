using System;
using System.Collections.Generic;
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
using TTS_2019.Tools.Controls;
using TTS_2019.View;
using TTS_2019.View.LineManage;
using TTS_2019.View.SystemInformation;
using TTS_2019.View.TicketTask;
using TTS_2019.View.TrainOrder;

namespace TTS_2019
{
    /// <summary>
    /// MainWindow3.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow3 : Window
    {
        public MainWindow3()
        {
            InitializeComponent();
        }       
       
        /// <summary>
        /// 1.0 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //最大化窗口
            Img_Max_MouseLeftButtonDown(null, null);          
            //获取Main页面的TabControl
            TC = tab_Main;
            //实例化用户控件
            UC_FirstPage myFirstPage = new UC_FirstPage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "数据统计", myFirstPage);
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_Min_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            wd_Main.WindowState = WindowState.Minimized;
        }
        Rect rcnormal;//定义一个全局rect记录还原状态下窗口的位置和大小。
        /// <summary>
        /// 缩小全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_Normal_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;
            this.Img_Max.Visibility = Visibility.Visible;
            this.Img_Normal.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_Max_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Img_Max.Visibility = Visibility.Collapsed;
            this.Img_Normal.Visibility = Visibility.Visible;
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
        private void Img_Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("你确定要退出吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        #region 菜单按钮(页面嵌套)
        #region 选项卡方法封装
        /// <summary>
        /// 添加选项卡
        /// </summary>
        /// <returns></returns>
        static TabControl TC;//静态选项卡
        public static void AddItem(object sender, string Trname, UserControl UC)
        {
            //是否存在当前选项
            bool bolWhetherBe = false;
            if (TC.Items.Count > 0)
            {
                for (int i = 0; i < TC.Items.Count; i++)
                {
                    //判断是否存在
                    if (((UCTabItemWithClose)TC.Items[i]).Name == Trname)
                    {
                        //选中子选项
                        TC.SelectedItem = ((UCTabItemWithClose)TC.Items[i]);
                        bolWhetherBe = true;
                        break;
                    }
                }
                if (bolWhetherBe == false)
                {
                    //创建子选项
                    UCTabItemWithClose item = new UCTabItemWithClose();
                    //名称
                    item.Name = Trname;
                    //标头名称
                    item.Header = string.Format(Trname);
                    //子选择里面的内容
                    item.Content = UC;
                    item.Margin = new Thickness(0, 0, 1, 0);
                    item.Height = 28;
                    //添加子选项
                    TC.Items.Add(item);
                    //选中子选项
                    TC.SelectedItem = item;
                }
            }
            else
            {
                UCTabItemWithClose item = new UCTabItemWithClose();//创建子选项
                item.Name = Trname;//名称
                item.Header = string.Format(Trname);//标头名称
                item.Content = UC;//子选择里面的内容           
                item.Margin = new Thickness(0, 0, 1, 0);
                item.Height = 28;
                TC.Items.Add(item);//添加子选项
                TC.SelectedItem = item;//选中子选项
            }
        }
        #endregion
        private void btnUpdatePassword_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_UpdatePassword myUpdatePassword = new UC_UpdatePassword();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "修改密码", myUpdatePassword);
        }

        private void btnStaffInformation_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StaffInformation myStaffInformation = new UC_StaffInformation();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "员工管理", myStaffInformation);
        }

        private void btnTravellerInformation_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TravellerInformation myTravellerInformation = new UC_TravellerInformation();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "旅客管理", myTravellerInformation);
        }

        private void btnStaffAccountManage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StaffAccountManage myAccountManage = new UC_StaffAccountManage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "员工账户管理", myAccountManage);
        }

        private void btnPowerManage_Click(object sender, RoutedEventArgs e)
        {
            UC_PowerManage myPowerManage = new UC_PowerManage();
            AddItem(sender, "权限管理", myPowerManage);
        }

        private void btnSystemOperateLog_Click(object sender, RoutedEventArgs e)
        {
            UC_SystemOperateLog mySystemOperateLog = new UC_SystemOperateLog();
            AddItem(sender, "系统操作日志", mySystemOperateLog);
        }

        private void btnStationManage_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_StationManage myStationManage = new UC_StationManage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "站点管理", myStationManage);
        }

        private void btnCreateLine_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_CreateLines myCreateLine = new UC_CreateLines();          
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "生成线路", myCreateLine);
        }

        private void btnCompartment_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_Compartment myCompartment = new UC_Compartment();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车辆管理", myCompartment);
        }

        private void btnCarOrder_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_CarOrder myCarOrder = new UC_CarOrder();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车次管理", myCarOrder);
        }

        private void btnTicketPrice_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TicketPrice myTicketPrice = new UC_TicketPrice();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "票价规则", myTicketPrice);
        }

        private void btnTicketDetails_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_TicketDetails myTicketDetails = new UC_TicketDetails();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车票分配", myTicketDetails);
        }

        private void btnTicket_Click(object sender, RoutedEventArgs e)
        {
            //实例化用户控件
            UC_Ticket myTicket = new UC_Ticket();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "车票生成", myTicket);
        }

        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {

            //实例化用户控件
            UC_FirstPage myFirstPage = new UC_FirstPage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "数据统计", myFirstPage);
        }
        #endregion
    }
}
