using System;
using System.Collections.Generic;
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

namespace v2rayN.Views
{
    /// <summary>
    /// Interaction logic for SubInfoView.xaml
    /// </summary>
    public partial class SubInfoView : UserControl
    {
        
        public SubInfoView()
        {
            InitializeComponent();
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
