using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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
    class WindowVerifyViewModel : ViewModelBase
    {
        private Window _ownerWindow;
        private WindowVerify _windowVerify;
        private ICommand _Command_Button_Enter;
        private ICommand _Command_Button_Exit;

        private string _password = "";
        private string _buttonContent;

        public WindowVerifyViewModel()
        {
            _ownerWindow = Application.Current.MainWindow;
            if (Properties.Settings.Default.savePassword == "")
                ButtonContent = "Сохранить";
            else
                ButtonContent = "Вход";
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        public string ButtonContent
        {
            get
            { return _buttonContent; }
            set
            {
                _buttonContent = value;
                RaisePropertyChanged("ButtonContent");
            }
        }

        public void Check()
        {
            _windowVerify = new WindowVerify();
            _ownerWindow.Effect = new BlurEffect();
            _windowVerify.Owner = _ownerWindow;

            if (Properties.Settings.Default.savePassword == "")
            {
                WindowFirstEnter windowFirstEnter = new WindowFirstEnter();
                windowFirstEnter.Owner = _ownerWindow;
                windowFirstEnter.ShowDialog();
            }
            _windowVerify.ShowDialog();
        }


        public ICommand Command_Button_Enter
        {
            get
            {
                if (_Command_Button_Enter == null)
                {
                    return _Command_Button_Enter = new RelayCommand(execute: ButtonEnter);
                }
                return _Command_Button_Enter;
            }
        }

        public ICommand Command_Button_Exit
        {
            get
            {
                if (_Command_Button_Exit == null)
                {
                    return _Command_Button_Exit = new RelayCommand(execute: ButtonExit);
                }
                return _Command_Button_Exit;
            }
        }

        public void ButtonEnter()
        {
            CryptoModel cryptoModel = new CryptoModel();

            if (ButtonContent == "Сохранить")
            {
                Properties.Settings.Default.savePassword = cryptoModel.EncryptPassPhrase(Password);
                Properties.Settings.Default.Save();

            }
            else
            {
                Properties.Settings.Default.savePassword = cryptoModel.EncryptPassPhrase(Password);
                Properties.Settings.Default.Save();
            }

            _ownerWindow.Effect = null;
        }

        public void ButtonExit()
        {
            Application.Current.Shutdown();
        }
    }
}
