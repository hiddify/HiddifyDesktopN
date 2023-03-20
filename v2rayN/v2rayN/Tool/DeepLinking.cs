using Microsoft.Win32;
using v2rayN.Handler;

namespace v2rayN.Tool
{
    public class DeepLinking
    {
        const string FriendlyName = "HiddifyDesktopN URI Scheme";
        const string SchemeSeperator = "://";
        readonly static string applicationLocation = Utils.GetExePath();
        public static void RegisterSchemes()
        {

            foreach (var scheme in System.Enum.GetNames(typeof(Scheme)))
            {
                // If scheme registered we just skip it
                if (CheckSchemeRegistered(scheme))
                {
                    continue;
                }

                using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + scheme))
                {
                    // Replace typeof(App) by the class that contains the Main method or any class located in the project that produces the exe.
                    // or replace typeof(App).Assembly.Location by anything that gives the full path to the exe

                    key.SetValue("", "URL:" + FriendlyName);
                    key.SetValue("URL Protocol", "");

                    using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                    {
                        defaultIcon.SetValue("", applicationLocation + ",1");
                    }

                    using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                    {
                        commandKey.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                    }
                }

            }
        }

        public static bool CheckSchemeRegistered(string scheme)
        {
            using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + scheme))
            {
                if (key?.GetValue("")?.ToString() == "URL:" + FriendlyName)
                {
                    using (var defaultIcon = key.OpenSubKey("DefaultIcon"))
                    {
                        if (defaultIcon?.GetValue("").ToString() == applicationLocation + ",1")
                        {
                            using (var commandKey = key.OpenSubKey(@"shell\open\command"))
                            {
                                if (commandKey?.GetValue("").ToString() == "\"" + applicationLocation + "\" \"%1\"")
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static (bool, string?) IsUriForProgram(string uri)
        {
            uri = uri.ToLower();

            foreach (var scheme in System.Enum.GetNames(typeof(Scheme)))
            {
                if (uri.StartsWith(scheme + SchemeSeperator))
                {
                    return (true, scheme);
                }

            }
            return (false, null);
        }
        private static (Protocol?, string?) ParseProtocol(string uri)
        {
            Protocol protocol = new Protocol();
            string err = null;

            // Parse Vless Uri
            if (uri.StartsWith("vless" + SchemeSeperator))
            {
                protocol.Uri = uri;
                protocol.Scheme = Scheme.vless;
            }
            // Parse Vmess Uri
            else if (uri.StartsWith("vmess" + SchemeSeperator))
            {
                protocol.Uri = uri;
                protocol.Scheme = Scheme.vmees;
            }
            // Parse Shadowsocks(ss) Uri
            else if (uri.StartsWith("ss" + SchemeSeperator))
            {
                protocol.Uri = uri;
                protocol.Scheme = Scheme.ss;
            }
            // Parse Trojan Uri
            else if (uri.StartsWith("trojan" + SchemeSeperator))
            {
                protocol.Uri = uri;
                protocol.Scheme = Scheme.trojan;
            }
            // Handle Unexcepted Uri
            else
            {
                return (res: null, err: $"The {uri} is not supported in deep linking");
            }

            return (protocol, err);
        }

        private static (Subscription?, string?) ParseSubscription(string uri)
        {
            // Valid uri sample: hiddify://install-sub?url=domain.com/path/clash.yml&name=sub_name
            if (!uri.Contains("url="))
            {
                return (null, "Invalid uri");
            }

            // Extract url
            //var url Utils.ExtractUrlParameterFromUri(uri);
           
            #region Download sub link data
            //// Download url data
            //string data = "";
            //string err = "";
            //var task = Task.Run(async () =>
            //{
            //    try
            //    {
            //        var downloader = new DownloadHandle();

            //        // Download through proxy
            //        string url_res = await downloader.TryDownloadString(subscription.Url, false, "");
            //        if (Utils.IsNullOrEmpty(url_res.Trim()))
            //        {
            //            // Download without proxy
            //            url_res = await downloader.TryDownloadString(subscription.Url, false, "");
            //        }
            //        data = url_res;
            //    }
            //    catch (Exception e)
            //    {
            //        err = e.Message;
            //    }

            //});
            //task.Wait();

            //if (err != "")
            //{
            //    return (null, err);
            //}
            //if (Utils.IsNullOrEmpty(data.Trim()))
            //{
            //    return (null, "Error occurred getting url data");
            //}
            //subscription.Data = data;
            #endregion

            Subscription subscription = new Subscription();
            subscription.Url = Utils.ChangeHiddifySubDeeplinkToNormalSubUri(uri);
            return (subscription, "");
        }
        private static bool IsUriSub(string uri)
        {
            if (uri.StartsWith("hiddify://install-sub"))
            {
                return true;
            }
            return false;
        }
        public static (ParseResult? res, string? err) ParseUri(string uri)
        {

            ParseResult result = new ParseResult();
            string err = null;
            // If the uri is a subscription link we should download it and then add it
            // If the uri is just a protocol link, we have nothing much to do
            if (IsUriSub(uri))
            {
                (result.subscription,err) = ParseSubscription(uri);
            }
            else
            {
                (result.protocol,err) = ParseProtocol(uri);

            }
            if (err != null && err != "")
            {
                return (null, err);
            }
            // Return result (error field is null)
            return (result, null);

        }
    }
    public class ParseResult
    {
        public Protocol? protocol { get; set; }

        public Subscription? subscription { get; set; }
    }

    public class Protocol
    {
        // It will be extracted when it's getting add
        //public string? Name { get; set; }
        public string Uri { get; set; }
        // It's useless for now
        public Scheme Scheme { get; set; }
    }


    public class Subscription
    {
        // Name of the sub will be extracted when it's getting add
        //public string Name { get; set; }
        public string Url { get; set; }
        // We don't download sub link data, we just imoprt it and then update the sub
        //public string Data { get; set; }
    }


    public enum Scheme
    {
        // Clash will be regarded as a subscription
        //clash
        vless,
        vmees,
        ss,
        trojan,
        // The program main scheme
        hiddify,
    }
}