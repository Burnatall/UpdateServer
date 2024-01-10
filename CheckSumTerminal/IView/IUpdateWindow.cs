using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CheckSumTerminal.IView
{
    public interface IUpdateWindow
    {
        string[] _files { get; set; }
        DataGrid CurrentGrid { get; set; }
        DataGrid NewFilesGrid { get; set; }
        TextBox TextBoxCurrent { get; set; }
        TextBox TextBoxNew { get; set; }
        Dispatcher dispatcher { get; set; }

        event EventHandler loadData;
        event EventHandler updateData;
        event DragEventHandler dragNewData;
        event EventHandler<OpenFileDialog> addopenFileDialogHandler;
        event EventHandler deleteData;
        event EventHandler deleteDataClear;

        void InitializeComponent();
    }
}