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
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

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

            if(fromCurrency.ToLower() == toCurrency.ToLower())
                throw new ArgumentException("fromCurrency and toCurrency can not be euqal");

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
        _semaphore.Wait();
        try
        {
            if (!exchangeRates.Any())
                throw new InvalidOperationException("currency configuration does not set");

            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            var allNodes = exchangeRates.SelectMany(p => p.Value).ToList();

            if (!allNodes.Any(e => e.Key == fromCurrency) || !allNodes.Any(e => e.Key == toCurrency))
            {
                throw new ArgumentException("Invalid currency codes.");
            }

            // Perform BFS to find the shortest conversion path
            var trackingDictionary = CreateTrackingDictionary(fromCurrency, toCurrency);

            if (string.IsNullOrEmpty(trackingDictionary[toCurrency]))
                throw new ArgumentException("No conversion path found.");

            List<string> path = FindShortestPath(fromCurrency, toCurrency, trackingDictionary);

            var result = CalculateCurrencyConversion(fromCurrency, amount, path);
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void ClearConfiguration()
    {
        _semaphore.Wait();
        try
        {
            exchangeRates.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public IReadOnlyDictionary<string, Dictionary<string, double>> GetExchangeRates()
    {
        return exchangeRates;
    }

    private double CalculateCurrencyConversion(string fromCurrency, double amount, List<string> path)
    {
        double nextAmount = amount;
        string from = fromCurrency;

        foreach (var item in path)
        {
            nextAmount *= exchangeRates[from][item];
            from = item;
        }

        return nextAmount;
    }

    private List<string> FindShortestPath(string fromCurrency, string toCurrency,
        Dictionary<string, string> distTable)
    {
        var path = new List<string>();
        string node = toCurrency;

        while (node != fromCurrency && distTable.ContainsKey(node))
        {
            path.Add(node);
            node = distTable[node];
        }

        path.Reverse();
        return path;
    }

    private Dictionary<string, string> CreateTrackingDictionary(string fromCurrency, string toCurrency)
    {
        var queue = new Queue<string>();
        var visited = new HashSet<string>();
        var trackingDictionary = new Dictionary<string, string>();

        foreach (var item in exchangeRates)
        {
            trackingDictionary.Add(item.Key, "");
        }

        queue.Enqueue(fromCurrency);

        while (queue.Count > 0)
        {
            var currentCurrency = queue.Dequeue();

            visited.Add(currentCurrency);

            foreach (var nextCurrency in exchangeRates[currentCurrency].Keys)
            {
                if (!visited.Contains(nextCurrency))
                {
                    queue.Enqueue(nextCurrency);
                    trackingDictionary[nextCurrency] = currentCurrency;

                    if (nextCurrency == toCurrency)
                        return trackingDictionary;
                }
            }
        }

        return trackingDictionary;
    }
}
