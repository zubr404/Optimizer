using AccountResult;
using Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritms
{
    /// <summary>
    /// Логика выбросов от скользящей средней
    /// </summary>
    public class MovingExtremum : AccountResult.EquityChange
    {
        MarketTradesRepository mTR;

        const string multi_lots_log = "multi_lots_log.txt";

        public MovingExtremum()
        {
            mTR = new MarketTradesRepository();
            mTR = (MarketTradesRepository)StaticService.Deserializes(mTR.GetType().ToString(), mTR);

            StaticService.DeleteFile("moving_log.txt");
            StaticService.DeleteFile("pr.txt");
            StaticService.DeleteFile(multi_lots_log);
        }

        /* PointInput....
        public void PointInput1()
        {
            //--Settings--
            decimal set_otstup_order = 30;
            decimal set_delta = 20;
            decimal set_profit = 30;
            //------------

            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 5, 0), new TimeSpan(10, 15, 0));
            ClusterSearch clusterSearch = new ClusterSearch(40);
            OrdersEmulator orderEmulator = new OrdersEmulator();

            decimal? mov_val = null;

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;
                decimal _delta = 0;

                //
                decimal _priceFillBuy = 0;
                decimal _priceFillSell = 0;
                decimal _move_round = 0;
                decimal _delta_orderBuy = 0;
                decimal _delta_orderSell = 0;

                mov_val = MovAv.MovingValue(_price);

                if (mov_val != null)
                {
                    _move_round = StaticCalculations.AlignmentValue((decimal)mov_val, 10);
                    _delta = _price - (decimal)mov_val;
                    clusterSearch.ClusterDefenition((decimal)mov_val);

                    //-1-
                    base.VariationMarginCalc(_price);

                    // OPEN POSITION
                    if (base.TP == 0)
                    {
                        decimal _temp_price;
                        // OPEN LONG
                        if (clusterSearch.Cluster_type == ClusterType.ClusterDown && orderEmulator.OrderBuy.Price == 0)
                        {
                            _temp_price = _move_round - set_otstup_order;
                            orderEmulator.NewOrderBuy(_temp_price);
                            StaticService.LogFileWriteNotDateTime("NewOrderBuy " + _temp_price + "\t[ _move_round = " + _move_round + " ]", "moving_log.txt", true);
                        }

                        // OPEN SHORT
                        if (clusterSearch.Cluster_type == ClusterType.ClusterUp && orderEmulator.OrderSell.Price == 0)
                        {
                            _temp_price = _move_round + set_otstup_order;
                            orderEmulator.NewOrderSell(_temp_price);
                            StaticService.LogFileWriteNotDateTime("NewOrderSell " + _temp_price + "\t[ _move_round = " + _move_round + " ]", "moving_log.txt", true);
                        }
                    }
                    //-----open position----------

                    // CLOSE POSITION
                    if (base.TP != 0)
                    {
                        if (base.VariationMargin < -20)
                        {
                            StaticService.LogFileWriteNotDateTime("Stop loss " + base.TP + " " + _price, "moving_log.txt", true);
                            base.CalcEquity(_datetime, _price, base.TP * -1);
                            orderEmulator.KillOrderBuy();
                            orderEmulator.KillOrderSell();
                        }
                    }


                    // FILL ORDERS
                    _priceFillBuy = orderEmulator.FillOrderBuy(_price);
                    if (_priceFillBuy > 0)
                    {
                        base.CalcEquity(_datetime, _priceFillBuy, 1);
                        StaticService.LogFileWriteNotDateTime("FillBuy " + base.TP + " " + _priceFillBuy, "moving_log.txt", true);

                        if (base.TP > 0)
                        {
                            orderEmulator.NewOrderSell(base.PricePosition + set_profit);
                        }
                    }

                    _priceFillSell = orderEmulator.FillOrderSell(_price);
                    if (_priceFillSell > 0)
                    {
                        base.CalcEquity(_datetime, _priceFillSell, -1);
                        StaticService.LogFileWriteNotDateTime("FillSell " + base.TP + " " + _priceFillSell, "moving_log.txt", true);

                        if (base.TP < 0)
                        {
                            orderEmulator.NewOrderBuy(base.PricePosition - set_profit);
                        }
                    }

                    // MOVE ORDER
                    if (base.TP == 0 && orderEmulator.OrderBuy.Price > 0) // order buy
                    {
                        _delta_orderBuy = _move_round - orderEmulator.OrderBuy.Price;

                        if (_delta_orderBuy < set_otstup_order)
                        {
                            orderEmulator.MoveOrderBuy(_move_round - set_otstup_order);
                            StaticService.LogFileWriteNotDateTime("MoveOrderBuy " + (_move_round - set_otstup_order) + "\t[ _move_round = " + _move_round + " OrderBuy = " + orderEmulator.OrderBuy.Price + " ^ " + _delta_orderBuy + " ]", "moving_log.txt", true);
                        }
                    }
                    if (base.TP == 0 && orderEmulator.OrderSell.Price > 0) // order sell
                    {
                        _delta_orderSell = orderEmulator.OrderSell.Price - _move_round;

                        if (_delta_orderSell < set_otstup_order)
                        {
                            orderEmulator.MoveOrderSell(_move_round + set_otstup_order);
                            StaticService.LogFileWriteNotDateTime("MoveOrderSell " + (_move_round + set_otstup_order) + "\t[ _move_round = " + _move_round + " OrderSell = " + orderEmulator.OrderSell.Price + " ^ " + _delta_orderSell + " ]", "moving_log.txt", true);
                        }
                    }


                    // KILL ORDERS
                    if (base.TP == 0 && orderEmulator.OrderBuy.Price > 0 && clusterSearch.Cluster_type != ClusterType.ClusterDown)
                    {
                        StaticService.LogFileWriteNotDateTime("KillOrderBuy " + orderEmulator.OrderBuy.Price, "moving_log.txt", true);
                        orderEmulator.KillOrderBuy();
                    }
                    if (base.TP == 0 && orderEmulator.OrderSell.Price > 0 && clusterSearch.Cluster_type != ClusterType.ClusterUp)
                    {
                        StaticService.LogFileWriteNotDateTime("KillOrderSell " + orderEmulator.OrderSell.Price, "moving_log.txt", true);
                        orderEmulator.KillOrderSell();
                    }


                    //------------LOGS-----------------------
                    if (Math.Abs(_delta) >= 30)
                    {
                        StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\t$ " + trad.PriceTrades + "\t" + clusterSearch.Cluster_type + "\t" + mov_val + "\t# " + _delta, "moving_log.txt", true);
                    }
                    else
                    {
                        StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\t$ " + trad.PriceTrades + "\t" + clusterSearch.Cluster_type + "\t" + mov_val + "\t" + _delta, "moving_log.txt", true);
                    }
                }
            }

            StaticService.LogFileWriteNotDateTime(base.EqCurrent.ToString(), "moving_log.txt", true);
        }


        public void PointInput2()
        {
            //--Settings--
            decimal set_otstup_order = 30;
            decimal set_delta = 20;
            decimal set_profit = 0;
            //------------

            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 55, 0));
            ClusterSearch clusterSearch = new ClusterSearch(60);
            OrdersEmulator orderEmulator = new OrdersEmulator();

            decimal? mov_val = null;

            decimal _minprice = -1;
            decimal _maxprice = -1;
            decimal _stopLong = -1;
            decimal _stopShort = -1;

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;
                decimal _delta = 0;

                //
                decimal _move_round = 0;                

                mov_val = MovAv.MovingValue(_price);

                if (mov_val != null)
                {
                    _move_round = StaticCalculations.AlignmentValue((decimal)mov_val, 10);
                    _delta = _price - (decimal)mov_val;
                    clusterSearch.ClusterDefenition((decimal)mov_val);


                    /// Фиксация минимального/максимального значения
                    /// цены при ClusterDown/ClusterUp
                    
                    // фиксация
                    if (clusterSearch.Cluster_type == ClusterType.ClusterDown)
                    {
                        if (_price < _minprice | _minprice < 0)
                        {
                            _minprice = _price;
                        }

                        _maxprice = -1;
                    }
                    if (clusterSearch.Cluster_type == ClusterType.ClusterUp)
                    {
                        if (_price > _maxprice | _maxprice < 0)
                        {
                            _maxprice = _price;
                        }

                        _minprice = -1;
                    }
                    //------------------------------------------------------------





                    //-1-
                    base.VariationMarginCalc(_price); // определение прибыли открытой позиции

                    // OPEN POSITION
                    if (base.TP == 0)
                    {
                        decimal _temp_price;

                        // OPEN LONG
                        if (clusterSearch.Cluster_type == ClusterType.ClusterDown && orderEmulator.OrderBuy.Price == 0)
                        {
                            if (_price > mov_val)
                            {
                                _temp_price = _price;
                                base.CalcEquity(_datetime, _price, 1);
                                _stopLong = _minprice;

                                //StaticService.LogFileWriteNotDateTime("NewOrderBuy " + _temp_price + "\t[ _stopLong = " + _stopLong + " ]", "moving_log.txt", true);
                            }
                        }

                        // OPEN SHORT
                        if (clusterSearch.Cluster_type == ClusterType.ClusterUp && orderEmulator.OrderSell.Price == 0)
                        {
                            if (_price < mov_val)
                            {
                                _temp_price = _price;
                                base.CalcEquity(_datetime, _price, -1);
                                _stopShort = _maxprice;

                                //StaticService.LogFileWriteNotDateTime("NewOrderSell " + _temp_price + "\t[ _stopShort = " + _stopShort + " ]", "moving_log.txt", true);
                            }
                        }
                    }
                    //-----open position----------

                    // CLOSE POSITION
                    if (base.TP != 0)
                    {
                        // -profit-
                        if (base.TP > 0)
                        {
                            if (_price < mov_val && (_price > base.PricePosition + set_profit | clusterSearch.ClusterClusters == ClusterType.ClusterFlatUp))
                            {
                                //StaticService.LogFileWriteNotDateTime("Long profit " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);
                            }
                        }
                        if (base.TP < 0)
                        {
                            if (_price > mov_val && (_price < base.PricePosition - set_profit | clusterSearch.ClusterClusters == ClusterType.ClusterFlatDown))
                            {
                                //StaticService.LogFileWriteNotDateTime("Short profit " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);
                            }
                        }

                        // -loss-
                        if (base.TP > 0)
                        {
                            if (_price < _stopLong && _stopLong > 0)
                            {
                                //StaticService.LogFileWriteNotDateTime("Long loss " + base.TP + " " + _price + "\t[ExtremumPrice = " + _stopLong + "]", "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);

                                _stopLong = -1;
                            }
                        }
                        if (base.TP < 0)
                        {
                            if (_price > _stopShort && _stopShort > 0)
                            {
                                //StaticService.LogFileWriteNotDateTime("Short loss " + base.TP + " " + _price + "\t[ExtremumPrice = " + _stopShort + "]", "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);

                                _stopShort = -1;
                            }
                        }
                    }
                    //-----close position------------

                    //------------LOGS-----------------------
                    if (Math.Abs(_delta) >= 30)
                    {
                        //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\t$ " + trad.PriceTrades + "\t" + clusterSearch.Cluster_type + "\tcc " + clusterSearch.ClusterClusters + "\t" + mov_val + "\t# " + _delta, "moving_log.txt", true);
                    }
                    else
                    {
                        //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\t$ " + trad.PriceTrades + "\t" + clusterSearch.Cluster_type + "\tcc " + clusterSearch.ClusterClusters + "\t" + mov_val + "\t" + _delta, "moving_log.txt", true);
                    }
                }
            }

            //StaticService.LogFileWriteNotDateTime(base.EqCurrent.ToString(), "moving_log.txt", true);
        }
        */

        #region -PointInput old-
        public void PointInput1()
        {
            for (int profit = 50; profit <= 100; profit += 10) // profit
            {
                for (int delta = 20; delta <= 40; delta += 10) // delta
                {
                    for (int ma = 250; ma <= 1300; ma += 100) // ma
                    {
                        StaticService.LogFileWriteNotDateTime("profit = " + profit + " delta = " + delta + " ma = " + ma, EquityChange.FILE_NAME3, true);

                        MovingAverage MovAv = new MovingAverage(ma);
                        TestStart(MovAv, profit, delta);
                    }
                }
            }
        }

        private void TestStart(MovingAverage MovAv, decimal set_profit, decimal set_delta)
        {
            //-Settings-
            //decimal set_profit = 20;
            //decimal set_loss = 10000000;
            //decimal set_delta = 40;
            //----------

            bool _long = true;
            decimal? mov_val = null;
            decimal set_step = 10;

            base.ResetEquity();
            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 55, 0));

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;
                decimal _delta = 0;

                mov_val = MovAv.MovingValue(_price);

                if (mov_val != null)
                {
                    _delta = _price - (decimal)mov_val;

                    // OPEN POSITION
                    if (base.TP == 0)
                    {
                        if (_long && _price > mov_val && _delta > set_delta)
                        {
                            StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tNewOrderBuy " + _price, "moving_log.txt", true);
                            base.CalcEquity(_datetime, _price, 1);
                            _long = false;
                        }

                        if (!_long && _price < mov_val && _delta < set_delta * -1)
                        {
                            StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tNewOrderSell " + _price, "moving_log.txt", true);
                            base.CalcEquity(_datetime, _price, -1);
                            _long = true;
                        }
                    }

                    // CLOSE POSITION
                    if (base.TP != 0)
                    {
                        // -profit-
                        if (base.TP > 0)
                        {
                            if (_price > base.PricePosition + set_profit)
                            {
                                StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tLong profit " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price - set_step, base.TP * -1);
                            }
                        }
                        if (base.TP < 0)
                        {
                            if (_price < base.PricePosition - set_profit)
                            {
                                StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tShort profit " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price + set_step, base.TP * -1);
                            }
                        }
                        //---------

                        // -loss-
                        if (base.TP > 0)
                        {
                            if (_price < mov_val - 20) // добавлен отступ -20
                            {
                                StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tLong loss " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);
                            }
                        }
                        if (base.TP < 0)
                        {
                            if (_price > mov_val + 20) // добавлен отступ +20
                            {
                                StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tShort loss " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);
                            }
                        }
                        //---------
                    }
                }
            }

            base.SuccessTradesPro();
            //StaticService.LogFileWriteNotDateTime(base.EqCurrent.ToString(), "moving_log.txt", true);
        }

        private void TestStart1(MovingAverage MovAv, decimal set_profit, decimal set_delta)
        {
            //-Settings-
            //decimal set_profit = 20;
            //decimal set_loss = 10000000;
            //decimal set_delta = 40;
            //----------

            bool _long = true;
            decimal? mov_val = null;
            decimal set_step = 10;

            base.ResetEquity();
            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 55, 0));

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;
                decimal _delta = 0;

                mov_val = MovAv.MovingValue(_price);

                if (mov_val != null)
                {
                    _delta = _price - (decimal)mov_val;

                    // OPEN POSITION
                    if (base.TP == 0)
                    {
                        if (_price > mov_val && _delta < set_delta)
                        {
                            //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tNewOrderBuy " + _price, "moving_log.txt", true);
                            base.CalcEquity(_datetime, _price, 1);
                        }

                        if(_price < mov_val && _delta > set_delta * -1)
                        {
                            //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tNewOrderSell " + _price, "moving_log.txt", true);
                            base.CalcEquity(_datetime, _price, -1);
                        }
                    }

                    // CLOSE POSITION
                    if (base.TP != 0)
                    {
                        // -profit-
                        if (base.TP > 0)
                        {
                            if (_price > base.PricePosition + set_profit)
                            {
                                //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tLong profit " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price - set_step, base.TP * -1);
                                _long = true;
                            }
                        }
                        if (base.TP < 0)
                        {
                            if (_price < base.PricePosition - set_profit)
                            {
                                //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tShort profit " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price + set_step, base.TP * -1);
                                _long = false;
                            }
                        }
                        //---------

                        // -loss-
                        if (base.TP > 0)
                        {
                            if (_price < mov_val - 20) // добавлен отступ -20
                            {
                                //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tLong loss " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);
                                _long = false;
                            }
                        }
                        if (base.TP < 0)
                        {
                            if (_price > mov_val + 20) // добавлен отступ +20
                            {
                                //StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tShort loss " + base.TP + " " + _price, "moving_log.txt", true);
                                base.CalcEquity(_datetime, _price, base.TP * -1);
                                _long = true;
                            }
                        }
                        //---------
                    }
                }
            }

            base.SuccessTradesPro();
            //StaticService.LogFileWriteNotDateTime(base.EqCurrent.ToString(), "moving_log.txt", true);
        }
        #endregion

        #region -intensity-
        public void PointInput2()
        {
            StaticService.DeleteFile("intensity.txt");
            const decimal _set_range = 30;

            Dictionary<decimal, decimal> _dictCalc = new Dictionary<decimal, decimal>();
            decimal _back_price;
            decimal _intensity = 0;
            decimal _intensi_back;

            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 50, 0));

            _back_price = mTR_filtr[0].PriceTrades;
            _dictCalc.Add(_back_price, 1);

            foreach (var trad in mTR_filtr)
            {
                decimal _p = trad.PriceTrades;
                decimal _maxKey;
                decimal _minKey;

                bool _pUp = false;
                bool _pDown = false;

                if (_back_price == _p) { continue; }
                else { _back_price = _p; }

                _maxKey = _dictCalc.Keys.Max();
                _minKey = _dictCalc.Keys.Min();

                if (_p > _minKey + _set_range)
                {
                    _pUp = true;
                }

                if (_p < _maxKey - _set_range)
                {
                    _pDown = true;
                }

                if (_pUp | _pDown)
                {
                    if (_pUp)
                    {
                        _intensi_back = _intensity;
                        _intensity = _dictCalc[_minKey] / (_set_range / 10);
                        StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tprice " + _p + "\t_minKey " + _minKey + "\t" + _intensity, "intensity.txt", true);
                        _dictCalc.Remove(_minKey);

                        //OpenTest(trad.DateTimeTrades, _p, _intensity, _intensi_back);
                    }

                    if (_pDown)
                    {
                        _intensi_back = _intensity;
                        _intensity = _dictCalc[_maxKey] * -1 / (_set_range / 10);
                        StaticService.LogFileWriteNotDateTime(trad.DateTimeTrades + "\tprice " + _p + "\t_maxKey " + _maxKey + "\t" + _intensity, "intensity.txt", true);
                        _dictCalc.Remove(_maxKey);

                        //OpenTest(trad.DateTimeTrades, _p, _intensity, _intensi_back);
                    }
                }
                else
                {
                    for (int i = 0; i < _dictCalc.Keys.Count; i++)
                    {
                        var _k = _dictCalc.Keys.ElementAt(i);
                        _dictCalc[_k]++;
                    }
                }

                if (!_dictCalc.Keys.Any(x => x == _p))
                {
                    _dictCalc.Add(_p, 1);
                }

                //CloseTest(trad.DateTimeTrades, _p, 20);
            }
        }

        private void OpenTest(DateTime _dt, decimal _price, decimal _intensity, decimal _intensi_back)
        {
            if (base.TP == 0)
            {
                if (Math.Abs(_intensity) < 4 && Math.Abs(_intensi_back) > 15)
                {
                    if (_intensity > 0 & _intensi_back < 0)
                    {
                        StaticService.LogFileWriteNotDateTime(_dt + "\tNewOrderBuy " + _price, "moving_log.txt", true);
                        base.CalcEquity(_dt, _price, 1);
                    }
                    if (_intensity < 0 & _intensi_back > 0)
                    {
                        StaticService.LogFileWriteNotDateTime(_dt + "\tNewOrderSell " + _price, "moving_log.txt", true);
                        base.CalcEquity(_dt, _price, -1);
                    }
                }
            }
        }

        private void CloseTest(DateTime _dt, decimal _price, decimal _set_profit)
        {
            if (base.TP != 0)
            {
                // -profit-
                if (base.TP > 0)
                {
                    if (_price > base.PricePosition + _set_profit)
                    {
                        StaticService.LogFileWriteNotDateTime(_dt + "\tLong profit " + base.TP + " " + _price, "moving_log.txt", true);
                        base.CalcEquity(_dt, _price - 10, base.TP * -1);
                    }
                }
                if (base.TP < 0)
                {
                    if (_price < base.PricePosition - _set_profit)
                    {
                        StaticService.LogFileWriteNotDateTime(_dt + "\tShort profit " + base.TP + " " + _price, "moving_log.txt", true);
                        base.CalcEquity(_dt, _price + 10, base.TP * -1);
                    }
                }
                //---------

                // -loss-
                if (base.TP > 0)
                {
                    if (_price < base.PricePosition - _set_profit)
                    {
                        StaticService.LogFileWriteNotDateTime(_dt + "\tLong loss " + base.TP + " " + _price, "moving_log.txt", true);
                        base.CalcEquity(_dt, _price, base.TP * -1);
                    }
                }
                if (base.TP < 0)
                {
                    if (_price > base.PricePosition + _set_profit)
                    {
                        StaticService.LogFileWriteNotDateTime(_dt + "\tShort loss " + base.TP + " " + _price, "moving_log.txt", true);
                        base.CalcEquity(_dt, _price, base.TP * -1);
                    }
                }
                //---------
            }
        }

        #endregion

        #region -delta av-
        public void PointInput3()
        {
            MovingAverage MovAv = new MovingAverage(1000);
            base.ResetEquity();
            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 56, 0));

            decimal? mov_val = null;
            List<decimal> deltaList = new List<decimal>();
            decimal delta = 0;
            
            decimal delta1 = 0;
            decimal delta2 = 0;

            int delta_sign = 0;

            StaticService.LogFileWriteNotDateTime("Время\tДельта\tДельтаСр\tДельта1\tДельта2\tЦена\tБлиж\tДальн", "moving_log.txt", true);

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;
                decimal _price1 = 0;
                decimal _price2 = 0;
                decimal deltaAv = 0;

                mov_val = MovAv.MovingValue(_price);

                if (mov_val != null)
                {
                    
                    delta = _price - (decimal)mov_val;

                    if (delta_sign != Math.Sign(delta))
                    {
                        deltaList.Clear();
                        delta_sign = Math.Sign(delta);
                    }

                    if (Math.Abs(delta) > 30)
                    {
                        deltaList.Add(delta);
                        deltaAv = deltaList.Average();
                        delta1 = (decimal)mov_val + deltaAv / 2;
                        delta2 = (decimal)mov_val + (deltaAv + deltaAv / 2);
                    }

                    if (deltaAv != 0)
                    {
                        if (_price > mov_val)
                        {
                            if (_price < delta1)
                            {
                                _price1 = _price;
                            }
                            if (_price > delta2)
                            {
                                _price2 = _price;
                            }
                        }
                        else
                        {
                            if (_price > delta1)
                            {
                                _price1 = _price;
                            }
                            if (_price < delta2)
                            {
                                _price2 = _price;
                            }
                        }
                    }


                    if (_price1 != 0 | _price2 != 0)
                    {
                        StaticService.LogFileWriteNotDateTime(_datetime + "\t" + delta + "\t" + deltaAv + "\t" + delta1 + "\t" + delta2 + "\t" + _price + "\t" + _price1 + "\t" + _price2, "moving_log.txt", true);
                    }
                }
            }
        }
        #endregion


        // profit 20; loss 100; otstup 20
        public void PointInput_2()
        {
            const decimal set_profit = 20;
            const decimal set_loss = 100;
            const decimal set_otstup = 50;
            decimal price_point = 0;
            decimal? mov_val = null;

            //MovingAverage MovAv = new MovingAverage(1000);
            base.ResetEquity();
            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 56, 0));

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;

                //mov_val = MovAv.MovingValue(_price);

                //if (mov_val == null) { continue; }

                if (base.TP == 0 && (_price < price_point - set_otstup | price_point == 0))
                {
                    //if (_price > mov_val)
                    //{
                    //    base.CalcEquity(_datetime, _price, 1);
                    //}

                    base.CalcEquity(_datetime, _price, 1);
                    StaticService.LogFileWriteNotDateTime(_datetime + "\t" + _price, "moving_log.txt", true);
                }

                if (base.TP != 0)
                {
                    if (_price >= base.PricePosition + set_profit)  // profit
                    {
                        base.CalcEquity(_datetime, _price, base.TP * -1);
                        price_point = _price;
                    }

                    if (_price <= base.PricePosition - set_loss)    // loss
                    {
                        base.CalcEquity(_datetime, _price-10, base.TP * -1);
                        price_point = _price;
                    }
                }

                if (_price > price_point)
                {
                    price_point = _price;
                }
            }
        }

        // profit 20; loss 100; otstup 20
        // defender high
        public void PointInput_1()
        {
            const decimal set_profit = 10;
            const decimal set_loss = 100;
            const decimal set_otstup = 10;

            decimal price_point = 0;
            decimal _high_price = 0;
            decimal _low_price = Decimal.MaxValue;
            bool _startH = true;

            base.ResetEquity();
            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 56, 0));

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;

                //StaticService.LogFileWriteNotDateTime(_price.ToString(), "pr.txt", true);

                // I
                if (_startH && _price > _high_price)
                {
                    _high_price = _price;
                    //StaticService.LogFileWriteNotDateTime("High " + _price, "pr.txt", true);
                }

                // II
                if (_startH && _high_price - _price > 40)
                {
                    _startH = false;
                    _low_price = _price;
                    //StaticService.LogFileWriteNotDateTime("High " + _high_price + "; price " + _price, "pr.txt", true);
                }

                // III
                if (!_startH && _price < _low_price)
                {
                    _low_price = _price;
                    //StaticService.LogFileWriteNotDateTime("Low " + _price, "pr.txt", true);
                }

                // IV
                if (!_startH && _price - _low_price > 200)
                {
                    _startH = true;
                    _high_price = _price;
                    //StaticService.LogFileWriteNotDateTime("Low " + _low_price + "; price " + _price, "pr.txt", true);
                }


                if (!_startH && base.TP == 0 && (_price < price_point - set_otstup | price_point == 0))
                {
                    base.CalcEquity(_datetime, _price, 1);
                    //StaticService.LogFileWriteNotDateTime(_datetime + "\t" + _price, "moving_log.txt", true);
                }

                if (base.TP != 0)
                {
                    if (_price >= base.PricePosition + set_profit)  // profit
                    {
                        base.CalcEquity(_datetime, _price, base.TP * -1);
                        price_point = _price;
                    }

                    if (_price <= base.PricePosition - set_loss)    // loss
                    {
                        base.CalcEquity(_datetime, _price - 10, base.TP * -1);
                        price_point = _price;
                    }
                }

                if (_price > price_point)
                {
                    price_point = _price;
                }
            }
        }



        // 5 лотов с loss = 50 от макс.; шаг = 10; profit = 20
        public void PointInput_3()
        {
            List<decimal> pricesPos = new List<decimal>();
            decimal nett_global = 0;

            const decimal set_profit = 20;
            const decimal set_loss = 1000;
            const decimal set_otstup = 20;

            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 56, 0));

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;

                //if (pricesPos.Count < 100) // new long
                //{
                    if (!pricesPos.Exists(x => x == _price))
                    {
                        pricesPos.Add(_price);
                        StaticService.LogFileWriteNotDateTime(_datetime + "\tAdd " + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);
                    }
                //}
                //else
                //{

                    decimal _max_price_pos = pricesPos.Max();
                    if (_max_price_pos - _price > set_loss) // loss
                    {
                        foreach (var item in pricesPos)
                        {
                            nett_global += _price - item;
                            StaticService.LogFileWriteNotDateTime(nett_global.ToString(), multi_lots_log, true);
                        }

                        int _countCl = pricesPos.Count();
                        pricesPos.Clear();

                        StaticService.LogFileWriteNotDateTime(_datetime + "\tClear " + _countCl + "\t" + _price, "moving_log.txt", true);
                    }
                //}

                if (pricesPos.Count > 0) // profit
                {
                    List<decimal> _temp = new List<decimal>();

                    foreach (var item in pricesPos)
                    {
                        if (_price - item >= set_profit)
                        {
                            nett_global += _price - item;
                            _temp.Add(item);
                            StaticService.LogFileWriteNotDateTime(nett_global.ToString(), multi_lots_log, true);
                        }
                    }

                    foreach (var item in _temp)
                    {
                        pricesPos.Remove(item);
                        StaticService.LogFileWriteNotDateTime(_datetime + "\tRemove " + item + "\t" + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);
                    }
                }
            }
        }


        // неограниченное количество лотов с закрытием самого убыточного,
        // наработана прибыль
        public void PointInput_4()
        {
            List<decimal> pricesPos = new List<decimal>();
            decimal nett_global = 0;
            int max_count_pos = 0;

            const decimal set_profit = 10;
            const decimal set_loss = 100;

            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 56, 0));

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;

                decimal _max_price_pos = Decimal.MaxValue;

                if (pricesPos.Count > 0)
                {
                    _max_price_pos = pricesPos.Max();
                }

                if (_price < _max_price_pos)
                {
                    if (!pricesPos.Exists(x => x == _price))
                    {
                        pricesPos.Add(_price);
                        StaticService.LogFileWriteNotDateTime(_datetime + "\tAdd " + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);

                        if (pricesPos.Count > max_count_pos)
                        {
                            max_count_pos = pricesPos.Count;
                        }
                    }
                }

                _max_price_pos = pricesPos.Max();

                if (_max_price_pos - _price > set_loss) // loss
                {
                    pricesPos.Sort();

                    for (int i = pricesPos.Count - 1; i >= 0; i--)
                    {
                        decimal _pr = pricesPos[i];
                        decimal _res = _pr - _price;
                        if (_res > 0)
                        {
                            if (_res < nett_global)
                            {
                                nett_global -= _res;
                                pricesPos.RemoveAt(i);
                                StaticService.LogFileWriteNotDateTime(_datetime + "\tRemove loss " + _pr + "\t" + _price + "\t(" + pricesPos.Count + ")\t" + "_res\t" + _res + "\tnett_global\t" + nett_global, "moving_log.txt", true);
                            }
                            else { break; }
                        }
                        else { break; }
                    }
                }

                if (pricesPos.Count > 0) // profit
                {
                    List<decimal> _temp = new List<decimal>();

                    foreach (var item in pricesPos)
                    {
                        if (_price - item > set_profit)
                        {
                            nett_global += _price - item - 10;
                            _temp.Add(item);
                            StaticService.LogFileWriteNotDateTime(nett_global.ToString(), multi_lots_log, true);
                        }
                    }

                    foreach (var item in _temp)
                    {
                        pricesPos.Remove(item);
                        StaticService.LogFileWriteNotDateTime(_datetime + "\tRemove " + item + "\t" + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);
                    }
                }
            }

            StaticService.LogFileWriteNotDateTime("Max count position " + max_count_pos, "moving_log.txt", true);
        }



        // неограниченное количество лотов с закрытием самого убыточного,
        // наработана прибыль с шагом позиции
        public void PointInput_5()
        {
            decimal set_profit = 10;
            decimal set_loss = 10;
            decimal set_step = 10;

            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 56, 0));

            StaticService.LogFileWriteNotDateTime("Count trades\tMax lots\tProfit", multi_lots_log, true);

            for (decimal i = set_profit; i < 200; i+=10)
            {
                for (decimal ii = set_loss; ii < 200; ii+=10)
                {
                    for (decimal iii = set_step; iii < 200; iii+=10)
                    {
                        WorkInput(mTR_filtr, i, ii, iii);
                    }
                }
            }
        }


        private void WorkInput(MarketTradesRepository mTR_filtr, decimal set_profit, decimal set_loss, decimal set_step)
        {
            List<decimal> pricesPos = new List<decimal>();
            decimal nett_global = 0;
            int max_count_pos = 0;
            int count_trades = 0;

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;

                decimal _max_price_pos = Decimal.MaxValue;
                decimal _min_price_pos = Decimal.MaxValue;

                if (pricesPos.Count > 0)
                {
                    _max_price_pos = pricesPos.Max();
                    _min_price_pos = pricesPos.Min();
                }

                if (_price <= _min_price_pos - set_step)
                {
                    pricesPos.Add(_price);
                    //StaticService.LogFileWriteNotDateTime(_datetime + "\tAdd " + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);

                    if (pricesPos.Count > max_count_pos)
                    {
                        max_count_pos = pricesPos.Count;
                    }
                }

                _max_price_pos = pricesPos.Max();

                if (_max_price_pos - _price > set_loss) // loss
                {
                    pricesPos.Sort();

                    for (int i = pricesPos.Count - 1; i >= 0; i--)
                    {
                        decimal _pr = pricesPos[i];
                        decimal _res = _pr - _price;
                        if (_res > set_loss)
                        {
                            if (_res < nett_global)
                            {
                                nett_global -= _res;
                                pricesPos.RemoveAt(i);
                                count_trades++;
                                //StaticService.LogFileWriteNotDateTime(_datetime + "\tRemove loss " + _pr + "\t" + _price + "\t(" + pricesPos.Count + ")\t" + "_res\t" + _res + "\tnett_global\t" + nett_global, "moving_log.txt", true);
                            }
                            else { break; }
                        }
                        else { break; }
                    }
                }

                if (pricesPos.Count > 0) // profit
                {
                    List<decimal> _temp = new List<decimal>();

                    foreach (var item in pricesPos)
                    {
                        if (_price - item > set_profit)
                        {
                            nett_global += _price - item - 10;
                            _temp.Add(item);
                            count_trades++;
                            //StaticService.LogFileWriteNotDateTime(nett_global.ToString(), multi_lots_log, true);
                        }
                    }

                    foreach (var item in _temp)
                    {
                        pricesPos.Remove(item);
                        //StaticService.LogFileWriteNotDateTime(_datetime + "\tRemove " + item + "\t" + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);
                    }
                }
            }

            //StaticService.LogFileWriteNotDateTime("Max count position " + max_count_pos, "moving_log.txt", true);

            StaticService.LogFileWriteNotDateTime(count_trades + "\t" + max_count_pos + "\t" + nett_global + "\t\t(" + set_profit + set_loss + set_step + ")", multi_lots_log, true);
        }
        public void WorkInput_old()
        {
            List<decimal> pricesPos = new List<decimal>();
            decimal nett_global = 0;
            int max_count_pos = 0;

            const decimal set_profit = 30;
            const decimal set_loss = 200;
            const decimal set_step = 40;

            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 1, 0), new TimeSpan(18, 56, 0));

            foreach (var trad in mTR_filtr)
            {
                DateTime _datetime = trad.DateTimeTrades;
                decimal _price = trad.PriceTrades;

                decimal _max_price_pos = Decimal.MaxValue;
                decimal _min_price_pos = Decimal.MaxValue;

                if (pricesPos.Count > 0)
                {
                    _max_price_pos = pricesPos.Max();
                    _min_price_pos = pricesPos.Min();
                }

                if (_price <= _min_price_pos - set_step)
                {
                    pricesPos.Add(_price);
                    StaticService.LogFileWriteNotDateTime(_datetime + "\tAdd " + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);

                    if (pricesPos.Count > max_count_pos)
                    {
                        max_count_pos = pricesPos.Count;
                    }
                }

                _max_price_pos = pricesPos.Max();

                if (_max_price_pos - _price > set_loss) // loss
                {
                    pricesPos.Sort();

                    for (int i = pricesPos.Count - 1; i >= 0; i--)
                    {
                        decimal _pr = pricesPos[i];
                        decimal _res = _pr - _price;
                        if (_res > set_loss)
                        {
                            if (_res < nett_global)
                            {
                                nett_global -= _res;
                                pricesPos.RemoveAt(i);
                                StaticService.LogFileWriteNotDateTime(_datetime + "\tRemove loss " + _pr + "\t" + _price + "\t(" + pricesPos.Count + ")\t" + "_res\t" + _res + "\tnett_global\t" + nett_global, "moving_log.txt", true);
                            }
                            else { break; }
                        }
                        else { break; }
                    }
                }

                if (pricesPos.Count > 0) // profit
                {
                    List<decimal> _temp = new List<decimal>();

                    foreach (var item in pricesPos)
                    {
                        if (_price - item > set_profit)
                        {
                            nett_global += _price - item - 10;
                            _temp.Add(item);
                            StaticService.LogFileWriteNotDateTime(nett_global.ToString(), multi_lots_log, true);
                        }
                    }

                    foreach (var item in _temp)
                    {
                        pricesPos.Remove(item);
                        StaticService.LogFileWriteNotDateTime(_datetime + "\tRemove " + item + "\t" + _price + "\t(" + pricesPos.Count + ")", "moving_log.txt", true);
                    }
                }
            }

            StaticService.LogFileWriteNotDateTime("Max count position " + max_count_pos, "moving_log.txt", true);
        }
        //---end




        // 20.09.2017
        public void PointInput()
        {
            Algoritms.FinanceResultAllMarket.PositionRepository positionRepository = new FinanceResultAllMarket.PositionRepository();
            MarketTradesRepository mTR_filtr = ResWhereTime(mTR, new TimeSpan(10, 0, 0), new TimeSpan(18, 50, 0));

            foreach (var trad in mTR_filtr)
            {
                positionRepository.TradesFilled(trad);
            }
        }






        /// <summary>
        /// Выбор нужного интервала сделок по времени
        /// </summary>
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

    /// <summary>
    /// Расчет скользящей средней налету
    /// </summary>
    public class MovingAverage
    {
        private int countvalues;
        List<decimal> values;

        public MovingAverage(int _countvalues)
        {
            if (_countvalues > 1)
            {
                countvalues = _countvalues;
            }
            else
            {
                System.Windows.MessageBox.Show("Недопустимое значение параметра _countvalues в конструкторе.");
            }

            values = new List<decimal>();
        }

        public decimal? MovingValue(decimal _inpvalue)
        {
            decimal? result = null;

            values.Add(_inpvalue);

            if (values.Count >= countvalues)
            {
                result = values.Average();
                values.RemoveAt(0);
            }

            return result;
        }
    }

    /// <summary>
    /// Поиск кластеров в значениях МА
    /// </summary>
    public class ClusterSearch
    {
        // cluster
        decimal av_value1;
        decimal start_avvalue;
        public ClusterType Cluster_type { get; private set; }
        public ClusterType ClusterClusters { get; private set; }
        public decimal ExtremumPrice { get; private set; }

        decimal count_step;
        ClusterType clusterTypeSave;

        public ClusterSearch(decimal _countstep)
        {
            count_step = _countstep;
            av_value1 = 0;
            start_avvalue = 0;
            Cluster_type = ClusterType.ClusterNaN;
            ClusterClusters = ClusterType.ClusterNaN;
            clusterTypeSave = Cluster_type;

            //StaticService.DeleteFile("cluster_log.txt");
        }


        /// <summary>
        /// Определение характера текущего кластерного изменения МА
        /// </summary>
        public void ClusterDefenition(decimal _input)
        {
            if (av_value1 == 0)
            {
                av_value1 = _input;
                return;
            }

            //-1-
            ClusterUpDefinition(_input);
            ClusterDownDefinition(_input);
            //-2-
            ClusterFlatDefinition(Cluster_type);

            av_value1 = _input;
        }

        private void ClusterUpDefinition(decimal _input)
        {
            decimal _delta = 0;

            if (Cluster_type != ClusterType.ClusterUp && Cluster_type != ClusterType.ClusterUpUnfinishedResult)
            {
                if (_input > av_value1)
                {
                    start_avvalue = _input;
                    Cluster_type = ClusterType.ClusterUpUnfinishedResult;
                    this.ExtremumPrice = Decimal.MinValue;
                    return;
                }
            }

            if (Cluster_type == ClusterType.ClusterUpUnfinishedResult)
            {
                if (_input >= av_value1)
                {
                    _delta = _input - start_avvalue;

                    //StaticService.LogFileWriteNotDateTime("ClusterUpDefinition " + _delta, "cluster_log.txt", true);

                    if (_delta >= count_step)
                    {
                        Cluster_type = ClusterType.ClusterUp;

                        // max extrem
                        if (_input > this.ExtremumPrice)
                        {
                            this.ExtremumPrice = _input;
                        }
                    }
                }
            }
        }

        private void ClusterDownDefinition(decimal _input)
        {
            decimal _delta = 0;

            if (Cluster_type != ClusterType.ClusterDown && Cluster_type != ClusterType.ClusterDownUnfinishedResult)
            {
                if (_input < av_value1)
                {
                    start_avvalue = _input;
                    Cluster_type = ClusterType.ClusterDownUnfinishedResult;
                    this.ExtremumPrice = Decimal.MaxValue;
                    return;
                }
            }

            if (Cluster_type == ClusterType.ClusterDownUnfinishedResult)
            {
                if (_input <= av_value1)
                {
                    _delta = start_avvalue - _input;

                    //StaticService.LogFileWriteNotDateTime("ClusterDownDefinition " + _delta, "cluster_log.txt", true);

                    if (_delta >= count_step)
                    {
                        Cluster_type = ClusterType.ClusterDown;

                        // min extrem
                        if (_input < this.ExtremumPrice)
                        {
                            this.ExtremumPrice = _input;
                        }
                    }
                }
            }
        }

        private void ClusterFlatDefinition(ClusterType _clusterType)
        {
            if (_clusterType == ClusterType.ClusterUp | _clusterType == ClusterType.ClusterDown)
            {
                clusterTypeSave = _clusterType;
                ClusterClusters = ClusterType.ClusterNaN;
            }

            if (clusterTypeSave == ClusterType.ClusterDown && _clusterType != ClusterType.ClusterDown)
            {
                ClusterClusters = ClusterType.ClusterFlatDown;
            }

            if (clusterTypeSave == ClusterType.ClusterUp && _clusterType != ClusterType.ClusterUp)
            {
                ClusterClusters = ClusterType.ClusterFlatUp;
            }
        }
    }

    public enum ClusterType
    {
        /// <summary>
        /// определен рост
        /// </summary>
        ClusterUp,
        /// <summary>
        /// определено падение
        /// </summary>
        ClusterDown,
        /// <summary>
        /// кластер не определен
        /// </summary>
        ClusterNaN,
        /// <summary>
        /// предопределенный рост
        /// </summary>
        ClusterUpUnfinishedResult,
        /// <summary>
        /// предопределенное падение
        /// </summary>
        ClusterDownUnfinishedResult,
        /// <summary>
        /// ClasterClasters: боковик после роста
        /// </summary>
        ClusterFlatUp,
        /// <summary>
        /// ClasterClasters: боковик после падения
        /// </summary>
        ClusterFlatDown
    }
}
