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
            _view.ComboChange += View_comboChange;
            _view.Load += View_load;
            _view.ShowChanges += View_showChanges;
        }

        private void View_showChanges(object sender, EventArgs e)
        {
            var vrs = _view.VersionComboBox.SelectedItem.ToString();
            if (_model.GetLastFullVersion() != vrs)
                _view.ShowTextBox(File.ReadAllText(Properties.Resources.VersionFolderName + @"\" + _view.VersionComboBox.SelectedItem.ToString() + @"\" + Properties.Resources.ChangesDocument));
            else
                _view.ShowTextBox(File.ReadAllText(Properties.Resources.ChangesDocument));
        }

        private void View_comboChange(object sender, EventArgs e)
        {
            string vers = _view.VersionComboBox.SelectedItem.ToString();
            
            string versPrev = _model.GetChosenFullVersion( _model.GetPreviousVersionByNumber(vers));
            var prevList = _model.GetListFilesFromTable(Properties.Resources.VersionFolderName + @"\" + versPrev + @"\" + versPrev + ".csv");
            List<FileTableModel> curList;
            if (_view.VersionComboBox.SelectedIndex != _view.VersionComboBox.Items.Count - 1)
                curList = _model.GetListFilesFromTable(Properties.Resources.VersionFolderName + @"\" + vers + @"\" + vers + ".csv");
            else
                curList = _model.GetListFilesFromTable(Environment.CurrentDirectory + @"\" + Properties.Resources.CSVTableName);


            List<long> ids = new();
            //Создаем список измененных id

            //Если мы не дошли до самой первой версии
            //todo: Не оч работает?
            if (prevList != null)
            {
                if (curList.Count >= prevList.Count)
                {
                    for (int i = 0; i < curList.Count; i++)
                    {
                        if (i >= prevList.Count || curList[i].Version != prevList[i].Version|| !prevList.Select(x=>x.Name).Contains( curList[i].Name))
                            ids.Add(curList[i].Id);
                    }
                }
                else
                {
                    for (int i = 0; i < prevList.Count; i++)
                    {
                        if (i >= curList.Count || curList[i].Version != prevList[i].Version || !curList.Select(x => x.Name).Contains(prevList[i].Name))
                            ids.Add(prevList[i].Id);
                    }
                }
                _view.Prev.ItemsSource = prevList.Where(x => ids.Contains(x.Id));
                _view.PrevText.Text = versPrev;
            }
            else if(curList!=null) //Если это первая версия и предшественника не существует
            {
                ids = curList.Select(x => x.Id).ToList();
                _view.Prev.ItemsSource = null;
                _view.PrevText.Text = "Отсутсвует";
            }
            else
            {
                MessageBox.Show("Версия единственная, чтобы увидеть изменения нужно ее обновить", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _view.Close();
                return;
            }

            _view.Current.ItemsSource = curList.Where(x => ids.Contains(x.Id));
            _view.CurrentText.Text = vers;
        }

        private void View_load(object sender, EventArgs e)
        {
            var lst = _model.GetVersionModels().Select(x=>x.Версия);
            _view.VersionComboBox.ItemsSource = lst;
            _view.VersionComboBox.SelectedIndex = _view.VersionComboBox.Items.Count - 1;
        }
    }
}
