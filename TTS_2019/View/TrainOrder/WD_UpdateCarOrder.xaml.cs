using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TTS_2019.View.TrainOrder
{
    /// <summary>
    /// WD_UpdateCarOrder.xaml 的交互逻辑
    /// </summary>
    public partial class WD_UpdateCarOrder : Window
    {
        DataRowView DGVR;
        public WD_UpdateCarOrder(DataRowView dgvr)
        {
            InitializeComponent();
            DGVR = dgvr;
        }
        BLL.UC_CarOrder.UC_CarOrderClient myClient = new BLL.UC_CarOrder.UC_CarOrderClient();
        BLL.UC_CreateLine.UC_CreateLineClient myLineClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        int orderID = 0;
        int LineID=0;
        int intTypeId = 0;
        int intTrainId=0;  
        DataTable dtTrain;//车辆信息
        DataTable dtOrderDetail;//车次明细
        DataTable dtDetai;
        #region 1 事件
        //1.1 加载
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 绑定数据    
            orderID = Convert.ToInt32(DGVR.Row["order_id"]);          
            LineID = Convert.ToInt32(DGVR.Row["line_id"]);
            txt_OrderNumber.Text = (DGVR.Row["order_number"]).ToString();
            txt_disdence.Text = (DGVR.Row["distance"]).ToString();
            txt_line.Text = (DGVR.Row["line"]).ToString();
            txt_day.Text = (DGVR.Row["hours"]).ToString(); 
            DataTable dtTrain = myClient.UserControl_Loaded_SelectTrainByOrderID(orderID).Tables[0];//调用查询方法，把返回的值，赋给DGV
            intTrainId = Convert.ToInt32(dtTrain.Rows[0]["train_id"]);//车辆ID
            intTypeId = Convert.ToInt32(dtTrain.Rows[0]["as_type_id"]);//车辆类型ID
            txt_TrainNumber.Text = dtTrain.Rows[0]["train_number"].ToString();
            txt_TrainType.Text = dtTrain.Rows[0]["train_type"].ToString();
            txt_rate.Text = dtTrain.Rows[0]["rate"].ToString(); 
            #region  绑定站点信息
            DataTable dtOderDetail = myClient.UserControl_Loaded_SelectOrderDetail(orderID).Tables[0];
            dgOrderDetail.ItemsSource = dtOderDetail.DefaultView;
            #endregion
            #endregion
        }
        //1.2 页面重置
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            #region 绑定数据    
            orderID = Convert.ToInt32(DGVR.Row["order_id"]);
            LineID = Convert.ToInt32(DGVR.Row["line_id"]);
            txt_OrderNumber.Text = (DGVR.Row["order_number"]).ToString();
            txt_disdence.Text = (DGVR.Row["distance"]).ToString();
            txt_line.Text = (DGVR.Row["line"]).ToString();
            txt_day.Text = (DGVR.Row["hours"]).ToString();
            DataTable dtTrain = myClient.UserControl_Loaded_SelectTrainByOrderID(orderID).Tables[0];//调用查询方法，把返回的值，赋给DGV
            intTrainId = Convert.ToInt32(dtTrain.Rows[0]["train_id"]);//车辆ID
            intTypeId = Convert.ToInt32(dtTrain.Rows[0]["as_type_id"]);//车辆类型ID
            txt_TrainNumber.Text = dtTrain.Rows[0]["train_number"].ToString();
            txt_TrainType.Text = dtTrain.Rows[0]["train_type"].ToString();
            txt_rate.Text = dtTrain.Rows[0]["rate"].ToString();
            #region  绑定站点信息
            DataTable dtOderDetail = myClient.UserControl_Loaded_SelectOrderDetail(orderID).Tables[0];
            dgOrderDetail.ItemsSource = dtOderDetail.DefaultView;
            #endregion
            #endregion
        }
        //1.3 保存
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (orderID!=0 && LineID != 0 && intTrainId!=0 && txt_day.Text != "" && txt_disdence.Text != "" && txt_line.Text != "" && txt_OrderNumber.Text != "" &&
                txt_rate.Text != "")
                {
                    //第一步：删除原来的车次明细 
                    myClient.UserControl_Loaded_DeleteOrderDetail(orderID);                    
                    string strorder_number = txt_OrderNumber.Text.ToString().Trim();
                    string strhours = txt_day.Text.ToString().Trim();
                    string strdistance = txt_disdence.Text.ToString().Trim();
                    //第二步：修改车次信息
                    DataTable dtOrder = myClient.UserControl_Loaded_UpdateOrder(LineID, strorder_number, strhours, strdistance, orderID).Tables[0];                   
                    //第三步：通过车辆ID修改车辆里的车次ID
                    myClient.UserControl_Loaded_UpdateTrainOrderID(intTrainId, orderID);
                    for (int i = 0; i < dgOrderDetail.Items.Count; i++)
                    {                        
                        int station_id = Convert.ToInt32(((DataRowView)dgOrderDetail.Items[i]).Row["site_id"]);
                        string station_order = ((DataRowView)dgOrderDetail.Items[i]).Row["number"].ToString();
                        string day = ((DataRowView)dgOrderDetail.Items[i]).Row["day"].ToString();
                        string arrival_time = ((DataRowView)dgOrderDetail.Items[i]).Row["arrival_time"].ToString();
                        string driving_time = ((DataRowView)dgOrderDetail.Items[i]).Row["driving_time"].ToString();
                        string using_time = ((DataRowView)dgOrderDetail.Items[i]).Row["using_time"].ToString();
                        string mileage = ((DataRowView)dgOrderDetail.Items[i]).Row["distance"].ToString();
                        DataTable resultOrderDetail =
                            myClient.UserControl_Loaded_InsertOrderDetail(orderID, intTypeId, station_id,
                                station_order,
                                day, arrival_time, driving_time, using_time, mileage).Tables[0];
                        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient =
                            new BLL.PublicFunction.PublicFunctionClient();
                        myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63,
                            "修改车次【" + strorder_number + "】", DateTime.Now);
                    }
                    MessageBoxResult dr = MessageBox.Show("修改车次成功！", "系统提示", MessageBoxButton.OKCancel,
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
        //1.4 关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
        //1.5 先选车辆
        private void btn_ChoiceTrain_Click(object sender, RoutedEventArgs e)
        {
            WD_ChoiceTrain myWD_ChoiceTrain = new WD_ChoiceTrain();
            myWD_ChoiceTrain.ShowDialog();
            try
            {               
                intTrainId = Convert.ToInt32(WD_ChoiceTrain.DRV.Row["train_id"].ToString());//车辆ID
                //int intOrderId = Convert.ToInt32(WD_ChoiceTrain.DRV.Row["order_id"].ToString());//车次ID
                intTypeId = Convert.ToInt32(WD_ChoiceTrain.DRV.Row["as_type_id"].ToString());//车辆类型ID
                txt_TrainNumber.Text = WD_ChoiceTrain.DRV.Row["train_number"].ToString();//车牌编号                
                txt_TrainType.Text = WD_ChoiceTrain.DRV.Row["train_type"].ToString();//车辆类型
                txt_rate.Text = WD_ChoiceTrain.DRV.Row["rate"].ToString();//车辆速度

            }
            catch { }
        }
        //1.6 选择线路
        private void btn_ChoiceLine_Click(object sender, RoutedEventArgs e)
        {
            LineManage.WD_ChoiceLine myWD_ChoiceLine = new LineManage.WD_ChoiceLine();
            myWD_ChoiceLine.ShowDialog();
            try
            {
                txt_line.Text = LineManage.WD_ChoiceLine.drv.Row["line_name"].ToString();
                txt_disdence.Text = LineManage.WD_ChoiceLine.drv.Row["mileage"].ToString() + "公里";
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
                    if (dgOrderDetail.Items.Count != 0)//若返回的值不为0,即有值
                    {
                        dtOrderDetail.Rows[0]["driving_time"] = "请双击输入";//为第一行开车时间赋初始值

                    }
                }
            }
            catch
            {
                MessageBox.Show("选择线路失败！");
            }
        }
        #endregion

        private void dgOrderDetail_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dgc != null && dgc.Column.IsReadOnly == false)//若当前单元格不为空且当前单元格可编辑
                {
                    dgc.Column.IsReadOnly = true;//设置当前单元格只读，即不可编辑
                }
            }
            catch { }
        }

        private void dgOrderDetail_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            try
            {
                string header = dgOrderDetail.CurrentColumn.Header.ToString().Trim();
                if (header == "开车时间" || header == "停留时间" || header == "站序" || header == "车站" || header == "里程" || header == "到达时间" || header == "运行时间" || header == "天数")
                {
                    btnSave.Focus();
                    TimeCount(dgOrderDetail.SelectedIndex);
                    int sex = dgOrderDetail.Items.Count;
                    txt_day.Text = ((DataRowView)dgOrderDetail.Items[sex - 1]).Row["using_time"].ToString();
                }
            }
            catch { }
        }
        int intIndex;
        int oldstop_time;
        DataGridCellInfo dgc;
        private void dgOrderDetail_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (dgOrderDetail.CurrentCell.Column.Header.ToString() == "开车时间")//若点击单元格列标题为开车时间
                {
                    intIndex = dgOrderDetail.SelectedIndex;//获取当前选择单元格索引值
                    if (dgOrderDetail.SelectedIndex == 0)//若索引值为0，即第一行
                    {
                        if (txt_line.Text == "")
                        {
                            dgOrderDetail.CurrentCell.Column.IsReadOnly = true;//设置当前单元格只读，即不可编辑
                            MessageBox.Show("请选择线路");

                        }
                        else
                        {
                            dgOrderDetail.CurrentCell.Column.IsReadOnly = false;//设置当前单元格可编辑
                        }
                    }
                    else//若不为0
                    {
                        dgOrderDetail.CurrentCell.Column.IsReadOnly = true;//设置当前单元格只读，即不可编辑
                    }
                }
                else if (dgOrderDetail.CurrentCell.Column.Header.ToString() == "停留时间")//若点击单元格列标题为停留时间
                {
                    intIndex = dgOrderDetail.SelectedIndex;//获取当前选择单元格索引值
                    if (dgOrderDetail.SelectedIndex == 0 || dgOrderDetail.SelectedIndex == dtOrderDetail.Rows.Count - 1)//若索引值为0（即第一行）或索引值为最后一行
                    {
                        dgOrderDetail.CurrentCell.Column.IsReadOnly = true;//设置当前单元格只读，即不可编辑
                    }
                    else if (dtOrderDetail.Rows[intIndex]["stop_time"].ToString() != "")//若不为第一行或最后一行，且当前索引停车时间不为空
                    {
                        dgOrderDetail.CurrentCell.Column.IsReadOnly = false;//设置当前单元格可编辑
                    }
                    oldstop_time = Convert.ToInt32(dtOrderDetail.Rows[intIndex]["stop_time"]);//获取当前停车时间
                }
                dgc = dgOrderDetail.CurrentCell;//获取当前单元格
            }
            catch
            {

            }
        }

        private void dgOrderDetail_BeginningEdit(object sender, System.Windows.Controls.DataGridBeginningEditEventArgs e)
        {
            if (dtOrderDetail.Rows[intIndex]["driving_time"].ToString() == "请双击输入")
            {
                dtOrderDetail.Rows[intIndex]["driving_time"] = "";
            }
        }
        #region 计算时间事件
        string SuDu;
        private void TimeCount(int Index)
        {

            for (int i = Index; i < dtOrderDetail.Rows.Count; i++)
            {
                if (i < dtOrderDetail.Rows.Count - 1)
                {
                    int JuLi = Convert.ToInt32(dtOrderDetail.Rows[i + 1]["distance"]);//计算两站之间的距离
                    SuDu = txt_rate.Text.ToString().Trim();
                    if (dtOrderDetail.Rows[i]["stop_time"].ToString().Trim() != "")
                    {
                        string time2 = dtOrderDetail.Rows[dgOrderDetail.SelectedIndex]["arrival_time"].ToString()
                                                  .Trim();
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
                    int shijian = Convert.ToInt32((Convert.ToDouble(JuLi) / (Convert.ToDouble(SuDu) / Convert.ToDouble(60))).ToString("0"));//根据距离和速度计算出时间
                    string time = dtOrderDetail.Rows[i]["driving_time"].ToString().Trim();//获取输入的开车时间
                    int hour = Convert.ToInt32(time.Split(':')[0]);//截取时间中的小时
                    int min = Convert.ToInt32(time.Split(':')[1]);//截取时间中的分钟
                    hour = hour + (shijian + JuLi / 4) / 60;//按照路况进行每公里耽误15秒来计算
                    hour = hour > 23 ? hour - 24 : hour;
                    shijian = (shijian + JuLi / 4) % 60;
                    hour = (min + shijian) > 59 ? hour + 1 : hour;
                    hour = hour == 24 ? 00 : hour;
                    min = min + shijian > 59 ? min + shijian - 60 : min + shijian;
                    dtOrderDetail.Rows[i + 1]["arrival_time"] = (hour < 10 ? "0" + hour : hour.ToString()) + ":" + (min < 10 ? "0" + min : min.ToString());
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
                        dtOrderDetail.Rows[i + 1]["driving_time"] = (hour < 10 ? "0" + hour : hour.ToString()) + ":" + (min < 10 ? "0" + min : min.ToString());
                    }
                }
            }
            TimeCountdtOrderDetail();
        }
        public void TimeCountdtOrderDetail()
        {
            int mincount = 0;//分钟计算总和
            int hourcount = 0;//小时计算总和
            string[] str;//获取开车时间
            string[] nextstr;//获取下一行的到达时间
            string[] currstr;//获取到达时间
            int stop_time = 0;//停留时间
            int stop_timeCount = 0;//停留时间总和
            int dtOrderDetailhour = 0;//运行小时
            int dtOrderDetailmin = 0;//运行分钟
            for (int i = 0; i < dtOrderDetail.Rows.Count; i++)
            {
                str = dtOrderDetail.Rows[i]["driving_time"].ToString().Trim().Split(':');//截取开车时间
                if (i > 0 && i < dtOrderDetail.Rows.Count - 1)
                {
                    currstr = dtOrderDetail.Rows[i]["arrival_time"].ToString().Trim().Split(':');//截取到达时间
                    stop_time = Convert.ToInt32(str[1]) - Convert.ToInt32(currstr[1]);//开车时间-到达时间
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
                    dtOrderDetail.Rows[i + 1]["using_time"] = (hourcount > 9 ? hourcount.ToString() : "0" + hourcount) + "小时" + (dtOrderDetailmin > 9 ? dtOrderDetailmin.ToString() : "0" + dtOrderDetailmin) + "分";
                }
            }
        }
        #endregion
    }
}
