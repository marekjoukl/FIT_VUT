#include "../mathLib.h"
#include "gtest/gtest.h"

TEST(Addition, Integers)
{
    EXPECT_EQ(Addition(5, 7), 12);
    EXPECT_EQ(Addition(0, 0), 0);
    EXPECT_EQ(Addition(-128, 5), -123);
    EXPECT_EQ(Addition(-128, -128), -256);
    EXPECT_EQ(Addition(-128, 0), -128);
}

TEST(Addition, Decimals)
{
    EXPECT_EQ(Addition(2.5, 1.5), 4.0);
    EXPECT_EQ(Addition(0.0, 0.0), 0.0);
    EXPECT_EQ(Addition(-0.5, 1.5), 1.0);
    EXPECT_EQ(Addition(-2.0, -3.0), -5.0);
    EXPECT_EQ(Addition(-2.5, 0.5), -2.0);
}

TEST(Subtraction, Integers)
{
    EXPECT_EQ(Subtraction(0, 0), 0);
    EXPECT_EQ(Subtraction(256, 0), 256);
    EXPECT_EQ(Subtraction(1024, 512), 512);
    EXPECT_EQ(Subtraction(512, 1024), -512);
    EXPECT_EQ(Subtraction(-128, -128), 0);
    EXPECT_EQ(Subtraction(-128, 128), -256);
}

TEST(Subtraction, Decimals)
{
    EXPECT_EQ(Subtraction(0.0, 0.0), 0.0);
    EXPECT_EQ(Subtraction(0.33, 0.33), 0.0);
    EXPECT_EQ(Subtraction(256.0, 0.5), 255.5);
    EXPECT_EQ(Subtraction(1024.0, 512.5), 511.5);
    EXPECT_EQ(Subtraction(512.5, 1024.0), -511.5);
    EXPECT_EQ(Subtraction(-128.33, -128.33), 0.0);
}

TEST(Multiplication, Zero)
{
    EXPECT_EQ(Multiplication(0, 0), 0);
    EXPECT_EQ(Multiplication(10, 0), 0);
    EXPECT_EQ(Multiplication(-10, 0), 0);
}

TEST(Multiplication, Integers)
{
    EXPECT_EQ(Multiplication(0, 1), 0);
    EXPECT_EQ(Multiplication(1, 1), 1);
    EXPECT_EQ(Multiplication(-1, -1), 1);
    EXPECT_EQ(Multiplication(-1, 1), -1);
    EXPECT_EQ(Multiplication(-256, 2), -512);
    EXPECT_EQ(Multiplication(-256, -2), 512);
}

TEST(Multiplication, Decimals)
{
    EXPECT_EQ(Multiplication(0.0, 1.0), 0.0);
    EXPECT_EQ(Multiplication(1.5, 2.0), 3.0);
    EXPECT_EQ(Multiplication(-1.2, -2.5), 3.0);
    EXPECT_EQ(Multiplication(-1.2, 2.5), -3.0);
    EXPECT_EQ(Multiplication(-3.1415, 2.0), -6.283);
    EXPECT_EQ(Multiplication(-3.1415, -2.0), 6.283);
}

TEST(Division, DividerZero)
{
    EXPECT_THROW(Division(1, 0), std::overflow_error);
    EXPECT_THROW(Division(0, 0), std::overflow_error);
}

TEST(Division, PositiveDividend)
{

    // Ordinary values
    EXPECT_EQ(Division(0, 1), 0);
    EXPECT_EQ(Division(2, 1), 2);
    EXPECT_EQ(Division(2, -1), -2);
    EXPECT_EQ(Division(10, 2), 5);
    EXPECT_EQ(Division(10, -2), -5);
    EXPECT_EQ(Division(128, 2), 64);
    EXPECT_EQ(Division(128, -2), -64);
    EXPECT_EQ(Division(256, 2), 128);
    EXPECT_EQ(Division(256, -2), -128);
}

TEST(Division, NegativeDividend)
{
    EXPECT_EQ(Division(-256, 2), -128);
    EXPECT_EQ(Division(-256, -2), 128);
    EXPECT_EQ(Division(-512, 2), -256);
    EXPECT_EQ(Division(-512, -2), 256);
    EXPECT_EQ(Division(-512, 4), -128);
    EXPECT_EQ(Division(-512, -4), 128);
}

TEST(Division, DecimalResults)
{
    EXPECT_EQ(Division(32767, 8), 4095.875);
    EXPECT_EQ(Division(32767, -8), -4095.875);
    EXPECT_EQ(Division(-32767, -8), 4095.875);
}

TEST(Power, ZeroExponent)
{
    EXPECT_EQ(Power(2, 0), 1);
    EXPECT_EQ(Power(-2, 0), 1);
    EXPECT_EQ(Power(0, 0), 1);
}

TEST(Power, IntegerPower)
{
    EXPECT_EQ(Power(2, 0), 1);
    EXPECT_EQ(Power(2, 1), 2);
    EXPECT_EQ(Power(2, 2), 4);
    EXPECT_EQ(Power(2, 3), 8);
    EXPECT_EQ(Power(3, 4), 81);
}

TEST(Power, NegativeBase)
{
    EXPECT_EQ(Power(-2, 0), 1);
    EXPECT_EQ(Power(-2, 1), -2);
    EXPECT_EQ(Power(-2, 2), 4);
    EXPECT_EQ(Power(-2, 3), -8);
}

TEST(Factorial, WholeNumbers)
{
    EXPECT_EQ(Factorial(0), 1);
    EXPECT_EQ(Factorial(1), 1);
    EXPECT_EQ(Factorial(2), 2);
    EXPECT_EQ(Factorial(3), 6);
    EXPECT_EQ(Factorial(4), 24);
    EXPECT_EQ(Factorial(5), 120);
}
TEST(Nthroot, ZeroDegree)
{
    EXPECT_THROW(Nthroot(2.0, 0.0), std::overflow_error);
    EXPECT_THROW(Nthroot(-2.0, 0.0), std::overflow_error);
}

TEST(Nthroot, RadicandLessThanZero)
{
    EXPECT_THROW(Nthroot(-8, 3), std::overflow_error);
    EXPECT_THROW(Nthroot(-27, 3), std::overflow_error);
}

TEST(Nthroot, WholeRadicand)
{
    EXPECT_EQ(Nthroot(8, 3), 2);
    EXPECT_EQ(Nthroot(27, 3), 3);
    EXPECT_EQ(Nthroot(16, 4), 2);
}

TEST(Nthroot, DecimalResult)
{
    EXPECT_NEAR(Nthroot(9, 3), 2.0800, 0.0001);
    EXPECT_NEAR(Nthroot(15, 4), 1.9679, 0.0001);
    EXPECT_NEAR(Nthroot(8, 2), 2.8284, 0.0001);
}

TEST(PercentileTest, PositiveNumbers)
{
    EXPECT_EQ(Percentile(5), 0.05);
    EXPECT_EQ(Percentile(25), 0.25);
    EXPECT_EQ(Percentile(50), 0.50);
    EXPECT_EQ(Percentile(75), 0.75);
    EXPECT_EQ(Percentile(90), 0.90);
}

TEST(PercentileTest, NegativeNumbers)
{
    EXPECT_EQ(Percentile(-5), -0.05);
    EXPECT_EQ(Percentile(-25), -0.25);
    EXPECT_EQ(Percentile(-50), -0.50);
    EXPECT_EQ(Percentile(-75), -0.75);
    EXPECT_EQ(Percentile(-90), -0.90);
}

TEST(PercentileTest, Zero)
{
    EXPECT_EQ(Percentile(0), 0.0);
}

int main(int argc, char **argv)
{
    ::testing::InitGoogleTest(&argc, argv);
    return RUN_ALL_TESTS();
}