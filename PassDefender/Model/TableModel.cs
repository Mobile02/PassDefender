﻿using GalaSoft.MvvmLight;

namespace PassDefender
{
    public class TableModel : ViewModelBase
    {
        private string _info;
        private string _login;
        private string _password;
        private bool _cryptOnLostFocus;


        public TableModel(string info, string login, string password)
        {
            Info = info;
            Login = login;
            Password = password;
        }

        public string Info
        {
            get { return _info; }
            set 
            { 
                _info = value;
                RaisePropertyChanged("Info");
            }
        }

        public string Login
        {
            get { return _login; }
            set 
            {
                _login = value;
                RaisePropertyChanged("Login");
            }
        }
        
        public string Password
        {
            get { return _password; }
            set 
            {
                if (_cryptOnLostFocus)
                {
                    CryptoModel cryptoModel = new CryptoModel();
                    _password = new Crypto().Encrypt(value, cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector);
                    RaisePropertyChanged("Password");
                }
                else
                {
                    _password = value;
                    _cryptOnLostFocus = true;
                }
            }
        }
    }
}
