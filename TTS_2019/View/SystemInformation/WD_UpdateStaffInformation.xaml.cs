using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_UpdateStaffInformation.xaml 的交互逻辑
    /// </summary>
    public partial class WD_UpdateStaffInformation : Window
    {
        #region 全局变量
        DataRowView DGVR;//接收一行数据
        string myPictureByte;//接收图片路径
        List<string> lisWenJianMing = new List<string>();
        List<byte[]> lstBytes = new List<byte[]>();
        string strOldLuJing;
        int intFid;
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        BLL.UC_StaffInformation.UC_StaffInformationClient myClient = new BLL.UC_StaffInformation.UC_StaffInformationClient();
        #endregion
        //1.0 构造函数：函数名与类名相同的函数
        public WD_UpdateStaffInformation(DataRowView drv)
        {
            InitializeComponent();
            //接收主页面传递过来的数据
            DGVR = drv;
        }
        //1.1 页面加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //1.0绑定下拉框

            #region 绑定性别
            DataTable dtGender = myClient.InsertStaffInformation_Loaded_SelectGender().Tables[0];
            cbo_gender.ItemsSource = dtGender.DefaultView;
            cbo_gender.DisplayMemberPath = "Gender_name";
            cbo_gender.SelectedValuePath = "Gender_id";
            #endregion
            #region 绑定工作状态
            DataTable dtWorke = myClient.InsertStaffInformation_Loaded_SelectWorkStatus().Tables[0];
            cbo_Work.ItemsSource = dtWorke.DefaultView;
            cbo_Work.DisplayMemberPath = "WorkStatus_name";
            cbo_Work.SelectedValuePath = "WorkStatus_id";
            #endregion
            #region 绑定机构部门
            DataTable dtPost = myClient.InsertStaffInformation_Loaded_SelectPost().Tables[0];
            cbo_Post.ItemsSource = dtPost.DefaultView;
            cbo_Post.DisplayMemberPath = "Post_name";
            cbo_Post.SelectedValuePath = "Post_id";
            #endregion
            //1.1页面数据回填
            #region 跨界面传输数据
            txt_Number.Text = (DGVR.Row["staff_number"]).ToString();
            txt_Name.Text = (DGVR.Row["staff_name"]).ToString();
            cbo_Post.SelectedValue = Convert.ToInt32(DGVR.Row["as_branch_id"]);
            txt_PhoneNumber.Text = (DGVR.Row["phone_number"]).ToString();
            cbo_Work.SelectedValue = Convert.ToInt32(DGVR.Row["as_work_status_id"]);
            cbo_gender.SelectedValue = Convert.ToInt32(DGVR.Row["as_gender_id"]);
            //cbo_StaffType.SelectedValue = Convert.ToInt32(DGVR.Row["as_employee_type_id"]);
            txt_idCar.Text = (DGVR.Row["id_card"]).ToString();
            dtp_Birthday.Text = (DGVR.Row["birth"]).ToString();
            txt_Age.Text = (DGVR.Row["age"]).ToString();
            txt_Address.Text = (DGVR.Row["address"]).ToString();
            txt_Email.Text = (DGVR.Row["e_mail"]).ToString();
            dtp_EnterDate.Text = (DGVR.Row["entry_date"]).ToString();
            dtp_LeaveDate.Text = (DGVR.Row["departure_date"]).ToString();
            if (DGVR.Row["operator_no"].ToString() == "操作员")
            {
                chk_operator.IsChecked = true;
            }
            txt_Note.Text = (DGVR.Row["note"]).ToString();
            txt_Load.Text = (DGVR.Row["picture"]).ToString();
            #region 显示图片
            strOldLuJing = (DGVR.Row["picture"]).ToString();
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
                    img_photo.Source = bi;
                }
                catch (Exception)
                {
                    img_photo.Source = null;
                }
            }
            #endregion
            #endregion
        }
        //1.2 部门下拉框改变事件（绑定工作状态）
        private void cbo_Post_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (Convert.ToInt32(cbo_Post.SelectedValue) != 0)
                {
                    intFid = Convert.ToInt32(cbo_Post.SelectedValue);                   
                }
                else
                {
                    intFid = Convert.ToInt32(DGVR.Row["as_branch_id"]);
                }

                DataTable dt = myClient.UserControl_Loaded_SelectEmployeeType(intFid).Tables[0];
                cbo_StaffType.ItemsSource = dt.DefaultView;
                cbo_StaffType.DisplayMemberPath = "EmployeeType_name";
                cbo_StaffType.SelectedValuePath = "EmployeeType_id";
                cbo_StaffType.SelectedValue = Convert.ToInt32(DGVR.Row["as_employee_type_id"]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //1.3 验证电话号码
        private void txt_PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strPhoneNumber = txt_PhoneNumber.Text.Trim();
            if (strPhoneNumber.Length == 11)
            {
                ////使用正则表达式判断是否匹配               
                if (!Regex.IsMatch(strPhoneNumber, @"^0?(13[0-9]|14[5-9]|15[012356789]|166|17[0-8]|18[0-9]|19[89])[0-9]{8}$"))
                {
                    MessageBox.Show("手机号格式不对，请重新输入！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    txt_PhoneNumber.Text = "";
                }

            }
        }
        //1.4 验证身份证号
        private void txt_idCar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strIdCard = txt_idCar.Text.Trim();
            #region 原来方法 
            try
            {
                if (strIdCard.Length == 18)
                {

                    //闰年出生日期的合法性正则表达式 || 平年出生日期的合法性正则表达式 
                    if (!Regex.IsMatch(strIdCard, @"(^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9Xx]$)") || !Regex.IsMatch(strIdCard, @"(^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9Xx]$)"))
                    {
                        MessageBox.Show("身份证不合法！");
                        txt_idCar.Text = "";
                    }
                    else
                    {
                        string keys = strIdCard;
                        ////获取地址码
                        //string dmzm = keys.Substring(0, 6);
                        //性别
                        int sex = int.Parse(keys.Substring(16, 1));
                        //年
                        string birth_y = keys.Substring(6, 4);
                        //月
                        string birth_m = keys.Substring(10, 2);
                        //日
                        string birth_d = keys.Substring(12, 2);
                        ListViewItem l = new ListViewItem();
                        //绑定出生日期
                        dtp_Birthday.Text = birth_y + "年" + birth_m + "月" + birth_d + "日";
                        //l.Content = i.dmmc + "公安局";
                        //l.SubItems.Add(i.dmmcl);
                        //获取今年年份
                        string strNow = DateTime.Now.Year.ToString();
                        //把今年转化成数字
                        decimal decNow = Convert.ToDecimal(strNow);
                        //获取（截取身份证）出生年份
                        decimal decbirth_y = Convert.ToDecimal(birth_y);
                        //获取虚岁
                        decimal decAge = Convert.ToDecimal(decNow - decbirth_y) + 1;
                        //绑定年龄
                        txt_Age.Text = decAge.ToString().Trim();
                        //取余
                        if (sex % 2 == 0)
                        {
                            //l.SubItems.Add("女");
                            cbo_gender.SelectedValue = 77;//77跟下拉框ID值对应
                        } //if
                        else
                        {
                            //l.SubItems.Add("男");
                            cbo_gender.SelectedValue = 76;//76跟下拉框ID值对应
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show("输入的身份证不正确。");
            }
            #endregion
            #region 使用高级语法
            if (txt_idCar.Text.ToString().Length == 6)
            {

                string strAddress = CheckIDCardGetDiQu.LoadAddress(txt_idCar.Text.ToString());
                if (strAddress == "")
                {
                    MessageBox.Show("身份证不合法！");
                }
                else
                {
                    txt_Address.Text = strAddress;
                }
            }
            if (txt_idCar.Text.ToString().Length < 6)
            {
                txt_Address.Text = "";
            }

            #endregion
        }
        //1.5 选择图片
        private void btn_Open_Click(object sender, RoutedEventArgs e)
        {
            Stream phpto = null;
            int length;
            OpenFileDialog ofdWenJian = new OpenFileDialog();
            //允许获取获取多张图片
            ofdWenJian.Multiselect = true;//多选图片
            ofdWenJian.Filter = "ALL Image Files|*.*";
            if ((bool)ofdWenJian.ShowDialog())
            {
                if ((phpto = ofdWenJian.OpenFile()) != null)
                {
                    length = (int)phpto.Length;
                    byte[] bytes = new byte[length];
                    phpto.Read(bytes, 0, length);
                    lstBytes.Add(bytes);
                    BitmapImage images = new BitmapImage(new Uri(ofdWenJian.FileName));
                    img_photo.Source = images;
                    txt_Load.Text = ofdWenJian.FileName;
                }
            }
            else
            {
                MessageBox.Show("图片为空！");
            }
        }
        //1.6 取消图片选择
        private void btn_Clean_Click(object sender, RoutedEventArgs e)
        {
            lstBytes.Clear();
            img_photo.Source = null;
            txt_Load.Text = string.Empty;
        }
        //1.7 保存修改
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //第一步：判断页面数据
                if (cbo_gender.SelectedValue.ToString() != "" && cbo_Post.SelectedValue.ToString() != ""
                && cbo_StaffType.SelectedValue.ToString() != ""
               && cbo_Work.SelectedValue.ToString() != "" && txt_Address.Text != "" && txt_Age.Text != "" && txt_Email.Text != ""
               && txt_idCar.Text != "" && txt_Name.Text != "" && txt_Note.Text != "" &&
               txt_Number.Text != "" && txt_PhoneNumber.Text != "" && dtp_Birthday.Text.ToString() != "" &&
               dtp_EnterDate.Text.ToString() != "")
                {
                    //第二步：获取页面数据
                    int intas_employee_type_id = Convert.ToInt32(cbo_StaffType.SelectedValue);
                    int intas_work_status_id = Convert.ToInt32(cbo_Work.SelectedValue);
                    int intas_branch_id = Convert.ToInt32(cbo_Post.SelectedValue);
                    string strstaff_name = txt_Name.Text.ToString();
                    string strstaff_number = txt_Number.Text.ToString();
                    int intas_gender_id = Convert.ToInt32(cbo_gender.SelectedValue);
                    string strage = txt_Age.Text.ToString();
                    string strid_card = txt_idCar.Text.ToString();
                    DateTime dtmbirth = Convert.ToDateTime(dtp_Birthday.Text.ToString());
                    string strphone_number = txt_PhoneNumber.Text.ToString();
                    string straddress = txt_Address.Text.ToString();
                    string stre_mail = txt_Email.Text.ToString();
                    DateTime dtmentry_date = Convert.ToDateTime(dtp_EnterDate.Text.ToString());
                    DateTime dtmdeparture_date = Convert.ToDateTime(dtp_LeaveDate.Text.ToString());
                    bool bloperator_no = (bool)chk_operator.IsChecked;
                    string strnote = txt_Note.Text.ToString();
                    int intstaff_id = Convert.ToInt32(DGVR.Row["staff_id"]);
                    string strTxtLuJing = txt_Load.Text.ToString().Trim();//获取路径
                    byte[][] bytWenJian = new byte[lstBytes.Count][];
                    for (int i = 0; i < lstBytes.Count; i++)
                    {
                        bytWenJian[i] = lstBytes[i];
                    }
                    //第三步：调用服务器保存方法
                    int intCount = myClient.btn_Affirm_Click_UpdateStaff(intas_employee_type_id, intas_work_status_id, intas_branch_id, strstaff_name,
                  strstaff_number, intas_gender_id, strage, strid_card, dtmbirth, strphone_number, straddress, stre_mail, dtmentry_date, dtmdeparture_date, bloperator_no,
                   bytWenJian, strnote, intstaff_id, strOldLuJing, strTxtLuJing);
                    //第四步：根据保存情况做提示
                    if (intCount > 0)
                    {
                        MessageBoxResult dr = MessageBox.Show("修改【" + strstaff_number + "】员工信息！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
                        if (dr == MessageBoxResult.OK)//如果点了确定按钮
                        {
                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 64, "修改【" + strstaff_name + "】员工信息", DateTime.Now);
                            //关闭当前窗口                      
                            this.Close();
                        }
                    }
                    else if (intCount == -1)
                    {
                        MessageBox.Show("修改员工信息重复！");
                    }
                }
                else
                {
                    MessageBox.Show("请检查是否存在空数据。");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        //1.8 取消
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
