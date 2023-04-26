using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace TTS_2019.View.TrainOrder
{
    /// <summary>
    /// UC_CarOrder.xaml 的交互逻辑
    /// </summary>
    public partial class UC_CarOrder : UserControl
    {
        public UC_CarOrder()
        {
            InitializeComponent();
        }
        DataTable dtOrder;
        BLL.UC_CarOrder.UC_CarOrderClient myClient = new BLL.UC_CarOrder.UC_CarOrderClient();
        int orderID = 0;
        //页面加载事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {           
            btnUpdate.IsEnabled = false;//禁用停用按钮
            btnUpdate.Opacity = 0.5;
            btnDelete.IsEnabled = false;//禁用删除按钮
            btnDelete.Opacity = 0.5;
            #region 绑定车辆类型（下拉框）
            DataTable dtOrderType = myClient.UserControl_Loaded_SelectTrainType().Tables[0];
            cbo_TrainType.ItemsSource = dtOrderType.DefaultView;
            cbo_TrainType.DisplayMemberPath = "TrainTyp";
            cbo_TrainType.SelectedValuePath = "TrainTypID";
            #endregion
            #region  绑定车次信息
            dtOrder = myClient.UserControl_Loaded_SelectOrder().Tables[0];
            dgOrder.ItemsSource = dtOrder.DefaultView;
            #endregion
        }
        //查询
        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {

        }
        //新增
        private void btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            WD_InsertCarOrder myWD_InsertCarOrder = new WD_InsertCarOrder();
            myWD_InsertCarOrder.ShowDialog();
            #region  绑定车次信息
            dtOrder = myClient.UserControl_Loaded_SelectOrder().Tables[0];
            dgOrder.ItemsSource = dtOrder.DefaultView;
            #endregion
        }
        //修改
        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int order_id = Convert.ToInt32(((DataRowView)dgOrder.SelectedItem).Row["order_id"]);
                if (order_id != 0 && order_id != null)
                {
                    WD_UpdateCarOrder myWD_UpdateCarOrder = new WD_UpdateCarOrder((DataRowView)dgOrder.SelectedItem);
                    myWD_UpdateCarOrder.ShowDialog();
                    #region  绑定车次信息
                    dtOrder = myClient.UserControl_Loaded_SelectOrder().Tables[0];
                    dgOrder.ItemsSource = dtOrder.DefaultView;
                    #endregion
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        //删除
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            //思路:
            try
            {
                MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //弹出确定对话框
                if (dr == MessageBoxResult.OK) //如果点了确定按钮
                {
                    int order_id = Convert.ToInt32(((DataRowView)dgOrder.SelectedItem).Row["order_id"]);
                    
                    if (order_id != 0)
                    {                        
                        //第一步：删除车次信息
                        int intDCount = myClient.UserControl_Loaded_DeleteOrder(order_id); //执行删除事件 
                        if (intDCount > 0)
                        {
                            int intDSum = 0;//记录删除的行数
                            //第二步：删除车次明细信息
                            for (int i = 0; i < dgOderDetail.Items.Count; i++)
                            {
                                //获取主键ID
                                int intorder_detail_id = Convert.ToInt32(((DataRowView)dgOderDetail.Items[i]).Row["order_detail_id"]);                             
                                myClient.UserControl_Loaded_DeleteOrderDetail(intorder_detail_id);
                                intDSum++;

                            }
                            //删除行数相同
                            if (intDSum == dgOderDetail.Items.Count)
                            {
                                DataTable dtTrain = myClient.UserControl_Loaded_SelectTrainByOrderID(orderID).Tables[0];//调用查询方法，把返回的值，赋给DGV
                                int intTrainId = Convert.ToInt32(dtTrain.Rows[0]["train_id"]);//车辆ID
                                //第三步：修改新增车辆的车次ID=NULL
                                myClient.UserControl_Loaded_DUpdateTrainOrderID(intTrainId);
                                MessageBox.Show("删除线路成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                #region  绑定车次信息
                                dtOrder = myClient.UserControl_Loaded_SelectOrder().Tables[0];
                                dgOrder.ItemsSource = dtOrder.DefaultView;
                                #endregion
                            }

                        }

                    }
                    else
                    {
                        MessageBox.Show("请选择要删除的行。。");
                    }
                }
                   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
               
        #region 查询车次明细信息
        private void dgOrder_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dgOrder.CurrentCell != null)
                {
                    orderID = Convert.ToInt32(((DataRowView)dgOrder.CurrentItem).Row["order_id"]);
                    if (orderID != 0)
                    {
                        //激活修改与删除按钮
                        btnUpdate.IsEnabled = true;
                        btnUpdate.Opacity = 1;
                        btnDelete.IsEnabled = true;
                        btnDelete.Opacity = 1;
                        #region  绑定站点信息
                        DataTable dtOderDetail = myClient.UserControl_Loaded_SelectOrderDetail(orderID).Tables[0];
                        dgOderDetail.ItemsSource = dtOderDetail.DefaultView;
                        #endregion
                    }
                    else
                    {
                        btnUpdate.IsEnabled = false;//禁用停用按钮
                        btnUpdate.Opacity = 0.5;
                        btnDelete.IsEnabled = false;//禁用删除按钮
                        btnDelete.Opacity = 0.5;
                        MessageBox.Show("操作失误！");
                    }
                }
                else
                {
                    MessageBox.Show("抱歉！没有可供查询的站点！");
                }
            }
            catch
            {
            }
        }
        #endregion
        //单元格修改事件
        private void dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            orderID = Convert.ToInt32(((DataRowView)dgOrder.CurrentItem).Row["order_id"]);

        }
        //行明细显示改变事件
        private void dgOrder_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {

            DataGridRow row = e.Row;
            int intIndex = 0;
            for (int i = 0; i < dtOrder.Rows.Count; i++)
            {
                if (Convert.ToInt32(dtOrder.Rows[i]["order_id"]) == Convert.ToInt32(((DataRowView)dgOrder.Items[this.dgOrder.SelectedIndex]).Row["order_id"]))
                {
                    intIndex = i;
                    break;
                }
            }
            DataGrid dgvSiteList = e.DetailsElement as DataGrid;           
            if (dtOrder.Rows[intIndex]["extend"].ToString() == "-")
            {
                dtOrder.Rows[intIndex]["extend"] = "-";
                row.DetailsVisibility = System.Windows.Visibility.Visible;

                int intorderID = Convert.ToInt32(dtOrder.Rows[intIndex]["order_id"]);
                DataTable dtOrderList = myClient.UserControl_Loaded_SelectTrainByOrderID(intorderID).Tables[0];//调用查询方法，把返回的值，赋给DGV
                dgvSiteList.ItemsSource = dtOrderList.AsDataView();
                if ((dtOrderList.Rows.Count * 25) >= 425)
                {
                    dgvSiteList.Height = 425;
                }
                else
                {
                    dgvSiteList.Height = 30 + dtOrderList.Rows.Count * 25;//设置内置DataGrid的高度 
                }
                dgvSiteList.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                row.Height = 25 + dgvSiteList.Height;
                dgvSiteList.Margin = new Thickness(20, 0, 10, 0);
            }
            else
            {
                dtOrder.Rows[intIndex]["extend"] = "+";
                row.DetailsVisibility = System.Windows.Visibility.Collapsed;
                row.Height = 25;
            }

        }
        //车次表（鼠标双击事件）
        private void dgOrder_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (dgOrder.SelectedIndex != -1)
                {
                    if (dgOrder.CurrentCell.Column.Header.ToString() == "")
                    {
                        DataGridRow row = (DataGridRow)dgOrder.ItemContainerGenerator.ContainerFromIndex(this.dgOrder.SelectedIndex);
                        int intIndex = 0;
                        for (int i = 0; i < dtOrder.Rows.Count; i++)
                        {
                            if (Convert.ToInt32(dtOrder.Rows[i]["order_id"]) == Convert.ToInt32(((DataRowView)dgOrder.Items[this.dgOrder.SelectedIndex]).Row["order_id"]))
                            {
                                intIndex = i;
                                break;
                            }
                        }
                        if (dtOrder.Rows[intIndex]["extend"].ToString() == "+")
                        {
                            dtOrder.Rows[intIndex]["extend"] = "-";
                            row.DetailsVisibility = System.Windows.Visibility.Visible;
                        }
                        else if (dtOrder.Rows[intIndex]["extend"].ToString() == "-")
                        {
                            dtOrder.Rows[intIndex]["extend"] = "+";
                            row.DetailsVisibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
            }
            catch { }
        }        
    }
}
