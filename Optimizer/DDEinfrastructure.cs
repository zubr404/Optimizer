using System;
using XlDde;
using System.Threading;
using System.Collections.Generic;

namespace Optimizer
{
    public sealed class DDEinfrastructure : IDisposable
    {
        #region Идентификатор DDE сервера. Идентификаторы каналов.
        XlDdeServer server;

        // Идентификатор DDE сервера. Его следует задать так же при настройке экспорта таблиц в Квике
        const string service = "DDE_Main";

        // Идентификаторы каналов. В Квике это поле "Рабочая книга", при этом
        // поле "Лист" следует оставить пустым.
        const string curtabTopic = "current_table";
        const string allTradesTopic = "alltrades";
        #endregion

        public DDEinfrastructure(RepositiryClasses _repoInf)
        {
            // server
            server = new XlDdeServer(service);

            server.AddChannel(curtabTopic, (CurrentTableChannel)_repoInf.GetReference("CurrentTableChannel"));
            server.AddChannel(allTradesTopic, (AllTradesChannel)_repoInf.GetReference("AllTradesChannel"));

            server.Register(); // Зарегистрируем сам DDE сервер
        }

        //DDE Disconnect
        public void Dispose()
        {
            server.Disconnect();
            server.Dispose();
        }
    }
}