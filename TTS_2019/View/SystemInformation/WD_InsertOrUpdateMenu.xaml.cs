using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_InsertOrUpdateMenu.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertOrUpdateMenu : Window
    {
        //实例化
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        BLL.UC_MenuManagement.UC_MenuManagementClient myClient = new BLL.UC_MenuManagement.UC_MenuManagementClient();
        DataRowView DGVR;//接收一行数据
        string myPictureByte;//接收图片路径
        List<string> lisWenJianMing = new List<string>();//接收图片
        List<byte[]> lstBytes = new List<byte[]>();
        string strOldLuJing;
        int intFid;
        bool blSwitch = false;//默认(false新增 ,true修改)
        /// <summary>
        /// 1、构造函数（新增）
        /// </summary>
        public WD_InsertOrUpdateMenu()
        {
            InitializeComponent();
        }
       
        /// <summary>
        /// 2、构造函数（修改）
        /// </summary>
        public WD_InsertOrUpdateMenu(DataRowView drv)
        {
            InitializeComponent();
            //接收主页面传递过来的数据
            DGVR = drv;
            //执行修改
            blSwitch = true;
        }
        /// <summary>
        /// 1.0 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //绑定下拉框（上级菜单）
            DataTable dtGender = myClient.frmMenuManagement_SelectLastMenu().Tables[0];
            cbo_FId.ItemsSource = dtGender.DefaultView;
            cbo_FId.DisplayMemberPath = "modular_name";
            cbo_FId.SelectedValuePath = "modular_id";

            //修改（回填数据）
            if (blSwitch)
            {
                //回填页面数据
                txt_Name.Text = (DGVR.Row["name"]).ToString();
                txt_Code.Text = (DGVR.Row["code"]).ToString();
                cbo_FId.SelectedValue = Convert.ToInt32((DGVR.Row["f_id"]).ToString());
                txt_Load.Text = (DGVR.Row["icon"]).ToString();
                #region 显示图片
                strOldLuJing = (DGVR.Row["icon"]).ToString();
                myPictureByte = myClient.UserControl_Loaded_SelectPhoro(strOldLuJing);
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
                        img_Icon.Source = bi;
                    }
                    catch (Exception)
                    {
                        img_Icon.Source = null;
                    }
                }
                #endregion
            }

        }

        /// <summary>
        /// 1.1 选择图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //声明两个局部变量
                Stream phpto = null;
                //1.1打开（文件框）
                OpenFileDialog ofdWenJian = new OpenFileDialog();
                //允许用户选择多个文件。               
                ofdWenJian.Multiselect = true;//多选图片
                                              //筛选文件类型（提示）
                ofdWenJian.Filter = "ALL Image Files|*.*";
                //显示对话框
                if ((bool)ofdWenJian.ShowDialog())
                {
                    //选定的文件(选定的文件打开只读流)
                    if ((phpto = ofdWenJian.OpenFile()) != null)
                    {
                        //获取文件长度（用字节表示的流长度 ）                                          
                        int length = (int)phpto.Length;
                        //声明数组
                        byte[] bytes = new byte[length];
                        //读取文件（字节数组，从零开始的字节偏移量，读取的字节数）
                        phpto.Read(bytes, 0, length);
                        lstBytes.Add(bytes);
                        BitmapImage images = new BitmapImage(new Uri(ofdWenJian.FileName));
                        //绑定图片
                        img_Icon.Source = images;
                        txt_Load.Text = ofdWenJian.FileName;
                    }
                }
                else
                {
                    MessageBox.Show("对话框没有显示，没办法选择图片！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("绑定图片出错，请重新选择！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 1.2 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //提取上传的文件
                byte[][] bytepicture = new byte[lstBytes.Count][];
                for (int i = 0; i < lstBytes.Count; i++)
                {
                    bytepicture[i] = lstBytes[i];
                }
                //0.判断必填项不能为空
                if (txt_Name.Text.ToString() != string.Empty  && txt_Code.Text.ToString() != string.Empty && cbo_FId.SelectedValue.ToString() != string.Empty)
                {
                    //1.获取页面输入的内容
                    string strmodular_name = txt_Name.Text.ToString();
                    string strmodular_code = txt_Code.Text.ToString();
                    int intf_id = Convert.ToInt32(cbo_FId.SelectedValue);
                    
                    int count = 0;
                    string strMBR = "";//提示
                    if (blSwitch)
                    {
                        int intmodular_id = Convert.ToInt32(DGVR.Row["modular_id"]);//获取主键ID
                        string strTxtLuJing = txt_Load.Text.ToString().Trim();//获取路径
                        //2.修改保存
                        count = myClient.frmMenuManagement_UpdatetMenu(strmodular_name, strmodular_code, bytepicture, intf_id, intmodular_id, strOldLuJing, strTxtLuJing);
                        strMBR = "修改菜单成功！";
                    }
                    else
                    {
                        //3.新增保存
                        count = myClient.frmMenuManagement_InsertMenu(strmodular_name, strmodular_code, bytepicture, intf_id);
                        strMBR = "新增菜单成功！";
                    }
                    if (count > 0)
                    {
                        MessageBoxResult dr = MessageBox.Show(strMBR, "系统提示", MessageBoxButton.OKCancel,
                           MessageBoxImage.Information); //弹出确定对话框
                        if (dr == MessageBoxResult.OK) //如果点了确定按钮
                        {
                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63,
                         "新增【" + strmodular_name + "】菜单", DateTime.Now);
                            //关闭当前窗口                  
                            this.Close();
                        }
                    }
                    else if (count == -1)
                    {
                        MessageBox.Show("菜单重复！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }

                }
                else
                {
                    MessageBox.Show("菜单资料还没完整填完，请继续！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 1.3 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
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
