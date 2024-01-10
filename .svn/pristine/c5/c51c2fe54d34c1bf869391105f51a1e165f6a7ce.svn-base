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

        List<string> fileNames = new List<string>();

        Dictionary<string,int> updateableFiles = new Dictionary<string,int>();

        Dictionary<string,int> delFiles = new Dictionary<string,int>();
        List<FileTableModel> deletedList = new List<FileTableModel>();

        List<FileTableModel> secondGr = new List<FileTableModel>();
 
        public UpdateWindowPresenter(IUpdateWindow view, IMainModel model)
        {
            _view = view;
            _model = model;
            _view.loadData += load_Data;
            _view.updateData += update_Data;
            _view.addopenFileDialogHandler += add_NewData;
            _view.dragNewData += drag_NewData;
            _view.deleteData += _view_deleteData;
            _view.deleteDataClear += _view_deleteDataClear;
        }

        private void _view_deleteDataClear(object sender, EventArgs e)
        {
            _view.NewFilesGrid.ItemsSource = secondGr.Where(x=>!deletedList.Contains(x));
            delFiles = new Dictionary<string, int>();
            deletedList = new List<FileTableModel>();
        }

        private void _view_deleteData(object sender, EventArgs e)
        {
            var l = _view.CurrentGrid.SelectedItems;
            List<FileTableModel> tb = new List<FileTableModel>();
            foreach(var c in l)
            {
                FileTableModel m = ((FileTableModel)c).clone();
                m.name += " (DEL)";
                tb.Add(m);
            }

            deletedList = tb;
            foreach (var g in tb)
            {
                if(!delFiles.ContainsKey(g.name.Replace(" (DEL)", "")))
                    delFiles.Add(g.name.Replace(" (DEL)",""), g.version);
            }
            setDataInGrids(_model.getListFiles(), null, null, deletedList);
        }

        private void drag_NewData(object sender, DragEventArgs e)
        {
            var l = (string[])e.Data.GetData(DataFormats.FileDrop);
            fileNames.AddRange(l);
            List<FileTableModel> ln = _model.comparisonByName(fileTableModels, l.ToList()).Item1;
            List<FileTableModel> lnNF = _model.comparisonByName(fileTableModels, l.ToList()).Item2;
            List<FileTableModel> newL = ln.Select(x=>x.clone()).ToList();
            List<FileTableModel> newN = _model.getAddedList(newL);
            foreach (var g in newN)
            {
                updateableFiles.Add(g.name, g.version);
            }
            setDataInGrids(ln, newN, lnNF, deletedList);
        }

        private void setDataInGrids(List<FileTableModel> firstGrid,List<FileTableModel> secondGrid,List<FileTableModel> secondGridNotFoundedFiles, List<FileTableModel> delFiles)
        {
            _view.CurrentGrid.ItemsSource = firstGrid;

            List<FileTableModel> sec = new List<FileTableModel>();
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

        private void add_NewData(object sender, OpenFileDialog e)
        {
            var f = e.FileNames;
            fileNames.AddRange(f);
            List<FileTableModel> ln = _model.comparisonByName(fileTableModels, f.ToList()).Item1;
            List<FileTableModel> lnNF = _model.comparisonByName(fileTableModels, f.ToList()).Item2;
            List<FileTableModel> newL = ln.Select(x => x.clone()).ToList();
            List<FileTableModel> newN = _model.getAddedList(newL);
            foreach(var g in newN)
            {
                if(!updateableFiles.ContainsKey(g.name))
                    updateableFiles.Add(g.name, g.version);
            }
            setDataInGrids(ln, newN, lnNF, deletedList);
        }

        private void update_Data(object sender, EventArgs e)
        {
            CreateDescriptionWindow w = new CreateDescriptionWindow(_view.TextBoxNew.Text, fileNames, updateableFiles, delFiles,_model);
            w.ShowDialog();

            //Старый вариант без описания изменения
            //_model.addVersion(Properties.Resources.ClientFolderName, Properties.Resources.VersionFolderName, _view.TextBoxNew.Text, fileNames, updateableFiles,delFiles);
            //_model.createTable(Environment.CurrentDirectory + @"\" + Properties.Resources.CSVTableName);
        }

        private void load_Data(object sender, EventArgs e)
        {
            var l = _model.getListFiles();
            _view.CurrentGrid.ItemsSource = l;
            fileTableModels = l;
            _view.TextBoxCurrent.Text = _model.getLastFullVersion();
            _view.TextBoxNew.Text = _model.getLastFullVersionByMainAndSubVersion();
        }
    }
}
