using CheckSumTerminal.IModels;
using CheckSumTerminal.IView;
using CheckSumTerminal.Models;
using CheckSumTerminal.View;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CheckSumTerminal.Presenter
{
    internal class VersionSelectPresenter
    {
        private IVersionSelectWindow _view;

        private IMainModel _model;
        private string str;
        private string pathToVersion;
        private string pathToMainClient;
        private string versionCurrent;
        private List<FileTableModel> list;

        private int current;
        private int max;

        public VersionSelectPresenter(IVersionSelectWindow view, IMainModel model)
        {
            _view = view;
            _model = model;
            _view.load += _view_load;
            _view.showChanges += _view_showChanges;
            _view.backToRevision += _view_backToRevision;
            _view.doWork += _view_doWork;
            _view.progressChanged += _view_progressChanged;
            _view.completed += _view_completed;

        }

        private void _view_completed(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            _view.ProgressBar.Visibility = Visibility.Hidden;
            //Выводим получившийся резульат
            _view.CreateDataGrid();
            _view.DataGrid.ItemsSource = list;
            _view.Window.Title = "Версия обновлена до " + str + " Полученная таблица:";
            _view.Window.Visibility = Visibility.Visible;
            _view.Window.Show();
            _view_load(sender, e);
        }

        private void _view_progressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _view.ProgressBar.Value = current;
        }

        private void _view_doWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            current = 1;
            max = 8 + list.Count;

            _view.BackgroundWorker.ReportProgress(current / max);
            //Сохраняем текущую версию
            _model.createZipAndTableByPath(pathToVersion + @"\" + versionCurrent, pathToMainClient, _model.getLastFullVersion());

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Перемещаем описание обновления в основную папку
            _model.createChangesFile(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument, File.ReadAllText(pathToVersion + @"\" + str + @"\" + Properties.Resources.ChangesDocument));

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Распаковываем выбранную версию в таблицу эталона
            _model.unzipToCurrentClient(pathToVersion + @"\" + str + @"\" + str + ".zip");

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Меняем таблицу в базе на выбранную версию
            Dictionary<string, int> updateableFiles = new Dictionary<string, int>();
            foreach (var g in list)
            {
                current++;
                _view.BackgroundWorker.ReportProgress(current / max);
                if (!updateableFiles.ContainsKey(g.name))
                    updateableFiles.Add(g.name, g.version);
                else
                    updateableFiles[g.name] = g.version;
            }
            current++;
            _view.BackgroundWorker.ReportProgress(current / max);
            _model.convertFilesToTableInBase(pathToMainClient, updateableFiles);

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Добавляем во все поздние версии пометку что они идут от узла к которому мы вернулись
            _model.addParentVersionToBase(_model.getVersionByNumber(str));

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Делаем id страой версии максимальным, что будет обозначать что именно на этой версии мы сейчас находимся
            _model.increaseIdOfVersion(str);

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Создаем csv таблицу из базы 
            _model.createTable(Environment.CurrentDirectory + @"\" + Properties.Resources.CSVTableName);

            current = max;
            _view.BackgroundWorker.ReportProgress(current / max);

        }

        private void _view_backToRevision(object sender, EventArgs e)
        {
            if (_view.VersionDataG.SelectedItem == null)
            {
                MessageBox.Show("Версия не выбрана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if(((VersionModelPretty)_view.VersionDataG.SelectedItem).Версия == _model.getLastFullVersion())
            {
                MessageBox.Show("Нельзя вернуться к текущей версии", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            str = ((VersionModelPretty)_view.VersionDataG.SelectedItem).Версия;
            pathToVersion = Environment.CurrentDirectory + @"\" + Properties.Resources.VersionFolderName;
            pathToMainClient = Environment.CurrentDirectory + @"\" + Properties.Resources.ClientFolderName;
            versionCurrent = _model.getLastFullVersion();

            //Получаем список файлов из таблицы версии
            list = _model.getListFilesFromTable(pathToVersion + @"\" + str + @"\" + str + ".csv");

            if (list == null)
            {
                MessageBox.Show(_model.ErrorInfo, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            MessageBoxResult b = MessageBox.Show("Текущая версия будет заменена на выбранную, продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (b == MessageBoxResult.Yes)
            {
                _view.ProgressBar.Visibility = Visibility.Visible;
                _view.BackgroundWorker.RunWorkerAsync(_model);
            }
        }

        private void _view_showChanges(object sender, EventArgs e)
        {
            IShowChangesWindow sw = new ShowChangesWindow(_model);
            sw.Show();
        }

        private void _view_load(object sender, EventArgs e)
        {
            _view.VersionDataG.ItemsSource = _model.getVersionModels();
        }
    }
}
