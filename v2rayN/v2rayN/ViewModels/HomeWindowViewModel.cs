using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using v2rayN.Views;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using v2rayN.ViewModels;


namespace v2rayN.ViewModels
{
    public class HomeWindowViewModel : ViewModelBase
    {
        
        public HomeWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {

            DemoItems = new ObservableCollection<DemoItem>
            {
              new DemoItem(
                    "Home",
                    typeof(Home),
                    selectedIcon: PackIconKind.Home,
                    unselectedIcon: PackIconKind.HomeOutline)
            };

            
            MainDemoItems = new ObservableCollection<DemoItem>
            {
            };

            _demoItemsView = CollectionViewSource.GetDefaultView(DemoItems);
            

            
            DismissAllNotificationsCommand = new AnotherCommandImplementation(
                _ => DemoItems[0].DismissAllNotifications(),
                _ => DemoItems[0].Notifications != null);

            AddNewNotificationCommand = new AnotherCommandImplementation(
                _ => DemoItems[0].AddNewNotification());

            AddNewNotificationCommand.Execute(new object());
        }
        private readonly ICollectionView _demoItemsView;
        private DemoItem? _selectedItem;
        private int _selectedIndex;
        private string? _searchKeyword;
        private bool _controlsEnabled = true;


        public ObservableCollection<DemoItem> DemoItems { get; }
        public ObservableCollection<DemoItem> MainDemoItems { get; }

        public DemoItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }
        private bool _connectProgress = false;
        public bool ConnectProgress {
            get =>_connectProgress; 
            set => SetProperty(ref _connectProgress, value); 
        }
        public bool ControlsEnabled
        {
            get => _controlsEnabled;
            set => SetProperty(ref _controlsEnabled, value);
        }

        
        public AnotherCommandImplementation DismissAllNotificationsCommand { get; }
        public AnotherCommandImplementation AddNewNotificationCommand { get; }

        private static IEnumerable<DemoItem> GenerateDemoItems(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue is null)
                throw new ArgumentNullException(nameof(snackbarMessageQueue));

            yield return null;
        }
    }
}
