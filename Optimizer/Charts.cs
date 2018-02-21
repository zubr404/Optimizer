using System;
using System.Collections.Generic;

namespace Optimizer
{
    /// <summary>
    /// Данные по прибыльности портфеля по сделкам для графика
    /// </summary>
    public class TradesProfitCharts : Dictionary<DateTime, double>
    {
        /// <summary>
        /// Добавление с проверкой ключа.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        public void AddInDictionary(DateTime _key, double _value)
        {
            DateTime _newKey = _key;

            if (this.ContainsKey(_newKey))
            {
                for (; ; )
                {
                    _newKey = _newKey.AddMilliseconds(1);
                    
                    if (!this.ContainsKey(_newKey))
                    {
                        this.Add(_newKey, _value);
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    this.Add(_newKey, _value);
                }
                catch (OutOfMemoryException memeEx)
                {
                    System.Windows.MessageBox.Show("Словарь TradesProfitCharts забит под завязку. Count = " + this.Count + "\n" + memeEx.Message);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Универсальные данные для графика
    /// </summary>
    public class DataUniversalCharts : Dictionary<string, double>
    {
        int id = 0;
        /// <summary>
        /// Добавление с проверкой ключа.
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        public void AddInDictionary(string _key, double _value)
        {
            string _newKey = _key;

            if (this.ContainsKey(_newKey))
            {
                id++;
                _newKey = _newKey + "." + id;
                this.Add(_newKey, _value);
            }
            else
            {
                try
                {
                    id = 0;
                    this.Add(_newKey, _value);
                }
                catch (OutOfMemoryException memeEx)
                {
                    System.Windows.MessageBox.Show("Словарь TradesProfitCharts забит под завязку. Count = " + this.Count + "\n" + memeEx.Message);
                    return;
                }
            }
        }
    }
}
