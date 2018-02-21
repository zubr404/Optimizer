using Bases;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Optimizer
{
    /// <summary>
    /// Параметры тестовых сделок
    /// </summary>
    [Serializable]
    public class ParametrTestTrades : PropertyChangedBase
    {
        private double numMarketTrades;
        private DateTime dateTime;
        private double price;
        private double qty;
        private Operation operation;
        private double profitPortfolio;

        public ParametrTestTrades(double _numMarket, DateTime _dt, double _price, double _qty, Operation _operation, double _profitPort)
        {
            numMarketTrades = _numMarket;
            dateTime = _dt;
            price = _price;
            qty = _qty;
            operation = _operation;
            profitPortfolio = _profitPort;
        }

        public DateTime DateTimeTestTrad
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                base.NotifyPropertyChanged();
            }
        }
        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                base.NotifyPropertyChanged();
            }
        }
        public double Qty
        {
            get { return qty; }
            set
            {
                qty = value;
                base.NotifyPropertyChanged();
            }
        }
        public Operation Operation
        {
            get { return operation; }
            set
            {
                operation = value;
                base.NotifyPropertyChanged();
            }
        }
        public double ProfitPortfolio
        {
            get { return profitPortfolio; }
            set
            {
                profitPortfolio = value;
                base.NotifyPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Хранение тестовых сделок
    /// </summary>
    [Serializable]
    public class TestTradesCollection : ObservableCollection<ParametrTestTrades>
    {

    }
}
