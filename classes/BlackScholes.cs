using System;
using System.Collections.Generic;
using System.Text;

namespace OptionManager
{

    class BlackScholes
    {
        public static double CalculateVolatility(double stockPrice, double strikePrice, double optionPrice, double timeToExpiration, double interestRate, double dividend)
        {
            double tolerance = 1.0e-5;
            double volatility = 0.5;
            double maxIterations = 100;

            for (int i = 0; i < maxIterations; i++)
            {
                double price = CalculateOptionPrice(stockPrice, strikePrice, volatility, timeToExpiration, interestRate, dividend);
                double vega = CalculateVega(stockPrice, strikePrice, volatility, timeToExpiration, interestRate, dividend);

                double diff = price - optionPrice;
                if (Math.Abs(diff) < tolerance)
                    break;

                volatility -= diff / vega;
            }

            return volatility;
        }

        public static double CalculateOptionPrice(double stockPrice, double strikePrice, double volatility, double timeToExpiration, double interestRate, double dividend)
        {
            double sqrtTime = Math.Sqrt(timeToExpiration);
            double d1 = (Math.Log(stockPrice / strikePrice) + (interestRate - dividend + 0.5 * Math.Pow(volatility, 2)) * timeToExpiration) / (volatility * sqrtTime);
            double d2 = d1 - volatility * sqrtTime;

            double optionPrice = stockPrice * CumulativeNormalDistribution(d1) - strikePrice * Math.Exp(-interestRate * timeToExpiration) * CumulativeNormalDistribution(d2);

            return optionPrice;
        }

        public static double CalculateVega(double stockPrice, double strikePrice, double volatility, double timeToExpiration, double interestRate, double dividend)
        {
            double sqrtTime = Math.Sqrt(timeToExpiration);
            double d1 = (Math.Log(stockPrice / strikePrice) + (interestRate - dividend + 0.5 * Math.Pow(volatility, 2)) * timeToExpiration) / (volatility * sqrtTime);

            double vega = stockPrice * Math.Sqrt(timeToExpiration) * Math.Exp(-dividend * timeToExpiration) * NormalDensityFunction(d1);

            return vega;
        }

        private static double CumulativeNormalDistribution(double x)
        {
            double z = Math.Abs(x);
            double t = 1.0 / (1.0 + 0.2316419 * z);
            double y = ((((1.330274429 * t - 1.821255978) * t + 1.781477937) * t - 0.356563782) * t + 0.319381530) * t;
            double result = 1.0 - 1.0 / (Math.Sqrt(2.0 * Math.PI)) * Math.Exp(-z * z / 2.0) * y;

            if (x < 0)
                result = 1.0 - result;

            return result;
        }

        private static double NormalDensityFunction(double x)
        {
            return Math.Exp(-x * x / 2.0) / Math.Sqrt(2.0 * Math.PI);
        }
    }



}
