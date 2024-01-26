/******************************************************
 * Project name: VUT FIT IVS Project 2 - Calculator
 * File: standardDeviation.cpp
 * Date: 14.4.2023
 * Authors: Marek Joukl (xjoukl00)
 *          Marko Olesak (xolesa00)
 *          Ondrej Kozanyi (xkozan01)
 *          Jan Findra (xfindr01)
 ******************************************************/
/**
 * @file standardDeviation.cpp
 *
 * @brief File that calculates standart deviation
 */
#include <vector>
#include <stdexcept>
#include <cstdio>
#include "mathLib.h"

/**
 * @brief Calculate the mean of the numbers in a vector
 * @param values A vector of double values
 * @return The mean value of the input vector
 */
double Mean(const std::vector<double> &values)
{
    double sum = 0.0;
    for (double value : values)
    {
        sum = Addition(sum, value);
    }

    double mean = Division(sum, values.size());
    return mean;
}

/**
 * @brief Calculates the sum of squares from the provided mean and values.
 * @param values A vector of double values
 * @param mean The mean value of the input vector
 * @return The sum of squares of the vector
 */
double SumOfSquares(const std::vector<double> &values, double mean)
{
    double sum = 0.0;
    for (double value : values)
    {
        double diff = Subtraction(value, mean);
        double square = Power(diff, 2.0);
        sum = Addition(sum, square);
    }
    return sum;
}

/**
 * @brief Calculates the variance of the provided vector.
 * @param values A vector of double values
 * @return The variance of the input vector
 */
double Variance(const std::vector<double> &values)
{
    if (values.empty())
        throw std::invalid_argument("Input vector is empty");

    double meanValue = Mean(values);
    double sumOfSquaresValue = SumOfSquares(values, meanValue);
    double variance = Division(sumOfSquaresValue, values.size());
    return variance;
}

/**
 * @brief Calculates the standard deviation of the provided vector.
 * @param values A vector of double values
 * @return The standard deviation of the input vector
 */
double StandardDeviation(const std::vector<double> &values)
{
    double varianceValue = Variance(values);
    double standardDeviation = Nthroot(varianceValue, 2.0);
    return standardDeviation;
}

/**
 * @brief Main function that reads numbers from the input and calculates their standard deviation.
 * @return 0
 */
int main()
{
    std::vector<double> values;
    double num;
    while (scanf("%lf", &num) == 1)
    {
        values.push_back(num);
    }

    double sd = StandardDeviation(values);
    printf("%lf\n", sd);

    return 0;
}
/************** END OF standardDeviation.cpp **************/