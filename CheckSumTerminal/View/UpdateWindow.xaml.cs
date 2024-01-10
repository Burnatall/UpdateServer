﻿using System;
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

        public event EventHandler loadData;
        public event EventHandler updateData;
        public event EventHandler<OpenFileDialog> addopenFileDialogHandler;
        public event DragEventHandler dragNewData;
        public event EventHandler deleteData;
        public event EventHandler deleteDataClear;

        public string[] _files { get; set; }

        public DataGrid CurrentGrid { get; set; }
        public DataGrid NewFilesGrid { get; set; }

        public TextBox TextBoxCurrent { get; set; }
        public TextBox TextBoxNew { get; set; }

        public Dispatcher dispatcher { get; set; }


        public UpdateWindow()
        {
            InitializeComponent();
            bindings();
            UpdateWindowPresenter p = new UpdateWindowPresenter(this,new MainModel());
        }

        private void bindings()
        {
            CurrentGrid = CurrentFilesDataGrid;
            NewFilesGrid = NewFilesDataGrid;
            TextBoxCurrent = TextBoxBefore;
            TextBoxNew = TextBoxAfter;
            dispatcher = Dispatcher;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var handler = loadData;
            if (handler != null) handler(this, e);
        }

        private void NewFilesDataGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                var handler = dragNewData;
                if (handler != null) handler(this, e);
                GridTextBlock.Visibility = Visibility.Collapsed;
                // Note that you can have more than one file.
                //_files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            var handler = updateData;
            if (handler != null) handler(this, e);
            Close();
        }

        private void ButtonData_Click(object sender, RoutedEventArgs e)
        {
            GridTextBlock.Visibility = Visibility.Collapsed;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                var handler = addopenFileDialogHandler;
                if (handler != null) handler(this, openFileDialog);
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            GridTextBlock.Visibility = Visibility.Collapsed;
            var handler = deleteData;
            if (handler != null) handler(this, e);
        }

        private void ButtonDeleteClean_Click(object sender, RoutedEventArgs e)
        {
            GridTextBlock.Visibility = Visibility.Visible;
            var handler = deleteDataClear;
            if (handler != null) handler(this, e);
        }
    }
}