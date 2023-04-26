using System.Data;
using System.Windows;

namespace TTS_2019.View.LineManage
{
    /// <summary>
    /// WD_ChoiceLine.xaml 的交互逻辑
    /// </summary>
    public partial class WD_ChoiceLine : Window
    {
        public WD_ChoiceLine()
        {
            InitializeComponent();
        }
        public static DataRowView drv;
        BLL.UC_CreateLine.UC_CreateLineClient myClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        DataTable dtLine;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            #region  绑定线路信息           
            dtLine = myClient.UserControl_Loaded_SelectLine().Tables[0];
            dgLine.ItemsSource = dtLine.DefaultView;//绑定DGV
            #endregion
        }
        private void btn_Choice(object sender, RoutedEventArgs e)
        {
            drv = (DataRowView)dgLine.SelectedItem;
            this.Close();
        }
        private void btn_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
