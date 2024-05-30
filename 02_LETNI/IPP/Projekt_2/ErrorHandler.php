<?php
namespace IPP\Student;

use IPP\Core\Interface\OutputWriter;

class ErrorHandler
{
    public static function handleError(Int $errorCode, String $message, OutputWriter $stderr): void
    {
        // Handle or log the error based on the error code
        $stderr->writeString("ERROR " . $errorCode . ": " . $message . "\n");
        exit($errorCode);
    }
}

class ErrCode
{
    const WRONG_XML_FORMAT = 31;
    const WRONG_XML_STRUCTURE = 32;
    const SEMANTIC_ERROR = 52;
    const WRONG_OPERAND = 53;
    const NON_EXISTING_VAR = 54;
    const NON_EXISTING_FRAME = 55;
    const MISSING_VALUE = 56;
    const NON_VALID_VALUE = 57;
    const NON_VALID_STRING = 58;

    const INTEGRATION_ERROR = 88;
    const INTERNAL_ERROR = 99;
}