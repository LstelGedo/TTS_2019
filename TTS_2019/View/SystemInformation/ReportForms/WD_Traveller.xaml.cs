using System.Data;
using System.Windows;

namespace TTS_2019.View.SystemInformation.ReportForms
{
    /// <summary>
    /// WD_Traveller.xaml 的交互逻辑
    /// </summary>
    public partial class WD_Traveller : Window
    {
        public WD_Traveller()
        {
            InitializeComponent();
        }
        BLL.UC_TravellerInformation.UC_TravellerInformationClient myClient = new BLL.UC_TravellerInformation.UC_TravellerInformationClient();
        private void WD_Traveller_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = myClient.UserControl_Loaded_SelectTraveller().Tables[0];
            DS_TTS myTTS = new DS_TTS();
            myTTS.Tables["t_user_file"].Merge(dt);
            CRP_Traveller myCRP_Traveller = new CRP_Traveller();
            myCRP_Traveller.SetDataSource(myTTS);
            CRV_Traveller.ViewerCore.ReportSource = myCRP_Traveller;
        }
    }
}
