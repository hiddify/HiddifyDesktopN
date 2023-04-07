using DynamicData;
using DynamicData.Binding;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Pkcs;
using System.Security.RightsManagement;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using v2rayN.Base;
using v2rayN.Handler;
using v2rayN.Mode;
using v2rayN.Resx;
using v2rayN.Tool;
using v2rayN.Views;
using static Grpc.Core.ChannelOption;
using Application = System.Windows.Application;


namespace v2rayN.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        #region private prop

        private CoreHandler _coreHandler;
        private StatisticsHandler _statistics;
        private List<ProfileItem> _lstProfile;
        private string _subId = string.Empty;
        private string _serverFilter = string.Empty;
        private static Config _config;
        private readonly PaletteHelper _paletteHelper = new();
        private Dictionary<string, bool> _dicHeaderSort = new();
        private Action<string> _updateView;
        private bool IsConnected = false;
        private string DefaultProxyMode = "Auto";
        private bool IsForSettingBackLanguage = false;
        private bool IsDelayCalculationFinished = false;

        #endregion
        // It's public because we need it in MainWindow.xaml.cs
        public NoticeHandler? _noticeHandler;

        #region ObservableCollection

        private IObservableCollection<ProfileItemModel> _profileItems = new ObservableCollectionExtended<ProfileItemModel>();
        public IObservableCollection<ProfileItemModel> ProfileItems => _profileItems;

        private IObservableCollection<SubItem> _subItems = new ObservableCollectionExtended<SubItem>();
        public IObservableCollection<SubItem> SubItems => _subItems;

        private IObservableCollection<RoutingItem> _routingItems = new ObservableCollectionExtended<RoutingItem>();
        public IObservableCollection<RoutingItem> RoutingItems => _routingItems;

        private IObservableCollection<ComboItem> _servers = new ObservableCollectionExtended<ComboItem>();
        public IObservableCollection<ComboItem> Servers => _servers;

        //home
        [Reactive]
        public ListBoxItem HomeSelectedRoutingItem { get; set; }

        [Reactive]
        public ListBoxItem HomeSelectedProxyMode { get; set; }
        [Reactive]
        public int SelectedProfileDelay { get; set; }
        [Reactive]
        public bool V2RayNPanelVisible { get; set; } = false;
        [Reactive]
        public string ConnectVPNLabel { get; set; } = "Not Connected";

        [Reactive]
        public string ConnectVPNLabelColor { get; set; } = "#FFFF0000";

        [Reactive]
        public double WindowWidth { get; set; }
        [Reactive]
        public double MaxWindowWidth { get; set; }

        [Reactive]
        public ProfileItemModel SelectedProfile { get; set; }
        public IList<ProfileItemModel> SelectedProfiles { get; set; }
        [Reactive]
        public SubItem SelectedSub { get; set; }
        [Reactive]
        public SubItem SelectedMoveToGroup { get; set; }
        [Reactive]
        public RoutingItem SelectedRouting { get; set; }
        [Reactive]
        public ComboItem SelectedServer { get; set; }
        [Reactive]
        public string ServerFilter { get; set; }
        [Reactive]
        public bool BlServers { get; set; }
        #endregion

        #region Menu
        //home
        public ReactiveCommand<Unit, Unit> HomeNewProfileCmd { get; }
        public ReactiveCommand<Unit, Unit> HomeConnectCmd { get; }
        public ReactiveCommand<Unit, Unit> HomeUpdateUsageCmd { get; }
        public ReactiveCommand<Unit, Unit> HomeGotoProfileCmd { get; }
        
        public ReactiveCommand<Unit, Unit> HomeRealPingServerCmd { get; }

        //servers
        public ReactiveCommand<Unit, Unit> AddVmessServerCmd { get; }
        public ReactiveCommand<Unit, Unit> AddVlessServerCmd { get; }
        public ReactiveCommand<Unit, Unit> AddShadowsocksServerCmd { get; }
        public ReactiveCommand<Unit, Unit> AddSocksServerCmd { get; }
        public ReactiveCommand<Unit, Unit> AddTrojanServerCmd { get; }
        public ReactiveCommand<Unit, Unit> AddCustomServerCmd { get; }
        public ReactiveCommand<Unit, Unit> AddServerViaClipboardCmd { get; }
        public ReactiveCommand<Unit, Unit> AddServerViaScanCmd { get; }
        //servers delete
        public ReactiveCommand<Unit, Unit> EditServerCmd { get; }
        public ReactiveCommand<Unit, Unit> RemoveServerCmd { get; }
        public ReactiveCommand<Unit, Unit> RemoveDuplicateServerCmd { get; }
        public ReactiveCommand<Unit, Unit> CopyServerCmd { get; }
        public ReactiveCommand<Unit, Unit> SetDefaultServerCmd { get; }
        public ReactiveCommand<Unit, Unit> ShareServerCmd { get; }
        //servers move   
        public ReactiveCommand<Unit, Unit> MoveTopCmd { get; }
        public ReactiveCommand<Unit, Unit> MoveUpCmd { get; }
        public ReactiveCommand<Unit, Unit> MoveDownCmd { get; }
        public ReactiveCommand<Unit, Unit> MoveBottomCmd { get; }

        //servers ping 
        public ReactiveCommand<Unit, Unit> MixedTestServerCmd { get; }
        public ReactiveCommand<Unit, Unit> PingServerCmd { get; }
        public ReactiveCommand<Unit, Unit> TcpingServerCmd { get; }
        public ReactiveCommand<Unit, Unit> RealPingServerCmd { get; }
        public ReactiveCommand<Unit, Unit> SpeedServerCmd { get; }
        public ReactiveCommand<Unit, Unit> SortServerResultCmd { get; }
        //servers export
        public ReactiveCommand<Unit, Unit> Export2ClientConfigCmd { get; }
        public ReactiveCommand<Unit, Unit> Export2ServerConfigCmd { get; }
        public ReactiveCommand<Unit, Unit> Export2ShareUrlCmd { get; }
        public ReactiveCommand<Unit, Unit> Export2SubContentCmd { get; }

        //Subscription
        public ReactiveCommand<Unit, Unit> SubSettingCmd { get; }
        public ReactiveCommand<Unit, Unit> AddSubCmd { get; }
        public ReactiveCommand<Unit, Unit> SubUpdateCmd { get; }
        public ReactiveCommand<Unit, Unit> SubUpdateViaProxyCmd { get; }
        public ReactiveCommand<Unit, Unit> SubGroupUpdateCmd { get; }
        public ReactiveCommand<Unit, Unit> SubGroupUpdateViaProxyCmd { get; }

        //Setting
        public ReactiveCommand<Unit, Unit> OptionSettingCmd { get; }
        public ReactiveCommand<Unit, Unit> RoutingSettingCmd { get; }
        public ReactiveCommand<Unit, Unit> GlobalHotkeySettingCmd { get; }
        public ReactiveCommand<Unit, Unit> ClearServerStatisticsCmd { get; }
        public ReactiveCommand<Unit, Unit> ImportOldGuiConfigCmd { get; }

        //CheckUpdate
        public ReactiveCommand<Unit, Unit> CheckUpdateNCmd { get; }
        public ReactiveCommand<Unit, Unit> CheckUpdateV2flyCoreCmd { get; }
        public ReactiveCommand<Unit, Unit> CheckUpdateSagerNetCoreCmd { get; }
        public ReactiveCommand<Unit, Unit> CheckUpdateXrayCoreCmd { get; }
        public ReactiveCommand<Unit, Unit> CheckUpdateClashCoreCmd { get; }
        public ReactiveCommand<Unit, Unit> CheckUpdateClashMetaCoreCmd { get; }
        public ReactiveCommand<Unit, Unit> CheckUpdateGeoCmd { get; }

        public ReactiveCommand<Unit, Unit> ToggleV2rayNPanelCmd { get; }

        public ReactiveCommand<Unit, Unit> ReloadCmd { get; }
        [Reactive]
        public bool BlReloadEnabled { get; set; }

        public ReactiveCommand<Unit, Unit> NotifyLeftClickCmd { get; }
        [Reactive]
        public Icon NotifyIcon { get; set; }
        [Reactive]
        public ImageSource AppIcon { get; set; }
        #endregion

        #region System Proxy 
        [Reactive]
        public bool BlSystemProxyClear { get; set; }
        [Reactive]
        public bool BlSystemProxySet { get; set; }
        [Reactive]
        public bool BlSystemProxyNothing { get; set; }
        [Reactive]
        public bool BlSystemProxyPac { get; set; }
        public ReactiveCommand<Unit, Unit> SystemProxyClearCmd { get; }
        public ReactiveCommand<Unit, Unit> SystemProxySetCmd { get; }
        public ReactiveCommand<Unit,Unit> SystemProxyToggleCmd { get; }
        public ReactiveCommand<Unit, Unit> SystemProxyNothingCmd { get; }
        public ReactiveCommand<Unit, Unit> SystemProxyPacCmd { get; }

        [Reactive]
        public bool BlRouting { get; set; }
        [Reactive]
        public int SystemProxySelected { get; set; }
        #endregion

        #region UI


        [Reactive]
        public bool ConnectProgress { get; set; }

        [Reactive]
        public bool ProfileExpanded { get; set; }
        
        [Reactive]
        public bool DelayProgress { get; set; } = false;
        [Reactive]
        public string ConnectColor { get; set; } = "#FFE0E0E0";

        [Reactive]
        public string InboundDisplay { get; set; }
        [Reactive]
        public string InboundLanDisplay { get; set; }
        [Reactive]
        public string RunningServerDisplay { get; set; }
        [Reactive]
        public string RunningServerToolTipText { get; set; }
        [Reactive]
        public string RunningInfoDisplay { get; set; }
        [Reactive]
        public string SpeedProxyDisplay { get; set; }
        [Reactive]
        public string SpeedDirectDisplay { get; set; }
        [Reactive]
        public bool EnableTun { get; set; }
        [Reactive]
        public bool SysProxyState { get; set; }

        [Reactive]
        public bool ColorModeDark { get; set; }
        private IObservableCollection<Swatch> _swatches = new ObservableCollectionExtended<Swatch>();
        public IObservableCollection<Swatch> Swatches => _swatches;
        [Reactive]
        public Swatch SelectedSwatch { get; set; }
        [Reactive]
        public int CurrentFontSize { get; set; }

        [Reactive]
        public string CurrentLanguage { get; set; }

        
        #endregion

        #region Init

        // Program will change some values when it start
        // We will ignore that change, so we just consider user changes
        private bool ForInitiationLanguage = true;
        private bool ForInitiationTun = true;

        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue, Action<string> updateView)
        {
            _updateView = updateView;
            ThreadPool.RegisterWaitForSingleObject(App.ProgramStarted, OnProgramStarted, null, -1, false);

            Locator.CurrentMutable.RegisterLazySingleton(() => new NoticeHandler(snackbarMessageQueue), typeof(NoticeHandler));
            _noticeHandler = Locator.Current.GetService<NoticeHandler>();
            _config = LazyConfig.Instance.GetConfig();
            //ThreadPool.RegisterWaitForSingleObject(App.ProgramStarted, OnProgramStarted, null, -1, false);
            Init();

            SelectedProfile = new();
            SelectedSub = new();
            SelectedMoveToGroup = new();
            SelectedRouting = new();
            SelectedServer = new();
            if (_config.tunModeItem.enableTun && Utils.IsAdministrator())
            {
                EnableTun = true;
            }
            _subId = _config.subIndexId;

            InitSubscriptionView();
            RefreshRoutingsMenu();
            RefreshServers();


            this.WhenAnyValue(x => x.HomeSelectedRoutingItem).Subscribe(c => HomeSelectedRouteChanged());

            this.WhenAnyValue(
                x => x.HomeSelectedProxyMode).Subscribe(c => HomeSelectedProxyChanged());
            var canEditRemove = this.WhenAnyValue(
               x => x.SelectedProfile,
               selectedSource => selectedSource != null && !selectedSource.indexId.IsNullOrEmpty());

            this.WhenAnyValue(
                x => x.SelectedSub,
                y => y != null && !y.remarks.IsNullOrEmpty() && _subId != y.id)
                    .Subscribe(c => SubSelectedChanged(c));
            this.WhenAnyValue(
                 x => x.SelectedMoveToGroup,
                 y => y != null && !y.remarks.IsNullOrEmpty())
                     .Subscribe(c => MoveToGroup(c));

            this.WhenAnyValue(
                x => x.SelectedRouting,
                y => y != null && !y.remarks.IsNullOrEmpty())
                    .Subscribe(c => RoutingSelectedChanged(c));

            this.WhenAnyValue(
              x => x.SelectedServer,
              y => y != null && !y.Text.IsNullOrEmpty())
                  .Subscribe(c => ServerSelectedChanged(c));

            this.WhenAnyValue(
              x => x.ServerFilter,
              y => y != null && _serverFilter != y)
                  .Subscribe(c => ServerFilterChanged(c));

            SystemProxySelected = (int)_config.sysProxyType;
            this.WhenAnyValue(
              x => x.SystemProxySelected,
              y => y >= 0)
                  .Subscribe(c => DoSystemProxySelected(c));

            this.WhenAnyValue(
              x => x.EnableTun,
               y => y == true)
                  .Subscribe(c => DoEnableTun(c));

            BindingUI();
            RestoreUI();
            //AutoHideStartup();

            //home
            HomeNewProfileCmd = ReactiveCommand.Create(() =>
            {
                HomeNewProfile();
            });
            HomeConnectCmd = ReactiveCommand.CreateFromTask(() =>
            {
               return HomeConnect();
            });
            HomeUpdateUsageCmd = ReactiveCommand.Create(() =>
            {
                HomeUpdateUsage(SelectedSub.id);
            });
            HomeGotoProfileCmd = ReactiveCommand.Create(() =>
            {
                HomeGotoProfile(SelectedSub.id);
            });
            HomeRealPingServerCmd = ReactiveCommand.CreateFromTask(() =>
            {
                return HomeRealPingServer(_config.indexId);
            });
            //servers
            AddVmessServerCmd = ReactiveCommand.Create(() =>
            {
                EditServer(true, EConfigType.VMess);
            });
            AddVlessServerCmd = ReactiveCommand.Create(() =>
            {
                EditServer(true, EConfigType.VLESS);
            });
            AddShadowsocksServerCmd = ReactiveCommand.Create(() =>
            {
                EditServer(true, EConfigType.Shadowsocks);
            });
            AddSocksServerCmd = ReactiveCommand.Create(() =>
            {
                EditServer(true, EConfigType.Socks);
            });
            AddTrojanServerCmd = ReactiveCommand.Create(() =>
            {
                EditServer(true, EConfigType.Trojan);
            });
            AddCustomServerCmd = ReactiveCommand.Create(() =>
            {
                EditServer(true, EConfigType.Custom);
            });
            AddServerViaClipboardCmd = ReactiveCommand.Create(() =>
            {
                AddServerOrSubViaClipboard();
            });
            AddServerViaScanCmd = ReactiveCommand.CreateFromTask(() =>
            {
                return ScanScreenTaskAsync();
            });
            //servers delete
            EditServerCmd = ReactiveCommand.Create(() =>
            {
                EditServer(false, EConfigType.Custom);
            }, canEditRemove);
            RemoveServerCmd = ReactiveCommand.Create(() =>
            {
                RemoveServer();
            }, canEditRemove);
            RemoveDuplicateServerCmd = ReactiveCommand.Create(() =>
            {
                RemoveDuplicateServer();
            });
            CopyServerCmd = ReactiveCommand.Create(() =>
            {
                CopyServer();
            }, canEditRemove);
            SetDefaultServerCmd = ReactiveCommand.Create(() =>
            {
                SetDefaultServer();
            }, canEditRemove);
            ShareServerCmd = ReactiveCommand.Create(() =>
            {
                ShareServer();
            }, canEditRemove);
            //servers move   
            MoveTopCmd = ReactiveCommand.Create(() =>
            {
                MoveServer(EMove.Top);
            }, canEditRemove);
            MoveUpCmd = ReactiveCommand.Create(() =>
            {
                MoveServer(EMove.Up);
            }, canEditRemove);
            MoveDownCmd = ReactiveCommand.Create(() =>
            {
                MoveServer(EMove.Down);
            }, canEditRemove);
            MoveBottomCmd = ReactiveCommand.Create(() =>
            {
                MoveServer(EMove.Bottom);
            }, canEditRemove);

            //servers ping
            MixedTestServerCmd = ReactiveCommand.Create(() =>
            {
                ServerSpeedtest(ESpeedActionType.Mixedtest);
            });
            PingServerCmd = ReactiveCommand.Create(() =>
            {
                ServerSpeedtest(ESpeedActionType.Ping);
            }, canEditRemove);
            TcpingServerCmd = ReactiveCommand.Create(() =>
            {
                ServerSpeedtest(ESpeedActionType.Tcping);
            }, canEditRemove);
            RealPingServerCmd = ReactiveCommand.Create(() =>
            {
                ServerSpeedtest(ESpeedActionType.Realping);
            }, canEditRemove);
            SpeedServerCmd = ReactiveCommand.Create(() =>
            {
                ServerSpeedtest(ESpeedActionType.Speedtest);
            }, canEditRemove);
            SortServerResultCmd = ReactiveCommand.Create(() =>
            {
                SortServer(EServerColName.delayVal.ToString());
            });
            //servers export
            Export2ClientConfigCmd = ReactiveCommand.Create(() =>
            {
                Export2ClientConfig();
            }, canEditRemove);
            Export2ServerConfigCmd = ReactiveCommand.Create(() =>
            {
                Export2ServerConfig();
            }, canEditRemove);
            Export2ShareUrlCmd = ReactiveCommand.Create(() =>
            {
                Export2ShareUrl();
            }, canEditRemove);
            Export2SubContentCmd = ReactiveCommand.Create(() =>
            {
                Export2SubContent();
            }, canEditRemove);


            //Subscription
            SubSettingCmd = ReactiveCommand.Create(() =>
            {
                SubSetting();
            });
            AddSubCmd = ReactiveCommand.Create(() =>
            {
                AddSub();
            });
            SubUpdateCmd = ReactiveCommand.Create(() =>
            {
                UpdateSubscriptionProcess("", false);
            });
            SubUpdateViaProxyCmd = ReactiveCommand.Create(() =>
            {
                UpdateSubscriptionProcess("", true);
            });
            SubGroupUpdateCmd = ReactiveCommand.Create(() =>
            {
                UpdateSubscriptionProcess(_subId, false);
            });
            SubGroupUpdateViaProxyCmd = ReactiveCommand.Create(() =>
            {
                UpdateSubscriptionProcess(_subId, true);
            });

            //Setting
            OptionSettingCmd = ReactiveCommand.Create(() =>
            {
                OptionSetting();
            });
            RoutingSettingCmd = ReactiveCommand.Create(() =>
            {
                RoutingSetting();
            });
            GlobalHotkeySettingCmd = ReactiveCommand.Create(() =>
            {
                if ((new GlobalHotkeySettingWindow()).ShowDialog() == true)
                {
                    _noticeHandler?.Enqueue(ResUI.OperationSuccess);
                }
            });
            ClearServerStatisticsCmd = ReactiveCommand.Create(() =>
            {
                _statistics?.ClearAllServerStatistics();
                RefreshServers();
            });
            ImportOldGuiConfigCmd = ReactiveCommand.Create(() =>
            {
                ImportOldGuiConfig();
            });

            //CheckUpdate
            CheckUpdateNCmd = ReactiveCommand.Create(() =>
            {
                CheckUpdateN();
            });
            CheckUpdateV2flyCoreCmd = ReactiveCommand.Create(() =>
            {
                CheckUpdateCore(ECoreType.v2fly_v5);
            });
            CheckUpdateSagerNetCoreCmd = ReactiveCommand.Create(() =>
            {
                CheckUpdateCore(ECoreType.SagerNet);
            });
            CheckUpdateXrayCoreCmd = ReactiveCommand.Create(() =>
            {
                CheckUpdateCore(ECoreType.Xray);
            });
            CheckUpdateClashCoreCmd = ReactiveCommand.Create(() =>
            {
                CheckUpdateCore(ECoreType.clash);
            });
            CheckUpdateClashMetaCoreCmd = ReactiveCommand.Create(() =>
            {
                CheckUpdateCore(ECoreType.clash_meta);
            });
            CheckUpdateGeoCmd = ReactiveCommand.Create(() =>
            {
                CheckUpdateGeo();
            });

            ReloadCmd = ReactiveCommand.Create(() =>
            {
                Reload();
            });

            NotifyLeftClickCmd = ReactiveCommand.Create(() =>
            {
                ShowHideWindow(null);
            });

            //System proxy
            SystemProxyClearCmd = ReactiveCommand.Create(() =>
            {
                UnsetSysProxy();
            });
            SystemProxySetCmd = ReactiveCommand.Create(() =>
            {
                SetSysProxy();
            });
            SystemProxyToggleCmd = ReactiveCommand.Create(() =>
            {
                ToggleSysProxy();
            });
            SystemProxyNothingCmd = ReactiveCommand.Create(() =>
            {
                SetListenerType(ESysProxyType.Unchanged);
            });
            SystemProxyPacCmd = ReactiveCommand.Create(() =>
            {
                SetListenerType(ESysProxyType.Pac);
            });

            ToggleV2rayNPanelCmd = ReactiveCommand.Create(() =>
            {
                ToggleV2rayPanel();
            });
            Global.ShowInTaskbar = true;
        }

        private void Init()
        {
            ConfigHandler.InitBuiltinRouting(ref _config);
            //MainFormHandler.Instance.BackupGuiNConfig(_config, true);
            _coreHandler = new CoreHandler(UpdateHandler);

            if (_config.guiItem.enableStatistics)
            {
                _statistics = new StatisticsHandler(_config, UpdateStatisticsHandler);
            }

            MainFormHandler.Instance.UpdateTask(_config, UpdateTaskHandler);
            MainFormHandler.Instance.RegisterGlobalHotkey(_config, OnHotkeyHandler, UpdateTaskHandler);

            Reload();
            ChangeSystemProxyStatus(_config.sysProxyType, true);
        }
        private void OnProgramStarted(object state, bool timeout)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                ShowHideWindow(true);
            }));
        }

        #endregion

        #region Actions
        private void UpdateHandler(bool notify, string msg)
        {
            _noticeHandler?.SendMessage(msg);
        }
        private void UpdateTaskHandler(bool success, string msg)
        {
            _noticeHandler?.SendMessage(msg);
            if (success)
            {
                var indexIdOld = _config.indexId;
                RefreshServers();
                if (indexIdOld != _config.indexId)
                {
                    Reload();
                }
                if (_config.uiItem.enableAutoAdjustMainLvColWidth)
                {
                    _updateView("AdjustMainLvColWidth");
                }
            }
        }
        private void UpdateStatisticsHandler(ServerSpeedItem update)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (!Global.ShowInTaskbar)
                    {
                        return;
                    }
                    SpeedProxyDisplay = string.Format("{0}:{1}/s¡ü | {2}/s¡ý", Global.agentTag, Utils.HumanFy(update.proxyUp), Utils.HumanFy(update.proxyDown));
                    SpeedDirectDisplay = string.Format("{0}:{1}/s¡ü | {2}/s¡ý", Global.directTag, Utils.HumanFy(update.directUp), Utils.HumanFy(update.directDown));

                    if (update.proxyUp + update.proxyDown > 0)
                    {
                        var second = DateTime.Now.Second;
                        if (second % 3 == 0)
                        {
                            var item = _profileItems.Where(it => it.indexId == update.indexId).FirstOrDefault();
                            if (item != null)
                            {
                                item.todayDown = Utils.HumanFy(update.todayDown);
                                item.todayUp = Utils.HumanFy(update.todayUp);
                                item.totalDown = Utils.HumanFy(update.totalDown);
                                item.totalUp = Utils.HumanFy(update.totalUp);

                                if (SelectedProfile?.indexId == item.indexId)
                                {
                                    var temp = Utils.DeepCopy(item);
                                    _profileItems.Replace(item, temp);
                                    SelectedProfile = temp;
                                }
                                else
                                {
                                    _profileItems.Replace(item, Utils.DeepCopy(item));
                                }
                            }
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                Utils.SaveLog(ex.Message, ex);
            }
        }
        private void UpdateSpeedtestHandler(string indexId, string delay, string speed)
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                SetTestResult(indexId, delay, speed);
            }));
        }
        private void SetTestResult(string indexId, string delay, string speed)
        {
            if (Utils.IsNullOrEmpty(indexId))
            {
                _noticeHandler?.SendMessage(delay, true);
                _noticeHandler?.Enqueue(delay);
                return;
            }
            var item = _profileItems.Where(it => it.indexId == indexId).FirstOrDefault();
            if (item != null)
            {
                if (!Utils.IsNullOrEmpty(delay))
                {
                    int.TryParse(delay, out int temp);
                    item.delay = temp;
                    item.delayVal = $"{delay} {Global.DelayUnit}";
                }
                if (!Utils.IsNullOrEmpty(speed))
                {
                    item.speedVal = $"{speed} {Global.SpeedUnit}";
                }
                _profileItems.Replace(item, Utils.DeepCopy(item));
            }
        }

        private void OnHotkeyHandler(EGlobalHotkey e)
        {
            switch (e)
            {
                case EGlobalHotkey.ShowForm:
                    ShowHideWindow(null);
                    break;
                case EGlobalHotkey.SystemProxyClear:
                    SetListenerType(ESysProxyType.ForcedClear);
                    break;
                case EGlobalHotkey.SystemProxySet:
                    SetListenerType(ESysProxyType.ForcedChange);
                    break;
                case EGlobalHotkey.SystemProxyUnchanged:
                    SetListenerType(ESysProxyType.Unchanged);
                    break;
                case EGlobalHotkey.SystemProxyPac:
                    SetListenerType(ESysProxyType.Pac);
                    break;
            }
        }
        private void PreExit(bool blWindowsShutDown)
        {
            Utils.SaveLog("PreExit Begin");

            StorageUI();
            ConfigHandler.SaveConfig(ref _config);

            //HttpProxyHandle.CloseHttpAgent(config);
            if (blWindowsShutDown)
            {
                SysProxyHandle.ResetIEProxy4WindowsShutDown();
            }
            else
            {
                SysProxyHandle.UpdateSysProxy(_config, true);
            }

            ProfileExHandler.Instance.SaveTo();

            _statistics?.SaveTo();
            _statistics?.Close();

            _coreHandler.CoreStop();
            Utils.SaveLog("PreExit End");
        }
        public void MyAppExit(bool blWindowsShutDown)
        {
            try
            {
                PreExit(blWindowsShutDown);
            }
            catch { }
            finally
            {
                Utils.ExitSuccess();
            }
        }

        #endregion

        #region Servers && Groups

        public void SubSelectedChanged(bool c)
        {
            ProfileExpanded = false;
            if (!c)
            {
                return;
            }
            _subId = SelectedSub?.id;
            _config.subIndexId = _subId;

            RefreshServers();

            _updateView("ProfilesFocus");
        }

        private void ServerFilterChanged(bool c)
        {
            if (!c)
            {
                return;
            }
            _serverFilter = ServerFilter;
            RefreshServers();
        }

        private void RefreshServers()
        {
            List<ProfileItemModel> lstModel = LazyConfig.Instance.ProfileItems(_subId, _serverFilter);

            ConfigHandler.SetDefaultServer(_config, lstModel);

            List<ServerStatItem> lstServerStat = new();
            if (_statistics != null && _statistics.Enable)
            {
                lstServerStat = _statistics.ServerStat;
            }
            var lstProfileExs = ProfileExHandler.Instance.ProfileExs;
            lstModel = (from t in lstModel
                        join t2 in lstServerStat on t.indexId equals t2.indexId into t2b
                        from t22 in t2b.DefaultIfEmpty()
                        join t3 in lstProfileExs on t.indexId equals t3.indexId into t3b
                        from t33 in t3b.DefaultIfEmpty()
                        select new ProfileItemModel
                        {
                            indexId = t.indexId,
                            configType = t.configType,
                            remarks = t.remarks,
                            address = t.address,
                            port = t.port,
                            security = t.security,
                            network = t.network,
                            streamSecurity = t.streamSecurity,
                            subid = t.subid,
                            subRemarks = t.subRemarks,
                            isActive = t.indexId == _config.indexId,
                            sort = t33 == null ? 0 : t33.sort,
                            delay = t33 == null ? 0 : t33.delay,
                            delayVal = t33?.delay != 0 ? $"{t33?.delay} {Global.DelayUnit}" : string.Empty,
                            speedVal = t33?.speed != 0 ? $"{t33?.speed} {Global.SpeedUnit}" : string.Empty,
                            todayDown = t22 == null ? "" : Utils.HumanFy(t22.todayDown),
                            todayUp = t22 == null ? "" : Utils.HumanFy(t22.todayUp),
                            totalDown = t22 == null ? "" : Utils.HumanFy(t22.totalDown),
                            totalUp = t22 == null ? "" : Utils.HumanFy(t22.totalUp)
                        }).OrderBy(t => t.sort).ToList();
            _lstProfile = Utils.FromJson<List<ProfileItem>>(Utils.ToJson(lstModel));

            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                _profileItems.Clear();
                _profileItems.AddRange(lstModel);
                if (lstModel.Count > 0)
                {
                    var selected = lstModel.FirstOrDefault(t => t.indexId == _config.indexId);
                    if (selected != null)
                    {
                        SelectedProfile = selected;
                    }
                    else
                    {
                        SelectedProfile = lstModel[0];
                    }
                }

                RefreshServersMenu();

                //display running server 
                var running = ConfigHandler.GetDefaultServer(ref _config);
                if (running != null)
                {
                    var runningSummary = running.GetSummary();
                    RunningServerDisplay = $"{ResUI.menuServers}:{runningSummary}";
                    RunningServerToolTipText = runningSummary;
                }
                else
                {
                    RunningServerDisplay =
                    RunningServerToolTipText = ResUI.CheckServerSettings;
                }
            }));
        }

        private void RefreshServersMenu()
        {
            _servers.Clear();
            if (_lstProfile.Count > _config.guiItem.trayMenuServersLimit)
            {
                BlServers = false;
                return;
            }

            BlServers = true;
            for (int k = 0; k < _lstProfile.Count; k++)
            {
                ProfileItem it = _lstProfile[k];
                string name = it.GetSummary();

                var item = new ComboItem() { ID = it.indexId, Text = name };
                _servers.Add(item);
                if (_config.indexId == it.indexId)
                {
                    SelectedServer = item;
                }
            }
        }

        public void InitSubscriptionView()
        {
            _subItems.Clear();

            _subItems.Add(new SubItem { remarks = ResUI.AllGroupServers });
            foreach (var item in LazyConfig.Instance.SubItems().OrderByDescending(t => t.sort))
            {
                _subItems.Add(item);
            }
            if (_subId != null && _subItems.FirstOrDefault(t => t.id == _subId) != null)
            {
                SelectedSub = _subItems.FirstOrDefault(t => t.id == _subId);
            }
            else
            {
                SelectedSub = _subItems[0];
            }
        }

        #endregion

        #region Add Servers
        private int GetProfileItems(out List<ProfileItem> lstSelecteds, bool latest)
        {
            lstSelecteds = new List<ProfileItem>();
            if (SelectedProfiles == null || SelectedProfiles.Count <= 0)
            {
                return -1;
            }

            var orderProfiles = SelectedProfiles?.OrderBy(t => t.sort);
            if (latest)
            {
                foreach (var profile in orderProfiles)
                {
                    var item = LazyConfig.Instance.GetProfileItem(profile.indexId);
                    if (item is not null)
                    {
                        lstSelecteds.Add(item);
                    }
                }
            }
            else
            {
                lstSelecteds = Utils.FromJson<List<ProfileItem>>(Utils.ToJson(orderProfiles));
            }

            return 0;
        }

        public void EditServer(bool blNew, EConfigType eConfigType)
        {
            ProfileItem item;
            if (blNew)
            {
                item = new()
                {
                    subid = _subId,
                    configType = eConfigType,
                };
            }
            else
            {
                if (Utils.IsNullOrEmpty(SelectedProfile?.indexId))
                {
                    return;
                }
                item = LazyConfig.Instance.GetProfileItem(SelectedProfile.indexId);
                if (item is null)
                {
                    _noticeHandler?.Enqueue(ResUI.PleaseSelectServer);
                    return;
                }
                eConfigType = item.configType;
            }
            bool? ret = false;
            if (eConfigType == EConfigType.Custom)
            {
                ret = (new AddServer2Window(item)).ShowDialog();
            }
            else
            {
                ret = (new AddServerWindow(item)).ShowDialog();
            }
            if (ret == true)
            {
                RefreshServers();
                if (item.indexId == _config.indexId)
                {
                    Reload();
                }
            }
        }
        public (int, List<string>) HomeAddServerOrSubViaClipboard(string cData)
        {
            var (addedServersCount, addedSubIds) = ConfigHandler.HomeAddBatchServers(ref _config, cData, _subId, false, null);
            if (addedSubIds.Count > 0)
            {
                foreach (string id in addedSubIds)
                {
                    if (Utils.IsSystemProxyEnabled(_config.sysProxyType))
                    {
                        UpdateSubscriptionProcess(id, true);
                    }
                    else
                    {
                        UpdateSubscriptionProcess(id, false);
                    }
                }
            }
            return (addedServersCount, addedSubIds);
        }
        public void AddServerOrSubViaClipboard()
        {
            string clipboardData = Utils.GetClipboardData();
            int ret = ConfigHandler.AddBatchServers(ref _config, clipboardData, _subId, false);
            if (ret > 0)
            {
                InitSubscriptionView();
                RefreshServers();
                _noticeHandler?.Enqueue(string.Format(ResUI.SuccessfullyImportedServerViaClipboard, ret));

                // This update all subscriptions
                // TODO: update just added sub, if was a sub added
                UpdateSubscriptionProcess("", Utils.IsSystemProxyEnabled(_config.sysProxyType));
            }
        }
        public void AddServerOrSubViaDeepLink(string url)
        {
            int ret = ConfigHandler.AddBatchServers(ref _config, url, _subId, false);
            if (ret > 0)
            {
                InitSubscriptionView();
                RefreshServers();
                _noticeHandler?.Enqueue(string.Format(ResUI.SuccessfullyImportedServerViaClipboard, ret));

                // This update all subscriptions
                // TODO: update just added sub, if was a sub added
                UpdateSubscriptionProcess("", Utils.IsSystemProxyEnabled(_config.sysProxyType));
            }
        }
        public void AddServersViaDeeplink(string servers_link)
        {

        }
        public void AddSubViaDeeplink(string sub_url)
        {

        }
        public void AddSubAndServerViaDeeplink(string data)
        {

        }
        public async Task ScanScreenTaskAsync()
        {
            ShowHideWindow(false);

            string result = await Task.Run(() =>
            {
                return Utils.ScanScreen();
            });

            ShowHideWindow(true);

            if (Utils.IsNullOrEmpty(result))
            {
                _noticeHandler?.Enqueue(ResUI.NoValidQRcodeFound);
            }
            else
            {
                int ret = ConfigHandler.AddBatchServers(ref _config, result, _subId, false);
                if (ret > 0)
                {
                    InitSubscriptionView();
                    RefreshServers();
                    _noticeHandler?.Enqueue(ResUI.SuccessfullyImportedServerViaScan);
                }
            }
        }
        public void RemoveServer()
        {
            if (GetProfileItems(out List<ProfileItem> lstSelecteds, false) < 0)
            {
                return;
            }

            if (UI.ShowYesNo(ResUI.RemoveServer) == DialogResult.No)
            {
                return;
            }
            var exists = lstSelecteds.Exists(t => t.indexId == _config.indexId);

            ConfigHandler.RemoveServer(_config, lstSelecteds);
            _noticeHandler?.Enqueue(ResUI.OperationSuccess);

            RefreshServers();
            if (exists)
            {
                Reload();
            }
        }

        private void RemoveDuplicateServer()
        {
            int oldCount = _lstProfile.Count;
            int newCount = ConfigHandler.DedupServerList(ref _config, ref _lstProfile);
            RefreshServers();
            Reload();
            _noticeHandler?.Enqueue(string.Format(ResUI.RemoveDuplicateServerResult, oldCount, newCount));
        }
        private void CopyServer()
        {
            if (GetProfileItems(out List<ProfileItem> lstSelecteds, false) < 0)
            {
                return;
            }
            if (ConfigHandler.CopyServer(ref _config, lstSelecteds) == 0)
            {
                RefreshServers();
                _noticeHandler?.Enqueue(ResUI.OperationSuccess);
            }
        }

        public void SetDefaultServer()
        {
            if (Utils.IsNullOrEmpty(SelectedProfile?.indexId))
            {
                return;
            }
            SetDefaultServer(SelectedProfile.indexId);
        }
        private void SetDefaultServer(string indexId)
        {
            if (Utils.IsNullOrEmpty(indexId))
            {
                return;
            }
            if (indexId == _config.indexId)
            {
                return;
            }
            var item = LazyConfig.Instance.GetProfileItem(indexId);
            if (item is null)
            {
                _noticeHandler?.Enqueue(ResUI.PleaseSelectServer);
                return;
            }

            if (ConfigHandler.SetDefaultServerIndex(ref _config, indexId) == 0)
            {
                RefreshServers();
                Reload();
            }
        }
        private void ServerSelectedChanged(bool c)
        {
            if (!c)
            {
                return;
            }
            if (SelectedServer == null)
            {
                return;
            }
            if (Utils.IsNullOrEmpty(SelectedServer.ID))
            {
                return;
            }
            SetDefaultServer(SelectedServer.ID);
        }


        public async void ShareServer()
        {
            var item = LazyConfig.Instance.GetProfileItem(SelectedProfile.indexId);
            if (item is null)
            {
                _noticeHandler?.Enqueue(ResUI.PleaseSelectServer);
                return;
            }
            string url = ShareHandler.GetShareUrl(item);
            if (Utils.IsNullOrEmpty(url))
            {
                return;
            }
            var img = QRCodeHelper.GetQRCode(url);
            var dialog = new QrcodeView()
            {
                imgQrcode = { Source = img },
                txtContent = { Text = url },
            };

            await DialogHost.Show(dialog, "RootDialog");
        }

        public void SortServer(string colName)
        {
            if (Utils.IsNullOrEmpty(colName))
            {
                return;
            }

            if (!_dicHeaderSort.ContainsKey(colName))
            {
                _dicHeaderSort.Add(colName, true);
            }
            _dicHeaderSort.TryGetValue(colName, out bool asc);
            if (ConfigHandler.SortServers(ref _config, _subId, colName, asc) != 0)
            {
                return;
            }
            _dicHeaderSort[colName] = !asc;
            RefreshServers();
        }

        public void TestServerAvailability()
        {
            var item = ConfigHandler.GetDefaultServer(ref _config);
            if (item == null || item.configType == EConfigType.Custom)
            {
                return;
            }
            (new UpdateHandle()).RunAvailabilityCheck((bool success, string msg) =>
            {
                _noticeHandler?.SendMessage(msg, true);
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (!Global.ShowInTaskbar)
                    {
                        return;
                    }
                    RunningInfoDisplay = msg;
                }));
            });
        }

        //move server
        private void MoveToGroup(bool c)
        {
            if (!c)
            {
                return;
            }

            if (GetProfileItems(out List<ProfileItem> lstSelecteds, false) < 0)
            {
                return;
            }

            ConfigHandler.MoveToGroup(_config, lstSelecteds, SelectedMoveToGroup.id);
            _noticeHandler?.Enqueue(ResUI.OperationSuccess);

            RefreshServers();
            SelectedMoveToGroup = new();
            //Reload();
        }

        public void MoveServer(EMove eMove)
        {
            var item = _lstProfile.FirstOrDefault(t => t.indexId == SelectedProfile.indexId);
            if (item is null)
            {
                _noticeHandler?.Enqueue(ResUI.PleaseSelectServer);
                return;
            }

            int index = _lstProfile.IndexOf(item);
            if (index < 0)
            {
                return;
            }
            if (ConfigHandler.MoveServer(ref _config, ref _lstProfile, index, eMove) == 0)
            {
                RefreshServers();
            }
        }

        public void MoveServerTo(int startIndex, ProfileItemModel targetItem)
        {
            var targetIndex = _profileItems.IndexOf(targetItem);
            if (startIndex >= 0 && targetIndex >= 0 && startIndex != targetIndex)
            {
                if (ConfigHandler.MoveServer(ref _config, ref _lstProfile, startIndex, EMove.Position, targetIndex) == 0)
                {
                    RefreshServers();
                }
            }
        }

        public void ServerSpeedtest(ESpeedActionType actionType)
        {
            if (actionType == ESpeedActionType.Mixedtest)
            {
                SelectedProfiles = _profileItems;
            }
            if (GetProfileItems(out List<ProfileItem> lstSelecteds, false) < 0)
            {
                return;
            }
            //ClearTestResult();
            new SpeedtestHandler(_config, _coreHandler, lstSelecteds, actionType, UpdateSpeedtestHandler);
        }

        private void Export2ClientConfig()
        {
            var item = LazyConfig.Instance.GetProfileItem(SelectedProfile.indexId);
            if (item is null)
            {
                _noticeHandler?.Enqueue(ResUI.PleaseSelectServer);
                return;
            }
            MainFormHandler.Instance.Export2ClientConfig(item, _config);
        }

        private void Export2ServerConfig()
        {
            var item = LazyConfig.Instance.GetProfileItem(SelectedProfile.indexId);
            if (item is null)
            {
                _noticeHandler?.Enqueue(ResUI.PleaseSelectServer);
                return;
            }
            MainFormHandler.Instance.Export2ServerConfig(item, _config);
        }

        public void Export2ShareUrl()
        {
            if (GetProfileItems(out List<ProfileItem> lstSelecteds, true) < 0)
            {
                return;
            }

            StringBuilder sb = new();
            foreach (var it in lstSelecteds)
            {
                string url = ShareHandler.GetShareUrl(it);
                if (Utils.IsNullOrEmpty(url))
                {
                    continue;
                }
                sb.Append(url);
                sb.AppendLine();
            }
            if (sb.Length > 0)
            {
                Utils.SetClipboardData(sb.ToString());
                _noticeHandler?.SendMessage(ResUI.BatchExportURLSuccessfully);
            }
        }

        private void Export2SubContent()
        {
            if (GetProfileItems(out List<ProfileItem> lstSelecteds, true) < 0)
            {
                return;
            }

            StringBuilder sb = new();
            foreach (var it in lstSelecteds)
            {
                string? url = ShareHandler.GetShareUrl(it);
                if (Utils.IsNullOrEmpty(url))
                {
                    continue;
                }
                sb.Append(url);
                sb.AppendLine();
            }
            if (sb.Length > 0)
            {
                Utils.SetClipboardData(Utils.Base64Encode(sb.ToString()));
                _noticeHandler?.SendMessage(ResUI.BatchExportSubscriptionSuccessfully);
            }
        }

        #endregion

        #region Subscription

        private void SubSetting()
        {
            if ((new SubSettingWindow()).ShowDialog() == true)
            {
                // Update view
                InitSubscriptionView();
                SubSelectedChanged(true);

                // Update Subscription after add
                SubItem latestSubItem = LazyConfig.Instance.GetLastSubItem();
                if (latestSubItem != null)
                {
                    UpdateSubscriptionProcess(latestSubItem.id, true);
                }
            }
        }
        private void AddSub()
        {
            SubItem item = new();
            var ret = (new SubEditWindow(item)).ShowDialog();
            if (ret == true)
            {
                // Update view
                InitSubscriptionView();
                SubSelectedChanged(true);

                // Update Subscription after add
                SubItem latestSubItem = LazyConfig.Instance.GetLastSubItem();
                if (latestSubItem != null)
                {
                    UpdateSubscriptionProcess(latestSubItem.id, true);
                }
            }
        }


        public void UpdateSubscriptionProcess(string subId, bool blProxy)
        {
            (new UpdateHandle()).UpdateSubscriptionProcess(_config, subId, blProxy, UpdateTaskHandler);
        }

        #endregion

        #region Setting

        private void OptionSetting()
        {
            var ret = (new OptionSettingWindow()).ShowDialog();
            if (ret == true)
            {
                //RefreshServers();
                Reload();
                TunModeSwitch();
            }
        }
        private void RoutingSetting()
        {
            var ret = (new RoutingSettingWindow()).ShowDialog();
            if (ret == true)
            {
                ConfigHandler.InitBuiltinRouting(ref _config);
                RefreshRoutingsMenu();
                //RefreshServers();
                Reload();
            }
        }

        private void ImportOldGuiConfig()
        {
            OpenFileDialog fileDialog = new()
            {
                Multiselect = false,
                Filter = "guiNConfig|*.json|All|*.*"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = fileDialog.FileName;
            if (Utils.IsNullOrEmpty(fileName))
            {
                return;
            }

            var ret = ConfigHandler.ImportOldGuiConfig(ref _config, fileName);
            if (ret == 0)
            {
                RefreshRoutingsMenu();
                InitSubscriptionView();
                RefreshServers();
                Reload();
                UI.Show(ResUI.OperationSuccess);
            }
            else
            {
                _noticeHandler.Enqueue(ResUI.OperationFailed);
            }
        }

        #endregion

        #region CheckUpdate

        private void CheckUpdateN()
        {
            void _updateUI(bool success, string msg)
            {
                _noticeHandler?.SendMessage(msg);
                if (success)
                {
                    MyAppExit(false);
                }
            };
            (new UpdateHandle()).CheckUpdateGuiN(_config, _updateUI, _config.guiItem.checkPreReleaseUpdate);
        }

        private void CheckUpdateCore(ECoreType type)
        {
            void _updateUI(bool success, string msg)
            {
                _noticeHandler?.SendMessage(msg);
                if (success)
                {
                    CloseV2ray();

                    string fileName = Utils.GetTempPath(Utils.GetDownloadFileName(msg));
                    string toPath = Utils.GetBinPath("", type);

                    FileManager.ZipExtractToFile(fileName, toPath, _config.guiItem.ignoreGeoUpdateCore ? "geo" : "");

                    _noticeHandler?.SendMessage(ResUI.MsgUpdateV2rayCoreSuccessfullyMore);

                    Reload();

                    _noticeHandler?.SendMessage(ResUI.MsgUpdateV2rayCoreSuccessfully);

                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
            };
            (new UpdateHandle()).CheckUpdateCore(type, _config, _updateUI, _config.guiItem.checkPreReleaseUpdate);
        }

        private void CheckUpdateGeo()
        {
            Task.Run(() =>
            {
                var updateHandle = new UpdateHandle();
                updateHandle.UpdateGeoFile("geosite", _config, UpdateTaskHandler);
                updateHandle.UpdateGeoFile("geoip", _config, UpdateTaskHandler);
            });
        }

        #endregion

        #region v2ray job

        public void Reload()
        {
            _ = LoadV2ray();
        }


        async Task LoadV2ray()
        {
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                BlReloadEnabled = false;
            }));

            //if (Global.reloadV2ray)
            //{
            //    _noticeHandler?.SendMessage(Global.CommandClearMsg);
            //}
            await Task.Run(() =>
            {
                _coreHandler.LoadCore(_config);

                //ConfigHandler.SaveConfig(ref _config, false);

                ChangeSystemProxyStatus(_config.sysProxyType, false);
            });

            TestServerAvailability();

            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                BlReloadEnabled = true;
            }));
            ServerSpeedtest(ESpeedActionType.Realping);
        }

        private void CloseV2ray()
        {
            ConfigHandler.SaveConfig(ref _config, false);

            ChangeSystemProxyStatus(ESysProxyType.ForcedClear, false);

            _coreHandler.CoreStop();
        }

        #endregion

        #region System proxy and Routings

        public void SetListenerType(ESysProxyType type)
        {
            if (_config.sysProxyType == type)
            {
                return;
            }
            _config.sysProxyType = type;
            ChangeSystemProxyStatus(type, true);

            SystemProxySelected = (int)_config.sysProxyType;
            ConfigHandler.SaveConfig(ref _config, false);
        }

        private void ChangeSystemProxyStatus(ESysProxyType type, bool blChange)
        {
            SysProxyHandle.UpdateSysProxy(_config, false);
            _noticeHandler?.SendMessage(ResUI.TipChangeSystemProxy, true);

            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                BlSystemProxyClear = (type == ESysProxyType.ForcedClear);
                BlSystemProxySet = (type == ESysProxyType.ForcedChange);
                BlSystemProxyNothing = (type == ESysProxyType.Unchanged);
                BlSystemProxyPac = (type == ESysProxyType.Pac);

                InboundDisplayStaus();

                if (blChange)
                {
                    NotifyIcon = MainFormHandler.Instance.GetNotifyIcon(_config);
                    AppIcon = MainFormHandler.Instance.GetAppIcon(_config);
                }
                if (BlSystemProxySet || BlSystemProxyPac)
                {
                    SysProxyState = true;
                }
                else
                {
                    SysProxyState = false;
                }
            }));
        }

        private void RefreshRoutingsMenu()
        {
            _routingItems.Clear();
            if (!_config.routingBasicItem.enableRoutingAdvanced)
            {
                BlRouting = false;
                return;
            }

            BlRouting = true;
            var routings = LazyConfig.Instance.RoutingItems();
            foreach (var item in routings)
            {
                _routingItems.Add(item);
                if (item.id == _config.routingBasicItem.routingIndexId)
                {
                    SelectedRouting = item;
                }
            }
        }
        private void RoutingSelectedChanged(bool c)
        {
            if (!c)
            {
                return;
            }

            if (SelectedRouting == null)
            {
                return;
            }

            var item = LazyConfig.Instance.GetRoutingItem(SelectedRouting?.id);
            if (item is null)
            {
                return;
            }
            if (_config.routingBasicItem.routingIndexId == item.id)
            {
                return;
            }

            if (ConfigHandler.SetDefaultRouting(ref _config, item) == 0)
            {
                _noticeHandler?.SendMessage(ResUI.TipChangeRouting, true);
                Reload();
            }
        }

        void DoSystemProxySelected(bool c)
        {
            if (!c)
            {
                return;
            }
            if (_config.sysProxyType == (ESysProxyType)SystemProxySelected)
            {
                return;
            }
            SetListenerType((ESysProxyType)SystemProxySelected);
        }
        private void SetSysProxy()
        {
            SetListenerType(ESysProxyType.ForcedChange);
            SysProxyState = true;
            _config.sysProxyType= ESysProxyType.ForcedChange;
        }
        private void UnsetSysProxy()
        {
            SetListenerType(ESysProxyType.ForcedClear);
            SysProxyState = false;
            _config.sysProxyType = ESysProxyType.ForcedClear;
        }
        public void ToggleSysProxy()
        {
            // The "SysProxyState" variable will be change in the UI
            // that means when it's off and user clicks on that we will have "SysProxyState" true, cause it was changed when user clicked
            if (SysProxyState)
            {
                SetSysProxy();
            }
            else
            {
                UnsetSysProxy();
            }
        }

        void DoEnableTun(bool c)
        {
            // If it's for initiation, we ignore (user didn't change the value)
            if (ForInitiationTun)
            {
                ForInitiationTun = false;
                return;
            }

            if (Utils.IsAdministrator())
            {
                if (_config.tunModeItem.enableTun != EnableTun)
                {
                    _config.tunModeItem.enableTun = EnableTun;
                }
                TunModeSwitch();
            }

        }

        void TunModeSwitch()
        {
            if (EnableTun)
            {
                TunHandler.Instance.Start();
            }
            else
            {
                TunHandler.Instance.Stop();
            }
        }

        #endregion

        #region UI

        public void ShowHideWindow(bool? blShow)
        {
            var bl = blShow ?? !Global.ShowInTaskbar;
            if (bl)
            {
                //Application.Current.MainWindow.ShowInTaskbar = true;
                Application.Current.MainWindow.Show();
                if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                }
                Application.Current.MainWindow.Activate();
                Application.Current.MainWindow.Focus();
            }
            else
            {
                Application.Current.MainWindow.Hide();
                //Application.Current.MainWindow.ShowInTaskbar = false;
                //IntPtr windowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                //Utils.RegWriteValue(Global.MyRegPath, Utils.WindowHwndKey, Convert.ToString((long)windowHandle));
            };
            Global.ShowInTaskbar = bl;
        }

        private void RestoreUI()
        {
            ModifyTheme(_config.uiItem.colorModeDark);

            if (!_config.uiItem.colorPrimaryName.IsNullOrEmpty())
            {
                var swatch = new SwatchesProvider().Swatches.FirstOrDefault(t => t.Name == _config.uiItem.colorPrimaryName);
                if (swatch != null
                   && swatch.ExemplarHue != null
                   && swatch.ExemplarHue?.Color != null)
                {
                    ChangePrimaryColor(swatch.ExemplarHue.Color);
                }
            }
        }
        private void StorageUI()
        {
        }

        private void BindingUI()
        {
            ColorModeDark = _config.uiItem.colorModeDark;
            _swatches.AddRange(new SwatchesProvider().Swatches);
            if (!_config.uiItem.colorPrimaryName.IsNullOrEmpty())
            {
                SelectedSwatch = _swatches.FirstOrDefault(t => t.Name == _config.uiItem.colorPrimaryName);
            }
            CurrentFontSize = _config.uiItem.currentFontSize;
            CurrentLanguage = _config.uiItem.currentLanguage;

            this.WhenAnyValue(
                  x => x.ColorModeDark,
                  y => y == true)
                      .Subscribe(c =>
                      {
                          
                              if (_config.uiItem.colorModeDark != ColorModeDark)
                              {
                                  _config.uiItem.colorModeDark = ColorModeDark;
                                  ModifyTheme(ColorModeDark);
                                  ConfigHandler.SaveConfig(ref _config);
                              }
                          
                      });

            this.WhenAnyValue(
              x => x.SelectedSwatch,
              y => y != null && !y.Name.IsNullOrEmpty())
                 .Subscribe(c =>
                 {
                     if (SelectedSwatch == null
                     || SelectedSwatch.Name.IsNullOrEmpty()
                     || SelectedSwatch.ExemplarHue == null
                     || SelectedSwatch.ExemplarHue?.Color == null)
                     {
                         return;
                     }
                     if (_config.uiItem.colorPrimaryName != SelectedSwatch?.Name)
                     {
                         _config.uiItem.colorPrimaryName = SelectedSwatch?.Name;
                         ChangePrimaryColor(SelectedSwatch.ExemplarHue.Color);
                         ConfigHandler.SaveConfig(ref _config);
                     }
                 });

            this.WhenAnyValue(
               x => x.CurrentFontSize,
               y => y > 0)
                  .Subscribe(c =>
                  {
                      if (CurrentFontSize >= Global.MinFontSize)
                      {
                          _config.uiItem.currentFontSize = CurrentFontSize;
                          double size = (long)CurrentFontSize;
                          Application.Current.Resources["StdFontSize"] = size;
                          Application.Current.Resources["StdFontSize1"] = size + 1;
                          Application.Current.Resources["StdFontSize2"] = size + 2;
                          Application.Current.Resources["StdFontSizeMsg"] = size - 1;

                          ConfigHandler.SaveConfig(ref _config);
                      }
                  });

            this.WhenAnyValue(
             x => x.CurrentLanguage,
             y => y != null && !y.IsNullOrEmpty())
                .Subscribe(c =>
                {
                    if (!Utils.IsNullOrEmpty(CurrentLanguage))
                    {
                        if (ForInitiationLanguage)
                        {
                            ForInitiationLanguage = false;
                        }
                        else
                        {
                            if (IsForSettingBackLanguage)
                            {
                                IsForSettingBackLanguage = false;
                                return;
                            }

                            //var userRes = UI.ShowYesNo(ResUI.MsgProgramNeedsRestarting);
                            //if (userRes == DialogResult.Yes)
                            {
                                Thread.CurrentThread.CurrentUICulture = new(CurrentLanguage);
                                CultureInfo.DefaultThreadCurrentUICulture = new(_config.uiItem.currentLanguage);
                                _config.uiItem.currentLanguage = CurrentLanguage;
                                ConfigHandler.SaveConfig(ref _config);

                                // Restart program
                                // PreExit(true);
                                //Utils.RestartProgram();
                                Thread.CurrentThread.CurrentUICulture = new(_config.uiItem.currentLanguage);
                                var newWindow = new MainWindow();

                                // copy over any necessary properties from the old window
                                newWindow.DataContext = this;

                                // show the new window and close the old window
                                var old = Application.Current.MainWindow;

                                newWindow.Show();
                                newWindow.WindowState = WindowState.Minimized;
                                newWindow.WindowState = WindowState.Normal;
                                newWindow.Activate();
                                newWindow.Topmost = true;
                                newWindow.Topmost = false;
                                newWindow.Focus();

                                newWindow.Show();
                                old.Close();
                                Application.Current.MainWindow = newWindow;

                                // Refresh subs information when language changed
                                InitSubscriptionView();
                                // Exit the current instance of the application
                                //Application.Current.Shutdown();

                            }
                            /*else
                            {
                                //TODO: it doesn't setting back language, in fact it does for variable but in ui doesn't
                                IsForSettingBackLanguage = true;
                                CurrentLanguage = _config.uiItem.currentLanguage;
                            }*/
                        }

                    }
                });
        }

        public void InboundDisplayStaus()
        {
            StringBuilder sb = new();
            sb.Append($"[{Global.InboundSocks}:{LazyConfig.Instance.GetLocalPort(Global.InboundSocks)}]");
            sb.Append(" | ");
            //if (_config.sysProxyType == ESysProxyType.ForcedChange)
            //{
            //    sb.Append($"[{Global.InboundHttp}({ResUI.SystemProxy}):{LazyConfig.Instance.GetLocalPort(Global.InboundHttp)}]");
            //}
            //else
            //{
            sb.Append($"[{Global.InboundHttp}:{LazyConfig.Instance.GetLocalPort(Global.InboundHttp)}]");
            //}
            InboundDisplay = $"{ResUI.LabLocal}:{sb}";

            if (_config.inbound[0].allowLANConn)
            {
                if (_config.inbound[0].newPort4LAN)
                {
                    StringBuilder sb2 = new();
                    sb2.Append($"[{Global.InboundSocks}:{LazyConfig.Instance.GetLocalPort(Global.InboundSocks2)}]");
                    sb2.Append(" | ");
                    sb2.Append($"[{Global.InboundHttp}:{LazyConfig.Instance.GetLocalPort(Global.InboundHttp2)}]");
                    InboundLanDisplay = $"{ResUI.LabLAN}:{sb2}";
                }
                else
                {
                    InboundLanDisplay = $"{ResUI.LabLAN}:{sb}";
                }
            }
            else
            {
                InboundLanDisplay = $"{ResUI.LabLAN}:None";
            }
        }

        public void ModifyTheme(bool isDarkTheme)
        {
            var theme = _paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            _paletteHelper.SetTheme(theme);

            Utils.SetDarkBorder(Application.Current.MainWindow, isDarkTheme);
        }
        public void ChangePrimaryColor(System.Windows.Media.Color color)
        {
            var theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());

            _paletteHelper.SetTheme(theme);
        }

        private void AutoHideStartup()
        {
            if (_config.uiItem.autoHideStartup)
            {
                Observable.Range(1, 1)
                 .Delay(TimeSpan.FromSeconds(1))
                 .Subscribe(x =>
                 {
                     Application.Current.Dispatcher.Invoke(() =>
                     {
                         ShowHideWindow(false);
                     });
                 });
            }
        }

        #endregion

        #region Home
        public void HomeNewProfile()
        {
            // Get clipboard data
            string? cData = Utils.GetClipboardData();
            if (cData == null)
            {
                _noticeHandler.Enqueue("Please copy servers or subscriptions");
                // Pop up to user that they should copy some address first
            }
            else
            {
                var (addedServersCount, addedSubsIds) = HomeAddServerOrSubViaClipboard(cData);
                _noticeHandler.SendMessage($"{addedServersCount} addded\n{addedSubsIds.Count} sub added");
                Console.WriteLine();
            }
        }
        public void HomeUpdateUsage(string subId)
        {
            SubItem sub = LazyConfig.Instance.GetSubItem(subId);
            if (sub != null)
            {
                var headers = Utils.GetUrlResponseHeader(sub.url);
                if (headers != null)
                {
                    var subInfo = Utils.GetSubscriptionInfoFromHeaders(headers);
                    if (subInfo != null)
                    {
                        sub.upload = subInfo.Upload;
                        sub.download = subInfo.Download;
                        sub.total = subInfo.Total;
                        sub.expireDate = subInfo.ExpireDate;
                        sub.remaningExpireDays = sub.DaysLeftToExpire();
                        sub.UsedDataGB = sub.UsedDataGigaBytes();
                        sub.TotalDataGB = sub.TotalDataGigaBytes();
                        sub.profileWebPageUrl = subInfo.ProfileWebPageUrl;

                        // Replace the sub with new information
                        if (ConfigHandler.AddSubItem(ref _config, sub) == 0)
                        {
                            //successed
                        }
                        else
                        {
                            //failed
                        }
                    }
                }
            }
        }
        public async Task HomeConnect(bool forceConnect = false)
        {
            if (SelectedSub == null)
            {
                _noticeHandler.Enqueue("Please select a sub");
                return;
            }
            // It's disconnected or it should be connected again
            if (forceConnect || !IsConnected)
            {
                // Change connectVPN button color
                ConnectProgress = true;
                ConnectColor = "#FFFF0000";

                // User selected a proxy mode
                if (HomeSelectedProxyMode != null)
                {
                    string proxyModeRemark = HomeSelectedProxyMode.Content.ToString();
                    // Handle manual mode
                    if (proxyModeRemark == "Manual")
                    {
                        if (Utils.IsNullOrEmpty(_config.indexId))
                        {
                            //TODO: Send message to user about what happend
                            _noticeHandler.Enqueue("Please select a server to connect");
                            ConnectColor = "#FFE0E0E0";
                            ConnectProgress = false;
                            return;
                        }

                        SetDefaultServer(_config.indexId);
                    }
                    else
                    {
                        // Now the selected proxy mode is auto either load balance
                        ProfileItem server = GetSelectedServer(SelectedSub.id, proxyModeRemark);
                        if (server == null)
                        {
                            //TODO: Send message to user about what happend
                            ConnectColor = "#FFE0E0E0";
                            ConnectProgress = false;
                            return;
                        }
                        SetDefaultServer(server.indexId);
                    }
                }
                // There is no selected proxy mode (we use default proxy setting)
                else
                {
                    ProfileItem? server = null;
                    // Just set a default mode
                    var subServers = LazyConfig.Instance.ProfileItems(SelectedSub.id);
                    if (DefaultProxyMode == "Auto")
                        server = subServers.FirstOrDefault(s => s.remarks == "Lowest Ping");
                    else if (DefaultProxyMode == "Load Balance")
                        server = subServers.FirstOrDefault(s => s.remarks == DefaultProxyMode);

                    if (server == null)
                    {
                        //TODO Send message to user about what happend
                        ConnectColor = "#FFE0E0E0";
                        ConnectProgress = false;
                        return;
                    }

                    SetDefaultServer(server.indexId);
                }

                // Till now, we started a server
                // Now we calculate real ping of the server to make sure, it's working
                DelayProgress = true;
                HomeRealPingServer(_config.indexId);
                // Wait for delay calculation (10 seconds)
                short count = 1;
                while (!IsDelayCalculationFinished)
                {
                    // We don't want to stuck in a infinite loop
                    if (count == 25)
                        break;

                    await Task.Delay(400).ConfigureAwait(false);
                    count += 1;
                }
                ConnectProgress = false;
                DelayProgress = false;
                // Check delay
                if (SelectedProfileDelay > 0 && SelectedProfileDelay != -1)
                {
                    // The server works

                    //TODO: @hiddify1; change the connectVPN color to whatever should be
                    ConnectVPNLabelColor = "#7CFC0000";
                    ConnectVPNLabel = "Connected";
                    ConnectColor = "#7CFC00";
                    IsConnected = true;
                }
                else
                {
                    // The server doesn't work

                    //TODO: @hiddify1; change the connectVPN color to whatever should be
                    ConnectColor = "#FFFF0000";
                    ConnectVPNLabel = "Not Connected";
                    ConnectVPNLabelColor = "#FFFF0000";
                    IsConnected = false;
                }
                SetSysProxy();
                return;
            }

            // It's connected, should be disconnected

            UnsetSysProxy();

            //TODO: @hiddify1; the connectVPN color should be reset
            ConnectColor = "#FFE0E0E0";

            //SelectedProfile = null;
        }
        public void HomeGotoProfile(string subId)
        {
            SubItem sub = LazyConfig.Instance.GetSubItem(subId);
            var ret = (new SubEditWindow(sub)).ShowDialog();
            if (ret == true)
            {
                InitSubscriptionView();
            }
        }

        public void HomeSelectedRouteChanged()
        {
            Console.WriteLine(HomeSelectedRoutingItem);
            
        }
        public async Task HomeSelectedProxyChanged()
        {
            ProfileExpanded = false;
            if (HomeSelectedProxyMode?.Content?.ToString() == "Manual")
            { 
                ToggleV2rayPanel();
                return;
            }
            else if (V2RayNPanelVisible)
            {
                ToggleV2rayPanel();
            }
            if (IsConnected)
            {
                await HomeConnect(true).ConfigureAwait(false);
            }
        }
        #endregion

        private void ToggleV2rayPanel()
        {
            V2RayNPanelVisible = !V2RayNPanelVisible;

            MaxWindowWidth = V2RayNPanelVisible ? 2100 : 420;
            WindowWidth = V2RayNPanelVisible ? 1200 : 420;
            ColorModeDark = !ColorModeDark;
            ColorModeDark = !ColorModeDark;
        }

        private ProfileItem? GetSelectedServer(string subId,string proxyMode)
        {
            if (subId.IsNullOrEmpty() || proxyMode.IsNullOrEmpty())
                return null;


            // Get selected sub items proxies/servers
            var subServers = LazyConfig.Instance.ProfileItems(SelectedSub.id);
            if (subServers.Count < 1)
            {
                return null;
            }

            if (proxyMode == "Manual")
                return null;

            ProfileItem server = null;

            if (proxyMode == "Auto")
            {
                server = subServers.FirstOrDefault(s => s.remarks == "Lowest Ping");
            }
            else if (proxyMode == "Load Balance")
            {
                server = subServers.FirstOrDefault(s => s.remarks == "Load Balance");
            }
            else
            {
                return null;
            }
            return server;
        }

        private async Task HomeRealPingServer(string serverIndexId)
        {
            IsDelayCalculationFinished = false;
            //DelayProgress = true;
            ProfileItem server = LazyConfig.Instance.GetProfileItem(serverIndexId);
            if (server != null)
            {
                //ClearTestResult();
                await Task.Run(() =>
                {
                    new SpeedtestHandler(_config, _coreHandler, new List<ProfileItem>() { server }, ESpeedActionType.Realping, UpdateHomeRealPingServer);
                });
                
            }
        }
        private void UpdateHomeRealPingServer(string indexId, string delay, string speed)
        {
            bool isNumberic = int.TryParse(delay, out int res);
            if (isNumberic)
            {
                SelectedProfileDelay = Convert.ToInt32(res);
                IsDelayCalculationFinished = true;
                //DelayProgress = false;
            }
            
        }
       
    }
}
