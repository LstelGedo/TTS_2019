using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.LineManage
{
    /// <summary>
    /// UC_CreateLines.xaml 的交互逻辑
    /// </summary>
    public partial class UC_CreateLines : UserControl
    {
        public UC_CreateLines()
        {
            InitializeComponent();
        }
        //实例化服务端
        BLL.UC_CreateLine.UC_CreateLineClient myClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        public int intLineID;//公共线路ID
        public string strType;//线路状态（停用，为停用）
        /// <summary>
        /// 1.0 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //动态生成线路
            SelectLines();
        }
        /// <summary>
        /// 1.1 站点表格：序号生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgStation_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        #region 动态生成线路

        public void SelectLines()
        {
            //1、每次你来都清空WrapPanel：环绕面板的内容 
            if (WPLines != null)
            {
                WPLines.Children.Clear();
            }
            //2、查询线路信息   
            DataTable dataTable = myClient.UserControl_Loaded_SelectLine().Tables[0];
            //判断数据是否为空
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    //获取线路ID
                    int LineID = Convert.ToInt32(dataTable.Rows[i]["line_id"]);
                    //获取线路状态
                    string strStopNo = dataTable.Rows[i]["stop_no"].ToString().Trim();                    
                    //3、查询线路明细
                    DataTable dtDetail = myClient.UserControl_Loaded_SelectDetailLine(LineID).Tables[0];
                    int intCount = dtDetail.Rows.Count;//获取总数

                    #region 生成线路
                    //（1）外边框
                    Border border = new Border();
                    border.Width = 125;
                    border.Height = 105;
                    border.Background = Brushes.White;//背景色
                    border.BorderBrush = Brushes.SkyBlue;//边框颜色
                    border.BorderThickness = new Thickness(1, 1, 1, 1);//边框粗度
                    border.CornerRadius = new CornerRadius(5);//圆角
                    border.Margin = new Thickness(5);

                    //（2）Grid
                    Grid grdShow = new Grid();
                    grdShow.Width = 120;
                    grdShow.Height = 100;                   
                    //Grid 分成 2 行
                    grdShow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });//行高占3*
                    grdShow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });//行高占2*
                    //事件：按下鼠标右键
                    grdShow.MouseRightButtonDown += new MouseButtonEventHandler(grd_MouseRightButtonDown);
                    //事件：按下鼠标左键
                    grdShow.MouseLeftButtonDown += new MouseButtonEventHandler(grd_MouseLeftButtonDown);
                    //事件：鼠标移入
                    grdShow.MouseEnter += new MouseEventHandler(grd_MouseEnter);
                    //事件：鼠标移开
                    grdShow.MouseLeave += new MouseEventHandler(grd_MouseLeave);
                    grdShow.ToolTip = strStopNo; //记录线路状态
                    grdShow.Tag = LineID;//记录线路ID

                    //（1）、线路                   
                    TextBlock txt_LineName = new TextBlock();
                    txt_LineName.Text = dataTable.Rows[i]["line_name"].ToString().Trim(); //线路名称
                    //引用当前用户控件的资源样式txt_Style（字体颜色）
                    txt_LineName.Foreground = strStopNo=="停用" ? Brushes.Red : Brushes.SkyBlue;//停用（红色），未停用（天蓝）
                    txt_LineName.FontSize = 18;//字体大小
                    txt_LineName.HorizontalAlignment = HorizontalAlignment.Center;//水平居中
                    txt_LineName.VerticalAlignment = VerticalAlignment.Center;//垂直居中

                    //单元格（第一行，第一列）线路      
                    txt_LineName.SetValue(Grid.RowProperty, 0);//附加属性Grid.Row：       
                    txt_LineName.SetValue(Grid.ColumnProperty, 0);//附加属性Grid.Column：            
                    //内容添加到 grdShow
                    grdShow.Children.Add(txt_LineName);

                    //（2）、线路明细总数                  
                    TextBlock txt_DetailCount = new TextBlock();                    
                    txt_DetailCount.Text ="总站数："+ intCount; //线路名称
                    //引用当前用户控件的资源样式txt_Style（字体颜色）
                    //txt_DetailCount.SetValue(TextBlock.StyleProperty, Application.Current.Resources["txtStyle"]);
                    txt_DetailCount.Foreground = strStopNo == "停用" ? Brushes.Red : Brushes.SkyBlue;//停用（红色），未停用（天蓝）
                    txt_DetailCount.FontSize = 16;//字体大小                 
                    txt_DetailCount.HorizontalAlignment = HorizontalAlignment.Center;//水平居中
                    txt_DetailCount.VerticalAlignment = VerticalAlignment.Center;//垂直居中

                    //单元格（第二行，第一列）线路      
                    txt_DetailCount.SetValue(Grid.RowProperty, 1);
                    txt_DetailCount.SetValue(Grid.ColumnProperty, 0);
                    //内容添加到 grdShow
                    grdShow.Children.Add(txt_DetailCount);                  

                    border.Child = grdShow;//边框（Grid）
                    WPLines.Children.Add(border);//WraPannel(border)
                    #endregion
                }
            }
        }
        /// <summary>
        /// 4.1 按钮 鼠标右键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grd_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            //实例化菜单（添加右键菜单）
            ContextMenu cm = this.FindResource("cmButton") as ContextMenu;
            //目标
            cm.PlacementTarget = sender as Button;
            //位置
            cm.Placement = PlacementMode.MousePoint;
            //显示菜单
            cm.IsOpen = true;

            //获取按钮显示信息==进行分割字符获取床号
            Grid dg = sender as Grid;
            //记录点击的线路ID（修改，删除使用到）
            intLineID = Convert.ToInt32(dg.Tag);
            strType = dg.ToolTip.ToString().Trim();//线路状态
        }

        /// <summary>
        /// 4.2 按钮 鼠标左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grd_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {           
            Grid dg= sender as Grid;
            //背景 天蓝
            dg.Background = new SolidColorBrush(Colors.SkyBlue);
            //获取元素的所有子元素TextBlock
            List<System.Windows.Controls.TextBlock> tb = GetChildObjects.GetChildObject<System.Windows.Controls.TextBlock>(dg);
            for (int i = 0; i < tb.Count; i++)
            {
                tb[i].Foreground = Brushes.White;//字体颜色白色
            }
            //获取线路ID
            int intID = Convert.ToInt32(dg.Tag);
            //3、查询线路明细
            DataTable dtDetail = myClient.UserControl_Loaded_SelectDetailLine(intID).Tables[0];
            dgStation.ItemsSource = dtDetail.DefaultView;//绑定数据
        }

        /// <summary>
        /// 4.3 鼠标移入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grd_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid dg = sender as Grid;            
            //背景 天蓝
            dg.Background = new SolidColorBrush(Colors.SkyBlue);
            //获取元素的所有子元素TextBlock
            List<System.Windows.Controls.TextBlock> tb = GetChildObjects.GetChildObject<System.Windows.Controls.TextBlock>(dg);
            for (int i = 0; i < tb.Count; i++)
            {
                tb[i].Foreground = Brushes.White;//字体颜色白色
            }
        }
        /// <summary>
        /// 4.4 鼠标移开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grd_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid dg = sender as Grid;
            strType = dg.ToolTip.ToString().Trim();//线路状态
            //背景 天蓝
            dg.Background = new SolidColorBrush(Colors.White);
            //获取元素的所有子元素TextBlock
            List<System.Windows.Controls.TextBlock> tb = GetChildObjects.GetChildObject<System.Windows.Controls.TextBlock>(dg);
            for (int i = 0; i < tb.Count; i++)
            {
                tb[i].Foreground = strType == "停用" ? Brushes.Red : Brushes.SkyBlue;//停用（红色），未停用（天蓝）
            }

        }
        #endregion

        #region 右键菜单按钮事件
        /// <summary>
        /// 2.0 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_Insert_Click(object sender, RoutedEventArgs e)
        {
            //1、实例化弹出窗口
            WD_InsertCreateLine myWD_InsertCreateLine = new WD_InsertCreateLine();
            myWD_InsertCreateLine.ShowDialog();
            //2、刷新页面数据
            SelectLines();
        }
        /// <summary>
        /// 2.1 停用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (intLineID != 0)
            {
                //判断数据状态和按钮Content
                if (strType == "未停用")
                {
                    MessageBoxResult dr = MessageBox.Show("是否停用该线路？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
                    if (dr == MessageBoxResult.OK)//如果点了确定按钮
                    {
                        //（1）停用线路                      
                        myClient.UserControl_Loaded_UpdateLine(true, intLineID);
                        SelectLines();//刷新数据
                        //改变菜单名称
                        MenuItem mi = sender as MenuItem;
                        mi.Header = "启用";
                    }
                }
                if (strType == "停用")
                {
                    MessageBoxResult dr = MessageBox.Show("是否启用该线路？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
                    if (dr == MessageBoxResult.OK)//如果点了确定按钮
                    {
                        //（2）启用线路                     
                        myClient.UserControl_Loaded_UpdateLine(false, intLineID);
                        SelectLines();//刷新数据
                        //改变菜单名称
                        MenuItem mi = sender as MenuItem;
                        mi.Header = "停用";
                    }
                }
            }
            else
            {
                MessageBox.Show("操作失误！");
            }
        }
        /// <summary>
        /// 2.2 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mi_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //弹出确定对话框
                if (dr == MessageBoxResult.OK) //如果点了确定按钮
                {
                    int intline_id = intLineID; //线路ID
                    //1、删除线路数据
                    if (intline_id != 0) //如果会员类别ID不为0
                    {
                        //第一步：删除线路信息
                        int intDCount = myClient.UserControl_Loaded_DeleteLine(intline_id); //执行删除事件 
                        if (intDCount > 0)
                        {
                            int intDSum = 0;//记录删除的行数
                            //第二步：删除线路明细站点信息
                            for (int i = 0; i < dgStation.Items.Count; i++)
                            {
                                //获取主键ID
                                int intdetailed_line_id = Convert.ToInt32(((DataRowView)dgStation.Items[i]).Row["detailed_line_id"]);
                                myClient.UserControl_Loaded_DeleteDetailLine(intdetailed_line_id);
                                intDSum++;

                            }
                            //删除行数相同
                            if (intDSum == dgStation.Items.Count)
                            {
                                MessageBox.Show("删除线路成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                //2、刷新页面数据
                                SelectLines();
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择要删除的行。。");
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        
    }
}

