using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Optimizer
{
    /// <summary>
    /// Принцип переносимой библиотеки классов.
    /// ЭКЗЕМПЛЯРЫ ВСЕХ КЛАССОВ ДОЛЖНЫ СОДАВАТЬСЯ ЗДЕСЬ
    /// </summary>
    public class RepositiryClasses : IDisposable
    {
        // Dictionary for dde channel and object
        Dictionary<string, object> _objectRepository;

        // dde (экземпляры создаются с DDEChannelCreate)
        AllTradesChannel allTradesChannel;
        CurrentTableChannel currentTableChannel;
        DDEinfrastructure ddeInfra;

        // Repository
        MarketTradesRepository marketTradesRepository;
        TestResultRepositiry testResultRepository;

        // Driver
        ObtainingDriver obtainingDriver;
        TestMarketDriver testMarketDriver;
        ConvertToDat convertToDat;
        CalculationBolinger calcBolinger;

        //
        SettingsClass settingsClass;

        #region -Constructor-
        public RepositiryClasses()
        {
            DDEChannelCreate();

            // --1--
            marketTradesRepository = new MarketTradesRepository();
            testResultRepository = new TestResultRepositiry();
            settingsClass = new SettingsClass();

            // --2--
            // восстанавливаем сохраненные экземпляры
            DeserializesObject();

            // последний
            obtainingDriver = new ObtainingDriver(this);
            convertToDat = new ConvertToDat();
            calcBolinger = new CalculationBolinger();
            testMarketDriver = new TestMarketDriver();
        }
        #endregion

        #region -Properties-
        public MarketTradesRepository MarketTradRepo
        {
            get { return marketTradesRepository; }
        }
        public TestResultRepositiry TestResReposit
        {
            get { return testResultRepository; }
        }
        public ObtainingDriver ObtainingDriv
        {
            get { return obtainingDriver; }
        }
        public ConvertToDat ConvToDat
        {
            get { return convertToDat; }
        }
        public CalculationBolinger CalcBolinger
        {
            get { return calcBolinger; }
        }
        public TestMarketDriver TestmarketDriver
        {
            get { return testMarketDriver; }
        }
        public SettingsClass SettClass
        {
            get { return settingsClass; }
        }
        #endregion

        #region -Method-
        // создание экземпляров dde экспортов и запуск конструктора DDEinfrastructure()
        private void DDEChannelCreate()
        {
            _objectRepository = new Dictionary<string, object>();

            allTradesChannel = new AllTradesChannel();
            currentTableChannel = new CurrentTableChannel();

            AddReference(_objectRepository, allTradesChannel.GetType().Name, allTradesChannel);
            AddReference(_objectRepository, currentTableChannel.GetType().Name, currentTableChannel);

            ddeInfra = new DDEinfrastructure(this);
        }

        /// <summary>
        /// Десериализация сохраненных объектов
        /// </summary>
        private void DeserializesObject()
        {
            settingsClass = (SettingsClass)StaticService.Deserializes(settingsClass.GetType().ToString(), settingsClass);
            testResultRepository = (TestResultRepositiry)StaticService.Deserializes(StaticService.RelativePatchCreate(SettingsClass.TestResult + SettingsClass.FolderSBRF) + "!_actul_result", new TestResultRepositiry());
        }

        #region - ХРАНИЛИЩЕ ЭКЗЕМПЛЯРОВ IReference -
        // добавление ссылок в ObjectRepository
        private void AddReference(Dictionary<string, object> dict_col, string type, object obj)
        {
            dict_col.Add(type, obj);
        }

        // получение ссылок из ObjectRepository
        public object GetReference(string type)
        {
            try
            {
                return _objectRepository[type];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.TargetSite + " ( " + type + " ):" + ex.Message + "\nПриложение будет зарыто.", "Critical error.", MessageBoxButton.OK);
                // Это применить только после отрисовки формы. mww.Close();
                return null;
            }

        }
        #endregion

        #endregion

        #region - Implement IDisposable -
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ddeInfra.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
