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

    public void ClearConfiguration()
    {
        throw new NotImplementedException();
    }

    public double Convert(string fromCurrency, string toCurrency, double amount)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, Dictionary<string, double>> GetExchangeRates()
    {
        throw new NotImplementedException();
    }

    public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
    {
        throw new NotImplementedException();
    }
}
