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
    /// WD_InsertStaffInformation.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertStaffInformation
         : Window
    {
        public int intNumber;
        public WD_InsertStaffInformation(int intNB)
        {
            InitializeComponent();
            intNumber = intNB;//获取当前最大编号
        }
        #region 全局变量       
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        BLL.UC_StaffInformation.UC_StaffInformationClient myClient = new BLL.UC_StaffInformation.UC_StaffInformationClient();
        List<byte[]> lstBytes = new List<byte[]>();
        #endregion
        // 页面加载事件
        private void InsertStaffInformation_Loaded(object sender, RoutedEventArgs e)
        {
           
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

            #region 自动生成员工编号 
            int intNewNumber = intNumber + 1;
            string strNumber = intNewNumber.ToString();
            switch (strNumber.Length)
            {
                case 1:
                    strNumber = "000" + strNumber;
                    break;
                case 2:
                    strNumber = "00" + strNumber;
                    break;
                case 3:
                    strNumber = "0" + strNumber;
                    break;
                case 4:
                    strNumber = "" + strNumber;
                    break;
                default:
                    break;
            }
            txt_Number.Text = "YG" + strNumber;

            #endregion


        }
       
        // 身份证改变事件（验证有效身份证）
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
        // 文本改变事件（验证手机号码）
        private void txt_PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            /*
                  国内手机号码的规则:前3位为网络识别号；第4-7位为地区编码；第8-11位为用户号码。
                   现有手机号段:

                   移动：134 135 136 137 138 139 147 148 150 151 152 157 158 159 172 178 182 183 184 187 188 198
                   联通：130 131 132 145 146 155 156 166 171 175 176 185 186  
                   电信：133 149 153 173 174 177 180 181 189 199 
                   虚拟运营商：170

                   整理后：130~139    14[5-9]    15[012356789]    166   17[0-8]    18[0-9]    19[8-9]
               */

            /*
            
            */
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
        // 文本失去焦点事件（验证电子邮箱）
        private void txt_Email_LostFocus(object sender, RoutedEventArgs e)
        {
            string strEmail = txt_Email.Text.Trim();
            ////使用正则表达式判断是否匹配               
            if (!Regex.IsMatch(strEmail, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                MessageBox.Show("Email地址有误，请重新输入！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                txt_Email.Text = "";
            }
        }
        //浏览图片按钮
        private void btn_Open_Click(object sender, RoutedEventArgs e)
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
                        img_photo.Source = images;
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
        // 清空图片
        private void btn_Clean_Click(object sender, RoutedEventArgs e)
        {
            //清理元素
            lstBytes.Clear();
            //清空图片框
            img_photo.Source = null;
            //去除路径
            txt_Load.Text = string.Empty;
        }
        //绑定员工类型
        private void cbo_Post_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                int intFid = Convert.ToInt32(cbo_Post.SelectedValue);
                DataTable dt = myClient.UserControl_Loaded_SelectEmployeeType(intFid).Tables[0];
                cbo_StaffType.ItemsSource = dt.DefaultView;
                cbo_StaffType.DisplayMemberPath = "EmployeeType_name";
                cbo_StaffType.SelectedValuePath = "EmployeeType_id";
            }
            catch (Exception)
            {
                throw;
            }
        }
        // 新增保存事件
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
                if (cbo_StaffType.SelectedValue.ToString() != string.Empty && cbo_Work.SelectedValue.ToString() != string.Empty
                    && cbo_Post.SelectedValue.ToString() != string.Empty && txt_Name.Text.ToString() != string.Empty
                    && txt_Number.Text.ToString() != string.Empty && cbo_gender.SelectedValue.ToString() != string.Empty
                    && txt_idCar.Text.ToString() != string.Empty && dtp_Birthday.Text.ToString() != string.Empty
                    && dtp_EnterDate.Text.ToString() != string.Empty && dtp_LeaveDate.Text.ToString() != string.Empty)
                {
                    //1.获取页面输入的内容
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
                    bool bloperator_no = (bool)chk_operator.IsChecked;//获取checkbox
                    string strnote = txt_Note.Text.ToString();

                    //2.新增保存
                    int count = myClient.btn_Affirm_Click_InsertStaff(intas_employee_type_id, intas_work_status_id, intas_branch_id, strstaff_name,
                       strstaff_number, intas_gender_id, strage, strid_card, dtmbirth,
                     strphone_number, straddress, stre_mail, dtmentry_date, dtmdeparture_date, bloperator_no,
                      bytepicture, strnote);
                    if (count > 0)
                    {
                        MessageBoxResult dr = MessageBox.Show("新增员工资料成功！", "系统提示", MessageBoxButton.OKCancel,
                           MessageBoxImage.Information); //弹出确定对话框
                        if (dr == MessageBoxResult.OK) //如果点了确定按钮
                        {
                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63,
                         "新增【" + strstaff_name + "】员工资料", DateTime.Now);
                            //关闭当前窗口                  
                            this.Close();
                        }
                    }
                    else if (count == -1)
                    {
                        MessageBox.Show("账号重复！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }

                }
                else
                {
                    MessageBox.Show("员工资料还没完整填完，请继续！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // 取消保存
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
