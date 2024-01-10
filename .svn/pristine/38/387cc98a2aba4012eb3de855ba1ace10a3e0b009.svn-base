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

        event EventHandler addTableEvent;
        event EventHandler updateEvent;
        event EventHandler showVersionEvent;
        event EventHandler load;

        void Initalisers();
    }
}