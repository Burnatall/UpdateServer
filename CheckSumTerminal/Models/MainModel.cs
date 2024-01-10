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

        public int iterator { get; set; }
        public int count { get; set; }
        public string whatHapend { get; set; }



        private ApplicationContext _applicationContext;

        public List<FileTableModel> updatedList { get; set; }

        public string ErrorInfo { get; set; }

        public MainModel()
        {
            _applicationContext = new ApplicationContext();
            _applicationContext.Database.EnsureCreated();
            addNewVersionToBase(Properties.Resources.StartMainVersion, Properties.Resources.StartSubVersion);
        }

        /// <summary>
        /// Добавление версии в таблицу версий
        /// </summary>
        /// <param name="main">Основная цифра версии</param>
        /// <param name="sub">Второстепенная цифра версии</param>
        public void addNewVersionToBase(string main, string sub)
        {
            if (!_applicationContext.versions.Where(x => (x.main_version + x.sub_version) == main + sub).Any())
            {
                _applicationContext.versions.Add(new VersionModel()
                {
                    sub_version = sub,
                    main_version = main,
                    date_time = DateTime.Now
                });
                _applicationContext.SaveChanges();
            }
            else if (!_applicationContext.versions.Any())
            {
                _applicationContext.versions.Add(new VersionModel()
                {
                    sub_version = sub,
                    main_version = main,
                    date_time = DateTime.Now
                });
                _applicationContext.SaveChanges();
            }
        }

        /// <summary>
        /// Заносит данные о файлах в выбранной директории в таблицу файлов
        /// </summary>
        /// <param name="DirectoryNameMain">Выбранная директория</param>
        /// <returns></returns>
        public List<FileTableModel> convertFilesToTableInBase(string DirectoryNameMain)
        {
            List<FileTableModel> filesList = new List<FileTableModel>();
            var files = Directory.GetFiles(Path.GetFullPath(DirectoryNameMain));
            int i = 1;
            foreach (var c in files)
            {
                FileTableModel ftm = new FileTableModel();
                ftm.id = i;
                ftm.name = Path.GetFileName(c);
                ftm.version = 1;
                ftm.date_time = DateTime.Now.ToLongTimeString();
                filesList.Add(ftm);
                i++;
            }
            try
            {
                _applicationContext.files.RemoveRange(_applicationContext.files);
                _applicationContext.SaveChanges();
                _applicationContext.files.AddRange(filesList);
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
        public List<FileTableModel> convertFilesToTableInBase(string DirectoryNameMain,Dictionary<string,int> updateableFiles)
        {
            List<FileTableModel> filesList = new List<FileTableModel>();
            var files = Directory.GetFiles(Path.GetFullPath(DirectoryNameMain));
            int i = 1;
            foreach (var c in files)
            {
                FileTableModel ftm = new FileTableModel();
                ftm.id = i;
                ftm.name = Path.GetFileName(c);
                if(!updateableFiles.ContainsKey(ftm.name))
                    ftm.version = 1;
                else
                    ftm.version = updateableFiles[ftm.name];
                ftm.date_time = DateTime.Now.ToLongTimeString();
                filesList.Add(ftm);
                i++;
            }
            try
            {
                _applicationContext.files.RemoveRange(_applicationContext.files);
                _applicationContext.SaveChanges();
                _applicationContext.files.AddRange(filesList);
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
        public List<FileTableModel> addVersion(string DirectoryNameMain, string DirectoryNameVersion, string NumberOfVersion,
            List<string> filesOfNewVesion,Dictionary<string,int> listsOfUpdateableFiles, Dictionary<string,int> deletedList, BackgroundWorker backgroundWorker)
        {
            List<FileTableModel> filesList = new List<FileTableModel>();
            if (_applicationContext.files == null||_applicationContext.files.Count()==0)
            {
                createChangesFile(Environment.CurrentDirectory + @"\" + Properties.Resources.ChangesDocument, "Первая версия");
                return convertFilesToTableInBase(DirectoryNameMain);
            }
            else
            {
                if (filesOfNewVesion.Any()|| deletedList.Any())
                {
                    iterator = 1;
                    count = filesOfNewVesion.Count + deletedList.Count + 10;
                    backgroundWorker.ReportProgress(iterator / count);

                    //Получаем номер старой версии
                    string NumberOfVersionOld = getLastFullVersion();

                    whatHapend = "Архивирование последней версии";
                    iterator++;
                    backgroundWorker.ReportProgress(iterator / count);

                    //Создаем папку для сохранения старой версии
                    var s = Environment.CurrentDirectory + @"\"+ DirectoryNameVersion;

                    iterator++;
                    backgroundWorker.ReportProgress(iterator / count);

                    //Создаем папку для сохранения старой версии, а в ней архив, таблицу и описание обновления в директории версий
                    createZipAndTableByPath(s + @"\" + NumberOfVersionOld, Environment.CurrentDirectory + @"\" + DirectoryNameMain, NumberOfVersionOld);

                    whatHapend = "Копирование файлов в папку с эталоном";
                    iterator++;
                    backgroundWorker.ReportProgress(iterator / count);

                    //Копируем файлы в папку с эталоном
                    //count = filesOfNewVesion.Count;
                    var t = Environment.CurrentDirectory + @"\" + DirectoryNameMain;
                    foreach (var c in filesOfNewVesion)
                    {
                        File.Copy(c, t+@"\"+Path.GetFileName(c), true);

                        iterator++;
                        backgroundWorker.ReportProgress(iterator / count);
                    }
                    whatHapend = "Удаление выбранных файлов";
                    backgroundWorker.ReportProgress(iterator / count);

                    //Удаляем те файлы, которые нужно удалить
                    if (deletedList.Any())
                    {
                        foreach (var c in deletedList)
                        {
                            File.Delete(t + @"\" + Path.GetFileName(c.Key));

                            iterator++;
                            backgroundWorker.ReportProgress(iterator / count);
                        }
                    }
                    //Добавляем новую версию в таблицу версий
                    createDbTableFromClient(NumberOfVersion);

                    whatHapend = "Завершение";
                    iterator = count;
                    backgroundWorker.ReportProgress(iterator);

                    //Переводим файлы из папки эталона в таблицу файлов, если таблицы нет создаем первую версию всех файлов в папке
                    if (listsOfUpdateableFiles != null)
                        return convertFilesToTableInBase(DirectoryNameMain, listsOfUpdateableFiles);
                    else
                        return convertFilesToTableInBase(DirectoryNameMain);
                }
            }
            return filesList;
        }

        /// <summary>
        /// Создание версии по ее номеру
        /// </summary>
        /// <param name="NumberOfVersion">Номер версии в формате %Основная версия%.%побочная версия%</param>
        public void createDbTableFromClient(string NumberOfVersion)
        {
            _applicationContext.versions.Add(new VersionModel()
            {
                id = _applicationContext.versions.Max(x=>x.id)+1,
                main_version = NumberOfVersion.Split(".")[0],
                sub_version = NumberOfVersion.Split(".")[1],
                date_time = DateTime.Now
            });
            _applicationContext.SaveChanges();
        }

        /// <summary>
        /// Метод сохраниения архива, таблицы и файла с изменениями из эталона в папку с номером этой версии
        /// </summary>
        /// <param name="pathToVersion">Путь до папки с версиями</param>
        /// <param name="pathToMainClient">Путь до эталона</param>
        /// <param name="NumberOfVersionOld">Номер сохранияемой версии</param>
        public void createZipAndTableByPath(string pathToVersion,string pathToMainClient, string NumberOfVersionOld)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pathToVersion);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            //dirInfo.CreateSubdirectory(NumberOfVersionOld);

            //Сохраняем архив со старой версией в папку с версиями (DirectoryNameMain)
            zipCurrentClient(pathToMainClient, pathToVersion + @"\" + NumberOfVersionOld + ".zip");
            //Создаем в той же папке таблицу этой версии
            createTable(pathToVersion + @"\" + NumberOfVersionOld + ".csv");
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
        public bool zipCurrentClient(string folderToPack, string nameOfZip)
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
        public bool unzipToCurrentClient(string pathToVersion)
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
        public void increaseIdOfVersion(string version)
        {
            string main = version.Split('.')[0];
            string sub = version.Split('.')[1];
            var save = _applicationContext.versions.Where(x => x.main_version == main && x.sub_version == sub).First();
            _applicationContext.versions.Remove(save);
            _applicationContext.SaveChanges();
            save.id = _applicationContext.versions.Max(x => x.id) + 1;
            _applicationContext.versions.Add(save);
            _applicationContext.SaveChanges();
        }

        /// <summary>
        /// Создание папки с версией
        /// </summary>
        /// <param name="versFolder">Путь к папке</param>
        /// <param name="vers">номер версии</param>
        public void createDirectoryOfVersion(string versFolder,string vers)
        {
            Directory.CreateDirectory(versFolder+@"\"+vers);
        }

        /// <summary>
        /// Создание csv таблицы на основе таблицы из базы
        /// </summary>
        /// <param name="path">Путь к будущей таблице</param>
        public void createTable(string path)
        {
            var files = _applicationContext.files.ToList();
            files = files.OrderBy(x => x.id).ToList();
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
            using (var sw = new StreamWriter(path, false, Encoding.Default))
            {
                sw.WriteLine(s);
                for(int i = 0; i < files.Count; i++)
                {
                    sw.WriteLine(files[i].id + "," + files[i].name + "," + files[i].version + "," + files[i].date_time.ToString());
                }
            }
        }

        /// <summary>
        /// Создание txt файла с изменениями
        /// </summary>
        /// <param name="path">Путь для создания файла</param>
        /// <param name="text">Содержимое файла</param>
        public void createChangesFile(string path,string text)
        {
            using (var sw = new StreamWriter(path, false, Encoding.Default))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// Получение списка всех версий в double формате
        /// </summary>
        /// <returns></returns>
        public List<double> getAllVersions()
        {
           return _applicationContext.versions.Select(x => x.main_version+"," + x.sub_version).ToList().Select(x => double.Parse(x)).ToList();
        }

        /// <summary>
        /// Получение предидущей версии в базе по номеру выбранной (предидущая по id, а не по версии!!!)
        /// </summary>
        /// <param name="number">Номер версии</param>
        /// <returns></returns>
        public VersionModel getPreviousVersionByNumber(string number)
        {
            var lst = _applicationContext.versions.ToArray();
            VersionModel ans = null;
            for (int i = 0; i< lst.Length && lst[i].main_version + "." + lst[i].sub_version!=number;i++)
                ans = lst[i];
            return ans;
        }

        /// <summary>
        /// Получение списка всех версий в формате (номер\дата)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,DateTime> getAllVersionWithDates()
        {
            var vers = _applicationContext.versions;
            Dictionary<string, DateTime> ans = new Dictionary<string, DateTime>();
            foreach (var item in vers)
            {
                ans.Add(item.main_version + "," + item.sub_version, item.date_time);
            }
            return ans;
        }

        /// <summary>
        /// Получение записей о файлах из таблицы файлов
        /// </summary>
        /// <param name="pathToTable">Путь до таблицы файлов</param>
        /// <returns></returns>
        public List<FileTableModel> getListFilesFromTable(string pathToTable)
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
                        id = long.Parse(snext[0]),
                        name = snext[1],
                        version = int.Parse(snext[2]),
                        date_time = snext[3]
                    });
                }
            }
            return  list.OrderBy(x=>x.id).ToList();
        }

        /// <summary>
        /// Получение списка файлов из БД (сортировка по возрастанию)
        /// </summary>
        /// <returns></returns>
        public List<FileTableModel> getListFiles()
        {
            return _applicationContext.files.OrderBy(x=>x.id).ToList();
        }

        /// <summary>
        /// Получение версий из базы в "читабельном" формате (VersionModelPretty)
        /// </summary>
        /// <returns></returns>
        public List<VersionModelPretty> getVersionModels()
        {
            List<VersionModelPretty> ans = new List<VersionModelPretty>();
            var l = _applicationContext.versions;
            foreach(var c in l)
            {
                ans.Add(new VersionModelPretty(c.id, c.main_version, c.sub_version, c.date_time, c.parent_branch));
            }
            return ans.OrderBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Получение основной версии по самому большому id версии 
        /// </summary>
        /// <returns></returns>
        public string getLastMainVersion()
        {
            return _applicationContext.versions.Where(j => j.id == _applicationContext.versions.Max(x => x.id)).First().main_version;
        }

        /// <summary>
        /// Получение побочной версии по самому большому id версии 
        /// </summary>
        /// <returns></returns>
        public string getLastSubVersion() 
        {
            return _applicationContext.versions.Where(j => j.id == _applicationContext.versions.Max(x => x.id)).First().sub_version;
        }

        /// <summary>
        /// Получение общей версии для максимального id
        /// </summary>
        /// <returns></returns>
        public string getLastFullVersion()
        {
            return getLastMainVersion() + "." + getLastSubVersion();
        }

        /// <summary>
        /// Получния строки с полной версией выбранной модели
        /// </summary>
        /// <param name="m">Выбранная модель</param>
        /// <returns></returns>
        public string getChosenFullVersion(VersionModel m)
        {
            if (m != null)
                return m.main_version + "." + m.sub_version;
            return "";
        }

        /// <summary>
        /// Получение модели версии по ее полному номеру
        /// </summary>
        /// <param name="number">Номер в формате %Основная версия%.%Второстепенная версия%</param>
        /// <returns></returns>
        public VersionModel getVersionByNumber(string number)
        {
            return _applicationContext.versions.Where(x => x.main_version + "." + x.sub_version == number).FirstOrDefault();
        }

        /// <summary>
        /// Добавление записи о родительской версии во все версии которые старше чем полученная
        /// </summary>
        /// <param name="m">Полученная версия</param>
        public void addParentVersionToBase(VersionModel m)
        {
            foreach (var c in _applicationContext.versions)
            {
                if (c.id > m.id)
                    c.parent_branch = getChosenFullVersion(m);
            }
        }

        /// <summary>
        /// Метод для получения последней версии именно по полю версии и субверсии а не id (второстепенный номер версии сразу увеличен на 1)
        /// </summary>
        /// <returns>Полученная версия</returns>
        public string getLastFullVersionByMainAndSubVersion()
        {
            var maxMain = _applicationContext.versions.Select(x => int.Parse(x.main_version)).ToList().Max().ToString();
            var maxSubInMaxMain = _applicationContext.versions.Where(x => x.main_version == maxMain).ToList().Select(x=>int.Parse(x.sub_version)).ToList().Max();
            return maxMain.ToString()+"."+(maxSubInMaxMain+1).ToString();
        }

        /// <summary>
        /// Сравнение списка с моделями со списком имен и выдача двух массивов - результатов сравнения
        /// </summary>
        /// <param name="input">Входной список</param>
        /// <param name="names">Список имен</param>
        /// <returns></returns>
        public Tuple<List<FileTableModel>,List<FileTableModel>> comparisonByName(List<FileTableModel> input,List<string> names)
        {
            List<FileTableModel> ans = new List<FileTableModel>();
            List<FileTableModel> ans2 = new List<FileTableModel>();
            long id = input.Max(x => x.id)+1;
            foreach (var c in names)
            {
                var s = Path.GetFileName(c);
                var k = input.Where(x => x.name == s).ToList();
                if (k!=null&&k.Count!=0)
                {
                    ans.Add(k.First());
                }
                else
                {
                    ans2.Add(new FileTableModel()
                    {
                        id = id,
                        name = Path.GetFileName(c),
                        version = 1,
                        date_time = DateTime.Now.ToLongTimeString()
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
        public List<FileTableModel> getAddedList(List<FileTableModel> input)
        {
            foreach(var c in input)
            {
                c.version++;
                c.date_time = DateTime.Now.ToLongTimeString();
            }
            return input;
        }


        /// <summary>
        /// Проверяет для всех ли версий в базе существуют сохраненные папки
        /// </summary>
        /// <param name="pathToFolder">папка куда сохраняются версии</param>
        /// <returns>Список версий с проблемными папками (папка отсутсвует, либо внутри не 3 файла)</returns>
        public List<string> versionCheck(string pathToFolder)
        {
            var lstV = _applicationContext.versions.ToList();
            List<string> result = new List<string>();
            if (Directory.Exists(pathToFolder) && Directory.GetDirectories(pathToFolder).Length == lstV.Count - 1)
            {
                foreach (var c in lstV)
                {
                    if (!Directory.GetDirectories(pathToFolder).Contains(getChosenFullVersion(c)))
                        result.Add(getChosenFullVersion(c));
                    else if (Directory.GetFiles(Directory.GetDirectories(pathToFolder).Where(x=>x==getChosenFullVersion(c)).First()).Length!=3)
                        result.Add(getChosenFullVersion(c));
                }
                return result;
            }
            else
                return null;
        }
    }
}
