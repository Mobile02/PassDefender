using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace PassDefender
{
    class Body
    {
        private MainWindow _mainWindow;
        private WindowVerify _windowVerify;
        public List<Data> _data = new List<Data>();
        public Data _dataString;
        public DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private bool _verify = false;

        public string PlainText { get; set; }
        public string PassPhrase { get; set; }
        public string SaltValue { get; set; }
        public string InitVector { get; set; }

        public Body(Window window)
        {
            PassPhrase = Properties.Settings.Default.savePassword;
            InitVector = "qhdE4Fhy$?d><hRw";
            SaltValue = "DF544D%%#ssdsf#@";

            _mainWindow = (MainWindow)window;

            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _mainWindow.buttonAdd.Click += ButtonAdd_Click;
            _mainWindow.buttonSave.Click += ButtonSave_Click;
            // Создание привязки
            CommandBinding bind = new CommandBinding(ApplicationCommands.New);

            // Присоединение обработчика событий
            bind.Executed += Bind_Executed;

            // Регистрация привязки
            //this.CommandBindings.Add(bind);

            StartVerifyUser(window);
            Loading();
        }

        private void Bind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.dataGrid.Items.Refresh();
            FileOperation fileOperation = new FileOperation();
            fileOperation.SaveFile("keys.pdk", _data, this);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            _dataString = new Data("", new Crypto().Encrypt("", new DecryptPass(PassPhrase, this).Decrypt(), SaltValue, InitVector), "", this);
            _data.Add(_dataString);
            _mainWindow.dataGrid.Items.Refresh();
        }

        private void Loading()
        {
            FileOperation fileOperation = new FileOperation();

            if (!fileOperation.CheckFileExists("keys.pdk"))
            {
                _dataString = new Data("Test", new Crypto().Encrypt("test", new DecryptPass(PassPhrase, this).Decrypt(), SaltValue, InitVector), "Эту запись можно удалить.", this);
                _data.Add(_dataString);

                fileOperation.CreateFile("keys.pdk");
                fileOperation.SaveFile("keys.pdk", _data, this);
            }
            
            _data = fileOperation.OpenFile("keys.pdk", this);
            _mainWindow.dataGrid.ItemsSource = _data;
            _mainWindow.dataGrid.Items.Refresh();

            
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _mainWindow.LabelInfo.Content = "Буфер обмена будет очищен через " + $"{(int)(2000 - _mainWindow.progressBar.Value) / 100}";
            if ((_mainWindow.progressBar.Value += 1) == 2000)
            {
                _mainWindow.LabelInfo.Visibility = Visibility.Hidden;
                _dispatcherTimer.Stop();
                _mainWindow.progressBar.Value = 0;
                Clipboard.Clear();
            }
        }

        private void StartVerifyUser(Window mainWindow)
        {
            mainWindow.Effect = new BlurEffect();
            _windowVerify = new WindowVerify();
            
            _windowVerify.passwordBox.Focus();

            _windowVerify.Closed += WindowVerify_Closed;
            _windowVerify.buttonEnter.Click += ButtonEnter_Click;
            _windowVerify.KeyUp += _windowVerify_KeyUp;

            if (Properties.Settings.Default.savePassword == "")
            {
                _windowVerify.buttonEnter.Content = "Сохранить";
                WindowFirstEnter windowFirstEnter = new WindowFirstEnter();
                windowFirstEnter.Owner = mainWindow;
                windowFirstEnter.ShowDialog();
            }

            _windowVerify.Owner = mainWindow;
            _windowVerify.ShowDialog();
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (_windowVerify.buttonEnter.Content.ToString() == "Сохранить")
            {
                Crypto crypto = new Crypto();

                Properties.Settings.Default.savePassword = crypto.Encrypt(_windowVerify.passwordBox.Password, "Dfe4%^GteE4@sw!21dssdfE", SaltValue, InitVector);
                Properties.Settings.Default.Save();

                PassPhrase = Properties.Settings.Default.savePassword;
            }

            if (new DecryptPass(PassPhrase, this).Decrypt() == _windowVerify.passwordBox.Password)
            {
                _verify = true;
                _mainWindow.Effect = null;
                _windowVerify.Close();
            }
            else
            {
                _verify = false;
                MessageBox.Show("Неправильный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void _windowVerify_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ButtonEnter_Click(sender, e);
        }

        private void WindowVerify_Closed(object sender, EventArgs e)
        {
            if (!_verify)
                _mainWindow.Close();
        }
    }
}
