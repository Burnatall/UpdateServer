using CheckSumTerminal.IModels;
using CheckSumTerminal.IView;
using CheckSumTerminal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CheckSumTerminal.Presenter
{
    internal class ShowChangesPresenter
    {
        private IShowChangesWindow _view;

        private IMainModel _model;

        public ShowChangesPresenter(IShowChangesWindow view, IMainModel model)
        {
            _view = view;
            _model = model;
            _view.comboChange += _view_comboChange;
            _view.load += _view_load;
            _view.showChanges += _view_showChanges;
        }

        private void _view_showChanges(object sender, EventArgs e)
        {
            var vrs = _view.versionComboBox.SelectedItem.ToString();
            if (_model.GetLastFullVersion() != vrs)
                _view.ShowTextBox(File.ReadAllText(Properties.Resources.VersionFolderName + @"\" + _view.versionComboBox.SelectedItem.ToString() + @"\" + Properties.Resources.ChangesDocument));
            else
                _view.ShowTextBox(File.ReadAllText(Properties.Resources.ChangesDocument));
        }

        private void _view_comboChange(object sender, EventArgs e)
        {
            string vers = _view.versionComboBox.SelectedItem.ToString();
            
            string versPrev = _model.GetChosenFullVersion( _model.GetPreviousVersionByNumber(vers));
            var prevList = _model.GetListFilesFromTable(Properties.Resources.VersionFolderName + @"\" + versPrev + @"\" + versPrev + ".csv");
            List<FileTableModel> curList;
            if (_view.versionComboBox.SelectedIndex != _view.versionComboBox.Items.Count - 1)
                curList = _model.GetListFilesFromTable(Properties.Resources.VersionFolderName + @"\" + vers + @"\" + vers + ".csv");
            else
                curList = _model.GetListFilesFromTable(Environment.CurrentDirectory + @"\" + Properties.Resources.CSVTableName);


            List<long> ids = new List<long>();
            //Создаем список измененных id

            //Если мы не дошли до самой первой версии
            //todo: Не оч работает?
            if (prevList != null)
            {
                if (curList.Count >= prevList.Count)
                {
                    for (int i = 0; i < curList.Count; i++)
                    {
                        if (i >= prevList.Count || curList[i].version != prevList[i].version|| !prevList.Select(x=>x.name).Contains( curList[i].name))
                            ids.Add(curList[i].id);
                    }
                }
                else
                {
                    for (int i = 0; i < prevList.Count; i++)
                    {
                        if (i >= curList.Count || curList[i].version != prevList[i].version || !curList.Select(x => x.name).Contains(prevList[i].name))
                            ids.Add(prevList[i].id);
                    }
                }
                _view.prev.ItemsSource = prevList.Where(x => ids.Contains(x.id));
                _view.prevText.Text = versPrev;
            }
            else if(curList!=null) //Если это первая версия и предшественника не существует
            {
                ids = curList.Select(x => x.id).ToList();
                _view.prev.ItemsSource = null;
                _view.prevText.Text = "Отсутсвует";
            }
            else
            {
                MessageBox.Show("Версия единственная, чтобы увидеть изменения нужно ее обновить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _view.Close();
                return;
            }

            _view.current.ItemsSource = curList.Where(x => ids.Contains(x.id));
            _view.currentText.Text = vers;
        }

        private void _view_load(object sender, EventArgs e)
        {
            var lst = _model.GetVersionModels().Select(x=>x.Версия);
            _view.versionComboBox.ItemsSource = lst;
            _view.versionComboBox.SelectedIndex = _view.versionComboBox.Items.Count - 1;
        }
    }
}
