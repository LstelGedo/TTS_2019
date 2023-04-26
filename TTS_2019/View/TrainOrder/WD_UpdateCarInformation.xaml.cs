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
    /// WD_UpdateCarInformation.xaml 的交互逻辑
    /// </summary>
    public partial class WD_UpdateCarInformation : Window
    {
        DataRowView DGVR;
        public WD_UpdateCarInformation(DataRowView dgvr)
        {
            DGVR = dgvr;
            InitializeComponent();
        }
        #region 声明全局变量
        BLL.UC_Compartment.UC_CompartmentClient myClient = new BLL.UC_Compartment.UC_CompartmentClient();
        string[] myPicture;
        string strOldLuJing;
        int inttrain_id = 0;
        int intcar_id = 0;
        public DataTable dg;
        int rowIndex;
        int seatRowIndex;
        DataTable dgSite;
        #endregion
        #region Loaded事件
        private void WD_UpdateCarInformation_Loaded(object sender, RoutedEventArgs e)
        {            
            #region 绑定车辆类型
            DataTable dt = myClient.UserControl_Loaded_SelectTrainType().Tables[0];
            cbo_TrainType.ItemsSource = dt.DefaultView;
            cbo_TrainType.DisplayMemberPath = "TrainType";
            cbo_TrainType.SelectedValuePath = "TrainTypeID";
            #endregion
            #region 绑定车厢类型
            DataTable dtCompartmentType = myClient.UserControl_Loaded_SelectCompartmentType().Tables[0];
            CarType.ItemsSource = dtCompartmentType.DefaultView;
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
            txt_BiggestNumber.Text = DGVR["biggest_pitch"].ToString();
            txt_train_number.Text = DGVR["train_number"].ToString();
            txt_bottom_number.Text = DGVR["bottom_number"].ToString();
            txt_Location.Text = DGVR["image_path"].ToString();
            txt_Rate.Text = DGVR["rate"].ToString();
            txt_Remark.Text = DGVR["note"].ToString();
            cbo_linkman.SelectedValue = Convert.ToInt32(DGVR["linkman_id"]);
            cbo_principal.SelectedValue = Convert.ToInt32(DGVR["principal_id"]);
            cbo_TrainType.SelectedValue = Convert.ToInt32(DGVR["as_type_id"]);
            inttrain_id = Convert.ToInt32(DGVR["train_id"]);
            #region 显示图片
            try
            {
                strOldLuJing = (DGVR.Row["image_path"]).ToString();
                DockPanel dp = new DockPanel();//初始化一个DockPanel控件用来装图片
                string[] strLuJingZu = DGVR.Row["image_path"].ToString().Split(';');//获取分割图片路径
                string lujing = DGVR.Row["image_path"].ToString();//获取路径
                myPicture = myClient.UserControl_Loaded_SelectPhoro(lujing);//（方法在服务端）
                for (int i = 1; i < strLuJingZu.Length; i++)//循环图片的个数用以生成DockPanel控件的个数。
                {
                    BitmapImage images = new BitmapImage(new Uri(myPicture[i - 1].ToString()));
                    Image photo = new Image();
                    photo.Height = 204;
                    photo.Margin = new Thickness(5, 0, 0, 0);
                    photo.Source = images.Clone();
                    dp.Children.Add(photo);
                }
                img_pictrue.Content = dp;
            }
            catch (Exception)
            {
                img_pictrue.Content = null;
            }
            #endregion
            #region 车厢个数
            dg = new DataTable();
            dg.Columns.Add("CarID", typeof(int));
            dg.Columns.Add("CarTypeID", typeof(int));
            dg.Columns.Add("CarType", typeof(string));
            dg.Columns.Add("car_number", typeof(string));
            dg.Columns.Add("note", typeof(string));
            dg.Columns.Add("seat_number", typeof(int));
            dg.Columns.Add("seat_id", typeof(string));

            DataTable dtCompartment = myClient.UserControl_Loaded_SelectCompartment(inttrain_id).Tables[0];
            for (int i = 0; i < dtCompartment.Rows.Count; i++)
            {
                dg.Rows.Add();
                dg.Rows[i]["CarID"] = dtCompartment.Rows[i]["car_id"];
                dg.Rows[i]["CarTypeID"] = dtCompartment.Rows[i]["as_car_type_id"];
                dg.Rows[i]["CarType"] = dtCompartment.Rows[i]["car_type"];
                DataTable dtST = myClient.UserControl_Loaded_SelectSeat(Convert.ToInt32(dtCompartment.Rows[i]["car_id"])).Tables[0];
                dg.Rows[i]["seat_id"] = dtCompartment.Rows[i]["as_car_type_id"];/////
                dg.Rows[i]["seat_number"] = dtST.Rows.Count;
                dg.Rows[i]["CarType"] = dtCompartment.Rows[i]["car_type"];
                dg.Rows[i]["car_number"] = dtCompartment.Rows[i]["car_number"];
                dg.Rows[i]["note"] = dtCompartment.Rows[i]["note"];

            }
            dgCompartment.ItemsSource = dg.DefaultView;
            #endregion
        }
        #endregion             
        #region txt_BiggestNumber_TextChange车厢最大数

        private void txt_BiggestNumber_TextChange(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
        #region 浏览文件夹加载图片
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
                        //  BitmapImage images = new BitmapImage(new Uri(ofdWenJian.FileNames[i]));

                        #region 防止线程冲突
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        //增加这一行（指定位图图像如何利用内存缓存=在加载时将整个图像缓存到内存中。对图像数据的所有请求将通过内存存储区进行填充。）                
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(ofdWenJian.FileNames[i]);
                        bi.EndInit();
                        #endregion
                        Image photo = new Image();
                        photo.Height = 204;
                        photo.Margin = new Thickness(5, 0, 0, 0);
                        photo.Source = bi.Clone();
                        dp.Children.Add(photo);
                        txt_Location.Text += ofdWenJian.FileNames[i] + ";";
                    }
                    img_pictrue.Content = dp;
                }
            }
        }
        #endregion
        #region 清空图片
        private void btn_Clean_Click(object sender, RoutedEventArgs e)
        {
            img_pictrue.Content = null;
            txt_Location.Text = "";
        }
        #endregion
        #region dgCompartment_MouseUp事件
        private void dgCompartment_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgCompartment.SelectedIndex != -1)
            {
                KaiGuan = true;
                if (rowIndex != dgCompartment.SelectedIndex)
                {
                    dgSeat.ItemsSource = null;
                }
                rowIndex = dgCompartment.SelectedIndex;
                DataRowView drv = (DataRowView)dgCompartment.SelectedItem;
                if (dg.Rows[rowIndex]["seat_number"].ToString() != "" && dg.Rows[rowIndex]["seat_id"].ToString() != "" && dg.Rows[rowIndex]["CarTypeID"].ToString() != "")
                {
                    DataTable dt = InsertSeat(drv["seat_id"].ToString(), drv["seat_number"].ToString());
                    dgSeat.ItemsSource = dt.DefaultView;
                }
            }
        }
        #endregion
        #region txt_CompartmentNumber_SelectionChanged事件
        private void txt_CompartmentNumber_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }
        #endregion
        #region 按钮新增车厢数
        private void btn_InsertCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txt_CompartmentNumber.Text != "")
                {

                    int number = Convert.ToInt32(txt_CompartmentNumber.Text);
                    if (dgCompartment.Items.Count > 0)
                    {
                        dg = ((DataView)dgCompartment.ItemsSource).Table;
                        int j = dg.Rows.Count;
                        for (int i = j; i < j + number; i++)
                        {
                            dg.Rows.Add();
                            dg.Rows[i]["car_number"] = i + 1;
                        }
                        dgCompartment.ItemsSource = dg.DefaultView;
                    }
                    else
                    {
                        for (int i = 1; i <= number; i++)
                        {
                            dg.Rows.Add();
                            dg.Rows[i - 1]["car_number"] = i;
                        }
                        dgCompartment.ItemsSource = dg.DefaultView;
                    }
                }
                txt_CompartmentNumber.Text = "";
            }
            catch
            {
                return;
            }
        }
        #endregion
        #region  删除车厢个数

        private void btn_DeleteCar_Click(object sender, RoutedEventArgs e)
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
        #endregion
        #region 按钮新增座位数
        private void btn_InsertSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((dg.Rows[rowIndex]["CarTypeID"].ToString() != "" && txt_SeatNumber.Text.ToString() != ""))
                {
                    DataTable dt = InsertSeat(dg.Rows[rowIndex]["CarTypeID"].ToString(), txt_SeatNumber.Text.Trim());//获取第一次的座位数（调用新增座位方法）
                    dgSeat.ItemsSource = dt.DefaultView;
                }
            }
            catch
            {
                return;
            }
            txt_SeatNumber.Text = "";
        }
        #endregion
        #region 新增座位方法
        public DataTable InsertSeat(string SeatType, string SeatNum)
        {
            dgSite = new DataTable();
            if (SeatType != "" && SeatNum != "")
            {
                dgSite.Columns.Add("seat_type", typeof(string));
                dgSite.Columns.Add("seat_number", typeof(string));
                dgSite.Columns.Add("note", typeof(string));
                if (dgSeat.Items.Count > 0 && KaiGuan == false)
                {
                    dgSite = ((DataView)dgSeat.ItemsSource).Table;
                    int j = dgSite.Rows.Count;
                    dg.Rows[rowIndex]["seat_number"] = Convert.ToInt32(SeatNum) + j;
                    dg.Rows[rowIndex]["seat_id"] = SeatType;
                    for (int i = j; i < j + Convert.ToInt32(SeatNum); i++)//根据座位个数时时生成座位
                    {
                        dgSite.Rows.Add();
                        if (Convert.ToInt32(SeatType) == 12)
                        {
                            dgSite.Rows[i]["seat_number"] = "SW" + Convert.ToInt32(i + 1);//商务座
                            dgSite.Rows[i]["seat_type"] = "商务座";
                        }
                        if (Convert.ToInt32(SeatType) == 13)
                        {
                            dgSite.Rows[i]["seat_number"] = "TD" + Convert.ToInt32(i + 1);//特等座
                            dgSite.Rows[i]["seat_type"] = "特等座";
                        }
                        if (Convert.ToInt32(SeatType) == 14)
                        {
                            dgSite.Rows[i]["seat_number"] = "YD" + Convert.ToInt32(i + 1);//一等座
                            dgSite.Rows[i]["seat_type"] = "一等座";
                        }
                        if (Convert.ToInt32(SeatType) == 15)
                        {
                            dgSite.Rows[i]["seat_number"] = "ED" + Convert.ToInt32(i + 1);//二等座
                            dgSite.Rows[i]["seat_type"] = "二等座";
                        }
                        if (Convert.ToInt32(SeatType) == 16)
                        {
                            dgSite.Rows[i]["seat_number"] = "GJRW" + Convert.ToInt32(i + 1);//高级软卧
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
                            dgSite.Rows[i]["seat_number"] = "RW" + Convert.ToInt32(i + 1);//软卧
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
                            dgSite.Rows[i]["seat_number"] = "YW" + Convert.ToInt32(i + 1);//硬卧
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
                            dgSite.Rows[i]["seat_number"] = "DW" + Convert.ToInt32(i + 1);//动卧
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
                            dgSite.Rows[i]["seat_number"] = "GJDW" + Convert.ToInt32(i + 1);//高级动卧
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
                            dgSite.Rows[i]["seat_number"] = "RZ" + Convert.ToInt32(i + 1);//软座
                            dgSite.Rows[i]["seat_type"] = "软座";
                        }
                        if (Convert.ToInt32(SeatType) == 85)
                        {
                            dgSite.Rows[i]["seat_number"] = "YZ" + Convert.ToInt32(i + 1);//硬座
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
                    for (int i = 1; i <= Convert.ToInt32(SeatNum); i++)//根据座位个数时时生成座位
                    {

                        dgSite.Rows.Add();
                        if (Convert.ToInt32(SeatType) == 12)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "SW" + Convert.ToInt32(i);//商务座
                            dgSite.Rows[i - 1]["seat_type"] = "商务座";
                        }
                        if (Convert.ToInt32(SeatType) == 13)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "TD" + Convert.ToInt32(i);//特等座
                            dgSite.Rows[i - 1]["seat_type"] = "特等座";
                        }
                        if (Convert.ToInt32(SeatType) == 14)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "YD" + Convert.ToInt32(i);//一等座
                            dgSite.Rows[i - 1]["seat_type"] = "一等座";
                        }
                        if (Convert.ToInt32(SeatType) == 15)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "ED" + Convert.ToInt32(i);//二等座
                            dgSite.Rows[i - 1]["seat_type"] = "二等座";
                        }
                        if (Convert.ToInt32(SeatType) == 16)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "GJRW" + Convert.ToInt32(i);//高级软卧
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
                            dgSite.Rows[i - 1]["seat_number"] = "RW" + Convert.ToInt32(i);//软卧
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
                            dgSite.Rows[i - 1]["seat_number"] = "YW" + Convert.ToInt32(i);//硬卧
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
                            dgSite.Rows[i - 1]["seat_number"] = "DW" + Convert.ToInt32(i);//动卧
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
                            dgSite.Rows[i - 1]["seat_number"] = "GJDW" + Convert.ToInt32(i);//高级动卧
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
                            dgSite.Rows[i - 1]["seat_number"] = "RZ" + Convert.ToInt32(i);//软座
                            dgSite.Rows[i - 1]["seat_type"] = "软座";
                        }
                        if (Convert.ToInt32(SeatType) == 85)
                        {
                            dgSite.Rows[i - 1]["seat_number"] = "YZ" + Convert.ToInt32(i);//硬座
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
        #region 删除座位个数
        bool KaiGuan = false;
        private void btn_DeleteSeat_Click(object sender, RoutedEventArgs e)
        {
            KaiGuan = true;
            seatRowIndex = dgSeat.SelectedIndex;
            MessageBoxResult dr = MessageBox.Show("是否删除当前座位？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                dgSite.Rows.RemoveAt(seatRowIndex);
            }
            DataTable dt = InsertSeat(dg.Rows[rowIndex]["seat_id"].ToString(), dgSite.Rows.Count.ToString());
            dgSeat.ItemsSource = dgSite.DefaultView;//重新加载座位信息

        }
        #endregion
        #region  保存修改数据       
        private void btn_Affirm_Click(object sender, RoutedEventArgs e)
        {
            #region 删除车厢和座位信息                      
            try
            {
                DataTable dtCompartment = myClient.UserControl_Loaded_SelectCompartment(Convert.ToInt32(DGVR["train_id"])).Tables[0];
                for (int i = 0; i < dtCompartment.Rows.Count; i++)
                {
                    dg.Rows.Add();
                    dg.Rows[i]["CarID"] = dtCompartment.Rows[i]["car_id"];
                    //1、删除车厢信息
                    myClient.UserControl_Loaded_DeleteCar(Convert.ToInt32(dg.Rows[i]["CarID"]));
                    //根据车厢查询座位信息
                    DataTable dtST = myClient.UserControl_Loaded_SelectSeat(Convert.ToInt32(dtCompartment.Rows[i]["car_id"])).Tables[0];
                    for (int j = 0; j < dtST.Rows.Count; j++)
                    {
                        //2、删除座位信息
                        dg.Rows[j]["seat_id"] = dtST.Rows[j]["seat_id"];
                        myClient.UserControl_Loaded_DeleteCarSeat(Convert.ToInt32(dg.Rows[j]["seat_id"]));
                    }
                }
                #endregion
                #region 修改车辆并新增车厢和座位信息
                rowIndex = -1;
                byte[][] bytWenJian = new byte[lstBytes.Count][];
                for (int i = 0; i < lstBytes.Count; i++)
                {
                    bytWenJian[i] = lstBytes[i];
                }
                if (cbo_TrainType.SelectedValue.ToString() != "" && cbo_principal.SelectedValue.ToString() != "" && cbo_linkman.SelectedValue.ToString() != ""
                    && txt_train_number.Text != "" && txt_bottom_number.Text != "" && txt_BiggestNumber.Text != "" && txt_Remark.Text != "")
                {
                    //获取页面数据
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
                    string strTxtLuJing = txt_Location.Text.ToString().Trim();//获取路径
                    //1、修改车辆信息	   
                    DataTable dtTrain = myClient.UserControl_Loaded_UpdateTrain(intas_type_id, intprincipal_id, intlinkman_id, strtrain_number,
                      strbottom_number, dtproduction_date, dtexpiration_date, strbiggest_pitch, bytWenJian, blusing_no, bldisable, strnote, strrate, Convert.ToInt32(DGVR["train_id"]), strOldLuJing, strTxtLuJing).Tables[0];
                }
                //2、新增车厢信息
                for (int i = 0; i < dg.Rows.Count; i++)
                {
                    if (dg.Rows[i]["CarTypeID"].ToString().Trim() != "")
                    {


                        KaiGuan = true;
                        int intas_car_type_id = Convert.ToInt32(dg.Rows[i]["CarTypeID"]);
                        string strcar_number = dg.Rows[i]["car_number"].ToString();
                        string seat_number = dg.Rows[i]["seat_number"].ToString();
                        string strnote = ((DataRowView)dgCompartment.Items[i]).Row["note"].ToString();
                        DataTable dts = InsertSeat(intas_car_type_id.ToString(), seat_number.ToString());
                        //（新增车厢信息）
                        DataTable dtCar = myClient.UserControl_Loaded_InsertCar(intas_car_type_id, strcar_number, strnote, Convert.ToInt32(DGVR["train_id"])).Tables[0];
                        intcar_id = Convert.ToInt32(dtCar.Rows[0][0].ToString());
                        if (intcar_id > 0)
                        {
                            for (int j = 0; j < dts.Rows.Count; j++)
                            {
                                string strseat_number = dts.Rows[j]["seat_number"].ToString();
                                string strseatnote = dts.Rows[j]["seat_type"].ToString();
                                //（新增座位信息）
                                int count = myClient.UserControl_Loaded_InsertCarSeat(intcar_id, intas_car_type_id, strseat_number, strseatnote);
                                if (count > 0)
                                {
                                    if (i == dg.Rows.Count - 1 && j == dts.Rows.Count - 1)
                                    {
                                        MessageBoxResult dr = MessageBox.Show("成功修改车辆信息！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);//弹出确定对话框
                                        if (dr == MessageBoxResult.OK)//如果点了确定按钮
                                        {
                                            BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
                                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 64, "修改车辆【" + strcar_number + "】信息", DateTime.Now);
                                            this.Close();
                                        }
                                    }
                                    else
                                    {
                                        this.Close();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("修改车辆失败");
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("修改车辆失败");
                            break;
                        }
                    }
                }
                #endregion
            }
            catch (Exception)
            {
                throw;
            }            
        }
        #endregion
        #region 退出事件
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
        #endregion
    }
}
