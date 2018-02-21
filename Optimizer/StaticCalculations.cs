using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Optimizer
{
    public static class StaticCalculations
    {
        /// <summary>
        /// Расчитывает 3 линии БоллинджерБандс с k-ым количеством стандартных отклонений
        /// </summary>
        /// <param name="map_list">коллекция значений(цен)</param>
        /// <param name="k_">количество стандартных отклонений</param>
        /// <param name="high_line">верхняя линия</param>
        /// <param name="midle_line">средняя линия</param>
        /// <param name="low_line">нижняя линия</param>
        public static void BollingerBands(List<double> map_list, double k_, ref double high_line, ref double midle_line, ref double low_line)
        {
            double average = 0;
            double count_element = 0;
            double sum = 0;
            double std_dev = 0;

            lock (((ICollection)map_list).SyncRoot)
            {
                count_element = map_list.Count();
                average = map_list.Average();

                for (int i = 0; i < count_element; i++)
                {
                    double pow_dif = Math.Pow(map_list[i] - average, 2);
                    sum += pow_dif;
                }
            }

            std_dev = Math.Sqrt(sum / (count_element - 1));

            high_line = average + std_dev * k_;
            midle_line = average;
            low_line = average - std_dev * k_;
        }

        /// <summary>
        /// Прибыль в закрытой сделке.
        /// </summary>
        public static double CalcProfit(double _priceB, double _priceS)
        {
            double value = 0;

            if (_priceB > 0 && _priceS > 0)
            {
                value = _priceS - _priceB;
            }

            return value;
        }

        //
        public static void CalcParametrTest(double _profit, DateTime _dtTrade, ref int _countTrd, ref double _profPortf, ref double _maxP, ref double _minP, ref int _countProf, ref int _countLoss)
        {
            _countTrd++;
            _profPortf += _profit;
            SetMaxMinProfit(ref _maxP, ref _minP, _profPortf);
            SetCountProfitLoss(ref _countProf, ref _countLoss, _profit);
        }

        //
        public static void SetMaxMinProfit(ref double _maxProf, ref double _minProf, double _currentProf)
        {
            if (_currentProf > _maxProf) { _maxProf = _currentProf; }
            if (_currentProf < _minProf) { _minProf = _currentProf; }
        }

        //
        public static void SetCountProfitLoss(ref int _countProfit, ref int _countLoss, double _profit)
        {
            if (_profit > 0) { _countProfit++; }
            if (_profit < 0) { _countLoss++; }
        }

        /// <summary>
        /// Округление price с точностью step.
        /// Направление: если direction больше 0 - округление вверх
        /// если direction меньше 0 - округление вниз
        /// </summary>
        public static decimal AlignmentPrice(decimal price, decimal step, int direction)
        {
            decimal result = 0;

            if (direction < 0)
            {
                try
                {
                    result = ((Math.Floor(price / step)) * step);
                }
                catch (DivideByZeroException)
                {
                    result = -1;
                }

            }
            if (direction > 0)
            {
                try
                {
                    result = ((Math.Ceiling(price / step)) * step);
                }
                catch (DivideByZeroException)
                {
                    result = -1;
                }
            }
            return result;
        }
        /// <summary>
        /// Округление price с точностью step
        /// !!! ТОЛЬКО ДЛЯ ЦЕН В ВИДЕ ЦЕЛОГО
        /// </summary>
        public static decimal AlignmentValue(decimal _value, decimal _step)
        {
            decimal result = 0;
            _value = Math.Round(_value);

            try
            {
                result = ((Math.Round(_value / _step)) * _step);
            }
            catch (DivideByZeroException)
            {
                result = -1;
            }

            return result;
        }
    }
}
