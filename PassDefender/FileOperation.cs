using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassDefender
{
    class FileOperation
    {
        private Data _dataString;

        public void SaveFile(string path, List<Data> data, Body body)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            Crypto crypto = new Crypto();
            string _passPhrase = new DecryptPass(body.PassPhrase, body).Decrypt();

            for (int i = 0; i < data.Count; i++)
            {
                streamWriter.WriteLine(crypto.Encrypt(data[i].Login, _passPhrase, body.SaltValue, body.InitVector));
                streamWriter.WriteLine(crypto.Encrypt(data[i].Password, _passPhrase, body.SaltValue, body.InitVector));
                streamWriter.WriteLine(crypto.Encrypt(data[i].Info, _passPhrase, body.SaltValue, body.InitVector));
            }
            streamWriter.Close();
        }

        public List<Data> OpenFile(string path, Body body)
        {
            StreamReader streamReader = new StreamReader(path);
            List<Data> listData = new List<Data>();

            while (!streamReader.EndOfStream)
            {
                Crypto crypto = new Crypto();
                string _passPhrase = new DecryptPass(body.PassPhrase, body).Decrypt();

                _dataString = new Data
                    (
                    crypto.Decrypt(streamReader.ReadLine(), _passPhrase, body.SaltValue, body.InitVector),
                    crypto.Decrypt(streamReader.ReadLine(), _passPhrase, body.SaltValue, body.InitVector),
                    crypto.Decrypt(streamReader.ReadLine(), _passPhrase, body.SaltValue, body.InitVector),
                    body
                    );
                listData.Add(_dataString);
            }
            streamReader.Close();
            return listData;
        }

        public bool CheckFileExists(string path)
        {
            if (File.Exists(path))
                return true;
            else
                return false;
        }

        public void CreateFile(string path)
        {
            File.Create(path).Close();
        }
    }
}
