﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
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
        private DispatcherTimer _dispatcherTimer;
        private string filePath;
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

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            _dispatcherTimer.Tick += DispatcherTimer_Tick;

            Application.Current.MainWindow.Loaded += MainWindow_Loaded;
            
            filePath = Environment.CurrentDirectory + "\\keys.pdk";
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            new WindowVerifyViewModel().Check();
            LoadData();
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
            
            if (_dispatcherTimer.IsEnabled)
            {
                ProgressBarValue = 1500;
                SetIcons(false);
            }
            else
            {
                _dispatcherTimer.Start();
                SetIcons(true);
            }
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
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                try { filePath = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0]; }
                catch { filePath = Environment.CurrentDirectory + "\\keys.pdk"; }
            }

            new FileOperation().SaveFile(filePath, DataCollections);
        }

        private void LoadData()
        {
            FileOperation fileOperation = new FileOperation();

            if (!fileOperation.CheckFileExists(filePath))
            {
                DataCollections.Add(new TableModel("", "", ""));

                fileOperation.CreateFile(filePath);
                fileOperation.SaveFile(filePath, DataCollections);
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
            LabelInfo = "Буфер обмена будет очищен через " + $"{(1500 - ProgressBarValue) / 100}";
            if ((ProgressBarValue += 1) >= 1500)
            {

                _dispatcherTimer.Stop();
                ProgressBarValue = 0;
                LabelInfo = "";

                SetIcons(false);
                Clipboard.Clear();
            }
        }

        private void SetIcons(bool imageButton)
        {
            foreach (var tmp in _dataCollections)
            {
                if (imageButton)
                {
                    tmp.ImageButton = true;
                    tmp.ToolTip = "Очистить буфер обмена";
                }
                else
                {
                    tmp.ImageButton = false;
                    tmp.ToolTip = "Скопировать пароль в буфер обмена";
                }
            }
        }
    }
}
