using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для ShowChangesWindow.xaml
    /// </summary>
    public partial class ShowChangesWindow : Window, IShowChangesWindow
    {
        public DataGrid Prev { get; set; }
        public DataGrid Current { get; set; }
        public TextBlock PrevText { get; set; }
        public TextBlock CurrentText { get; set; }
        public ComboBox VersionComboBox { get; set; }


        public event EventHandler ComboChange;

        public event EventHandler Load;

        public event EventHandler ShowChanges;

        public ShowChangesWindow(IMainModel model)
        {
            InitializeComponent();
            Bindings();
            ShowChangesPresenter sp = new(this, model);
        }

        private void Bindings()
        {
            Prev = previousDataGrid;
            Current = CurrentDataGrid;
            PrevText = PreviousTextBlock;
            CurrentText = CurrentTextBlock;
            VersionComboBox = VersionSelector;
        }

        public void ShowTextBox(string content)
        {
            Window w = new()
            {
                ResizeMode = ResizeMode.NoResize,
                
                WindowStyle = WindowStyle.ToolWindow
            };
            RichTextBox rtb = new()
            {
                FontSize = 20,
                IsReadOnly = true,
            };
            rtb.Document.Blocks.Add(new Paragraph(new Run(content)));
            w.Content = rtb;
            w.Show();
        }

        private void VersionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboChange?.Invoke(this, e);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Load?.Invoke(this, e);
        }

        private void ShowChangesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowChanges?.Invoke(this, e);
        }
    }
}
