using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
