using CheckSumTerminal.IModels;
using CheckSumTerminal.IView;
using CheckSumTerminal.Models;
using CheckSumTerminal.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CheckSumTerminal.Presenter
{
    internal class UpdateWindowPresenter
    {
        private IUpdateWindow _view;

        private IMainModel _model;

        List<FileTableModel> fileTableModels;

        List<string> fileNames = new();

        Dictionary<string,int> updateableFiles = new();

        Dictionary<string,int> delFiles = new();
        List<FileTableModel> deletedList = new();

        List<FileTableModel> secondGr = new();
 
        public UpdateWindowPresenter(IUpdateWindow view, IMainModel model)
        {
            _view = view;
            _model = model;
            _view.LoadData += Load_Data;
            _view.UpdateData += Update_Data;
            _view.AddopenFileDialogHandler += Add_NewData;
            _view.DragNewData += Drag_NewData;
            _view.DeleteData += View_deleteData;
            _view.DeleteDataClear += View_deleteDataClear;
        }

        private void View_deleteDataClear(object sender, EventArgs e)
        {
            _view.NewFilesGrid.ItemsSource = secondGr.Where(x=>!deletedList.Contains(x));
            delFiles = new Dictionary<string, int>();
            deletedList = new List<FileTableModel>();
        }

        private void View_deleteData(object sender, EventArgs e)
        {
            var l = _view.CurrentGrid.SelectedItems;
            List<FileTableModel> tb = new();
            foreach(var c in l)
            {
                FileTableModel m = ((FileTableModel)c).Clone();
                m.Name += " (DEL)";
                tb.Add(m);
            }

            deletedList = tb;
            foreach (var g in tb)
            {
                if(!delFiles.ContainsKey(g.Name.Replace(" (DEL)", "")))
                    delFiles.Add(g.Name.Replace(" (DEL)",""), g.Version);
            }
            SetDataInGrids(_model.GetListFiles(), null, null, deletedList);
        }

        private void Drag_NewData(object sender, DragEventArgs e)
        {
            var l = (string[])e.Data.GetData(DataFormats.FileDrop);
            fileNames.AddRange(l);
            List<FileTableModel> ln = _model.ComparisonByName(fileTableModels, l.ToList()).Item1;
            List<FileTableModel> lnNF = _model.ComparisonByName(fileTableModels, l.ToList()).Item2;
            List<FileTableModel> newL = ln.Select(x=>x.Clone()).ToList();
            List<FileTableModel> newN = _model.GetAddedList(newL);
            foreach (var g in newN)
            {
                updateableFiles.Add(g.Name, g.Version);
            }
            SetDataInGrids(ln, newN, lnNF, deletedList);
        }

        private void SetDataInGrids(List<FileTableModel> firstGrid,List<FileTableModel> secondGrid,List<FileTableModel> secondGridNotFoundedFiles, List<FileTableModel> delFiles)
        {
            _view.CurrentGrid.ItemsSource = firstGrid;

            List<FileTableModel> sec = new();
            if (secondGrid != null)
                sec = new List<FileTableModel>(secondGrid);
            if (secondGridNotFoundedFiles != null)
            {
                sec.AddRange(secondGridNotFoundedFiles);
            }
            if (delFiles.Count != 0)
                sec.AddRange(delFiles);
            secondGr = new List<FileTableModel>(sec);
            _view.NewFilesGrid.ItemsSource = sec;

        }

        private void Add_NewData(object sender, OpenFileDialog e)
        {
            var f = e.FileNames;
            fileNames.AddRange(f);
            List<FileTableModel> ln = _model.ComparisonByName(fileTableModels, f.ToList()).Item1;
            List<FileTableModel> lnNF = _model.ComparisonByName(fileTableModels, f.ToList()).Item2;
            List<FileTableModel> newL = ln.Select(x => x.Clone()).ToList();
            List<FileTableModel> newN = _model.GetAddedList(newL);
            foreach(var g in newN)
            {
                if(!updateableFiles.ContainsKey(g.Name))
                    updateableFiles.Add(g.Name, g.Version);
            }
            SetDataInGrids(ln, newN, lnNF, deletedList);
        }

        private void Update_Data(object sender, EventArgs e)
        {
            CreateDescriptionWindow w = new(_view.TextBoxNew.Text, fileNames, updateableFiles, delFiles,_model);
            w.ShowDialog();

            //Старый вариант без описания изменения
            //_model.addVersion(Properties.Resources.ClientFolderName, Properties.Resources.VersionFolderName, _view.TextBoxNew.Text, fileNames, updateableFiles,delFiles);
            //_model.createTable(Environment.CurrentDirectory + @"\" + Properties.Resources.CSVTableName);
        }

        private void Load_Data(object sender, EventArgs e)
        {
            var l = _model.GetListFiles();
            _view.CurrentGrid.ItemsSource = l;
            fileTableModels = l;
            _view.TextBoxCurrent.Text = _model.GetLastFullVersion();
            _view.TextBoxNew.Text = _model.GetLastFullVersionByMainAndSubVersion();
        }
    }
}
