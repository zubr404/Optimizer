using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Optimizer
{
    /// <summary>
    /// Определяет события и свойства для класоов dde-экспорта
    /// </summary>
    interface IExportDDE
    {
        event System.EventHandler LoadedLineEvent;
        event System.EventHandler ObtainingDataCompletedEvent;
        event System.EventHandler ObtainingDataStartedEvent;

        int CountRowsExport { get; }
        void SetCountRowsExport(int value);
    }
}