using CurrencyConverterTask.Lib;
using NUnit.Framework;

namespace CurrencyConverterTask.Tests
{
    [TestFixture]
    public class CurrencyConverterTests
    {
        private ICurrencyConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new CurrencyConverter();
        }


        [Test]
        public void UpdateConfiguration_UpdateExistingConversionRate_ShouldAddRates()
        {
            // Arrange
            var existingConversionRates = new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "EUR", 0.90)
            };

            // Act
            _converter.UpdateConfiguration(existingConversionRates);
            var exchangeRates = _converter.GetExchangeRates();

            // Assert
            Assert.That(0.90 == exchangeRates["USD"]["EUR"]);
            Assert.That(1 / 0.90 == exchangeRates["EUR"]["USD"]);
        }

        [Test]
        public void UpdateConfiguration_AddMultipleConversionRates_ShouldAddMultipleRates()
        {
            // Arrange
            var multipleConversionRates = new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "GBP", 0.75),
                Tuple.Create("EUR", "GBP", 0.88),
                Tuple.Create("GBP", "JPY", 150.0)
            };

            // Act
            _converter.UpdateConfiguration(multipleConversionRates);
            var exchangeRates = _converter.GetExchangeRates();

            // Assert
            Assert.That(0.75 == exchangeRates["USD"]["GBP"]);
            Assert.That(0.88 == exchangeRates["EUR"]["GBP"]);
            Assert.That(150 == exchangeRates["GBP"]["JPY"]);
        }


        [Test]
        public void UpdateConfiguration_UpdateExistingConversionRate_ShouldUpdateRates()
        {
            // Arrange
            var existingConversionRates = new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "EUR", 0.90)
            };

            // Act
            _converter.UpdateConfiguration(existingConversionRates);
            existingConversionRates = new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "EUR", 0.86)
            };
            _converter.UpdateConfiguration(existingConversionRates);

            var exchangeRates = _converter.GetExchangeRates();

            // Assert
            Assert.That(0.86 == exchangeRates["USD"]["EUR"]);
            Assert.That(1 / 0.86 == exchangeRates["EUR"]["USD"]);
        }      
    }
}

