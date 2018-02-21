using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    /// <summary>
    /// Представляет методы асинхронного расчета Болинжера
    /// </summary>
    public class CalculationBolinger
    {
        private string patern1 = @"*.dat";
        private string folderOut = SettingsClass.AllTrades_bb + SettingsClass.FolderSBRF;
        private string folderInput = SettingsClass.AllTrades_Inp + SettingsClass.FolderSBRF;

        /// <summary>
        /// Получение входных данных для расчета Болинжера
        /// </summary>
        private async Task<string> GetInputData(SettingsClass _sc)
        {
            TimeSpan _runTime;
            DateTime _startRun = DateTime.Now;

            List<Task> _tasks = new List<Task>();
            string[] filePathes = StaticService.GetPathFiles(folderInput, patern1);

            foreach (string _path in filePathes) // проходим по файлам
            {
                _tasks.Add(TaskMultiParametrBB(_path, _sc));
            }

            await Task.WhenAll(_tasks);

            _runTime = DateTime.Now - _startRun;
            return String.Format("{0:00}:{1:00}:{2:00}.{3:000}", _runTime.Hours, _runTime.Minutes, _runTime.Seconds, _runTime.Milliseconds);
        }

        /// <summary>
        /// Запуск расчета Болинжера по множеству параметров
        /// </summary>
        private async Task TaskMultiParametrBB(string _path, SettingsClass _sc)
        {
            MarketTradesRepository _marketTrad = (MarketTradesRepository)StaticService.Deserializes(_path);
            List<Task> tasks = new List<Task>();

            int countPeriod = _sc.CountPeriodBB_Start;
            double k_stddev;
            string _fileNameInp = Path.GetFileNameWithoutExtension(_path) + "_";

            for (int i = 0; i < _sc.CountStepPeriod; i++)
            {
                k_stddev = _sc.CountStdDevBB_Start;

                for (int ii = 0; ii < _sc.CountStepStdDev; ii++)
                {
                    Task theTask = CreateRepositoryBolinger(_marketTrad, countPeriod, k_stddev, _fileNameInp);
                    tasks.Add(theTask);

                    k_stddev += _sc.StdDevBBStep;
                }
                countPeriod += _sc.PeriodBBStep;
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Расчет Болинжера по _countPeriod и _kstd
        /// </summary>
        private async Task CreateRepositoryBolinger(MarketTradesRepository _mtr, int _countPeriod, double _kstd, string _fileNameInp)
        {
            double High_line = 0;
            double Midl_line = 0;
            double Low_line = 0;

            double price;
            List<double> priceList = new List<double>();
            TradesBolingerRepository _trdBReposit = new TradesBolingerRepository();

            await Task.Run(() =>
            {
                foreach (ParametrMarketTrades item in _mtr)
                {
                    price = (double)item.PriceTrades;

                    priceList.Add(price);

                    if (priceList.Count == _countPeriod)
                    {
                        StaticCalculations.BollingerBands(priceList, _kstd, ref High_line, ref Midl_line, ref Low_line);
                        priceList.RemoveAt(0);
                        _trdBReposit.Add(new ParametrTradesBolinger(item.DateTimeTrades, item.NumberTrades, item.PriceTrades, item.SeccodeTrades, High_line, Low_line, Midl_line));
                    }
                }
                string _fileName = _fileNameInp + StaticService.GenerateKey(_countPeriod, _kstd);
                StaticService.Serializes(_trdBReposit, folderOut + _fileName);
            });
        }

        #region -Диагностика-
        public async Task<string> CalcBBAsync(SettingsClass _sc)
        {
            string _result = await this.GetInputData(_sc);
            StaticService.LogFileWrite("-- CalculationBBAsync = " + _result, "runtime.txt", true);

            return "Расчет закончен.";
        }
        #endregion
    }
}
