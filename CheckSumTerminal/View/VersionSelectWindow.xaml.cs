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
        public event EventHandler backToRevision;
        public event EventHandler showChanges;
        public event EventHandler<RoutedEventArgs> load;
        public event EventHandler<RunWorkerCompletedEventArgs> completed;
        public event EventHandler<DoWorkEventArgs> doWork;
        public event EventHandler<ProgressChangedEventArgs> progressChanged;

        public DataGrid VersionDataG { get; set; }
        public Window Window { get; set; }
        public DataGrid DataGrid { get; set; }
        public BackgroundWorker BackgroundWorker { get; set; }
        public ProgressBar ProgressBar { get; set; }

        public VersionSelectWindow(IMainModel model)
        {
            InitializeComponent();
            Initalisers();
            VersionSelectPresenter vp = new VersionSelectPresenter(this, model);
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
            var handler = backToRevision;
            if (handler != null) handler(this, e);
        }

        private void ShowChangesButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = showChanges;
            if (handler != null) handler(this, e);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var handler = load;
            if (handler != null) handler(this, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //todo: доделать backgroundWorker
        private void BackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            var handler = completed;
            if (handler != null) handler(this, e);
        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var handler = doWork;
            if (handler != null) handler(this, e);
        }

        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            var handler = progressChanged;
            if (handler != null) handler(this, e);
        }
    }
}
