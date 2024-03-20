using CheckSumTerminal.Context;
using CheckSumTerminal.IModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CheckSumTerminal.Models
{
    public class MainModel : IMainModel
    {

        public int Iterator { get; set; }
        public int Count { get; set; }
        public string WhatHapend { get; set; }



        private ApplicationContext _applicationContext;

        public List<FileTableModel> UpdatedList { get; set; }

        public string ErrorInfo { get; set; }

        public MainModel()
        {
            _applicationContext = new ApplicationContext();
            _applicationContext.Database.EnsureCreated();
            AddNewVersionToBase(Properties.Resources.StartMainVersion, Properties.Resources.StartSubVersion);
        }

        /// <summary>
        /// Добавление версии в таблицу версий
        /// </summary>
        /// <param name="main">Основная цифра версии</param>
        /// <param name="sub">Второстепенная цифра версии</param>
        public void AddNewVersionToBase(string main, string sub)
        {
            if (!_applicationContext.Versions.Where(x => (x.MainVersion + x.SubVersion) == main + sub).Any())
            {
                _applicationContext.Versions.Add(new VersionModel()
                {
                    SubVersion = sub,
                    MainVersion = main,
                    DateTime = DateTime.Now
                });
                _applicationContext.SaveChanges();
            }
            else if (!_applicationContext.Versions.Any())
            {
                _applicationContext.Versions.Add(new VersionModel()
                {
                    SubVersion = sub,
                    MainVersion = main,
                    DateTime = DateTime.Now
                });
                _applicationContext.SaveChanges();
            }
        }

        /// <summary>
        /// Заносит данные о файлах в выбранной директории в таблицу файлов
        /// </summary>
        /// <param name="DirectoryNameMain">Выбранная директория</param>
        /// <returns></returns>
        public List<FileTableModel> ConvertFilesToTableInBase(string DirectoryNameMain)
        {
            List<FileTableModel> filesList = new();
            var files = Directory.GetFiles(Path.GetFullPath(DirectoryNameMain));
            int i = 1;
            foreach (var c in files)
            {
                FileTableModel ftm = new()
                {
                    Id = i,
                    Name = Path.GetFileName(c),
                    Version = 1,
                    DateTime = DateTime.Now.ToLongTimeString()
                };
                filesList.Add(ftm);
                i++;
            }
            try
            {
                _applicationContext.Files.RemoveRange(_applicationContext.Files);
                _applicationContext.SaveChanges();
                _applicationContext.Files.AddRange(filesList);
                _applicationContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ErrorInfo = ex.Message;
                return null;
            }
            catch (InvalidOperationException ex)
            {
                ErrorInfo = ex.Message;
                return null;
            }
            return filesList;

        }

        /// <summary>
        /// Изменяем записи файлов в таблице в соответствии с новым списком изменяемых файлов, переписываются все.
        /// Удаленные файлы уже отсутсвуют в эталоне поэтому не будут записаны
        /// </summary>
        /// <param name="DirectoryNameMain">Путь к директории эталона</param>
        /// <param name="updateableFiles">Список измененных файлов (название\версия)</param>
        /// <returns></returns>
        public List<FileTableModel> ConvertFilesToTableInBase(string DirectoryNameMain,Dictionary<string,int> updateableFiles)
        {
            List<FileTableModel> filesList = new();
            var files = Directory.GetFiles(Path.GetFullPath(DirectoryNameMain));
            int i = 1;
            foreach (var c in files)
            {
                FileTableModel ftm = new()
                {
                    Id = i,
                    Name = Path.GetFileName(c)
                };
                if (!updateableFiles.ContainsKey(ftm.Name))
                    ftm.Version = 1;
                else
                    ftm.Version = updateableFiles[ftm.Name];
                ftm.DateTime = DateTime.Now.ToLongTimeString();
                filesList.Add(ftm);
                i++;
            }
            try
            {
                _applicationContext.Files.RemoveRange(_applicationContext.Files);
                _applicationContext.SaveChanges();
                _applicationContext.Files.AddRange(filesList);
                _applicationContext.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ErrorInfo = ex.Message;
                return null;
            }
            catch (InvalidOperationException ex)
            {
                ErrorInfo = ex.Message;
                return null;
            }
            return filesList;
        }

        /// <summary>
        /// Метод создания нового обновления
        /// </summary>
        /// <param name="DirectoryNameMain">Название папки с эталоном</param>
        /// <param name="DirectoryNameVersion">Название папки с версиями</param>
        /// <param name="NumberOfVersion">Номер новой версии</param>
        /// <param name="filesOfNewVesion">Список путей к файлам для обновления</param>
        /// <param name="listsOfUpdateableFiles">Словарь названий и их версий для обновления в таблице</param>
        /// <param name="deletedList">Лист удаляемых файлов (название\версия)</param>
        /// <param name="backgroundWorker">Ссылка на бекграунд воркер для отслеживания прогресса</param>
        /// <returns>Полученный список файлов после обновления</returns>
        public List<FileTableModel> AddVersion(string DirectoryNameMain, string DirectoryNameVersion, string NumberOfVersion,
            List<string> filesOfNewVesion,Dictionary<string,int> listsOfUpdateableFiles, Dictionary<string,int> deletedList, BackgroundWorker backgroundWorker)
        {
            List<FileTableModel> filesList = new();
            if (_applicationContext.Files == null|| !_applicationContext.Files.Any())
            {
                CreateChangesFile(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument, "Первая версия");
                return ConvertFilesToTableInBase(DirectoryNameMain);
            }
            else
            {
                if (filesOfNewVesion.Any()|| deletedList.Any())
                {
                    Iterator = 1;
                    Count = filesOfNewVesion.Count + deletedList.Count + 10;
                    backgroundWorker.ReportProgress(Iterator / Count);

                    //Получаем номер старой версии
                    string NumberOfVersionOld = GetLastFullVersion();

                    WhatHapend = "Архивирование последней версии";
                    Iterator++;
                    backgroundWorker.ReportProgress(Iterator / Count);

                    //Создаем папку для сохранения старой версии
                    var s = Environment.CurrentDirectory + @"\"+ DirectoryNameVersion;

                    Iterator++;
                    backgroundWorker.ReportProgress(Iterator / Count);

                    //Создаем папку для сохранения старой версии, а в ней архив, таблицу и описание обновления в директории версий
                    CreateZipAndTableByPath(s + @"\" + NumberOfVersionOld, Environment.CurrentDirectory + @"\" + DirectoryNameMain, NumberOfVersionOld);

                    WhatHapend = "Копирование файлов в папку с эталоном";
                    Iterator++;
                    backgroundWorker.ReportProgress(Iterator / Count);

                    //Копируем файлы в папку с эталоном
                    //count = filesOfNewVesion.Count;
                    var t = Environment.CurrentDirectory + @"\" + DirectoryNameMain;
                    foreach (var c in filesOfNewVesion)
                    {
                        File.Copy(c, t+@"\"+Path.GetFileName(c), true);

                        Iterator++;
                        backgroundWorker.ReportProgress(Iterator / Count);
                    }
                    WhatHapend = "Удаление выбранных файлов";
                    backgroundWorker.ReportProgress(Iterator / Count);

                    //Удаляем те файлы, которые нужно удалить
                    if (deletedList.Any())
                    {
                        foreach (var c in deletedList)
                        {
                            File.Delete(t + @"\" + Path.GetFileName(c.Key));

                            Iterator++;
                            backgroundWorker.ReportProgress(Iterator / Count);
                        }
                    }
                    //Добавляем новую версию в таблицу версий
                    CreateDbTableFromClient(NumberOfVersion);

                    WhatHapend = "Завершение";
                    Iterator = Count;
                    backgroundWorker.ReportProgress(Iterator);

                    //Переводим файлы из папки эталона в таблицу файлов, если таблицы нет создаем первую версию всех файлов в папке
                    if (listsOfUpdateableFiles != null)
                        return ConvertFilesToTableInBase(DirectoryNameMain, listsOfUpdateableFiles);
                    else
                        return ConvertFilesToTableInBase(DirectoryNameMain);
                }
            }
            return filesList;
        }

        /// <summary>
        /// Создание версии по ее номеру
        /// </summary>
        /// <param name="NumberOfVersion">Номер версии в формате %Основная версия%.%побочная версия%</param>
        public void CreateDbTableFromClient(string NumberOfVersion)
        {
            _applicationContext.Versions.Add(new VersionModel()
            {
                Id = _applicationContext.Versions.Max(x=>x.Id)+1,
                MainVersion = NumberOfVersion.Split(".")[0],
                SubVersion = NumberOfVersion.Split(".")[1],
                DateTime = DateTime.Now
            });
            _applicationContext.SaveChanges();
        }

        /// <summary>
        /// Метод сохраниения архива, таблицы и файла с изменениями из эталона в папку с номером этой версии
        /// </summary>
        /// <param name="pathToVersion">Путь до папки с версиями</param>
        /// <param name="pathToMainClient">Путь до эталона</param>
        /// <param name="NumberOfVersionOld">Номер сохранияемой версии</param>
        public void CreateZipAndTableByPath(string pathToVersion,string pathToMainClient, string NumberOfVersionOld)
        {
            DirectoryInfo dirInfo = new(pathToVersion);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            //dirInfo.CreateSubdirectory(NumberOfVersionOld);

            //Сохраняем архив со старой версией в папку с версиями (DirectoryNameMain)
            ZipCurrentClient(pathToMainClient, pathToVersion + @"\" + NumberOfVersionOld + ".zip");
            //Создаем в той же папке таблицу этой версии
            CreateTable(pathToVersion + @"\" + NumberOfVersionOld + ".csv");
            //Перемещаем файл с описанием обновления в папку с этим обновлением
            if(File.Exists(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument))
                File.Copy(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument, pathToVersion + @"\" + Properties.Resources.ChangesDocument,true);
        }

        /// <summary>
        /// Создание zip архива из выьранной папки
        /// </summary>
        /// <param name="folderToPack">Папка для архивирования</param>
        /// <param name="nameOfZip">Название будущего архива</param>
        /// <returns></returns>
        public bool ZipCurrentClient(string folderToPack, string nameOfZip)
        {
            try
            {
                if(File.Exists(nameOfZip))
                    File.Delete(nameOfZip);
                ZipFile.CreateFromDirectory(folderToPack, nameOfZip,CompressionLevel.Fastest,false);
                return true;
            }
            catch (Exception ex) 
            {
                ErrorInfo = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// Распаковка выбранного архива с файлами в основную версию
        /// </summary>
        /// <param name="pathToVersion">Путь к основной версии</param>
        /// <returns></returns>
        public bool UnzipToCurrentClient(string pathToVersion)
        {
            try
            {
                string zipFile = pathToVersion; // сжатый файл
                string targetFolder = Environment.CurrentDirectory+@"\"+ Properties.Resources.ClientFolderName; // папка, куда распаковывается файл

                Directory.Delete(targetFolder,true);

                ZipFile.ExtractToDirectory(zipFile, targetFolder,true);
                return true;
            }
            catch (Exception ex)
            {
                ErrorInfo = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// Делает id выбранной версии максимальным в таблице (необходимо для возврата к версии, последняя версия в базе всегда актуальная)
        /// </summary>
        /// <param name="version">Полная версия</param>
        public void IncreaseIdOfVersion(string version)
        {
            string main = version.Split('.')[0];
            string sub = version.Split('.')[1];
            var save = _applicationContext.Versions.Where(x => x.MainVersion == main && x.SubVersion == sub).First();
            _applicationContext.Versions.Remove(save);
            _applicationContext.SaveChanges();
            save.Id = _applicationContext.Versions.Max(x => x.Id) + 1;
            _applicationContext.Versions.Add(save);
            _applicationContext.SaveChanges();
        }

        /// <summary>
        /// Создание папки с версией
        /// </summary>
        /// <param name="versFolder">Путь к папке</param>
        /// <param name="vers">номер версии</param>
        public void CreateDirectoryOfVersion(string versFolder,string vers)
        {
            Directory.CreateDirectory(versFolder+@"\"+vers);
        }

        /// <summary>
        /// Создание csv таблицы на основе таблицы из базы
        /// </summary>
        /// <param name="path">Путь к будущей таблице</param>
        public void CreateTable(string path)
        {
            var files = _applicationContext.Files.ToList();
            files = files.OrderBy(x => x.Id).ToList();
            Type t = typeof(FileTableModel);
            var f = t.GetProperties();
            string s = "";
            for(int i = 0,j =0; i < f.Length; i++,j++)
            {
                s += f[i].Name;
                if (j < f.Length - 1)
                {
                    s += ",";
                }
            }
            s.Substring(0, s.Length - 1);
            using var sw = new StreamWriter(path, false, Encoding.Default);
            sw.WriteLine(s);
            for (int i = 0; i < files.Count; i++)
            {
                sw.WriteLine(files[i].Id + "," + files[i].Name + "," + files[i].Version + "," + files[i].DateTime.ToString());
            }
        }

        /// <summary>
        /// Создание txt файла с изменениями
        /// </summary>
        /// <param name="path">Путь для создания файла</param>
        /// <param name="text">Содержимое файла</param>
        public void CreateChangesFile(string path,string text)
        {
            using var sw = new StreamWriter(path, false, Encoding.Default);
            sw.Write(text);
        }

        /// <summary>
        /// Получение списка всех версий в double формате
        /// </summary>
        /// <returns></returns>
        public List<double> GetAllVersions()
        {
           return _applicationContext.Versions.Select(x => x.MainVersion+"," + x.SubVersion).ToList().Select(x => double.Parse(x)).ToList();
        }

        /// <summary>
        /// Получение предидущей версии в базе по номеру выбранной (предидущая по id, а не по версии!!!)
        /// </summary>
        /// <param name="number">Номер версии</param>
        /// <returns></returns>
        public VersionModel GetPreviousVersionByNumber(string number)
        {
            var lst = _applicationContext.Versions.ToArray();
            VersionModel ans = null;
            for (int i = 0; i< lst.Length && lst[i].MainVersion + "." + lst[i].SubVersion!=number;i++)
                ans = lst[i];
            return ans;
        }

        /// <summary>
        /// Получение списка всех версий в формате (номер\дата)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,DateTime> GetAllVersionWithDates()
        {
            var vers = _applicationContext.Versions;
            Dictionary<string, DateTime> ans = new();
            foreach (var item in vers)
            {
                ans.Add(item.MainVersion + "," + item.SubVersion, item.DateTime);
            }
            return ans;
        }

        /// <summary>
        /// Получение записей о файлах из таблицы файлов
        /// </summary>
        /// <param name="pathToTable">Путь до таблицы файлов</param>
        /// <returns></returns>
        public List<FileTableModel> GetListFilesFromTable(string pathToTable)
        {
            if (!File.Exists(pathToTable))
                return null;
            var list = new List<FileTableModel>();
            using (var sr = new StreamReader(pathToTable, Encoding.Default))
            {
                sr.ReadLine();
                while(!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    string[] snext = s.Split(',');
                    list.Add(new FileTableModel()
                    {
                        Id = long.Parse(snext[0]),
                        Name = snext[1],
                        Version = int.Parse(snext[2]),
                        DateTime = snext[3]
                    });
                }
            }
            return  list.OrderBy(x=>x.Id).ToList();
        }

        /// <summary>
        /// Получение списка файлов из БД (сортировка по возрастанию)
        /// </summary>
        /// <returns></returns>
        public List<FileTableModel> GetListFiles()
        {
            return _applicationContext.Files.OrderBy(x=>x.Id).ToList();
        }

        /// <summary>
        /// Получение версий из базы в "читабельном" формате (VersionModelPretty)
        /// </summary>
        /// <returns></returns>
        public List<VersionModelPretty> GetVersionModels()
        {
            List<VersionModelPretty> ans = new();
            var l = _applicationContext.Versions;
            foreach(var c in l)
            {
                ans.Add(new VersionModelPretty(c.Id, c.MainVersion, c.SubVersion, c.DateTime, c.ParentBranch));
            }
            return ans.OrderBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Получение основной версии по самому большому id версии 
        /// </summary>
        /// <returns></returns>
        public string GetLastMainVersion()
        {
            return _applicationContext.Versions.Where(j => j.Id == _applicationContext.Versions.Max(x => x.Id)).First().MainVersion;
        }

        /// <summary>
        /// Получение побочной версии по самому большому id версии 
        /// </summary>
        /// <returns></returns>
        public string GetLastSubVersion() 
        {
            return _applicationContext.Versions.Where(j => j.Id == _applicationContext.Versions.Max(x => x.Id)).First().SubVersion;
        }

        /// <summary>
        /// Получение общей версии для максимального id
        /// </summary>
        /// <returns></returns>
        public string GetLastFullVersion()
        {
            return GetLastMainVersion() + "." + GetLastSubVersion();
        }

        /// <summary>
        /// Получния строки с полной версией выбранной модели
        /// </summary>
        /// <param name="m">Выбранная модель</param>
        /// <returns></returns>
        public string GetChosenFullVersion(VersionModel m)
        {
            if (m != null)
                return m.MainVersion + "." + m.SubVersion;
            return "";
        }

        /// <summary>
        /// Получение модели версии по ее полному номеру
        /// </summary>
        /// <param name="number">Номер в формате %Основная версия%.%Второстепенная версия%</param>
        /// <returns></returns>
        public VersionModel GetVersionByNumber(string number)
        {
            return _applicationContext.Versions.Where(x => x.MainVersion + "." + x.SubVersion == number).FirstOrDefault();
        }

        /// <summary>
        /// Добавление записи о родительской версии во все версии которые старше чем полученная
        /// </summary>
        /// <param name="m">Полученная версия</param>
        public void AddParentVersionToBase(VersionModel m)
        {
            foreach (var c in _applicationContext.Versions)
            {
                if (c.Id > m.Id)
                    c.ParentBranch = GetChosenFullVersion(m);
            }
        }

        /// <summary>
        /// Метод для получения последней версии именно по полю версии и субверсии а не id (второстепенный номер версии сразу увеличен на 1)
        /// </summary>
        /// <returns>Полученная версия</returns>
        public string GetLastFullVersionByMainAndSubVersion()
        {
            var maxMain = _applicationContext.Versions.Select(x => int.Parse(x.MainVersion)).ToList().Max().ToString();
            var maxSubInMaxMain = _applicationContext.Versions.Where(x => x.MainVersion == maxMain).ToList().Select(x=>int.Parse(x.SubVersion)).ToList().Max();
            return maxMain.ToString()+"."+(maxSubInMaxMain+1).ToString();
        }

        /// <summary>
        /// Сравнение списка с моделями со списком имен и выдача двух массивов - результатов сравнения
        /// </summary>
        /// <param name="input">Входной список</param>
        /// <param name="names">Список имен</param>
        /// <returns></returns>
        public Tuple<List<FileTableModel>,List<FileTableModel>> ComparisonByName(List<FileTableModel> input,List<string> names)
        {
            List<FileTableModel> ans = new();
            List<FileTableModel> ans2 = new();
            long id = input.Max(x => x.Id)+1;
            foreach (var c in names)
            {
                var s = Path.GetFileName(c);
                var k = input.Where(x => x.Name == s).ToList();
                if (k!=null&&k.Count!=0)
                {
                    ans.Add(k.First());
                }
                else
                {
                    ans2.Add(new FileTableModel()
                    {
                        Id = id,
                        Name = Path.GetFileName(c),
                        Version = 1,
                        DateTime = DateTime.Now.ToLongTimeString()
                    });
                    id++;
                }
            }
            return new Tuple<List<FileTableModel>, List<FileTableModel>> ( ans,ans2);
        }

        /// <summary>
        /// Повышение номера версии во всех файлах списка на 1
        /// </summary>
        /// <param name="input">Входной список</param>
        /// <returns></returns>
        public List<FileTableModel> GetAddedList(List<FileTableModel> input)
        {
            foreach(var c in input)
            {
                c.Version++;
                c.DateTime = DateTime.Now.ToLongTimeString();
            }
            return input;
        }


        /// <summary>
        /// Проверяет для всех ли версий в базе существуют сохраненные папки
        /// </summary>
        /// <param name="pathToFolder">папка куда сохраняются версии</param>
        /// <returns>Список версий с проблемными папками (папка отсутсвует, либо внутри не 3 файла)</returns>
        public List<string> VersionCheck(string pathToFolder)
        {
            var lstV = _applicationContext.Versions.ToList();
            List<string> result = new();
            if (Directory.Exists(pathToFolder) && Directory.GetDirectories(pathToFolder).Length == lstV.Count - 1)
            {
                foreach (var c in lstV)
                {
                    if (!Directory.GetDirectories(pathToFolder).Contains(GetChosenFullVersion(c)))
                        result.Add(GetChosenFullVersion(c));
                    else if (Directory.GetFiles(Directory.GetDirectories(pathToFolder).Where(x=>x==GetChosenFullVersion(c)).First()).Length!=3)
                        result.Add(GetChosenFullVersion(c));
                }
                return result;
            }
            else
                return null;
        }
    }
}
