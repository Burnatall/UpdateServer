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
        List<FileTableModel> updatedList { get; set; }
        int iterator { get; set; }
        string whatHapend { get; set; }
        int count { get; set; }
        Tuple<List<FileTableModel>, List<FileTableModel>> comparisonByName(List<FileTableModel> input, List<string> names);
        List<FileTableModel> getAddedList(List<FileTableModel> input);
        string getLastSubVersion();
        string getLastMainVersion();
        List<FileTableModel> getListFiles();
        void createTable(string path);
        List<FileTableModel> addVersion(string DirectoryNameMain, string DirectoryNameVersion, string NumberOfVersion,
            List<string> filesOfNewVesion, Dictionary<string, int> listsOfUpdateableFiles, Dictionary<string, int> deletedList, BackgroundWorker backgroundWorker);
        List<VersionModelPretty> getVersionModels();
        List<FileTableModel> getListFilesFromTable(string pathToTable);
        void createZipAndTableByPath(string pathToVersion, string pathToMainClient, string NumberOfVersionOld);
        bool unzipToCurrentClient(string pathToVersion);
        void createDbTableFromClient(string NumberOfVersion);
        List<FileTableModel> convertFilesToTableInBase(string DirectoryNameMain, Dictionary<string, int> updateableFiles);
        void increaseIdOfVersion(string version);
        string getLastFullVersion();
        string getLastFullVersionByMainAndSubVersion();
        void createChangesFile(string path, string text);
        List<double> getAllVersions();
        Dictionary<string, DateTime> getAllVersionWithDates();
        VersionModel getPreviousVersionByNumber(string number);
        string getChosenFullVersion(VersionModel m);
        void addParentVersionToBase(VersionModel m);
        VersionModel getVersionByNumber(string number);
        List<string> versionCheck(string pathToFolder);
    }
}