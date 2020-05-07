using System.Windows;


namespace PassDefender
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            
            FormPassDefender.Left = Properties.Settings.Default.Left;
            FormPassDefender.Top = Properties.Settings.Default.Top;
            FormPassDefender.Width = Properties.Settings.Default.Width;
            FormPassDefender.Height = Properties.Settings.Default.Height;
        }

        private void FormPassDefender_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Left = FormPassDefender.Left;
            Properties.Settings.Default.Top = FormPassDefender.Top;
            Properties.Settings.Default.Width = FormPassDefender.Width;
            Properties.Settings.Default.Height = FormPassDefender.Height;
            Properties.Settings.Default.Save();
        }
    }
}
