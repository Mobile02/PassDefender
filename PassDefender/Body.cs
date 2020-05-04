using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace PassDefender
{
    class Body
    {
        private Window _formPassDefender;
        WindowVerify _windowVerify;
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

            _formPassDefender = window;
            StartVerifyUser(window);
        }

        private void StartVerifyUser(Window formPassDefender)
        {
            formPassDefender.Effect = new BlurEffect();
            _windowVerify = new WindowVerify();

            _windowVerify.passwordBox.Focus();

            _windowVerify.Closed += WindowVerify_Closed;
            _windowVerify.buttonEnter.Click += ButtonEnter_Click;
            _windowVerify.KeyUp += _windowVerify_KeyUp;

            if (Properties.Settings.Default.savePassword == "")
            {
                _windowVerify.buttonEnter.Content = "Сохранить";
                WindowFirstEnter windowFirstEnter = new WindowFirstEnter();
                windowFirstEnter.Owner = formPassDefender;
                windowFirstEnter.ShowDialog();
            }

            _windowVerify.Owner = formPassDefender;
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
                _formPassDefender.Effect = null;
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
                _formPassDefender.Close();
        }
    }
}
