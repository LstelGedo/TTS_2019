using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using TTS_2019.Tools.Utils;

namespace TTS_2019.View.SystemInformation
{
    /// <summary>
    /// WD_InsertTravellerInformation.xaml 的交互逻辑
    /// </summary>
    public partial class WD_InsertTravellerInformation : Window
    {
        //选中行（是新增和修改的判断依据）
        DataRowView DGVR;
        //一 新增窗口
        public WD_InsertTravellerInformation()
        {
            InitializeComponent();
            DGVR = null;
        }
        //二 修改窗口     
        public WD_InsertTravellerInformation(DataRowView dgvr)
        {
            InitializeComponent();
            DGVR = dgvr;
        }
        
        #region 全局变量
        BLL.UC_TravellerInformation.UC_TravellerInformationClient myClient = new BLL.UC_TravellerInformation.UC_TravellerInformationClient();
        BLL.PublicFunction.PublicFunctionClient myPublicFunctionClient = new BLL.PublicFunction.PublicFunctionClient();
        #endregion
        //1.0 页面加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region 新增窗口和修改窗口公共部分
            dtp_RegisterDate.Text = DateTime.Now.ToString();
            //1 绑定证件类型
            DataTable dt = myClient.UserControl_Loaded_SelectCertificateType().Tables[0];
            cbo_CertificateType.ItemsSource = dt.DefaultView;
            cbo_CertificateType.DisplayMemberPath = "CertificateType_name";
            cbo_CertificateType.SelectedValuePath = "CertificateType_id";
           //2 绑定旅客类型
            DataTable dtTravellerType = myClient.UserControl_Loaded_SelectTravellerType().Tables[0];
            cbo_TravellerType.ItemsSource = dtTravellerType.DefaultView;
            cbo_TravellerType.DisplayMemberPath = "TravellerType_name";
            cbo_TravellerType.SelectedValuePath = "TravellerType_id";
            //3 绑定性别
            DataTable dtGender = myClient.InsertTravellerInformation_Loaded_SelectGender().Tables[0];
            cbo_Gender.ItemsSource = dtGender.DefaultView;
            cbo_Gender.DisplayMemberPath = "Gender_name";
            cbo_Gender.SelectedValuePath = "Gender_id";
            #endregion
            //(1)判断当前窗口是新增还是修改（）
            if (DGVR!=null)
            {
                //修改窗口(回填数据)
                #region 跨界面传输数据
                txt_Age.Text = (DGVR.Row["age"]).ToString();
                txt_CertificateType.Text = (DGVR.Row["certificate_number"]).ToString();
                txt_Contry.Text = (DGVR.Row["country"]).ToString();
                txt_address.Text = (DGVR.Row["address"]).ToString();
                txt_emergency_address.Text = (DGVR.Row["emergency_address"]).ToString();
                txt_emergency_phone.Text = (DGVR.Row["emergency_phone"]).ToString();
                txt_Name.Text = (DGVR.Row["name"]).ToString();
                txt_phone.Text = (DGVR.Row["phone_number"]).ToString();
                txt_zip_code.Text = (DGVR.Row["zip_code"]).ToString();
                dtp_Birthday.Text = (DGVR.Row["birthday"]).ToString();
                dtp_RegisterDate.Text = (DGVR.Row["register_date"]).ToString();
                cbo_CertificateType.SelectedValue = Convert.ToInt32(DGVR.Row["as_certificate_type_id"]);
                cbo_Gender.SelectedValue = Convert.ToInt32(DGVR.Row["as_gender_id"]);
                cbo_TravellerType.SelectedValue = Convert.ToInt32(DGVR.Row["as_passenger_type_id"]);
                #endregion

            }
            else
            {
                //新增窗口（不作操作）
            }

        }
        //1.1 身份证号码验证
        private void txt_CertificateType_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strIdCard = txt_CertificateType.Text.Trim();
            #region 获取地址
            if (txt_CertificateType.Text.ToString().Length == 6)
            {

                string strAddress = CheckIDCardGetDiQu.LoadAddress(txt_CertificateType.Text.ToString());
                if (strAddress == "")
                {
                    MessageBox.Show("身份证不合法！");
                }
                else
                {
                    txt_address.Text = strAddress;
                }
            }
            if (txt_CertificateType.Text.ToString().Length < 6)
            {
                txt_address.Text = "";
            }

            #endregion
            #region 验证身份证
            try
            {
                if (strIdCard.Length == 18)
                {

                    //闰年出生日期的合法性正则表达式 || 平年出生日期的合法性正则表达式 
                    if (!Regex.IsMatch(strIdCard, @"(^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9Xx]$)") || !Regex.IsMatch(strIdCard, @"(^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9Xx]$)"))
                    {
                        MessageBox.Show("身份证不合法！");
                        txt_CertificateType.Text = "";
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
                            cbo_Gender.SelectedValue = 77;//77跟下拉框ID值对应
                        } //if
                        else
                        {
                            //l.SubItems.Add("男");
                            cbo_Gender.SelectedValue = 76;//76跟下拉框ID值对应
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show("输入的身份证不正确。");
            }
            #endregion
            
        }
        //1.2 联系电话验证
        private void txt_phone_TextChanged(object sender, TextChangedEventArgs e)
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
            string strPhoneNumber = txt_phone.Text.Trim();
            if (strPhoneNumber.Length == 11)
            {
                ////使用正则表达式判断是否匹配               
                if (!Regex.IsMatch(strPhoneNumber, @"^0?(13[0-9]|14[5-9]|15[012356789]|166|17[0-8]|18[0-9]|19[89])[0-9]{8}$"))
                {
                    MessageBox.Show("手机号格式不对，请重新输入！", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    txt_phone.Text = "";
                }

            }
        }
        //1.3 保存按钮
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbo_CertificateType.SelectedValue.ToString() != "" && cbo_TravellerType.SelectedValue.ToString() != "" &&
                cbo_Gender.SelectedValue.ToString() != "" && txt_Name.Text != "" && txt_Age.Text != "" && txt_CertificateType.Text != ""
               && txt_Contry.Text != "" && txt_CertificateType.Text != "" && txt_Name.Text != "" && txt_phone.Text != "" &&
               txt_emergency_phone.Text != "" && dtp_Birthday.Text.ToString() != "" && txt_address.Text.ToString() != "" &&
               txt_emergency_address.Text.ToString() != "" && txt_zip_code.Text.ToString() != ""
               && dtp_Birthday.Text.ToString() != "")
                {
                    int intas_certificate_type_id = Convert.ToInt32(cbo_CertificateType.SelectedValue);
                    int intas_passenger_type_id = Convert.ToInt32(cbo_TravellerType.SelectedValue);
                    string strtraveller_name = txt_Name.Text.ToString();
                    int intas_gender_id = Convert.ToInt32(cbo_Gender.SelectedValue);
                    DateTime date_of_birth = Convert.ToDateTime(dtp_Birthday.Text.ToString());
                    string strage = txt_Age.Text.ToString();
                    string strcountry_or_region = txt_Contry.Text.ToString();
                    string strcertificate_number = txt_CertificateType.Text.ToString();
                    string strphone_number = txt_phone.Text.ToString();
                    string stremergency_phone = txt_emergency_phone.Text.ToString();
                    string straddress = txt_address.Text.ToString();
                    string stremergency_address = txt_emergency_address.Text.ToString();
                    string strzip_code = txt_zip_code.Text.ToString();
                    DateTime register_date = Convert.ToDateTime(dtp_RegisterDate.Text.ToString());
                    //判断按钮执行新增或者修改
                    int count = 0;
                    if (DGVR != null)
                    {
                        //修改保存
                        count = myClient.btn_Affirm_Click_UpdateTraveller(intas_certificate_type_id, intas_passenger_type_id, intas_gender_id, strtraveller_name, date_of_birth, strage, strcountry_or_region, strcertificate_number, strphone_number, straddress, stremergency_phone, stremergency_address, strzip_code, register_date, Convert.ToInt32(DGVR.Row["traveller_id"]));
                    }
                    else
                    {
                        //新增保存
                        count = myClient.btn_Affirm_Click_InsertTraveller(intas_certificate_type_id, intas_passenger_type_id, intas_gender_id, strtraveller_name, date_of_birth, strage, strcountry_or_region, strcertificate_number, strphone_number, straddress, stremergency_phone, stremergency_address, strzip_code, register_date);

                    }
                    #region 判断结果（提示）
                    if (count > 0)
                    {
                        //系统操作日志
                        if (DGVR != null)
                        {
                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63, "修改【" + strtraveller_name + "】旅客信息", DateTime.Now);
                        }
                        else
                        {
                            myPublicFunctionClient.InsertSystem_operation_log(LoginWindow.intStaffID, 63, "新增【" + strtraveller_name + "】旅客信息", DateTime.Now);
                        }
                        //系统提示
                        MessageBox.Show("成功编辑员工资料", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else if (count == -1)
                    {
                        MessageBox.Show("旅客信息重复！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("旅客信息还没完整填完，请继续", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    #endregion
                }
                else
                {
                    MessageBox.Show("旅客信息还没完整填完，请继续!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //1.4 取消按钮
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("退出界面数据将不保留。", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);//弹出确定对话框
            if (dr == MessageBoxResult.OK)//如果点了确定按钮
            {
                this.Close();
            }
        }
    }
}
