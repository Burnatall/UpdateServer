using System;
using System.Windows.Controls;

namespace CheckSumTerminal.IView
{
    public interface IShowChangesWindow
    {
        DataGrid Current { get; set; }
        TextBlock CurrentText { get; set; }
        DataGrid Prev { get; set; }
        TextBlock PrevText { get; set; }
        ComboBox VersionComboBox { get; set; }

        event EventHandler ComboChange;
        event EventHandler Load;
        event EventHandler ShowChanges;

        void InitializeComponent();

        void Show();

        void Close();
        void ShowTextBox(string content);
    }
}