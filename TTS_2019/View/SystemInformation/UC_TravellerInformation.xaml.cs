using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using TTS_2019.Tools.Utils;
using TTS_2019.View.SystemInformation.ReportForms;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// US_TravellerInformation.xaml 的交互逻辑
    /// </summary>
    public partial class UC_TravellerInformation : UserControl
    {
        public UC_TravellerInformation()
        {
            InitializeComponent();
        }
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        BLL.UC_TravellerInformation.UC_TravellerInformationClient myUS_TravellerInformationClient = new BLL.UC_TravellerInformation.UC_TravellerInformationClient();
        private void GetData()//绑定表格数据
        {
            DataTable dtTraveller = myUS_TravellerInformationClient.UserControl_Loaded_SelectTraveller().Tables[0];
            dgTraveller.ItemsSource = dtTraveller.DefaultView;
        }       
        //1.0 页面载入事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetData();
            //初始化操作按钮
            btnUpdate.IsEnabled = false;//禁用停用按钮
            btnUpdate.Opacity = 0.5;
            btnDelete.IsEnabled = false;//禁用停用按钮
            btnDelete.Opacity = 0.5;

        }
        //1.1 新增按钮
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            WD_InsertTravellerInformation myWD_InsertTravellerInformation = new WD_InsertTravellerInformation();
            myWD_InsertTravellerInformation.ShowDialog();
            GetData();
        }
        //1.2 修改按钮
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if ((DataRowView)dgTraveller.SelectedItem != null)
            {
                int TravellerID = Convert.ToInt32(((DataRowView)dgTraveller.SelectedItem).Row["traveller_id"]);
                if (TravellerID != 0)
                {
                    WD_InsertTravellerInformation myWD_UpdateTravellerInformation = new WD_InsertTravellerInformation((DataRowView)dgTraveller.SelectedItem);
                    myWD_UpdateTravellerInformation.ShowDialog();
                    GetData();//刷新数据
                }
            }
            else
            {
                MessageBox.Show("请选择要修改的旅客信息！");
            }
        }
        //1.3 删除按钮
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if ((DataRowView)dgTraveller.CurrentItem != null)
            {
                int TravellerID = Convert.ToInt32(((DataRowView)dgTraveller.SelectedItem).Row["traveller_id"]);
                string Traveller = (((DataRowView)dgTraveller.SelectedItem).Row["name"]).ToString();
                if (TravellerID != 0)
                {
                    MessageBoxResult dr = MessageBox.Show("删除旅客信息？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
                    if (dr == MessageBoxResult.OK)//如果点了确定按钮
                    {
                        myUS_TravellerInformationClient.btn_Delete_Click_DelectTraveller(TravellerID);
                        myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 65, "删除【" + Traveller + "】旅客信息", DateTime.Now);
                        #region  绑定旅客信息
                        DataTable dtTraveller = myUS_TravellerInformationClient.UserControl_Loaded_SelectTraveller().Tables[0];
                        dgTraveller.ItemsSource = dtTraveller.DefaultView;
                        #endregion
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择要删除的旅客信息！");
            }
        }
        //1.4 打印按钮
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            WD_Traveller myWD_Traveller = new WD_Traveller();
            myWD_Traveller.ShowDialog();
            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 65, "打印【旅客】信息", DateTime.Now);
        }
        //1.5 条件筛选
        private void txt_Select_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string select = "";
            if (txt_Select.Text != "")
            {
                select += " name like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or certificate_type like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or country like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or certificate_number like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or passenger_type like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or gender like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or phone_number like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or address like '%'+'" + txt_Select.Text.Trim() + "'+'%'" +
                            " or zip_code like '%'+'" + txt_Select.Text.Trim() + "'+'%'";
                //累加模糊查询内容

            }
            DataTable dtTraveller = myUS_TravellerInformationClient.UserControl_Loaded_SelectTraveller().Tables[0];
            DataView dv = new DataView(dtTraveller);
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
            dgTraveller.ItemsSource = dt.DefaultView;
        }
        //1.6 选中改变事件
        private void dgTraveller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((DataRowView)dgTraveller.SelectedItem != null)
            {
                //激活
                btnUpdate.IsEnabled = true;
                btnUpdate.Opacity = 1;
                btnDelete.IsEnabled = true;
                btnDelete.Opacity = 1;               
            }
            else
            {                
                btnUpdate.IsEnabled = false;//禁用停用按钮
                btnUpdate.Opacity = 0.5;
                btnDelete.IsEnabled = false;//禁用停用按钮
                btnDelete.Opacity = 0.5;               
            }
        }
        //1.7  导出数据
        private void btnEmport_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("是否要导出当前数据？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr==MessageBoxResult.OK)
            {
                if (ExportToExcel.Export(dgTraveller, "旅客信息") == true)
                {
                    MessageBox.Show("数据导出成功!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("数据导出失败!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        
    }
}
