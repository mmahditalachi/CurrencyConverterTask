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

        [Test]
        public void ClearConfiguration_ShouldRemoveAllRates()
        {
            // Arrange
            _converter.UpdateConfiguration(new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "CAD", 1.34),
                Tuple.Create("CAD", "GBP", 0.58),
                Tuple.Create("USD", "EUR", 0.86)
            });

            // Act
            _converter.ClearConfiguration();

            // Assert
            Assert.IsEmpty(_converter.GetExchangeRates());
        }



        [Test]
        public void Convert_SameCurrency_ShouldReturnSameAmount()
        {
            // Arrange
            _converter.UpdateConfiguration(new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "CAD", 1.34),
                Tuple.Create("CAD", "GBP", 0.58),
                Tuple.Create("USD", "EUR", 0.86)
            });

            // Act
            var result = _converter.Convert("USD", "USD", 100);

            // Assert
            Assert.That(100 == result);
        }

        [Test]
        public void Convert_DirectPath_ShouldReturnConvertedAmount()
        {
            // Arrange
            _converter.UpdateConfiguration(new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "CAD", 1.34),
                Tuple.Create("CAD", "GBP", 0.58),
                Tuple.Create("USD", "EUR", 0.86)
            });

            // Act
            var result = _converter.Convert("USD", "EUR", 100);

            // Assert
            Assert.That(86 == Math.Round(result, 2));
        }

        [Test]
        public void Convert_IndirectPath_ShouldReturnConvertedAmount()
        {
            // Arrange
            _converter.UpdateConfiguration(new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "CAD", 1.34),
                Tuple.Create("CAD", "GBP", 0.58),
                Tuple.Create("USD", "EUR", 0.86)
            });

            // Act
            var result = _converter.Convert("CAD", "EUR", 100);

            // Assert
            Assert.That(64.18 == Math.Round(result, 2));
        }

        [Test]
        public void Convert_InvalidCurrency_ShouldThrowException()
        {
            // Arrange
            _converter.UpdateConfiguration(new List<Tuple<string, string, double>>
            {
                Tuple.Create("USD", "CAD", 1.34),
                Tuple.Create("CAD", "GBP", 0.58),
                Tuple.Create("USD", "EUR", 0.86)
            });

            // Act and Assert
            Assert.Throws<ArgumentException>(() => _converter.Convert("USD", "JPY", 100));
        }
    }
}

