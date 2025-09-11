using System;
using System.Collections.Generic;
using System.Text;

namespace OptionManager
{
    public class StockOption
    {
        public string Ticker { get; set; } // Ticker symbol for the stock
        public decimal StrikePrice { get; set; } // The price at which the option holder can buy or sell the stock
        public DateTime ExpiryDate { get; set; } // The date when the option expires
        public OptionType Type { get; set; } // Type of the option (Call or Put)

        public decimal OptionPrice { get; set; }

        public decimal Contract { get; set; }

    }

    public enum OptionType
    {
        Call,
        Put
    }
}
