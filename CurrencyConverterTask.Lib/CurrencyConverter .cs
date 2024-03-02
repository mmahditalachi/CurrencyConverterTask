using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CurrencyConverterTask.Lib;

public class CurrencyConverter : ICurrencyConverter
{
    private readonly Dictionary<string, Dictionary<string, double>> exchangeRates;

    public CurrencyConverter()
    {
        exchangeRates = new Dictionary<string, Dictionary<string, double>>();
    }

    public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        foreach (var rate in conversionRates)
        {
            var fromCurrency = rate.Item1;
            var toCurrency = rate.Item2;
            var rateValue = rate.Item3;

            if (!exchangeRates.ContainsKey(fromCurrency))
            {
                exchangeRates[fromCurrency] = new Dictionary<string, double>();
            }
            exchangeRates[fromCurrency][toCurrency] = rateValue;

            if (!exchangeRates.ContainsKey(toCurrency))
            {
                exchangeRates[toCurrency] = new Dictionary<string, double>();
            }
            exchangeRates[toCurrency][fromCurrency] = 1 / rateValue;
        }
    }

    public double Convert(string fromCurrency, string toCurrency, double amount)
    {
        throw new NotImplementedException();
    }

    public void ClearConfiguration()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, Dictionary<string, double>> GetExchangeRates()
    {
        return exchangeRates;
    }
}
