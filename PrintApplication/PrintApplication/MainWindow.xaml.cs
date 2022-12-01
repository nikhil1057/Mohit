using PrintApplication.Model;
using PrintApplication.ViewModel;
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
using PrintApplication.ViewModel;

namespace PrintApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PrintViewModel _model;
    
        public PrintViewModel Model
        {
            get => _model;
            set
            {
                //this.myDataGrid.CommitEdit();
                _model = value;
                _model.CloseWindowAction = () =>
                {
                    this.Close();
                };
                // girdContext.DataContext = _model;
                DataContext = _model;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Model = new PrintViewModel();
           // Model = new ViewProjectVM(dirModel, selectedProject);
        }
    }
}
