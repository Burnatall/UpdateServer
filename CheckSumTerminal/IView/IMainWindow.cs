using CheckSumTerminal.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CheckSumTerminal.IView
{
    public interface IMainWindow
    {
        ComboBox DirectoryComboBox { get; set; }
        MessageBoxResult MessageBox { get; set; }
        Window Window { get; set; }
        DataGrid DataGrid { get; set; }
        ComboBox FileComboBox { get; set; }
        TextBox VersionTextBox { get; set; }

        event EventHandler AddTableEvent;
        event EventHandler UpdateEvent;
        event EventHandler ShowVersionEvent;
        event EventHandler Load;

        void Initalisers();
    }
}