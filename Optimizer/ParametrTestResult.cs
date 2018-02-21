using Bases;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Optimizer
{
    /// <summary>
    /// Параметры результата теста
    /// </summary>
    [Serializable]
    public class ParametrTestResult : PropertyChangedBase
    {
        private string key;
        private string dateRes;
        private string settingsBolinger;
        private double profit;
        private int countTrades;
        private double maxProfit;
        private double minProfit;
        private int countProfitTrades;
        private int countLossTrades;
        private double countProfitIndex;

        public ParametrTestResult(string _key, string _dateRes, string _settBB, double _prof, int _countTrad, double _maxProf, double _minProf, int _qtyProf, int _qtyLoss)
        {
            key = _key;
            dateRes = _dateRes;
            settingsBolinger = _settBB;
            profit = _prof;
            countTrades = _countTrad;
            maxProfit = _maxProf;
            minProfit = _minProf;
            countProfitTrades = _qtyProf;
            countLossTrades = _qtyLoss;

            if (countProfitTrades + countLossTrades > 0)
            {
                countProfitIndex = ((double)countProfitTrades / ((double)countProfitTrades + (double)countLossTrades)) * 100;
            }
            else
            {
                countProfitIndex = 0;
            }
        }

        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                base.NotifyPropertyChanged();
            }
        }
        public string DateRes
        {
            get { return dateRes; }
            set
            {
                dateRes = value;
                base.NotifyPropertyChanged();
            }
        }
        public string SettingsBolinger
        {
            get { return settingsBolinger; }
            set
            {
                settingsBolinger = value;
                base.NotifyPropertyChanged();
            }
        }
        public double Profit
        {
            get { return profit; }
            set
            {
                profit = value;
                base.NotifyPropertyChanged();
            }
        }
        public int CountTrades
        {
            get { return countTrades; }
            set
            {
                countTrades = value;
                base.NotifyPropertyChanged();
            }
        }
        public double MaxProfit
        {
            get { return maxProfit; }
            set
            {
                maxProfit = value;
                base.NotifyPropertyChanged();
            }
        }
        public double MinProfit
        {
            get { return minProfit; }
            set
            {
                minProfit = value;
                base.NotifyPropertyChanged();
            }
        }
        public int CountProfitTrades
        {
            get { return countProfitTrades; }
            set
            {
                countProfitTrades = value;
                base.NotifyPropertyChanged();
            }
        }
        public int CountLossTrades
        {
            get { return countLossTrades; }
            set
            {
                countLossTrades = value;
                base.NotifyPropertyChanged();
            }
        }
        public double CountProfitIndex
        {
            get { return Math.Round(countProfitIndex, 1); }
            set
            {
                countProfitIndex = value;
                base.NotifyPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Хранение результатов тестов
    /// </summary>
    [Serializable]
    public class TestResultRepositiry : ObservableCollection<ParametrTestResult>
    {
        
    }

    /// <summary>
    /// Класс параметров окончательной группировки по парметрам ББ
    /// </summary>
    public class ParametrGroupSettingsResult : PropertyChangedBase
    {
        private string settingsBB;
        private int countGroup;

        public ParametrGroupSettingsResult(string _settBB, int _count)
        {
            settingsBB = _settBB;
            countGroup = _count;
        }

        public string SettingsBB
        {
            get { return settingsBB; }
            set
            {
                settingsBB = value;
                base.NotifyPropertyChanged();
            }
        }
        public int CountGroup
        {
            get { return countGroup; }
            set
            {
                countGroup = value;
                base.NotifyPropertyChanged();
            }
        }
    }
}
