using Bases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    public class ManipulationCharts
    {
        /// <summary>
        /// получает заглавные буквы
        /// </summary>
        public const string Patern3 = @"\p{Lu}+";

        int count;
        int count1;
        int count2;

        public ManipulationCharts()
        {
            count = 50;
            count1 = count;
            count2 = count;
        }


        /// <summary>
        /// Подготовка данных для графика
        /// </summary>
        public DataUniversalCharts CreateDataCharts<T, U>(T _inputData)
            where T : IEnumerable<U>
            where U : IDataCharts
        {
            DataUniversalCharts result = new DataUniversalCharts();

            foreach (U item in _inputData)
            {
                result.AddInDictionary(item.ID, item.Value);
            }

            return result;
        }
        private DataUniversalCharts CreateDataCharts(Queue<decimal> _inputData)
        {
            DataUniversalCharts result = new DataUniversalCharts();
            int _id = 0;

            foreach (decimal item in _inputData)
            {
                _id++;
                result.Add(_id.ToString(), (double)item);
            }

            return result;
        }

        /// <summary>
        /// строим пул графиков
        /// </summary>
        /// <param name="_inputAllPull">List коллекций с полными данными</param>
        /// <returns>готовые данные для построения</returns>
        public List<DataUniversalCharts> ChartsPull(List<DataUniversalCharts> _inputAllPull, ActionCharts _actionCharts, List<DataUniversalCharts> _originalPull = null)
        {
            List<DataUniversalCharts> result = new List<DataUniversalCharts>();
            DataUniversalCharts _parentalChart = null;
            DataUniversalCharts _childrenChart;
            int i = 0;

            IEnumerable<DataUniversalCharts> _enumerAll = _inputAllPull.OrderByDescending(x => x.Count);
            IEnumerable<DataUniversalCharts> _enumerOriginal = null;

            if (_originalPull != null)
            {
                _enumerOriginal = _originalPull.OrderByDescending(x => x.Count);
            }

            foreach (var item in _enumerAll)
            {
                if (i == 0)
                {
                    switch (_actionCharts)
                    {
                        case ActionCharts.ShiftForvard:
                            _parentalChart = ChartShift(_enumerOriginal.ElementAt(0), item);
                            break;
                        case ActionCharts.ShiftBack:
                            _parentalChart = ChartShift(_enumerOriginal.ElementAt(0), item, -10);
                            break;
                        case ActionCharts.Compress:
                            _parentalChart = StretchCharts(_enumerOriginal.ElementAt(0), item, 10);
                            break;
                        case ActionCharts.Stretch:
                            _parentalChart = StretchCharts(_enumerOriginal.ElementAt(0), item, -10);
                            break;
                        case ActionCharts.None:
                            _parentalChart = FirstVievCharts(item);
                            break;
                    }
                    result.Add(_parentalChart);
                }
                else
                {
                    _childrenChart = ChildrenCharts(_parentalChart, item);
                    result.Add(_childrenChart);
                }
                i++;
            }

            return result;
        }

        // строим дочерние графики
        public DataUniversalCharts ChildrenCharts(DataUniversalCharts _parentalData, DataUniversalCharts _allDataChildren)
        {
            DataUniversalCharts result = new DataUniversalCharts();
            Dictionary<string, double>.KeyCollection _keys = _parentalData.Keys;

            if (_keys.Count == 0) 
            { return result; }

            DateTime _keystart = Convert.ToDateTime(_keys.ElementAt(0));
            DateTime _keyfinish = Convert.ToDateTime(_keys.ElementAt(_parentalData.Keys.Count - 1));

            foreach (var item in _allDataChildren)
            {
                DateTime _key = Convert.ToDateTime(item.Key);

                if (_key >= _keystart && _key <= _keyfinish)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Движение графика вправо/влево
        /// </summary>
        public DataUniversalCharts ChartShift(Dictionary<string, double> _dataOriginal, Dictionary<string, double> _dataAll, int _shiftstep = 10)
        {
            DataUniversalCharts result = new DataUniversalCharts();
            int _count = _dataOriginal.Count;

            if (_count == 0) 
            { return result; }

            string _idFirts = _dataOriginal.ElementAt(0).Key;
            int _index = this.FindIndexKey(_idFirts, _dataAll);
            int _index_stop = 0;

            _index += _shiftstep;

            if (_index < 0) { _index = 0; }

            _index_stop = _index + _count;

            
            if (_index_stop >= _dataAll.Count) { _index_stop = _dataAll.Count - 1; }

            for (int i = _index; i < _index_stop; i++)
            {
                result.Add(_dataAll.ElementAt(i).Key, _dataAll.ElementAt(i).Value);
            }

            return result;
        }

        /// <summary>
        /// Сжимаем/растягиваем график
        /// </summary>
        public DataUniversalCharts StretchCharts(Dictionary<string, double> _dataOriginal, Dictionary<string, double> _dataAll, int _step)
        {
            DataUniversalCharts result = new DataUniversalCharts();

            if (_dataOriginal.Count == 0) 
            { return result; }

            int _datacount = _dataAll.Count;
            int _newcount = _dataOriginal.Count + _step;
            string _idFirts = _dataOriginal.ElementAt(0).Key;
            int _index = this.FindIndexKey(_idFirts, _dataAll);

            if (_newcount <= 0) { _newcount = 1; }

            for (int i = 0; i < _newcount; i++)
            {
                if (_index + i >= _datacount) { break; }
                result.Add(_dataAll.ElementAt(_index + i).Key, _dataAll.ElementAt(_index + i).Value);
            }
            return result;
        }

        /// <summary>
        /// Первый показ графика
        /// </summary>
        public DataUniversalCharts FirstVievCharts(DataUniversalCharts _inputData)
        {
            DataUniversalCharts result = new DataUniversalCharts();

            int i = 0;

            foreach (var item in _inputData)
            {
                i++;
                result.Add(item.Key, item.Value);
                if (i >= count)
                {
                    break;
                }
            }

            return result;
        }
        public DataUniversalCharts FirstVievCharts(DataUniversalCharts _inputData, int _numcount)
        {
            DataUniversalCharts result = new DataUniversalCharts();
            int _count = 0;
            int i = 0;

            switch (_numcount)
            {
                case 0:
                    _count = count;
                    break;
                case 1:
                    _count = count1;
                    break;
                case 2:
                    _count = count2;
                    break;
            }

            foreach (var item in _inputData)
            {
                i++;
                result.Add(item.Key, item.Value);
                if (i >= _count)
                {
                    break;
                }
            }

            return result;
        }
        //----------------------------------------------------------

        // Ищем индекс ключа
        private int FindIndexKey(string _key, Dictionary<string, double> _data)
        {
            int index = -1;

            for (int i = 0; i < _data.Count; i++)
            {
                if (_key == _data.ElementAt(i).Key)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Возвращает данные для графика(прибыли по сделкам). Десериализация сохраненного файла
        /// </summary>
        public TradesProfitCharts GetDataForCharts(string _path)
        {
            TradesProfitCharts result = new TradesProfitCharts();
            TestTradesCollection _testTradCol = (TestTradesCollection)StaticService.Deserializes(Directory.GetCurrentDirectory() + _path);

            foreach (ParametrTestTrades _testTrd in _testTradCol)
            {
                result.AddInDictionary(_testTrd.DateTimeTestTrad, _testTrd.ProfitPortfolio);
            }

            return result;
        }

        /// <summary>
        /// График по SettingsBolinger(X) и CountProfitIndex(Y)
        /// </summary>
        public DataUniversalCharts GetSettingsBBForCharts(TestResultRepositiry _testResRepo)
        {
            DataUniversalCharts result = new DataUniversalCharts();

            var _groupRes = from res in _testResRepo
                            where !String.IsNullOrEmpty(StaticService.GetComment(res.Key, Patern3))
                            group res by res.SettingsBolinger into _paramRes
                            orderby _paramRes.Key
                            select _paramRes;

            foreach (var item in _groupRes)
            {
                string keyX = item.Key;
                double avgY = item.Average(x => x.CountProfitIndex);
                result.Add(keyX, avgY);
            }

            return result;
        }
    }
}
