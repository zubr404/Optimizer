using System;
using Optimizer;
using System.Collections;
using System.Collections.Generic;
using AccountResult;

namespace Algoritms
{
    /// <summary>
    /// Открываем лонги, если цена выше самой верхней линии.
    /// Открываем шорты, если цена ниже самой нижней линии.
    /// Закрытие: переворачиваемся.
    /// </summary>
    public class BollingerCrossing
    {
        static double price_buy = 0;
        static double price_sell = 0;

        static double lineUp;
        static double lineDown;
        static double lineMidl;

        // OPT
        static bool longProfit = false;
        static bool shortProfit = false;
        // end opt

        public static void Algoritm(ParametrTradesBolinger pcT, TestTradesCollection testTradColl, TestTradesCollection testTradCollOPT, string file_name, double price, DateTime dateTimeTrade, ref int tp, ref int countTrades, ref int countTradesOpt, ref double profitPortfolio, ref double profitPortfolioOpt, ref double maxProfit, ref double maxProfitOpt, ref double minProfit, ref double minProfitOpt, ref int countProfitTrades, ref int countProfitTradesOpt, ref int countLossTrades, ref int countLossTradesOpt)
        {
            
            double profit = 0; // прибыль самой сделки
            double qty = 0;

            lineUp = pcT.LineUp;
            lineDown = pcT.LineDown;
            lineMidl = pcT.LineMidl;

            // long
            if (tp <= 0 && price > lineUp)
            {
                price_buy = price + SettingsClass.slipPrice;
                profit = StaticCalculations.CalcProfit(price_buy, price_sell);

                qty = Math.Abs(tp) + 1; //--1--
                tp += Math.Abs(tp) + 1; //--2--

                StaticCalculations.CalcParametrTest(profit, dateTimeTrade, ref countTrades, ref profitPortfolio, ref maxProfit, ref minProfit, ref countProfitTrades, ref countLossTrades);

                testTradColl.Add(new ParametrTestTrades(pcT.NumberTrades, dateTimeTrade, price_buy, qty, Operation.Buuy, profitPortfolio));

                // log
                StaticService.LogFileWrite("-----" + "\t" + pcT.NumberTrades + "\t" + dateTimeTrade + "\t" + price_buy + "\t" + qty + "\t" + Operation.Buuy + " \t" + profit + "\t" + profitPortfolio + "\tlongProfit: " + longProfit + "\tshortProfit: " + shortProfit + "\t level_0", file_name, true);

                //---Opt---
                //if (StaticOptimizer.PsevdoRealTrades(profit, dateTimeTrade, ref counter_prof, ref counter_loss))
                //{
                //    CalcParametrTest(profit, dateTimeTrade, ref countTradesOpt, ref profitPortfolioOpt, ref maxProfitOpt, ref minProfitOpt, ref countProfitTradesOpt, ref countLossTradesOpt);
                //    testTradCollOPT.Add(new ParametrTestTrades(pcT.NumberTrades, dateTimeTrade, price_buy, qty, Operation.Buy, profitPortfolioOpt));

                //    // log Opt
                //    StaticService.LogFileWrite("-----" + "\t" + pcT.NumberTrades + "\t" + dateTimeTrade + "\t" + price_buy + "\t" + qty + "\t" + Operation.Buy + "\t" + profitPortfolioOpt + "\t level_1", file_name, true);
                //}

                if (shortProfit)
                {
                    StaticCalculations.CalcParametrTest(profit, dateTimeTrade, ref countTradesOpt, ref profitPortfolioOpt, ref maxProfitOpt, ref minProfitOpt, ref countProfitTradesOpt, ref countLossTradesOpt);
                    testTradCollOPT.Add(new ParametrTestTrades(pcT.NumberTrades, dateTimeTrade, price_buy, qty, Operation.Buuy, profitPortfolioOpt));

                    // log Opt
                    StaticService.LogFileWrite("-----" + "\t" + pcT.NumberTrades + "\t" + dateTimeTrade + "\t" + price_buy + "\t" + qty + "\t" + Operation.Buuy + " \t" + profit + "\t" + profitPortfolioOpt + "\tshortProfit: " + shortProfit + "\t level_1", file_name, true);
                }

                StaticOptimizer.PsevdoRealTrades(profit, false, ref longProfit, ref shortProfit);

                //---end Opt---
            }

            // short
            if (tp >= 0 && price < lineDown)
            {
                price_sell = price - SettingsClass.slipPrice;
                profit = StaticCalculations.CalcProfit(price_buy, price_sell);

                qty = Math.Abs(tp) + 1; //--1--
                tp -= Math.Abs(tp) + 1; //--2--

                StaticCalculations.CalcParametrTest(profit, dateTimeTrade, ref countTrades, ref profitPortfolio, ref maxProfit, ref minProfit, ref countProfitTrades, ref countLossTrades);

                testTradColl.Add(new ParametrTestTrades(pcT.NumberTrades, dateTimeTrade, price_sell, qty, Operation.Sell, profitPortfolio));

                // log
                StaticService.LogFileWrite("-----" + "\t" + pcT.NumberTrades + "\t" + dateTimeTrade + "\t" + price_sell + "\t" + qty + "\t" + Operation.Sell + "\t" + profit + "\t" + profitPortfolio + "\tlongProfit: " + longProfit + "\tshortProfit: " + shortProfit + "\t level_0", file_name, true);

                // Opt
                //if (StaticOptimizer.PsevdoRealTrades(profit, dateTimeTrade, ref counter_prof, ref counter_loss))
                //{
                //    CalcParametrTest(profit, dateTimeTrade, ref countTradesOpt, ref profitPortfolioOpt, ref maxProfitOpt, ref minProfitOpt, ref countProfitTradesOpt, ref countLossTradesOpt);
                //    testTradCollOPT.Add(new ParametrTestTrades(pcT.NumberTrades, dateTimeTrade, price_sell, qty, Operation.Sell, profitPortfolioOpt));

                //    // log Opt
                //    StaticService.LogFileWrite("-----" + "\t" + pcT.NumberTrades + "\t" + dateTimeTrade + "\t" + price_sell + "\t" + qty + "\t" + Operation.Sell + "\t" + profitPortfolioOpt + "\t level_1", file_name, true);

                //}

                if (longProfit)
                {
                    StaticCalculations.CalcParametrTest(profit, dateTimeTrade, ref countTradesOpt, ref profitPortfolioOpt, ref maxProfitOpt, ref minProfitOpt, ref countProfitTradesOpt, ref countLossTradesOpt);
                    testTradCollOPT.Add(new ParametrTestTrades(pcT.NumberTrades, dateTimeTrade, price_sell, qty, Operation.Sell, profitPortfolioOpt));

                    // log Opt
                    StaticService.LogFileWrite("-----" + "\t" + pcT.NumberTrades + "\t" + dateTimeTrade + "\t" + price_sell + "\t" + qty + "\t" + Operation.Sell + "\t" + profit + "\t" + profitPortfolioOpt + "\tlongProfit: " + longProfit + "\t level_1", file_name, true);
                }

                StaticOptimizer.PsevdoRealTrades(profit, true, ref longProfit, ref shortProfit);

                //---end Opt---
            }
        }
    }


    /// <summary>
    /// Пока только лонги.
    /// Открытие: если цена поднялась от минимума на величину большую Н.
    /// Закрытие: если цена опустилась от максимума больше, чем на 
    /// величину Н.
    /// Стоп-лосс: ниже минимума, по которому открывались.
    /// </summary>
    public class BackDraft
    {
        public static void Algoritm(TestTradesCollection testTradColl, TestTradesCollection testTradCollOPT, string file_name, DateTime dateTimeTrade, double price, ref int tp, ref int countTrades, ref double profitPortfolio, ref double maxProfit, ref double minProfit, ref int countProfitTrades, ref int countLossTrades)
        {

        }
    }

    /// <summary>
    /// Открытие:
    ///     - лонги: по рынку, как только нашли минимум
    ///     - шорты: по рынку, как только нашли максимум
    /// Закрытие:
    ///     - лонги/шорты лимитом с заданным профитом
    /// </summary>
    public class ExtremumProfit
    {
        Queue<ParamTradForExtremProf> tradesQueue;

        int position;
        decimal pricePos;
        decimal stopPrice;
        int num_trades;

        decimal high;
        decimal low;
        DateTime datetime;

        public ExtremumProfit()
        {
            tradesQueue = new Queue<ParamTradForExtremProf>();
        }

        //---Properti---
        public Queue<ParamTradForExtremProf> TradesQueue
        {
            get { return tradesQueue; }
        }
        //--------------

        /// <summary>
        /// Запуск поиска экстремумов
        /// </summary>
        public void CreateExtrem(MarketTradesRepository _mTR, decimal _otkat, decimal _profit)
        {
            position = 0;
            num_trades = 0;

            foreach (var item in _mTR)
            {
                this.ExtremumSearch(item.PriceTrades, item.DateTimeTrades, _otkat, _profit);
            }
        }
        public void CreateExtrem(TradesBolingerRepository _tBR, decimal _otkat, decimal _profit)
        {
            position = 0;
            num_trades = 0;

            foreach (var item in _tBR)
            {
                this.ExtremumSearch(item.PriceTrades, item.DateTimeTrades, item.LineUp, item.LineMidl, item.LineDown, _otkat, _profit);
            }
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

        /// <summary>
        /// Поиск экстремумов
        /// </summary>
        private void ExtremumSearch(decimal _price, DateTime _datetime, decimal _otkat, decimal _profit)
        {
            if (high == 0 && low == 0)
            {
                high = _price;
                low = _price;
            }

            if (high > 0 && _price > high)
            {
                high = _price;
                datetime = _datetime;
            }

            if (low > 0 && _price < low)
            {
                low = _price;
                datetime = _datetime;
            }

            if (high > 0 && high - _price > _otkat) // нашли локальный максимум
            {
                WriteTradesMyOpen(_datetime, low, high, _shortPrice: _price);
                low = _price;
                high = -1;
            }

            if (low > 0 && _price - low > _otkat) // нашли локальный минимум
            {
                WriteTradesMyOpen(_datetime, low, high, _longPrice: _price);
                high = _price;
                low = -1;
            }

            WriteTradesMyClose(_datetime, _price, _profit);
        }

        /// <summary>
        /// Экстремумы с учетом болинжера
        /// </summary>
        private void ExtremumSearch(decimal _price, DateTime _datetime, double _bolingerHigh, double _bolingerMidl, double _bolingerLow, decimal _otkat, decimal _profit)
        {
            if (high == 0 && low == 0)
            {
                high = _price;
                low = _price;
            }

            if (high > 0 && _price > high)
            {
                high = _price;
                datetime = _datetime;
            }

            if (low > 0 && _price < low)
            {
                low = _price;
                datetime = _datetime;
            }

            if (high > 0 && high - _price > _otkat) // нашли локальный максимум
            {
                if (high > (decimal)_bolingerHigh && _bolingerHigh > 0)
                {
                    WriteTradesMyOpen(_datetime, low, high, _shortPrice: _price);
                }
                low = _price;
                high = -1;
            }

            if (low > 0 && _price - low > _otkat) // нашли локальный минимум
            {
                if (low < (decimal)_bolingerLow && _bolingerLow > 0)
                {
                    WriteTradesMyOpen(_datetime, low, high, _longPrice: _price);
                }
                high = _price;
                low = -1;
            }

            WriteTradesMyClose(_datetime, _price, _profit);
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-

        /// <summary>
        /// Запись сделок на открытие по сигналу. Больше 0 должен быть только один параметр:
        /// _longPrice или _shortPrice
        /// </summary>
        private void WriteTradesMyOpen(DateTime _dt, decimal _low, decimal _high, decimal _longPrice = 0, decimal _shortPrice = 0)
        {
            if (position == 0)
            {
                num_trades++;

                if (_longPrice > 0)   // open long
                {
                    tradesQueue.Enqueue(new ParamTradForExtremProf( _dt, num_trades, _longPrice, Operation.Buuy, 1, "sbrf"));
                    position++;
                    pricePos = _longPrice;
                    stopPrice = _low;
                }

                if (_shortPrice > 0)  // open short
                {
                    tradesQueue.Enqueue(new ParamTradForExtremProf(_dt, num_trades, _shortPrice, Operation.Sell, 1, "sbrf"));
                    position--;
                    pricePos = _shortPrice;
                    stopPrice = _high;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void WriteTradesMyClose(DateTime _dt, decimal _price, decimal _profit)
        {
            if (position != 0)
            {
                if (position > 0)   // close long
                {
                    if (_price > pricePos + _profit | _price < stopPrice)
                    {
                        tradesQueue.Enqueue(new ParamTradForExtremProf(_dt, num_trades, _price, Operation.Sell, 1, "sbrf"));
                        position--;
                        pricePos = 0;
                    }
                }

                if (position < 0)   // close short
                {
                    if (_price < pricePos - _profit | _price > stopPrice)
                    {
                        tradesQueue.Enqueue(new ParamTradForExtremProf(_dt, num_trades, _price, Operation.Buuy, 1, "sbrf"));
                        position++;
                        pricePos = 0;
                    }
                }
            }
        }


        /// <summary>
        /// Фильтруем brutto
        /// </summary>
        public Queue<decimal> FiltrBrutto(Queue<decimal> _queue)
        {
            bool positive = false;
            Queue<decimal> value = new Queue<decimal>();

            foreach (var item in _queue)
            {
                if (positive)
                {
                    value.Enqueue(item);
                }

                if (item > 0)
                {
                    positive = true;
                }
                else
                {
                    positive = false;
                }
            }

            return value;
        }

        /// <summary>
        /// Фильтруем брутто взависимости от количества прибыльных/убыточных результатов
        /// </summary>
        public Queue<decimal> FiltrBruttoPro(Queue<decimal> _queue, int _positive_sett, int _negative_sett)
        {
            int positive = 0;
            int negative = 0;
            Queue<decimal> value = new Queue<decimal>();

            foreach (var item in _queue)
            {
                if (positive >= _positive_sett && negative >= _negative_sett)
                {
                    value.Enqueue(item);
                }

                if (item > 0)
                {
                    positive++;
                    negative = 0;
                }
                else
                {
                    negative++;
                    positive = 0;
                }
            }

            return value;
        }
    }

    /*
     -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
     */

    /// <summary>
    /// Поиск локальных минимумов/максимумов
    /// </summary>
    public class ExtremumPrice : EquityChange
    {
        Queue<ExtremumPriceParametr> extremums;
        Queue<ExtremumPriceParametr> extremums_distance;
        Queue<decimal> deltaExtremums;
        Queue<decimal> filtrDeltas;

        decimal high;
        decimal low;
        decimal high1;
        decimal low1;
        decimal highlast;       // последний максимум
        decimal lowlast;        // последний минимум
        decimal? high_dist_last; // последний максимум с дистанцией
        decimal? low_dist_last;  // последний минимум с дистанцией
        DateTime datetime;

        ///если  1 - то был последний максимум
        ///если -1 - то был последний минимум
        int type_current_extremum;
        decimal high_position;  // максимум, при котором открыта позиция
        decimal low_position;   // минимум, при котором открыта позиция

        #region-Constructor-
        public ExtremumPrice()
        {
            extremums = new Queue<ExtremumPriceParametr>();
            extremums_distance = new Queue<ExtremumPriceParametr>();
            deltaExtremums = new Queue<decimal>();
            filtrDeltas = new Queue<decimal>();
            high = 0;
            low = 0;
            high1 = 0;
            low1 = 0;
            high_dist_last = null;
            low_dist_last = null;
            type_current_extremum = 0;
        }
        #endregion

        #region-Properties-
        public Queue<ExtremumPriceParametr> Extremums
        {
            get { return extremums; }
        }
        public Queue<ExtremumPriceParametr> ExtremumsDistance
        {
            get { return extremums_distance; }
        }
        public Queue<decimal> DeltaExtremums
        {
            get { return deltaExtremums; }
        }
        public Queue<decimal> FiltrDeltas
        {
            get { return filtrDeltas; }
        }
        #endregion

        /// <summary>
        /// Поиск экстремумов
        /// </summary>
        private void ExtremumSearch(decimal _price, DateTime _datetime, decimal _otkat)
        {
            if (high == 0 && low == 0)
            {
                high = _price;
                low = _price;
            }

            if (high > 0 && _price > high)
            {
                high = _price;
                datetime = _datetime;
            }

            if (low > 0 && _price < low)
            {
                low = _price;
                datetime = _datetime;
            }

            if (high > 0 && high - _price > _otkat) // нашли локальный максимум
            {
                extremums.Enqueue(new ExtremumPriceParametr(high, 0, datetime));
                low = _price;
                high = -1;
            }

            if (low > 0 && _price - low > _otkat) // нашли локальный минимум
            {
                extremums.Enqueue(new ExtremumPriceParametr(0, low, datetime));
                high = _price;
                low = -1;
            }
        }

        /// <summary>
        /// Устарело. Поиск экстремумов с дистанцией между ними.
        /// </summary>
        private void ExtremumSearchDistance1(decimal _price, DateTime _datetime, decimal _otkat, decimal _distance)
        {
            if (high1 == 0 && low1 == 0)
            {
                high1 = _price;
                low1 = _price;
                high_dist_last = high1;
                low_dist_last = low1;
            }

            if (high1 > 0 && _price > high1)
            {
                high1 = _price;
                datetime = _datetime;
            }

            if (low1 > 0 && _price < low1)
            {
                low1 = _price;
                datetime = _datetime;
            }

            if (high1 > 0 && high1 - _price > _otkat) // нашли локальный максимум
            {
                StaticService.LogFileWriteNotDateTime("High  " + high1 + " Time " + _datetime, "trade_log.txt", true);

                if (high1 - low_dist_last >= _distance && high1 > high_dist_last)
                {
                    StaticService.LogFileWriteNotDateTime("HighD " + high1 + " Time " + _datetime, "trade_log.txt", true);

                    extremums_distance.Enqueue(new ExtremumPriceParametr(high1, 0, datetime));

                    high_dist_last = high1;
                    low_dist_last = Decimal.MaxValue;

                    if (type_current_extremum != 1) // если до этого был не максимум
                    {
                        int _qty = 0;

                        if(base.TP == 0)
                        {
                            _qty = -1;
                        }
                        else if (base.TP > 0)
                        {
                            _qty = -2;
                        }

                        StaticService.LogFileWriteNotDateTime("Short " + (high1 - _otkat) + " Qty " + _qty + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                        base.CalcEquity(_datetime, high1 - _otkat, _qty);
                        high_position = high1;
                        low_position = 0;
                    }
                    type_current_extremum = 1;
                }
                low1 = _price;
                highlast = high1;
                high1 = -1;
            }

            if (low1 > 0 && _price - low1 > _otkat) // нашли локальный минимум
            {
                StaticService.LogFileWriteNotDateTime("Low   " + low1 + " Time " + _datetime, "trade_log.txt", true);

                if (high_dist_last - low1 >= _distance && low1 < low_dist_last)
                {
                    StaticService.LogFileWriteNotDateTime("LowD  " + low1 + " Time " + _datetime, "trade_log.txt", true);

                    extremums_distance.Enqueue(new ExtremumPriceParametr(0, low1, datetime));
                    low_dist_last = low1;
                    high_dist_last = 0;

                    if (type_current_extremum != -1) // если до этого был не минимум
                    {
                        int _qty = 0;

                        if (base.TP == 0)
                        {
                            _qty = 1;
                        }
                        else if (base.TP < 0)
                        {
                            _qty = 2;
                        }

                        StaticService.LogFileWriteNotDateTime("Long  " + (low1 + _otkat) + " Qty " + _qty + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                        base.CalcEquity(_datetime, low1 + _otkat, _qty);
                        high_position = 0;
                        low_position = low1;
                    }
                    type_current_extremum = -1;
                }
                high1 = _price;
                lowlast = low1;
                low1 = -1;
            }

            if (high_position > 0 && _price > high_position) // close short
            {

                StaticService.LogFileWriteNotDateTime("Short close " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                base.CalcEquity(_datetime, _price, base.TP * -1);
                high_position = 0;
            }
            if (low_position > 0 && _price < low_position) // close long
            {

                StaticService.LogFileWriteNotDateTime("Long close  " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                base.CalcEquity(_datetime, _price, base.TP * -1);
                low_position = 0;
            }
        }
        /// <summary>
        /// Поиск экстремумов с дистанцией между ними
        /// Торговля с разворотом.
        /// </summary>
        private void ExtremumSearchDistance2(decimal _price, DateTime _datetime, decimal _otkat, decimal _distance)
        {
            if (high1 == 0 && low1 == 0)
            {
                high1 = _price;
                low1 = _price;
            }

            if (high1 > 0 && _price > high1)
            {
                high1 = _price;
                datetime = _datetime;
            }

            if (low1 > 0 && _price < low1)
            {
                low1 = _price;
                datetime = _datetime;
            }

            if (high1 > 0 && high1 - _price > _otkat) // нашли локальный максимум
            {
                //StaticService.LogFileWriteNotDateTime("High  " + high1 + " Time " + _datetime, "trade_log.txt", true);

                if (high_dist_last == null)
                {
                    high_dist_last = high1;
                    low_dist_last = -1;
                }

                if (high_dist_last < 0) // если макс_дист. еще не найден
                {
                    if (high1 - low_dist_last >= _distance) // если нашли макс_дист.
                    {
                        //StaticService.LogFileWriteNotDateTime("HighD " + high1 + " Time " + _datetime, "trade_log.txt", true);

                        extremums_distance.Enqueue(new ExtremumPriceParametr(high1, 0, datetime));

                        high_dist_last = high1;
                        low_dist_last = -1;

                        if (type_current_extremum != 1) // если до этого был не максимум
                        {
                            int _qty = 0;

                            if (base.TP == 0)
                            {
                                _qty = -1;
                            }
                            else if (base.TP > 0)
                            {
                                _qty = -2;
                            }

                            StaticService.LogFileWriteNotDateTime("Short " + (high1 - _otkat) + " Qty " + _qty + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                            base.CalcEquity(_datetime, high1 - _otkat, _qty);
                            high_position = high1;
                            low_position = 0;
                        }
                        type_current_extremum = 1;
                    }
                }
                else
                {
                    if (high1 > high_dist_last)
                    {
                        //StaticService.LogFileWriteNotDateTime("HighD " + high1 + " Time " + _datetime, "trade_log.txt", true);
                        extremums_distance.Enqueue(new ExtremumPriceParametr(high1, 0, datetime));
                        high_dist_last = high1;
                    }
                }
                
                low1 = _price;
                highlast = high1;
                high1 = -1;
            }

            //*******************************************************

            if (low1 > 0 && _price - low1 > _otkat) // нашли локальный минимум
            {
                //StaticService.LogFileWriteNotDateTime("Low   " + low1 + " Time " + _datetime, "trade_log.txt", true);

                if (low_dist_last == null)
                {
                    low_dist_last = low1;
                    high_dist_last = -1;
                }

                if (low_dist_last < 0) // если min_дист. еще не найден
                {
                    if (high_dist_last - low1 >= _distance) // если нашли min_дист.
                    {
                        //StaticService.LogFileWriteNotDateTime("LowD  " + low1 + " Time " + _datetime, "trade_log.txt", true);

                        extremums_distance.Enqueue(new ExtremumPriceParametr(0, low1, datetime));
                        low_dist_last = low1;
                        high_dist_last = -1;

                        if (type_current_extremum != -1) // если до этого был не минимум
                        {
                            int _qty = 0;

                            if (base.TP == 0)
                            {
                                _qty = 1;
                            }
                            else if (base.TP < 0)
                            {
                                _qty = 2;
                            }

                            StaticService.LogFileWriteNotDateTime("Long  " + (low1 + _otkat) + " Qty " + _qty + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                            base.CalcEquity(_datetime, low1 + _otkat, _qty);
                            high_position = 0;
                            low_position = low1;
                        }
                        type_current_extremum = -1;
                    }
                }
                else
                {
                    if (low1 < low_dist_last)
                    {
                        //StaticService.LogFileWriteNotDateTime("LowD  " + low1 + " Time " + _datetime, "trade_log.txt", true);
                        extremums_distance.Enqueue(new ExtremumPriceParametr(0, low1, datetime));
                        low_dist_last = low1;
                    }
                }
                
                high1 = _price;
                lowlast = low1;
                low1 = -1;
            }

            if (high_position > 0 && _price > high_position) // close short
            {

                StaticService.LogFileWriteNotDateTime("Short close " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                base.CalcEquity(_datetime, _price, base.TP * -1);
                high_position = 0;
            }
            if (low_position > 0 && _price < low_position) // close long
            {

                StaticService.LogFileWriteNotDateTime("Long close  " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);

                base.CalcEquity(_datetime, _price, base.TP * -1);
                low_position = 0;
            }
        }
        /// <summary>
        /// Поиск экстремумов с дистанцией между ними
        /// Торговля с профитом.
        /// </summary>
        private void ExtremumSearchDistance(decimal _price, DateTime _datetime, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            if (_otkat <= 0 | _distance <= 0 | _profit <= 0) { return; }

            if (high1 == 0 && low1 == 0)
            {
                high1 = _price;
                low1 = _price;
            }

            if (high1 > 0 && _price > high1)
            {
                high1 = _price;
                datetime = _datetime;
            }

            if (low1 > 0 && _price < low1)
            {
                low1 = _price;
                datetime = _datetime;
            }

            if (high1 > 0 && high1 - _price > _otkat) // нашли локальный максимум
            {
                if (_write_trades)
                {
                    StaticService.LogFileWriteNotDateTime("High  " + high1 + " Time " + _datetime, "trade_log.txt", true);
                }

                if (high_dist_last == null)
                {
                    high_dist_last = high1;
                    low_dist_last = -1;
                }

                if (high_dist_last < 0) // если макс_дист. еще не найден
                {
                    if (high1 - low_dist_last >= _distance) // если нашли макс_дист.
                    {
                        if (_write_trades)
                        {
                            StaticService.LogFileWriteNotDateTime("HighD " + high1 + " Time " + _datetime, "trade_log.txt", true);
                        }

                        extremums_distance.Enqueue(new ExtremumPriceParametr(high1, 0, datetime));

                        high_dist_last = high1;
                        low_dist_last = -1;

                        if (type_current_extremum != 1) // если до этого был не максимум
                        {
                            int _qty = 0;

                            if (base.TP == 0)
                            {
                                _qty = -1;


                                base.CalcEquity(_datetime, high1 - _otkat, _qty);

                                if (_write_trades)
                                {
                                    StaticService.LogFileWriteNotDateTime("Short " + (high1 - _otkat) + " Qty " + _qty + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);
                                }
                                
                                high_position = high1;
                                low_position = 0;
                            }
                        }
                        type_current_extremum = 1;
                    }
                }
                else
                {
                    if (high1 > high_dist_last)
                    {
                        if (_write_trades)
                        {
                            StaticService.LogFileWriteNotDateTime("HighD " + high1 + " Time " + _datetime, "trade_log.txt", true);
                        }
                        
                        extremums_distance.Enqueue(new ExtremumPriceParametr(high1, 0, datetime));
                        high_dist_last = high1;
                    }
                }

                low1 = _price;
                highlast = high1;
                high1 = -1;
            }

            //*******************************************************

            if (low1 > 0 && _price - low1 > _otkat) // нашли локальный минимум
            {
                if (_write_trades)
                {
                    StaticService.LogFileWriteNotDateTime("Low   " + low1 + " Time " + _datetime, "trade_log.txt", true);
                }

                if (low_dist_last == null)
                {
                    low_dist_last = low1;
                    high_dist_last = -1;
                }

                if (low_dist_last < 0) // если min_дист. еще не найден
                {
                    if (high_dist_last - low1 >= _distance) // если нашли min_дист.
                    {
                        if (_write_trades)
                        {
                            StaticService.LogFileWriteNotDateTime("LowD  " + low1 + " Time " + _datetime, "trade_log.txt", true);
                        }

                        extremums_distance.Enqueue(new ExtremumPriceParametr(0, low1, datetime));
                        low_dist_last = low1;
                        high_dist_last = -1;

                        if (type_current_extremum != -1) // если до этого был не минимум
                        {
                            int _qty = 0;

                            if (base.TP == 0)
                            {
                                _qty = 1;


                                base.CalcEquity(_datetime, low1 + _otkat, _qty);

                                if (_write_trades)
                                {
                                    StaticService.LogFileWriteNotDateTime("Long  " + (low1 + _otkat) + " Qty " + _qty + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);
                                }

                                high_position = 0;
                                low_position = low1;
                            }
                        }
                        type_current_extremum = -1;
                    }
                }
                else
                {
                    if (low1 < low_dist_last)
                    {
                        if (_write_trades)
                        {
                            StaticService.LogFileWriteNotDateTime("LowD  " + low1 + " Time " + _datetime, "trade_log.txt", true);
                        }
                        
                        extremums_distance.Enqueue(new ExtremumPriceParametr(0, low1, datetime));
                        low_dist_last = low1;
                    }
                }

                high1 = _price;
                lowlast = low1;
                low1 = -1;
            }

            if (base.TP < 0 && _price < base.PricePosition - _profit) // close short profit
            {
                base.CalcEquity(_datetime, _price, base.TP * -1);

                if (_write_trades)
                {
                    StaticService.LogFileWriteNotDateTime("Short profit " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);
                }
                
                high_position = 0;
            }
            if (high_position > 0 && _price > high_position) // close short loss
            {
                base.CalcEquity(_datetime, _price, base.TP * -1);

                if (_write_trades)
                {
                    StaticService.LogFileWriteNotDateTime("Short loss " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);
                }
                
                high_position = 0;
            }

            if (base.TP > 0 && _price > base.PricePosition + _profit) // close long profit
            {
                base.CalcEquity(_datetime, _price, base.TP * -1);

                if (_write_trades)
                {
                    StaticService.LogFileWriteNotDateTime("Long profit  " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);
                }
                
                low_position = 0;
            }
            if (low_position > 0 && _price < low_position) // close long loss
            {
                base.CalcEquity(_datetime, _price, base.TP * -1);

                if (_write_trades)
                {
                    StaticService.LogFileWriteNotDateTime("Long loss  " + _price + " Qty " + base.TP * -1 + " Time " + _datetime + " Equity " + base.EqCurrent, "trade_log.txt", true);
                }
                
                low_position = 0;
            }
        }




        #region -Запуски поиска экстремумов-
        /// <summary>
        /// Запуск поиска экстремумов
        /// </summary>
        public void CreateExtrem(MarketTradesRepository _mTR, decimal _otkat)
        {
            foreach (var item in _mTR)
            {
                this.ExtremumSearch(item.PriceTrades, item.DateTimeTrades, _otkat);
            }
        }
        public void CreateExtrem(MarketTradesRepository _mTR, decimal _otkat, TimeSpan _starttime, TimeSpan _finishtime)
        {
            bool _search = false;
            TimeSpan _time_trades;

            foreach (var item in _mTR)
            {
                _time_trades = item.DateTimeTrades.TimeOfDay;

                if (_time_trades > _starttime && _time_trades < _finishtime)
                {
                    this.ExtremumSearch(item.PriceTrades, item.DateTimeTrades, _otkat);
                    _search = true;
                }
                if (_time_trades > _finishtime && _search)
                {
                    break;
                }
            }
        }//--------------------------

        /// <summary>
        /// Запуск поиска экстремумов с дистанцией
        /// Возвращает string
        /// </summary>
        public string CreateExtremDistance(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            string _result = String.Empty;
            
            ExtremumDistanceProcess(_mTR, _otkat, _distance, _profit, _write_trades);

            if (base.CountTrades > 0)
            {
                _result = base.EqCurrent + "\t" + base.CountTrades + "\t" + base.CountProfit + "\t" + base.CountLoss + "\t" + base.Drawdown + "\t" + _otkat + "\t" + _distance + "\t" + _profit + "\n";
            }

            return _result;
        }
        public string CreateExtremDistance(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades, TimeSpan _starttime, TimeSpan _finishtime)
        {
            string _result = String.Empty;

            ExtremumDistanceProcess(_mTR, _otkat, _distance, _profit, _write_trades, _starttime, _finishtime);

            if (base.CountTrades > 0)
            {
                _result = base.EqCurrent + "\t" + base.CountTrades + "\t" + base.CountProfit + "\t" + base.CountLoss + "\t" + base.Drawdown + "\t" + _otkat + "\t" + _distance + "\t" + _profit + "\n";
            }

            return _result;
        }//----------------

        /// <summary>
        /// Запуск поиска экстремумов с дистанцией
        /// Возвращает ResultExtremDistance
        /// </summary>
        public ResultExtremDistance CreateExtremDistanceResult(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            ExtremumDistanceProcess(_mTR, _otkat, _distance, _profit, _write_trades);

            if (base.EqCurrent > 0 && base.CountTrades > 0)
            {
                return new ResultExtremDistance(base.EqCurrent, base.Drawdown, _otkat, _distance, _profit);
            }

            return new ResultExtremDistance();
        }

        //----
        private void ExtremumDistanceProcess(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades, TimeSpan _starttime, TimeSpan _finishtime)
        {
            bool _search = false;
            TimeSpan _time_trades;

            foreach (var item in _mTR)
            {
                _time_trades = item.DateTimeTrades.TimeOfDay;

                if (_time_trades > _starttime && _time_trades < _finishtime)
                {
                    this.ExtremumSearchDistance(item.PriceTrades, item.DateTimeTrades, _otkat, _distance, _profit, _write_trades);
                    _search = true;
                }
                if (_time_trades > _finishtime && _search)
                {
                    break;
                }
            }
        }
        private void ExtremumDistanceProcess(MarketTradesRepository _mTR, decimal _otkat, decimal _distance, decimal _profit, bool _write_trades)
        {
            foreach (var item in _mTR)
            {
                this.ExtremumSearchDistance(item.PriceTrades, item.DateTimeTrades, _otkat, _distance, _profit, _write_trades);
            }
        }
        //----
        #endregion


        /// <summary>
        /// Считаем дельту между экстремумами
        /// </summary>
        public void DeltaExtr()
        {
            decimal _previous = 0;
            decimal _delta = 0;
            decimal _h = 0;
            decimal _l = 0;

            foreach (var item in extremums)
            {
                _h = item.PriceHigh;
                _l = item.PriceLow;

                if (_h > 0)
                {
                    if (_previous > 0)
                    {
                        _delta = _previous - _h;
                    }
                    _previous = _h;
                }
                if (_l > 0)
                {
                    if (_previous > 0)
                    {
                        _delta = _previous - _l;
                    }
                    _previous = _l;
                }

                deltaExtremums.Enqueue(_delta);
            }
        }

        /// <summary>
        /// Фильтруем дельты, которые выше порога
        /// </summary>
        public void FiltrDelta(decimal _porog)
        {
            bool _porogUp = false;

            foreach (var item in deltaExtremums)
            {
                if (_porogUp)
                {
                    filtrDeltas.Enqueue(item);
                }

                if (Math.Abs(item) > _porog)
                {
                    _porogUp = true;
                }
                else
                {
                    _porogUp = false;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExtremumPriceParametr : Bases.IDataCharts
    {
        decimal priceHigh;
        decimal priceLow;
        DateTime dateTime;

        string id;
        double value;

        public ExtremumPriceParametr(decimal _priceHigh, decimal _priceLow, DateTime _dateTime)
        {
            priceHigh = _priceHigh;
            priceLow = _priceLow;
            dateTime = _dateTime;

            id = _dateTime.ToString();

            if (priceHigh > 0)
            {
                value = (double)priceHigh;
            }
            if (priceLow > 0)
            {
                value = (double)priceLow;
            }
        }

        public decimal PriceHigh
        {
            get { return priceHigh; }
        }
        public decimal PriceLow
        {
            get { return priceLow; }
        }
        public DateTime Date_Time
        {
            get { return dateTime; }
        }

        // Implement IDataCharts
        public string ID
        {
            get { return id; }
        }

        public double Value
        {
            get { return value; }
        }
    }

    public struct ResultExtremDistance
    {
        public decimal eqcurrent { get; private set; }
        public decimal drawdown { get; private set; }
        public decimal otkat { get; private set; }
        public decimal distance { get; private set; }
        public decimal profit { get; private set; }

        public ResultExtremDistance(decimal _eq, decimal _down, decimal _otkat, decimal _distance, decimal _profit)
            : this()
        {
            eqcurrent = _eq;
            drawdown = _down;
            otkat = _otkat;
            distance = _distance;
            profit = _profit;
        }
    }
}