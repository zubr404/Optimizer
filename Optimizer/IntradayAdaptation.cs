using Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritms
{
    /// <summary>
    /// Внутредневная адаптация параметров для
    /// поиска экстремумов с учетом дистанции 
    /// между ними.
    /// </summary>
    public class IntradayAdaptation
    {
        public async Task AdaptationAsync()
        {
            string _result_test = String.Empty;

            TimeSpan _runTime;
            DateTime _startRun = DateTime.Now;

            TimeSpan steptime = new TimeSpan(2, 0, 0);
            TimeSpan steptime1 = new TimeSpan(2, 0, 0);
            TimeSpan starttime = new TimeSpan(10, 1, 0).Subtract(steptime1);
            TimeSpan finishtime = starttime.Add(steptime);
            TimeSpan stoptime = new TimeSpan(19, 0, 0);
            decimal otkat = 30;
            decimal distance = 30;
            decimal profit = 30;

            MarketTradesRepository mTR = new MarketTradesRepository();
            mTR = (MarketTradesRepository)StaticService.Deserializes(mTR.GetType().ToString(), mTR);

            ExtremumPrice _extPrice = new ExtremumPrice();  // !!!

            StaticService.DeleteFile("trade_log.txt");
            StaticService.DeleteFile("equ.txt");
            StaticService.DeleteFile("trade_log.txt");
            StaticService.DeleteFile("group_log.txt");

            StaticService.LogFileWriteNotDateTime("EqCurrent\tCountTrades\tCountProfit\tCountLoss\tDrawdown\t_otkat\t_distance\t_profit", "equ.txt", true);
            //StaticService.LogFileWriteNotDateTime("EqCurrent\tDrawdown\t_otkat\t_distance\t_profit", "trade_log.txt", true);

            do
            {
                //-------------------------------------
                finishtime = finishtime.Add(steptime1);
                starttime = finishtime.Subtract(steptime);

                if (finishtime > stoptime)
                {
                    finishtime = stoptime;
                }
                //--------------------------------------


                // I поиск оптимальных параметров
                List<Task<ResultExtremDistance>> tasksResult = new List<Task<ResultExtremDistance>>();
                List<Task<string>> tasksString = new List<Task<string>>();

                List<ResultExtremDistance> resultsExDist = new List<ResultExtremDistance>();

                MarketTradesRepository _mtr_range = ResWhereTime(mTR, starttime, finishtime);
                
                for (int i = (int)otkat; i <= 200; i += 10)
                {
                    for (int ii = i; ii <= 200; ii += 10)
                    {
                        for (int iii = i; iii <= 200; iii += 10)
                        {
                            Task<ResultExtremDistance> theTask = TaskCalculation(_mtr_range, i, ii, iii);
                            tasksResult.Add(theTask);
                        }
                    }
                }

                await Task<ResultExtremDistance>.WhenAll(tasksResult);

                foreach (Task<ResultExtremDistance> item in tasksResult)
                {
                    if (item.Result.otkat > 0)
                    {
                        resultsExDist.Add(item.Result);
                    }
                }

                // II выборка лучших параметров
                ResultExtremDistance _bestResExDist = MyGroupBy(resultsExDist);


                /*
                // III следующий интервал торгуем с выбранными параметрами
                MarketTradesRepository _mtr = ResWhereTime(mTR, finishtime, finishtime.Add(steptime1));

                StaticService.LogFileWriteNotDateTime("\nParametr\t" + _bestResExDist.eqcurrent + "\t" + _bestResExDist.drawdown + "\t" + _bestResExDist.otkat + "\t" + _bestResExDist.distance + "\t" + _bestResExDist.profit, "trade_log.txt", true);

                Task<string> theTaskMarket = TaskCalculationString(_mtr, _extPrice, _bestResExDist.otkat, _bestResExDist.distance, _bestResExDist.profit);
                tasksString.Add(theTaskMarket);
                
                await Task<string>.WhenAll(tasksString);

                foreach (Task<string> item in tasksString)
                {
                    _result_test += item.Result;
                }*/



            } 
            while (finishtime < stoptime);

            _runTime = DateTime.Now - _startRun;
            StaticService.LogFileWriteNotDateTime(String.Format("{0:00}:{1:00}:{2:00}.{3:000}", _runTime.Hours, _runTime.Minutes, _runTime.Seconds, _runTime.Milliseconds), "runtime.txt", true);

            StaticService.LogFileWriteNotDateTime(_result_test, "equ.txt", true);
        }




        // GroupBy
        private ResultExtremDistance MyGroupBy(List<ResultExtremDistance> _inputList)
        {
            if (_inputList.Count == 0 || _inputList == null)
            {
                return new ResultExtremDistance(0, 0, 0, 0, 0);
            }

            StaticService.LogFileWriteNotDateTime("otkat\tdistance\tprofit\tdrawdown\teqcurrent", "group_log.txt", true);

            var otkatGroup = from result in _inputList
                             group result by new { _otkat = result.otkat, _distance = result.distance } into resGroup
                             orderby resGroup.Key._otkat
                             select resGroup;

            //foreach (var item in otkatGroup)
            //{
            //    StaticService.LogFileWriteNotDateTime(item.Key._otkat + " " + item.Key._distance, "group_log.txt", true);
            //    //StaticService.LogFileWriteNotDateTime("\t" + ((int)item.Average(x => x.profit) / 10) * 10 + " " + item.Count(), "group_log.txt", true);
            //    StaticService.LogFileWriteNotDateTime("\t" + item.Min(x=>x.profit) + " " + item.Count(), "group_log.txt", true);
            //}

            //foreach (var item in otkatGroup)
            //{
            //    StaticService.LogFileWriteNotDateTime(item.Key._otkat + " " + item.Key._distance, "group_log.txt", true);

            //    foreach (var item1 in item)
            //    {
            //        StaticService.LogFileWriteNotDateTime("\t" + item1.otkat + " " + item1.distance + " " + item1.profit + " ** " + item1.drawdown, "group_log.txt", true);
            //    }
            //}

            foreach (var item in otkatGroup)
            {
                StaticService.LogFileWriteNotDateTime("\n", "group_log.txt", true);

                foreach (var item1 in item)
                {
                    StaticService.LogFileWriteNotDateTime(item1.otkat + "\t" + item1.distance + "\t" + item1.profit + "\t" + item1.drawdown + "\t" + item1.eqcurrent, "group_log.txt", true);
                }
            }

            //return new ResultExtremDistance(0, 0, otkatGroup.ElementAt(0).Key._otkat, otkatGroup.ElementAt(0).Key._distance, ((int)otkatGroup.ElementAt(0).Average(x => x.profit) / 10) * 10);
            return new ResultExtremDistance(0, 0, otkatGroup.ElementAt(0).Key._otkat, otkatGroup.ElementAt(0).Key._distance, otkatGroup.ElementAt(0).Min(x=>x.profit));
        }



        /// <summary>
        /// Асинхронный вызов CalculationExtremumPrice
        /// </summary>
        private Task<string> TaskCalculationString(MarketTradesRepository mTR, ExtremumPrice _extPrice, decimal i, decimal ii, decimal iii)
        {
            return Task.Run(() =>
            {
                return CalcExtremPriceString(mTR, _extPrice, i, ii, iii, true);
            });
        }
        private Task<ResultExtremDistance> TaskCalculation(MarketTradesRepository mTR, decimal i, decimal ii, decimal iii)
        {
            return Task.Run(() =>
            {
                return CalculationExtremumPrice(mTR, i, ii, iii, false);
            });
        }

        /// <summary>
        /// Вызов поиска экстремумов и вызов поиска экстремумов с дистанцией
        /// </summary>
        private string CalcExtremPriceString(MarketTradesRepository _mTR, ExtremumPrice _extPrice, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            //_extPrice.CreateExtrem(_mTR, _otkat);
            return _extPrice.CreateExtremDistance(_mTR, _otkat, _distance, _profit, _write_trades);
        }
        private ResultExtremDistance CalculationExtremumPrice(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            ExtremumPrice _extPrice = new ExtremumPrice();
            //_extPrice.CreateExtrem(_mTR, _otkat);
            return _extPrice.CreateExtremDistanceResult(_mTR, _otkat, _distance, _profit, _write_trades);
        }

        //---------
        private MarketTradesRepository ResWhereTime(MarketTradesRepository mTR, TimeSpan _start, TimeSpan _finish)
        {
            IEnumerable<ParametrMarketTrades> _whereRes = mTR.Where(x => x.DateTimeTrades.TimeOfDay >= _start && x.DateTimeTrades.TimeOfDay <= _finish);

            MarketTradesRepository _mtr_range = new MarketTradesRepository();
            foreach (ParametrMarketTrades item in _whereRes)
            {
                _mtr_range.Add(item);
            }

            return _mtr_range;
        }

    }
}
