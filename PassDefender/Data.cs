using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassDefender
{
    class Data
    {
        private string _login;
        private string _password;
        private string _info;
        Body _body;

        public Data(string login, string password, string info, Body body)
        {
            this._login = login;
            this._password = password;
            this._info = info;
            this._body = body;
        }

        public string Login { get { return this._login; } set { this._login = value; } }
        public string Info { get { return this._info; } set { this._info = value; } }
        public string Password
        {
            get { return this._password; }
            set 
            {
                this._password = new Crypto().Encrypt(value, new DecryptPass(_body.PassPhrase, _body).Decrypt(), _body.SaltValue, _body.InitVector);
            } 
        }
    }
}
