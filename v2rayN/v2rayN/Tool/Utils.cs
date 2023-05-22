using Downloader;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Printing.IndexedProperties;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using v2rayN.Base;
using v2rayN.Handler;
using v2rayN.Mode;
using v2rayN.Tool;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

using System.Net.Mail;
using System.Windows.Data;
using System.Globalization;

namespace v2rayN
{
    internal class Utils
    {
        #region 资源Json操作

        /// <summary>
        /// 获取嵌入文本资源
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public static string GetEmbedText(string res)
        {
            string result = string.Empty;

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using Stream? stream = assembly.GetManifestResourceStream(res);
                ArgumentNullException.ThrowIfNull(stream);
                using StreamReader reader = new(stream);
                result = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 取得存储资源
        /// </summary>
        /// <returns></returns>
        public static string? LoadResource(string res)
        {
            try
            {
                if (!File.Exists(res)) return null;
                return File.ReadAllText(res);
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return null;
        }

        /// <summary>
        /// 反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T? FromJson<T>(string? strJson)
        {
            try
            {
                if (string.IsNullOrEmpty(strJson))
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(strJson);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 序列化成Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(object? obj, bool indented = true)
        {
            string result = string.Empty;
            try
            {
                if (obj == null)
                {
                    return result;
                }
                if (indented)
                {
                    result = JsonConvert.SerializeObject(obj,
                                           Formatting.Indented,
                                           new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
                else
                {
                    result = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return result;
        }

        /// <summary>
        /// 保存成json文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static int ToJsonFile(object? obj, string filePath, bool nullValue = true)
        {
            int result;
            try
            {
                using StreamWriter file = File.CreateText(filePath);
                JsonSerializer serializer;
                if (nullValue)
                {
                    serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                }
                else
                {
                    serializer = new JsonSerializer() { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };
                }

                serializer.Serialize(file, obj);
                result = 0;
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
                result = -1;
            }
            return result;
        }

        public static JObject? ParseJson(string strJson)
        {
            try
            {
                return JObject.Parse(strJson);
            }
            catch (Exception ex)
            {
                //SaveLog(ex.Message, ex);
                return null;
            }
        }

        #endregion 资源Json操作

        #region 转换函数

        /// <summary>
        /// List<string>转逗号分隔的字符串
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static string List2String(List<string> lst, bool wrap = false)
        {
            try
            {
                if (lst == null)
                {
                    return string.Empty;
                }
                if (wrap)
                {
                    return string.Join("," + Environment.NewLine, lst);
                }
                else
                {
                    return string.Join(",", lst);
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 逗号分隔的字符串,转List<string>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> String2List(string str)
        {
            try
            {
                str = str.Replace(Environment.NewLine, "");
                return new List<string>(str.Split(',', StringSplitOptions.RemoveEmptyEntries));
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// 逗号分隔的字符串,先排序后转List<string>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> String2ListSorted(string str)
        {
            try
            {
                str = str.Replace(Environment.NewLine, "");
                List<string> list = new(str.Split(',', StringSplitOptions.RemoveEmptyEntries));
                list.Sort();
                return list;
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                SaveLog("Base64Encode", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Decode(string plainText)
        {
            try
            {
                plainText = plainText.TrimEx()
                  .Replace(Environment.NewLine, "")
                  .Replace("\n", "")
                  .Replace("\r", "")
                  .Replace('_', '/')
                  .Replace('-', '+')
                  .Replace(" ", "");

                if (plainText.Length % 4 > 0)
                {
                    plainText = plainText.PadRight(plainText.Length + 4 - plainText.Length % 4, '=');
                }

                byte[] data = Convert.FromBase64String(plainText);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
                SaveLog("Base64Decode", ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 转Int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int ToInt(object obj)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                //SaveLog(ex.Message, ex);
                return 0;
            }
        }

        public static bool ToBool(object obj)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch (Exception ex)
            {
                //SaveLog(ex.Message, ex);
                return false;
            }
        }

        public static string ToString(object obj)
        {
            try
            {
                return obj?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                //SaveLog(ex.Message, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// byte 转成 有两位小数点的 方便阅读的数据
        ///     比如 2.50 MB
        /// </summary>
        /// <param name="amount">bytes</param>
        /// <param name="result">转换之后的数据</param>
        /// <param name="unit">单位</param>
        public static void ToHumanReadable(long amount, out double result, out string unit)
        {
            uint factor = 1024u;
            //long KBs = amount / factor;
            long KBs = amount;
            if (KBs > 0)
            {
                // multi KB
                long MBs = KBs / factor;
                if (MBs > 0)
                {
                    // multi MB
                    long GBs = MBs / factor;
                    if (GBs > 0)
                    {
                        // multi GB
                        long TBs = GBs / factor;
                        if (TBs > 0)
                        {
                            result = TBs + ((GBs % factor) / (factor + 0.0));
                            unit = "TB";
                            return;
                        }
                        result = GBs + ((MBs % factor) / (factor + 0.0));
                        unit = "GB";
                        return;
                    }
                    result = MBs + ((KBs % factor) / (factor + 0.0));
                    unit = "MB";
                    return;
                }
                result = KBs + ((amount % factor) / (factor + 0.0));
                unit = "KB";
                return;
            }
            else
            {
                result = amount;
                unit = "B";
            }
        }

        public static string HumanFy(long amount)
        {
            ToHumanReadable(amount, out double result, out string unit);
            return $"{string.Format("{0:f1}", result)} {unit}";
        }

        public static string UrlEncode(string url)
        {
            return Uri.EscapeDataString(url);
            //return  HttpUtility.UrlEncode(url);
        }

        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        public static string GetMD5(string str)
        {
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            byte[] byteNew = MD5.HashData(byteOld);
            StringBuilder sb = new(32);
            foreach (byte b in byteNew)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static ImageSource IconToImageSource(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height),
                BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// idn to idc
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetPunycode(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }
            try
            {
                Uri uri = new(url);
                if (uri.Host == uri.IdnHost)
                {
                    return url;
                }
                else
                {
                    return url.Replace(uri.Host, uri.IdnHost);
                }
            }
            catch
            {
                return url;
            }
        }

        public static bool IsBase64String(string plainText)
        {
            var buffer = new Span<byte>(new byte[plainText.Length]);
            return Convert.TryFromBase64String(plainText, buffer, out int _);
        }

        public static string Convert2Comma(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            return text.Replace("，", ",").Replace(Environment.NewLine, ",");
        }

        #endregion 转换函数

        #region 数据检查

        /// <summary>
        /// 判断输入的是否是数字
        /// </summary>
        /// <param name="oText"></param>
        /// <returns></returns>
        public static bool IsNumberic(string oText)
        {
            try
            {
                int var1 = ToInt(oText);
                return true;
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 文本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }
            if (text == "null")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证IP地址是否合法
        /// </summary>
        /// <param name="ip"></param>
        public static bool IsIP(string ip)
        {
            //如果为空
            if (IsNullOrEmpty(ip))
            {
                return false;
            }

            //清除要验证字符串中的空格
            //ip = ip.TrimEx();
            //可能是CIDR
            if (ip.IndexOf(@"/") > 0)
            {
                string[] cidr = ip.Split('/');
                if (cidr.Length == 2)
                {
                    if (!IsNumberic(cidr[0]))
                    {
                        return false;
                    }
                    ip = cidr[0];
                }
            }

            //模式字符串
            string pattern = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

            //验证
            return IsMatch(ip, pattern);
        }

        /// <summary>
        /// 验证Domain地址是否合法
        /// </summary>
        /// <param name="domain"></param>
        public static bool IsDomain(string domain)
        {
            //如果为空
            if (IsNullOrEmpty(domain))
            {
                return false;
            }

            return Uri.CheckHostName(domain) == UriHostNameType.Dns;
        }

        /// <summary>
        /// 验证输入字符串是否与模式字符串匹配，匹配返回true
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="pattern">模式字符串</param>
        public static bool IsMatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsIpv6(string ip)
        {
            if (IPAddress.TryParse(ip, out IPAddress? address))
            {
                return address.AddressFamily switch
                {
                    AddressFamily.InterNetwork => false,
                    AddressFamily.InterNetworkV6 => true,
                    _ => false,
                };
            }
            return false;
        }

        #endregion 数据检查

        #region 开机自动启动

        /// <summary>
        /// 开机自动启动
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public static void SetAutoRun(bool run)
        {
            try
            {
                var autoRunName = $"{Global.AutoRunName}_{GetMD5(StartupPath())}";

                //delete first
                RegWriteValue(Global.AutoRunRegPath, autoRunName, "");
                if (IsAdministrator())
                {
                    AutoStart(autoRunName, "", "");
                }

                if (run)
                {
                    string exePath = $"\"{GetExePath()}\"";
                    if (IsAdministrator())
                    {
                        AutoStart(autoRunName, exePath, "");
                    }
                    else
                    {
                        RegWriteValue(Global.AutoRunRegPath, autoRunName, exePath);
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
        }

        /// <summary>
        /// 是否已经设置开机自动启动
        /// </summary>
        /// <returns></returns>
        public static bool IsAutoRun()
        {
            try
            {
                //clear
                if (!RegReadValue(Global.AutoRunRegPath, Global.AutoRunName, "").IsNullOrEmpty())
                {
                    RegWriteValue(Global.AutoRunRegPath, Global.AutoRunName, "");
                }

                string value = RegReadValue(Global.AutoRunRegPath, Global.AutoRunName, "");
                string exePath = GetExePath();
                if (value == exePath || value == $"\"{exePath}\"")
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return false;
        }

        /// <summary>
        /// 获取启动了应用程序的可执行文件的路径
        /// </summary>
        /// <returns></returns>
        public static string GetPath(string fileName)
        {
            string startupPath = StartupPath();
            if (IsNullOrEmpty(fileName))
            {
                return startupPath;
            }
            return Path.Combine(startupPath, fileName);
        }

        /// <summary>
        /// 获取启动了应用程序的可执行文件的路径及文件名
        /// </summary>
        /// <returns></returns>
        public static string GetExePath()
        {
            return Environment.ProcessPath;
        }

        public static string StartupPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string? RegReadValue(string path, string name, string def)
        {
            RegistryKey? regKey = null;
            try
            {
                regKey = Registry.CurrentUser.OpenSubKey(path, false);
                string? value = regKey?.GetValue(name) as string;
                if (IsNullOrEmpty(value))
                {
                    return def;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            finally
            {
                regKey?.Close();
            }
            return def;
        }

        public static void RegWriteValue(string path, string name, object value)
        {
            RegistryKey? regKey = null;
            try
            {
                regKey = Registry.CurrentUser.CreateSubKey(path);
                if (IsNullOrEmpty(value.ToString()))
                {
                    regKey?.DeleteValue(name, false);
                }
                else
                {
                    regKey?.SetValue(name, value);
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            finally
            {
                regKey?.Close();
            }
        }

        /// <summary>
        /// 判断.Net Framework的Release是否符合
        /// (.Net Framework 版本在4.0及以上)
        /// </summary>
        /// <param name="release">需要的版本4.6.2=394802;4.8=528040</param>
        /// <returns></returns>
        public static bool CheckForDotNetVersion(int release = 528040)
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using RegistryKey? ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey);
            if (ndpKey?.GetValue("Release") != null)
            {
                return (int)ndpKey.GetValue("Release") >= release;
            }
            return false;
        }

        /// <summary>
        /// Auto Start via TaskService
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="fileName"></param>
        /// <param name="description"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AutoStart(string taskName, string fileName, string description)
        {
            if (string.IsNullOrEmpty(taskName))
            {
                return;
            }
            string TaskName = taskName;
            var logonUser = WindowsIdentity.GetCurrent().Name;
            string taskDescription = description;
            string deamonFileName = fileName;

            using var taskService = new TaskService();
            var tasks = taskService.RootFolder.GetTasks(new Regex(TaskName));
            foreach (var t in tasks)
            {
                taskService.RootFolder.DeleteTask(t.Name);
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var task = taskService.NewTask();
            task.RegistrationInfo.Description = taskDescription;
            task.Settings.DisallowStartIfOnBatteries = false;
            task.Settings.StopIfGoingOnBatteries = false;
            task.Settings.RunOnlyIfIdle = false;
            task.Settings.IdleSettings.StopOnIdleEnd = false;
            task.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            task.Triggers.Add(new LogonTrigger { UserId = logonUser, Delay = TimeSpan.FromSeconds(10) });
            task.Principal.RunLevel = TaskRunLevel.Highest;
            task.Actions.Add(new ExecAction(deamonFileName));

            taskService.RootFolder.RegisterTaskDefinition(TaskName, task);
        }

        #endregion 开机自动启动

        #region 测速

        /// <summary>
        /// 取得本机 IP Address
        /// </summary>
        /// <returns></returns>
        //public static List<string> GetHostIPAddress()
        //{
        //    List<string> lstIPAddress = new List<string>();
        //    try
        //    {
        //        IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
        //        foreach (IPAddress ipa in IpEntry.AddressList)
        //        {
        //            if (ipa.AddressFamily == AddressFamily.InterNetwork)
        //                lstIPAddress.Add(ipa.ToString());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SaveLog(ex.Message, ex);
        //    }
        //    return lstIPAddress;
        //}

        public static void SetSecurityProtocol(bool enableSecurityProtocolTls13)
        {
            if (enableSecurityProtocolTls13)
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            }
            else
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
            ServicePointManager.DefaultConnectionLimit = 256;
        }

        public static bool PortInUse(int port)
        {
            bool inUse = false;
            try
            {
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

                var lstIpEndPoints = new List<IPEndPoint>(IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners());

                foreach (IPEndPoint endPoint in ipEndPoints)
                {
                    if (endPoint.Port == port)
                    {
                        inUse = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return inUse;
        }

        #endregion 测速

        #region 杂项

        /// <summary>
        /// 取得版本
        /// </summary>
        /// <returns></returns>
        public static string GetVersion(bool blFull = true)
        {
            try
            {
                string location = GetExePath();
                if (blFull)
                {
                    return string.Format("HiddifyN - V{0} - {1}-Test",
                            FileVersionInfo.GetVersionInfo(location).FileVersion.ToString(),
                            File.GetLastWriteTime(location).ToString("yyyy/MM/dd"));
                }
                else
                {
                    return string.Format("HiddifyN/{0}",
                        FileVersionInfo.GetVersionInfo(location).FileVersion.ToString());
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 深度拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // Serialize object to JSON
                System.Text.Json.JsonSerializer.Serialize(stream, obj);
                stream.Seek(0, SeekOrigin.Begin);

                // Deserialize JSON to new object
                return System.Text.Json.JsonSerializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// 获取剪贴板数
        /// </summary>
        /// <returns></returns>
        public static string? GetClipboardData()
        {
            string? strData = string.Empty;
            try
            {
                IDataObject data = Clipboard.GetDataObject();
                if (data.GetDataPresent(DataFormats.UnicodeText))
                {
                    strData = data.GetData(DataFormats.UnicodeText)?.ToString();
                }
                return strData;
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return strData;
        }

        /// <summary>
        /// 拷贝至剪贴板
        /// </summary>
        /// <returns></returns>
        public static void SetClipboardData(string strData)
        {
            try
            {
                Clipboard.SetText(strData);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 取得GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGUID(bool full = true)
        {
            try
            {
                if (full)
                {
                    return Guid.NewGuid().ToString("D");
                }
                else
                {
                    return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString();
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// IsAdministrator
        /// </summary>
        /// <returns></returns>
        public static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
                //WindowsBuiltInRole可以枚举出很多权限，例如系统用户、User、Guest等等
                return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
                return false;
            }
        }

        public static string GetDownloadFileName(string url)
        {
            var fileName = Path.GetFileName(url);
            fileName += "_temp";

            return fileName;
        }

        public static IPAddress? GetDefaultGateway()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up)
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                .Select(g => g?.Address)
                .Where(a => a != null)
                // .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                // .Where(a => Array.FindIndex(a.GetAddressBytes(), b => b != 0) >= 0)
                .FirstOrDefault();
        }

        public static bool IsGuidByParse(string strSrc)
        {
            return Guid.TryParse(strSrc, out Guid g);
        }

        public static void ProcessStart(string fileName)
        {
            try
            {
                Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
        }

        public static void SetDarkBorder(System.Windows.Window window, bool dark)
        {
            // Make sure the handle is created before the window is shown
            IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(window).EnsureHandle();
            int attribute = dark ? 1 : 0;
            uint attributeSize = (uint)Marshal.SizeOf(attribute);
            DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref attribute, attributeSize);
            DwmSetWindowAttribute(hWnd, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref attribute, attributeSize);
        }

        #endregion 杂项

        #region TempPath

        // return path to store temporary files
        public static string GetTempPath(string filename = "")
        {
            string _tempPath = Path.Combine(StartupPath(), "guiTemps");
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
            if (string.IsNullOrEmpty(filename))
            {
                return _tempPath;
            }
            else
            {
                return Path.Combine(_tempPath, filename);
            }
        }

        public static string UnGzip(byte[] buf)
        {
            using MemoryStream sb = new();
            using GZipStream input = new(new MemoryStream(buf), CompressionMode.Decompress, false);
            input.CopyTo(sb);
            sb.Position = 0;
            return new StreamReader(sb, Encoding.UTF8).ReadToEnd();
        }

        public static string GetBackupPath(string filename)
        {
            string _tempPath = Path.Combine(StartupPath(), "guiBackups");
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
            return Path.Combine(_tempPath, filename);
        }

        public static string GetConfigPath(string filename = "")
        {
            string _tempPath = Path.Combine(StartupPath(), "guiConfigs");
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
            if (string.IsNullOrEmpty(filename))
            {
                return _tempPath;
            }
            else
            {
                return Path.Combine(_tempPath, filename);
            }
        }

        public static string GetBinPath(string filename, ECoreType? coreType = null)
        {
            string _tempPath = Path.Combine(StartupPath(), "bin");
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
            if (coreType != null)
            {
                _tempPath = Path.Combine(_tempPath, coreType.ToString()!);
                if (!Directory.Exists(_tempPath))
                {
                    Directory.CreateDirectory(_tempPath);
                }
            }
            if (string.IsNullOrEmpty(filename))
            {
                return _tempPath;
            }
            else
            {
                return Path.Combine(_tempPath, filename);
            }
        }

        public static string GetLogPath(string filename = "")
        {
            string _tempPath = Path.Combine(StartupPath(), "guiLogs");
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
            if (string.IsNullOrEmpty(filename))
            {
                return _tempPath;
            }
            else
            {
                return Path.Combine(_tempPath, filename);
            }
        }

        public static string GetFontsPath(string filename = "")
        {
            string _tempPath = Path.Combine(StartupPath(), "guiFonts");
            if (!Directory.Exists(_tempPath))
            {
                Directory.CreateDirectory(_tempPath);
            }
            if (string.IsNullOrEmpty(filename))
            {
                return _tempPath;
            }
            else
            {
                return Path.Combine(_tempPath, filename);
            }
        }

        #endregion TempPath

        #region Log

        public static void SaveLog(string strContent)
        {
            if (LogManager.IsLoggingEnabled())
            {
                var logger = LogManager.GetLogger("Log1");
                logger.Info(strContent);
            }
        }

        public static void SaveLog(string strTitle, Exception ex)
        {
            if (LogManager.IsLoggingEnabled())
            {
                var logger = LogManager.GetLogger("Log2");
                logger.Debug($"{strTitle},{ex.Message}");
                logger.Debug(ex.StackTrace);
                if (ex?.InnerException != null)
                {
                    logger.Error(ex.InnerException);
                }
            }
        }

        #endregion Log

        #region scan screen

        public static string ScanScreen(float dpiX, float dpiY)
        {
            try
            {
                var left = (int)(SystemParameters.WorkArea.Left);
                var top = (int)(SystemParameters.WorkArea.Top);
                var width = (int)(SystemParameters.WorkArea.Width / dpiX);
                var height = (int)(SystemParameters.WorkArea.Height / dpiY);

                using Bitmap fullImage = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(fullImage))
                {
                    g.CopyFromScreen(left, top, 0, 0, fullImage.Size, CopyPixelOperation.SourceCopy);
                }
                int maxTry = 10;
                for (int i = 0; i < maxTry; i++)
                {
                    int marginLeft = (int)((double)fullImage.Width * i / 2.5 / maxTry);
                    int marginTop = (int)((double)fullImage.Height * i / 2.5 / maxTry);
                    Rectangle cropRect = new(marginLeft, marginTop, fullImage.Width - marginLeft * 2, fullImage.Height - marginTop * 2);
                    Bitmap target = new(width, height);

                    double imageScale = (double)width / (double)cropRect.Width;
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
                                        cropRect,
                                        GraphicsUnit.Pixel);
                    }

                    BitmapLuminanceSource source = new(target);
                    BinaryBitmap bitmap = new(new HybridBinarizer(source));
                    QRCodeReader reader = new();
                    Result result = reader.decode(bitmap);
                    if (result != null)
                    {
                        string ret = result.Text;
                        return ret;
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.Message, ex);
            }
            return string.Empty;
        }

        public static Tuple<float, float> GetDpiXY(Window window)
        {
            IntPtr hWnd = new WindowInteropHelper(window).EnsureHandle();
            Graphics g = Graphics.FromHwnd(hWnd);

            return new(96 / g.DpiX, 96 / g.DpiY);
        }

        #endregion scan screen

        #region Windows API

        [Flags]
        public enum DWMWINDOWATTRIBUTE : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19,
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref int attributeValue, uint attributeSize);

        #endregion Windows API

        // It doesn't really lock the file, because if the program be closed, the file will be unlocked
        // Instead of that, it will write locked in the file
        private static void LockMainFormReloadFile()
        {
            File.WriteAllText(Global.MainFormReloadFilePath, "locked");
        }
        private static void UnlockMainFormReloadFile()
        {
            File.WriteAllText(Global.MainFormReloadFilePath, "unlocked");
        }
        private static bool IsMainFormReloadFileLocked()
        {
           if (!File.Exists(Global.MainFormReloadFilePath))
           {
                return false;
           }
           string content = File.ReadAllText(Global.MainFormReloadFilePath);
           if (content == "locked")
           {
                return true;
           }
           else if (content == "unlocked")
           {
                return false;
           }
           else
           {
                throw new Exception($"the {Global.MainFormReloadFilePath} lock file, abnormaly changed, please delete it");
           }
        }

        public static void SetMainPageReload()
        {
            bool tryAgain = false;
            do
            {
                try
                {
                    LockMainFormReloadFile();
                }
                catch (Exception)
                {
                    tryAgain = true;
                }

            } while (tryAgain);
        }
        public static void UnsetMainPageReload()
        {
            bool tryAgain = false;
            do
            {
                try
                {
                    UnlockMainFormReloadFile();
                }
                catch (Exception)
                {
                    tryAgain = true;
                }
            }while(tryAgain);
        }
        public static bool DoesMainPageNeedReload()
        {
            return IsMainFormReloadFileLocked();
        }
        public static void ExitSuccess()
        {
            System.Windows.Application.Current.Shutdown();
            //int id = GetAppProcessID();
            //Process.GetProcessById(id).Kill(true);
            Environment.Exit(0);
        }
        public static void ExitError(int err)
        {
            System.Windows.Application.Current.Shutdown();
            //int id = GetAppProcessID();
            //Process.GetProcessById(id).Kill(true);
            Environment.Exit(err);
        }
        public static int GetAppProcessID()
        {
            return Process.GetCurrentProcess().Id;
        }
        public static IWebProxy GetAppProxyAddress()
        {
            var httpPort = LazyConfig.Instance.GetLocalPort(Global.InboundHttp);

            return new WebProxy(Global.Loopback, httpPort);
        }
        public static void RestartProgram(bool asAdmin = false)
        {
            new Thread(delegate ()
            {
                var mainProgramID = GetAppProcessID();
                var pInfo = new ProcessStartInfo();
                pInfo.FileName = Global.RestartProgramExePath;
                pInfo.WorkingDirectory = Environment.CurrentDirectory;
                if (asAdmin)
                {
                    pInfo.Arguments = $"{mainProgramID} --a";
                }
                else
                {
                    pInfo.Arguments = $"{mainProgramID}";
                }
                pInfo.CreateNoWindow = true;

                Process.Start(pInfo);

            }).Start();
        }
        // I think it's the right way
        public static bool IsSystemProxyEnabled(ESysProxyType sysProxyType)
        {
            if (sysProxyType == ESysProxyType.ForcedChange || sysProxyType == ESysProxyType.Pac)
            {
                return true;
            }
            return false;
        }

        public static string? ExtractNameParameterFromUri(string uri)
        {
            // Extract name
            if (uri.Contains("name="))
            {
                var splitted = uri.Split("name=");
                if (splitted.Length > 1)
                {
                    string name = "";
                    foreach (char c in splitted[1].ToCharArray())
                    {
                        if (c == '&' || c == '?')
                        {
                            break;
                        }
                        name += c;
                    }
                    return name;
                }
            }
            return null;
        }
        public static string? ExtractUrlParameterFromUri(string uri)
        {
            var splitted = uri.Split("url=");
            string? url = "";
            if (splitted.Length > 1)
            {
                foreach (char c in splitted[1].ToCharArray())
                {
                    if (c == '&' || c == '?')
                    {
                        break;
                    }
                    url += c;
                }
            }
            if (Utils.IsNullOrEmpty(url))
            {
                return null;
            }
            return url;
        }

        public static string ChangeHiddifySubDeeplinkToNormalSubUri(string hiddifySubDepplink)
        {
            // Valid uri sample: hiddify://install-sub?url=domain.com/path/clash.yml&name=sub_name
            // We need after "url=" part

            // Remove "hiddify://install-sub?"
            if (hiddifySubDepplink.Contains("url="))
            {
                return System.Web.HttpUtility.UrlDecode(hiddifySubDepplink.Split("url=")[1]);

            }
            return hiddifySubDepplink;
        }

        public static HttpResponseHeaders? GetUrlResponseHeader(string url, bool useProgramProxy)
        {
            HttpClientHandler handler;
            if (!useProgramProxy)
                handler = new HttpClientHandler() { UseProxy = false };
            else
                handler = new HttpClientHandler() { UseProxy = true };

            using (var client = new HttpClient(handler))
            {
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Clear();
                try
                {
                    var response = client.Send(new HttpRequestMessage(HttpMethod.Head,url));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response.Headers;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
        public async static Task<HttpStatusCode?> GetUrlResponseStatusCode(string url,bool useProgramProxy)
        {
            HttpClientHandler handler;
            if (!useProgramProxy)
                handler = new HttpClientHandler() { UseProxy = false};
            else
                handler = new HttpClientHandler() {  UseProxy =true};

            using (var client = new HttpClient(handler))
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Clear();
                try
                {
                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                    return response.StatusCode;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async static Task<bool> IsUrlStatusCode204(string url, bool useProgramProxy)
        {
            HttpClientHandler handler;
            if (!useProgramProxy)
                handler = new HttpClientHandler() { UseProxy = false };
            else
                handler = new HttpClientHandler() { UseProxy = true };

            using (var client = new HttpClient(handler))
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Clear();
                try
                {
                    var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        return true;
                    else
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static DateTime EpochToDate(long epoch)
        {
            return DateTimeOffset.FromUnixTimeSeconds(epoch).DateTime;
        }


        public static string? GetHostAndFirstTwoPathInUri(string url)
        {
            // For example:
            //   https://domain.com/first/two/all.txt?parameter=value
            // Will be:
            //   https://domain.com/first/two/

            Uri uri = new Uri(url);
            string[] splitted = uri.AbsolutePath.TrimStart('/').Split('/', 3);
            if (splitted.Length < 3)
            {
                return null;
            }
            return uri.Scheme + "://" + uri.Host + '/' + splitted[0] + '/' + splitted[1] + '/';

        }

        // First return value is for stdout and second one is for stderr
        public static (string?,string?) StartProcess(ProcessStartInfo sInfo)
        {
            var errors = new StringBuilder();
            var output = new StringBuilder();

            var hadError = false;

            var p = Process.Start(sInfo);
            p.EnableRaisingEvents = true;

            p.OutputDataReceived += (s, d) =>
            {
                output.Append(d.Data);
            };

            p.ErrorDataReceived += (s, d) =>
            {
                if (!hadError)
                {
                    hadError = !String.IsNullOrEmpty(d.Data);
                }
                errors.Append(d.Data);
            };

            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();

            string stdout = output.ToString();
            string stderr = errors.ToString();
            if (p.ExitCode != 0 || hadError)
            {
                return (null,stderr);
            }
            return (stdout,null);
        }
        #region Clash Subscription Info
        public static SubscriptionInfo? GetSubscriptionInfoFromHeaders(HttpResponseHeaders headers)
        {
            if (headers == null)
            {
                return null;
            }
            SubscriptionInfo clashSubscriptionInfo = new SubscriptionInfo();

            IEnumerable<string> userSubInfoValues;
            if (headers.TryGetValues("Subscription-Userinfo", out userSubInfoValues))
            {
                // Split values
                foreach (var item in userSubInfoValues.FirstOrDefault().Split(";"))
                {
                    // Split key and values
                    int equalsSignIndex = item.IndexOf("=");
                    string key = item.Substring(0, equalsSignIndex);
                    string value = item.Substring(equalsSignIndex + 1);

                    if (key == "upload")
                    {
                        long longValue;
                        if (long.TryParse(value, out longValue))
                        {
                            clashSubscriptionInfo.Upload = longValue;
                        }
                    }
                    else if (key == "download")
                    {
                        long longValue;
                        if (long.TryParse(value, out longValue))
                        {
                            clashSubscriptionInfo.Download = longValue;
                        }
                    }
                    else if (key == "total")
                    {
                        long longValue;
                        if (long.TryParse(value, out longValue))
                        {
                            clashSubscriptionInfo.Total = longValue;
                        }
                    }
                    else if (key == "expire")
                    {
                        long longValue;
                        if (long.TryParse(value, out longValue))
                        {
                            clashSubscriptionInfo.ExpireDate = longValue;
                        }

                    }
                }
                
            }


            // Get profile web page url from header
            IEnumerable<string> profileWebPageValue;
            if (headers.TryGetValues("Profile-Web-Page-Url",out profileWebPageValue))
            {
                string profileWebPageUrl = profileWebPageValue.FirstOrDefault();
                if (profileWebPageUrl != null)
                    clashSubscriptionInfo.ProfileWebPageUrl = profileWebPageUrl;
            }

            IEnumerable<string> profileTitleValue;
            if (headers.TryGetValues("Profile-Title", out profileTitleValue))
            {
                string profileTitle = profileTitleValue.FirstOrDefault();
                if (profileTitle != null)
                    clashSubscriptionInfo.ProfileTitle = profileTitle;
            }

            IEnumerable<string> profileUpdateIntervalValue;
            if (headers.TryGetValues("Profile-Update-Interval",out profileUpdateIntervalValue))
            {
                int profileUpdateInterval = Convert.ToInt32(profileUpdateIntervalValue.FirstOrDefault());
                if (profileUpdateIntervalValue != null)
                    clashSubscriptionInfo.ProfileUpdateInterval = profileUpdateInterval;
            }
            return clashSubscriptionInfo;
        }

        public int CalculateRemaningExpireDays(DateTime dateTime)
        {
            return DateTime.Now.Subtract(dateTime).Days;
        }
        #endregion
    }
    

}
