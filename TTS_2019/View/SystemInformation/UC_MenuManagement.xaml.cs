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

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// UC_MenuManagement.xaml 的交互逻辑
    /// </summary>
    public partial class UC_MenuManagement : UserControl
    {
        public UC_MenuManagement()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 实例化服务引用
        /// </summary>
        BLL.UC_MenuManagement.UC_MenuManagementClient myClient = new BLL.UC_MenuManagement.UC_MenuManagementClient();
        /// <summary>
        /// 1.0 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCMenuManagement_Loaded(object sender, RoutedEventArgs e)
        {
            //调用数据查询
            GetData();
        }
        /// <summary>
        /// 1.1 查询表格数据
        /// </summary>
        public void GetData()
        {
            //获取数据
            DataTable dt = myClient.frmMenuManagement_SelectModular().Tables[0];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                #region 显示图片
                string strOldLuJing = (dt.Rows[i]["icon"]).ToString();
                string myPictureByte = myClient.UserControl_Loaded_SelectPhoro(strOldLuJing);
                if (myPictureByte != null)
                {
                    try
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        //增加这一行（指定位图图像如何利用内存缓存=在加载时将整个图像缓存到内存中。对图像数据的所有请求将通过内存存储区进行填充。）                
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.UriSource = new Uri(myPictureByte);
                        bi.EndInit();
                        //获取内存图片
                        dt.Rows[i]["iconUri"] = bi;
                    }
                    catch (Exception)
                    {
                        dt.Rows[i]["iconUri"] = null;
                    }
                }
                #endregion


            }


            //绑定表格
            dgMenuManagement.ItemsSource = dt.DefaultView;
        }
        /// <summary>
        /// 1.1 WPF下给DataGrid自动增加序列号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMenuManagement_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        /// <summary>
        /// 1.2 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_Select_SelectionChanged(object sender, RoutedEventArgs e)
        {
            
        }
        /// <summary>
        /// 1.3 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            //实例化窗口
            WD_InsertOrUpdateMenu WD_Menu = new WD_InsertOrUpdateMenu();
            WD_Menu.ShowDialog();
            //调用数据查询（刷新表格）
            GetData();
        }
       
        /// <summary>
        /// 1.4 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_Update(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //1.0 判断选中数据
                //获取选中行的ID
                int intModularId = Convert.ToInt32(((DataRowView)dgMenuManagement.SelectedItem).Row["modular_id"]);

                if (intModularId != 0)
                {
                    //1.1 数据回填（选中行数据）
                    WD_InsertOrUpdateMenu WD_Menu = new WD_InsertOrUpdateMenu((DataRowView)dgMenuManagement.SelectedItem);
                    WD_Menu.ShowDialog();
                    //调用数据查询（刷新表格）
                    GetData();                    
                }
            }
            catch (Exception)
            {
                return;
            }


            
        }
        /// <summary>
        /// 1.5 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_Delete(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //1、获取选中行的数据
                int intModularId = Convert.ToInt32(((DataRowView)dgMenuManagement.SelectedItem).Row["modular_id"]);
                string strpicture = ((DataRowView)dgMenuManagement.SelectedItem).Row["icon"].ToString().Trim();
                if (intModularId != 0) //如果菜单ID不为0
                {
                    MessageBoxResult dr = MessageBox.Show("是否删除当前菜单？", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);//弹出确定对话框
                    if (dr == MessageBoxResult.OK)//如果点了确定按钮
                    {

                        int count = myClient.frmMenuManagement_DeleteMenu(intModularId, strpicture); //执行删除事件                         
                        //2、执行删除操作(主键ID，图片)
                        if (count == -1)
                        {
                            MessageBox.Show("菜单使用中，不能删除！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                        }
                        else
                        {
                            //调用数据查询（刷新表格）
                            GetData();
                            MessageBox.Show("删除成功!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择要删除的行!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("请选择要删除的行!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }
    }
}
