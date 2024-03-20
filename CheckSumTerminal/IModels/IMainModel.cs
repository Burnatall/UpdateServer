using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using CheckSumTerminal.Models;

namespace CheckSumTerminal.IModels
{
    public interface IMainModel
    {
        string ErrorInfo { get; set; }
        List<FileTableModel> UpdatedList { get; set; }
        int Iterator { get; set; }
        string WhatHapend { get; set; }
        int Count { get; set; }
        Tuple<List<FileTableModel>, List<FileTableModel>> ComparisonByName(List<FileTableModel> input, List<string> names);
        List<FileTableModel> GetAddedList(List<FileTableModel> input);
        string GetLastSubVersion();
        string GetLastMainVersion();
        List<FileTableModel> GetListFiles();
        void CreateTable(string path);
        List<FileTableModel> AddVersion(string DirectoryNameMain, string DirectoryNameVersion, string NumberOfVersion,
            List<string> filesOfNewVesion, Dictionary<string, int> listsOfUpdateableFiles, Dictionary<string, int> deletedList, BackgroundWorker backgroundWorker);
        List<VersionModelPretty> GetVersionModels();
        List<FileTableModel> GetListFilesFromTable(string pathToTable);
        void CreateZipAndTableByPath(string pathToVersion, string pathToMainClient, string NumberOfVersionOld);
        bool UnzipToCurrentClient(string pathToVersion);
        void CreateDbTableFromClient(string NumberOfVersion);
        List<FileTableModel> ConvertFilesToTableInBase(string DirectoryNameMain, Dictionary<string, int> updateableFiles);
        void IncreaseIdOfVersion(string version);
        string GetLastFullVersion();
        string GetLastFullVersionByMainAndSubVersion();
        void CreateChangesFile(string path, string text);
        List<double> GetAllVersions();
        Dictionary<string, DateTime> GetAllVersionWithDates();
        VersionModel GetPreviousVersionByNumber(string number);
        string GetChosenFullVersion(VersionModel m);
        void AddParentVersionToBase(VersionModel m);
        VersionModel GetVersionByNumber(string number);
        List<string> VersionCheck(string pathToFolder);
    }
}