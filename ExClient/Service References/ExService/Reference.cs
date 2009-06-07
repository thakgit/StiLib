﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3074
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExClient.ExService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ExService.IExService")]
    public interface IExService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExService/Run", ReplyAction="http://tempuri.org/IExService/RunResponse")]
        string Run(string ex);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExService/RunScript", ReplyAction="http://tempuri.org/IExService/RunScriptResponse")]
        string RunScript(string ex, string script);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExService/GetEx", ReplyAction="http://tempuri.org/IExService/GetExResponse")]
        string[] GetEx();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IExServiceChannel : ExClient.ExService.IExService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class ExServiceClient : System.ServiceModel.ClientBase<ExClient.ExService.IExService>, ExClient.ExService.IExService {
        
        public ExServiceClient() {
        }
        
        public ExServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ExServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ExServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ExServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string Run(string ex) {
            return base.Channel.Run(ex);
        }
        
        public string RunScript(string ex, string script) {
            return base.Channel.RunScript(ex, script);
        }
        
        public string[] GetEx() {
            return base.Channel.GetEx();
        }
    }
}
