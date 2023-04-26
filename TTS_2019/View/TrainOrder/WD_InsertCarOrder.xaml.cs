using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TTS_2019.View.TrainOrder
{
    /// <summary>
    /// WD_InsertCarOrder.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertCarOrder : Window
    {
        public WD_InsertCarOrder()
        {
            InitializeComponent();
        }
        BLL.UC_CarOrder.UC_CarOrderClient myClient = new BLL.UC_CarOrder.UC_CarOrderClient();
        BLL.UC_CreateLine.UC_CreateLineClient myLineClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        int LineID;
        int intTrainId;
        DataTable dtTrain;//车辆信息
        DataTable dtOrderDetail;//车次明细
        DataTable dtDetai;
        #region Loaded事件
        private void WD_InsertCarOrder_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        #endregion
        #region 退出按钮时间
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
        #endregion
        #region 选择线路事件

        private void btn_Sele_Line_Click(object sender, RoutedEventArgs e)
        {
            LineManage.WD_ChoiceLine myWD_ChoiceLine = new LineManage.WD_ChoiceLine();
            myWD_ChoiceLine.ShowDialog();
            try
            {
                txt_line.Text = LineManage.WD_ChoiceLine.drv.Row["line_name"].ToString();
                txt_disdence.Text = LineManage.WD_ChoiceLine.drv.Row["mileage"].ToString();
                LineID = Convert.ToInt32(LineManage.WD_ChoiceLine.drv.Row["line_id"]);
                dtDetai = myLineClient.UserControl_Loaded_SelectDetailLine(LineID).Tables[0];
                if (LineID > 0)
                {
                    dtOrderDetail = new DataTable();
                    dtOrderDetail.Columns.Add("number", typeof(string));
                    dtOrderDetail.Columns.Add("order_detail_id", typeof(int));
                    dtOrderDetail.Columns.Add("order_id", typeof(int));
                    dtOrderDetail.Columns.Add("site_id", typeof(int));
                    dtOrderDetail.Columns.Add("site_name", typeof(string));
                    dtOrderDetail.Columns.Add("distance", typeof(decimal));
                    dtOrderDetail.Columns.Add("arrival_time", typeof(string));
                    dtOrderDetail.Columns.Add("driving_time", typeof(string));
                    dtOrderDetail.Columns.Add("stop_time", typeof(string));
                    dtOrderDetail.Columns.Add("using_time", typeof(string));
                    dtOrderDetail.Columns.Add("day", typeof(string));
                    for (int i = 0; i < dtDetai.Rows.Count; i++)
                    {
                        dtOrderDetail.Rows.Add(i);
                        dtOrderDetail.Rows[i]["number"] = dtDetai.Rows[i]["number"].ToString();
                        dtOrderDetail.Rows[i]["site_name"] = dtDetai.Rows[i]["site_name"].ToString();
                        dtOrderDetail.Rows[i]["distance"] = Convert.ToDecimal(dtDetai.Rows[i]["distance"]);
                        dtOrderDetail.Rows[i]["site_id"] = Convert.ToInt32(dtDetai.Rows[i]["site_id"]);
                    }
                    dgOrderDetail.ItemsSource = dtOrderDetail.DefaultView;
                    if (dgOrderDetail.Items.Count != 0) //若返回的值不为0,即有值
                    {
                        dtOrderDetail.Rows[0]["driving_time"] = "请双击输入"; //为第一行开车时间赋初始值
                    }
                }
            }
            catch
            {
            }
        }
        #endregion
        #region 计算时间事件
        string SuDu;

        private void TimeCount(int Index)
        {
            for (int i = Index; i < dtOrderDetail.Rows.Count; i++)
            {
                if (i < dtOrderDetail.Rows.Count - 1)
                {
                    int JuLi = Convert.ToInt32(dtOrderDetail.Rows[i + 1]["distance"]); //计算两站之间的距离
                    SuDu = txt_rate.Text.ToString().Trim();
                    if (dtOrderDetail.Rows[i]["stop_time"].ToString().Trim() != "")
                    {
                        string time2 = dtOrderDetail.Rows[dgOrderDetail.SelectedIndex]["arrival_time"].ToString().Trim();
                        int min1 = Convert.ToInt32(time2.Split(':')[1]);
                        int hour1 = Convert.ToInt32(time2.Split(':')[0]);
                        min1 = (min1 + Convert.ToInt32(dtOrderDetail.Rows[dgOrderDetail.SelectedIndex]["stop_time"])) % 60;
                        hour1 = hour1 +
                                (Convert.ToInt32(time2.Split(':')[1]) +
                                 Convert.ToInt32(dtOrderDetail.Rows[dgOrderDetail.SelectedIndex]["stop_time"])) / 60;
                        hour1 = hour1 > 23 ? hour1 - 24 : hour1;
                        dtOrderDetail.Rows[dgOrderDetail.SelectedIndex]["driving_time"] = (hour1 > 9
                            ? hour1.ToString()
                            : "0" + hour1) + ":" + (min1 > 9 ? min1.ToString() : "0" + min1);
                    }
                    int shijian =
                        Convert.ToInt32(
                            (Convert.ToDouble(JuLi) / (Convert.ToDouble(SuDu) / Convert.ToDouble(60))).ToString("0"));
                    //根据距离和速度计算出时间
                    string time = dtOrderDetail.Rows[i]["driving_time"].ToString().Trim(); //获取输入的开车时间
                    int hour = Convert.ToInt32(time.Split(':')[0]); //截取时间中的小时
                    int min = Convert.ToInt32(time.Split(':')[1]); //截取时间中的分钟
                    hour = hour + (shijian + JuLi / 4) / 60; //按照路况进行每公里耽误15秒来计算
                    hour = hour > 23 ? hour - 24 : hour;
                    shijian = (shijian + JuLi / 4) % 60;
                    hour = (min + shijian) > 59 ? hour + 1 : hour;
                    hour = hour == 24 ? 00 : hour;
                    min = min + shijian > 59 ? min + shijian - 60 : min + shijian;
                    dtOrderDetail.Rows[i + 1]["arrival_time"] = (hour < 10 ? "0" + hour : hour.ToString()) + ":" +
                                                                (min < 10 ? "0" + min : min.ToString());
                    string time1 = dtOrderDetail.Rows[i + 1]["arrival_time"].ToString().Trim();
                    string strstop_time = dtOrderDetail.Rows[i + 1]["stop_time"].ToString().Trim();
                    int stop_time = 5;
                    if (strstop_time != "")
                    {
                        stop_time = Convert.ToInt32(strstop_time);
                    }
                    hour = Convert.ToInt32(time1.Split(':')[0]);
                    hour = min + stop_time > 59 ? hour + 1 : hour;
                    min = min + stop_time > 59 ? min + stop_time - 60 : min + stop_time;
                    hour = hour == 24 ? 00 : hour;
                    if (i + 1 < dtOrderDetail.Rows.Count - 1)
                    {
                        dtOrderDetail.Rows[i + 1]["driving_time"] = (hour < 10 ? "0" + hour : hour.ToString()) + ":" +
                                                                    (min < 10 ? "0" + min : min.ToString());
                    }
                }
            }
            TimeCountdtOrderDetail();
        }
        public void TimeCountdtOrderDetail()
        {
            int mincount = 0; //分钟计算总和
            int hourcount = 0; //小时计算总和
            string[] str; //获取开车时间
            string[] nextstr; //获取下一行的到达时间
            string[] currstr; //获取到达时间
            int stop_time = 0; //停留时间
            int stop_timeCount = 0; //停留时间总和
            int dtOrderDetailhour = 0; //运行小时
            int dtOrderDetailmin = 0; //运行分钟
            for (int i = 0; i < dtOrderDetail.Rows.Count; i++)
            {
                str = dtOrderDetail.Rows[i]["driving_time"].ToString().Trim().Split(':'); //截取开车时间
                if (i > 0 && i < dtOrderDetail.Rows.Count - 1)
                {
                    currstr = dtOrderDetail.Rows[i]["arrival_time"].ToString().Trim().Split(':'); //截取到达时间
                    stop_time = Convert.ToInt32(str[1]) - Convert.ToInt32(currstr[1]); //开车时间-到达时间
                    if (stop_time <= 0)
                    {
                        stop_time = 60 + stop_time;
                    }
                    stop_timeCount += stop_time;
                    dtOrderDetail.Rows[i]["stop_time"] = stop_time;
                }
                else
                {
                    dtOrderDetail.Rows[i]["stop_time"] = 0;
                }
                if (i < dtOrderDetail.Rows.Count - 1)
                {
                    nextstr = dtOrderDetail.Rows[i + 1]["arrival_time"].ToString().Trim().Split(':');
                    int k = Convert.ToInt32(nextstr[0]) - Convert.ToInt32(str[0]);
                    if (k < 0)
                    {
                        hourcount += 24 + k;
                    }
                    else
                    {
                        hourcount += k;
                    }
                    int j = Convert.ToInt32(nextstr[1]) - Convert.ToInt32(str[1]);
                    if (j <= 0)
                    {
                        mincount += 60 + j;
                        hourcount--;
                    }
                    else
                    {
                        mincount += j;
                    }
                    hourcount += ((j <= 0 ? (60 + j) : j) + dtOrderDetailmin + stop_time) / 60;
                    dtOrderDetailmin = (stop_timeCount + mincount) % 60;
                    dtOrderDetail.Rows[i + 1]["using_time"]
                        = (hourcount > 9 ? hourcount.ToString() : "0" + hourcount) +
                          "小时" + (dtOrderDetailmin > 9 ? dtOrderDetailmin.ToString() : "0" + dtOrderDetailmin) + "分";
                }
            }
        }
        #endregion
        #region dgOrderDetail_SelectedCellsChanged事件
        private void dgOrderDetail_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dgc != null && dgc.Column.IsReadOnly == false) //若当前单元格不为空且当前单元格可编辑
                {
                    dgc.Column.IsReadOnly = true; //设置当前单元格只读，即不可编辑
                }
            }
            catch
            {
            }
        }
        #endregion
        #region dgOrderDetail_CellEditEnding事件
        private void dgOrderDetail_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                string header = dgOrderDetail.CurrentColumn.Header.ToString().Trim();
                if (header == "开车时间" || header == "停留时间" || header == "站序" || header == "车站" || header == "里程" ||
                    header == "到达时间" || header == "运行时间" || header == "天数")
                {
                    btn_Affirm.Focus();
                    //CellEditEnding();
                    TimeCount(dgOrderDetail.SelectedIndex);
                    int sex = dgOrderDetail.Items.Count;
                    txt_day.Text = ((DataRowView)dgOrderDetail.Items[sex - 1]).Row["using_time"].ToString();
                }
            }
            catch
            {
            }
        }
        #endregion
        #region  dgOrderDetail_MouseLeftButtonDown事件
        int intIndex;
        int oldstop_time;
        DataGridCellInfo dgc;
        private void dgOrderDetail_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (dgOrderDetail.CurrentCell.Column.Header.ToString() == "开车时间") //若点击单元格列标题为开车时间
                {
                    intIndex = dgOrderDetail.SelectedIndex; //获取当前选择单元格索引值
                    if (dgOrderDetail.SelectedIndex == 0) //若索引值为0，即第一行
                    {
                        if (txt_line.Text == "")
                        {
                            dgOrderDetail.CurrentCell.Column.IsReadOnly = true; //设置当前单元格只读，即不可编辑
                            MessageBox.Show("请选择线路");
                        }
                        else
                        {
                            dgOrderDetail.CurrentCell.Column.IsReadOnly = false; //设置当前单元格可编辑
                        }
                    }
                    else //若不为0
                    {
                        dgOrderDetail.CurrentCell.Column.IsReadOnly = true; //设置当前单元格只读，即不可编辑
                    }
                }
                else if (dgOrderDetail.CurrentCell.Column.Header.ToString() == "停留时间") //若点击单元格列标题为停留时间
                {
                    intIndex = dgOrderDetail.SelectedIndex; //获取当前选择单元格索引值
                    if (dgOrderDetail.SelectedIndex == 0 || dgOrderDetail.SelectedIndex == dtOrderDetail.Rows.Count - 1)
                    //若索引值为0（即第一行）或索引值为最后一行
                    {
                        dgOrderDetail.CurrentCell.Column.IsReadOnly = true; //设置当前单元格只读，即不可编辑
                    }
                    else if (dtOrderDetail.Rows[intIndex]["stop_time"].ToString() != "") //若不为第一行或最后一行，且当前索引停车时间不为空
                    {
                        dgOrderDetail.CurrentCell.Column.IsReadOnly = false; //设置当前单元格可编辑
                    }
                    oldstop_time = Convert.ToInt32(dtOrderDetail.Rows[intIndex]["stop_time"]); //获取当前停车时间
                }
                dgc = dgOrderDetail.CurrentCell; //获取当前单元格
            }
            catch
            {
            }
        }
        #endregion
        #region dgOrderDetail_BeginningEdit事件
        private void dgOrderDetail_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (dtOrderDetail.Rows[intIndex]["driving_time"].ToString() == "请双击输入")
            {
                dtOrderDetail.Rows[intIndex]["driving_time"] = "";
            }
        }
        #endregion
        #region dgTrain_MouseLeftButtonDown事件
        private void dgTrain_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Convert.ToBoolean(dtTrain.Rows[0]["chked"]) == false)
            {
                dtTrain.Rows[0]["chked"] = true;

            }
        }
        #endregion
        #region btn_ChoiceTrain_Click事件
        private void btn_ChoiceTrain_Click(object sender, RoutedEventArgs e)
        {
            WD_ChoiceTrain myWD_ChoiceTrain = new WD_ChoiceTrain();
            myWD_ChoiceTrain.ShowDialog();
            try
            {
                intTrainId = Convert.ToInt32(WD_ChoiceTrain.DRV.Row["train_id"].ToString());//车辆ID
                //int intOrderId = Convert.ToInt32(WD_ChoiceTrain.DRV.Row["order_id"].ToString());//车次ID
                int intTypeId = Convert.ToInt32(WD_ChoiceTrain.DRV.Row["as_type_id"].ToString());//车辆类型ID
                txt_TrainNumber.Text = WD_ChoiceTrain.DRV.Row["train_number"].ToString();//车牌编号                
                txt_TrainType.Text= WD_ChoiceTrain.DRV.Row["train_type"].ToString();//车辆类型
                txt_rate.Text = WD_ChoiceTrain.DRV.Row["rate"].ToString();//车辆速度
                // dgTrain.ItemsSource = WD_ChoiceTrain.drv.DataView;
            }
            catch
            {
            }
        }
        #endregion        
        #region  保存按钮事件
        private void btn_Affirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_day.Text != "" && txt_disdence.Text != "" && txt_line.Text != "" && txt_OrderNumber.Text != "" &&
                txt_rate.Text != "")
                {
                    int intline_id = Convert.ToInt32(LineManage.WD_ChoiceLine.drv.Row["line_id"]);
                    string strorder_number = txt_OrderNumber.Text.ToString().Trim();
                    string strhours = txt_day.Text.ToString().Trim();
                    string strdistance = txt_disdence.Text.ToString().Trim();
                    DataTable dtOrder =
                        myClient.UserControl_Loaded_InsertOrder(intline_id, strorder_number, strhours,
                            strdistance).Tables[0];
                    int order_id = Convert.ToInt32(dtOrder.Rows[0][0]);
                    myClient.UserControl_Loaded_UpdateTrainOrderID(intTrainId, order_id);
                    for (int i = 0; i < dgOrderDetail.Items.Count; i++)
                    {
                        int as_train_type = Convert.ToInt32(WD_ChoiceTrain.DRV.Row["as_type_id"]);
                        int station_id = Convert.ToInt32(((DataRowView)dgOrderDetail.Items[i]).Row["site_id"]);
                        string station_order = ((DataRowView)dgOrderDetail.Items[i]).Row["number"].ToString();
                        string day = ((DataRowView)dgOrderDetail.Items[i]).Row["day"].ToString();
                        string arrival_time = ((DataRowView)dgOrderDetail.Items[i]).Row["arrival_time"].ToString();
                        string driving_time = ((DataRowView)dgOrderDetail.Items[i]).Row["driving_time"].ToString();
                        string using_time = ((DataRowView)dgOrderDetail.Items[i]).Row["using_time"].ToString();
                        string mileage = ((DataRowView)dgOrderDetail.Items[i]).Row["distance"].ToString();
                        DataTable resultOrderDetail =
                            myClient.UserControl_Loaded_InsertOrderDetail(order_id, as_train_type, station_id,
                                station_order,
                                day, arrival_time, driving_time, using_time, mileage).Tables[0];
                        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient =
                            new BLL.PublicFunction.PublicFunctionClient();
                        myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63,
                            "新增车次【" + strorder_number + "】", DateTime.Now);
                    }
                    MessageBoxResult dr = MessageBox.Show("新增车次成功！", "系统提示", MessageBoxButton.OKCancel,
                        MessageBoxImage.Information); //弹出确定对话框
                    if (dr == MessageBoxResult.OK) //如果点了确定按钮
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("请把页面数据填写完整！", "系统提示", MessageBoxButton.OK,
                        MessageBoxImage.Stop); //弹出确定对话框
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());                
            }

        }
        #endregion
        //重置
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txt_day.Text = string.Empty;
            txt_disdence.Text = string.Empty;
            txt_line.Text = string.Empty;
            txt_OrderNumber.Text = string.Empty;
            txt_rate.Text = string.Empty;
            txt_TrainNumber.Text = string.Empty;
            txt_TrainType.Text = string.Empty;
            dgOrderDetail.ItemsSource = null;

        }
    }
}
