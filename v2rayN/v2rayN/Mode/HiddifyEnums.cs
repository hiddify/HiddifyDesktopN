using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using v2rayN.Resx;
using System.Resources;
using v2rayN.Mode;

namespace v2rayN.Mode
{
    internal class HiddifyEnums
    {
    }
    public enum RoutingEnum
    {
        All,
        Blocked,
        BypassNotblocked
    }
    public enum ProxyModeEnum : int
    {
        [LocalizedDescription("HomeProxyAuto", typeof(ResUI))]
        Smart,
        [LocalizedDescription("HomeProxyLoadBalance", typeof(ResUI))]
        Loadbalance,
        [LocalizedDescription("HomeProxyManual", typeof(ResUI))]
        Manual
    

    }
}

    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private readonly string _resourceName;
        private readonly Type _resourceType;

        public LocalizedDescriptionAttribute(string resourceName, Type resourceType)
        {
            _resourceName = resourceName;
            _resourceType = resourceType;
        }

        public override string Description
        {
            get
            {
                ResourceManager rm = new ResourceManager(_resourceType);
                return rm.GetString(_resourceName);
            }
        }

    
}


public static class ProxyModeEnumExtensions
{
    public static string ToLocalizedDescriptionString(this ProxyModeEnum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = fieldInfo.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false) as LocalizedDescriptionAttribute[];
        return attributes?.Length > 0 ? attributes[0].Description : value.ToString();
    }

}