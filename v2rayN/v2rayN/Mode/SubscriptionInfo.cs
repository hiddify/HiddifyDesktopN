using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace v2rayN.Mode
{
    public class SubscriptionInfo
    {
        public long Upload { get; set; }
        public long Download { get; set; }
        public long Total { get; set; }
        public long ExpireDate { get; set; }
        public string? ProfileWebPageUrl { get; set; }
    }
}
