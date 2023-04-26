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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TTS_2019.View.LineManage
{
    /// <summary>
    /// UC_CreateLine.xaml 的交互逻辑
    /// </summary>
    public partial class UC_CreateLine : UserControl
    {
        public UC_CreateLine()
        {
            InitializeComponent();
        }

        //实例化服务端
        BLL.UC_CreateLine.UC_CreateLineClient myClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        BLL.PublicFunction.PublicFunctionClient myFunctionClient = new BLL.PublicFunction.PublicFunctionClient();//分页服务
        #region  数据库查询需要的参数，字段名，表名，排序字段,每页的大小，查询第几页，where条件，页数
        string strGetFields = "ROW_NUMBER () over(order by  line_id) as number, line_id,RTRIM(line_name) AS line_name,RTRIM(budget_days) AS budget_days,"
            + "RTRIM(simple_code) AS simple_code, RTRIM(mileage) AS mileage,"
            + " RTRIM(case WHEN stop_no = 'true' THEN '停用' ELSE '未停用' END) AS stop_no,"
            + " RTRIM(case WHEN stop_no = 'true' THEN 'Red' ELSE 'Blue' END) AS color,RTRIM(note) AS note";

        string strTblName = "t_line";
        string strFldName = "line_id ";//排序的字段名
        int intPageSize = 20;//页尺寸
        int intPageIndex = 1;//页码
        string strWhere = "1=1";   //查询条件 (注意: 不要加 where)   
        decimal decPageCount = 0;  //页数
        #endregion
        int LineID;//接收线路ID
        DataTable dtLine;//线路数据表

        //绑定线路信息   
        private void GetTable()
        {
            dtLine = myClient.UserControl_Loaded_SelectLine().Tables[0];//查询线路信息         
            dgLine.ItemsSource = dtLine.DefaultView;//绑定表格数据（左边线路表）
        }

        //加载事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            #region 页面初始化          
            btnStop.IsEnabled = false;//禁用停用按钮
            btnStop.Opacity = 0.5;      //半透明  
            btnDelete.IsEnabled = false;//禁用停用按钮
            btnDelete.Opacity = 0.5;      //半透明  
            txtCurrentPage.Text = "0";//当前页数
            lblMaxPage.Content = decPageCount;//最大页数
            int[] intPageLineCounts = { 17, 34, 51, 68, 85, 102 };//创建int数组
            cboPageLineCount.ItemsSource = intPageLineCounts;//绑定每页行数下拉框
            cboPageLineCount.Text = intPageSize.ToString();//设置默认值 
            #endregion
            GetTable();//绑定表格数据
        }

        //新增
        private void btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            //1、实例化弹出窗口
            WD_InsertCreateLine myWD_InsertCreateLine = new WD_InsertCreateLine();
            myWD_InsertCreateLine.ShowDialog();
            //2、刷新页面数据
            GetTable();//刷新
        }
        //删除
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //弹出确定对话框
                if (dr == MessageBoxResult.OK) //如果点了确定按钮
                {
                    int intline_id = Convert.ToInt32(((DataRowView)dgLine.CurrentItem).Row["line_id"]); //线路ID
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
                                GetTable();//刷新数据
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
        //停用
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            //获取当前按钮的Content
            Button btn = sender as Button;
            string strbtn = btn.Content.ToString();

            if (dgLine.CurrentCell != null)
            {
                LineID = Convert.ToInt32(((DataRowView)dgLine.SelectedItem).Row["line_id"]);
                string strType = ((DataRowView)dgLine.SelectedItem).Row["stop_no"].ToString().Trim();
                bool blStop = false;
                if (LineID != 0)
                {
                    //判断数据状态和按钮Content
                    if (strType== "未停用" && strbtn == "停用")
                    {                      
                        MessageBoxResult dr = MessageBox.Show("是否停用该线路？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
                        if (dr == MessageBoxResult.OK)//如果点了确定按钮
                        {
                            //（1）停用线路
                            blStop = true;//停用
                            myClient.UserControl_Loaded_UpdateLine(blStop,LineID);
                            GetTable();//刷新数据
                            btnStop.Content = "启用";
                        }
                    }
                    if (strType == "停用" && strbtn == "启用")
                    {
                        MessageBoxResult dr = MessageBox.Show("是否启用该线路？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
                        if (dr == MessageBoxResult.OK)//如果点了确定按钮
                        {
                            //（2）启用线路
                            blStop = false;//启用
                            myClient.UserControl_Loaded_UpdateLine(blStop, LineID);
                            GetTable();//刷新数据
                            btnStop.Content = "停用";
                        }
                    }                    
                }
                else
                {
                    MessageBox.Show("操作失误！");
                }
            }
            else
            {
                MessageBox.Show("抱歉！没有选中数据！");
            }
        }
        //选中行改变事件（线路表）
        private void dgTicket_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //获取左边（线路表的行数据）
            //判断选中的行是否为空
            if (((DataRowView)dgLine.SelectedItem) != null)
            {
                //激活停用按钮
                btnStop.IsEnabled = true;
                btnStop.Opacity = 1;
                btnDelete.IsEnabled = true;
                btnDelete.Opacity = 1;
                //获取选中行数据改变按钮的Content
                string strType = ((DataRowView)dgLine.SelectedItem).Row["stop_no"].ToString().Trim();
                if (strType == "未停用")
                {
                    btnStop.Content = "停用";
                }
                if (strType == "停用")
                {
                    btnStop.Content = "启用";
                }
            }
            else
            {
                //禁用停用按钮
                btnStop.IsEnabled = false;
                btnStop.Opacity = 0.5;
                btnDelete.IsEnabled = false;
                btnDelete.Opacity = 0.5;
            }
            //绑定右边表格（线路明细信息）
            try
            {
                //判断选中行有值
                if (dgLine.CurrentCell != null)
                {
                    //获取单元格值：主键(线路ID)
                    LineID = Convert.ToInt32(((DataRowView)dgLine.CurrentItem).Row["line_id"]);
                    if (LineID != 0)
                    {                       
                        #region  绑定线路明细信息（右边表格）
                        DataTable dtOderDetail = myClient.UserControl_Loaded_SelectDetailLine(LineID).Tables[0];
                        dgStation.ItemsSource = dtOderDetail.DefaultView;//绑定数据源                        
                        #endregion
                    }
                    else
                    {
                        MessageBox.Show("操作失误！");
                    }
                }

                else
                {
                    MessageBox.Show("抱歉！没有可供查询的站点！");
                }
            }
            catch
            {

            }
        }
        #region 控制底部分页按钮
        // 分页查询方法
        DataTable ChaXunShuJu(int intPageLineCount, int intCurrentPage, bool bolSeleteSum, bool bolInvertedOrder)
        {
            if (txtSelectContent.Text != "")
            {
                //累加模糊查询内容
                strWhere = "line_name like '%'+'" + txtSelectContent.Text.Trim() + "'+'%'" +
                            " or simple_code like '%'+'" + txtSelectContent.Text.Trim() + "'+'%'" +
                            " or note like '%'+'" + txtSelectContent.Text.Trim() + "'+'%'";
            }
            else if (txtSelectContent.Text == "" || txtSelectContent.Text == string.Empty)
            {
                strWhere = "1=1";
            }
            //获取查询结果的行数
            decPageCount = Convert.ToDecimal(myFunctionClient.PublicPagingSelect(strTblName, strGetFields, strFldName, intPageSize, intPageIndex, true, true, strWhere).Tables[0].Rows[0][0]);
            //判断有多少页
            if (decPageCount / intPageLineCount <= Convert.ToInt32(decPageCount / intPageLineCount))
            {
                //得到最新总页数（条件）
                decPageCount = Convert.ToInt32(decPageCount / intPageLineCount);
            }
            else
            {
                //默认
                decPageCount = Convert.ToInt32(decPageCount / intPageLineCount) + 1;
            }
            if (intCurrentPage > decPageCount)//如果当前页数大于最大页数，则重新赋值为最大页数
            {
                //当前页==最大页
                intPageIndex = (int)decPageCount;
                intCurrentPage = (int)decPageCount;
            }
            if (intPageIndex == 0 || intCurrentPage == 0)//如果当前页数为0，则重新赋值为1
            {
                //当前页==1
                intPageIndex = 1;
                intCurrentPage = 1;
            }
            lblMaxPage.Content = decPageCount.ToString();//给最大页数赋值
            txtCurrentPage.Text = intCurrentPage.ToString();//给当前页数文本框赋值
            //分页得到的Data
            return myFunctionClient.PublicPagingSelect(strTblName, strGetFields, strFldName, intPageLineCount, intCurrentPage, bolSeleteSum, bolInvertedOrder, strWhere).Tables[0];//返回查询出来的结果
        }
        //查询(条件查询)
        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            dtLine = ChaXunShuJu(intPageSize, intPageIndex, false, true);//调用查询方法，把返回的值，赋给DGV
            dgLine.ItemsSource = dtLine.AsDataView();//得到分页数据
            //返回数据为空
            if (dtLine.Rows.Count < 1)
            {
                intPageIndex = 0;
                txtCurrentPage.Text = "0";
                cboPageLineCount.IsEnabled = false;//禁用每页行数下拉框
                txtCurrentPage.IsEnabled = false;//禁用当前页文本框
            }
            else
            {
                cboPageLineCount.IsEnabled = true;
                txtCurrentPage.IsEnabled = true;
            }
            UpdatePagingButton();
        }
        //首页、上一页、尾页、下一页按钮初始化
        public void UpdatePagingButton()
        {
            //如果是首页，则把上一页和首页按钮禁用(样式)
            if (Convert.ToInt32(txtCurrentPage.Text) <= 1)
            {
                //禁用首页、上一页
                imgMostUp.Opacity = 0.3;
                imgMostUp.Cursor = Cursors.Arrow;
                imgUp.Opacity = 0.3;
                imgUp.Cursor = Cursors.Arrow;

            }
            else
            {
                //启用首页、上一页
                imgMostUp.Opacity = 1;
                imgMostUp.Cursor = Cursors.Hand;
                imgUp.Opacity = 1;
                imgUp.Cursor = Cursors.Hand;
            }

            if (Convert.ToInt32(txtCurrentPage.Text) >= Convert.ToInt32(lblMaxPage.Content))//如果是尾页，则把下一页和尾页按钮禁用(样式)
            {
                //禁用首页、上一页
                imgMostDown.Opacity = 0.3;
                imgMostDown.Cursor = Cursors.Arrow;
                imgDown.Opacity = 0.3;
                imgDown.Cursor = Cursors.Arrow;
            }
            else
            {
                //启用首页、上一页
                imgMostDown.Opacity = 1;
                imgMostDown.Cursor = Cursors.Hand;
                imgDown.Opacity = 1;
                imgDown.Cursor = Cursors.Hand;
            }
        }

        //首页按钮（鼠标左键按下事件）
        private void imgMostUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //首页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex = 1;//当前显示页:第一页
                txtCurrentPage.Text = "1";//首页
                btn_Select_Click(null, null);
            }
        }
        //上一页按钮（鼠标左键按下事件）
        private void imgUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //上一页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex--;
                txtCurrentPage.Text = intPageIndex.ToString();
                btn_Select_Click(null, null);
            }
        }
        //下一页按钮（鼠标左键按下事件）
        private void imgDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //下一页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex++;
                txtCurrentPage.Text = intPageIndex.ToString();
                btn_Select_Click(null, null);
            }
        }
        //尾页按钮（鼠标左键按下事件）
        private void imgMostDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //尾页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex = Convert.ToInt32(lblMaxPage.Content);
                txtCurrentPage.Text = intPageIndex.ToString();
                btn_Select_Click(null, null);
            }
        }
        //当前页输入文本（文本改变事件）
        private void txtCurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //屏蔽中文输入和非法字符粘贴输入
                TextBox textBox = sender as TextBox;
                TextChange[] change = new TextChange[e.Changes.Count];
                e.Changes.CopyTo(change, 0);

                int offset = change[0].Offset;
                if (change[0].AddedLength > 0)
                {
                    int num = 0;
                    if (!int.TryParse(textBox.Text, out num))
                    {
                        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength).ToString().Trim();
                        textBox.Select(offset, 0);
                    }
                }
                if (decPageCount != 0)
                {
                    if (Convert.ToInt32(textBox.Text.Trim()) > decPageCount)
                    {
                        textBox.Text = decPageCount.ToString();
                    }
                    else if (Convert.ToInt32(textBox.Text.Trim()) < 1)
                    {
                        textBox.Text = "1";
                    }
                }
                if (intPageIndex != Convert.ToInt32(textBox.Text.Trim()) && Convert.ToInt32(textBox.Text.Trim()) >= 1)
                {
                    //当前页==输入值
                    intPageIndex = Convert.ToInt32(textBox.Text.Trim());
                    btn_Select_Click(null, null);
                }
            }
            catch (Exception)
            {
                btn_Select_Click(null, null);
            }
        }
        //自定义每页行数(下拉框改变事件)
        private void cboPageLineCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //每页行数
            intPageSize = Convert.ToInt32(cboPageLineCount.SelectedValue);
            intPageIndex = 1;
            //触发分页
            btn_Select_Click(null, null);
        }
        #endregion
       // 自定义一列显示序号
        private void dgLine_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void dgStation_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
