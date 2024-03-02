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
        var distTable = CreateDistanceTable(fromCurrency);

        if (distTable[toCurrency].dis == -1)
            throw new ArgumentException("No conversion path found.");

        List<string> path = FindShortestPath(fromCurrency, toCurrency, distTable);

        return CalculateCurrencyConversion(fromCurrency, amount, path);
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

    private static List<string> FindShortestPath(string fromCurrency, string toCurrency, Dictionary<string, (string prev, int dis)> distTable)
    {
        var path = new List<string>();
        string node = toCurrency;

        while (node != fromCurrency && distTable.ContainsKey(node))
        {
            path.Add(node);
            node = distTable[node].prev;
        }

        path.Reverse();
        return path;
    }

    private Dictionary<string, (string prev, int dis)> CreateDistanceTable(string fromCurrency)
    {
        var queue = new Queue<string>();
        var visited = new HashSet<string>();
        var distTable = new Dictionary<string, (string prev, int dis)>();

        foreach (var item in exchangeRates)
        {
            distTable.Add(item.Key, ("", -1));
        }

        distTable[fromCurrency] = ("", 0);

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
                    distTable[nextCurrency] = (currentCurrency, distTable[currentCurrency].dis + 1);
                }
            }
        }

        return distTable;
    }

    public void ClearConfiguration()
    {
        exchangeRates.Clear();
    }

    public IReadOnlyDictionary<string, Dictionary<string, double>> GetExchangeRates()
    {
        return exchangeRates;
    }
}
