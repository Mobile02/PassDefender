using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassDefender
{
    class DecryptPass
    {
        private string _passPhrase;
        private Body _body;

        public DecryptPass(string passPhrase, Body body)
        {
            _body = body;
            _passPhrase = passPhrase;
        }

        public string Decrypt()
        {
            return new Crypto().Decrypt(_passPhrase, "Dfe4%^GteE4@sw!21dssdfE", _body.SaltValue, _body.InitVector);
        }
    }
}
