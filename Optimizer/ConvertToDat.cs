using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Optimizer
{
    /// <summary>
    /// Конвертация txt файла с данными всех сделок в dat
    /// </summary>
    public class ConvertToDat
    {
        private string patern1 = @"*.txt";
        private string folder = SettingsClass.AllTrades_Inp + SettingsClass.FolderSBRF;

        private string paternRegEx1 = @" ";

        #region -Constructor-
        
        #endregion

        #region -Method-
        /// <summary>
        /// Синхронная конвертация в .dat
        /// </summary>
        private void ConvertMy()
        {
            StreamReader file;
            string[] filePathes = StaticService.GetPathFiles(folder, patern1);
            
            foreach (string _path in filePathes) // проходим по файлам
            {
                MarketTradesRepository marketTradRepo;
                string _date = String.Empty;
                string _time = String.Empty;
                decimal _price = 0;

                using (file = new StreamReader(_path))
                {
                    string line;
                    marketTradRepo = new MarketTradesRepository();
                    int _id = 0;
                    
                    while ((line = file.ReadLine()) != null) // проходим по стркам
                    {
                        string[] _splites = Regex.Split(line, paternRegEx1);

                        _id++;
                        _date = _splites[0];
                        _time = _splites[1];

                        try
                        {
                            _price = Convert.ToDecimal(_splites[2]);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                         
                        marketTradRepo.Add(new ParametrMarketTrades(_id.ToString(), _date, _time, 0, _price, ""));
                    }
                }

                if (marketTradRepo.Count > 0)
                {
                    StaticService.Serializes(marketTradRepo, folder + marketTradRepo[0].DateTimeTrades.ToShortDateString());
                }
            }
        }

        /// <summary>
        /// Асинхронная конвертация в .dat
        /// </summary>
        /// <returns></returns>
        private async Task<string> ConvertAsync()
        {
            TimeSpan _runTime;
            DateTime _startRun = DateTime.Now;

            List<Task> tasks = new List<Task>();
            List<FileStream> sourceStreams = new List<FileStream>();

            string[] filePathes = StaticService.GetPathFiles(folder, patern1);

            int idTask = 0;

            foreach (string _path in filePathes) // проходим по файлам
            {
                int sourceLength;

                try
                {
                    FileStream sourceStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
                    sourceLength = (int)sourceStream.Length;

                    byte[] _result = new byte[sourceLength];
                    await sourceStream.ReadAsync(_result, 0, sourceLength); // асинх. чтение

                    Task theTask = TaskConvert(_result, idTask++.ToString());
                    sourceStreams.Add(sourceStream);
                    tasks.Add(theTask);
                }
                finally
                {
                    foreach (FileStream _fs in sourceStreams)
                    {
                        _fs.Close();
                    }
                }
            }

            await Task.WhenAll(tasks);

            _runTime = DateTime.Now - _startRun;
            return String.Format("{0:00}:{1:00}:{2:00}.{3:000}", _runTime.Hours, _runTime.Minutes, _runTime.Seconds, _runTime.Milliseconds);
        }

        /// <summary>
        /// Task для конвертации в .dat
        /// </summary>
        private Task TaskConvert(byte[] _result, string _id)
        {
            StaticService.LogFileWrite(DateTime.Now.ToLongTimeString() + "  " + _id, "runtime.txt", true);

            return Task.Run(() => 
            {
                string _text = Encoding.UTF8.GetString(_result);
                string[] _separator = new string[3] { "\r\n", "\n", "\r" };
                string[] _mapStr = _text.Split(_separator, StringSplitOptions.RemoveEmptyEntries);

                string _date = String.Empty;
                string _time = String.Empty;
                decimal _price = 0;

                MarketTradesRepository marketTradRepo = new MarketTradesRepository();
                int _iD = 0;

                foreach (string _line in _mapStr)
                {
                    string[] _splites = Regex.Split(_line, paternRegEx1);

                    _iD++;
                    _date = _splites[0];
                    _time = _splites[1];

                    try
                    {
                        _price = Convert.ToDecimal(_splites[2]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    marketTradRepo.Add(new ParametrMarketTrades(_iD.ToString(), _date, _time, 0, _price, ""));
                }

                if (marketTradRepo.Count > 0)
                {
                    StaticService.Serializes(marketTradRepo, folder + marketTradRepo[0].DateTimeTrades.ToShortDateString());
                }

                StaticService.LogFileWrite(DateTime.Now.ToLongTimeString() + "  " + _id, "runtime.txt", true);
            });
        }
        #endregion

        #region -Диагностика-
        public void ConvertMyDiagnostics()
        {
            string _runtime = StaticService.RutimeMethod(() => 
            { 
                this.ConvertMy();
            });
            StaticService.LogFileWrite("-- ConvertMy = " +_runtime, "runtime.txt", true);
        }

        public async void ConvertAsyncDiagnostic()
        {
            string _result = await this.ConvertAsync();

            StaticService.LogFileWrite("-- ConvertAsync = " + _result, "runtime.txt", true);
        }
        #endregion
    }
}
