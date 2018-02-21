using System;
using System.Collections.ObjectModel;
using ExtensionMethods;
using System.Windows;
using Bases;

namespace Optimizer
{
    /// <summary>
    /// Сделка из таблицы всех сделок
    /// </summary>
    [Serializable]
    public class ParametrMarketTrades : IDataCharts
    {
        private DateTime datetimeTrades;
        private double numberTrades;
        private decimal priceTrades;
        private string seccodeTrades;
        private string timeMsk;
        private string operation;
        private decimal quantity;

        private string id;
        private double value;

        #region -Constructor-
        public ParametrMarketTrades(string _id, string _date, string _time, double _num, decimal _price, string _seccode)
        {
            try
            {
                datetimeTrades = Convert.ToDateTime(_date + " " + _time);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            numberTrades = _num;
            priceTrades = _price;
            seccodeTrades = _seccode;
            value = (double)priceTrades;
            id = _id;
        }
        public ParametrMarketTrades(string _id, string _date, string _time, double _num, decimal _price, string _seccode, string _timeMsk, string _operation, decimal _count)
        {
            try
            {
                datetimeTrades = Convert.ToDateTime(_date + " " + _time);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            numberTrades = _num;
            priceTrades = _price;
            seccodeTrades = _seccode;
            timeMsk = _timeMsk;
            operation = _operation;
            quantity = _count;
            value = (double)priceTrades;
            id = _id;
        }
        #endregion

        #region -Properties-
        public DateTime DateTimeTrades
        {
            get { return datetimeTrades; }
        }
        public double NumberTrades
        {
            get { return numberTrades; }
        }
        public decimal PriceTrades
        {
            get { return priceTrades; }
        }
        public string SeccodeTrades
        {
            get { return seccodeTrades; }
        }
        public string TimeMsk
        {
            get { return timeMsk; }
        }
        public string Operation
        {
            get { return operation; }
        }
        public decimal Quantity
        {
            get { return quantity; }
        }
        #endregion

        // Implement IDataCharts
        public string ID
        {
            get { return id; }
        }

        public double Value
        {
            get { return value; }
        }
        //-------------------------------------
    }

    /// <summary>
    /// Хранение всех сделок. Это класс должен быть сериализован.
    /// Для экспорта из Квика.
    /// </summary>
    [Serializable]
    public class MarketTradesRepository : ObservableCollection<ParametrMarketTrades>
    {
        public void AddTrades(ParametrMarketTrades _pmt)
        {
            if (!this.ExistsMain(x => x.NumberTrades == _pmt.NumberTrades))
            {
                this.Add(_pmt);
            }
        }

        public void AddTrades(ParametrMarketTrades _pmt, DateTime _dt)
        {
            if (!this.ExistsMain(x => x.NumberTrades == _pmt.NumberTrades))
            {
                if (_pmt.DateTimeTrades <= _dt)
                {
                    this.Add(_pmt);
                }
            }
        }
    }
}
