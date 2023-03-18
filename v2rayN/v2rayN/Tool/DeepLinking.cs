using Google.Protobuf.WellKnownTypes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using v2rayN.Mode;

namespace v2rayN.Tool
{
    public class DeepLinking
    {
        const string FriendlyName = "HiddifyDesktopN URI Scheme";
        const string SchemeSeperator = "://";
        public static void RegisterSchemes()
        {
            string applicationLocation = Utils.GetExePath();

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
                    return true;
                }
            }
            return false;
        }

        public static (bool,string?) IsUriForProgram(string uri)
        {
            uri = uri.ToLower();

            foreach (var scheme in System.Enum.GetNames(typeof(Scheme)))
            {
                if (uri.StartsWith(scheme + SchemeSeperator))
                {
                    return (true, scheme);
                }

            }
            return (false,null);
        }
        public static (ParseResult? res,string? err) ParseUri(string uri)
        {
            uri = uri.ToLower();

            ParseResult result = new ParseResult();


            // Parse Clash Uri
            if (uri.StartsWith("clash" + SchemeSeperator))
            {
                // Valid URI sample = clash://install-config?url=https://mysite.com/all.yml&name=profilename

                string correct_uri, name;

                // Remove additional things
                var prunedUri = uri.Split("url=");
                if (prunedUri.Length < 2)
                {
                    return (null,$"Invalid Uri: {uri}");
                }
                // Just keep url and profile name
                prunedUri = prunedUri[1].Split('&');
                // Check to see if profile name provided
                if (prunedUri.Length > 1)
                {
                    correct_uri = prunedUri[0];

                    // For extract profile name from uri
                    prunedUri = prunedUri[1].Split('=');
                    if (prunedUri.Length < 2)
                    {
                        return (null, $"Invalid Uri: {uri}");
                    }
                    name = prunedUri[1];

                    result.Name = name;
                    result.Url = correct_uri;
                }
                else
                {
                    result.Name = "Url Subscription";
                    result.Url = prunedUri[0];           
                }
                result.Scheme = Scheme.clash;
            }
            // Parse Vless Uri
            else if (uri.StartsWith("vless" + SchemeSeperator))
            {

                result.Scheme = Scheme.vless;
            }
            // Parse Vmess Uri
            else if (uri.StartsWith("vmess" + SchemeSeperator))
            {

                result.Scheme = Scheme.vmees;
            }
            // Parse Shadowsocks(ss) Uri
            else if (uri.StartsWith("ss" + SchemeSeperator))
            {
                string name, correct_uri;
                if (uri.Contains("#"))
                {
                    name = uri.Split("#", 1)[1];
                }

                result.Scheme = Scheme.ss;
            }
            // Parse Trojan Uri
            else if (uri.StartsWith("trojan" + SchemeSeperator))
            {

                result.Scheme = Scheme.trojan;
            }
            // Handle Unexcepted Uri
            else
            {
                return (res:null,err:$"The {uri} is not supported in deep linking");
            }

            
            // Return result (error field is null)
            return (result, null);

        }
    }
    public class ParseResult
    {
        public Scheme Scheme { get; set; }
        public string Url { get; set; }
        public string? Name { get; set; }
    }

    public enum Scheme
    {
        clash,
        vless,
        vmees,
        ss,
        trojan
    }
}