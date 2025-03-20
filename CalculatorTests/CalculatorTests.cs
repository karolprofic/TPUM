namespace CalculatorTests
{
    [TestClass]
    public class CalculatorTests
    {
        private Calculator.Calculator calculator;

        [TestInitialize]
        public void Setup()
        {
            calculator = new Calculator.Calculator();
        }

        [TestMethod]
        public void AddShouldReturnCorrectSum()
        {
            Assert.AreEqual(5, calculator.Add(2, 3));
            Assert.AreEqual(-1, calculator.Add(2, -3));
        }

        [TestMethod]
        public void SubtractShouldReturnCorrectDifference()
        {
            Assert.AreEqual(1, calculator.Subtract(3, 2));
            Assert.AreEqual(5, calculator.Subtract(2, -3));
        }

        [TestMethod]
        public void MultiplyShouldReturnCorrectProduct()
        {
            Assert.AreEqual(6, calculator.Multiply(2, 3));
            Assert.AreEqual(-6, calculator.Multiply(2, -3));
        }

        [TestMethod]
        public void DivideShouldReturnCorrectQuotient()
        {
            Assert.AreEqual(2, calculator.Divide(6, 3));
            Assert.AreEqual(-2, calculator.Divide(6, -3));
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void DivideByZeroShouldThrowException()
        {
            calculator.Divide(5, 0);
        }

        [TestMethod]
        public void PowerShouldReturnCorrectResult()
        {
            Assert.AreEqual(8, calculator.Power(2, 3));
            Assert.AreEqual(1, calculator.Power(5, 0));
        }
    }
}
