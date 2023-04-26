using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TTS_2019.Tools.Controls;
using TTS_2019.View;
using TTS_2019.View.LineManage;
using TTS_2019.View.SystemInformation;
using TTS_2019.View.TicketTask;
using TTS_2019.View.TrainOrder;

namespace TTS_2019
{
    /// <summary>
    /// MainWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow1 : Window
    {
        /// <summary>
        /// 可操作模块ID字符串
        /// </summary>
        string strModularDetailIds;
        /// <summary>
        /// 声明变量（用来记录左边菜单状态）
        /// </summary>
        bool blShow = true;//默认:显示

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strDepartment">部门</param>
        /// <param name="strStaffName">员工姓名</param>
        /// <param name="strMDI">可操作性模块ID字符串</param>
        public MainWindow1(string strDepartment, string strStaffName, string strMDI)
        {
            InitializeComponent();

            strModularDetailIds = strMDI.ToString().Trim();//获取菜单拼接的ID(权限获取)         
            this.NavigationTree.ItemsSource = GetTrees(0, GetProject(blShow));   //数据树形递归绑定
        }
        /// <summary>
        /// 实例化服务
        /// </summary>
        BLL.Login.LoginClient myLoginClient = new BLL.Login.LoginClient();
        BLL.UC_MenuManagement.UC_MenuManagementClient myClient = new BLL.UC_MenuManagement.UC_MenuManagementClient();
        #region 树形菜单数据绑定
        /// <summary>
        /// 自定义公共项目模块类
        /// </summary>
        public class ProjectModule
        {
            //添加列表实体节点字段
            public List<ProjectModule> Nodes { get; set; }
            //添加id字段
            public int id { get; set; }//id
            //添加项目名称字段
            public string ProjectName { get; set; }//菜单名称
            //添加项目ToolTip
            public string ProjectToolTip { get; set; }//提示ToolTip
            //添加图片logo字段
            public string imagePath { get; set; }//图片logo
            //添加父类id字段
            public int ParentId { get; set; }//父类id
            //自定义项目模块构造
            public ProjectModule()
            {
                //创建当前节点
                this.Nodes = new List<ProjectModule>();
                //当前父级ID为零
                this.ParentId = 0;//主节点的父id默认为0
            }
        }

        /// <summary>
        /// 获取项目列表--自定义数据
        /// </summary>
        /// <returns></returns>
        public List<ProjectModule> GetProject(bool getblShow)
        {        
            //实例化
            List<ProjectModule> listPM = new List<ProjectModule>();//列表
            #region 方法一（根据数据库生成菜单）
            //权限获取系统模块
            DataTable dt = myLoginClient.Main1_SelectAllModular(strModularDetailIds).Tables[0];          

            //循环差集生成菜单
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ProjectModule addProjectModule = new ProjectModule();//实例化实体
                //模块ID
                addProjectModule.id = Convert.ToInt32(dt.Rows[i]["modular_id"]);
                //模块名称
                if (getblShow == true)
                {
                    //显示名称（菜单正常时）
                    addProjectModule.ProjectName = dt.Rows[i]["name"].ToString().Trim();
                }
                else
                {
                    //ToolTip提示（菜单缩小时）
                    addProjectModule.ProjectToolTip = dt.Rows[i]["name"].ToString().Trim();
                }

                #region 图标生成
                //方法一：固定模块logo

                if (Convert.ToInt32(dt.Rows[i]["f_id"]) == 0)
                {
                    addProjectModule.imagePath = "/Images/Main1/1.png";
                }
                else
                {
                    addProjectModule.imagePath = "/Images/Main1/2.png";
                }

                //方法二：根据数据库数据生成图片
                string myPictureByte = myClient.UserControl_Loaded_SelectPhoro(dt.Rows[i]["icon"].ToString().Trim());
                if (myPictureByte != null)
                {
                    //获取内存图片
                    addProjectModule.imagePath = myPictureByte;
                }
                #endregion
                //模块父ID
                addProjectModule.ParentId = Convert.ToInt32(dt.Rows[i]["f_id"]);
                //把实体添加到列表中
                listPM.Add(addProjectModule);
            }
            #endregion

            #region 方法二(直接固定)
            ////实例化一个列表
            //List<ProjectModule> dplst = new List<ProjectModule>(){

            //    //父模块ID_1
            //    new ProjectModule(){id=1,ProjectName="系统管理",imagePath="/Images/Main1/1.png",ParentId=0},
            //    //父模块ID_2
            //    new ProjectModule(){id=2,ProjectName="线路管理",imagePath="/Images/Main1/1.png",ParentId=0},
            //    //父模块ID_3
            //    new ProjectModule(){id=3,ProjectName="车次管理",imagePath="/Images/Main1/1.png",ParentId=0},
            //    //父模块ID_4
            //    new ProjectModule(){id=4,ProjectName="票务管理",imagePath="/Images/Main1/1.png",ParentId=0},
            //    //父模块ID_5
            //    new ProjectModule(){id=5,ProjectName="数据统计",imagePath="/Images/Main1/1.png",ParentId=0},
            //    //连接父模块ID_1
            //    new ProjectModule(){id=6,ProjectName="修改密码",imagePath="/Images/Main1/2.png",ParentId=1},
            //    new ProjectModule(){id=7,ProjectName="员工管理",imagePath="/Images/Main1/2.png",ParentId=1},
            //    new ProjectModule(){id=8,ProjectName="旅客管理",imagePath="/Images/Main1/2.png",ParentId=1},
            //    new ProjectModule(){id=9,ProjectName="员工账户管理",imagePath="/Images/Main1/2.png",ParentId=1},
            //    new ProjectModule(){id=10,ProjectName="权限管理",imagePath="/Images/Main1/2.png",ParentId=1},
            //    new ProjectModule(){id=11,ProjectName="系统操作日志",imagePath="/Images/Main1/2.png",ParentId=1},

            //    //连接父模块ID_2
            //    new ProjectModule(){id=12,ProjectName="站点管理",imagePath="/Images/Main1/2.png",ParentId=2},
            //    new ProjectModule(){id=13,ProjectName="生成线路",imagePath="/Images/Main1/2.png",ParentId=2},

            //    //连接父模块ID_3
            //    new ProjectModule(){id=14,ProjectName="车辆管理",imagePath="/Images/Main1/2.png",ParentId=3},
            //    new ProjectModule(){id=15,ProjectName="车次管理",imagePath="/Images/Main1/2.png",ParentId=3},

            //    //连接父模块ID_4
            //    new ProjectModule(){id=16,ProjectName="票价规则",imagePath="/Images/Main1/2.png",ParentId=4},
            //    new ProjectModule(){id=17,ProjectName="车票分配",imagePath="/Images/Main1/2.png",ParentId=4},
            //    new ProjectModule(){id=18,ProjectName="车票生成",imagePath="/Images/Main1/2.png",ParentId=4},                

            //    //连接父模块ID_5
            //    new ProjectModule(){id=36,ProjectName="数据统计",imagePath="/Images/Main1/2.png",ParentId=5},
            //    new ProjectModule(){id=37,ProjectName="QQ邮箱",imagePath="/Images/Main1/2.png",ParentId=5},


            //};
            #endregion
            //返回菜单列表
            return listPM;
        }

        /// <summary>
        /// 递归生成树形数据
        /// </summary>
        /// <param name="delst"></param>
        /// <returns></returns>
        public List<ProjectModule> GetTrees(int parentid, List<ProjectModule> nodes)
        {
            //实例化一个主节点，并且进行linq条件查询，为tolist列表形式。
            List<ProjectModule> MainNodes = nodes.Where(x => x.ParentId == parentid).ToList<ProjectModule>();
            //实例化一个其他节点，并且进行linq条件查询，为tolist列表形式。
            List<ProjectModule> OtherNodes = nodes.Where(x => x.ParentId != parentid).ToList<ProjectModule>();
            //foreach将主节点的数据赋值给新创建的pro列表中
            foreach (ProjectModule pro in MainNodes)
            {
                //GetTrees递归赋值
                pro.Nodes = GetTrees(pro.id, OtherNodes);
            }
            //返回主节点，跳出递归
            return MainNodes;
        }
        #endregion

        #region 选项卡封装
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

        //1.0 页面加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //隐藏还原按钮
            this.btn_Normal.Visibility = Visibility.Collapsed;
            //最大化
            btn_Max_Click(null, null);

            //获取Main页面的TabControl
            TC = tab_Main;
            //实例化用户控件
            UC_FirstPage myFirstPage = new UC_FirstPage();
            //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
            AddItem(sender, "首页", myFirstPage);
        }

        #region 页面嵌套
        //公共静态字符串成员
        public static string Txtvalue;
        //页面嵌套
        private void SelectionModule_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //----基础管理（BasicManagement）路径：/Views/BasicManagement----
            if (Txtvalue == "修改密码")
            {
                //实例化用户控件
                UC_UpdatePassword myUpdatePassword = new UC_UpdatePassword();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myUpdatePassword);
            }
            if (Txtvalue == "员工管理")
            {
                //实例化用户控件
                UC_StaffInformation myStaffInformation = new UC_StaffInformation();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myStaffInformation);
            }
            if (Txtvalue == "员工账户管理")
            {
                //实例化用户控件
                UC_StaffAccountManage myAccountManage = new UC_StaffAccountManage();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myAccountManage);
            }
            if (Txtvalue == "权限管理")
            {
                UC_PowerManage myPowerManage = new UC_PowerManage();
                AddItem(sender, Txtvalue, myPowerManage);
            }
            if (Txtvalue == "系统操作日志")
            {

                ////加载层
                //LoadingLayer loadingLayer = new LoadingLayer();
                //loadingLayer.Show();
                ////多线程延迟Thread.Sleep系统挂起1秒
                //Thread t = new Thread(o => Thread.Sleep(1000));
                //t.Start(this);
                //while (t.IsAlive)
                //{
                //    //防止UI假死
                //    //System.Windows.Forms:引用
                //    System.Windows.Forms.Application.DoEvents();
                //}
                //loadingLayer.Hide();
                UC_SystemOperateLog mySystemOperateLog = new UC_SystemOperateLog();
                AddItem(sender, Txtvalue, mySystemOperateLog);
            }
            if (Txtvalue == "数据统计")
            {
                //实例化用户控件
                UC_FirstPage myFirstPage = new UC_FirstPage();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myFirstPage);
            }
            if (Txtvalue == "旅客管理")
            {
                //实例化用户控件
                UC_TravellerInformation myTravellerInformation = new UC_TravellerInformation();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myTravellerInformation);
            }
            if (Txtvalue == "菜单管理")
            {
                //实例化用户控件
                UC_MenuManagement myMenuManagement = new UC_MenuManagement();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myMenuManagement);
            }
            if (Txtvalue == "站点管理")
            {
                //实例化用户控件
                UC_StationManage myStationManage = new UC_StationManage();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myStationManage);
            }
            //----销售管理（SalesManagement）路径：/View/SalesManagement----
            if (Txtvalue == "生成线路")
            {
                //实例化用户控件
                UC_CreateLines myCreateLine = new UC_CreateLines();
                //UC_CreateLine myCreateLine = new UC_CreateLine();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myCreateLine);
            }
            if (Txtvalue == "车辆管理")
            {
                //实例化用户控件
                UC_Compartment myCompartment = new UC_Compartment();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myCompartment);
            }


            if (Txtvalue == "车次管理")
            {
                //实例化用户控件
                UC_CarOrder myCarOrder = new UC_CarOrder();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myCarOrder);
            }
            if (Txtvalue == "车票生成")
            {
                //实例化用户控件
                UC_Ticket myTicket = new UC_Ticket();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myTicket);
            }
            if (Txtvalue == "车票分配")
            {
                //实例化用户控件
                UC_TicketDetails myTicketDetails = new UC_TicketDetails();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myTicketDetails);
            }
            if (Txtvalue == "票价规则")
            {
                //实例化用户控件
                UC_TicketPrice myTicketPrice = new UC_TicketPrice();
                //调用添加选项卡的方法（当前控件，选项卡名称，用户控件）
                AddItem(sender, Txtvalue, myTicketPrice);
            }

        }
        /// <summary>
        /// 菜单名称鼠标左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //获取当前TextBlock
                TextBlock TextBlocks = sender as TextBlock;
                string value = "";
                if (TextBlocks == null)
                {
                    //点击的是图片
                    //获取图片ToolTip
                    Image img = sender as Image;
                    value = img.ToolTip.ToString();//提示
                }
                else
                {
                    //TextBlocks.Text值赋值给value
                    value = TextBlocks.Text;//文本
                }


                //value值赋值给公共静态字符串成员
                Txtvalue = value;
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion

        #region 窗口（放大、缩小、还原）
        /// <summary>
        /// 菜单隐藏和显示 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgMenuShow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            blShow = !blShow;//状态切换     

            if (blShow == false)
            {
                //隐藏：比例1:12

                //使用*号布局，当值为1的时候，表示*  值为10的时候，表示12*  就是说，第二列的宽度是第一列宽度的10倍
                cd_Menu.Width = new GridLength(1, GridUnitType.Star);
                cd_Content.Width = new GridLength(15, GridUnitType.Star);
            }
            else
            {
                //隐藏：比例1:5
                //使用*号布局，当值为1的时候，表示1*  值为5的时候，表示5*  
                cd_Menu.Width = new GridLength(1, GridUnitType.Star);
                cd_Content.Width = new GridLength(5, GridUnitType.Star);
            }
            //数据树形递归绑定
            this.NavigationTree.ItemsSource = GetTrees(0, GetProject(blShow));
        }
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
        /// 最大化
        /// </summary>
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
        /// 还原
        /// </summary>
        private void btn_Normal_Click(object sender, RoutedEventArgs e)
        {

            this.Left = rcnormal.Left;
            this.Top = rcnormal.Top;
            this.Width = rcnormal.Width;
            this.Height = rcnormal.Height;
            //控制最大化、还原按钮的显示和隐藏
            this.btn_Max.Visibility = Visibility.Visible;
            this.btn_Normal.Visibility = Visibility.Collapsed;
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
        #endregion
    }
}
