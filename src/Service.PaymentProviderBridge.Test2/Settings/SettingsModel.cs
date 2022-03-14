using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.PaymentProviderBridge.Test2.Settings
{
    public class SettingsModel
    {
        [YamlProperty("PaymentProviderBridge.Test2.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test2.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test2.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("PaymentProviderBridge.Test2.ServiceUrl")]
        public string ServiceUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test2.OkUrl")]
        public string OkUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test2.FailUrl")]
        public string FailUrl { get; set; }
    }
}
