using Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    /// <summary>
    /// Класс пользовательских настроек
    /// </summary>
    [Serializable]
    public class SettingsClass : PropertyChangedBase
    {
        // предустановки
        public const double slipPrice = 3;    // проскальзывание при испонении по рынку

        /// <summary>
        /// "\DataFiles\AllTrades_Inp"
        /// </summary>
        public const string AllTrades_Inp = @"\DataFiles\AllTrades_Inp";
        /// <summary>
        /// "\DataFiles\AllTrades_bb"
        /// </summary>
        public const string AllTrades_bb = @"\DataFiles\AllTrades_bb";
        /// <summary>
        /// \DataFiles\TestTrades
        /// </summary>
        public const string TestTrades = @"\DataFiles\TestTrades";
        /// <summary>
        /// \DataFiles\TestResult
        /// </summary>
        public const string TestResult = @"\DataFiles\TestResult";
        /// <summary>
        /// \Simple\
        /// </summary>
        public const string FolderSimple = @"\Simple\";
        /// <summary>
        /// \OPT\
        /// </summary>
        public const string FolderOPT = @"\OPT\";
        /// <summary>
        /// "\SBRF\"
        /// </summary>
        public const string FolderSBRF = @"\SBRF\";

        /// <summary>
        /// Дата в формате YYYY-MM-DD
        /// </summary>
        public const string PaternDate = @"[0-9]{4}-(0[1-9]|1[012])-(0[1-9]|1[0-9]|2[0-9]|3[01])"; //Дата в формате YYYY-MM-DD
        /// <summary>
        /// Дата в формате DD.MM.YYYY
        /// </summary>
        public const string PaternDate1 = @"(0[1-9]|1[0-9]|2[0-9]|3[01]).(0[1-9]|1[012]).[0-9]{4}"; //Дата в формате DD.MM.YYYY
        /// <summary>
        /// для получения нужного фрагмента комментария
        /// </summary>
        public const string Patern = @"[_]\d+";
        /// <summary>
        /// для получения любого числа
        /// </summary>
        public const string Patern2 = @"\d+";
        //---end

        private int _countPeriodBB_Start;     // количество периодов в ББ
        private double _countStdDevBB_Start;  // количество отклонений в ББ
        
        private int _periodBB_Step;
        private double _stdDevBB_step;

        private int _countStepPeriod;
        private int _countStepStdDev;

        #region -Constructor-
        public SettingsClass()
        {
            _countPeriodBB_Start = 600;
            _countStdDevBB_Start = 0.5;
            _periodBB_Step = 10;
            _stdDevBB_step = 0.1;
            _countStepPeriod = 10;
            _countStepStdDev = 10;
        }
        #endregion

        #region -Properties-
        public int CountPeriodBB_Start
        {
            get 
            {
                if (_countPeriodBB_Start < 1)
                {
                    _countPeriodBB_Start = 1;
                    return _countPeriodBB_Start;
                }
                return _countPeriodBB_Start;
            }
            set
            {
                if (value >= 1)
                {
                    _countPeriodBB_Start = value;
                }
                else
                {
                    _countPeriodBB_Start = 1;
                }
                base.NotifyPropertyChanged();
                StaticService.Serializes(this, this.GetType().ToString());
            }
        }
        public double CountStdDevBB_Start
        {
            get
            {
                if (_countStdDevBB_Start < 0.1)
                {
                    _countStdDevBB_Start = 0.1;
                    return _countStdDevBB_Start;
                }
                return _countStdDevBB_Start;
            }
            set
            {
                if (value >= 0.1)
                {
                    _countStdDevBB_Start = value;
                }
                else
                {
                    _countStdDevBB_Start = 0.1;
                }
                base.NotifyPropertyChanged();
                StaticService.Serializes(this, this.GetType().ToString());
            }
        }

        public int PeriodBBStep
        {
            get { return _periodBB_Step; }
            set
            {
                if (value > 0)
                {
                    _periodBB_Step = value;
                    base.NotifyPropertyChanged();
                    StaticService.Serializes(this, this.GetType().ToString());
                }
            }
        }
        public double StdDevBBStep
        {
            get { return _stdDevBB_step; }
            set
            {
                if (value > 0)
                {
                    _stdDevBB_step = value;
                    base.NotifyPropertyChanged();
                    StaticService.Serializes(this, this.GetType().ToString());
                }
            }
        }

        public int CountStepPeriod
        {
            get { return _countStepPeriod; }
            set
            {
                if (value > 0)
                {
                    _countStepPeriod = value;
                    base.NotifyPropertyChanged();
                    StaticService.Serializes(this, this.GetType().ToString());
                }
            }
        }
        public int CountStepStdDev
        {
            get { return _countStepStdDev; }
            set
            {
                if (value > 0)
                {
                    _countStepStdDev = value;
                    base.NotifyPropertyChanged();
                    StaticService.Serializes(this, this.GetType().ToString());
                }
            }
        }
        #endregion
    }
}
