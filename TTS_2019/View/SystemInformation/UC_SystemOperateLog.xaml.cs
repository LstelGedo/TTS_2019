using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// UC_SystemOperateLog.xaml 的交互逻辑
    /// </summary>
    public partial class UC_SystemOperateLog : UserControl
    {
        public UC_SystemOperateLog()
        {
            InitializeComponent();
        }
        private BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        private DataTable dtOperateLog;
        #region 数据库查询需要的参数，字段名，表名，排序字段,每页的大小，查询第几页，where条件，页数
        /// <summary>
        /// 使用linq 的语句实现数据获取
        /// </summary>

        private string strGetFields = "ROW_NUMBER () over(order by  t_data_record.data_record_id) as number,t_data_record.data_record_id,"
                                      +
                                      "  t_data_record.operator_id,RTRIM(t_staff.staff_name) AS staff_name, t_data_record.as_operation_type_id,"
                                      +
                                      " RTRIM(t_detailed_attribute_gather.detailed_attribute_gather_name) AS operation_type,"
                                      +
                                      " RTRIM(t_data_record.operational_context)AS operational_context, t_data_record.operation_time";

        private string strTblName = "  t_data_record INNER JOIN  t_staff ON t_data_record.operator_id = t_staff.staff_id INNER JOIN"
                                    +
                                    "  t_detailed_attribute_gather ON  t_data_record.as_operation_type_id = t_detailed_attribute_gather.detailed_attribute_gather_id  ";

        private string strFldName = "t_data_record.data_record_id"; //排序的字段名
        private int intPageSize = 20; //页尺寸
        private int intPageIndex = 1; //页码
        private string strWhere = ""; //查询条件 (注意: 不要加 where)   
        private decimal decPageCount = 0; //页数
        #endregion
        /*分页查询*/
        DataTable ChaXunShuJu(int intPageLineCount, int intCurrentPage, bool bolSeleteSum, bool bolInvertedOrder)
        {
            try
            {
                strWhere = " 1 = 1 "; //初始化查询条件
                if (txt_Select.Text != "")
                {
                    string strContent = txt_Select.Text.Trim();
                    //累加模糊查询内容
                    strWhere = " staff_name like '%'+'" + strContent + "'+'%'" +
                               " or detailed_attribute_gather_name like '%'+'" + strContent + "'+'%'" +
                               " or operational_context like '%'+'" + strContent + "'+'%'";
                }
                if (dtp_BeginTime.Text!="" && dtp_EndTime.Text!="")//时间为条件
                {
                    DateTime BeginTime = Convert.ToDateTime(dtp_BeginTime.Text);
                    DateTime EndTime = Convert.ToDateTime(dtp_EndTime.Text);

                    if (strWhere != "")
                    {
                        strWhere = strWhere + "and (operation_time>='" + BeginTime + "'and operation_time<='" + EndTime + "')";
                    }
                    else
                    {
                        strWhere = "(operation_time>='" + BeginTime + "'and operation_time<='" + EndTime + "')";
                    }

                }
                decPageCount =Convert.ToDecimal(myPublicFunctionClient.PublicPagingSelect(strTblName, strGetFields, strFldName, intPageSize,
                            intPageIndex, true, true, strWhere).Tables[0].Rows[0][0]); //获取查询结果的行数
                if (decPageCount / intPageLineCount <= Convert.ToInt32(decPageCount / intPageLineCount)) //判断有多少页
                {
                    decPageCount = Convert.ToInt32(decPageCount / intPageLineCount);
                }
                else
                {
                    decPageCount = Convert.ToInt32(decPageCount / intPageLineCount) + 1;
                }
                if (intCurrentPage > decPageCount) //如果当前页数大于最大页数，则重新赋值为最大页数
                {
                    intPageIndex = (int)decPageCount;
                    intCurrentPage = (int)decPageCount;
                }
                if (intPageIndex == 0 || intCurrentPage == 0) //如果当前页数为0，则重新赋值为1
                {
                    intPageIndex = 1;
                    intCurrentPage = 1;
                }
                lblMaxPage.Content = decPageCount.ToString(); //给最大页数赋值
                txtCurrentPage.Text = intCurrentPage.ToString(); //给当前页数文本框赋值
                return  myPublicFunctionClient.PublicPagingSelect(strTblName, strGetFields, strFldName, intPageLineCount,
                        intCurrentPage, bolSeleteSum, bolInvertedOrder, strWhere).Tables[0]; //返回查询出来的结果
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
                return null;
                MessageBox.Show(ex1,"系统提示",MessageBoxButton.OK,MessageBoxImage.Error);                
            }
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {            
            txtCurrentPage.Text = "0"; //当前页数
            lblMaxPage.Content = decPageCount; //最大页数
            int[] intPageLineCounts = { 18, 36, 54, 72, 90, 108 }; //创建int数组
            cboPageLineCount.ItemsSource = intPageLineCounts; //绑定每页行数下拉框
            cboPageLineCount.Text = intPageSize.ToString(); //设置默认值
            btnSelect_Click(null, null);

            #region 虚构绑定日期的下拉框

            string[] countType = { "---请选择---","今天", "昨天", "本周", "上周", "本月", "上月", "本季度", "上季度", "本年", "上年" };
            DataTable dtTable = new DataTable();
            dtTable.Columns.Add("id", typeof(int));
            dtTable.Columns.Add("name", typeof(string));
            for (int i = 0; i < countType.Length; i++)
            {
                dtTable.Rows.Add();
                dtTable.Rows[i]["id"] = i;
                dtTable.Rows[i]["name"] = countType[i];
            }
            cboTimeInterval.ItemsSource = dtTable.DefaultView; //绑定自定义类型的下拉框值
            cboTimeInterval.DisplayMemberPath = "name";
            cboTimeInterval.SelectedValuePath = "id";
            #endregion
        }
        #region 控制底部分页按钮
        public void UpdatePagingButton()
        {
            if (Convert.ToInt32(txtCurrentPage.Text) <= 1) //如果是首页，则把上一页和首页按钮禁用(样式)
            {
                imgMostUp.Opacity = 0.3;
                imgMostUp.Cursor = Cursors.Arrow;
                imgUp.Opacity = 0.3;
                imgUp.Cursor = Cursors.Arrow;
            }
            else
            {
                imgMostUp.Opacity = 1;
                imgMostUp.Cursor = Cursors.Hand;
                imgUp.Opacity = 1;
                imgUp.Cursor = Cursors.Hand;
            }
            if (Convert.ToInt32(txtCurrentPage.Text) >= Convert.ToInt32(lblMaxPage.Content)) //如果是尾页，则把下一页和尾页按钮禁用(样式)
            {
                imgMostDown.Opacity = 0.3;
                imgMostDown.Cursor = Cursors.Arrow;
                imgDown.Opacity = 0.3;
                imgDown.Cursor = Cursors.Arrow;
            }
            else
            {
                imgMostDown.Opacity = 1;
                imgMostDown.Cursor = Cursors.Hand;
                imgDown.Opacity = 1;
                imgDown.Cursor = Cursors.Hand;
            }
        }
        #endregion
        /*首页*/
        private void imgMostUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex = 1;
                txtCurrentPage.Text = "1";
                btnSelect_Click(null, null);
            }
        }

        /*上一页*/
        private void imgUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex--;
                txtCurrentPage.Text = intPageIndex.ToString();
                btnSelect_Click(null, null);
            }
        }

        /*当前页*/
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
                    intPageIndex = Convert.ToInt32(textBox.Text.Trim());
                    btnSelect_Click(null, null);
                }
            }
            catch (Exception)
            {
                btnSelect_Click(null, null);
            }
        }

        /*下一页*/
        private void imgDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex++;
                txtCurrentPage.Text = intPageIndex.ToString();
                btnSelect_Click(null, null);
            }
        }

        /*尾页*/
        private void imgMostDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex = Convert.ToInt32(lblMaxPage.Content);
                txtCurrentPage.Text = intPageIndex.ToString();
                btnSelect_Click(null, null);
            }
        }

        /*定义每页显示行数*/
        private void cboPageLineCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            intPageSize = Convert.ToInt32(cboPageLineCount.SelectedValue);
            intPageIndex = 1;
            btnSelect_Click(null, null);
        }
        /*自定义时间段*/
        private void cboTimeInterval_DropDownClosed(object sender, EventArgs e)
        {
            #region 时间段查询

            DateTime dtpDateNow = DateTime.Now; //声明一个日期变量
            DateTime dtpDateNowAddMonth = dtpDateNow.AddMonths(1);
            DateTime dtpNowDay = DateTime.Now.Date; //获取今天的日期和时间
            string strNowDay = DateTime.Now.Day.ToString(); //获取今天日号

            if (cboTimeInterval.Text == "---请选择---")
            {
                dtp_BeginTime.Text = "";
                dtp_EndTime.Text = "";
            }

            if (cboTimeInterval.Text == "今天") //根据今天进行时间日期查询
            {
                ////方法一：
                //decimal decYesterday = Convert.ToDecimal(strNowDay) - 1;//获取昨天的日号
                //decimal decTomorrow = Convert.ToDecimal(strNowDay) + 1;//获取明天的日号
                ////获取昨天的日期和时间
                //string strYesterday = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? "-" + DateTime.Now.Month.ToString() : "-0" 
                //    + DateTime.Now.Month.ToString()) + "-" + decYesterday + " 23:59:59";
                ////获取明天的日期和时间
                //string strTomorrow = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? "-" + DateTime.Now.Month.ToString() : "-0" 
                //    + DateTime.Now.Month.ToString()) + "-" + decTomorrow + " 00:00:00";
                //dtp_BeginTime.Value = DateTime.Now.Date;
                //dtp_EndTime.Value = DateTime.Now.Date;

                //方法二：
                string strYesterday = DateTime.Now.AddDays(-1).ToShortDateString() + " 23:59:59";
                string strTomorrow = DateTime.Now.AddDays(1).ToShortDateString() + " 00:00:00";
                dtp_BeginTime.Text = strYesterday; //把昨天的日期和时间绑定到登记时间的控件上
                dtp_EndTime.Text = strTomorrow; //把明天的日期和时间绑定到结束时间的控件上 
            }
            if (cboTimeInterval.Text == "昨天")
            {
                decimal decTheDayBeforeYesterday = Convert.ToDecimal(strNowDay) - 2; //获取前天的日号
                //获取前天的日期和时间
                string strTheDayBeforeYesterday = DateTime.Now.Year.ToString() +
                                                  (DateTime.Now.Month > 9
                                                      ? "-" + DateTime.Now.Month.ToString()
                                                      : "-0"
                                                        + DateTime.Now.Month.ToString()) + "-" +
                                                  decTheDayBeforeYesterday + " 23:59:59";
                dtp_BeginTime.Text = strTheDayBeforeYesterday; //把前天的日期和时间绑定登记时间的控件上
                dtp_EndTime.Text = dtpNowDay.ToString(); //把今天的日期和时间绑定到结束时间的控件上

                //dtp_BeginTime.Value = DateTime.Now.AddDays(-1).Date;
                //dtp_EndTime.Value = DateTime.Now.AddDays(-1).Date;
            }
            if (cboTimeInterval.Text == "本周")
            {
                int intThisWeek = Convert.ToInt32(dtpDateNow.DayOfWeek.ToString("d")); //根据获取的星期的星期天数传型为整形对应的号码
                DateTime dtpStartWeek = dtpDateNow.AddDays(1 - intThisWeek); //获取本周周一
                DateTime dtpEndweek = dtpStartWeek.AddDays(6); //获取本周周日
                dtp_BeginTime.Text = dtpStartWeek.ToString(); //把本周周一绑定到开始时间的控件中
                dtp_EndTime.Text = dtpEndweek.ToString(); //把本周周日绑定到结束时间的控件中
            }
            if (cboTimeInterval.Text == "上周")
            {
                int intLastWeek = Convert.ToInt32(dtpDateNow.DayOfWeek.ToString("d")); ///根据获取的星期的星期天数传型为整形对应的号码
                DateTime dtpStartweek = dtpDateNow.AddDays(1 - intLastWeek - 7); //获取上周周一
                DateTime dtpEndWeek = dtpStartweek.AddDays(6); //获取上周周日
                dtp_BeginTime.Text = dtpStartweek.ToString(); //把上周周一绑定到开始时间的控件中
                dtp_EndTime.Text = dtpEndWeek.ToString(); //把上周周日绑定到结束时间的控件中
            }
            if (cboTimeInterval.Text == "本月")
            {
                string strThisMonthStart = dtpDateNow.AddDays(-(dtpDateNow.Day) + 1).ToString("yyyy-MM-dd"); //获取本月月初 
                string strThisMonthEnd = dtpDateNowAddMonth.AddDays(-dtpDateNow.Day).ToString("yyyy-MM-dd"); //本月最后一天
                dtp_BeginTime.Text = strThisMonthStart; //把本月初绑定到开始时间上
                dtp_EndTime.Text = strThisMonthEnd; //把本月末绑定到结束时间上
            }
            if (cboTimeInterval.Text == "上月")
            {
                string strLastMonthStart = dtpDateNow.AddMonths(-1)
                    .AddDays(-(dtpDateNow.Day) + 1)
                    .ToString("yyyy-MM-dd"); //获取上个月的月初
                string strLastMonthEnd = dtpDateNow.AddDays(-(dtpDateNow.Day)).ToString("yyyy-MM-dd"); //获取上个月的月末
                dtp_BeginTime.Text = strLastMonthStart; //把上个月初绑定登记开始时间上
                dtp_EndTime.Text = strLastMonthEnd; //把上个月末绑定到结束时间上
            }
            if (cboTimeInterval.Text == "本季度")
            {
                DateTime dtpThisStartQuarter =
                    dtpDateNow.AddMonths(0 - (dtpDateNow.Month - 1) % 3).AddDays(1 - dtpDateNow.Day); //获取本季度初
                DateTime dtpThisEndQuarter = dtpDateNow.AddMonths(3).AddDays(-1); //获取本季度末
                dtp_BeginTime.Text = dtpThisStartQuarter.ToString(); //把本季度初绑定到开始时间上
                dtp_EndTime.Text = dtpThisEndQuarter.ToString(); //把本月末绑定到结束时间上
            }
            if (cboTimeInterval.Text == "上季度")
            {
                DateTime dtpLastStartQuarter =
                    dtpDateNow.AddMonths(-3 - (dtpDateNow.Month - 1) % 3).AddDays(1 - dtpDateNow.Day); //上季度初
                DateTime dtpLastEndQuarter = dtpLastStartQuarter.AddMonths(3).AddDays(-1); //上季度末
                dtp_BeginTime.Text = dtpLastStartQuarter.ToString(); //把上季度初绑定到开始时间上
                dtp_EndTime.Text = dtpLastEndQuarter.ToString(); //把上季度末绑定到结束时间上
            }
            if (cboTimeInterval.Text == "本年")
            {
                string strThisYearStart = DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).ToShortDateString();
                //获取本年初
                string strThisYearEnd =
                    DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddYears(1).AddDays(-1).ToShortDateString();
                //获取本年末
                dtp_BeginTime.Text = strThisYearStart.ToString(); //把本年初绑定到开始时间上
                dtp_EndTime.Text = strThisYearEnd.ToString(); //把本年末绑定到结束时间上
            }
            if (cboTimeInterval.Text == "上年")
            {
                string strLastYearStart =
                    DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddYears(-1).ToShortDateString(); //获取上年年初
                string strLastYearEnd =
                    DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddDays(-1).ToShortDateString(); //获取上年年末
                dtp_BeginTime.Text = strLastYearStart.ToString(); //把上年初绑定到开始时间上
                dtp_EndTime.Text = strLastYearEnd.ToString(); //把上年末绑定到结束时间上
            }
            #endregion
        }

        /*查询*/
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            //调用查询方法，把返回的值，赋给DGV
            dtOperateLog = ChaXunShuJu(intPageSize, intPageIndex, false, true);
            dgDateRecord.ItemsSource = dtOperateLog.AsDataView();
            if (dtOperateLog.Rows.Count < 1)
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

        /*删除*/
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 62, "删除操作记录【" + (((DataRowView)dgDateRecord.SelectedItem).Row["operational_context"]).ToString() + "】", DateTime.Now);

            }
        }       
        
        //导出数据
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgDateRecord.Items.Count>0)
                {
                    string strFildName = "操作日志";
                    if (ExportToExcel.Export(dgDateRecord, strFildName) == true)
                    {
                        MessageBox.Show("数据导出成功!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("数据导出失败!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
