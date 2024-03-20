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

        event EventHandler BackToRevision;
        event EventHandler<RoutedEventArgs> Load;
        event EventHandler ShowChanges;
        event EventHandler<RunWorkerCompletedEventArgs> Completed;
        event EventHandler<DoWorkEventArgs> DoWork;
        event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        void CreateDataGrid();
    }
}