using System;
using System.Data;
using System.IO;
using System.Windows;

namespace TTS_2019.View.SystemInformation.ReportForms
{
    /// <summary>
    /// WD_Staff.xaml 的交互逻辑
    /// </summary>
    public partial class WD_Staff : Window
    {
        #region 全局变量声明
        DataRowView drv;
        string strload = "";
        byte[] bytes;
        //实例化 服务
        BLL.UC_StaffInformation.UC_StaffInformationClient myClient = new BLL.UC_StaffInformation.UC_StaffInformationClient();
        #endregion
        public WD_Staff(DataRowView DRV)
        {
            //接收主页面传递过来的选中行数据
            drv = DRV;
            InitializeComponent();
        }
        /// <summary>
        /// 窗口Loaded事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 1、提取数据
            #region （1）、获取页面传递过来的数据
            //获取传递过来的员工ID
            int staff_id = Convert.ToInt32(drv.Row["staff_id"]);
            //创建临时数据表格dt1保存基本数据          
            DataTable dt1 = myClient.UserControl_Loaded_SelectWorkZheng(staff_id).Tables[0];
            //提取图片名字   
            string strpicture = dt1.Rows[0]["picture"].ToString().Trim();
            #endregion
            #region （2）、读取图片
            try
            {
                //获取图片路径
                strload = myClient.UserControl_Loaded_SelectPhoro(strpicture);
                //IO流读取图片
                FileStream filstream = new FileStream(strload, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                //IO流转换为byte[]
                bytes = new byte[filstream.Length];
                //从流中读取字节块并将该数据写入给定缓冲区中。
                filstream.Read(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {
                return;
            }
            #endregion           
            #region （3）、创建临时表格数据
            DataTable dt = new DataTable();//实例化数据表
            dt.Columns.Add("picture", typeof(byte[]));//给数据表创建对象
            DataRow myDataRow = dt.NewRow();//新建行          
            myDataRow["picture"] = bytes;//给单元格赋值：把读取的图片添加到数据行里面
            dt.Rows.Add(myDataRow);//给数据表添加行数据（获取图片）
            #endregion
            #endregion
            #region 2、合并数据集
            //实例化数据集
            DS_TTS myTTS = new DS_TTS();
            //把数据集和表格合并（给数据集绑定数据）
            myTTS.Tables["t_staff"].Merge(dt1);
            myTTS.Tables["t_picture"].Merge(dt);
            #endregion
            #region 3、实例化页面控件添加数据源
            //实例化水晶报表
            CRP_Staff myCRP_Staff = new CRP_Staff();
            //给水晶报表设置数据源
            myCRP_Staff.SetDataSource(myTTS);
            //打开第三方控件（绑定数据）
            CRV_Staff.ViewerCore.ReportSource = myCRP_Staff;
            #endregion
        }
    }
}
