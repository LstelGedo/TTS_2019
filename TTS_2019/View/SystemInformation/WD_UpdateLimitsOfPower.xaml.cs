using System;
using System.Data;
using System.Windows;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_UpdateLimitsOfPower.xaml 的交互逻辑
    /// </summary>
    public partial class WD_UpdateLimitsOfPower : Window
    {
        DataRowView DRV = null;
        public WD_UpdateLimitsOfPower(DataRowView drvLimits)
        {
            InitializeComponent();            
            DRV = drvLimits;
        }
        BLL.UC_PowerManage.UC_PowerManageClient myClient = new BLL.UC_PowerManage.UC_PowerManageClient();
        DataTable dt = null;
        DataTable dtOld;//记录修改前的表格数据(临时表)
        int intPGroupId;//修改主键ID

        //1.0 页面加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           
            //页面数据绑定
            dt = myClient.limitsOfPower_Loaded_SelectAllModular().Tables[0];
            dgModel.ItemsSource = dt.DefaultView;
            #region 页面数据回填
            intPGroupId = Convert.ToInt32(DRV.Row["p_group_id"].ToString().Trim());
            txt_Name.Text = DRV.Row["p_name"].ToString().Trim();
            txt_Remark.Text = DRV.Row["remarks"].ToString().Trim();
            string strPermissionGroup = DRV.Row["p_group"].ToString().Trim();
            if (strPermissionGroup != string.Empty)
            {
                //查询修改前数据
                dtOld = myClient.limitsOfPower_Loaded_SelectModular(strPermissionGroup).Tables[0];

                //循环全部数据
                for (int i = 0; i < dt.Rows.Count; i++) //dt 代表表格数据（全部数据）
                {
                    //循环修改前模块
                    for (int j = 0; j < dtOld.Rows.Count; j++)//dtOld一部分数据
                    {
                        if (Convert.ToInt32(dt.Rows[i]["modular_id"]) == Convert.ToInt32(dtOld.Rows[j]["modular_id"]))
                        {
                            //绑定表格数值（单元格数值赋值）
                            dt.Rows[i]["chked"] = true;

                            DataTable dtOperation = myClient.limitsOfPower_Loaded_SelectOperation(dtOld.Rows[j]["as_operation_id"].ToString().Trim()).Tables[0];
                            //循环修改前的操作
                            for (int k = 0; k < dtOperation.Rows.Count; k++)//模块对应操作
                            {
                                int intID = Convert.ToInt32(dtOperation.Rows[k]["Id"].ToString());
                                if (intID == 62)
                                {
                                    dt.Rows[i]["SelectID"] = true;
                                }
                                else if (intID == 63)
                                {
                                    dt.Rows[i]["InsertID"] = true;
                                }
                                else if (intID == 64)
                                {
                                    dt.Rows[i]["UpdateID"] = true;
                                }
                                else if (intID == 65)
                                {
                                    dt.Rows[i]["DeleteID"] = true;
                                }
                            }

                        }
                    }
                }
            }
            #endregion
        }

        //1.1 保存按钮（单击事件）
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                //获取页面数据判断不能为空
                if (txt_Name.Text.ToString() != "" && txt_Remark.Text.ToString() != "")
                {
                    //第一步：权限组表（执行修改）
                    string strPname = txt_Name.Text.ToString().Trim();
                    string strRemarks = txt_Remark.Text.ToString().Trim();
                    //(1) 循环勾选的表格信息 --获取模块ID
                    string strPgroup = string.Empty;
                    //第二步:删除原来的明细，新增现在的
                    //(2)新增权限操作                    
                    for (int i = 0; i < dgModel.Items.Count; i++)
                    {
                        if (Convert.ToBoolean(dt.Rows[i]["chked"]) == true)
                        {
                            // 获取模块ID
                            int intFid = Convert.ToInt32(((DataRowView)dgModel.Items[i]).Row["modular_id"]);
                            string strAsOperationId = string.Empty;
                            // 获取操作ID
                            if (Convert.ToBoolean(dt.Rows[i]["SelectID"]) == true)
                            {
                                strAsOperationId += "62,";
                            }
                            if (Convert.ToBoolean(dt.Rows[i]["InsertID"]) == true)
                            {
                                strAsOperationId += "63,";
                            }
                            if (Convert.ToBoolean(dt.Rows[i]["UpdateID"]) == true)
                            {
                                strAsOperationId += "64,";
                            }
                            if (Convert.ToBoolean(dt.Rows[i]["DeleteID"]) == true)
                            {
                                strAsOperationId += "65,";
                            }
                            //(3) --新增模块操作
                            DataTable dtModularDetailId = myClient.Window_Loaded_UInsertModularOperation(intFid, strAsOperationId,strPname,intPGroupId).Tables[0];
                            if (dtModularDetailId.Rows.Count > 0)
                            {
                                //(4) --提取操作模块ID 拼接
                                //获取单元格（站点ID）                           
                                strPgroup += dtModularDetailId.Rows[0][0].ToString().Trim() + ',';
                            }
                            else {
                                MessageBox.Show("已存在当前权限组，请重新输入！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Error);
                            }                            
                        }
                    }
                    //（执行修改）
                    int intCount = myClient.Window_Loaded_UpdatePermissionGroup(strPname, strPgroup, strRemarks, intPGroupId);
                    if (intCount == -1)
                    {
                        MessageBox.Show("已存在当前权限组，请重新输入！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (intCount > 0)
                    {
                       
                        //（5）、批量删除模块操作明细表
                        for (int i = 0; i < dtOld.Rows.Count; i++)
                        {
                            //获取主键ID
                            int intModularDetailId = Convert.ToInt32(dtOld.Rows[i]["modular_detail_id"]);
                            int intNeighborCount = myClient.Window_Loaded_DeleteModularOperation(intModularDetailId);
                        }
                        MessageBox.Show("权限组：" + strPname+"修改成功！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Information);
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
        //1.2 取消按钮（单击事件）
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel,
                 MessageBoxImage.Information); //弹出确定对话框
            if (dr == MessageBoxResult.OK) //如果点了确定按钮
            {
                //关闭窗口
                this.Close();
            }
        }
    }
}
