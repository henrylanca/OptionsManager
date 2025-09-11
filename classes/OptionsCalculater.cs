using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptionManager
{
    public class OptionsCalculater
    {
        private DateTime _tradingDate;
        private double _interest;
        private double _dividendRatio;

        public OptionsCalculater(DateTime tradingDate, double interest, double dividendRatio)
        {
            _tradingDate = tradingDate;
            _interest = interest;
            _dividendRatio = dividendRatio;
        }

        public OptionPLByPrice CalculateOptionSPL(List<StockOption> stkOptions, double volatility, decimal startPrice, decimal endPrice)
        {
            OptionPLByPrice optionPLs = new OptionPLByPrice();

            DateTime expiryDate = stkOptions.Min(o => o.ExpiryDate);

            double timeToExpiry = ((expiryDate - _tradingDate).Days) / 365.0;

            if (timeToExpiry > 0)
            {                
                double priceIncrease = (double)(endPrice - startPrice) / 50.0;

                List<decimal> lstStkPrice = new List<decimal>();
                while (startPrice < endPrice)
                {
                    lstStkPrice.Add(startPrice);
                    startPrice += (decimal)priceIncrease;
                }               

                foreach(StockOption option in stkOptions)
                {
                    if(option.StrikePrice>=startPrice && option.StrikePrice<=endPrice)
                    {
                        if (!lstStkPrice.Contains(option.StrikePrice))
                        {
                            lstStkPrice.Add(option.StrikePrice);
                        }
                    }
                }

                lstStkPrice.Sort();

                optionPLs.PLs = new List<PL>();
                foreach (decimal stkPrice in lstStkPrice)
                {
                    decimal profit = 0;
                   
                    foreach(StockOption option in stkOptions)
                    {
                        timeToExpiry = ((option.ExpiryDate - _tradingDate).Days) / 365.0;

                        double optPrice = BlackScholes.CalculateOptionPrice((double)stkPrice, (double)option.StrikePrice, volatility, timeToExpiry, _interest, _dividendRatio);
                        profit += (decimal)(((decimal)optPrice - option.OptionPrice) * option.Contract) ;

                       
                    }
                    optionPLs.PLs.Add(new PL { StockPrice = stkPrice, Profit = profit });

                }
            }       

            return optionPLs;
        }
    }

    public struct OptionPLByPrice
    {
        public string Title { get; set; }

        public List<PL> PLs { get; set; }

    }

    public struct PL
    {
        public decimal StockPrice { get; set; }
        public decimal Profit { get; set; }
    }
}
