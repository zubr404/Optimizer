using Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritms.FinanceResultAllMarket
{
    /// <summary>
    /// Хранилище позиций
    /// </summary>
    class PositionRepository : List<ParametrPosition>
    {
        const string out_data_log = "input_data_log.txt";
        const string data_price_log = "data_price.txt";
        const string data_position_log = "data_position.txt";

        decimal accumulatedIncome = 0; // накопленный доход

        public PositionRepository()
        {
            StaticService.DeleteFile(out_data_log);
            StaticService.DeleteFile(data_price_log);
            StaticService.DeleteFile(data_position_log);

            StaticService.LogFileWriteNotDateTime("Time\tTimeMSK\tOperation\tQantity\tPrice\t*\tAccum. icome\tVar. Margin\tOpen pos.\t*\tvarMarLots\taccumIncomeLots", out_data_log, true);
        }

        public void TradesFilled(ParametrMarketTrades _paramMarketTrades)
        {
            decimal variation_margin = 0; // вариационная маржа после каждой сделки
            decimal varMarLots = 0; // вариационка на один лот при каждой сделке
            decimal accumIncomeLots = 0; // накопленный доход на лот при каждой сделке
            decimal countOpenPosition = 0; // количество открытых позиций после каждой сделки

            decimal qty_trad = _paramMarketTrades.Quantity;

            if (_paramMarketTrades.Operation == Constants.OperationSell)
            {
                qty_trad *= -1;
            }

            if (this.Count == 0)
            {
                // добавляем новую позу
                this.Add(new ParametrPosition(_paramMarketTrades.PriceTrades, qty_trad));
            }
            else
            {
                // провeрка корректности элементов
                int sign_position = Math.Sign(this[0].Quantity);
                bool correct_element = this.All(x => Math.Sign(x.Quantity) == sign_position);

                if (!correct_element)
                {
                    System.Windows.MessageBox.Show("Есть лонги и шорты!");
                    return;
                }

                // сортировка
                this.Sort((a, b) => a.Price.CompareTo(b.Price));

                // если позиции длинные
                if (sign_position > 0)
                {
                    // если оперция покупка - добаление позиции
                    if (_paramMarketTrades.Operation == Constants.OperationBuy)
                    {
                        AddPosition(_paramMarketTrades.PriceTrades, qty_trad);
                    }
                    else // в протвном случае - закрытие позиции
                    {
                        decimal balance = qty_trad;
                        for (int i = 0; i < Math.Abs(qty_trad); i++)
                        {
                            if (balance != 0)
                            {
                                if (this.Count > 0) // если еще есть, что закрывать
                                {
                                    decimal income = 0;
                                    balance = this[0].CountCloseLong(balance, _paramMarketTrades.PriceTrades, out income);
                                    accumulatedIncome += income;

                                    this.RemoveAll(x => x.Quantity == 0); // удаляем позы с нулевыми остатками
                                }
                                else // в протвном случае добляем позу
                                {
                                    this.Add(new ParametrPosition(_paramMarketTrades.PriceTrades, balance));
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                // если позиции короткие
                if (sign_position < 0)
                {
                    // если оперция продажа - добаление позиции
                    if (_paramMarketTrades.Operation == Constants.OperationSell)
                    {
                        AddPosition(_paramMarketTrades.PriceTrades, qty_trad);
                    }
                    else // в протвном случае - закрытие позиции
                    {
                        decimal balance = qty_trad;
                        for (int i = 0; i < Math.Abs(qty_trad); i++)
                        {
                            if (balance != 0)
                            {
                                if (this.Count > 0) // если еще есть, что закрывать
                                {
                                    decimal income = 0;
                                    balance = this[this.Count - 1].CountCloseShort(balance, _paramMarketTrades.PriceTrades, out income);
                                    accumulatedIncome += income;

                                    this.RemoveAll(x => x.Quantity == 0); // удаляем позы с нулевыми остатками
                                }
                                else // в протвном случае добляем позу
                                {
                                    this.Add(new ParametrPosition(_paramMarketTrades.PriceTrades, balance));
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            // установить вариационную маржу + общая вариационка + общее кол. откр. поз
            foreach (ParametrPosition item in this)
            {
                variation_margin += item.VariationMarginChange(_paramMarketTrades.PriceTrades);
                countOpenPosition += item.Quantity;
            }

            if (countOpenPosition != 0)
            {
                varMarLots = variation_margin / Math.Abs(countOpenPosition);
                accumIncomeLots = accumulatedIncome / Math.Abs(countOpenPosition);
            }

            StaticService.LogFileWriteNotDateTime(_paramMarketTrades.DateTimeTrades + "\t" + _paramMarketTrades.TimeMsk + "\t" + _paramMarketTrades.Operation + "\t" + _paramMarketTrades.Quantity + "\t" + _paramMarketTrades.PriceTrades + "\t*\t" + accumulatedIncome + "\t" + variation_margin + "\t" + countOpenPosition + "\t*\t" + Math.Abs(varMarLots).ToString("#") + "\t" + accumIncomeLots.ToString("#"), out_data_log, true);

            StaticService.LogFileWriteNotDateTime(_paramMarketTrades.PriceTrades.ToString(), data_price_log, true);
            StaticService.LogFileWriteNotDateTime(countOpenPosition.ToString(), data_position_log, true);
        }


        private void AddPosition(decimal _price, decimal _count)
        {
            ParametrPosition pP = this.Find(x => x.Price == _price);

            if (pP == null)
            {
                // новая позиция
                this.Add(new ParametrPosition(_price, _count));
            }
            else
            {
                // увеличение позиции
                pP.CountOpen(_count);
            }
        }
    }
}
