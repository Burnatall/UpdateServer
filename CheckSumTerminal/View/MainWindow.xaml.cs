
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

        public event EventHandler AddTableEvent;
        public event EventHandler DelTableEvent;
        public event EventHandler UpdateEvent;
        public event EventHandler ShowVersionEvent;
        public event EventHandler Load;

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
            Bindings();
            MainWindowPresenter p = new(this, new MainModel());
        }

        private void Bindings()
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
            AddTableEvent?.Invoke(this, e);
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            DelTableEvent?.Invoke(this, e);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Load?.Invoke(this, e);
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateEvent?.Invoke(this, e);
        }

        private void ButtonVersion_Click(object sender, RoutedEventArgs e)
        {
            ShowVersionEvent?.Invoke(this, e);
        }
    }
}
