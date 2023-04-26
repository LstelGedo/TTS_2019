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
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.TicketTask
{
    /// <summary>
    /// UC_TicketPrice.xaml 的交互逻辑
    /// </summary>
    public partial class UC_TicketPrice : UserControl
    {
        public UC_TicketPrice()
        {
            InitializeComponent();
        }
        BLL.UC_MakePriceRule.UC_MakePriceRuleClient myClient = new BLL.UC_MakePriceRule.UC_MakePriceRuleClient();
        private void US_MakePriceRule_Loaded(object sender, RoutedEventArgs e)
        {
            PublicStaticMothd.SetDgStyle(dgvFareSection);//地远递减率
            DataTable dt = myClient.US_MakePriceRule_Loaded_SelectFareSection().Tables[0];
           
            dgvFareSection.ItemsSource = dt.DefaultView;

            PublicStaticMothd.SetDgStyle(dgvPriceRatio);//票价率
            DataTable dtPriceRatio = myClient.US_MakePriceRule_Loaded_SelectTicketPriceRatio().Tables[0];
            dgvPriceRatio.ItemsSource = dtPriceRatio.DefaultView;//旅程区段
            PublicStaticMothd.SetDgStyle(dgvTTPJP);
            DataTable dtTTPJP = myClient.US_MakePriceRule_Loaded_SelectTicketTTPJP().Tables[0];
            dgvTTPJP.ItemsSource = dtTTPJP.DefaultView;
        }
        /// <summary>
        /// 新增递远递减率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_InsertFareSection_Click(object sender, RoutedEventArgs e)
        {
            WD_InsertFareSection myWD_InsertFareSection = new WD_InsertFareSection();
            myWD_InsertFareSection.ShowDialog();
        }
        /// <summary>
        /// 新增票价率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_InsertPriceRatio_Click(object sender, RoutedEventArgs e)
        {
            WD_InsertPriceRatio myWD_InsertPriceRatio = new WD_InsertPriceRatio();
            myWD_InsertPriceRatio.ShowDialog();
        }
        /// <summary>
        /// 新增旅客票价旅程区段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_InsertTTPJP_Click(object sender, RoutedEventArgs e)
        {
            WD_InsertTTPJP myWD_InsertTTPJP = new WD_InsertTTPJP();
            myWD_InsertTTPJP.ShowDialog();
        }
    }
}
