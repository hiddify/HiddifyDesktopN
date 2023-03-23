using System.Windows;
using System.Windows.Threading;
using v2rayN.Handler;
using v2rayN.Mode;
using v2rayN.Tool;
using v2rayN.ViewModels;
using v2rayN.Tool;
using ByteSizeLib;
using System.Net;
using HiddifyN.Tool;

namespace v2rayN
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static EventWaitHandle ProgramStarted;
        public static bool IsNewInstance = false;
        private static Config _config;
        

        public App()
        {
            // Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        /// <summary>
        /// 只打开一个进程
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {

            DeepLinking.RegisterSchemes();
            
            Global.ExePathKey = Utils.GetMD5(Utils.GetExePath());

            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, Global.ExePathKey, out bool bCreatedNew);
            if (!bCreatedNew)
            {
                ProgramStarted.Set();
                IsNewInstance = true;
            }

            Global.processJob = new Job();

            Logging.Setup();
            Utils.SaveLog($"v2rayN start up | {Utils.GetVersion()} | {Utils.GetExePath()}");
            Logging.ClearLogs();

            Init();
            //var t = new TestSpeed();
            //var res = t.DownloadSpeed(false);
            //var p = (WebProxy)Utils.GetAppProxyAddress();

            Thread.CurrentThread.CurrentUICulture = new(_config.uiItem.currentLanguage);

            base.OnStartup(e);
        }


        private void Init()
        {
            if (ConfigHandler.LoadConfig(ref _config) != 0)
            {
                UI.ShowWarning($"Loading GUI configuration file is abnormal,please restart the application{Environment.NewLine}加载GUI配置文件异常,请重启应用");
                Current.Shutdown();
                Environment.Exit(0);
                return;
            }
        }
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Utils.SaveLog("App_DispatcherUnhandledException", e.Exception);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                Utils.SaveLog("CurrentDomain_UnhandledException", (Exception)e.ExceptionObject!);
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Utils.SaveLog("TaskScheduler_UnobservedTaskException", e.Exception);
        }
    }
}
