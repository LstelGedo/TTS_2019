using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TTS_2019.View.TrainOrder
{
    /// <summary>
    /// WD_InsertCarInformation.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertCarInformation : Window
    {
        public WD_InsertCarInformation()
        {
            InitializeComponent();
        }
        BLL.UC_Compartment.UC_CompartmentClient myClient = new BLL.UC_Compartment.UC_CompartmentClient();

        int inttrain_id = 0;
        int intcar_id = 0;
        public DataTable dg;
        int rowIndex;
        int seatRowIndex;
        DataTable dgSite;
        #region TrainOrder_Loaded事件
        private void TrainOrder_Loaded(object sender, RoutedEventArgs e)
        {            
            #region 绑定车辆类型
            DataTable dt = myClient.UserControl_Loaded_SelectTrainType().Tables[0];
            cbo_TrainType.ItemsSource = dt.DefaultView;
            cbo_TrainType.DisplayMemberPath = "TrainType";
            cbo_TrainType.SelectedValuePath = "TrainTypeID";
            #endregion
            #region 绑定车厢类型
            DataTable dtCompartment = myClient.UserControl_Loaded_SelectCompartmentType().Tables[0];
            CarType.ItemsSource = dtCompartment.DefaultView;
            CarType.DisplayMemberPath = "CarType";
            CarType.SelectedValuePath = "CarTypeID";
            #endregion           
            #region 列车负责人
            DataTable dtprincipal = myClient.UserControl_Loaded_Selectprincipal().Tables[0];
            cbo_principal.ItemsSource = dtprincipal.DefaultView;
            cbo_principal.DisplayMemberPath = "principal";
            cbo_principal.SelectedValuePath = "staff_id";
            #endregion
            #region 紧急联系人
            DataTable dtlinkman = myClient.UserControl_Loaded_Selectlinkman().Tables[0];
            cbo_linkman.ItemsSource = dtlinkman.DefaultView;
            cbo_linkman.DisplayMemberPath = "linkman";
            cbo_linkman.SelectedValuePath = "staff_id";
            #endregion
        }
        #endregion
        #region 保存按钮
        private void btn_Affirm_Click(object sender, RoutedEventArgs e)
        {
            rowIndex = -1;
            byte[][] bytWenJian = new byte[lstBytes.Count][];
            for (int i = 0; i < lstBytes.Count; i++)
            {
                bytWenJian[i] = lstBytes[i];
            }
            if (cbo_TrainType.SelectedValue.ToString() != "" && cbo_principal.SelectedValue.ToString() != "" && cbo_linkman.SelectedValue.ToString() != ""
                && txt_train_number.Text != "" && txt_bottom_number.Text != "" && txt_BiggestNumber.Text != "" && txt_Remark.Text != "")
            {
                int intas_type_id = Convert.ToInt32(cbo_TrainType.SelectedValue);
                int intprincipal_id = Convert.ToInt32(cbo_principal.SelectedValue);
                int intlinkman_id = Convert.ToInt32(cbo_linkman.SelectedValue);
                string strtrain_number = txt_train_number.Text.ToString();
                string strbottom_number = txt_bottom_number.Text.ToString();
                DateTime dtproduction_date = DateTime.Now;
                DateTime dtexpiration_date = DateTime.Now;
                string strbiggest_pitch = txt_BiggestNumber.Text.ToString();
                bool blusing_no = false;
                bool bldisable = false;
                string strnote = txt_Remark.Text.ToString();
                string strrate = txt_Rate.Text.ToString().Trim();
                DataTable dtTrain = myClient.UserControl_Loaded_InsertTrain(intas_type_id, intprincipal_id, intlinkman_id, strtrain_number,
                  strbottom_number, dtproduction_date, dtexpiration_date, strbiggest_pitch, bytWenJian, blusing_no, bldisable, strnote, strrate).Tables[0];
                inttrain_id = Convert.ToInt32(dtTrain.Rows[0][0].ToString());
            }
            if (inttrain_id > 0)
            {
                for (int i = 0; i < dg.Rows.Count; i++)
                {
                    KaiGuan = true;
                    int intas_car_type_id = Convert.ToInt32(dg.Rows[i]["CarTypeID"]);
                    string strcar_number = dg.Rows[i]["car_number"].ToString();
                    string seat_number = dg.Rows[i]["seat_number"].ToString();
                    string strnote = ((DataRowView)dgCompartment.Items[i]).Row["note"].ToString();
                    DataTable dts = InsertSeat(intas_car_type_id.ToString(), seat_number.ToString());
                    DataTable dtCar = myClient.UserControl_Loaded_InsertCar(intas_car_type_id, strcar_number, strnote, inttrain_id).Tables[0];
                    intcar_id = Convert.ToInt32(dtCar.Rows[0][0].ToString());
                    if (intcar_id > 0)
                    {
                        for (int j = 0; j < dts.Rows.Count; j++)
                        {
                            string strseat_number = dts.Rows[j]["seat_number"].ToString();
                            string strseatnote = dts.Rows[j]["seat_type"].ToString();
                            int count = myClient.UserControl_Loaded_InsertCarSeat(intcar_id, intas_car_type_id, strseat_number, strseatnote);
                            if (count > 0)
                            {
                                if (i == dg.Rows.Count - 1 && j == dts.Rows.Count - 1)
                                {
                                    MessageBoxResult dr = MessageBox.Show("新增车辆成功！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);//弹出确定对话框
                                    if (dr == MessageBoxResult.OK)//如果点了确定按钮
                                    {
                                        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
                                        myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63, "新增车辆【" + strcar_number + "】", DateTime.Now);
                                        this.Close();
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("新增车辆失败");
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("新增车辆失败");
                    }


                }
            }

        }
        #endregion
        #region 退出按钮
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
        #endregion       
        #region 加载图片       
        List<string> lisWenJianMing = new List<string>();
        List<byte[]> lstBytes = new List<byte[]>();
        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            Stream phpto = null;
            int length;
            Microsoft.Win32.OpenFileDialog ofdWenJian = new Microsoft.Win32.OpenFileDialog();
            ofdWenJian.Multiselect = true;//多选图片
            ofdWenJian.Filter = "ALL Image Files|*.*";
            if ((bool)ofdWenJian.ShowDialog())
            {
                if ((phpto = ofdWenJian.OpenFile()) != null)
                {
                    DockPanel dp = new DockPanel();
                    for (int i = 0; i < ofdWenJian.FileNames.Length; i++)
                    {
                        /////////加载图片返回bytes数组到服务端。
                        phpto = new FileStream(ofdWenJian.FileNames[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        length = (int)phpto.Length;//获取图片的长度
                        byte[] bytes = new byte[length];
                        phpto.Read(bytes, 0, length);
                        lstBytes.Add(bytes);
                        ////////显示图片
                        BitmapImage images = new BitmapImage(new Uri(ofdWenJian.FileNames[i]));
                        Image photo = new Image();
                        photo.Height = 204;
                        photo.Margin = new Thickness(5, 0, 0, 0);
                        photo.Source = images.Clone();
                        dp.Children.Add(photo);
                        txt_Location.Text += ofdWenJian.FileNames[i] + ";";
                    }
                    img_pictrue.Content = dp;
                }
            }
            else
            {
                MessageBox.Show("加载的图片数据为空！");
            }
        }
        #endregion
        #region btn_Clean_Click事件
        private void btn_Clean_Click(object sender, RoutedEventArgs e)
        {
            img_pictrue.Content = null;
            txt_Location.Text = "";
        }
        #endregion
        #region 新增车厢信息
        private void btn_InsertCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //判断车厢个数是否为空
                if (txt_CompartmentNumber.Text != "")
                {
                    //创建临时车厢表并构建列
                    dg = new DataTable();
                    dg.Columns.Add("CarTypeID", typeof(int));
                    dg.Columns.Add("CarType", typeof(string));
                    dg.Columns.Add("car_number", typeof(string));
                    dg.Columns.Add("note", typeof(string));
                    dg.Columns.Add("seat_number", typeof(int));
                    dg.Columns.Add("seat_id", typeof(string));
                    int number = Convert.ToInt32(txt_CompartmentNumber.Text);
                    //判断车厢DataGrid是否有数据
                    if (dgCompartment.Items.Count > 0)
                    {
                        //1、（有数据）在原来的数据后面添加数据（索引为行的总数）
                        dg = ((DataView)dgCompartment.ItemsSource).Table;
                        int j = dg.Rows.Count;//获取行的总数
                        for (int i = j; i < j + number; i++)//循环添加车厢数据
                        {
                            dg.Rows.Add();
                            dg.Rows[i]["car_number"] = i + 1;
                        }
                        //绑定表格
                        dgCompartment.ItemsSource = dg.DefaultView;
                    }
                    else
                    {
                        //2、（无数据）（索引从0开始）
                        for (int i = 1; i <= number; i++)
                        {
                            dg.Rows.Add();
                            dg.Rows[i - 1]["car_number"] = i;
                        }
                        dgCompartment.ItemsSource = dg.DefaultView;
                    }
                }
                //清空文本
                txt_CompartmentNumber.Text = "";
            }
            catch
            {
            }
        }
        #endregion
        #region    dgCompartment_MouseUp事件
        private void dgCompartment_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //判断是否选中车厢数据
            if (dgCompartment.SelectedIndex != -1)
            {
                //默认true
                KaiGuan = true;
                //行索引和选中索引不一致，当前选择中第一个项的索引。默认值为 -1。
                if (rowIndex != dgCompartment.SelectedIndex)
                {
                    //清空座位表
                    dgSeat.ItemsSource = null;
                }
                //行索引=选中行索引
                rowIndex = dgCompartment.SelectedIndex;
                //获取选中行提取单元格数据
                DataRowView drv = (DataRowView)dgCompartment.SelectedItem;                
                if (dg.Rows[rowIndex]["seat_number"].ToString() != "" && dg.Rows[rowIndex]["seat_id"].ToString() != "" && dg.Rows[rowIndex]["CarTypeID"].ToString() != "")
                {                   
                    //调用新增座位方法
                    DataTable dt = InsertSeat(drv["seat_id"].ToString(), drv["seat_number"].ToString());
                    //绑定座位表数据
                    dgSeat.ItemsSource = dt.DefaultView;
                }

            }
        }
        #endregion
        #region 按钮新增座位信息
        private void btn_InsertSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((dg.Rows[rowIndex]["CarTypeID"].ToString() != "" && txt_SeatNumber.Text.ToString() != ""))
                {
                    DataTable dt = InsertSeat(dg.Rows[rowIndex]["CarTypeID"].ToString(), txt_SeatNumber.Text.Trim());
                    //获取第一次的座位数（调用新增座位方法）
                    dgSeat.ItemsSource = dt.DefaultView;
                }
            }
            catch
            {
            }

            txt_SeatNumber.Text = "";
        }
        #endregion
        #region 新增座位方法
        public DataTable InsertSeat(string SeatType, string SeatNum)
        {
            //创建临时表
            dgSite = new DataTable();
            //判断传递数据是否为空
            if (SeatType != "" && SeatNum != "")
            {
                //创建列
                dgSite.Columns.Add("seat_type", typeof(string));
                dgSite.Columns.Add("seat_number", typeof(string));
                dgSite.Columns.Add("note", typeof(string));
                //判断座位表是否有数据
                if (dgSeat.Items.Count > 0 && KaiGuan == false)
                {
                    //1、（有数据）在原来的数据后面添加数据（索引为行的总数）
                    dgSite = ((DataView)dgSeat.ItemsSource).Table;
                    int j = dgSite.Rows.Count;
                    dg.Rows[rowIndex]["seat_number"] = Convert.ToInt32(SeatNum) + j;
                    dg.Rows[rowIndex]["seat_id"] = SeatType;
                    for (int i = j; i < j + Convert.ToInt32(SeatNum); i++) //根据座位个数时时生成座位
                    {
                        dgSite.Rows.Add();
                        if (Convert.ToInt32(SeatType) == 12)
                        {
                            dgSite.Rows[i]["seat_number"] = "SW" + Convert.ToInt32(i + 1); //商务座
                            dgSite.Rows[i]["seat_type"] = "商务座";
                        }
                        if (Convert.ToInt32(SeatType) == 13)
                        {
                            dgSite.Rows[i]["seat_number"] = "TD" + Convert.ToInt32(i + 1); //特等座
                            dgSite.Rows[i]["seat_type"] = "特等座";
                        }
                        if (Convert.ToInt32(SeatType) == 14)
                        {
                            dgSite.Rows[i]["seat_number"] = "YD" + Convert.ToInt32(i + 1); //一等座
                            dgSite.Rows[i]["seat_type"] = "一等座";
                        }
                        if (Convert.ToInt32(SeatType) == 15)
                        {
                            dgSite.Rows[i]["seat_number"] = "ED" + Convert.ToInt32(i + 1); //二等座
                            dgSite.Rows[i]["seat_type"] = "二等座";
                        }
                        if (Convert.ToInt32(SeatType) == 16)
                        {
                            dgSite.Rows[i]["seat_number"] = "GJRW" + Convert.ToInt32(i + 1); //高级软卧
                            if ((i + 1) % 2 == 0)
                            {
                                dgSite.Rows[i]["seat_type"] = "高级软卧下";
                            }
                            else
                            {
                                dgSite.Rows[i]["seat_type"] = "高级软卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 17)
                        {
                            dgSite.Rows[i]["seat_number"] = "RW" + Convert.ToInt32(i + 1); //软卧
                            if ((i + 1) % 3 == 0)
                            {
                                dgSite.Rows[i]["seat_type"] = "软卧下";
                            }
                            else if (((i + 1) % 3) % 2 == 0)
                            {
                                dgSite.Rows[i]["seat_type"] = "软卧中";
                            }
                            else
                            {
                                dgSite.Rows[i]["seat_type"] = "软卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 18)
                        {
                            dgSite.Rows[i]["seat_number"] = "YW" + Convert.ToInt32(i + 1); //硬卧
                            if ((i + 1) % 3 == 0)
                            {
                                dgSite.Rows[i]["seat_type"] = "硬卧下";
                            }
                            else if (((i + 1) % 3) % 2 == 0)
                            {
                                dgSite.Rows[i]["seat_type"] = "硬卧中";
                            }
                            else
                            {
                                dgSite.Rows[i]["seat_type"] = "硬卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 19)
                        {
                            dgSite.Rows[i]["seat_number"] = "DW" + Convert.ToInt32(i + 1); //动卧
                            if ((i + 1) % 2 == 0)
                            {
                                dgSite.Rows[i]["seat_type"] = "动卧下";
                            }
                            else
                            {
                                dgSite.Rows[i]["seat_type"] = "动卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 20)
                        {
                            dgSite.Rows[i]["seat_number"] = "GJDW" + Convert.ToInt32(i + 1); //高级动卧
                            if ((i + 1) % 2 == 0)
                            {
                                dgSite.Rows[i]["seat_type"] = "高级动卧下";
                            }
                            else
                            {
                                dgSite.Rows[i]["seat_type"] = "高级动卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 21)
                        {
                            dgSite.Rows[i]["seat_number"] = "RZ" + Convert.ToInt32(i + 1); //软座
                            dgSite.Rows[i]["seat_type"] = "软座";
                        }
                        if (Convert.ToInt32(SeatType) == 85)
                        {
                            dgSite.Rows[i]["seat_number"] = "YZ" + Convert.ToInt32(i + 1); //硬座
                            dgSite.Rows[i]["seat_type"] = "硬座";
                        }
                    }
                    return dgSite;
                }
                else
                {
                    if (rowIndex >= 0)
                    {
                        dg.Rows[rowIndex]["seat_number"] = SeatNum;
                        dg.Rows[rowIndex]["seat_id"] = SeatType;
                    }
                    KaiGuan = false;
                    //2、（无数据）（索引从0开始）
                    for (int i = 1; i <= Convert.ToInt32(SeatNum); i++) //根据座位个数时时生成座位
                    {
                        dgSite.Rows.Add();
                        if (Convert.ToInt32(SeatType) == 12)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "SW" + Convert.ToInt32(i); //商务座
                            dgSite.Rows[i - 1]["seat_type"] = "商务座";
                        }
                        if (Convert.ToInt32(SeatType) == 13)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "TD" + Convert.ToInt32(i); //特等座
                            dgSite.Rows[i - 1]["seat_type"] = "特等座";
                        }
                        if (Convert.ToInt32(SeatType) == 14)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "YD" + Convert.ToInt32(i); //一等座
                            dgSite.Rows[i - 1]["seat_type"] = "一等座";
                        }
                        if (Convert.ToInt32(SeatType) == 15)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "ED" + Convert.ToInt32(i); //二等座
                            dgSite.Rows[i - 1]["seat_type"] = "二等座";
                        }
                        if (Convert.ToInt32(SeatType) == 16)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "GJRW" + Convert.ToInt32(i); //高级软卧
                            if (i % 2 == 0)
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "高级软卧下";
                            }
                            else
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "高级软卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 17)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "RW" + Convert.ToInt32(i); //软卧
                            if (i % 3 == 0)
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "软卧下";
                            }
                            else if ((i % 3) % 2 == 0)
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "软卧中";
                            }
                            else
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "软卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 18)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "YW" + Convert.ToInt32(i); //硬卧
                            if (i % 3 == 0)
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "硬卧下";
                            }
                            else if ((i % 3) % 2 == 0)
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "硬卧中";
                            }
                            else
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "硬卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 19)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "DW" + Convert.ToInt32(i); //动卧
                            if (i % 2 == 0)
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "动卧下";
                            }
                            else
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "动卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 20)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "GJDW" + Convert.ToInt32(i); //高级动卧
                            if (i % 2 == 0)
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "高级软卧下";
                            }
                            else
                            {
                                dgSite.Rows[i - 1]["seat_type"] = "高级软卧上";
                            }
                        }
                        if (Convert.ToInt32(SeatType) == 21)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "RZ" + Convert.ToInt32(i); //软座
                            dgSite.Rows[i - 1]["seat_type"] = "软座";
                        }
                        if (Convert.ToInt32(SeatType) == 85)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "YZ" + Convert.ToInt32(i); //硬座
                            dgSite.Rows[i - 1]["seat_type"] = "硬座";
                        }
                    }
                }
            }

            else
            {
                MessageBox.Show("请选择对应的车厢。");
            }
            return dgSite;

        }
        #endregion
        #region 作为编号txt_CompartmentNumber_SelectionChanged事件
        private void txt_CompartmentNumber_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (txt_CompartmentNumber.Text.ToString() != "" && txt_BiggestNumber.Text.ToString() != "")
            {
                try
                {
                    int ShiBianHao = Convert.ToInt32(txt_CompartmentNumber.Text);
                }
                catch
                {
                    txt_CompartmentNumber.Text = "";
                }

                if (Convert.ToInt32(txt_CompartmentNumber.Text) > Convert.ToInt32(txt_BiggestNumber.Text))
                {
                    MessageBox.Show("车厢节数不能大于最大车厢节数！");
                    txt_CompartmentNumber.Text = "";
                }
            }

        }
        #endregion
        #region 最大座位数的txt_BiggestNumber_TextChange事件
        private void txt_BiggestNumber_TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                int ShiBianHao = Convert.ToInt32(txt_BiggestNumber.Text);
            }
            catch
            {
                txt_BiggestNumber.Text = "";
            }
        }
        #endregion
        #region 删除车厢方法
        private void btn_DeleteCar_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                MessageBoxResult dr = MessageBox.Show("是否删除当前车厢？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
                if (dr == MessageBoxResult.OK)//如果点了确定按钮
                {
                    dg.Rows.RemoveAt(rowIndex);
                    dgSeat.ItemsSource = null;
                }
                for (int i = 1; i <= dg.Rows.Count; i++)
                {
                    dg.Rows[i - 1]["car_number"] = i;
                }
                dgCompartment.ItemsSource = dg.DefaultView;
            }
            catch (Exception)
            {
                return;
            }

        }
        #endregion
        #region  删除座位按钮事件
        bool KaiGuan = false;

        private void btn_DeleteSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KaiGuan = true;
                seatRowIndex = dgSeat.SelectedIndex;
                MessageBoxResult dr = MessageBox.Show("是否删除当前座位？", "系统提示", MessageBoxButton.OKCancel,
                    MessageBoxImage.Question); //弹出确定对话框
                if (dr == MessageBoxResult.OK) //如果点了确定按钮
                {
                    dgSite.Rows.RemoveAt(seatRowIndex);
                }
                DataTable dt = InsertSeat(dg.Rows[rowIndex]["seat_id"].ToString(), dgSite.Rows.Count.ToString());
                dgSeat.ItemsSource = dgSite.DefaultView; //重新加载座位信息
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion
    }
}
