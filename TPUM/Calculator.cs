using System;

namespace TPUM
{
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
        public int Subtract(int a, int b) => a - b;
        public int Multiply(int a, int b) => a * b;
        public int Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Nie można dzielić przez zero.");
            return a / b;
        }
        public int Power(int baseNum, int exponent) => (int)Math.Pow(baseNum, exponent);
    }
}