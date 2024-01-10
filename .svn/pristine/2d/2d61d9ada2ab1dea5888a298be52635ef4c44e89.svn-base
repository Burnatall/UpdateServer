
using CheckSumTerminal.Context;
using CheckSumTerminal.IView;
using CheckSumTerminal.Models;
using CheckSumTerminal.Presenter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace CheckSumTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {

        public event EventHandler addTableEvent;
        public event EventHandler delTableEvent;
        public event EventHandler updateEvent;
        public event EventHandler showVersionEvent;
        public event EventHandler load;

        public ComboBox DirectoryComboBox { get; set; }
        public ComboBox FileComboBox { get; set; }
        public TextBox VersionTextBox { get; set; }
        public MessageBoxResult MessageBox { get; set; }
        public Window Window { get; set; }  
        public DataGrid DataGrid { get; set; }  

        public MainWindow()
        {
            InitializeComponent();
            //Initalisers();
            bindings();
            MainWindowPresenter p = new MainWindowPresenter(this, new MainModel());
        }

        private void bindings()
        {
            DirectoryComboBox = DirectoryName;
            FileComboBox = VersionsFolderName;
            VersionTextBox = NewVersionTextBox;
        }

        public void Initalisers()
        {
            Window = new Window()
            {
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.Width,
                WindowStyle = WindowStyle.ToolWindow
            };
            DataGrid = new DataGrid()
            {         
            };
            Window.Content = DataGrid;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var handler = addTableEvent;
            if (handler != null) handler(this, e);
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var handler = delTableEvent;
            if (handler != null) handler(this, e);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var handler = load;
            if (handler != null) handler(this, e);
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            var handler = updateEvent;
            if (handler != null) handler(this, e);
        }

        private void ButtonVersion_Click(object sender, RoutedEventArgs e)
        {
            var handler = showVersionEvent;
            if (handler != null) handler(this, e);
        }
    }
}
