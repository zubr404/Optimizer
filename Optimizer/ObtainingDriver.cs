using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    /// <summary>
    /// Класс получения инфы из DDE
    /// </summary>
    public class ObtainingDriver
    {
        // dde
        AllTradesChannel _allTradChannel;

        //
        MarketTradesRepository marketTradRepo;
        SettingsClass settClass;

        //
        int id;

        #region -Constructor-
        public ObtainingDriver(RepositiryClasses _rC)
        {
            _allTradChannel = (AllTradesChannel)_rC.GetReference("AllTradesChannel");
            _allTradChannel.LoadedLineEvent += _allTradChannel_LoadedLineEvent;

            marketTradRepo = _rC.MarketTradRepo;
            settClass = _rC.SettClass;

            id = 0;
        }
        #endregion

        #region -Event-
        // обработка добавления строки в AllTradesChannel
        void _allTradChannel_LoadedLineEvent(object sender, EventArgs e)
        {
            id++;

            AllTradesChannel _send = (AllTradesChannel)sender;
            ParametrMarketTrades _pmt = new ParametrMarketTrades(id.ToString(), _send.Date, _send.Time, _send.Number, _send.Price, _send.Security, _send.TimeMsk, _send.Operation, _send.Quantity);

            marketTradRepo.Add(_pmt); // ! добавляем без проверки по номеру !
        }

        #endregion

        #region -Method-

        #endregion
    }
}
