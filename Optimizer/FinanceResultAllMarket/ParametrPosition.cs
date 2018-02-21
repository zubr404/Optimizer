using System;

namespace Algoritms.FinanceResultAllMarket
{
    /// <summary>
    /// Класс позиции
    /// </summary>
    class ParametrPosition
    {
        public decimal Price { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal Variation_margin { get; private set; }

        public ParametrPosition(decimal _price, decimal _count)
        {
            Price = _price;
            Quantity = _count;
            Variation_margin = 0;
        }

        /// <summary>
        /// Открытие позиции по той же цене
        /// </summary>
        /// <param name="_qtyTrades">количество в сделке</param>
        public void CountOpen(decimal _qtyTrades)
        {
            Quantity += _qtyTrades;
        }

        /// <summary>
        /// Закрытие лонгов.
        /// </summary>
        /// <param name="_qtyTrades">количество в сделке</param>
        public decimal CountCloseLong(decimal _qtyTrades, decimal _priceTrades, out decimal _income)
        {
            decimal balance = 0;
            _income = 0;

            if (Math.Sign(Quantity) != Math.Sign(_qtyTrades))
            {
                balance = Math.Abs(Quantity) - Math.Abs(_qtyTrades);

                // если количество в сделке больше, чем Quantity
                if (balance < 0)
                {
                    _income = (_priceTrades - Price) * Math.Abs(Quantity);
                    Quantity = 0;
                }
                else
                {
                    _income = (_priceTrades - Price) * Math.Abs(_qtyTrades);
                    Quantity += _qtyTrades;
                    balance = 0;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Ошибка в ParametrPosition.CountClose()");
            }

            return balance;
        }

        /// <summary>
        /// Закрытие шортов.
        /// </summary>
        /// <param name="_qtyTrades">количество в сделке</param>
        public decimal CountCloseShort(decimal _qtyTrades, decimal _priceTrades, out decimal _income)
        {
            decimal balance = 0;
            _income = 0;

            if (Math.Sign(Quantity) != Math.Sign(_qtyTrades))
            {
                balance = Math.Abs(Quantity) - Math.Abs(_qtyTrades);

                // если количество в сделке больше, чем Quantity
                if (balance < 0)
                {
                    _income = (Price - _priceTrades) * Math.Abs(Quantity);
                    Quantity = 0;
                    balance = Math.Abs(balance); // это для шортов
                }
                else
                {
                    _income = (Price - _priceTrades) * Math.Abs(_qtyTrades);
                    Quantity += _qtyTrades;
                    balance = 0;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Ошибка в ParametrPosition.CountClose()");
            }

            return balance;
        }



        /// <summary>
        /// Расчет вариационной маржи по позиции
        /// </summary>
        /// <param name="_priceTrades">цена сделки</param>
        public decimal VariationMarginChange(decimal _priceTrades)
        {
            decimal var_price = 0;

            if (Quantity > 0)
            {
                var_price = _priceTrades - Price;
            }
            if (Quantity < 0)
            {
                var_price = Price - _priceTrades;
            }

            Variation_margin = var_price * Math.Abs(Quantity);

            return Variation_margin;
        }
    }
}