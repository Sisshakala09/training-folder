using NUnit.Framework;
using CalculatorLibrary; 

namespace CalculatorLibraryTests
{
    public class Tests
    {
        private Calculator calculator;

        [SetUp]
        public void Setup()
        {
            calculator = new Calculator();
        }

        [Test]
        public void TestAddition()
        {
            double result = calculator.Add(2, 2);
            Assert.AreEqual(4, result);
        }

        [Test]
        public void TestSubtraction()
        {
            double result = calculator.Subtract(5, 3);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void TestMultiplication()
        {
            double result = calculator.Multiply(5, 5);
            Assert.AreEqual(25, result);
        }

        [Test]
        public void TestDivision()
        {
            double result = calculator.Divide(10, 5);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void TestDivisionByZero()
        {
            Assert.Throws<DivideByZeroException>(() => calculator.Divide(5, 0));
        }
    }
}
