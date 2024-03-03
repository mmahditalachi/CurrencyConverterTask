namespace CurrencyConverterTask.Lib
{
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Clears any prior configuration.
        /// </summary>
        void ClearConfiguration();

        /// <summary>
        /// Updates the configuration. Rates are inserted or replaced internally.
        /// </summary>
        void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates);

        /// <summary>
        /// Converts the specified amount from one currency to another.
        /// </summary>
        /// <param name="fromCurrency">The source currency code (e.g., "USD").</param>
        /// <param name="toCurrency">The target currency code (e.g., "EUR").</param>
        /// <param name="amount">The amount to convert.</param>
        /// <returns>The converted amount in the target currency.</returns>
        double Convert(string fromCurrency, string toCurrency, double amount);

        // For testing purposes
        IReadOnlyDictionary<string, Dictionary<string, double>> GetExchangeRates();
    }
}
