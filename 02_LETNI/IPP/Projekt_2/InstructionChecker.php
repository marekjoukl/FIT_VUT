<?php

namespace IPP\Student;

use IPP\Core\Interface\OutputWriter;

class InstructionChecker {
    public static function checkInstructions(mixed $root, OutputWriter $stderr): mixed {
        $instructions = [];

        if($root->nodeName  !== "program") ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Root element is not program", $stderr);
        foreach ($root->childNodes as $instruction) {
            if ($instruction->nodeType !== XML_ELEMENT_NODE) continue;
            
            if ($instruction->nodeName !== "instruction") {
                ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Expected instruction element", $stderr);
            }

            if ((intval($instruction->getAttribute("order")) < 1)) {
                ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Order number must be greater than 0", $stderr);
            }

            // Store opcode      
            $opcode = $instruction->getAttribute("opcode");

            // Check if opcode is present
            if (empty($opcode)) ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Missing opcode attribute", $stderr);

            // Check if arguments are valid
            $argOrder = 1;
            foreach ($instruction->childNodes as $childNode) {
                if ($childNode->nodeType == XML_ELEMENT_NODE ) {
                    if (!$childNode->getAttribute("type")) ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Missing type attribute", $stderr);
                    if (!str_starts_with($childNode->nodeName, "arg")) ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Wrong number of arguments", $stderr);
                    $argNumber = (int) substr($childNode->nodeName, 3);
                    if (($argNumber > self::numOfArgs($opcode, $stderr)) || ($argNumber < 0)) ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Wrong number of arguments", $stderr);
                    $argOrder++;
                }
            }
            
            // Check if the number of arguments is correct
            if($argOrder - 1 != self::numOfArgs($opcode, $stderr)) ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Wrong number of arguments", $stderr);

            // Add instruction to the array
            $instructions[] = $instruction;
        }
        return $instructions;
    }

    public static function numOfArgs(String $opcode, OutputWriter $stderr): Int{
        $noArgs = ["createframe", "pushframe", "popframe", "return", "break"];
        $var = ["defvar", "pops", "pushs", "write", "label", "jump", "dprint", "exit"];
        $varSym = ["move", "int2char", "strlen", "type", "not"];
        $varType = ["read"];
        $varVar = ["add", "sub", "mul", "idiv", "lt", "gt", "eq", "and", "or", "stri2int", "concat", "getchar", "setchar", "jumpifeq", "jumpifneq"];
        $label = ["call", "label"];
        if (in_array(strtolower($opcode), $noArgs)) return 0;
        if (in_array(strtolower($opcode), $var)) return 1;
        if (in_array(strtolower($opcode), $varSym)) return 2;
        if (in_array(strtolower($opcode), $varType)) return 2;
        if (in_array(strtolower($opcode), $varVar)) return 3;
        if (in_array(strtolower($opcode), $label)) return 1;
        ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Unknown opcode", $stderr);
        return -1;
    }

    public static function regroupInstructions(mixed $root, OutputWriter $stderr):mixed {
        $instructions = [];
    
        // Iterate over instruction nodes and store them in an array based on their order
        foreach ($root->getElementsByTagName('instruction') as $instruction) {
            $order = intval($instruction->getAttribute('order'));
            if (isset($instructions[$order])) {
                // If an instruction with the same order already exists, trigger an error
                ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Duplicate order number found: $order", $stderr);
            }
            $instructions[$order] = $instruction;
        }
    
        // Sort the instructions based on their order
        ksort($instructions);
    
        // Remove existing instruction nodes from the root node
        foreach ($root->getElementsByTagName('instruction') as $instruction) {
            $root->removeChild($instruction);
        }
    
        // Append the sorted instructions back to the root node
        foreach ($instructions as $instruction) {
            $root->appendChild($instruction);
        }
    
        return $root;
    }
}