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
using v2rayN.Converters;
using v2rayN.Mode;

namespace v2rayN.ViewModels
{
    public class HomeWindowViewModel : ViewModelBase
    {

        public HomeWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue;
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

            NewProfileCommand = new AnotherCommandImplementation(ExecuteNewProfileDialog);


        }
        private async void ExecuteNewProfileDialog(object? _)
        {
            //let's set up a little MVVM, cos that's what the cool kids are doing:


            //show the dialog
            _snackbarMessageQueue.Enqueue("Nothing find in the Clipboard");

            //check the result...

        }
        public SubItem SelectedProfile { get; } = new SubItem
        {
            id = "A",
            remarks = "Profile Selected",
            url = "XAML Toolkit"
        };
        public ObservableCollection<SubItem> Items1 { get; } = new ObservableCollection<SubItem>()
            {
                new SubItem
                {
                    id = "A",
                    remarks = "Profile2",
                    url= "XAML Toolkit"
                },
                new SubItem
                {
                    id = "B",
                    remarks = "Profile3",
                    url= "Material Design in XAML Toolkit"
                },
                new SubItem
                {
                    id = "C",
                    remarks = "Profile4",
                    url= "Material "
                },
            };
    
        private readonly ICollectionView _demoItemsView;
        private DemoItem? _selectedItem;
        private int _selectedIndex;
        private string? _searchKeyword;
        private bool _controlsEnabled = true;
        private ISnackbarMessageQueue _snackbarMessageQueue;

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
        public AnotherCommandImplementation NewProfileCommand { get; }

        private static IEnumerable<DemoItem> GenerateDemoItems(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue is null)
                throw new ArgumentNullException(nameof(snackbarMessageQueue));

            yield return null;
        }
    }
}
