using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TTS_2019.Tools.Controls;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.LineManage
{
    /// <summary>
    /// WD_InsertCreateLine.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertCreateLine : Window
    {

        private static combobox[] dataSource;//声明静态下拉框数组
        BLL.UC_CreateLine.UC_CreateLineClient myClient = new BLL.UC_CreateLine.UC_CreateLineClient();
        public WD_InsertCreateLine()
        {
            InitializeComponent();
        }
        //1.0 页面加载事件
        private void CreateLine_Loaded(object sender, RoutedEventArgs e)
        {            
            //绑定下拉框
            DataTable dt = myClient.UserControl_Loaded_SelectStation().Tables[0];
            cbo_StarStation.ItemsSource = dt.DefaultView;
            cbo_StarStation.DisplayMemberPath = "site_name";
            cbo_StarStation.SelectedValuePath = "site_id";

            cbo_EndStation.ItemsSource = dt.DefaultView;
            cbo_EndStation.DisplayMemberPath = "site_name";
            cbo_EndStation.SelectedValuePath = "site_id";


            dataSource = new combobox[dt.Rows.Count];
            //给下拉框数组绑定数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dataSource[i] = new combobox();
                dataSource[i].site_id = Convert.ToInt32(dt.Rows[i]["site_id"].ToString().Trim());
                dataSource[i].site_name = dt.Rows[i]["site_name"].ToString().Trim();
            }

        }
        //1.1 筛选数据
        private static ObservableCollection<combobox> GetStation(string Pattern)
        {
            return new ObservableCollection<combobox>(dataSource.Where((combobox, match) => combobox.site_name.ToLower().Contains(Pattern.ToLower())));
        }

        //1.2 开始站点下拉框（改版事件）
        private void station_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            //自动完成源(条件筛选数据)
            if (string.IsNullOrEmpty(args.Pattern))
            {
                //空白
                args.CancelBinding = true;
            }
            else
            {
                //筛选数据
                args.DataSource = GetStation(args.Pattern);
            }

        }
        //1.3 结束站点下拉框（改版事件）
        private void endstation_PatternChanged(object sender, AutoComplete.AutoCompleteArgs args)
        {
            //自动完成源(条件筛选数据)
            if (string.IsNullOrEmpty(args.Pattern))
            {
                //空白
                args.CancelBinding = true;
            }
            else
            {
                //筛选数据
                args.DataSource = GetStation(args.Pattern);
            }
        }
        //1.4 目的站点下拉框（事件值）
        private void cbo_EndStation_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                //给相应参数赋值
                string StartStation = this.cbo_StarStation.Text.ToString().Trim();
                string EndStation = this.cbo_EndStation.Text.ToString().Trim();
                string XianLuMing = StartStation + "-" + EndStation;
                string ZhuJiMa = StartStation + EndStation;
                //把生成的路线名称和助记码绑定到文本框
                txt_LineName.Text = XianLuMing;
                txt_Code.Text = ZhuJiMa;
                txt_Code.Text = PublicStaticMothd.GetChineseSpell(txt_LineName.Text.Trim());//引用拼音简码方法
            }
            catch (Exception)
            {
                return;
            }
        }
        //1.5 线路名称改变事件（生成助记码）
        private void txt_LineName_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txt_Code.Text = PublicStaticMothd.GetChineseSpell(txt_LineName.Text.Trim());//引用拼音简码方法 
        }
        //1.6 目的站点下拉事件（事件提供数据SelectionChangedEventArgs）
        private void cbo_EndStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //给相应参数赋值
                string StartStation = this.cbo_StarStation.Text.ToString().Trim();
                string EndStation = this.cbo_EndStation.Text.ToString().Trim();
                string XianLuMing = StartStation + "-" + EndStation;
                string ZhuJiMa = StartStation + EndStation;
                //把生成的路线名称和助记码绑定到文本框
                txt_LineName.Text = XianLuMing;
                txt_Code.Text = ZhuJiMa;
            }
            catch (Exception)
            {
                return;
            }
        }

        //1.7 生成线路
        public DataTable dtLinJu;//邻居站点数据表
        public DataTable dtXianLu;//线路数据表（自动生成）
        string QiShiWangDian;//开始站点
        string JieZhiWangDian;//结束站点
        string Distance;//距离
        decimal JulI;
        int intline_id;//声明线路ID(获取新增线路主键ID)            
        /// <summary>
        /// 生成线路
        /// </summary>
        /// <param name="strStartStationID">起始站点ID</param>
        /// <param name="strAllDistance">距离集合</param>
        /// <param name="strAllLine">线路名称</param>
        /// <param name="deDistance">距离</param>
        /// <param name="strAllID">站点ID集合</param>
        public void ShengChengXianLu(string strStartStationID, string strAllDistance, string strAllLine, decimal deDistance, string strAllID)
        {
            //1、知道站点关系（邻居站点表--体现了站点和邻居站点关系）
            //2、获取页面【开始站点】和【结束站点】
            //3、【开始站点】的站点==site_id,结束站点==site_neighbor_id
            //4、第一步:【开始站点】的站点==site_id匹配邻居站点1

            //（1）没有数据（提醒用户：不能生成线路）
            /*（2）可以生成线路： 邻居站点1 == site_id邻居站点2
                     结束站点 == site_neighbor_id 。拼接字符串生成线路
             */
            try
            {
                DataView dv = new DataView(dtLinJu);//创建DataView对象
                //获取或设置用于筛选在 System.Data.DataView 中查看行的表达式。
                dv.RowFilter = "site_id=" + strStartStationID;//根据站点ID=strStartStationID这个条件过滤数据。
                DataTable dtChild = dv.ToTable();//过滤的结果生成dtChild子数据表
                if (dtChild.Rows.Count > 0)
                {
                    for (int i = 0; i < dtChild.Rows.Count; i++)
                    {
                        //给相应变量赋值
                        string strLine = strAllLine;//线路名称
                        string strDistance = strAllDistance;//距离
                        string lstID = strAllID;//获取站点ID
                        string siteID = dtChild.Rows[i]["neighbor_site_id"].ToString();//把第i行的邻居站点id赋值给字符串siteID
                        if (lstID.Contains(siteID))//判断已经拼接的路线中是否包含这次循环的两个站点的字符串
                        {
                            continue;
                        }
                        lstID += ";" + siteID;
                        strLine += "-" + dtChild.Rows[i]["neighbor_site"].ToString().Trim();//把邻居站点的名称拼接到路线字符中
                        strDistance += ";" + dtChild.Rows[i]["distance"].ToString().Trim();//把两距离间拼接到路线字符中
                        deDistance += Convert.ToDecimal(dtChild.Rows[i]["distance"]);//把两站点的距离累加到整个路线的距离中。
                        if (siteID == JieZhiWangDian)//如果邻居站点ID是结束站点ID，则证明路线符合
                        {
                            //把该线路加到可选线路中 
                            DataRow dr = dtXianLu.NewRow();
                            dr["线路"] = strLine;
                            dr["距离"] = deDistance;
                            dr["站点ID集合"] = lstID;
                            dr["距离集合"] = strDistance;
                            dtXianLu.Rows.Add(dr);
                            continue;//跳出这一循环，不执行ShengChengXianLu()方法。
                        }
                        //递归:程序调用自身的编程技巧称为递归（ recursion）
                        ShengChengXianLu(siteID, strDistance, strLine, deDistance, lstID);//以siteID, strLine, Distance, lstID作为参数。
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
        //1.8 生成线路按钮
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //判断下列框数据是否为空（开始站点和结束站点）
            if (Convert.ToInt32(cbo_StarStation.SelectedValue) != 0 && Convert.ToInt32(cbo_EndStation.SelectedValue) != 0)
            {

                //给相应变量赋值（获取站点数据）
                QiShiWangDian = this.cbo_StarStation.SelectedValue.ToString().Trim();
                JieZhiWangDian = this.cbo_EndStation.SelectedValue.ToString().Trim();
                //实例化表对象并设定好结构
                dtXianLu = new DataTable();
                dtXianLu.Columns.Add("线路", typeof(string));
                dtXianLu.Columns.Add("距离", typeof(decimal));
                dtXianLu.Columns.Add("站点ID集合", typeof(string));
                dtXianLu.Columns.Add("距离集合", typeof(string));
                //查询邻接表(全部数据)
                dtLinJu = myClient.UserControl_Loaded_SelectSite().Tables[0];
                Distance = dtLinJu.Rows[0]["distance"].ToString();
                //执行生成线路方法（调用方法）    
                ShengChengXianLu(QiShiWangDian, Distance, cbo_StarStation.Text.Trim(), 0, QiShiWangDian);
                //判断>0
                if (dtXianLu.Rows.Count > 0)
                {
                    //绑定表格数据
                    DataView myDataView = new DataView(dtXianLu);
                    myDataView.Sort = "距离";//根据距离排序
                    DataTable dtXianShi = myDataView.ToTable();
                    //应用公共样式                  
                    dg_SelectLine.ItemsSource = dtXianShi.DefaultView;//把可选择的线路绑定到dgv
                }
                else
                {
                    MessageBox.Show("无法生成路线！");
                }
            }
            else
            {
                MessageBox.Show("数据为空！");
            }
        }
        //1.9 左边表（行改变事件）
        private void SelectLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //绑定线路明细信息
            if ((DataRowView)dg_SelectLine.CurrentItem != null)
            {
                JulI = Convert.ToDecimal(((DataRowView)dg_SelectLine.CurrentItem).Row["距离"]);
                string[] strStarStationId = ((DataRowView)dg_SelectLine.CurrentItem).Row["站点ID集合"].ToString().Split(';');
                string[] strDistant = ((DataRowView)dg_SelectLine.CurrentItem).Row["距离集合"].ToString().Split(';');
                string[] strStarStation = ((DataRowView)dg_SelectLine.CurrentItem).Row["线路"].ToString().Split('-');
                //实例化表对象并设置好四个列的属性
                DataTable myDataTable = new DataTable();
                myDataTable.Columns.Add("起始站点ID", typeof(string));
                myDataTable.Columns.Add("起始站点", typeof(string));
                myDataTable.Columns.Add("目的站点ID", typeof(string));
                myDataTable.Columns.Add("目的站点", typeof(string));
                myDataTable.Columns.Add("距离", typeof(string));
                for (int i = 0; i < strStarStation.Length; i++)//遍历strWangDianKaiShi数组，不包含最后一个字符，因为最后一个字符已经是终止网点了。
                {
                    if (i != strStarStation.Length - 1)
                    {
                        //实例化行对象，并且给对应单元格赋值
                        DataRow dr = myDataTable.NewRow();
                        dr["起始站点ID"] = strStarStationId[i];
                        dr["起始站点"] = strStarStation[i];
                        dr["距离"] = strDistant[i];
                        dr["目的站点ID"] = strStarStationId[i + 1];
                        dr["目的站点"] = strStarStation[i + 1];
                        myDataTable.Rows.Add(dr);//把行对象加到表对象中
                    }
                    else
                    { //实例化行对象，并且给对应单元格赋值
                        DataRow dr = myDataTable.NewRow();
                        dr["起始站点ID"] = strStarStationId[i];
                        dr["起始站点"] = strStarStation[i];
                        dr["距离"] = strDistant[i];
                        dr["目的站点ID"] = 0;
                        dr["目的站点"] = "无";
                        myDataTable.Rows.Add(dr);//把行对象加到表对象中
                    }                   
                    //绑定表格数据
                    dg_TheEndLine.ItemsSource = myDataTable.DefaultView;

                }
            }
        }
        //2.0 重置按钮
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            cbo_EndStation.SelectedValue = 0;
            cbo_StarStation.SelectedValue = 0;
            dg_SelectLine.ItemsSource = null;
            dg_TheEndLine.ItemsSource = null;
        }
        //2.1 保存按钮（保存线路）
        private void btn_Affirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //1、判断数据
                if ((dg_TheEndLine.ItemsSource as DataView).ToTable() != null && (dg_SelectLine.ItemsSource as DataView).ToTable() != null
                    && Convert.ToInt32(cbo_StarStation.SelectedValue) != 0
                    && Convert.ToInt32(cbo_EndStation.SelectedValue) != 0)
                {
                    //2、获取页面数据
                    DataTable dtEndLine = (dg_TheEndLine.ItemsSource as DataView).ToTable();
                    string strline_name = txt_LineName.Text.ToString().Trim();
                    string strsimple_code = txt_Code.Text.ToString();
                    string strmileage = JulI.ToString();
                    Boolean blstop_no = false;//默认启用
                    string note = "该线路为国家铁路局部门决定";
                    //1、线路表数据新增
                    DataTable dtLine = myClient.UserControl_Loaded_InsertLine(strline_name, strsimple_code,
                    strmileage, blstop_no, note).Tables[0];
                    if (dtLine.Rows.Count > 0)
                    {
                        intline_id = Convert.ToInt32(dtLine.Rows[0][0].ToString());//获取线路ID
                        //2、线路明细信息新增
                        int count = 0;
                        foreach (DataRow dr in dtEndLine.Rows)
                        {
                            int intsite_id = Convert.ToInt32(dr["起始站点ID"].ToString());
                            int intranking_sitea_id = Convert.ToInt32(dr["目的站点ID"].ToString());
                            decimal dedistance = Convert.ToDecimal(dr["距离"]);
                            count = myClient.UserControl_Loaded_InsertDetailLine(intline_id, intsite_id, intranking_sitea_id, dedistance);

                        }
                        if (count > 0)
                        {
                            MessageBoxResult dr = MessageBox.Show("生成线路成功！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
                            if (dr == MessageBoxResult.OK)//如果点了确定按钮
                            {
                                //关闭当前窗口                         
                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("线路新增重复，不做操作！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);//弹出确定对话框
                    }

                }
                else
                {
                    MessageBox.Show("请把数据填写完整！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);//弹出确定对话框
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //2.2 取消按钮（关闭窗口）
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }

    }
}
