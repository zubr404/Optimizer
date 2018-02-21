using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Bases;
using Algoritms;

namespace Optimizer
{
    public class ViewModel : PropertyChangedBase
    {
        /// <summary>
        /// для получения любого числа
        /// </summary>
        public const string Patern = @"\d+";
        /// <summary>
        /// любой знак, не являющейся цифрой
        /// </summary>
        public const string Patern1 = @"\D+";
        /// <summary>
        /// получает заглавные буквы
        /// </summary>
        public const string Patern3 = @"\p{Lu}+";

        private RepositiryClasses repositClasses;
        MarketTradesRepository marketTradRepo;

        //
        private TestResultRepositiry testResultRepo;
        private TestResultRepositiry testResultBufer; // исходная коллекция после теста
        private ParametrTestResult selectedTestResult;

        // работа через файлы
        private ConvertToDat convertToDat;
        private CalculationBolinger calcBolinger;
        // ---end

        //
        private SettingsClass settingsClass;
        private TestMarketDriver testMarketDriver;
        private IntradayAdaptation intraAdapt;

        // Chart
        private ManipulationCharts manipulationCharts;
        private TradesProfitCharts tradProfChart;
        private TradesProfitCharts tradProfChart1;

        private DataUniversalCharts settBBCharts;

        private string legend1; // для  легенды
        private string legend2;

        private string select_keyNum;    // для ключа(числовая часть) выбранного элемента в DataGrid
        private string select_keyLetter; // для ключа(буквенная часть) выбранного элемента в DataGrid
        //----end

        //
        private bool isEnabledChartsProfit;
        private bool isEnabledChartsSettBB;

        // Сортировка таблицы
        private bool isCheckOTP; // показывать только ОРТ
        private DateTime oneDay; // один день
        private DateTime fromDate; // от даты
        private DateTime toDate; // до даты
        private string settBB_Filtr; // по параметрам ББ
        private int countBest; // количество лучших результатов
        ObservableCollection<ParametrGroupSettingsResult> groupParametrBBObs;
        //--------

        #region -Constructor-
        public ViewModel()
        {
            repositClasses = new RepositiryClasses();
            manipulationCharts = new ManipulationCharts();

            marketTradRepo = repositClasses.MarketTradRepo;
            testResultBufer = repositClasses.TestResReposit;

            settingsClass = repositClasses.SettClass;
            testMarketDriver = repositClasses.TestmarketDriver;
            intraAdapt = new IntradayAdaptation();

            // работа через файлы
            convertToDat = repositClasses.ConvToDat;
            calcBolinger = repositClasses.CalcBolinger;

            fromDate = DateTime.Now;
            toDate = fromDate;
            oneDay = fromDate;
            countBest = 1;
        }
        #endregion

        #region -Properties-
        public SettingsClass SettClass
        {
            get { return settingsClass; }
            set
            {
                settingsClass = value;
                base.NotifyPropertyChanged();
            }
        }
        public TestResultRepositiry TestResultRepo
        {
            get { return testResultRepo; }
            set
            {
                testResultRepo = value;
                base.NotifyPropertyChanged();
            }
        }
        public ParametrTestResult SelectedTestResult
        {
            get { return selectedTestResult; }
            set
            {
                selectedTestResult = value;
                base.NotifyPropertyChanged();

                string _k = selectedTestResult.Key;
                string _delete = StaticService.GetComment(_k, Patern3);

                if (String.IsNullOrEmpty(_delete))
                {
                    select_keyNum = _k;
                }
                else
                {
                    select_keyNum = _k.Replace(_delete, String.Empty);
                }
            }
        }

        // Charts
        public TradesProfitCharts TradProfCharts
        {
            get { return tradProfChart; }
            set
            {
                tradProfChart = value;
                base.NotifyPropertyChanged();
            }
        }
        public TradesProfitCharts TradProfCharts1
        {
            get { return tradProfChart1; }
            set
            {
                tradProfChart1 = value;
                base.NotifyPropertyChanged();
            }
        }
        public DataUniversalCharts SettBBCharts
        {
            get { return settBBCharts; }
            set
            {
                settBBCharts = value;
                base.NotifyPropertyChanged();
            }
        }

        public string Legend1
        {
            get { return legend1; }
            set
            {
                legend1 = value;
                base.NotifyPropertyChanged();
            }
        }
        public string Legend2
        {
            get { return legend2; }
            set
            {
                legend2 = value;
                base.NotifyPropertyChanged();
            }
        }
        //---end

        //
        public bool IsEnabledChartsProfit
        {
            get { return isEnabledChartsProfit; }
            set 
            { 
                isEnabledChartsProfit = value;
                base.NotifyPropertyChanged();
            }
        }
        public bool IsEnabledChartsSettBB
        {
            get { return isEnabledChartsSettBB; }
            set
            {
                isEnabledChartsSettBB = value;
                base.NotifyPropertyChanged();
            }
        }

        // Сортировка таблицы
        public bool IsCheckOTP
        {
            get { return isCheckOTP; }
            set
            {
                isCheckOTP = value;
                base.NotifyPropertyChanged();

                bool error;
                TestResultRepo = FiltrTestResult(testResultBufer, fromDate, toDate, out error, isCheckOTP);

                if (error)
                {
                    MessageBox.Show("TestResultRepositiry содержит елементы NULL.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        public DateTime OneDay
        {
            get { return oneDay; }
            set
            {
                oneDay = value;
                base.NotifyPropertyChanged();

                bool error;
                TestResultRepo = FiltrTestResult(testResultBufer, oneDay, oneDay, out error, isCheckOTP);

                if (error)
                {
                    MessageBox.Show("TestResultRepositiry содержит елементы NULL.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        public DateTime FromDate
        {
            get 
            {
                if (fromDate == null)
                {
                    return DateTime.Now;
                }
                return fromDate;
            }
            set
            {
                fromDate = value;
                base.NotifyPropertyChanged();

                bool error;
                TestResultRepo = FiltrTestResult(testResultBufer, fromDate, toDate, out error, isCheckOTP);

                if (error)
                {
                    MessageBox.Show("TestResultRepositiry содержит елементы NULL.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        public DateTime ToDate
        {
            get
            {
                if (toDate == null)
                {
                    return DateTime.Now;
                }
                return toDate;
            }
            set
            {
                toDate = value;
                base.NotifyPropertyChanged();

                bool error;
                TestResultRepo = FiltrTestResult(testResultBufer, fromDate, toDate, out error, isCheckOTP);

                if (error)
                {
                    MessageBox.Show("TestResultRepositiry содержит елементы NULL.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }
        public string SettBB_Filtr
        {
            get { return settBB_Filtr; }
            set
            {
                settBB_Filtr = value;
                base.NotifyPropertyChanged();
            }
        }
        public int CountBest
        {
            get { return countBest; }
            set
            {
                if (value > 0)
                {
                    countBest = value;
                    base.NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<ParametrGroupSettingsResult> GroupParametrBBObs
        {
            get { return groupParametrBBObs ?? (groupParametrBBObs = new ObservableCollection<ParametrGroupSettingsResult>()); }
            set
            {
                if (value != null)
                {
                    groupParametrBBObs = value;
                    base.NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region -Command-

        #region -Кнопка сохранить все сделки-
        private RelayCommand _saveAllTradCommand;
        public RelayCommand SaveAllTradCommand
        {
            get
            {
               return _saveAllTradCommand ?? (_saveAllTradCommand = new RelayCommand(
                    (object arg) => 
                    {
                        if (MessageBox.Show("Сохранить сделки?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            StaticService.Serializes(marketTradRepo, marketTradRepo.GetType().ToString());
                        }
                    }
                    ));
            }
        }
        #endregion

        #region -Кнопка конвертировать в .dat-
        private RelayCommand _convToDatCommand;
        public RelayCommand ConvToDatCommand
        {
            get
            {
                return _convToDatCommand ?? (_convToDatCommand = new RelayCommand(
                    (object arg) => 
                    {
                        convertToDat.ConvertAsyncDiagnostic();
                    }
                    ));
            }
        }
        #endregion

        #region -Кнопка Расчитать ББ по множеству вариантов-
        private RelayCommand _calcBBCommand;
        public RelayCommand CalcBBCommand
        {
            get
            {
                return _calcBBCommand ?? (_calcBBCommand = new RelayCommand(
                    async (object arg) => 
                    {
                        if (MessageBox.Show("Запустить расчет?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            string _endcalc = await calcBolinger.CalcBBAsync(SettClass);
                            MessageBox.Show(_endcalc);
                        }
                    }
                    ));
            }
        }
        #endregion

        #region -Кнопка запустить тест-
        private RelayCommand _startTestCommand;
        public RelayCommand StartTestCommand
        {
            get
            {
                return _startTestCommand ?? (_startTestCommand = new RelayCommand(
                    async (object arg) => 
                    {
                        if (MessageBox.Show("Запустить тест?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            testResultBufer = await testMarketDriver.TestAsync();
                            MessageBox.Show("Тест окoнчен.");
                        }
                    }
                    ));
            }
        }
        #endregion-Кнопка Тест сегодня-
        
        #region -Кнопка тест сегодня-
        ExtremumPrice extremumPrice;

        private RelayCommand _testTodayCommand;
        public RelayCommand TestTodayCommand
        {
            get
            {
                return _testTodayCommand ?? (_testTodayCommand = new RelayCommand((object arg) =>
                {
                    extremumPrice = testMarketDriver.TestTradesToday1();
                }));
            }
        }
        #endregion

        #region -Кнопка тест сегодня ОПТИМИЗАЦИЯ-
        private RelayCommand _testTodayAsyncCommand;
        public RelayCommand TestTodayAsyncCommand
        {
            get
            {
                return _testTodayAsyncCommand ?? (_testTodayAsyncCommand = new RelayCommand(async (object arg) =>
                {
                    await testMarketDriver.TestTradesTodayAsync();
                    MessageBox.Show("Расчет закончен.");
                }));
            }
        }
        #endregion

        #region -Кнопка ADAPTATION-
        private RelayCommand _adaptationCommand;
        public RelayCommand AdaptationCommand
        {
            get
            {
                return _adaptationCommand ?? (_adaptationCommand = new RelayCommand((object arg) => 
                {
                    //intraAdapt.AdaptationAsync();
                    MovingExtremum _mex = new MovingExtremum();
                    _mex.PointInput();
                    MessageBox.Show("Расчет (adaptation) закончен.");
                }));
            }
        }
        #endregion

        #region -Построить график прибыли при каждой сделке-
        private RelayCommand _isChartsProfitCommand;
        public RelayCommand IsChartsProfitCommand
        {
            get
            {
                return _isChartsProfitCommand ?? (_isChartsProfitCommand = new RelayCommand(
                    (object arg) => 
                    {
                        if (IsEnabledChartsProfit)
                        {
                            IsEnabledChartsProfit = false;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(select_keyNum))
                            {
                                TradProfCharts = manipulationCharts.GetDataForCharts(SettingsClass.TestTrades + SettingsClass.FolderSBRF + SettingsClass.FolderSimple + select_keyNum + ".dat");
                                TradProfCharts1 = manipulationCharts.GetDataForCharts(SettingsClass.TestTrades + SettingsClass.FolderSBRF + SettingsClass.FolderOPT + select_keyNum + ".dat");
                                Legend1 = select_keyNum;
                                Legend2 = select_keyNum + "OPT";

                                IsEnabledChartsProfit = true;
                            }
                        }
                        
                    }
                    ));
            }
        }
        #endregion

        #region -Построить график SettingsBolinger-
        private RelayCommand _isChartsSettBBCommand;
        public RelayCommand IsChartsSettBBCommand
        {
            get
            {
                return _isChartsSettBBCommand ?? (_isChartsSettBBCommand = new RelayCommand((object arg) => 
                {
                    if (IsEnabledChartsSettBB)
                    {
                        IsEnabledChartsSettBB = false;
                    }
                    else
                    {
                        SettBBCharts = manipulationCharts.GetSettingsBBForCharts(testResultBufer);
                        IsEnabledChartsSettBB = true;
                    }
                }));
            }
        }
        #endregion

        #region -Универсальный график-
        DataUniversalCharts allDataForCharts1;
        DataUniversalCharts allDataForCharts2;
        DataUniversalCharts allDataForCharts3;

        List<DataUniversalCharts> _chartListAll = new List<DataUniversalCharts>();
        List<DataUniversalCharts> _chartListOriginal;

        private bool isEnabledChartUniver;
        public bool IsEnabledChartUniver
        {
            get { return isEnabledChartUniver; }
            set
            {
                isEnabledChartUniver = value;
                base.NotifyPropertyChanged();
            }
        }

        private DataUniversalCharts chartValues1;
        public DataUniversalCharts ChartValues1
        {
            get { return chartValues1 ?? (chartValues1 = new DataUniversalCharts()); }
            set
            {
                chartValues1 = value;
                base.NotifyPropertyChanged();
            }
        }
        private DataUniversalCharts chartValues2;
        public DataUniversalCharts ChartValues2
        {
            get { return chartValues2 ?? (chartValues2 = new DataUniversalCharts()); }
            set
            {
                chartValues2 = value;
                base.NotifyPropertyChanged();
            }
        }
        private DataUniversalCharts chartValues3;
        public DataUniversalCharts ChartValues3
        {
            get { return chartValues3 ?? (chartValues3 = new DataUniversalCharts()); }
            set
            {
                chartValues3 = value;
                base.NotifyPropertyChanged();
            }
        }

        // открываем график
        private RelayCommand _isChartUniverCommand;
        public RelayCommand IsChartUniverAllTradesCommand
        {
            get
            {
                return _isChartUniverCommand ?? (_isChartUniverCommand = new RelayCommand((object arg) => 
                {
                    if (isEnabledChartUniver)
                    {
                        IsEnabledChartUniver = false;
                    }
                    else
                    {
                        MarketTradesRepository mTR = new MarketTradesRepository();
                        mTR = (MarketTradesRepository)StaticService.Deserializes(mTR.GetType().ToString(), mTR);
                        allDataForCharts1 = manipulationCharts.CreateDataCharts<MarketTradesRepository, ParametrMarketTrades>(mTR);

                        ChartValues1 = manipulationCharts.CreateDataCharts<MarketTradesRepository, ParametrMarketTrades>(mTR);
                        IsEnabledChartUniver = true;
                    }
                }));
            }
        }
        public RelayCommand IsChartUniverExtremumPriceCommand
        {
            get
            {
                return _isChartUniverCommand ?? (_isChartUniverCommand = new RelayCommand((object arg) =>
                {
                    if (isEnabledChartUniver)
                    {
                        IsEnabledChartUniver = false;
                    }
                    else
                    {
                        if (extremumPrice == null) { return; }

                        allDataForCharts1 = manipulationCharts.CreateDataCharts<Queue<ExtremumPriceParametr>, ExtremumPriceParametr>(extremumPrice.Extremums);
                        allDataForCharts2 = manipulationCharts.CreateDataCharts<Queue<ExtremumPriceParametr>, ExtremumPriceParametr>(extremumPrice.ExtremumsDistance);
                        allDataForCharts3 = extremumPrice.DataUniverCharts;

                        _chartListAll.Add(allDataForCharts1);
                        _chartListAll.Add(allDataForCharts2);
                        _chartListAll.Add(allDataForCharts3);

                        _chartListOriginal = manipulationCharts.ChartsPull(_chartListAll, ActionCharts.None);

                        ChartValues1 = _chartListOriginal[0];
                        ChartValues2 = _chartListOriginal[1];
                        ChartValues3 = _chartListOriginal[2];

                        IsEnabledChartUniver = true;
                    }
                }));
            }
        }

        // двигаем график
        private RelayCommand _backShiftCommand;
        public RelayCommand BackShiftCommand
        {
            get { return _backShiftCommand ?? (_backShiftCommand = new RelayCommand((object arg) => 
            {
                _chartListOriginal = manipulationCharts.ChartsPull(_chartListAll, ActionCharts.ShiftBack, _chartListOriginal);

                ChartValues1 = _chartListOriginal[0];
                ChartValues2 = _chartListOriginal[1];
                ChartValues3 = _chartListOriginal[2];
            })); }
        }
        private RelayCommand _forwardShiftCommand;
        public RelayCommand ForwardShiftCommand
        {
            get
            {
                return _forwardShiftCommand ?? (_forwardShiftCommand = new RelayCommand((object arg) =>
                {
                    _chartListOriginal = manipulationCharts.ChartsPull(_chartListAll, ActionCharts.ShiftForvard, _chartListOriginal);

                    ChartValues1 = _chartListOriginal[0];
                    ChartValues2 = _chartListOriginal[1];
                    ChartValues3 = _chartListOriginal[2];
                }));
            }
        }
        //-*-*-*-*-*-*-*-

        // Сжимаем/растягиваем график
        private RelayCommand _compressCommand;
        public RelayCommand CompressCommand
        {
            get
            {
                return _compressCommand ?? (_compressCommand = new RelayCommand((object arg) =>
                {
                    _chartListOriginal = manipulationCharts.ChartsPull(_chartListAll, ActionCharts.Compress, _chartListOriginal);

                    ChartValues1 = _chartListOriginal[0];
                    ChartValues2 = _chartListOriginal[1];
                    ChartValues3 = _chartListOriginal[2];
                }));
            }
        }
        private RelayCommand _stretchCommand;
        public RelayCommand StretchCommand
        {
            get
            {
                return _stretchCommand ?? (_stretchCommand = new RelayCommand((object arg) =>
                {
                    _chartListOriginal = manipulationCharts.ChartsPull(_chartListAll, ActionCharts.Stretch, _chartListOriginal);

                    ChartValues1 = _chartListOriginal[0];
                    ChartValues2 = _chartListOriginal[1];
                    ChartValues3 = _chartListOriginal[2];
                }));
            }
        }
        //-*-*-*-*-*-*-*-

        
        //-*-*-*-*-*-*-*-

        

        #endregion

        #region -Кнопка Sett. BB-
        private RelayCommand _settBBCommand;
        public RelayCommand SettBBCommand
        {
            get
            {
                return _settBBCommand ?? (_settBBCommand = new RelayCommand((object arg) => 
                {
                    bool error;
                    TestResultRepo = FiltrBySettingsBB(testResultBufer, settBB_Filtr, out error);

                    if (error)
                    {
                        MessageBox.Show("TestResultRepositiry содержит елементы NULL.", "Warning.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }));
            }
        }
        #endregion

        #region -Кнопка Best-
        private RelayCommand _bestCommand;
        public RelayCommand BestCommand
        {
            get
            {
                return _bestCommand ?? (_bestCommand = new RelayCommand((object arg) => 
                {
                    TestResultRepo = GroupDateTestResult(testResultBufer, countBest);
                    GroupParametrBBObs = GroupParametrBBTestResult(testResultRepo);
                }));
            }
        }
        #endregion

        #region -Кнопка ALL-
        private RelayCommand _allCommand;
        public RelayCommand AllCommand
        {
            get
            {
                return _allCommand ?? (_allCommand = new RelayCommand(
                    (object arg) => { TestResultRepo = testResultBufer; }
                    ));
            }
        }
        #endregion

        #endregion

        #region -Method-

        #region -Charts-
        
        #endregion

        #region -Filtr-
        /// <summary>
        /// Фильтр по ОРТ
        /// </summary>
        private TestResultRepositiry FiltrTestResult(TestResultRepositiry _testResRepo, DateTime _fromDate, DateTime _toDate, out bool _error, bool _opt = true)
        {
            TestResultRepositiry result = new TestResultRepositiry();
            _error = false;

            try
            {
                foreach (ParametrTestResult item in _testResRepo)
                {
                    if (item != null)
                    {
                        string OPT = StaticService.GetComment(item.Key, Patern3);
                        DateTime dateRes = Convert.ToDateTime(item.DateRes);

                        if (dateRes >= _fromDate && dateRes <= _toDate)
                        {
                            if (_opt)
                            {
                                if (!String.IsNullOrEmpty(OPT))
                                {
                                    result.Add(item);
                                }
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(OPT))
                                {
                                    result.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        _error = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException + ex.Message);
                return result;
            }

            return result;
        }

        /// <summary>
        /// Фильтр по SettingsBolinger только для ОПТ
        /// </summary>
        private TestResultRepositiry FiltrBySettingsBB(TestResultRepositiry _testResRepo, string _setBB, out bool _error)
        {
            TestResultRepositiry result = new TestResultRepositiry();
            _error = false;

            try
            {
                foreach (ParametrTestResult item in _testResRepo)
                {
                    if (item != null)
                    {
                        string OPT = StaticService.GetComment(item.Key, Patern3);
                        string settingsBB = item.SettingsBolinger;

                        if (settingsBB == _setBB && !String.IsNullOrEmpty(OPT))
                        {
                            result.Add(item);
                        }
                    }
                    else
                    {
                        _error = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException + ex.Message);
                return result;
            }

            return result;
        }
        #endregion

        #region -Group-
        /// <summary>
        /// Группировка по Дате TestResultRepositiry только для OPT
        /// </summary>
        private TestResultRepositiry GroupDateTestResult(TestResultRepositiry _testResults, int _countBest = 1)
        {
            TestResultRepositiry result = new TestResultRepositiry();
            int counter;

            var _groupRes = from res in _testResults
                            where res.Profit > 0 & StaticService.GetComment(res.Key, Patern3) == "OPT"
                            orderby res.Profit descending
                            group res by res.DateRes into _dateGroup
                            orderby _dateGroup.Key
                            select _dateGroup;

            foreach (var testresults in _groupRes)
            {
                counter = 0;

                if (testresults != null)
                {
                    foreach (ParametrTestResult res in testresults)
                    {
                        result.Add(res);

                        counter++;

                        if (counter >= _countBest)
                        {
                            break;
                        }


                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Группировка по Параметрам ББ TestResultRepositiry
        /// </summary>
        private ObservableCollection<ParametrGroupSettingsResult> GroupParametrBBTestResult(TestResultRepositiry _testResults)
        {
            ObservableCollection<ParametrGroupSettingsResult> result = new ObservableCollection<ParametrGroupSettingsResult>();

            var _groupRes = from res in _testResults
                            group res by res.SettingsBolinger into _paramRes
                            select new {Papametr = _paramRes.Key, Count = _paramRes.Count()};

            foreach (var item in _groupRes)
            {
                result.Add(new ParametrGroupSettingsResult(item.Papametr, item.Count));
            }

            return result;
        }
        #endregion

        #endregion
    }
}
