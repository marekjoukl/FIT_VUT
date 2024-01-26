/******************************************************
 * Project name: VUT FIT IVS Project 2 - Calculator
 * File: main.cpp
 * Date: 14.4.2023
 * Authors: Marek Joukl (xjoukl00)
 *          Marko Olesak (xolesa00)
 *          Ondrej Kozanyi (xkozan01)
 *          Jan Findra (xfindr01)
******************************************************/
/**
 * @file main.cpp
 *
 * @brief File that contains body of the calculator
 */
#include "calculator.h"
#include <QApplication>

/**
 * @brief Main function that runs the calculator
 */
int main(int argc, char *argv[])
{
    QApplication a(argc, argv);
    calculator w;
    w.show();
    return a.exec();
}
/************** END OF main.cpp **************/