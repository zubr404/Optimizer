using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optimizer;

namespace AccountResult
{
    /// <summary>
    /// Изменения капитала в ходе торговли
    /// с фильтрацией сделок по успеху
    /// </summary>
    public class EquityChange
    {
        const string FILE_NAME = "eq.txt";
        const string FILE_NAME1 = "eq1.txt";
        const string FILE_NAME2 = "eq2.txt";
        public const string FILE_NAME3 = "eqPro.txt";

        Queue<ResultOnePosition> resultOnePositions;

        DataUniversalCharts dataUniversalCharts;
        int tp;
        decimal price_pos;
        decimal equity_current;         // с накоплением
        decimal equity_current_filtr;   // с накоплением после фильтрации
        decimal variation_margin;   // выриационная маржа 

        decimal drawdown;       // величина масимальной просадки %
        decimal count_trades;   // количество закрытых сделок
        decimal count_profit;
        decimal count_loss;

        decimal max_profit;
        decimal min_profit;
        decimal max_down;   // храним макс. просадку от макс. прибыли в пунктах
        decimal value_down; // значение текущей просадки в пунктах

        //- - - - - - - -
        const decimal exchange_fee = 0;//1.255m; // биржевой сбор SBRF 0.47
        const decimal brokerage_commission = 0;//0.59m; // комиссия брокера 0.59
        const decimal spred = 0;//10m;
        decimal transaction_costs;

        const decimal exchange_fee1 = 1.255m; // биржевой сбор SBRF 0.47
        const decimal brokerage_commission1 = 0.59m; // комиссия брокера 0.59
        const decimal spred1 = 0;//10m;
        decimal transaction_costs1;
        //- - - - - - - -

        //--Constructor--
        public EquityChange()
        {
            Initialization();

            StaticService.DeleteFile(FILE_NAME);
            StaticService.DeleteFile(FILE_NAME1);
            StaticService.DeleteFile(FILE_NAME2);
            StaticService.DeleteFile(FILE_NAME3);
        }
        public void ResetEquity()
        {
            Initialization();
        }
        private void Initialization()
        {
            dataUniversalCharts = new DataUniversalCharts();
            resultOnePositions = new Queue<ResultOnePosition>();

            tp = 0;
            price_pos = 0;
            equity_current = 0;
            equity_current_filtr = 0;
            transaction_costs = exchange_fee + brokerage_commission + spred;
            transaction_costs1 = exchange_fee1 + brokerage_commission1 + spred1;

            drawdown = 0;
            count_trades = 0;
            count_profit = 0;
            count_loss = 0;

            max_profit = Decimal.MinValue;
            min_profit = Decimal.MaxValue;
            max_down = Decimal.MinValue;
            value_down = 0;
        }
        //---------------

        #region -Properties-
        public DataUniversalCharts DataUniverCharts
        {
            get { return dataUniversalCharts; }
        }
        protected int TP
        {
            get { return tp; }
        }
        protected decimal VariationMargin
        {
            get { return variation_margin; }
        }
        protected decimal EqCurrent
        {
            get { return equity_current; }
        }
        protected decimal PricePosition
        {
            get { return price_pos; }
        }
        protected decimal Drawdown
        {
            get { return drawdown; }
        }
        protected decimal CountTrades
        {
            get { return count_trades; }
        }
        protected decimal CountProfit
        {
            get { return count_profit; }
        }
        protected decimal CountLoss
        {
            get { return count_loss; }
        }
        #endregion

        private void MetricsMarket(decimal _equity, decimal _eq_current)
        {
            count_trades++;

            if (_equity > 0)
            {
                count_profit++;
            }
            else
            {
                count_loss++;
            }

            if (_eq_current > max_profit)
            {
                max_profit = _eq_current;
            }
            if (_eq_current < min_profit)
            {
                min_profit = _eq_current;
            }

            value_down = max_profit - _eq_current;

            if (value_down > max_down)
            {
                max_down = value_down;
            }

            if (max_profit != 0)
            {
                drawdown = Math.Round((max_down * 100) / max_profit, 2);
            }
        }

        /// <summary>
        /// Расчет вариационной маржи
        /// </summary>
        /// <param name="_market_price">рыночная цена</param>
        protected void VariationMarginCalc(decimal _market_price)
        {
            if (tp > 0)
            {
                variation_margin = _market_price - price_pos;
            }
            if (tp < 0)
            {
                variation_margin = price_pos - _market_price;
            }
        }

        /// <summary>
        /// Расчет изменения капитала.
        /// </summary>
        protected void CalcEquity(DateTime _dt, decimal _price, int _qty_operation)
        {
            decimal _equity = 0;

            if (_qty_operation < -2 | _qty_operation > 2)
            {
                throw new ArgumentException("parametr: _qty_operation имеет недопустимое значение.");
            }

            if (_qty_operation > 0) // купля
            {
                if (tp < 0) // есть шорт
                {
                    _equity = price_pos - _price - transaction_costs * 2;
                    equity_current += _equity;

                    MetricsMarket(_equity, equity_current);
                    dataUniversalCharts.AddInDictionary(_dt.ToString(), (double)equity_current);

                    tp += _qty_operation;

                    if (tp == 0) // закрыли шорт
                    {
                        price_pos = 0;
                        Logs(_dt, equity_current, _equity);
                    }
                    else
                    {
                        if (tp > 0) // перевернулись в лонг
                        {
                            price_pos = _price;
                            Logs(_dt, equity_current, _equity);
                        }
                    }

                    Logs(_dt, _price, this.tp, this.equity_current, _equity);

                    return;
                }

                if (tp == 0) // позиция = 0
                {
                    tp += _qty_operation;
                    price_pos = _price;

                    Logs(_dt, _price, this.tp, this.equity_current, _equity);

                }
            }

            //---------------------------------------

            if (_qty_operation < 0) // продажа
            {
                if (tp > 0) // есть long
                {
                    _equity = _price - price_pos - transaction_costs * 2;
                    equity_current += _equity;

                    MetricsMarket(_equity, equity_current);
                    dataUniversalCharts.AddInDictionary(_dt.ToString(), (double)equity_current);

                    tp += _qty_operation;

                    if (tp == 0) // закрыли long
                    {
                        price_pos = 0;
                        Logs(_dt, equity_current, _equity);
                    }
                    else
                    {
                        if (tp < 0) // перевернулись в short
                        {
                            price_pos = _price;
                            Logs(_dt, equity_current, _equity);
                        }
                    }

                    Logs(_dt, _price, this.tp, this.equity_current, _equity);

                    return;
                }

                if (tp == 0) // позиция = 0
                {
                    tp += _qty_operation;
                    price_pos = _price;

                    Logs(_dt, _price, this.tp, this.equity_current, _equity);

                }
            }
        }

        // --logs--
        private void Logs(DateTime _dt, decimal _price, int _tp, decimal _equitySum, decimal _equity)
        {
            StaticService.LogFileWriteNotDateTime(_dt + "\tprice " + _price + "\tTP " + _tp + "\tsum " + _equitySum + "\tequity " + _equity, FILE_NAME, true);
        }
        private void Logs(DateTime _dt, decimal _eqSum, decimal _eq)
        {
            StaticService.LogFileWriteNotDateTime(_dt + "\t" + _eqSum + "\t" + _eq, FILE_NAME1, true);

            resultOnePositions.Enqueue(new ResultOnePosition { Date_Time = _dt, EquityOne = _eq });
            //SuccessTradesFiltr(_dt, _eq);
        }
        //-------------

        // -фильтрация-
        protected void SuccessTradesPro()
        {
            Queue<ResultOnePosition> _queueResInput = resultOnePositions;
            decimal _trans_cost = 0;
            int _stopI = 2;

            //-исходные-
            //StaticService.LogFileWriteNotDateTime("\t* -1 *", FILE_NAME3, true);
            foreach (var item in resultOnePositions)
            {
                StaticService.LogFileWriteNotDateTime(item.EquityOne.ToString(), FILE_NAME3, true);
            }
            return;
            //---------

            for (int i = 1; i <= _stopI; i++)
            {
                Queue<ResultOnePosition> _queueResOut = new Queue<ResultOnePosition>();
                decimal _eqSum_filtr = 0;
                bool? _success = null;

                if (i == _stopI) { _trans_cost = transaction_costs1 * 2; } // !!!  * 2 !!!!

                foreach (var item in _queueResInput)
                {
                    decimal _eq = item.EquityOne;
                    DateTime _dt = item.Date_Time;

                    if (_success == true)
                    {
                        if (_eq < 0 && i == _stopI)
                        {
                            _eq -= 2.5m; // увеличиваем лоcс на спред при убыточных сделках, т.к. закрытие будет происходить по рынку
                        }

                        _eqSum_filtr += _eq - _trans_cost;
                        
                        _queueResOut.Enqueue(new ResultOnePosition { Date_Time = _dt, EquityOne = _eq });
                    }

                    // это должно быть в последнюю очередь
                    if (_eq > 0)
                    {
                        _success = true;
                    }
                    else
                    {
                        _success = false;
                    }
                }

                _queueResInput.Clear();
                //StaticService.LogFileWriteNotDateTime("\t*" + i + " " + _eqSum_filtr + "*", FILE_NAME3, true);
                StaticService.LogFileWriteNotDateTime(_eqSum_filtr.ToString(), FILE_NAME3, true);

                foreach (var item in _queueResOut)
                {
                    _queueResInput.Enqueue(item);
                    //StaticService.LogFileWriteNotDateTime(item.Date_Time + "\t" + item.EquityOne, FILE_NAME3, true);
                }
            }
        }

        protected void SuccessTradesPro1()
        {
            decimal _eqSum_filtr = 0;
            bool? _success = null;

            int _count_loss = 0;
            int _count_profit = 0;

            foreach (var item in resultOnePositions)
            {
                decimal _eq = item.EquityOne;
                DateTime _dt = item.Date_Time;

                if (_success == true)
                {
                    if (_eq < 0)
                    {
                        _eq -= 2.5m; // увеличиваем лоcс на спред при убыточных сделках, т.к. закрытие будет происходить по рынку
                    }

                    _eqSum_filtr += _eq - transaction_costs1 * 2;
                }

                // это должно быть в последнюю очередь
                if (_eq > 0)
                {
                    _count_loss = 0;
                    _count_profit++;
                }
                else
                {
                    _count_loss++;
                    _count_profit = 0;
                }
            }
        }
        //------------
    }

    /// <summary>
    /// 
    /// </summary>
    public struct ResultOnePosition
    {
        public DateTime Date_Time { get; set; }
        public decimal EquityOne { get; set; }
    }

    /// <summary>
    /// Эмулятор высталения заявок
    /// </summary>
    public class OrdersEmulator
    {
        OrderParametr orderBuy;
        OrderParametr orderSell;

        public OrdersEmulator()
        {
            orderBuy = new OrderParametr();
            orderSell = new OrderParametr();
        }

        public OrderParametr OrderBuy { get { return orderBuy; } }
        public OrderParametr OrderSell { get { return orderSell; } }

        public void NewOrderBuy(decimal _price)
        {
            if (orderBuy.Price <= 0)
            {
                orderBuy.Price = _price;
            }
        }

        public void NewOrderSell(decimal _price)
        {
            if (orderSell.Price <= 0)
            {
                orderSell.Price = _price;
            }
        }

        public decimal FillOrderBuy(decimal _price)
        {
            decimal _pr = orderBuy.Price;

            if (_pr > 0)
            {
                if (_price < _pr)
                {
                    orderBuy.Price = 0;
                    return _pr;
                }
            }

            return 0;
        }

        public decimal FillOrderSell(decimal _price)
        {
            decimal _pr = orderSell.Price;

            if (_pr > 0)
            {
                if (_price > _pr)
                {
                    orderSell.Price = 0;
                    return _pr;
                }
            }

            return 0;
        }

        public void MoveOrderBuy(decimal _price)
        {
            if (orderBuy.Price > 0)
            {
                orderBuy.Price = _price;
            }
        }

        public void MoveOrderSell(decimal _price)
        {
            if (orderSell.Price > 0)
            {
                orderSell.Price = _price;
            }
        }

        public void KillOrderBuy()
        {
            if (orderBuy.Price > 0)
            {
                orderBuy.Price = 0;
            }
        }

        public void KillOrderSell()
        {
            if (orderSell.Price > 0)
            {
                orderSell.Price = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct OrderParametr
    {
        public decimal Price { get; set; }
    }
}
