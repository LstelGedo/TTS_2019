using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TTS_2019.View.LineManage
{
    /// <summary>
    /// UC_StationManage.xaml 的交互逻辑
    /// </summary>
    public partial class UC_StationManage : UserControl
    {
        public UC_StationManage()
        {
            InitializeComponent();
        }
        #region  实例化全局变量
        BLL.UC_CreateLine.UC_CreateLineClient myClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        DataTable dtStationManage;
        BLL.PublicFunction.PublicFunctionClient myPublicFunction = new BLL.PublicFunction.PublicFunctionClient();
        #region linq 查询分页
        #region 数据库查询需要的参数，字段名，表名，排序字段,每页的大小，查询第几页，where条件，页数
        string strGetFields = "ROW_NUMBER () over(order by t_site.site_id) as number,t_site.site_id,RTRIM(t_site.site_name) AS site_name,"
               + " RTRIM(t_site.short_code)AS short_code,RTRIM(t_site.full_code) AS full_code,RTRIM(t_site.pro_id) AS pro_id,"
               + " RTRIM(t_Province.pro_name)  AS pro_name, RTRIM(case WHEN t_site.stop_no = 'true' THEN '停用' ELSE '未停用' END) AS stop_no,"
               + " RTRIM(case WHEN t_site.stop_no = 'true' THEN 'Red' ELSE 'Blue' END) AS color";

        string strTblName = "t_site INNER JOIN t_Province ON t_site.pro_id = t_Province.pro_id";
        string strFldName = "site_id ";//排序的字段名
        int intPageSize = 20;//页尺寸（显示条数）
        int intPageIndex = 1;//页码
        string strWhere = "1=1";   //查询条件 (注意: 不要加 where)   
        decimal decPageCount = 0;  //页数
        #endregion
        #endregion


        #endregion

        //页面加载事件
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            #region 按钮操控
            btnStop.IsEnabled = false;//禁用停用按钮
            btnStop.Opacity = 0.5;
            btnDelete.IsEnabled = false;//禁用删除按钮
            btnDelete.Opacity = 0.5;
            btnUpdate.IsEnabled = false;//禁用修改按钮
            btnUpdate.Opacity = 0.5;
            #endregion
            #region 初始化窗体数据  
            txtCurrentPage.Text = "0";//当前页数
            lblMaxPage.Content = decPageCount;//最大页数
            int[] intPageLineCounts = {5,10,20,30, 50, 100};//创建int数组
            cboPageLineCount.ItemsSource = intPageLineCounts;//绑定每页行数下拉框
            cboPageLineCount.Text = intPageSize.ToString();//设置默认值                                                           
            GetTable();//刷新表格
            #endregion
        }
        #region  查询站点信息
        private void GetTable()
        {
            //接收表格
            dtStationManage = myClient.UserControl_Loaded_SelectStationManage().Tables[0];
            //绑定数据源
            dgStationManage.ItemsSource = dtStationManage.DefaultView;
        }
        #endregion
        #region 控制底部分页按钮
        //控制按钮启用和禁用
        public void UpdatePagingButton()
        {
            //如果是首页，则把上一页和首页按钮禁用(样式)
            if (Convert.ToInt32(txtCurrentPage.Text) <= 1)
            {
                //禁用（首页，上一页）按钮
                imgMostUp.Opacity = 0.3;
                imgMostUp.Cursor = Cursors.Arrow;
                imgUp.Opacity = 0.3;
                imgUp.Cursor = Cursors.Arrow;

            }
            else
            {
                //启用（首页，上一页）按钮
                imgMostUp.Opacity = 1;
                imgMostUp.Cursor = Cursors.Hand;
                imgUp.Opacity = 1;
                imgUp.Cursor = Cursors.Hand;
            }
            //如果是尾页，则把下一页和尾页按钮禁用(样式)
            if (Convert.ToInt32(txtCurrentPage.Text) >= Convert.ToInt32(lblMaxPage.Content))
            {
                //禁用（末尾页，下一页）按钮
                imgMostDown.Opacity = 0.3;
                imgMostDown.Cursor = Cursors.Arrow;
                imgDown.Opacity = 0.3;
                imgDown.Cursor = Cursors.Arrow;
            }
            else
            {
                //启用（末尾页，下一页）按钮
                imgMostDown.Opacity = 1;
                imgMostDown.Cursor = Cursors.Hand;
                imgDown.Opacity = 1;
                imgDown.Cursor = Cursors.Hand;
            }
        }
        //分页操作
        #region 分页查询方法（intCurrentPage每页显示条数，intCurrentPage当前页）        
        DataTable ChaXunShuJu(int intPageLineCount, int intCurrentPage, bool bolSeleteSum, bool bolInvertedOrder)
        {
            if (txt_station.Text.Trim() != "")
            {
                //累加模糊查询内容
                strWhere = "site_name like '%'+'" + txt_station.Text.Trim() + "'+'%'";
            }
            if (txt_pro.Text.Trim() != string.Empty)
            {
                //累加模糊查询内容
                strWhere = strWhere + "and  pro_name like '%" + txt_pro.Text.Trim() + "%'";
            }
            else if ((txt_station.Text == "" || txt_station.Text == string.Empty) && (txt_pro.Text.Trim() == "" || txt_station.Text.Trim() == string.Empty))
            {
                //默认查询内容
                strWhere = "1=1";
            }


            //获取数据总行数
            decPageCount = Convert.ToDecimal(myPublicFunction.PublicPagingSelect(strTblName, strGetFields, strFldName, intPageSize, intPageIndex, true, true, strWhere).Tables[0].Rows[0][0]);//获取查询结果的行数
            //页数的几种情况           
            if (decPageCount / intPageLineCount <= Convert.ToInt32(decPageCount / intPageLineCount))//判断有多少页
            {
                //页数(有条件)
                decPageCount = Convert.ToInt32(decPageCount / intPageLineCount);
            }
            else
            {
                //页数(默认decPageCount = 0)
                decPageCount = Convert.ToInt32(decPageCount / intPageLineCount) + 1;
            }
            if (intCurrentPage > decPageCount)//如果当前页数大于最大页数，则重新赋值为最大页数
            {
                intPageIndex = (int)decPageCount;
                intCurrentPage = (int)decPageCount;
            }
            if (intPageIndex == 0 || intCurrentPage == 0)//如果当前页数为0，则重新赋值为1
            {
                intPageIndex = 1;
                intCurrentPage = 1;
            }
            lblMaxPage.Content = decPageCount.ToString();//给最大页数赋值
            txtCurrentPage.Text = intCurrentPage.ToString();//给当前页数文本框赋值
            //执行分页得到具体的data数据
            return myPublicFunction.PublicPagingSelect(strTblName, strGetFields, strFldName, intPageLineCount, intCurrentPage, bolSeleteSum, bolInvertedOrder, strWhere).Tables[0];//返回查询出来的结果
        }
        #endregion
        //查询按钮
        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            //调用查询方法，把返回的值，赋给DGV
            dtStationManage = ChaXunShuJu(intPageSize, intPageIndex, false, true);
            dgStationManage.ItemsSource = dtStationManage.AsDataView();
            if (dtStationManage.Rows.Count < 1)
            {
                intPageIndex = 0;
                txtCurrentPage.Text = "0";
                cboPageLineCount.IsEnabled = false;//禁用每页行数下拉框
                txtCurrentPage.IsEnabled = false;//禁用当前页文本框
            }
            else
            {
                cboPageLineCount.IsEnabled = true;
                txtCurrentPage.IsEnabled = true;
            }
            UpdatePagingButton();
        }

        //首页按钮（鼠标左键按下事件）
        private void imgMostUp_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //首页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex = 1;//当前显示页:第一页
                txtCurrentPage.Text = "1";//首页
                btn_Select_Click(null, null);
            }
        }
        //上一页按钮（鼠标左键按下事件）
        private void imgUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //上一页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex--;//上一页：索引-1
                txtCurrentPage.Text = intPageIndex.ToString();//设置文本值显示当前页
                btn_Select_Click(null, null);
            }
        }
        //下一页按钮（鼠标左键按下事件）
        private void imgDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //下一页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex++;//下一页：当前索引+1
                txtCurrentPage.Text = intPageIndex.ToString();//当前页
                btn_Select_Click(null, null);
            }
        }
        //尾页按钮（鼠标左键按下事件）
        private void imgMostDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //尾页
            if (((Image)sender).Opacity == 1)
            {
                intPageIndex = Convert.ToInt32(lblMaxPage.Content);//获取最大页数
                txtCurrentPage.Text = intPageIndex.ToString();//当前页
                //触发事件
                btn_Select_Click(null, null);
            }
        }
        //自定义每页行数(下拉框改变事件)
        private void cboPageLineCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //每页行数
            intPageSize = Convert.ToInt32(cboPageLineCount.SelectedValue);
            intPageIndex = 1;
            //触发分页
            btn_Select_Click(null, null);
        }
        //页数（文本改变事件）
        private void txtCurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //屏蔽中文输入和非法字符粘贴输入
                TextBox textBox = sender as TextBox;
                TextChange[] change = new TextChange[e.Changes.Count];
                e.Changes.CopyTo(change, 0);

                int offset = change[0].Offset;
                if (change[0].AddedLength > 0)
                {
                    int num = 0;
                    if (!int.TryParse(textBox.Text, out num))
                    {
                        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength).ToString().Trim();
                        textBox.Select(offset, 0);
                    }
                }
                if (decPageCount != 0)
                {
                    //判断输入数字>总页数
                    if (Convert.ToInt32(textBox.Text.Trim()) > decPageCount)
                    {
                        //获取=总页数
                        textBox.Text = decPageCount.ToString();
                    }
                    //输入数字<1
                    else if (Convert.ToInt32(textBox.Text.Trim()) < 1)
                    {                     
                        textBox.Text = "1";
                    }
                }
                //当前输入的值！=现在的页数
                if (intPageIndex != Convert.ToInt32(textBox.Text.Trim()) && Convert.ToInt32(textBox.Text.Trim()) >= 1)
                {
                    //执行分页
                    intPageIndex = Convert.ToInt32(textBox.Text.Trim());
                    btn_Select_Click(null, null);
                }
            }
            catch (Exception)
            {
                btn_Select_Click(null, null);
            }
        }
        #endregion

        //新增按钮事件
        private void btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            //实例化窗口
            WD_InsertStationManage myWD_InsertStationManage = new WD_InsertStationManage();
            myWD_InsertStationManage.ShowDialog();
            GetTable();//刷新页面(绑定站点信息)
        }
        //修改按钮事件
        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((DataRowView)dgStationManage.SelectedItem != null)
                {
                    int staffid = Convert.ToInt32(((DataRowView)dgStationManage.SelectedItem).Row["site_id"]);
                    if (staffid != 0)
                    {
                        //实例化修改窗口
                        WD_UpdateStationManage myWD_UpdateStationManage = new WD_UpdateStationManage((DataRowView)dgStationManage.SelectedItem);
                        myWD_UpdateStationManage.ShowDialog();
                        GetTable();//绑定站点信息
                    }
                }
                else
                {
                    MessageBox.Show("请选择要修改的站点信息！");
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        //删除按钮事件
        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intDSum = 0;//记录删除成功条数
                //判断是否选中数据
                MessageBoxResult dr = MessageBox.Show("是否删除？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                //弹出确定对话框
                if (dr == MessageBoxResult.OK) //如果点了确定按钮
                {
                    int intsite_id = Convert.ToInt32(((DataRowView)dgStationManage.SelectedItem).Row["site_id"]); //站点ID
                    //1.删除单条数据(站点信息)
                    int intSitCount = myClient.UserControl_Loaded_DeleteStation(intsite_id);
                    //（站点表）删除成功
                    if (intSitCount > 0)
                    {

                        //2、批量删除当前站点相关的邻居站点数据
                        for (int i = 0; i < dgSite.Items.Count; i++)
                        {
                            intDSum++;
                            //获取主键ID
                            int intneighbor_site_id = Convert.ToInt32(((DataRowView)dgSite.Items[i]).Row["site_neighbor_id"]);
                            int intNeighborCount = myClient.UserControl_Loaded_DeleteNeighborSite(intneighbor_site_id);
                        }
                    }
                    //删除行数相同
                    if (intDSum == dgSite.Items.Count)
                    {
                        MessageBox.Show("删除站点成功！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    }
                }
                //调用绑定表格方法
                GetTable();//刷新页面
            }
            catch (Exception)
            {
                return;
            }
        }
        //停用按钮事件
        private void btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("一旦停用站点就不能启用了，确定吗？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            //弹出确定对话框            
            if (dr == MessageBoxResult.OK) //如果点了确定按钮
            {
                int intsite_id = Convert.ToInt32(((DataRowView)dgStationManage.SelectedItem).Row["site_id"]); //站点ID
                int intStopCount = myClient.UserControl_Loaded_StopSite(intsite_id);
                if (intStopCount > 0)
                {
                    MessageBox.Show("停用成功！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    //调用绑定表格方法
                    GetTable();//刷新页面
                }
            }
        }
        //当前站点表（选中行改变事件）
        private void dgStationManage_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {           
            try
            {
                if (((DataRowView)dgStationManage.SelectedItem) != null)//判断选中的行是否为空
                {
                    //激活停用按钮
                    btnStop.IsEnabled = true;
                    btnStop.Opacity = 1;
                    btnDelete.IsEnabled = true;
                    btnDelete.Opacity = 1;
                    btnUpdate.IsEnabled = true;
                    btnUpdate.Opacity = 1;
                    int intSiteID = Convert.ToInt32(((DataRowView)dgStationManage.CurrentItem).Row["site_id"]);
                    if (intSiteID != 0)
                    {
                        #region  绑定站点信息
                        DataTable dtSite = myClient.UserControl_Loaded_SelectSite_FromSiteId(intSiteID).Tables[0];
                        dgSite.ItemsSource = dtSite.DefaultView;
                        #endregion
                    }
                }
                else
                {
                    //禁用停用按钮
                    btnStop.IsEnabled = false;
                    btnStop.Opacity = 0.5;
                    btnUpdate.IsEnabled = false;
                    btnUpdate.Opacity = 0.5;
                    btnDelete.IsEnabled = false;
                    btnDelete.Opacity = 0.5;                    
                }               
            }
            catch
            {
                return;
            }
        }
    }
}
