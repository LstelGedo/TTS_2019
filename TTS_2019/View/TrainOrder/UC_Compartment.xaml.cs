using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TTS_2019.View.TrainOrder
{
    /// <summary>
    /// UC_Compartment.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Compartment : UserControl
    {
        public UC_Compartment()
        {
            InitializeComponent();
        }
        BLL.UC_Compartment.UC_CompartmentClient myClient = new BLL.UC_Compartment.UC_CompartmentClient();
        string[] myPicture;//图片
        #region loaded事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {           
            #region  绑定车辆信息
            DataTable dtTrain = myClient.UserControl_Loaded_SelectTrain().Tables[0];
            dgTrain.ItemsSource = dtTrain.DefaultView;
            #endregion

        }
        #endregion        
        #region 新增事件
        private void btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            WD_InsertCarInformation myWD_InsertCarInformation = new WD_InsertCarInformation();
            myWD_InsertCarInformation.ShowDialog();
            #region  绑定车辆信息
            DataTable dtTrain = myClient.UserControl_Loaded_SelectTrain().Tables[0];
            dgTrain.ItemsSource = dtTrain.DefaultView;
            #endregion
        }
        #endregion
        #region 修改事件
        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            if ((DataRowView)dgTrain.SelectedItem != null)
            {
                int train = Convert.ToInt32(((DataRowView)dgTrain.SelectedItem).Row["train_id"]);
                if (train != 0)
                {
                    WD_UpdateCarInformation myWD_UpdateCarInformation = new WD_UpdateCarInformation((DataRowView)dgTrain.SelectedItem);
                    myWD_UpdateCarInformation.ShowDialog();
                    #region  绑定车辆信息
                    DataTable dtTrain = myClient.UserControl_Loaded_SelectTrain().Tables[0];
                    dgTrain.ItemsSource = dtTrain.DefaultView;
                    #endregion
                }
            }
            else
            {
                MessageBox.Show("请选择要修改的员工信息！");
            }
        }
        #endregion
        #region 删除事件
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

       
        private void dgTrain_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if ((DataRowView)dgTrain.CurrentItem != null)
            {
                int inttrain_id = Convert.ToInt32(((DataRowView)dgTrain.CurrentItem).Row["train_id"]);
                DataTable dt = myClient.UserControl_Loaded_SelectCompartment(inttrain_id).Tables[0];
                dgCompartment.ItemsSource = dt.DefaultView;
                try
                {
                    #region 显示图片
                    DockPanel dp = new DockPanel();
                    string[] strLuJingZu = ((DataRowView)dgTrain.CurrentItem).Row["image_path"].ToString().Split(';');
                    string lujing = ((DataRowView)dgTrain.CurrentItem).Row["image_path"].ToString();
                    myPicture = myClient.UserControl_Loaded_SelectPhoro(lujing);
                    for (int i = 1; i < strLuJingZu.Length; i++)
                    {
                        // BitmapImage images = new BitmapImage(new Uri(myPicture[i - 1].ToString()));
                        #region 防止线程冲突
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        //增加这一行（指定位图图像如何利用内存缓存=在加载时将整个图像缓存到内存中。对图像数据的所有请求将通过内存存储区进行填充。）                
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(myPicture[i - 1].ToString());
                        bi.EndInit();
                        #endregion

                        Image photo = new Image();
                        photo.Height = 150;
                        photo.Margin = new Thickness(5, 0, 0, 0);
                        photo.Source = bi.Clone();
                        dp.Children.Add(photo);
                    }
                    img_pictrue.Content = dp;
                    #endregion
                }
                catch { }
            }

        }

        private void dgCompartment_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if ((DataRowView)dgCompartment.CurrentItem != null)
            {
                int intcar_id = Convert.ToInt32(((DataRowView)dgCompartment.CurrentItem).Row["car_id"]);
                DataTable dt = myClient.UserControl_Loaded_SelectSeat(intcar_id).Tables[0];
                dgSeat.ItemsSource = dt.DefaultView;
            }

        }
        #region 多条件搜索
        private void txt_Select_SelectionChanged(object sender, RoutedEventArgs e)
        {
            #region 多条件查询 
            string select = "";
            if (txt_Select.Text != "")
            {
                select += " train_number like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or type like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or principal like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or state like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or using_no like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            "or linkman like '%'+'" + txt_Select.Text.Trim() + "'+'%'";
                //累加模糊查询内容

            }
            DataTable dtselect = myClient.UserControl_Loaded_SelectTrain().Tables[0];
            DataView dv = new DataView(dtselect);
            DataTable dt = new DataTable();
            if (select != "")
            {
                dv.RowFilter = select;
                dt = dv.ToTable();
            }
            if (select == "")
            {
                dt = dv.ToTable();
            }
            dgTrain.ItemsSource = dt.DefaultView;
            #endregion
        }
        #endregion

        private void txt_Select_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
