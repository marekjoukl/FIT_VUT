/** 
 * Implementace překladače imperativního jazyka IFJ23
 * xzelen29  Jakub Zelenay
*/
#ifndef AUTOMAT_H
#define AUTOMAT_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <stdbool.h>


typedef enum AutomatState
{
    Error,
    Start,
    Ls,
    LsEqual,
    Gt,
    GtEqual,
    ExlPoint,
    NotEqual,
    Assign,
    Equal,
    Comma,
    Colon,
    RPar,
    LPar,
    LBrac,
    RBrac,
    SingleQmark,
    DoubleQmark,
    Id,
    IdNil,
    BeginString,
    StringLit,
    EscSeq,
    EscU,
    EscLBr,
    FirstHex,
    SecondHex,
    EndStringLit,
    EmptyString,
    MltLnStringStart,
    MltLnStringLit,
    MltLnStringStartEnd,
    FirstQuote,
    SecondQuote,
    FirstQuoteErr,
    SecondQuoteErr,
    ThirdQuoteErr,
    EndMltLnStringLit,
    IntLit,
    InvalidInt,
    DecPoint,
    DoubleLitDec,
    DoubleLitExp,
    Exp,
    ExpSign,
    Plus,
    Minus,
    Arrow,
    Asterisk,
    Slash,
    Comment,
    CommentBody,
    NestedComment,
    CommentEnding,
    BlockComment,
    Space,
    NewLine,
    EndOfFile,
} AutomatState;

AutomatState transition(AutomatState current, char edge);

#endif // AUTOMAT_H
