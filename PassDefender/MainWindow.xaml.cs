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
        private List<Data> _data = new List<Data>();
        private Data _dataString;
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();

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
            FileOperation fileOperation = new FileOperation();

            if (!fileOperation.CheckFileExists("keys.pdk"))
            {
                _dataString = new Data("Test", new Crypto().Encrypt("test", new DecryptPass(_body.PassPhrase, _body).Decrypt(), _body.SaltValue, _body.InitVector), "Эту запись можно удалить.", _body);
                _data.Add(_dataString);

                fileOperation.CreateFile("keys.pdk");
                fileOperation.SaveFile("keys.pdk", _data, _body);
            }

            _data = fileOperation.OpenFile("keys.pdk", _body);
            dataGrid.ItemsSource = _data;
            dataGrid.Items.Refresh();

            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }

        private void FormPassDefender_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Left = FormPassDefender.Left;
            Properties.Settings.Default.Top = FormPassDefender.Top;
            Properties.Settings.Default.Width = FormPassDefender.Width;
            Properties.Settings.Default.Height = FormPassDefender.Height;
            Properties.Settings.Default.Save();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            _dataString = new Data("", new Crypto().Encrypt("", new DecryptPass(_body.PassPhrase, _body).Decrypt(), _body.SaltValue, _body.InitVector), "", _body);
            _data.Add(_dataString);
            dataGrid.Items.Refresh();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            _data.RemoveAt(dataGrid.Items.IndexOf(dataGrid.SelectedCells[0].Item));
            dataGrid.Items.Refresh();
        }

        private void Button_generetion_Click(object sender, RoutedEventArgs e)
        {
            _dataString = (Data)dataGrid.SelectedCells[0].Item;
            _dataString.Password = new Randomizer().RndString(16);
            dataGrid.Items.Refresh();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.Items.Refresh();
            FileOperation fileOperation = new FileOperation();
            fileOperation.SaveFile("keys.pdk", _data, _body);
        }

        private void Button_copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            _dataString = (Data)dataGrid.SelectedCells[0].Item;
            Crypto crypto = new Crypto();
            Clipboard.SetText(crypto.Decrypt(_dataString.Password, new DecryptPass(_body.PassPhrase, _body).Decrypt(), _body.SaltValue, _body.InitVector));

            _dispatcherTimer.Start();
            LabelInfo.Visibility = Visibility.Visible;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            LabelInfo.Content = "Буфер обмена будет очищен через " + $"{(int)(2000 - progressBar.Value) / 100}";
            if ((progressBar.Value += 1) == 2000)
            {
                LabelInfo.Visibility = Visibility.Hidden;
                _dispatcherTimer.Stop();
                progressBar.Value = 0;
                Clipboard.Clear();
            }
        }

        private void Button_copyLogin_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            _dataString = (Data)dataGrid.SelectedCells[0].Item;
            Clipboard.SetText(_dataString.Login);
        }
    }
}
