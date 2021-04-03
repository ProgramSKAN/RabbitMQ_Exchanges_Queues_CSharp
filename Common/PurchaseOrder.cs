using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class PurchaseOrder
    {
        public decimal AmountToPay;
        public string PurchaseOrderNumber;
        public string CompanyName;
        public int PaymentDayTerms;

        public override string ToString()
        {
            return $"|{CompanyName},{PurchaseOrderNumber},{AmountToPay}|";
        }
    }
}
