using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    /// <summary>
    /// Класс оптимизации.
    /// </summary>
    public static class StaticOptimizer
    {
        // прекращаем торговлю после первой убыточной сделки / возобновляем после первой прибыльной
        // работает преимущественно в минус
        public static bool PsevdoRealTrades(double _profit, DateTime dateTimeTrade, ref int _counterP, ref int _counterL)
        {
            if (_profit > 0)
            {
                _counterP++;
                _counterL = 0;

                if (_counterP > 1)
                {
                    return true;
                }
            }
            else
            {
                if (_profit < 0)
                {
                    _counterL++;
                    _counterP = 0;

                    if (_counterL == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// оптимизация по прибыльности сделки с разделением на шорты и лонги
        /// </summary>
        /// <param name="_profit">прибыль/убыток</param>
        /// <param name="_long">закрытая позиция</param>
        /// <param name="_longProfit"></param>
        /// <param name="_shortProfit"></param>
        public static void PsevdoRealTrades(double _profit, bool _long, ref bool _longProfit, ref bool _shortProfit)
        {
            if (_long)
            {
                if (_profit > 0)
                {
                    _longProfit = true;
                }
                else if (_profit < 0)
                {
                    _longProfit = false;
                }
            }
            else
            {
                if (_profit > 0)
                {
                    _shortProfit = true;
                }
                else if (_profit < 0)
                {
                    _shortProfit = false;
                }
            }
        } 
    }
}
