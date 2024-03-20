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
            _view.Load += View_load;
            _view.ShowChanges += View_showChanges;
            _view.BackToRevision += View_backToRevision;
            _view.DoWork += View_doWork;
            _view.ProgressChanged += View_progressChanged;
            _view.Completed += View_completed;

        }

        private void View_completed(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            _view.ProgressBar.Visibility = Visibility.Hidden;
            //Выводим получившийся резульат
            _view.CreateDataGrid();
            _view.DataGrid.ItemsSource = list;
            _view.Window.Title = "Версия обновлена до " + str + " Полученная таблица:";
            _view.Window.Visibility = Visibility.Visible;
            _view.Window.Show();
            View_load(sender, e);
        }

        private void View_progressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _view.ProgressBar.Value = current;
        }

        private void View_doWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            current = 1;
            max = 8 + list.Count;

            _view.BackgroundWorker.ReportProgress(current / max);
            //Сохраняем текущую версию
            _model.CreateZipAndTableByPath(pathToVersion + @"\" + versionCurrent, pathToMainClient, _model.GetLastFullVersion());

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Перемещаем описание обновления в основную папку
            _model.CreateChangesFile(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument, File.ReadAllText(pathToVersion + @"\" + str + @"\" + Properties.Resources.ChangesDocument));

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Распаковываем выбранную версию в таблицу эталона
            _model.UnzipToCurrentClient(pathToVersion + @"\" + str + @"\" + str + ".zip");

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Меняем таблицу в базе на выбранную версию
            Dictionary<string, int> updateableFiles = new();
            foreach (var g in list)
            {
                current++;
                _view.BackgroundWorker.ReportProgress(current / max);
                if (!updateableFiles.ContainsKey(g.Name))
                    updateableFiles.Add(g.Name, g.Version);
                else
                    updateableFiles[g.Name] = g.Version;
            }
            current++;
            _view.BackgroundWorker.ReportProgress(current / max);
            _model.ConvertFilesToTableInBase(pathToMainClient, updateableFiles);

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Добавляем во все поздние версии пометку что они идут от узла к которому мы вернулись
            _model.AddParentVersionToBase(_model.GetVersionByNumber(str));

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Делаем id страой версии максимальным, что будет обозначать что именно на этой версии мы сейчас находимся
            _model.IncreaseIdOfVersion(str);

            current++;
            _view.BackgroundWorker.ReportProgress(current / max);

            //Создаем csv таблицу из базы 
            _model.CreateTable(Environment.CurrentDirectory + @"\" + Properties.Resources.CSVTableName);

            current = max;
            _view.BackgroundWorker.ReportProgress(current / max);

        }

        private void View_backToRevision(object sender, EventArgs e)
        {
            if (_view.VersionDataG.SelectedItem == null)
            {
                MessageBox.Show("Версия не выбрана", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if(((VersionModelPretty)_view.VersionDataG.SelectedItem).Версия == _model.GetLastFullVersion())
            {
                MessageBox.Show("Нельзя вернуться к текущей версии", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            str = ((VersionModelPretty)_view.VersionDataG.SelectedItem).Версия;
            pathToVersion = Environment.CurrentDirectory + @"\" + Properties.Resources.VersionFolderName;
            pathToMainClient = Environment.CurrentDirectory + @"\" + Properties.Resources.ClientFolderName;
            versionCurrent = _model.GetLastFullVersion();

            //Получаем список файлов из таблицы версии
            list = _model.GetListFilesFromTable(pathToVersion + @"\" + str + @"\" + str + ".csv");

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

        private void View_showChanges(object sender, EventArgs e)
        {
            IShowChangesWindow sw = new ShowChangesWindow(_model);
            sw.Show();
        }

        private void View_load(object sender, EventArgs e)
        {
            _view.VersionDataG.ItemsSource = _model.GetVersionModels();
        }
    }
}
