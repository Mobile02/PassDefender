using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PassDefender
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Body _body;

        public MainWindow()
        {
            InitializeComponent();
            
            FormPassDefender.Left = Properties.Settings.Default.Left;
            FormPassDefender.Top = Properties.Settings.Default.Top;
            FormPassDefender.Width = Properties.Settings.Default.Width;
            FormPassDefender.Height = Properties.Settings.Default.Height;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _body = new Body(this);
        }

        private void FormPassDefender_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Left = FormPassDefender.Left;
            Properties.Settings.Default.Top = FormPassDefender.Top;
            Properties.Settings.Default.Width = FormPassDefender.Width;
            Properties.Settings.Default.Height = FormPassDefender.Height;
            Properties.Settings.Default.Save();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            _body._data.RemoveAt(dataGrid.Items.IndexOf(dataGrid.SelectedCells[0].Item));
            dataGrid.Items.Refresh();
        }

        private void Button_generetion_Click(object sender, RoutedEventArgs e)
        {
            _body._dataString = (Data)dataGrid.SelectedCells[0].Item;
            _body._dataString.Password = new Randomizer().RndString(16);
            dataGrid.Items.Refresh();
        }

        private void Button_copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            _body._dataString = (Data)dataGrid.SelectedCells[0].Item;
            Crypto crypto = new Crypto();
            Clipboard.SetText(crypto.Decrypt(_body._dataString.Password, new DecryptPass(_body.PassPhrase, _body).Decrypt(), _body.SaltValue, _body.InitVector));

            _body._dispatcherTimer.Start();
            LabelInfo.Visibility = Visibility.Visible;
        }

        private void Button_copyLogin_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            _body._dataString = (Data)dataGrid.SelectedCells[0].Item;
            Clipboard.SetText(_body._dataString.Login);
        }
    }
}
