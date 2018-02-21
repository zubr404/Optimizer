using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Bases
{
    /// <summary>
    /// Формат данных для графика
    /// </summary>
    public interface IDataCharts
    {
        string ID { get; }
        double Value { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    /// <summary>
    /// Действия производимые с графиком
    /// </summary>
    public enum ActionCharts
    {
        ShiftForvard,   // двигаем вперед
        ShiftBack,      // двигаем назад
        Compress,       // сжимаем
        Stretch,        // растягиваем
        None
    }
}