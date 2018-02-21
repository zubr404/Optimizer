using System;
using System.Collections.ObjectModel;

namespace Optimizer
{
    /// <summary>
    /// Сделки с расчитанным Боллинжером
    /// </summary>
    [Serializable]
    public class ParametrTradesBolinger
    {
        private DateTime _datetimeTrades;
        private double _numberTrades;
        private decimal _priceTrades;
        private string _seccodeTrades;
        private double _lineUp;
        private double _lineDown;
        private double _lineMidl;

        #region -Constructor-
        public ParametrTradesBolinger(DateTime _dateTime, double _num, decimal _price, string _seccode, double _lUp, double _lDown, double _lMidl)
        {
            _datetimeTrades = _dateTime;
            _numberTrades = _num;
            _priceTrades = _price;
            _seccodeTrades = _seccode;
            _lineUp = _lUp;
            _lineDown = _lDown;
            _lineMidl = _lMidl;
        }
        #endregion

        #region -Properties-
        public DateTime DateTimeTrades
        {
            get { return _datetimeTrades; }
        }
        public double NumberTrades
        {
            get { return _numberTrades; }
        }
        public decimal PriceTrades
        {
            get { return _priceTrades; }
        }
        public string SeccodeTrades
        {
            get { return _seccodeTrades; }
        }
        public double LineUp
        {
            get { return _lineUp; }
        }
        public double LineDown
        {
            get { return _lineDown; }
        }
        public double LineMidl
        {
            get { return _lineMidl; }
        }
        #endregion
    }

    /// <summary>
    /// Хранение сделок с расчитанным Боллинжером
    /// </summary>
    [Serializable]
    public class TradesBolingerRepository : ObservableCollection<ParametrTradesBolinger>
    {

    }
}
