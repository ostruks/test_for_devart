using System;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Test.DB;
using Test.Helpers;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void ParametrizedMethodInvoker();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header.ToString())
            {
                case "New":
                    {
                        myTextBox.Text = String.Empty;
                        TempValueFromDB.FileName = String.Empty;
                    }
                    break;
                case "Open":
                    {
                        OpenWindow openWin = new OpenWindow();
                        openWin.ShowDialog();
                        myTextBox.Text = TempValueFromDB.Value;
                    }
                    break;
                case "Save":
                    {
                        if (TempValueFromDB.FileName.Equals(String.Empty))
                        {
                            SaveWindow saveWindow = new SaveWindow();
                            saveWindow.ShowDialog();
                        }
                        else
                        {
                            save();
                        }
                    }
                    break;
                case "SaveAs...":
                    {
                        TempValueFromDB.Bytes = Encoding.UTF8.GetBytes(myTextBox.Text);
                        SaveWindow saveWindow = new SaveWindow(TempValueFromDB.FileName);
                        saveWindow.ShowDialog();
                    }
                    break;
                case "Exit":
                    {
                        this.Close();
                    }
                    break;
            }
        }

        private async void save()
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker(log_left_accs));
                DBAccess.SaveFile(TempValueFromDB.FileName, TempValueFromDB.Bytes);
            });
        }

        void log_left_accs()
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker(log_left_accs));
                return;
            }
            TempValueFromDB.Bytes = Encoding.UTF8.GetBytes(myTextBox.Text);
        }
    }
}
