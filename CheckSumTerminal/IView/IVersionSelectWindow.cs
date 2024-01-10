using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CheckSumTerminal.IView
{
    public interface IVersionSelectWindow
    {
        DataGrid VersionDataG { get; set; }
        Window Window { get; set; }
        DataGrid DataGrid { get; set; }
        BackgroundWorker BackgroundWorker { get; set; }
        ProgressBar ProgressBar { get; set; }

        event EventHandler backToRevision;
        event EventHandler<RoutedEventArgs> load;
        event EventHandler showChanges;
        event EventHandler<RunWorkerCompletedEventArgs> completed;
        event EventHandler<DoWorkEventArgs> doWork;
        event EventHandler<ProgressChangedEventArgs> progressChanged;

        void CreateDataGrid();
    }
}