using CheckSumTerminal.IModels;
using CheckSumTerminal.IView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CheckSumTerminal.Presenter
{
    internal class CreateDescriptionPresenter
    {
        private ICreateDescriptionWindow _view;

        private IMainModel _model;

        public CreateDescriptionPresenter(ICreateDescriptionWindow view, IMainModel model)
        {
            _view = view;
            _model = model;
            _view.Start += View_Start;
            _view.ProgressChanged += View_ProgressChanged;
            _view.DoWork += View_DoWork;
            _view.Complete += View_Complete;
        }

        private void View_Complete(object sender, EventArgs e)
        {
            MessageBox.Show("Добавление версии успешно завершено", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            _view.Close();
        }

        private void View_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            _model.AddVersion(Properties.Resources.ClientFolderName, Properties.Resources.VersionFolderName, _view.NumberOfVersion, _view.FileNames, _view.FilesToUpdate, _view.FilesToDelete,_view.BackgroundWorker);
            _model.CreateTable(Environment.CurrentDirectory + @"\" + Properties.Resources.CSVTableName);
            _model.CreateChangesFile(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument, new TextRange(_view.RichTextBox.Document.ContentStart, _view.RichTextBox.Document.ContentEnd).Text);
        }

        private void View_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _view.ProgressBar.Maximum = _model.Count;
            _view.ProgressBar.Value = _model.Iterator;
            _view.WhatHppend.Text = _model.WhatHapend;
        }

        private void View_Start(object sender, EventArgs e)
        {
            _view.WhatHppend.Text = "";
            var txt = new TextRange(_view.RichTextBox.Document.ContentStart, _view.RichTextBox.Document.ContentEnd).Text;
            if (txt.Trim() != "")
            {
                _view.BlockControls();
                _view.BackgroundWorker.RunWorkerAsync(_model);
            }
            else
                _view.WhatHppend.Text = "Заполните описание обновления!";
        }
    }
}
