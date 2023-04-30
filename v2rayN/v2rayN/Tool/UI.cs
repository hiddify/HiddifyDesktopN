using System.Windows;

namespace v2rayN
{
    internal class UI
    {
        private static readonly string caption = "HiddifyN";

        public static void Show(string msg)
        {
            MessageBox.Show(msg, caption, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        public static void ShowWarning(string msg)
        {
            MessageBox.Show(msg, caption, MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
        }
        public static void ShowError(string msg)
        {
            MessageBox.Show(msg, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowYesNo(string msg)
        {
            return MessageBox.Show(msg, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}