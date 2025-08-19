using NUnit.Framework;
using MathLibrary;  // 🔸 This tells the file to use Calculator class from MathLibrary

namespace MathLibraryTests
{
    public class CalculatorTests
    {
        [Test]
        public void Add_TwoNumbers_ReturnsCorrectAnswer()
        {
            int result = Calculator.Add(2, 3);  // Calling the method from Calculator class
            Assert.AreEqual(5, result);         // Verifying the result is 5
        }
    }
}
