﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace TTS_2019.BLL.Login {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BLL.Login.Login")]
    public interface Login {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectLogin", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectLoginResponse")]
        System.Data.DataSet frmLogin_btn_Login_Click_SelectLogin(string strAccounts, string strPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectLogin", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectLoginResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_SelectLoginAsync(string strAccounts, string strPassword);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/UserControl_Loaded_SelectTrainSum", ReplyAction="http://tempuri.org/Login/UserControl_Loaded_SelectTrainSumResponse")]
        System.Data.DataSet UserControl_Loaded_SelectTrainSum();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/UserControl_Loaded_SelectTrainSum", ReplyAction="http://tempuri.org/Login/UserControl_Loaded_SelectTrainSumResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> UserControl_Loaded_SelectTrainSumAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/UserControl_Loaded_SelectTravellerCountrySum", ReplyAction="http://tempuri.org/Login/UserControl_Loaded_SelectTravellerCountrySumResponse")]
        System.Data.DataSet UserControl_Loaded_SelectTravellerCountrySum();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/UserControl_Loaded_SelectTravellerCountrySum", ReplyAction="http://tempuri.org/Login/UserControl_Loaded_SelectTravellerCountrySumResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> UserControl_Loaded_SelectTravellerCountrySumAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectModular", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectModularResponse")]
        System.Data.DataSet frmLogin_btn_Login_Click_SelectModular(string strModularDetailIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectModular", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_SelectModularResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_SelectModularAsync(string strModularDetailIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_NotInModular", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_NotInModularResponse")]
        System.Data.DataSet frmLogin_btn_Login_Click_Select_NotInModular(string strModularDetailIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_NotInModular", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_NotInModularResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_Select_NotInModularAsync(string strModularDetailIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_Picture", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_PictureResponse")]
        System.Data.DataSet frmLogin_btn_Login_Click_Select_Picture(int intStaff_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_Picture", ReplyAction="http://tempuri.org/Login/frmLogin_btn_Login_Click_Select_PictureResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_Select_PictureAsync(int intStaff_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/Main1_SelectAllModular", ReplyAction="http://tempuri.org/Login/Main1_SelectAllModularResponse")]
        System.Data.DataSet Main1_SelectAllModular(string strModularDetailIds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Login/Main1_SelectAllModular", ReplyAction="http://tempuri.org/Login/Main1_SelectAllModularResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> Main1_SelectAllModularAsync(string strModularDetailIds);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface LoginChannel : TTS_2019.BLL.Login.Login, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LoginClient : System.ServiceModel.ClientBase<TTS_2019.BLL.Login.Login>, TTS_2019.BLL.Login.Login {
        
        public LoginClient() {
        }
        
        public LoginClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LoginClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoginClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoginClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataSet frmLogin_btn_Login_Click_SelectLogin(string strAccounts, string strPassword) {
            return base.Channel.frmLogin_btn_Login_Click_SelectLogin(strAccounts, strPassword);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_SelectLoginAsync(string strAccounts, string strPassword) {
            return base.Channel.frmLogin_btn_Login_Click_SelectLoginAsync(strAccounts, strPassword);
        }
        
        public System.Data.DataSet UserControl_Loaded_SelectTrainSum() {
            return base.Channel.UserControl_Loaded_SelectTrainSum();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> UserControl_Loaded_SelectTrainSumAsync() {
            return base.Channel.UserControl_Loaded_SelectTrainSumAsync();
        }
        
        public System.Data.DataSet UserControl_Loaded_SelectTravellerCountrySum() {
            return base.Channel.UserControl_Loaded_SelectTravellerCountrySum();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> UserControl_Loaded_SelectTravellerCountrySumAsync() {
            return base.Channel.UserControl_Loaded_SelectTravellerCountrySumAsync();
        }
        
        public System.Data.DataSet frmLogin_btn_Login_Click_SelectModular(string strModularDetailIds) {
            return base.Channel.frmLogin_btn_Login_Click_SelectModular(strModularDetailIds);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_SelectModularAsync(string strModularDetailIds) {
            return base.Channel.frmLogin_btn_Login_Click_SelectModularAsync(strModularDetailIds);
        }
        
        public System.Data.DataSet frmLogin_btn_Login_Click_Select_NotInModular(string strModularDetailIds) {
            return base.Channel.frmLogin_btn_Login_Click_Select_NotInModular(strModularDetailIds);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_Select_NotInModularAsync(string strModularDetailIds) {
            return base.Channel.frmLogin_btn_Login_Click_Select_NotInModularAsync(strModularDetailIds);
        }
        
        public System.Data.DataSet frmLogin_btn_Login_Click_Select_Picture(int intStaff_id) {
            return base.Channel.frmLogin_btn_Login_Click_Select_Picture(intStaff_id);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> frmLogin_btn_Login_Click_Select_PictureAsync(int intStaff_id) {
            return base.Channel.frmLogin_btn_Login_Click_Select_PictureAsync(intStaff_id);
        }
        
        public System.Data.DataSet Main1_SelectAllModular(string strModularDetailIds) {
            return base.Channel.Main1_SelectAllModular(strModularDetailIds);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> Main1_SelectAllModularAsync(string strModularDetailIds) {
            return base.Channel.Main1_SelectAllModularAsync(strModularDetailIds);
        }
    }
}