using XlDde;
using System;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Optimizer
{

    #region class CurrentTableChannel текущая таблица параметров
    class CurrentTableChannel : XlDdeChannel, INotifyPropertyChanged
    {
        private string time_change;
        private decimal price_trade;
        private decimal bid;
        private decimal offer;
        private decimal high_possible_price;
        private decimal minimum_possible_price;
        private decimal step_price;
        private string stockcode;
        private string classcode;
        private string time_trade;

        #region -PROPERTIES-
        public string Time_change 
        {
            get { return time_change; }
            set
            {
                if (time_change != value)
                {
                    time_change = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal Price_trade
        {
            get { return price_trade; }
            set 
            {
                if (price_trade != value)
                {
                    price_trade = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal Bid
        {
            get { return bid; }
            set
            {
                if (bid != value)
                {
                    bid = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public decimal Offer 
        {
            get { return offer; }
            set
            {
                if (offer != value)
                {
                    offer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal High_possible_price 
        {
            get { return high_possible_price; }
            set
            {
                if (high_possible_price != value)
                {
                    high_possible_price = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public decimal Minimum_possible_price 
        {
            get { return minimum_possible_price; }
            set
            {
                if (minimum_possible_price != value)
                {
                    minimum_possible_price = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public decimal Step_price 
        {
            get { return step_price; }
            set
            {
                if (step_price != value)
                {
                    step_price = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public string StockCode 
        {
            get { return stockcode; }
            set
            {
                if (stockcode != value)
                {
                    stockcode = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public string ClassCode 
        {
            get { return classcode; }
            set
            {
                if (classcode != value)
                {
                    classcode = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public string Time_trade 
        {
            get { return time_trade; }
            set
            {
                if (time_trade != value)
                {
                    time_trade = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        protected override void ProcessTable(XlTable xt)
        {
            for (int row = 0; row < xt.Rows; row++)
            {
                for (int col = 0; col < xt.Columns; col++)
                {
                    xt.ReadValue();

                    switch (col)
                    {
                        case 0:
                            Time_change = xt.StringValue;
                            break;

                        case 1:
                            Price_trade = (decimal)xt.FloatValue;
                            break;

                        case 2:
                            Bid = (decimal)xt.FloatValue;
                            break;

                        case 3:
                            Offer = (decimal)xt.FloatValue;
                            break;

                        case 4:
                            High_possible_price = (decimal)xt.FloatValue;
                            break;

                        case 5:
                            Minimum_possible_price = (decimal)xt.FloatValue;
                            break;

                        case 6:
                            Step_price = (decimal)xt.FloatValue;
                            break;

                        case 7:
                            StockCode = xt.StringValue;
                            break;

                        case 8:
                            ClassCode = xt.StringValue;
                            break;

                        case 9:
                            Time_trade = xt.StringValue;
                            break;
                    }
                }
            }
        }

        #region Implementation INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
    #endregion

    #region class AllTradesChannel таблица всех сделок
    class AllTradesChannel : XlDdeChannel, IExportDDE
    {
        public double Number { get; private set; }
        public string Security { get; private set; }
        public decimal Price { get; private set; }
        public string Date { get; private set; }
        public string Time { get; private set; }
        public string TimeMsk { get; private set; }
        public string Operation { get; private set; }
        public decimal Quantity { get; private set; }

        protected override void ProcessTable(XlTable xt)
        {
            ObtainingDataStartedEvent(this, EventArgs.Empty);

            int xtRows = xt.Rows;
            SetCountRowsExport(xtRows);

            for (int row = 0; row < xtRows; row++)
            {
                xt.ReadValue();
                Number = xt.FloatValue;

                xt.ReadValue();
                Security = xt.StringValue;

                xt.ReadValue();
                Price = (decimal)xt.FloatValue;

                xt.ReadValue();
                Date = xt.StringValue;

                xt.ReadValue();
                Time = xt.StringValue;

                xt.ReadValue();
                TimeMsk = xt.FloatValue.ToString();

                xt.ReadValue();
                Operation = xt.StringValue;

                xt.ReadValue();
                Quantity = (decimal)xt.FloatValue;

                LoadedLineEvent(this, EventArgs.Empty);
            }
            ObtainingDataCompletedEvent(this, EventArgs.Empty);
        }

        #region реализация IExportDDE
        public event EventHandler LoadedLineEvent = delegate { };
        public event EventHandler ObtainingDataCompletedEvent = delegate { };
        public event EventHandler ObtainingDataStartedEvent = delegate { };

        private int countRowsExport;

        public int CountRowsExport
        {
            get { return countRowsExport; }
        }

        public void SetCountRowsExport(int value)
        {
            countRowsExport = value;
        }
        #endregion
    }
    #endregion

}