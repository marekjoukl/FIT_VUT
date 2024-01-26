/******************************************************
 * Project name: VUT FIT IVS Project 2 - Calculator
 * File: mathLib.h
 * Date: 14.4.2023
 * Authors: Marek Joukl (xjoukl00)
 *          Marko Olesak (xolesa00)
 *          Ondrej Kozanyi (xkozan01)
 *          Jan Findra (xfindr01)
******************************************************/
/**
 * @file mathLib.h
 * 
 * @brief Head file for math library containing declarations of functions from mathLib.cpp
 */
#include "mathLib.cpp"

/**
 * @brief Negation
 * @param oldValue value to be negated
 * @return negated value
 */
double Negation(double oldValue);
/**
 * @brief Addition
 * @param addend1 Left operand
 * @param addend2 Right operand
 * @return Addition of two numbers
 */
double Addition(double addend1, double addend2);
/**
 * @brief Subtraction
 * @param minuend Left operand
 * @param subtrahend Right operand
 * @return Substraction of two numbers
 */
double Subtraction(double minuend, double subtrahend);
/**
 * @brief Multiplication
 * @param multiplicand Left operand
 * @param multiplier Right operand
 * @return Multiplication of two numbers
 */
double Multiplication(double multiplicand, double multiplier);
/**
 * @brief Division
 * @param dividend Left operand
 * @param divisor Right operand
 * @return Division of two numbers
 */
double Division(double dividend, double divisor);
/**
 * @brief Power
 * @param base number
 * @param exponent power
 * @return Powered number
 */
double Power(double base, double exponent);
/**
 * @brief Factorial
 * @param factorial
 * @return Factiorial of number
 */
double Factorial(double factorial);
/**
 * @brief Nthroot
 * @param radicand
 * @param degree
 * @return Nth root of number
 */
double Nthroot(double radicand, double degree);
/**
 * @brief Percentile
 * @param number
 * @return Percentile of number
 */
double Percentile(double number);
/************** END OF mathLib.h **************/
