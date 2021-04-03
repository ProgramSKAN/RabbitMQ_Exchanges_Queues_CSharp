using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class Payment
    {
        public decimal AmountToPay;
        public string CardNumber;
        public string Name;
        public override string ToString()
        {
            return $"|{Name},{CardNumber},{AmountToPay}|";
        }
    }
}
