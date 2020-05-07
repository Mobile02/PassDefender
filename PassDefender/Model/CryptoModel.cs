using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassDefender
{
    public class CryptoModel
    {
        private string _passPhrase;

        public string PassPhrase
        {
            get { return _passPhrase; }
            set 
            {
                _passPhrase = new Crypto().Decrypt(value, "Dfe4%^GteE4@sw!21dssdfE", SaltValue, InitVector);
            }
        }

        public string SaltValue { get; set; }
        public string InitVector { get; set; }

        public CryptoModel()
        {
            SaltValue = "DF544D%%#ssdsf#@";
            InitVector = "qhdE4Fhy$?d><hRw";
            PassPhrase = Properties.Settings.Default.savePassword;
        }

        public string EncryptPassPhrase(string passPhrase)
        {
            return new Crypto().Encrypt(passPhrase, "Dfe4%^GteE4@sw!21dssdfE", SaltValue, InitVector);
        }
    }
}
