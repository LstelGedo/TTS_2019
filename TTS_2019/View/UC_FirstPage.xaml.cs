using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Visifire.Charts;

namespace TTS_2019.View
{
    /// <summary>
    /// UC_FirstPage.xaml 的交互逻辑
    /// </summary>
    public partial class UC_FirstPage : UserControl
    {
        public UC_FirstPage()
        {
            InitializeComponent();
        }
        #region 动态加载图表

        #region 全局变量
        BLL.Login.LoginClient myClient = new BLL.Login.LoginClient();

        //构建数据
        private List<DateTime> LsTime = new List<DateTime>()
            {
               new DateTime(2012,1,1),
               new DateTime(2012,2,1),
               new DateTime(2012,3,1),
               new DateTime(2012,4,1),
               new DateTime(2012,5,1),
               new DateTime(2012,6,1),
               new DateTime(2012,7,1),
               new DateTime(2012,8,1),
               new DateTime(2012,9,1),
               new DateTime(2012,10,1),
               new DateTime(2012,11,1),
               new DateTime(2012,12,1),
            };
        private List<string> cherry = new List<string>() { "173", "185", "190", "198", "67", "88", "39", "45", "73", "32", "145", "180" };
        private List<string> pineapple = new List<string>() { "73", "54", "78", "87", "120", "76", "136", "107", "97", "52", "176", "239" };
        #endregion
        //页面加载事件
        private void FirstPage_Loaded(object sender, RoutedEventArgs e)
        {
            #region 车辆的类型统计

            DataTable dt = myClient.UserControl_Loaded_SelectTrainSum().Tables[0];
            List<string> strx = new List<string>();
            List<string> stry = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strx.Add(dt.Rows[i]["name"].ToString().Trim());
                stry.Add(dt.Rows[i]["次数"].ToString().Trim());
            }
            //Simon.Children.Clear();
            CreateChartColumn("车辆的类型统计", strx, stry);

            #endregion

            #region 各国旅客人数统计

            DataTable dtCountrySum =
                myClient.UserControl_Loaded_SelectTravellerCountrySum().Tables[0];
            List<string> strListx = new List<string>();
            List<string> strListy = new List<string>();
            for (int i = 0; i < dtCountrySum.Rows.Count; i++)
            {
                strListx.Add(dtCountrySum.Rows[i]["country"].ToString().Trim());
                strListy.Add(dtCountrySum.Rows[i]["number"].ToString().Trim());
            }
            //Simon.Children.Clear();
            CreateChartPie("各国旅客人数统计", strListx, strListy);

            #endregion

            //Simon.Children.Clear();
            CreateChartSpline("2015年网上购票，窗口购票的人数", LsTime, cherry, pineapple);
        }
        #region 柱状图

        public void CreateChartColumn(string name, List<string> valuex, List<string> valuey)
        {
            //第一步：创建一个新的图表实例
            Chart chart = new Chart();
            /*第二步：设置图表属性*/

            //设置图标的宽度和高度
            chart.Width = 500;
            chart.Height = 250;
            chart.Margin = new Thickness(-550, -300, 0, 0);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false; //是否启用或禁用滚动
            chart.View3D = true; //3D效果显示

            //（1）创建一个标题的对象
            Title title = new Title();
            //Brushes.Gainsboro;
            //设置标题的名称
            title.Text = name;
            //title.FontColor = System.Windows.Media.Brushes.Red;
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);


            //（2）创建轴
            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = "辆";
            chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式
            //Pyramid黄金三角 Radar蜘蛛网 SectionFunnel 倒圆锥形 Spline折线图 StackedArea ,Area 连起来的柱状图  StackedArea100墙 横向柱状图
            dataSeries.RenderAs = RenderAs.Bar;
            //dataSeries.RenderAs = RenderAs.StackedColumn;//柱状Stacked


            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.AxisXLabel = valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
                //添加一个点击事件        
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            Simon.Children.Add(gr);
        }
        #endregion

        #region 饼状图

        public void CreateChartPie(string name, List<string> valuex, List<string> valuey)
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            chart.Width = 500;
            chart.Height = 250;
            chart.Margin = new Thickness(500, -300, 0, 0);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false; //是否启用或禁用滚动
            chart.View3D = true; //3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            //Axis yAxis = new Axis();
            ////设置图标中Y轴的最小值永远为0           
            //yAxis.AxisMinimum = 0;
            ////设置图表中Y轴的后缀          
            //yAxis.Suffix = "斤";
            //chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();

            // 设置数据线的格式
            dataSeries.RenderAs = RenderAs.Radar;
            /*  Column = 列,Line = 行,Pie = 派, Bar = 酒吧,Area = 面积,Doughnut = 甜甜圈, StackedColumn = 堆叠列,StackedColumn100 = 堆叠柱状100,StackedBar = 堆叠条,StackedBar100 = 堆叠杆100,
                StackedArea = 堆积面积,StackedArea100 = 堆叠面积100,Bubble = 泡沫,Point = 点,StreamLineFunnel = 流线漏斗,SectionFunnel = 截面漏斗,Stock = 股票,CandleStick = 烛台,
                StepLine = 步长,Spline = 花键,Radar = 雷达,Polar = 极地,Pyramid = 金字塔,QuickLine = 快线
             */
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < valuex.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.AxisXLabel = valuex[i];

                dataPoint.LegendText = "##" + valuex[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(valuey[i]);
                //添加一个点击事件        
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);
            Simon.Children.Add(gr);
        }
        #endregion

        #region 折线图

        public void CreateChartSpline(string name, List<DateTime> lsTime, List<string> cherry, List<string> pineapple)
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            chart.Width = 600;
            chart.Height = 250;
            chart.Margin = new Thickness(0, 300, 10, 5);
            //是否启用打印和保持图片
            chart.ToolBarEnabled = false;

            //设置图标的属性
            chart.ScrollingEnabled = false; //是否启用或禁用滚动
            chart.View3D = true; //3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            //设置标题的名称
            title.Text = name;
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            //初始化一个新的Axis
            Axis xaxis = new Axis();
            //设置Axis的属性
            //图表的X轴坐标按什么来分类，如时分秒
            xaxis.IntervalType = IntervalTypes.Months;
            //图表的X轴坐标间隔如2,3,20等，单位为xAxis.IntervalType设置的时分秒。
            xaxis.Interval = 1;
            //设置X轴的时间显示格式为7-10 11：20           
            xaxis.ValueFormatString = "MM月";
            //给图标添加Axis            
            chart.AxesX.Add(xaxis);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = "人";
            chart.AxesY.Add(yAxis);


            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式。               
            dataSeries.LegendText = "网上购票";
            dataSeries.RenderAs = RenderAs.Spline; //折线图
            dataSeries.XValueType = ChartValueTypes.DateTime;
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < lsTime.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.XValue = lsTime[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(cherry[i]);
                dataPoint.MarkerSize = 8;
                //dataPoint.Tag = tableName.Split('(')[0];
                //设置数据点颜色                  
                // dataPoint.Color = new SolidColorBrush(Colors.LightGray);                   
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }
            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);
            // 创建一个新的数据线。               
            DataSeries dataSeriesPineapple = new DataSeries();
            // 设置数据线的格式。
            dataSeriesPineapple.LegendText = "窗口购票";
            dataSeriesPineapple.RenderAs = RenderAs.Spline; //折线图

            dataSeriesPineapple.XValueType = ChartValueTypes.DateTime;
            // 设置数据点              

            DataPoint dataPoint2;
            for (int i = 0; i < lsTime.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint2 = new DataPoint();
                // 设置X轴点                    
                dataPoint2.XValue = lsTime[i];
                //设置Y轴点                   
                dataPoint2.YValue = double.Parse(pineapple[i]);
                dataPoint2.MarkerSize = 8;
                //dataPoint2.Tag = tableName.Split('(')[0];
                //设置数据点颜色                  
                // dataPoint.Color = new SolidColorBrush(Colors.LightGray);                   
                dataPoint2.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeriesPineapple.DataPoints.Add(dataPoint2);
            }
            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeriesPineapple);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);

            Simon.Children.Add(gr);
        }

        private void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataPoint dp = sender as DataPoint;
            MessageBox.Show(dp.YValue.ToString());
        }
        #endregion
        #endregion
    }
}
