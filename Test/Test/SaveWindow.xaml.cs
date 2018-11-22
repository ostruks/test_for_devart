using System;
using System.Threading.Tasks;
using System.Windows;
using Test.DB;
using Test.Helpers;

namespace Test
{
    /// <summary>
    /// Interaction logic for SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        delegate void ParametrizedMethodInvoker();
        public SaveWindow(String fileName)
        {
            InitializeComponent();
            saveTextBox.Text = fileName;
        }
        public SaveWindow()
        {
            InitializeComponent();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(new ParametrizedMethodInvoker(log_left_accs));
                    DBAccess.SaveFile(TempValueFromDB.FileName, TempValueFromDB.Bytes);
                });
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void log_left_accs()
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker(log_left_accs));
                return;
            }
            TempValueFromDB.FileName = saveTextBox.Text;
        }
    }
}
