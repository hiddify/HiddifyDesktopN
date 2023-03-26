using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using v2rayN.ViewModels;

namespace v2rayN.Views
{
    /// <summary>
    /// Interaction logic for HiddifyUI.xaml
    /// </summary>
    public partial class HiddifyUI : UserControl
    {
        public static Snackbar Snackbar = new();

        public HiddifyUI()
        {
            InitializeComponent();
            //FlowDirection = FlowDirection.RightToLeft;
            Task.Factory.StartNew(() => Thread.Sleep(2500)).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue?.Enqueue("Welcome to Hiddify");
            }, TaskScheduler.FromCurrentSynchronizationContext());
            //var ViewModel = new HiddifyUIViewModel(MainSnackbar.MessageQueue!);

           
            //this.BindCommand(ViewModel, vm => vm.NewProfileCommand, v => v.SubInfoView.NewProfile;
            ModifyTheme(false);
        }
        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e)
        {

                   // => ModifyTheme(DarkModeToggleButton.IsChecked == true);
        }

        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void DarkModeToggleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DarkModeToggleButton_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void CloseNotificationPanel_Click(object sender, RoutedEventArgs e) => NotificationPanel.Visibility = Visibility.Collapsed;

        private void FAB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectVPN_Click(object sender, RoutedEventArgs e)
        {
            ConnectVPN.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xF2, 0x67));
            //((HomeWindowViewModel)DataContext).ConnectProgress = true;
            connectlbl.Content = "Connecting...";
            Task.Factory.StartNew(() => Thread.Sleep(2500)).ContinueWith(t =>
            {
                speedpanel.Visibility = Visibility.Visible;
                //((ViewModels.HiddifyUIViewModel)DataContext).ConnectProgress = false;
                ConnectVPN.Background = new SolidColorBrush(Colors.LightGreen);
                connectlbl.Content = "Connected Successfully";
            }, TaskScheduler.FromCurrentSynchronizationContext());

            //var xx = this.DataContext as MainWindowViewModel;
            //Console.WriteLine();
        }

        private void NewProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Going to User Page");
        }


        private void ActiveProfile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Going to User Page");
        }

        private void UpdateUsage_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Updateing Usage");
        }
    }

}
