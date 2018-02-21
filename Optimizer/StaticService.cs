using System;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Optimizer
{
    /// <summary>
    /// Представляет сервисные функции
    /// </summary>
    public static class StaticService
    {

        #region - Serializes -
        /// <summary>
        /// Серриализует объект obj в файл с именем file_name
        /// </summary>
        /// <param name="obj">серриализуемый объект</param>
        /// <param name="file_name">имя файла</param>
        public static void Serializes(object obj, string file_name)
        {
            BinaryFormatter formter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(Directory.GetCurrentDirectory() + @"\" + file_name + ".dat", FileMode.OpenOrCreate))
                {
                    try
                    {
                        formter.Serialize(fs, obj);
                    }
                    catch (SerializationException ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.HelpLink);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void Serializes(object obj, string file_name, string path)
        {
            BinaryFormatter formter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path + @"\" + file_name + ".dat", FileMode.OpenOrCreate))
                {
                    try
                    {
                        formter.Serialize(fs, obj);
                    }
                    catch (SerializationException ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.HelpLink);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region - Deserialize -
        /// <summary>
        /// Десерриализует объект obj из файла с именем file_name
        /// </summary>
        /// <param name="file_name">имя файла</param>
        /// <param name="Object">десерриализуемый объект</param>
        /// <returns></returns>
        public static object Deserializes(string file_name, object Object)
        {
            BinaryFormatter formter = new BinaryFormatter();
            object obj = null;

            formter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(file_name + ".dat", FileMode.Open))
                {
                    try
                    {
                        obj = formter.Deserialize(fs);
                    }
                    catch (SerializationException ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.HelpLink);
                        obj = Object;
                    }
                }
            }
            catch (Exception)
            {
                obj = Object;
            }

            return obj;
        }
        public static object Deserializes(string path, string file_name, object Object)
        {
            BinaryFormatter formter = new BinaryFormatter();
            object obj = null;

            formter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path + @"\" + file_name + ".dat", FileMode.Open))
                {
                    try
                    {
                        obj = formter.Deserialize(fs);
                    }
                    catch (SerializationException ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.HelpLink);
                        obj = Object;
                    }
                }
            }
            catch (Exception)
            {
                obj = Object;
            }

            return obj;
        }
        public static object Deserializes(string path)
        {
            BinaryFormatter formter = new BinaryFormatter();
            object obj = null;

            formter = new BinaryFormatter();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    try
                    {
                        obj = formter.Deserialize(fs);
                    }
                    catch (SerializationException ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.HelpLink);
                        obj = new object();
                    }
                }
            }
            catch (Exception)
            {
                obj = new object();
            }

            return obj;
        }
        public static async Task<object> DeserializesAsync(string path)
        {
            BinaryFormatter formter = new BinaryFormatter();
            object obj = null;
            FileStream fs = null;

            try
            {
                await Task.Run(() =>
                {
                    formter = new BinaryFormatter();
                    fs = new FileStream(path, FileMode.Open);
                    obj = formter.Deserialize(fs);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.HelpLink);
                obj = new object();
            }
            

            fs.Close();

            return obj;
        }
        #endregion

        #region -Generate key-
        /// <summary>
        /// Генерация ключа в формате [0000][00] где:
        /// 0000 - количество преиодов
        /// 00 - количество stddev
        /// </summary>
        public static string GenerateKey(int _countperiod, double _countstd)
        {
            int r = (int)Math.Round((_countstd * 10), 0);

            return _countperiod.ToString("D4") + r.ToString("D2");
        }
        #endregion

        #region -Запись в файл-
        /// <summary>
        /// Запись в файл
        /// </summary>
        /// <param name="_str">запись</param>
        /// <param name="file_name">имя файла</param>
        /// <param name="append">append = true - добавление; append = false - перезапись</param>
        public static void LogFileWrite(string _str, string file_name, bool append)
        {
            string time = DateTime.Now.ToString();
            string str_log = time + "  " + _str;

            try
            {
                using (StreamWriter file = new StreamWriter(RelativePatchCreate(file_name), append))
                {
                    try
                    {
                        file.WriteLine(str_log);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.TargetSite.ToString() + "\n" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.TargetSite.ToString() + "\n" + ex.Message);
            }
        }
        public static void LogFileWriteNotDateTime(string _str, string file_name, bool append)
        {
            string str_log = _str;

            try
            {
                using (StreamWriter file = new StreamWriter(RelativePatchCreate(file_name), append))
                {
                    try
                    {
                        file.WriteLine(str_log);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.TargetSite.ToString() + "\n" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.TargetSite.ToString() + "\n" + ex.Message);
            }
        }
        #endregion

        #region -Создание относительного пути-
        /// <summary>
        /// Создание относительного пути
        /// </summary>
        public static string RelativePatchCreate(string file_name)
        {
            string patch = Directory.GetCurrentDirectory() + @"\";
            patch += file_name;
            return patch;
        }
        #endregion

        #region -Удаление файла в каталоге приложения-
        /// <summary>
        /// Удаление файла в каталоге приложения
        /// </summary>
        /// <param name="file_name">file_name</param>
        public static void DeleteFile(string file_name)
        {
            try
            {
                File.Delete(RelativePatchCreate(file_name));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.TargetSite.ToString() + "\n" + ex.Message);
            }
        }
        /// <summary>
        /// Удаляет все файлы, расширение которых соот. _paternExtencion в паке _patchFolder.
        /// </summary>
        /// <param name="_patchFolder">путь к папке</param>
        /// <param name="_paternExtencion">патерн для расширения файла</param>
        public static void DeleteAllFile(string _patchFolder, string _paternExtencion)
        {
            string[] filePathes = GetPathFiles(_patchFolder, _paternExtencion);

            foreach (string _patch in filePathes)
            {
                try
                {
                    File.Delete(_patch);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.TargetSite.ToString() + "\n" + ex.Message);
                }
            }
        }
        #endregion

        #region -Получение массива полных путей файлов в катологе "DataFiles"-
        /// <summary>
        /// Получение массива полных путей файлов в катологе "DataFiles", 
        /// в подкаталоге _folderName, в соответствии с _patern
        /// </summary>
        public static string[] GetPathFiles(string _folderName, string _patern)
        {
            string[] dir_map;

            try
            {
                dir_map = Directory.GetFiles(Directory.GetCurrentDirectory() + _folderName, _patern);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            
            return dir_map;
        }
        #endregion

        #region -Регулярные выражения-
        /// <summary>
        /// Возвращает фрагмент comment по правилам patern.
        /// RegularExpressions
        /// </summary>
        public static string GetComment(string comment, string patern)
        {
            string result = "";

            try
            {
                result = Regex.Match(comment, patern).ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.TargetSite + ": " + e.Message);
            }

            return result;
        }
        #endregion

        #region -Время выполнения-
        public static string RutimeMethod(Action _method)
        {
            Stopwatch _stwt = new Stopwatch();
            _stwt.Start();
            _method();
            _stwt.Stop();
            TimeSpan ts = _stwt.Elapsed;

            return String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
        #endregion
    }
}
