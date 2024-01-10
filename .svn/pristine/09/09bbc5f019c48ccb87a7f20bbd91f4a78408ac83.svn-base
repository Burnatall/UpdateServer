using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using CheckSumTerminal.Models;
using CheckSumTerminal.Presenter;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CheckSumTerminal.View
{
    /// <summary>
    /// Логика взаимодействия для CreateDescriptionWindow.xaml
    /// </summary>
    public partial class CreateDescriptionWindow : Window, ICreateDescriptionWindow
    {
        public string NumberOfVersion { get; set; }
        public List<string> FileNames { get; set; }

        public Dictionary<string, int> FilesToUpdate { get; set; }
        public Dictionary<string, int> FilesToDelete { get; set; }


        public BackgroundWorker BackgroundWorker { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public RichTextBox RichTextBox { get; set; }
        public TextBlock WhatHppend { get; set; }

        public event EventHandler<DoWorkEventArgs> DoWork;
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public event EventHandler Start;
        public event EventHandler Complete;

        public CreateDescriptionWindow(string numberOfVersion, List<string> fileNames, Dictionary<string, int> filsToUpdate, Dictionary<string, int> filesToDelete,IMainModel model)
        {
            InitializeComponent();
            Bindings();
            NumberOfVersion = numberOfVersion;
            FileNames = fileNames;
            FilesToUpdate = filsToUpdate;
            FilesToDelete = filesToDelete;
            CreateDescriptionPresenter p = new CreateDescriptionPresenter(this, model);
        }

        private void Bindings()
        {
            BackgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            ProgressBar = ProgressBarMain;
            RichTextBox = MainTextBox;
            WhatHppend = WhatHappendTextBlock;
        }

        public void BlockControls()
        {
            MainTextBox.IsEnabled = false;
            ButtonOk.IsEnabled = false;
            ButtonCancel.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var handler = Start;
            if (handler != null) handler(this, e);
        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var handler = DoWork;
            if (handler != null) handler(this, e);

        }

        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            var handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var handler = Complete;
            if (handler != null) handler(this, e);
        }
    }
}
