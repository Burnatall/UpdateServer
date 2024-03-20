using CheckSumTerminal.IModels;
using CheckSumTerminal.IView;
using CheckSumTerminal.Models;
using CheckSumTerminal.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CheckSumTerminal.Presenter
{
    internal class MainWindowPresenter
    {
        private IMainWindow _view;

        private IMainModel _model;

        List<FileTableModel> fileTableModels;

        public MainWindowPresenter(IMainWindow view, IMainModel model)
        {
            _view = view;
            _model = model;
            _view.AddTableEvent += BeginUpdate;
            _view.UpdateEvent += Update;
            _view.ShowVersionEvent += View_showVersionEvent;
            _view.Load += View_load;
        }

        private void View_load(object sender, EventArgs e)
        {
            _view.VersionTextBox.Text = _model.GetLastFullVersion();
        }

        private void View_showVersionEvent(object sender, EventArgs e)
        {
            VersionSelectWindow versionSelectWindow = new(_model);
            versionSelectWindow.Show();
            View_load(sender, e);
        }

        private void BeginUpdate(object sender, EventArgs e)
        {
            if (!_model.GetListFiles().Any())
            {
                var list = _model.AddVersion(_view.DirectoryComboBox.SelectedItem.ToString(), _view.FileComboBox.SelectedItem.ToString(), _view.VersionTextBox.Text, null, null, null,null);
                if (list == null)
                {
                    MessageBox.Show(_model.ErrorInfo, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _view.Initalisers();
                fileTableModels = new List<FileTableModel>(list);
                _view.DataGrid.ItemsSource = list;
                _view.Window.Title = "Таблица версии " + _view.VersionTextBox.Text;
                _view.Window.Visibility = Visibility.Visible;
                _view.Window.ShowDialog();
            }
        }
        private void Update(object sender, EventArgs e)
        {
            UpdateWindow w = new();
            w.ShowDialog();
            View_load(sender, e);
        }
    }
}
