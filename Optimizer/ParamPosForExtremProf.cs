using Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritms
{
    public class ParamTradForExtremProf
    {
        private DateTime datetimeTrades;
        private int numberTrades;
        private decimal priceTrades;
        private Operation operation;
        private double quantity;
        private string seccodeTrades;

        public ParamTradForExtremProf(DateTime _dt, int _num, decimal _price, Operation _operation, double _qty, string _seccode)
        {
            datetimeTrades = _dt;
            numberTrades = _num;
            priceTrades = _price;
            operation = _operation;
            quantity = _qty;
            seccodeTrades = _seccode;
        }

        public DateTime DateTimeTrades
        {
            get { return datetimeTrades; }
        }
        public double NumberTrades
        {
            get { return numberTrades; }
        }
        public decimal PriceTrades
        {
            get { return priceTrades; }
        }
        public Operation Operation
        {
            get { return operation; }
        }
        public double Quantity
        {
            get { return quantity; }
        }
        public string SeccodeTrades
        {
            get { return seccodeTrades; }
        }

        public override string ToString()
        {
            return datetimeTrades + "\t" + numberTrades + "\t" + seccodeTrades + "\t" + operation + "\t" + priceTrades + "\t" + quantity;
        }
    }
}
