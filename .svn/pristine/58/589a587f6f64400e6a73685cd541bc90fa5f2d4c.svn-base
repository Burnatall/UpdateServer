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
        public DataGrid prev { get; set; }
        public DataGrid current { get; set; }
        public TextBlock prevText { get; set; }
        public TextBlock currentText { get; set; }
        public ComboBox versionComboBox { get; set; }


        public event EventHandler comboChange;

        public event EventHandler load;

        public event EventHandler showChanges;

        public ShowChangesWindow(IMainModel model)
        {
            InitializeComponent();
            Bindings();
            ShowChangesPresenter sp = new ShowChangesPresenter(this, model);
        }

        private void Bindings()
        {
            prev = previousDataGrid;
            current = CurrentDataGrid;
            prevText = PreviousTextBlock;
            currentText = CurrentTextBlock;
            versionComboBox = VersionSelector;
        }

        public void ShowTextBox(string content)
        {
            Window w = new Window()
            {
                ResizeMode = ResizeMode.NoResize,
                
                WindowStyle = WindowStyle.ToolWindow
            };
            RichTextBox rtb = new RichTextBox()
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
            var handler = comboChange;
            if (handler != null) handler(this, e);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var handler = load;
            if (handler != null) handler(this, e);
        }

        private void ShowChangesButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = showChanges;
            if (handler != null) handler(this, e);
        }
    }
}
