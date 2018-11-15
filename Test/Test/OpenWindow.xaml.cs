using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Test.DB;
using Test.Helpers;

namespace Test
{
    /// <summary>
    /// Interaction logic for OpenWindow.xaml
    /// </summary>
    public partial class OpenWindow : Window
    {
        delegate void ParametrizedMethodInvoker5(string arg);
        delegate void ParametrizedMethodInvoker6();
        public OpenWindow()
        {
            InitializeComponent();
            addToListView();
        }

        public async void addToListView()
        {
            await Task.Run(() =>
            {
                List<string> strs = DBAccess.FilesName();
                foreach (string str in strs)
                {
                    Dispatcher.Invoke(new ParametrizedMethodInvoker5(log_left_accs), str);
                }
            });
        }

        void log_left_accs(string arg)
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker5(log_left_accs), arg);
                return;
            }
            filesName.Items.Add(arg);
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(new ParametrizedMethodInvoker6(log_left_accs2));
                    TempValueFromDB.Value = DBAccess.OpenFile(TempValueFromDB.FileName);
                });
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void log_left_accs2()
        {
            if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
            {
                Dispatcher.Invoke(new ParametrizedMethodInvoker6(log_left_accs2));
                return;
            }
            TempValueFromDB.FileName = filesName.SelectedValue.ToString();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
