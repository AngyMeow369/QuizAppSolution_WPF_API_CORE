using QuizApp.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuizApp.WPF
{
 
    /// Interaction logic for MainWindow.xaml
    
    public partial class MainWindow : Window
    {
        public MainWindow(string username, string token)
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(username, token);
        }
    }
}
