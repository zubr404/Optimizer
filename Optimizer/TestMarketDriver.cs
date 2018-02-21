using Algoritms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Optimizer
{
    /// <summary>
    /// Тест торговли. Алгоритм.
    /// </summary>
    public class TestMarketDriver
    {
        private const string patern1 = @"*.dat";

        private string folderOutTestResult = SettingsClass.TestResult + SettingsClass.FolderSBRF;
        private string folderOutTestTradesSimple = SettingsClass.TestTrades + SettingsClass.FolderSBRF + SettingsClass.FolderSimple;
        private string folderOutTestTradesOPT = SettingsClass.TestTrades + SettingsClass.FolderSBRF + SettingsClass.FolderOPT;
        private string folderInput = SettingsClass.AllTrades_bb + SettingsClass.FolderSBRF;

        string runTime;

        TestResultRepositiry testResultRepo;

        #region -Algoritm-
        /// <summary>
        /// Открываем лонги, если цена выше самой верхней линии.
        /// Открываем шорты, если цена ниже самой нижней линии.
        /// Закрытие: переворачиваемся.
        /// </summary>
        private Task TrendAlgoritmTest(string _path)
        {
            return Task.Run(() => 
            {
                TradesBolingerRepository _trdBRepo = null;
                string file_name = "test_result.txt";
                try
                {
                    _trdBRepo = (TradesBolingerRepository)StaticService.Deserializes(_path);
                }
                catch (Exception)
                {
                    StaticService.LogFileWrite(_path, "error_log.txt", true);
                }
                
                string _key = Path.GetFileNameWithoutExtension(_path);
                string _dateRes = StaticService.GetComment(_key, SettingsClass.PaternDate1);
                string _settBB = StaticService.GetComment(_key, SettingsClass.Patern);

                TestTradesCollection testTradColl = new TestTradesCollection();
                TestTradesCollection testTradCollOPT = new TestTradesCollection();

                int tp = 0;
                double profitPortfolio = 0; // прибыль портфеля на каждую сделку
                int countTrades = 0;
                double maxProfit = 0;
                double minProfit = 0;
                int countProfitTrades = 0;
                int countLossTrades = 0;

                // OPT
                double profitPortfolioOpt = 0;
                int countTradesOpt = 0;
                double maxProfitOpt = 0;
                double minProfitOpt = 0;
                int countProfitTradesOpt = 0;
                int countLossTradesOpt = 0;
                // end opt

                StaticService.LogFileWrite("\n         ---------" + _key + "---------", file_name, true);

                if (_trdBRepo != null)
                {
                    foreach (ParametrTradesBolinger pcT in _trdBRepo)
                    {
                        //StaticClassService.LogFileWrite(pcT.DateTimeTrades + "\t" + pcT.NumberTrades + "\t" + pcT.PriceTrades + "\t" + pcT.SeccodeTrades + "\t" + pcT.LineUp + "\t" + pcT.LineDown + "\t" + pcT.LineMidl, file_name, true);

                        DateTime dateTimeTrade = pcT.DateTimeTrades;

                        if (dateTimeTrade.TimeOfDay < new TimeSpan(19, 0, 0))
                        {
                            double price = (double)pcT.PriceTrades;

                            //---Здесь вызываем Класс нужного алгоритма---
                            Algoritms.BollingerCrossing.Algoritm(pcT, testTradColl, testTradCollOPT, file_name, price, dateTimeTrade, ref tp, ref countTrades, ref countTradesOpt, ref profitPortfolio, ref profitPortfolioOpt, ref maxProfit, ref maxProfitOpt, ref minProfit, ref minProfitOpt, ref countProfitTrades, ref countProfitTradesOpt, ref countLossTrades, ref countLossTradesOpt);
                            //--------------------------------------------
                        }
                    }
                }
                
                // save result
                testResultRepo.Add(new ParametrTestResult(_key, _dateRes, _settBB, profitPortfolio, countTrades, maxProfit, minProfit, countProfitTrades, countLossTrades));
                testResultRepo.Add(new ParametrTestResult(_key + "OPT",_dateRes, _settBB, profitPortfolioOpt, countTradesOpt, maxProfitOpt, minProfitOpt, countProfitTradesOpt, countLossTradesOpt)); // Opt

                // serialize TestTradesCollection end TestTradesCollection'OPT'
                StaticService.Serializes(testTradColl, folderOutTestTradesSimple + _key);
                StaticService.Serializes(testTradCollOPT, folderOutTestTradesOPT + _key);
            });
        }
        #endregion

        #region -Method-
        /// <summary>
        /// Получение входных данных.
        /// </summary>
        private async Task<TestResultRepositiry> GetInputData()
        {
            runTime = String.Empty;
            TimeSpan _runTime;
            DateTime _startRun = DateTime.Now;

            List<Task> _tasks = new List<Task>();
            string[] filePathes = StaticService.GetPathFiles(folderInput, patern1);

            testResultRepo = new TestResultRepositiry();

            // очищаем папки
            StaticService.DeleteAllFile(folderOutTestResult, patern1);
            StaticService.DeleteAllFile(folderOutTestTradesSimple, patern1);
            StaticService.DeleteAllFile(folderOutTestTradesOPT, patern1);

            foreach (string _path in filePathes) // проходим по файлам
            {
                _tasks.Add(TrendAlgoritmTest(_path));
            }

            await Task.WhenAll(_tasks);

            StaticService.Serializes(testResultRepo, folderOutTestResult + "!_actul_result");

            _runTime = DateTime.Now - _startRun;
            runTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", _runTime.Hours, _runTime.Minutes, _runTime.Seconds, _runTime.Milliseconds);

            return testResultRepo;
        }

        

        

        
        #endregion

        #region -Diagnostic-
        public async Task<TestResultRepositiry> TestAsync()
        {
            TestResultRepositiry _result = await this.GetInputData();
            StaticService.LogFileWrite("-- TestAsync = " + runTime, "runtime.txt", true);

            return _result;
        }
        #endregion

        #region-Тест по сегодняшним сделкам-
        public void TestTradesToday()
        {
            //string filename = "today_trades.txt";
            string fileSummary = "summary.xls";
            string fileVariability = "variability.txt";
            string fileBrutto = "brutto.txt";
            string fileBruttoFiltr = "brutto_filtr.txt";
            string fileBruttoFiltrPro = "brutto_filtr_pro.txt";
            string fileTrades = "trades.txt";
                        
            MarketTradesRepository mTR = new MarketTradesRepository();
            TradesBolingerRepository tBR = new TradesBolingerRepository();
            mTR = (MarketTradesRepository)StaticService.Deserializes(mTR.GetType().ToString(), mTR);
            tBR = (TradesBolingerRepository)StaticService.Deserializes("09.11.2016_070008", tBR);

            //for (int i = 3; i <= 3; i++)
            //{
            //    CalculationExtremumPrice(mTR, i, filename);
            //}

            StaticService.DeleteFile(fileSummary);
            StaticService.DeleteFile(fileVariability);
            StaticService.DeleteFile(fileTrades);
            StaticService.DeleteFile(fileBruttoFiltr);
            StaticService.DeleteFile(fileBruttoFiltrPro);
            StaticService.LogFileWriteNotDateTime("Profit sett\tProfit\tCount profit\tCount loss\tCount all", fileSummary, true);

            // оптимизация по профиту
            for (int i = 5; i <= 5; i++)
            {
                CalculationExtremumProfit(mTR, tBR, 5, i, fileSummary, fileTrades, fileVariability, fileBrutto, fileBruttoFiltr, fileBruttoFiltrPro);
            }
        }

        #region -Асинхронная оптимизация-
        public async Task TestTradesTodayAsync()
        {
            string _result_test = String.Empty;

            TimeSpan _runTime;
            DateTime _startRun = DateTime.Now;

            decimal otkat = 30;
            decimal distance = 30;
            decimal profit = 30;

            List<Task<string>> tasks = new List<Task<string>>();

            MarketTradesRepository mTR = new MarketTradesRepository();
            mTR = (MarketTradesRepository)StaticService.Deserializes(mTR.GetType().ToString(), mTR);

            StaticService.DeleteFile("trade_log.txt");
            StaticService.DeleteFile("equ.txt");

            StaticService.LogFileWriteNotDateTime("EqCurrent\tCountTrades\tCountProfit\tCountLoss\tDrawdown\t_otkat\t_distance\t_profit", "equ.txt", true);

            for (int i = (int)otkat; i <= 200; i+=10)
            {
                for (int ii = i; ii <= 200; ii+=10)
                {
                    for (int iii = i; iii <= 200; iii += 10)
                    {
                        //ExtremumPrice _extPrice2 = new ExtremumPrice();
                        //CalculationExtremumPrice1(mTR, _extPrice2, i, ii, iii, false);

                        Task<string> theTask = TaskCalculation(mTR, i, ii, iii);
                        tasks.Add(theTask);
                    }
                }
            }

            await Task<string>.WhenAll(tasks);

            foreach (Task<string> item in tasks)
            {
                _result_test += item.Result;
            }


            _runTime = DateTime.Now - _startRun;
            StaticService.LogFileWriteNotDateTime(String.Format("{0:00}:{1:00}:{2:00}.{3:000}", _runTime.Hours, _runTime.Minutes, _runTime.Seconds, _runTime.Milliseconds), "runtime.txt", true);

            StaticService.LogFileWriteNotDateTime(_result_test, "equ.txt", true);
        }

        /// <summary>
        /// Асинхронный вызов CalculationExtremumPrice
        /// </summary>
        private Task<string> TaskCalculation(MarketTradesRepository mTR, decimal i, decimal ii, decimal iii)
        {
            return Task.Run(() =>
            {
                return CalculationExtremumPrice(mTR, i, ii, iii, false);
            });
        }
        #endregion

        /// <summary>
        /// Прогон по единичным параметрам
        /// </summary>
        public ExtremumPrice TestTradesToday1()
        {
            decimal otkat = 80;
            decimal distance = 190;
            decimal profit = 170;

            MarketTradesRepository mTR = new MarketTradesRepository();
            mTR = (MarketTradesRepository)StaticService.Deserializes(mTR.GetType().ToString(), mTR);

            StaticService.DeleteFile("trade_log.txt");
            StaticService.DeleteFile("equ.txt");

            return CalculationExtremumPrice1(mTR, otkat, distance, profit, true);
        }


        // Вызов поиска экстремумов и вызов поиска экстремумов с дистанцией
        private string CalculationExtremumPrice(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            ExtremumPrice _extPrice = new ExtremumPrice();
            _extPrice.CreateExtrem(_mTR, _otkat);
            return _extPrice.CreateExtremDistance(_mTR, _otkat, _distance, _profit, _write_trades);
        }
        private ExtremumPrice CalculationExtremumPrice1(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            ExtremumPrice _extPrice = new ExtremumPrice();
            _extPrice.CreateExtrem(_mTR, _otkat);
            _extPrice.CreateExtremDistance(_mTR, _otkat, _distance, _profit, _write_trades);

            return _extPrice;
        }
        //-------------------------------------------


        //
        private void CalculationExtremumProfit(MarketTradesRepository _mTR, TradesBolingerRepository _tBR, decimal _otkat, decimal _profit, string _fileSummary, string _fileTrades, string _filevariability, string _fileBrutto, string _fileBruttoFiltr, string _fileBruttoFiltrPro)
        {
            ExtremumProfit extPrice = new ExtremumProfit();
            Queue<decimal> BruttoQue = new Queue<decimal>();

            decimal exchange_fee = 0.47m; // биржевой сбор SBRF 0.47
            decimal brokerage_commission = 0.59m; // комиссия брокера 0.59
            decimal spred = 3m;

            // --одновременно можно только один
            //extPrice.CreateExtrem(_mTR, _otkat, _profit);
            extPrice.CreateExtrem(_tBR, _otkat, _profit);
            //-------

            //----
            decimal _buyMany = 0;
            decimal _selMany = 0;
            decimal _bruttoGlobal = 0;
            int _countProfit = 0;
            int _countLoss = 0;
            int _countAll = 0;
            int _variability = 0;

            StaticService.LogFileWriteNotDateTime("Profit setting <" + _profit + ">", _fileTrades, true);
            StaticService.LogFileWriteNotDateTime("Profit setting <" + _profit + ">", _filevariability, true);
            StaticService.LogFileWriteNotDateTime("Profit setting <" + _profit + ">", _fileBrutto, true);
            StaticService.LogFileWriteNotDateTime("Profit setting <" + _profit + ">", _fileBruttoFiltr, true);
            StaticService.LogFileWriteNotDateTime("Profit setting <" + _profit + ">", _fileBruttoFiltrPro, true);

            foreach (var item in extPrice.TradesQueue)
            {
                StaticService.LogFileWriteNotDateTime(item.ToString(), _fileTrades, true);

                double _num = item.NumberTrades;
                Operation _oper = item.Operation;
                decimal _price = item.PriceTrades;
                decimal _brutto = 0;

                if (_oper == Operation.Buuy)
                {
                    _buyMany = _price;
                }
                else
                {
                    _selMany = _price;
                }

                if (_buyMany != 0 && _selMany != 0)
                {
                    _brutto = _selMany - _buyMany;
                    _bruttoGlobal += _brutto;
                    _buyMany = 0;
                    _selMany = 0;

                    if (_brutto > 0)
                    {
                        _countProfit++;
                    }
                    else
                    {
                        _countLoss++;
                    }

                    _countAll++;

                    BruttoQue.Enqueue(_brutto);
                    StaticService.LogFileWriteNotDateTime(_brutto.ToString(), _fileBrutto, true);

                    //
                    VariablityCalc(_brutto, _filevariability, ref _variability);
                    //-----
                }
            }

            // фильтрация pro
            for (int i = 1; i <= 10; i++)
            {
                Queue<decimal> resultPositive = extPrice.FiltrBruttoPro(BruttoQue, i, 0);
                Queue<decimal> resultNegative = extPrice.FiltrBruttoPro(BruttoQue, 0, i);

                decimal _sum = 0;

                foreach (decimal item in resultPositive)
                {
                    _sum += item;
                }

                StaticService.LogFileWriteNotDateTime("FILTR_PRO + <" + i + "> = " + _sum.ToString(), _fileBruttoFiltrPro, true);

                _sum = 0;

                foreach (decimal item in resultNegative)
                {
                    _sum += item;
                }

                StaticService.LogFileWriteNotDateTime("FILTR_PRO - <" + i + "> = " + _sum.ToString(), _fileBruttoFiltrPro, true);
            }


            // циклическая фильтрация
            StaticService.LogFileWriteNotDateTime("SUMMARY_INP = " + _bruttoGlobal.ToString(), _fileBruttoFiltr, true);
            Queue<decimal> _filtr_result = extPrice.FiltrBrutto(BruttoQue);

            for (int i = 0; i < 5; i++)
            {
                decimal _sum = 0;

                foreach (decimal item in _filtr_result)
                {
                    _sum += item;
                }
                StaticService.LogFileWriteNotDateTime("SUMMARY_" + i + " = " + _sum.ToString(), _fileBruttoFiltr, true);

                _filtr_result = extPrice.FiltrBrutto(_filtr_result);
            }

            /*
            // фильтруем brutto и выводим в файл
            Queue<decimal> _filtrBrut_I = extPrice.FiltrBrutto(BruttoQue);
            foreach (decimal item in _filtrBrut_I)
            {
                StaticService.LogFileWriteNotDateTime(item.ToString(), _fileBruttoFiltr, true);
            }

            // фильтруем _filtrBrut и выводим в файл
            StaticService.LogFileWriteNotDateTime("--------LEVEL II", _fileBruttoFiltr, true);
            Queue<decimal> _filtrBrut_II = extPrice.FiltrBrutto(_filtrBrut_I);
            foreach (decimal item in _filtrBrut_II)
            {
                StaticService.LogFileWriteNotDateTime(item.ToString(), _fileBruttoFiltr, true);
            }

            // summary brutto_filtr
            decimal _sum_brut_filtr = 0;
            foreach (decimal item in _filtrBrut_I)
            {
                _sum_brut_filtr += item;
            }
            StaticService.LogFileWriteNotDateTime("SUMMARY_I = " + _sum_brut_filtr.ToString(), _fileBruttoFiltr, true);

            _sum_brut_filtr = 0;
            foreach (decimal item in _filtrBrut_II)
            {
                _sum_brut_filtr += item;
            }
            StaticService.LogFileWriteNotDateTime("SUMMARY_II = " + _sum_brut_filtr.ToString(), _fileBruttoFiltr, true);
            //---------------------------
            */
            //StaticService.LogFileWriteNotDateTime(_bruttoGlobal + " <" + _profit + ">", _filename, true);
            StaticService.LogFileWriteNotDateTime(_profit + "\t" + _bruttoGlobal + "\t" + _countProfit + "\t" + _countLoss + "\t" + _countAll, _fileSummary, true);
        }


        // Просто по экстремумам
        private void CalculationExtremumPrice(MarketTradesRepository _mTR, decimal _otkat, string _filename)
        {
            ExtremumPrice extPrice = new ExtremumPrice();

            decimal exchange_fee = 0.47m; // биржевой сбор SBRF 0.47
            decimal brokerage_commission = 0.59m; // комиссия брокера 0.59
            decimal spred = 3m;

            // находим екстремумы
            extPrice.CreateExtrem(_mTR, _otkat);
            //StaticService.LogFileWrite("----------------Extremums--------------------", _filename, true);
            //foreach (var item in extPrice.Extremums)
            //{
            //    StaticService.LogFileWriteNotDateTime(item.Date_Time + "\t" + item.PriceHigh + "\t" + item.PriceLow, _filename, true);
            //}

            // считаем между ними дельту
            //StaticService.LogFileWrite("\n", filename, true);
            //StaticService.LogFileWriteNotDateTime("----------------считаем между ними дельту--------------------", _filename, true);
            extPrice.DeltaExtr();
            foreach (var item in extPrice.DeltaExtremums)
            {
                StaticService.LogFileWriteNotDateTime(Math.Abs(item).ToString(), "delta_log.txt", true);
            }

            StaticService.LogFileWriteNotDateTime("****************ОТКАТ <" + _otkat + ">****************", _filename, true);

            //&??????????????????
            for (decimal i = _otkat * 2; i <= _otkat * 8; i++)
            {
                decimal criticalSum = 0;

                // фильтруeм дельты
                StaticService.LogFileWriteNotDateTime("----------------фильтруeм дельты <" + i + ">--------------------", _filename, true);
                extPrice.FiltrDelta(i);
                foreach (var item in extPrice.FiltrDeltas)
                {
                    StaticService.LogFileWriteNotDateTime(Math.Abs(item).ToString(), _filename, true);

                    criticalSum += Math.Abs(item) - (_otkat * 2 + TransactionCosts(exchange_fee, brokerage_commission, spred) * 2);
                }
                StaticService.LogFileWriteNotDateTime(criticalSum.ToString(), _filename, true);
            }
        }

        
        #endregion

        /// <summary>
        /// Через какое количество последовательных прибылей/убытков происходит смена
        /// </summary>
        private void VariablityCalc(decimal _brutto, string _filename, ref int _variability)
        {
            if (Math.Sign(_brutto) != Math.Sign(_variability) & _variability != 0)
            {
                StaticService.LogFileWriteNotDateTime(_variability.ToString(), _filename, true);
                if (_brutto > 0)
                {
                    _variability = 1;
                    return;
                }
                else
                {
                    _variability = -1;
                    return;
                }
            }

            if ((_brutto > 0 && _variability > 0) | (_brutto > 0 && _variability == 0))
            {
                _variability++;
                return;
            }
            if ((_brutto < 0 && _variability < 0) | (_brutto < 0 && _variability == 0))
            {
                _variability--;
                return;
            }
        }

        /// <summary>
        /// Тарaнзакционные издержки в одну сторону
        /// </summary>
        private decimal TransactionCosts(decimal _exchange_fee, decimal _brokerage_commission, decimal _spred)
        {
            return _exchange_fee + _brokerage_commission + _spred / 2;
        }
    }
}
