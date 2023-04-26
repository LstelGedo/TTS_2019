using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TTS_2019.View.TrainOrder
{
    /// <summary>
    /// WD_ChoiceTrain.xaml 的交互逻辑
    /// </summary>
    public partial class WD_ChoiceTrain : Window
    {
        public WD_ChoiceTrain()
        {
            InitializeComponent();
        }
        public static DataRowView DRV;

        BLL.UC_CarOrder.UC_CarOrderClient myClient = new BLL.UC_CarOrder.UC_CarOrderClient();
        DataTable dtTrain;
        private void WD_ChoiceTrain_Loaded(object sender, RoutedEventArgs e)
        {

            #region 绑定车辆类型
            DataTable dt = myClient.UserControl_Loaded_SelectTrainType().Tables[0];
            cbo_TrainType.ItemsSource = dt.DefaultView;
            cbo_TrainType.DisplayMemberPath = "TrainType";
            cbo_TrainType.SelectedValuePath = "TrainTypID";
            cbo_TrainType.SelectedIndex = 0;
            cbo_TrainType_SelectionChanged(null, null);
            #endregion
            dtTrain = myClient.UserControl_Loaded_SelectTrainByOrderUsingNo().Tables[0];
            dgTrain.ItemsSource = dtTrain.AsDataView();
        }
        private void cbo_TrainType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {          
            try
            {
                string select = "";
                int  intId =Convert.ToInt32(cbo_TrainType.SelectedValue);
                if (intId >0)
                {
                    //模糊查询内容
                    select += "as_type_id =" + intId;
                }
                DataTable dtselect = myClient.UserControl_Loaded_SelectTrainByOrderUsingNo().Tables[0];
                DataView dv = new DataView(dtselect);
                DataTable dt = new DataTable();

                if (select != "")
                {
                    //筛选数据
                    dv.RowFilter = select;
                    dt = dv.ToTable();
                }
                if (select == "")
                {
                    //查询全部数据
                    dt = dv.ToTable();
                }
                dgTrain.ItemsSource = dt.DefaultView;
            }
            catch (Exception)
            {

                throw;
            }

        }      
        private void btn_Choice(object sender, RoutedEventArgs e)
        {
            DRV = (DataRowView)dgTrain.SelectedItem;
            this.Close();
        }
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
