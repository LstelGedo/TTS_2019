using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.LineManage
{
    /// <summary>
    /// WD_InsertStationManage.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertStationManage : Window
    {
        public WD_InsertStationManage()
        {
            InitializeComponent();
        }

        DataTable dt;//声明数据表格dt接收站点信息
        BLL.UC_CreateLine.UC_CreateLineClient myClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        //页面加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            #region 绑定省份信息
            DataTable dtProvince = myClient.UserControl_Loaded_SelectProvince().Tables[0];
            cbo_pro.ItemsSource = dtProvince.DefaultView;//绑定cbo数据
            cbo_pro.DisplayMemberPath = "pro_name";
            cbo_pro.SelectedValuePath = "pro_id";
            #endregion
            //绑定表格数据
            dt = myClient.UserControl_Loaded_SelectStationManage().Tables[0];
            dgSite.ItemsSource = dt.DefaultView;
        }
        //保存数据
        private void btn_Affirm_Click(object sender, RoutedEventArgs e)
        {
            #region 判断页面数据再获取值新增
            try
            {
                if (txt_Station.Text.ToString() != "" && txt_short_code.Text.ToString() != ""
                    && txt_full_code.Text.ToString() != "" && Convert.ToInt32(cbo_pro.SelectedValue) != 0)
                {
                    //获取页面数据
                    string strsite_name = txt_Station.Text.ToString().Trim();
                    string strshort_code = txt_short_code.Text.ToString().Trim();
                    string strfull_code = txt_full_code.Text.ToString().Trim();
                    int intpro_id = Convert.ToInt32(cbo_pro.SelectedValue);
                    Boolean blstop_no = false;
                    //执行站点新增：新增站点表
                    DataTable resules = myClient.UserControl_Loaded_InsertStation(strsite_name, strshort_code,
                        strfull_code, intpro_id, blstop_no).Tables[0];
                    //获取单元格（站点ID）
                    int intsite_id = Convert.ToInt32(resules.Rows[0][0].ToString());
                    //判断返回数据行resules.Rows.Count
                    if (resules.Rows.Count > 0)
                    {
                        int intNeighborCount = 0; //接收返回值
                        //循环新增（邻居站点信息）
                        for (int i = 0; i < dgSite.Items.Count; i++)
                        {
                            if (Convert.ToBoolean(dt.Rows[i]["chked"]) == true && ((DataRowView)dgSite.Items[i]).Row["distance"].ToString() != "")
                            {
                                //执行新增邻居站点
                                int intneighbor_site_id = Convert.ToInt32(((DataRowView)dgSite.Items[i]).Row["site_id"]);
                                Decimal decdistance = Convert.ToDecimal(((DataRowView)dgSite.Items[i]).Row["distance"]);
                                intNeighborCount = Convert.ToInt32(myClient.UserControl_Loaded_InsertNeighborSite(intsite_id, intneighbor_site_id, decdistance));
                            }
                        }
                        //判断是否执行成功
                        if (intNeighborCount > 0)
                        {
                            MessageBoxResult dr = MessageBox.Show("新增站点数据成功！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
                            if (dr == MessageBoxResult.OK)
                            {
                                this.Close();
                            }
                        }
                    }
                }
                else
                {
                    MessageBoxResult dr = MessageBox.Show("请把数据填写完整！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBoxResult dr = MessageBox.Show("数据重复！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
            #endregion
        }
        //关闭窗口
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
        //控制复选框的选中与否
        private void dgSite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Convert.ToBoolean(dt.Rows[0]["chked"]) == false)
            {
                //选中
                dt.Rows[0]["chked"] = true;

            }
            else
            {
                //不选中
                dt.Rows[0]["chked"] = false;
            }

        }
        //站点文本改变事件
        private void txt_Station_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txt_short_code.Text = PublicStaticMothd.GetChineseSpell(txt_Station.Text.Trim());//引用拼音简码方法 
            txt_full_code.Text = (new PinYin()).GetABC(txt_Station.Text);
        }
        //重置页面数据
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            txt_full_code.Text = string.Empty;
            txt_short_code.Text = string.Empty;
            txt_Station.Text = string.Empty;
            cbo_pro.SelectedValue = 0;
            dgSite.ItemsSource = null;
        }


    }
}
