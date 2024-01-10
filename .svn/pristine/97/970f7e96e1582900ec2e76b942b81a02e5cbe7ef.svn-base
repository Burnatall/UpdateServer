using System;
using System.Windows.Controls;

namespace CheckSumTerminal.IView
{
    public interface IShowChangesWindow
    {
        DataGrid current { get; set; }
        TextBlock currentText { get; set; }
        DataGrid prev { get; set; }
        TextBlock prevText { get; set; }
        ComboBox versionComboBox { get; set; }

        event EventHandler comboChange;
        event EventHandler load;
        event EventHandler showChanges;

        void InitializeComponent();

        void Show();

        void Close();
        void ShowTextBox(string content);
    }
}