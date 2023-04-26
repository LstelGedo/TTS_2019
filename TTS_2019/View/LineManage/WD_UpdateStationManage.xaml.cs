using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.LineManage
{
    /// <summary>
    /// WD_UpdateStationManage.xaml 的交互逻辑
    /// </summary>
    public partial class WD_UpdateStationManage : Window
    {
        DataRowView DGVR;
        DataTable dt = null;
        DataTable dtOld;//记录修改前的表格数据(临时表)
        int intOldsiteID;//修改主键ID
        BLL.UC_CreateLine.UC_CreateLineClient myClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        public WD_UpdateStationManage(DataRowView drv)
        {
            //接收主页面传递过来的数据
            DGVR = drv;
            InitializeComponent();
        }
        private void WD_UpdateStationManage_Loaded(object sender, RoutedEventArgs e)
        {
            //表格应用封装好的公共样式           
            #region 绑定省份信息(下拉框)
            DataTable dtProvince = myClient.UserControl_Loaded_SelectProvince().Tables[0];
            cbo_pro.ItemsSource = dtProvince.DefaultView;//绑定cbo数据
            cbo_pro.DisplayMemberPath = "pro_name";//显示值
            cbo_pro.SelectedValuePath = "pro_id";//选中值
            #endregion
            //绑定表格数据
            dt = myClient.UserControl_Loaded_SelectStationManage().Tables[0];
            dgSite.ItemsSource = dt.DefaultView;

            #region 页面数据回填
            txt_Station.Text = (DGVR.Row["site_name"]).ToString();
            txt_short_code.Text = (DGVR.Row["short_code"]).ToString();
            txt_full_code.Text = (DGVR.Row["full_code"]).ToString();
            cbo_pro.SelectedValue = Convert.ToInt32((DGVR.Row["pro_id"]));
            intOldsiteID = Convert.ToInt32(DGVR.Row["site_id"]);

            //获取邻居站点数据
            dtOld = myClient.UserControl_Loaded_SelectSite_FromSiteId(intOldsiteID).Tables[0];
            //回填邻居信息
            for (int i = 0; i < dt.Rows.Count; i++)//dt 代表表格数据（全部数据）
            {
                //循环表格数据
                for (int j = 0; j < dtOld.Rows.Count; j++)//一部分数据
                {
                    if (Convert.ToInt32(dt.Rows[i]["site_id"]) == Convert.ToInt32(dtOld.Rows[j]["neighbor_site_id"]))
                    {
                        //绑定表格数值（单元格数值赋值）
                        dt.Rows[i]["chked"] = true;
                        dt.Rows[i]["distance"] = dtOld.Rows[j]["distance"];
                    }
                }
            }

            #endregion

        }
        //站点文本改变事件（绑定简码和拼音码）
        private void txt_Station_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txt_short_code.Text = PublicStaticMothd.GetChineseSpell(txt_Station.Text.Trim());//引用拼音简码方法 
            txt_full_code.Text = (new PinYin()).GetABC(txt_Station.Text);
        }
        //重置按钮
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            txt_full_code.Text = string.Empty;
            txt_short_code.Text = string.Empty;
            txt_Station.Text = string.Empty;
            cbo_pro.SelectedValue = 0;
            dgSite.ItemsSource = null;
        }
        //保存修改（单击事件）
        private void btn_Affirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intIcount = 0;//记录新增数据
                //获取页面数据判断不能为空
                if (txt_Station.Text.ToString() != "" && txt_short_code.Text.ToString() != "" && txt_full_code.Text.ToString() != "")
                {
                    //第一步：站点表（执行修改）
                    string strsite_name = txt_Station.Text.ToString().Trim();
                    string strshort_code = txt_short_code.Text.ToString().Trim();
                    string strfull_code = txt_full_code.Text.ToString().Trim();
                    int intpro_id = Convert.ToInt32(cbo_pro.SelectedValue);
                    Boolean blstop_no = false;
                    //（执行修改）
                    int intCount = myClient.UserControl_Loaded_UpdateStation(strsite_name, strshort_code,
                    strfull_code, intpro_id, blstop_no, intOldsiteID);
                    if (intCount > 0)
                    {
                        //第二步：邻居站点表
                        //1、删除之前（批量删除）
                        int intDCount = 0;
                        for (int i = 0; i < dtOld.Rows.Count; i++)
                        {
                            //主键删除邻居站点数据
                            int intsite_neighbor_id = Convert.ToInt32(dtOld.Rows[i]["site_neighbor_id"]);
                            //（批量删除）
                            myClient.UserControl_Loaded_DeleteNeighborSite(intsite_neighbor_id);
                            intDCount++;
                        }
                        //判断删除是否成功
                        if (dtOld.Rows.Count == intDCount)
                        {
                            //2、：新增现在邻居表数据
                            #region 第二步：新增最新的站点                          
                            for (int i = 0; i < dgSite.Items.Count; i++)
                            {
                                if (Convert.ToBoolean(dt.Rows[i]["chked"]) == true && ((DataRowView)dgSite.Items[i]).Row["distance"].ToString() != "")
                                {
                                    //(新增邻居站点)
                                    int intneighbor_site_id = Convert.ToInt32(((DataRowView)dgSite.Items[i]).Row["site_id"]);//邻居站点ID
                                    Decimal decdistance = Convert.ToDecimal(((DataRowView)dgSite.Items[i]).Row["distance"]);//距离
                                    //执行新增                                                                                     //(站点ID，邻居站点ID，距离)
                                    intIcount = Convert.ToInt32(myClient.UserControl_Loaded_InsertNeighborSite(intOldsiteID, intneighbor_site_id, decdistance));

                                }
                            }
                            #endregion
                        }
                    }
                    //判断邻居站点数据是否新增成功
                    if (intIcount > 0)
                    {
                        MessageBoxResult dr = MessageBox.Show("修改数据成功！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
                        if (dr == MessageBoxResult.OK)
                        {
                            this.Close();//关闭当前窗口
                        }
                    }

                }
                else
                {
                    MessageBox.Show("页面数据不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);//弹出确定对话框
                }

            }
            catch (Exception)
            {

                throw;
            }

        }
        //取消按钮
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
        //CheckBox状态控制
        private void dgSite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToBoolean(dt.Rows[0]["chked"]) == false)
            {
                dt.Rows[0]["chked"] = true;
            }
            else
            {
                dt.Rows[0]["chked"] = false;
            }
        }
    }
}
