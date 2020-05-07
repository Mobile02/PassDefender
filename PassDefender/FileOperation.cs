using System.Collections.ObjectModel;
using System.IO;

namespace PassDefender
{
    class FileOperation
    {
        public void SaveFile(string path, ObservableCollection<TableModel> dataCollections)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            Crypto crypto = new Crypto();
            CryptoModel cryptoModel = new CryptoModel();

            for (int i = 0; i < dataCollections.Count; i++)
            {
                streamWriter.WriteLine(crypto.Encrypt(dataCollections[i].Info, cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector));
                streamWriter.WriteLine(crypto.Encrypt(dataCollections[i].Login, cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector));
                streamWriter.WriteLine(crypto.Encrypt(dataCollections[i].Password, cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector));
            }
            streamWriter.Close();
        }

        public ObservableCollection<TableModel> OpenFile(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            ObservableCollection<TableModel> dataCollections = new ObservableCollection<TableModel>();

            while (!streamReader.EndOfStream)
            {
                Crypto crypto = new Crypto();
                CryptoModel cryptoModel = new CryptoModel();
                TableModel tableModel = new TableModel
                    (
                    crypto.Decrypt(streamReader.ReadLine(), cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector),
                    crypto.Decrypt(streamReader.ReadLine(), cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector),
                    crypto.Decrypt(streamReader.ReadLine(), cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector)
                    );
                dataCollections.Add(tableModel);
            }
            streamReader.Close();
            return dataCollections;
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
