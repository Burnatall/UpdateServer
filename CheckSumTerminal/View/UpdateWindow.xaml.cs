using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CheckSumTerminal.IView;
using CheckSumTerminal.Models;
using CheckSumTerminal.Presenter;
using Microsoft.Win32;

namespace CheckSumTerminal
{
    /// <summary>
    /// Логика взаимодействия для UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window, IUpdateWindow
    {

        public event EventHandler LoadData;
        public event EventHandler UpdateData;
        public event EventHandler<OpenFileDialog> AddopenFileDialogHandler;
        public event DragEventHandler DragNewData;
        public event EventHandler DeleteData;
        public event EventHandler DeleteDataClear;

        public string[] Files { get; set; }

        public DataGrid CurrentGrid { get; set; }
        public DataGrid NewFilesGrid { get; set; }

        public TextBox TextBoxCurrent { get; set; }
        public TextBox TextBoxNew { get; set; }

        public Dispatcher Dispatcher { get; set; }


        public UpdateWindow()
        {
            InitializeComponent();
            Bindings();
            UpdateWindowPresenter p = new(this,new MainModel());
        }

        private void Bindings()
        {
            CurrentGrid = CurrentFilesDataGrid;
            NewFilesGrid = NewFilesDataGrid;
            TextBoxCurrent = TextBoxBefore;
            TextBoxNew = TextBoxAfter;
            Dispatcher = base.Dispatcher;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData?.Invoke(this, e);
        }

        private void NewFilesDataGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                DragNewData?.Invoke(this, e);
                GridTextBlock.Visibility = Visibility.Collapsed;
                // Note that you can have more than one file.
                //_files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            UpdateData?.Invoke(this, e);
            Close();
        }

        private void ButtonData_Click(object sender, RoutedEventArgs e)
        {
            GridTextBlock.Visibility = Visibility.Collapsed;

            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                AddopenFileDialogHandler?.Invoke(this, openFileDialog);
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            GridTextBlock.Visibility = Visibility.Collapsed;
            DeleteData?.Invoke(this, e);
        }

        private void ButtonDeleteClean_Click(object sender, RoutedEventArgs e)
        {
            GridTextBlock.Visibility = Visibility.Visible;
            DeleteDataClear?.Invoke(this, e);
        }
    }
}
