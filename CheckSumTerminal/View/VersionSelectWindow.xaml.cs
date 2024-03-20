using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CheckSumTerminal.IModels;
using CheckSumTerminal.IView;
using CheckSumTerminal.Presenter;

namespace CheckSumTerminal.View
{
    /// <summary>
    /// Логика взаимодействия для VersionSelectWindow.xaml
    /// </summary>
    public partial class VersionSelectWindow : Window, IVersionSelectWindow
    {
        public event EventHandler BackToRevision;
        public event EventHandler ShowChanges;
        public event EventHandler<RoutedEventArgs> Load;
        public event EventHandler<RunWorkerCompletedEventArgs> Completed;
        public event EventHandler<DoWorkEventArgs> DoWork;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        public DataGrid VersionDataG { get; set; }
        public Window Window { get; set; }
        public DataGrid DataGrid { get; set; }
        public BackgroundWorker BackgroundWorker { get; set; }
        public ProgressBar ProgressBar { get; set; }

        public VersionSelectWindow(IMainModel model)
        {
            InitializeComponent();
            Initalisers();
            VersionSelectPresenter vp = new(this, model);
        }

        private void Initalisers()
        {
            BackgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            VersionDataG = VersionDataGrid;
            ProgressBar = Progress;
        }

        public void CreateDataGrid()
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

        private void BackToRevisionButton_Click(object sender, RoutedEventArgs e)
        {
            BackToRevision?.Invoke(this, e);
        }

        private void ShowChangesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowChanges?.Invoke(this, e);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Load?.Invoke(this, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //todo: доделать backgroundWorker
        private void BackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Completed?.Invoke(this, e);
        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DoWork?.Invoke(this, e);
        }

        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }
    }
}
