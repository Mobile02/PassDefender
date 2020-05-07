using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PassDefender
{
    public class DataGridViewModel : ViewModelBase
    {
        private ObservableCollection<TableModel> _dataCollections;
        private TableModel _tableModel;
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        private int _progress;
        private string _info;

        private ICommand _Command_Copy_Login;
        private ICommand _Command_Copy_Password;
        private ICommand _Command_Generation_Password;
        private ICommand _Command_Delete_Row;
        private ICommand _Command_Add_Row;
        private ICommand _Command_Save;

        public DataGridViewModel()
        {
            DataCollections = new ObservableCollection<TableModel>();
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            Application.Current.MainWindow.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new WindowVerifyViewModel().Check();
            LoadData();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        public TableModel SelectedData
        {
            get { return _tableModel; }
            set
            {
                _tableModel = value;
                RaisePropertyChanged("SelectedData");

            }
        }

        public ObservableCollection<TableModel> DataCollections
        {
            get { return _dataCollections; }
            set
            {
                _dataCollections = value;
                RaisePropertyChanged("DataCollections");
            }
        }

        public int ProgressBarValue
        {
            get { return _progress; }
            set
            {
                _progress = value;
                RaisePropertyChanged("ProgressBarValue");
            }
        }

        public string LabelInfo
        {
            get { return _info; }
            set
            {
                _info = value;
                RaisePropertyChanged("LabelInfo");
            }
        }

        public ICommand Command_Copy_Login
        {
            get
            {
                if (_Command_Copy_Login == null)
                {
                    return _Command_Copy_Login = new RelayCommand(execute: CopyLogin);
                }
                return _Command_Copy_Login;
            }
        }

        public ICommand Command_Copy_Password
        {
            get
            {
                if (_Command_Copy_Password == null)
                {
                    return _Command_Copy_Password = new RelayCommand(execute: CopyPassword);
                }
                return _Command_Copy_Password;
            }
        }

        public ICommand Command_Generation_Password
        {
            get
            {
                if (_Command_Generation_Password == null)
                {
                    return _Command_Generation_Password = new RelayCommand(execute: GenerationPassword);
                }
                return _Command_Generation_Password;
            }
        }

        public ICommand Command_Delete_Row
        {
            get
            {
                if (_Command_Delete_Row == null)
                {
                    return _Command_Delete_Row = new RelayCommand(execute: DeleteRow);
                }
                return _Command_Delete_Row;
            }
        }

        public ICommand Command_Add_Row
        {
            get
            {
                if (_Command_Add_Row == null)
                {
                    return _Command_Add_Row = new RelayCommand(execute: AddRow);
                }
                return _Command_Add_Row;
            }
        }

        public ICommand Command_Save
        {
            get
            {
                if (_Command_Save == null)
                {
                    return _Command_Save = new RelayCommand(execute: Save);
                }
                return _Command_Save;
            }
        }

        private void CopyLogin()
        {
            Clipboard.Clear();
            Clipboard.SetText(SelectedData.Login);
        }

        private void CopyPassword()
        {
            CryptoModel cryptoModel = new CryptoModel();
            Clipboard.Clear();
            Clipboard.SetText(new Crypto().Decrypt(SelectedData.Password, cryptoModel.PassPhrase, cryptoModel.SaltValue, cryptoModel.InitVector));
            _dispatcherTimer.Start();
        }

        private void GenerationPassword()
        {
            SelectedData.Password = new Randomizer().RndString(16);
        }

        private void DeleteRow()
        {
            var result = MessageBox.Show("Удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                DataCollections.Remove(SelectedData);
        }

        private void AddRow()
        {
            DataCollections.Add(new TableModel("", "", ""));
        }

        private void Save()
        {
            string filePath = Environment.CurrentDirectory + "\\keys.pdk";

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                try { filePath = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0]; }
                catch { filePath = Environment.CurrentDirectory + "\\keys.pdk"; }
            }

            new FileOperation().SaveFile(filePath, DataCollections);
        }

        private void LoadData()
        {
            string filePath = Environment.CurrentDirectory + "\\keys.pdk";
            
            FileOperation fileOperation = new FileOperation();

            if (!fileOperation.CheckFileExists(Environment.CurrentDirectory + "\\keys.pdk"))
            {
                DataCollections.Add(new TableModel("", "", ""));

                fileOperation.CreateFile(Environment.CurrentDirectory + "\\keys.pdk");
                fileOperation.SaveFile(Environment.CurrentDirectory + "\\keys.pdk", DataCollections);
            }

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                try { filePath = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0]; }
                catch { filePath = Environment.CurrentDirectory + "\\keys.pdk"; }
            }

            DataCollections = new FileOperation().OpenFile(filePath);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            LabelInfo = "Буфер обмена будет очищен через " + $"{(2000 - ProgressBarValue) / 100}";
            if ((ProgressBarValue += 1) == 2000)
            {

                _dispatcherTimer.Stop();
                ProgressBarValue = 0;
                Clipboard.Clear();
            }
        }
    }
}
