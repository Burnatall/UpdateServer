using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CheckSumTerminal.IView
{
    public interface IUpdateWindow
    {
        string[] Files { get; set; }
        DataGrid CurrentGrid { get; set; }
        DataGrid NewFilesGrid { get; set; }
        TextBox TextBoxCurrent { get; set; }
        TextBox TextBoxNew { get; set; }
        Dispatcher Dispatcher { get; set; }

        event EventHandler LoadData;
        event EventHandler UpdateData;
        event DragEventHandler DragNewData;
        event EventHandler<OpenFileDialog> AddopenFileDialogHandler;
        event EventHandler DeleteData;
        event EventHandler DeleteDataClear;

        void InitializeComponent();
    }
}