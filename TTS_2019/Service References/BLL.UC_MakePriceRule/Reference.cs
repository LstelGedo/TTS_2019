﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace TTS_2019.BLL.UC_MakePriceRule {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BLL.UC_MakePriceRule.UC_MakePriceRule")]
    public interface UC_MakePriceRule {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectFareSectionResp" +
            "onse")]
        System.Data.DataSet US_MakePriceRule_Loaded_SelectFareSection();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectFareSectionResp" +
            "onse")]
        System.Threading.Tasks.Task<System.Data.DataSet> US_MakePriceRule_Loaded_SelectFareSectionAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketPriceRati" +
            "oResponse")]
        System.Data.DataSet US_MakePriceRule_Loaded_SelectTicketPriceRatio();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketPriceRati" +
            "oResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> US_MakePriceRule_Loaded_SelectTicketPriceRatioAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketTTPJPResp" +
            "onse")]
        System.Data.DataSet US_MakePriceRule_Loaded_SelectTicketTTPJP();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_SelectTicketTTPJPResp" +
            "onse")]
        System.Threading.Tasks.Task<System.Data.DataSet> US_MakePriceRule_Loaded_SelectTicketTTPJPAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertFareSectionResp" +
            "onse")]
        int US_MakePriceRule_Loaded_InsertFareSection(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertFareSectionResp" +
            "onse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_InsertFareSectionAsync(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketPriceRati" +
            "oResponse")]
        int US_MakePriceRule_Loaded_InsertTicketPriceRatio(string ticket_type, decimal ticket_price_ratio, string ratio);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketPriceRati" +
            "oResponse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_InsertTicketPriceRatioAsync(string ticket_type, decimal ticket_price_ratio, string ratio);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketTTPJPResp" +
            "onse")]
        int US_MakePriceRule_Loaded_InsertTicketTTPJP(string journey_paragraph, string paragraph_journey, string paragraph_number);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_InsertTicketTTPJPResp" +
            "onse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_InsertTicketTTPJPAsync(string journey_paragraph, string paragraph_journey, string paragraph_number);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateFareSectionResp" +
            "onse")]
        int US_MakePriceRule_Loaded_UpdateFareSection(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares, int section_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateFareSectionResp" +
            "onse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_UpdateFareSectionAsync(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares, int section_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketPriceRati" +
            "oResponse")]
        int US_MakePriceRule_Loaded_UpdateTicketPriceRatio(string ticket_type, decimal ticket_price_ratio, string ratio, int ticket_price_ratio_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketPriceRati" +
            "oResponse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_UpdateTicketPriceRatioAsync(string ticket_type, decimal ticket_price_ratio, string ratio, int ticket_price_ratio_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketTTPJPResp" +
            "onse")]
        int US_MakePriceRule_Loaded_UpdateTicketTTPJP(string journey_paragraph, string paragraph_journey, string paragraph_number, int TTPJP_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_UpdateTicketTTPJPResp" +
            "onse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_UpdateTicketTTPJPAsync(string journey_paragraph, string paragraph_journey, string paragraph_number, int TTPJP_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteFareSectionResp" +
            "onse")]
        int US_MakePriceRule_Loaded_DeleteFareSection(int section_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteFareSection", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteFareSectionResp" +
            "onse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_DeleteFareSectionAsync(int section_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketPriceRati" +
            "oResponse")]
        int US_MakePriceRule_Loaded_DeleteTicketPriceRatio(int ticket_price_ratio_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketPriceRati" +
            "o", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketPriceRati" +
            "oResponse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_DeleteTicketPriceRatioAsync(int ticket_price_ratio_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketTTPJPResp" +
            "onse")]
        int US_MakePriceRule_Loaded_DeleteTicketTTPJP(int TTPJP_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketTTPJP", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_MakePriceRule_Loaded_DeleteTicketTTPJPResp" +
            "onse")]
        System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_DeleteTicketTTPJPAsync(int TTPJP_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTrainTicket", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTrainTicketResponse")]
        System.Data.DataSet US_Ticket_Loaded_SelectTrainTicket();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTrainTicket", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTrainTicketResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> US_Ticket_Loaded_SelectTrainTicketAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectStation", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectStationResponse")]
        System.Data.DataSet US_Ticket_Loaded_SelectStation(int line_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectStation", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectStationResponse")]
        System.Threading.Tasks.Task<System.Data.DataSet> US_Ticket_Loaded_SelectStationAsync(int line_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTicketByCarType", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTicketByCarTypeRespons" +
            "e")]
        System.Data.DataSet US_Ticket_Loaded_SelectTicketByCarType(int train_id);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTicketByCarType", ReplyAction="http://tempuri.org/UC_MakePriceRule/US_Ticket_Loaded_SelectTicketByCarTypeRespons" +
            "e")]
        System.Threading.Tasks.Task<System.Data.DataSet> US_Ticket_Loaded_SelectTicketByCarTypeAsync(int train_id);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface UC_MakePriceRuleChannel : TTS_2019.BLL.UC_MakePriceRule.UC_MakePriceRule, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UC_MakePriceRuleClient : System.ServiceModel.ClientBase<TTS_2019.BLL.UC_MakePriceRule.UC_MakePriceRule>, TTS_2019.BLL.UC_MakePriceRule.UC_MakePriceRule {
        
        public UC_MakePriceRuleClient() {
        }
        
        public UC_MakePriceRuleClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UC_MakePriceRuleClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UC_MakePriceRuleClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UC_MakePriceRuleClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataSet US_MakePriceRule_Loaded_SelectFareSection() {
            return base.Channel.US_MakePriceRule_Loaded_SelectFareSection();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> US_MakePriceRule_Loaded_SelectFareSectionAsync() {
            return base.Channel.US_MakePriceRule_Loaded_SelectFareSectionAsync();
        }
        
        public System.Data.DataSet US_MakePriceRule_Loaded_SelectTicketPriceRatio() {
            return base.Channel.US_MakePriceRule_Loaded_SelectTicketPriceRatio();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> US_MakePriceRule_Loaded_SelectTicketPriceRatioAsync() {
            return base.Channel.US_MakePriceRule_Loaded_SelectTicketPriceRatioAsync();
        }
        
        public System.Data.DataSet US_MakePriceRule_Loaded_SelectTicketTTPJP() {
            return base.Channel.US_MakePriceRule_Loaded_SelectTicketTTPJP();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> US_MakePriceRule_Loaded_SelectTicketTTPJPAsync() {
            return base.Channel.US_MakePriceRule_Loaded_SelectTicketTTPJPAsync();
        }
        
        public int US_MakePriceRule_Loaded_InsertFareSection(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares) {
            return base.Channel.US_MakePriceRule_Loaded_InsertFareSection(extents, decline_rate, fares_rate, section_fares);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_InsertFareSectionAsync(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares) {
            return base.Channel.US_MakePriceRule_Loaded_InsertFareSectionAsync(extents, decline_rate, fares_rate, section_fares);
        }
        
        public int US_MakePriceRule_Loaded_InsertTicketPriceRatio(string ticket_type, decimal ticket_price_ratio, string ratio) {
            return base.Channel.US_MakePriceRule_Loaded_InsertTicketPriceRatio(ticket_type, ticket_price_ratio, ratio);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_InsertTicketPriceRatioAsync(string ticket_type, decimal ticket_price_ratio, string ratio) {
            return base.Channel.US_MakePriceRule_Loaded_InsertTicketPriceRatioAsync(ticket_type, ticket_price_ratio, ratio);
        }
        
        public int US_MakePriceRule_Loaded_InsertTicketTTPJP(string journey_paragraph, string paragraph_journey, string paragraph_number) {
            return base.Channel.US_MakePriceRule_Loaded_InsertTicketTTPJP(journey_paragraph, paragraph_journey, paragraph_number);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_InsertTicketTTPJPAsync(string journey_paragraph, string paragraph_journey, string paragraph_number) {
            return base.Channel.US_MakePriceRule_Loaded_InsertTicketTTPJPAsync(journey_paragraph, paragraph_journey, paragraph_number);
        }
        
        public int US_MakePriceRule_Loaded_UpdateFareSection(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares, int section_id) {
            return base.Channel.US_MakePriceRule_Loaded_UpdateFareSection(extents, decline_rate, fares_rate, section_fares, section_id);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_UpdateFareSectionAsync(string extents, decimal decline_rate, decimal fares_rate, decimal section_fares, int section_id) {
            return base.Channel.US_MakePriceRule_Loaded_UpdateFareSectionAsync(extents, decline_rate, fares_rate, section_fares, section_id);
        }
        
        public int US_MakePriceRule_Loaded_UpdateTicketPriceRatio(string ticket_type, decimal ticket_price_ratio, string ratio, int ticket_price_ratio_id) {
            return base.Channel.US_MakePriceRule_Loaded_UpdateTicketPriceRatio(ticket_type, ticket_price_ratio, ratio, ticket_price_ratio_id);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_UpdateTicketPriceRatioAsync(string ticket_type, decimal ticket_price_ratio, string ratio, int ticket_price_ratio_id) {
            return base.Channel.US_MakePriceRule_Loaded_UpdateTicketPriceRatioAsync(ticket_type, ticket_price_ratio, ratio, ticket_price_ratio_id);
        }
        
        public int US_MakePriceRule_Loaded_UpdateTicketTTPJP(string journey_paragraph, string paragraph_journey, string paragraph_number, int TTPJP_id) {
            return base.Channel.US_MakePriceRule_Loaded_UpdateTicketTTPJP(journey_paragraph, paragraph_journey, paragraph_number, TTPJP_id);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_UpdateTicketTTPJPAsync(string journey_paragraph, string paragraph_journey, string paragraph_number, int TTPJP_id) {
            return base.Channel.US_MakePriceRule_Loaded_UpdateTicketTTPJPAsync(journey_paragraph, paragraph_journey, paragraph_number, TTPJP_id);
        }
        
        public int US_MakePriceRule_Loaded_DeleteFareSection(int section_id) {
            return base.Channel.US_MakePriceRule_Loaded_DeleteFareSection(section_id);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_DeleteFareSectionAsync(int section_id) {
            return base.Channel.US_MakePriceRule_Loaded_DeleteFareSectionAsync(section_id);
        }
        
        public int US_MakePriceRule_Loaded_DeleteTicketPriceRatio(int ticket_price_ratio_id) {
            return base.Channel.US_MakePriceRule_Loaded_DeleteTicketPriceRatio(ticket_price_ratio_id);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_DeleteTicketPriceRatioAsync(int ticket_price_ratio_id) {
            return base.Channel.US_MakePriceRule_Loaded_DeleteTicketPriceRatioAsync(ticket_price_ratio_id);
        }
        
        public int US_MakePriceRule_Loaded_DeleteTicketTTPJP(int TTPJP_id) {
            return base.Channel.US_MakePriceRule_Loaded_DeleteTicketTTPJP(TTPJP_id);
        }
        
        public System.Threading.Tasks.Task<int> US_MakePriceRule_Loaded_DeleteTicketTTPJPAsync(int TTPJP_id) {
            return base.Channel.US_MakePriceRule_Loaded_DeleteTicketTTPJPAsync(TTPJP_id);
        }
        
        public System.Data.DataSet US_Ticket_Loaded_SelectTrainTicket() {
            return base.Channel.US_Ticket_Loaded_SelectTrainTicket();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> US_Ticket_Loaded_SelectTrainTicketAsync() {
            return base.Channel.US_Ticket_Loaded_SelectTrainTicketAsync();
        }
        
        public System.Data.DataSet US_Ticket_Loaded_SelectStation(int line_id) {
            return base.Channel.US_Ticket_Loaded_SelectStation(line_id);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> US_Ticket_Loaded_SelectStationAsync(int line_id) {
            return base.Channel.US_Ticket_Loaded_SelectStationAsync(line_id);
        }
        
        public System.Data.DataSet US_Ticket_Loaded_SelectTicketByCarType(int train_id) {
            return base.Channel.US_Ticket_Loaded_SelectTicketByCarType(train_id);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataSet> US_Ticket_Loaded_SelectTicketByCarTypeAsync(int train_id) {
            return base.Channel.US_Ticket_Loaded_SelectTicketByCarTypeAsync(train_id);
        }
    }
}