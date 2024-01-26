/******************************************************
 * Project name: VUT FIT IVS Project 2 - Calculator
 * File: calculator.h
 * Date: 14.4.2023
 * Authors: Marek Joukl (xjoukl00)
 *          Marko Olesak (xolesa00)
 *          Ondrej Kozanyi (xkozan01)
 *          Jan Findra (xfindr01)
******************************************************/
/**
 * @file calculator.h
 * @brief File that contains function and class declaration for calculator.cpp
 */
#ifndef CALCULATOR_H
#define CALCULATOR_H

#include <QMainWindow>
#include <QMessageBox>
#include <QMenu>
#include <QAction>
#include <QMenuBar>

QT_BEGIN_NAMESPACE
namespace Ui { class calculator; }
QT_END_NAMESPACE

class calculator : public QMainWindow
{
    Q_OBJECT

public:
    calculator(QWidget *parent = nullptr);
    ~calculator();

private:
    Ui::calculator *ui;

private slots:
    void digit_pressed();
    void operation_pressed();
    void on_ButtonDec_released();
    void on_ButtonNeg_released();
    void on_ButtonCE_released();
    void on_ButtonC_released();
    void on_ButtonEq_released();
    void on_ButtonFact_released();
    void on_ButtonPer_released();
    void on_actionHelp_list_triggered();
};
#endif // CALCULATOR_H
/************** END OF calculator.h **************/
