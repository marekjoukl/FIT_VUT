<?php

namespace IPP\Student;

use Error;
use DOMElement;
use IPP\Core\Interface\InputReader;
use IPP\Core\Interface\OutputWriter;

// use function PHPSTORM_META\type;

class Program {
    /** @var array<string, mixed> */
    public $globalFrame;
    /** @var array<string, mixed> */
    public $localFrame;
    /** @var array<string, mixed> */
    public ?array $tempFrame = null;
    /** @var array<string, mixed> */
    public $labels;
    /** @var mixed $instruction */
    public mixed $instructions;
    public int $currPosition;
    /** @var array<string, mixed> */
    public $callArray;
    /** @var array<string, mixed> */
    public $stack;
    public OutputWriter $stderr;
    public InputReader $stdin;


    public function __construct(mixed $instructions, OutputWriter $stderr, InputReader $stdin){
        $this->globalFrame = [];
        $this->localFrame = [];
        $this->tempFrame = null;
        $this->labels = [];
        $this->instructions = $instructions;
        $this->currPosition = 0;
        $this->callArray = [];
        $this->stack = [];
        $this->stderr = $stderr;
        $this->stdin = $stdin;
    }
   
    public function execute(Program $program): void {

        // Extract labels from the program
        $program->extractLabels();

        $this->currPosition = 0;

        // Loop through instructions and execute them
        while ($this->currPosition < count($this->instructions)){
            if (isset($this->instructions[$this->currPosition])) {
                $instruction = $this->instructions[$this->currPosition];
            } else {
                ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Instruction not found", $this->stderr);
                return;
            }
            $opcode = $instruction->getAttribute("opcode");
            $args = [];
            foreach ($instruction->childNodes as $childNode) {
                if ($childNode->nodeType == XML_ELEMENT_NODE) {
                    $args[] = $childNode;
                }
            }
            $argsArray = [];
            foreach ($args as $index => $argElement) {
                $argsArray["arg" . ($index + 1)] = $argElement;
            }  
            InstructionExecute::executeInstruction($opcode, $argsArray, $program);
            $this->currPosition++;
        }
    }

    public function extractLabels(): void{
        
        foreach ($this->instructions as $instruction) {
            if (strtoupper($instruction->getAttribute("opcode")) === "LABEL") {
                $label = $instruction->getElementsByTagName("arg1")->item(0)->nodeValue;
                if (in_array($label, $this->labels)) {
                    ErrorHandler::handleError(ErrCode::SEMANTIC_ERROR, "Label already defined: $label", $this->stderr);
                }
                $this->labels[] = $label;
            }            
        }
    }
    /**
     * @return array{value?: mixed, type?: string}
     */
    public function extractData(mixed $symb): array{
        $type = $symb->getAttribute("type");
        
        // Symbol can be either constant or variable
        if ($type === "int" || $type === "bool" || $type === "string" || $type === "nil") {
            // If constant, return value and its type
            return array("value" => $symb->nodeValue, "type" => $type);
        } elseif ($type === "var") {
            // If variable, check if it's defined and valid and return its value and type
            if (!self::isDefined($symb->nodeValue)) {
                ErrorHandler::handleError(ErrCode::NON_EXISTING_VAR, "Variable not defined: " . $symb->nodeValue, $this->stderr);
                return [];
            } elseif (!self::isValidVar($symb->nodeValue)) {
                ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Wrong format of variable: " . $symb->nodeValue, $this->stderr);
                return [];
            }
    
            $symbParts = explode("@", $symb->nodeValue);
            $frame = $symbParts[0];
            $name = $symbParts[1];

            switch ($frame) {
                case "GF":
                    $tmp = $this->globalFrame[$name];
                    break;
                case "LF":
                    if (count($this->localFrame) > 0) {
                        $tmp = end($this->localFrame)[$name];

                    } else {
                        ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No local frame to store data", $this->stderr);
                        return [];
                    }
                    break;
                case "TF":
                    if ($this->tempFrame === null) {
                        ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No temporary frame to store data", $this->stderr);
                        return [];
                    }
                    $tmp = $this->tempFrame[$name];
                    break;
                default:
                    ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "Wrong frame format: $symb", $this->stderr);
                    return [];  
            }
            return array("value" => $tmp["value"], "type" => $tmp["type"]);
        } else {
            ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Wrong type of value: " . $type, $this->stderr);
            return [];
        } 
    }
    
    public function storeData(mixed $var, mixed $value, mixed $type): void{

        // Check var validity
        if(!self::isDefined($var)){
            ErrorHandler::handleError(ErrCode::NON_EXISTING_VAR, "Variable not defined: " . $var, $this->stderr);
        } else if(!self::isValidVar($var)){
            ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Wrong format of variable: " . $var, $this->stderr);
        }
        $cleanedString = preg_replace('/\s+/', ' ', $var);
        $trimmedString = trim($cleanedString);
        $varParts = explode("@", $trimmedString);
        if (count($varParts) !== 2) {
            ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Wrong format of variable: $var", $this->stderr);
        }
        $frame = $varParts[0];
        $name = $varParts[1];
    
        switch ($frame) {
            case "GF":
                $this->globalFrame[$name] = array("value" => $value, "type" => $type);
                break;
            case "LF":
                if (count($this->localFrame) > 0) {
                    $this->localFrame[-1][$name] = array("value" => $value, "type" => $type);

                } else {
                    ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No local frame to store data", $this->stderr);
                }
                
                break;
            case "TF":
                if ($this->tempFrame === null) {
                    ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No temporary frame to store data", $this->stderr);
                    return;
                }
                $this->tempFrame[$name] = array("value" => $value, "type" => $type);
                break;
            default:
                ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "Wrong frame format: $var", $this->stderr);
        }
    }
    

    public function isValidVar(mixed $var): bool{
        $cleanedString = preg_replace('/\s+/', ' ', $var);
        $trimmedString = trim($cleanedString);
        $varParts = explode("@", $trimmedString);
        if (count($varParts) !== 2 && $var->getAttribute("type") !== "var") {
            ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Wrong format of variable: $var", $this->stderr);
        }
        $frame = $varParts[0];

        switch ($frame) {
            case "GF":
                return true;
            case "LF":
                return true;
            case "TF":
                return true;
            default:
                ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "Wrong frame format: $var", $this->stderr);
        }
        return false;
    }

    public function isDefined(String $var): bool{
        $cleanedString = preg_replace('/\s+/', ' ', $var);
        $trimmedString = trim($cleanedString);
        $varParts = explode("@", $trimmedString);
        if (count($varParts) !== 2) {
            ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Wrong format of variable: $var", $this->stderr);
        }
        $frame = $varParts[0];
        $name = $varParts[1];

        switch ($frame) {
            case "GF":
                if (array_key_exists($name, $this->globalFrame)) {
                    return true;
                }
                break;
            case "LF":
                
                if (count($this->localFrame) > 0) {
                    foreach ($this->localFrame as $frame) {
                        if (array_key_exists($name, $frame)) {
                            return true;
                        }
                    }
                }
                break;
            case "TF":
                if ($this->tempFrame === null) {
                    ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No temporary frame to store data", $this->stderr);
                    return false;
                }
                if (array_key_exists($name, $this->tempFrame)) {
                    return true;
                }
                break;
            default:
                ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "Wrong frame format: $var", $this->stderr);
        }
        return false;
    }

    public function executeOperation(Program $program, mixed $symb1, mixed $symb2, String $operation): int{
        $type1 = $symb1->getAttribute("type");
        $type2 = $symb2->getAttribute("type");
    
        // Check if both type values are either "var" or "int"
        if (($type1 !== "var" && $type1 !== "int") || ($type2 !== "var" && $type2 !== "int")) {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Invalid operand type", $this->stderr);
        }

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        $op1_value = $op1["value"];
        $op2_value = $op2["value"];

        $op1_type = $op1["type"];
        $op2_type = $op2["type"];

        if ($op1_type == "nil" || $op2_type  == "nil") {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Executing operation on nil value", $this->stderr);
            return -1;
        }
        if ($op1_type !== 'int' || $op2_type !== 'int') {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand", $this->stderr);
            return -1;
        }
        if ($operation === "DIV" && $op2_value == 0) {
            ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Division by zero", $this->stderr);
            return -1;
        }
        if (!is_numeric($op1_value) || !is_numeric($op2_value)) {
            ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Non-numeric value", $this->stderr);
            return -1;
        }

        switch ($operation) {
            case "ADD":
                return $op1_value + $op2_value;
            case "SUB":
                return $op1_value - $op2_value;
            case "MUL":
                return $op1_value * $op2_value;
            case "DIV":
                return $op1_value / $op2_value;
            default:
                ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Unknown operation: $operation", $this->stderr);
                return -1;
        }
    }

    public function findLabelIndex(String $label): int{
        $index = 0;
        foreach ($this->instructions as $instruction) {
            if (strtoupper($instruction->getAttribute("opcode")) === "LABEL") {
                $labelName = $instruction->getElementsByTagName("arg1")->item(0)->nodeValue;
                if ($labelName === $label) {
                    return $index;
                }
            }
            $index++;
        }
        ErrorHandler::handleError(ErrCode::SEMANTIC_ERROR, "Label not found: $label", $this->stderr);
        return -1;
    }
    public static function stringCheck(mixed $string): mixed {
        if ($string !== null) {
            $index = 0;
            while (($pos = strpos($string, '\\', $index)) !== false) {
                if (isset($string[$pos+1]) && is_numeric($string[$pos+1]) &&
                    isset($string[$pos+2]) && is_numeric($string[$pos+2]) &&
                    isset($string[$pos+3]) && is_numeric($string[$pos+3])) {

                    $ascii_code = intval(substr($string, $pos+1, 3));
                    $string = substr_replace($string, chr($ascii_code), $pos, 4);
                } else {
                    $index = $pos + 1;
                }
            }
        }
        return $string;
    }
}

abstract class Instruction {
    /**
     * @var array<string, mixed>
     */ 
    public static Array $args = [];
    /**
     * @param array<string, DOMElement> $args
     */
    public static function setArgs(Array $args): void{
        self::$args = $args;
    }
    /** 
     * @param array<string, DOMElement> $args
     */
    public function __construct(Array $args) {
        $this->args = $args;
    }
    public static function getArg(Int $index, OutputWriter $stderr):mixed {
        foreach (self::$args as $arg) {
            if ($arg->nodeName == "arg$index") {
                return $arg;
            }
        }
        ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Wrong format of arguments", $stderr);
        return -1;
    }
        
    public static function execute(Program $program): void{}
}

class InstructionExecute {
    /**
     * @param string $opcode
     * @param array<string, DOMElement> $args
     * @param Program $program
     * @return void
     */
    public static function executeInstruction(String $opcode, Array $args, Program $program): void {
        switch (strtoupper($opcode)) {
            case 'MOVE':
                Instruction::setArgs($args);
                Move::execute($program);
                break;
            case 'CREATEFRAME':
                Instruction::setArgs($args);
                CreateFrame::execute($program);
                break;
            case 'PUSHFRAME':
                Instruction::setArgs($args);
                PushFrame::execute($program);
                break;
            case 'POPFRAME':
                Instruction::setArgs($args);
                PopFrame::execute($program);
                break;
            case 'DEFVAR':
                Instruction::setArgs($args);
                DefVar::execute($program);
                break;
            case 'CALL':
                Instruction::setArgs($args);
                Call::execute($program);
                break;
            case 'RETURN':
                Instruction::setArgs($args);
                ReturnFromCall::execute($program);
                break;
            case 'PUSHS':
                Instruction::setArgs($args);
                PushS::execute($program);
                break;
            case 'POPS':
                Instruction::setArgs($args);
                PopS::execute($program);
                break;
            case 'ADD':
                Instruction::setArgs($args);
                Add::execute($program);
                break;
            case 'SUB':
                Instruction::setArgs($args);
                Sub::execute($program);
                break;
            case 'MUL':
                Instruction::setArgs($args);
                Mul::execute($program);
                break;
            case 'IDIV':
                Instruction::setArgs($args);
                IDiv::execute($program);
                break;
            case 'LT':
                Instruction::setArgs($args);
                Lt::execute($program);
                break;
            case 'GT':
                Instruction::setArgs($args);
                Gt::execute($program);
                break;
            case 'EQ':
                Instruction::setArgs($args);
                Eq::execute($program);
                break;
            case 'AND':
                Instruction::setArgs($args);
                AndInstr::execute($program);
                break;
            case 'OR':
                Instruction::setArgs($args);
                OrInstr::execute($program);
                break;
            case 'NOT':
                Instruction::setArgs($args);
                NotInstr::execute($program);
                break;
            case 'INT2CHAR':
                Instruction::setArgs($args);
                Int2Char::execute($program);
                break;
            case 'STRI2INT':
                Instruction::setArgs($args);
                Stri2Int::execute($program);
                break;
            case 'READ':
                Instruction::setArgs($args);
                Read::execute($program);
                break;
            case 'WRITE':
                Instruction::setArgs($args);
                Write::execute($program);
                break;
            case 'CONCAT':
                Instruction::setArgs($args);
                Concat::execute($program);
                break;
            case 'STRLEN':
                Instruction::setArgs($args);
                StrLen::execute($program);
                break;
            case 'GETCHAR':
                Instruction::setArgs($args);
                GetChar::execute($program);
                break;
            case 'SETCHAR':
                Instruction::setArgs($args);
                SetChar::execute($program);
                break;
            case 'TYPE':
                Instruction::setArgs($args);
                Type::execute($program);
                break;
            case 'LABEL':
                Instruction::setArgs($args);
                Label::execute($program);
                break;
            case 'JUMP':
                Instruction::setArgs($args);
                Jump::execute($program);
                break;
            case 'JUMPIFEQ':
                Instruction::setArgs($args);
                JumpIfEq::execute($program);
                break;
            case 'JUMPIFNEQ':
                Instruction::setArgs($args);
                JumpIfNeq::execute($program);
                break;
            case 'EXIT':
                Instruction::setArgs($args);
                ExitProg::execute($program);
                break;
            case 'DPRINT':
                Instruction::setArgs($args);
                DPrint::execute($program);
                break;
            case 'BREAK':
                Instruction::setArgs($args);
                Breakpoint::execute($program);
                break;
            default:
                ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Unknown opcode: $opcode", $program->stderr);
        }
    }
}

class Move extends Instruction {
    public mixed $var;
    public mixed $symb;
    /**
     * @var array{value: mixed, type: string} $data
     */
    public Array $data;
    public static function execute(Program $program): void{
        $var = parent::getArg(1, $program->stderr);
        $symb = parent::getArg(2, $program->stderr);

        $data = $program->extractData($symb);

        $data_value = $data["value"];

        $program->storeData($var->nodeValue, $data_value, $data["type"]);
    }
}

class CreateFrame extends Instruction {
    public static function execute(Program $program): void {
        $program->tempFrame = [];
    }
}

class PushFrame extends Instruction {
    public static function execute(Program $program): void {
        if ($program->tempFrame === null) {
            ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No temporary frame to push", $program->stderr);
        } else {
            
            foreach ($program->tempFrame as $key => $value) {
                $program->localFrame[-1][$key] = $value;
            }

            array_push($program->localFrame, $program->tempFrame);
            $program->tempFrame = null;
        }
    }
}

class PopFrame extends Instruction {
    public static function execute(Program $program): void {
        if (count($program->localFrame) > 0) {
            $program->tempFrame = array_pop($program->localFrame);
        } else {
            ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No local frame to pop", $program->stderr);
        }
    }
}

class DefVar extends Instruction {
    public mixed $var;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $cleanedString = preg_replace('/\s+/', ' ', $var->nodeValue);
        $trimmedString = trim($cleanedString);
        $varParts = explode("@", $trimmedString);
        if (!$program->isValidVar($var->nodeValue) || $program->isDefined($var->nodeValue)) {
            ErrorHandler::handleError(ErrCode::SEMANTIC_ERROR, "Wrong format of variable: $var->nodeValue", $program->stderr);
        }
        $frame = $varParts[0];
        $name = $varParts[1];
        switch ($frame) {
            case "GF":
                $program->globalFrame[$name] = null;
                break;
            case "LF":
                if (count($program->localFrame) > 0) {
                    $program->localFrame[-1][$name] = null;
                } else {
                    ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "No local frame to store data", $program->stderr);
                }
                break;
            case "TF":
                $program->tempFrame[$name] = null;
                break;
            default:
                ErrorHandler::handleError(ErrCode::NON_EXISTING_FRAME, "Wrong frame format: $var->nodeValue", $program->stderr);
        }
    }
}

class Call extends Instruction {
    public String $label;
    public static function execute(Program $program): void {
        $label = parent::getArg(1, $program->stderr);
        
        array_push($program->callArray, $program->currPosition);
        $program->currPosition = $program->findLabelIndex($label->nodeValue);
    }
}

class ReturnFromCall extends Instruction {
    public static function execute(Program $program): void {
        if (empty($program->callArray)) {
            ErrorHandler::handleError(ErrCode::MISSING_VALUE, "No call to return from", $program->stderr);
        }
        $program->currPosition = array_pop($program->callArray);
    }
}

class PushS extends Instruction {
    public mixed $symb;
    public static function execute(Program $program): void {
        $symb = parent::getArg(1, $program->stderr);

        $data = $program->extractData($symb);
        array_push($program->stack, $data);
    }
}

class PopS extends Instruction {
    public mixed $var;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        if(empty($program->stack)){
            ErrorHandler::handleError(ErrCode::MISSING_VALUE, "Empty stack", $program->stderr);
        } else if(!$program->isValidVar($var->nodeValue) || !$program->isDefined($var->nodeValue)){
            ErrorHandler::handleError(ErrCode::NON_EXISTING_VAR, "Variable not defined: " . $var->nodeValue, $program->stderr);
        }
        $data = array_pop($program->stack);
        $program->storeData($var->nodeValue, $data["value"], $data["type"]);
    }
}

class Add extends Instruction {
    public mixed $var;
    public mixed  $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $result = $program->executeOperation($program, $symb1, $symb2, "ADD");
        $program->storeData($var->nodeValue, $result, "int");
    }
}

class Sub extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $result = $program->executeOperation($program, $symb1, $symb2, "SUB");
        $program->storeData($var->nodeValue, $result, "int");
    }
}

class Mul extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $result = $program->executeOperation($program, $symb1, $symb2, "MUL");
        $program->storeData($var->nodeValue, $result, "int");
    }
}

class IDiv extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $result = $program->executeOperation($program, $symb1, $symb2, "DIV");
        $program->storeData($var->nodeValue, $result, "int");
    }
}

class Lt extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        if ($op1["type"] === "nil" || $op2["type"] === "nil") {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }

        if (($op1["type"] === 'int' && $op2["type"] === 'int') || ($op1["type"] === 'string' && $op2["type"] === 'string')) {
            $result = $op1["value"] < $op2["value"] ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } else if ($op1["type"] === 'bool' && $op2["type"] === 'bool') {
            $result_op1 = $op1["value"] === "true" ? 1 : 0;
            $result_op2 = $op2["value"] === "true" ? 1 : 0;
            $result = $result_op1 < $result_op2 ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } 
        else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}
class Gt extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        if ($op1["type"] === "nil" || $op2["type"] === "nil") {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }

        if (($op1["type"] === 'int' && $op2["type"] === 'int') || ($op1["type"] === 'string' && $op2["type"] === 'string')) {
            $result = $op1["value"] > $op2["value"] ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } else if ($op1["type"] === 'bool' && $op2["type"] === 'bool') {
            $result_op1 = $op1["value"] === "true" ? 1 : 0;
            $result_op2 = $op2["value"] === "true" ? 1 : 0;
            $result = $result_op1 > $result_op2 ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } 
        else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}
class Eq extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        if ($op1["type"] === "nil" || $op2["type"] === "nil") {
            if ($op1["type"] === "nil" && $op2["type"] === "nil") {
                $program->storeData($var->nodeValue, "true", "bool");
            }
            else {
                $program->storeData($var->nodeValue, "false", "bool");
            }
        } else if (($op1["type"] === 'int' && $op2["type"] === 'int') || ($op1["type"] === 'string' && $op2["type"] === 'string')) {
            $result = $op1["value"] === $op2["value"] ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } else if ($op1["type"] === 'bool' && $op2["type"] === 'bool') {
            $result = $op1["value"] === $op2["value"] ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } 
        else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}

class AndInstr extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        if ($op1["type"] === "bool" && $op2["type"] === "bool") {
            $result = $op1["value"] === "true" && $op2["value"] === "true" ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}
class OrInstr extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        if ($op1["type"] === "bool" && $op2["type"] === "bool") {
            $result = $op1["value"] === "true" || $op2["value"] === "true" ? "true" : "false";
            $program->storeData($var->nodeValue, $result, "bool");
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}
class NotInstr extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);

        $op1 = $program->extractData($symb1);

        if ($op1["type"] === "bool") {
            $result = $op1["value"] === "true" ? "false" : "true";
            $program->storeData($var->nodeValue, $result, "bool");
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}

    class Int2Char extends Instruction {
    public mixed $var;
    public mixed $symb;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb = parent::getArg(2, $program->stderr);

        $data = $program->extractData($symb);

        $data_value = $data["value"];
        if ($data["type"] === 'int') {
            if(mb_chr($data_value)){
                $program->storeData($var->nodeValue, mb_chr($data_value), "string");
            }else {
                ErrorHandler::handleError(ErrCode::NON_VALID_STRING, "Invalid ASCII code", $program->stderr);
            }
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}

class Stri2Int extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $data1 = $program->extractData($symb1);
        $data2 = $program->extractData($symb2);

        $data1_value = $data1["value"];
        $data2_value = $data2["value"];

        if ($data1["type"] === "string" && $data2["type"] === "int") {
            if($data2_value < 0 || $data2_value >= strlen($data1_value)){
                ErrorHandler::handleError(ErrCode::NON_VALID_STRING, "Index out of range", $program->stderr);
            }
            $result = mb_ord($data1_value[$data2_value]);
            $program->storeData($var->nodeValue, $result, "int");
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}

class Read extends Instruction {
    public mixed $var;
    public mixed $type;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $type = parent::getArg(2, $program->stderr);
        
        if($var->getAttribute("type") !== "var"){
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand", $program->stderr);
        } else if ($type->getAttribute("type") !== "type") {
            ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Wrong XML structure", $program->stderr);
        }

        if ($type->nodeValue === "int") {
            $input = $program->stdin->readInt();
            if (is_numeric($input) && $input !== null) {
                $program->storeData($var->nodeValue, intval($input), "int");
            } else {
                $program->storeData($var->nodeValue, "nil", "nil");
            }
        } else if ($type->nodeValue === "string") {
            $input = $program->stdin->readString();
            if ($input === null) {
                $program->storeData($var->nodeValue, "nil", "nil");
            } else {
                $program->storeData($var->nodeValue, $input, "string");
            }
        } else if ($type->nodeValue === "bool") {
            $input = $program->stdin->readBool();
            if ($input == "true") {
                $program->storeData($var->nodeValue, "true", "bool");
            } else if ($input == "false") {
                $program->storeData($var->nodeValue, "false", "bool");
            } else {
                $program->storeData($var->nodeValue, "nil", "nil");
            }
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand", $program->stderr);
        }
    }
}

class Write extends Instruction {
    public mixed $symb;
    public static function execute(Program $program): void {
        $symb = parent::getArg(1, $program->stderr);

        $data = $program->extractData($symb);

        $data_value = $data["value"];

        if ($data["type"] === 'nil') {
            $output = "";
        } else {
            $output = $program->stringCheck($data_value);
        }

        fwrite(STDOUT, $output);
    }
}

class Concat extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        $op1_value = $op1["value"];
        $op2_value = $op2["value"];

        if ($op1["type"] !== 'string' || $op2["type"] !== 'string') {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand", $program->stderr);
        }
        $result = $op1_value . $op2_value;
        $program->storeData($var->nodeValue, $result, "string");
    }
}

class StrLen extends Instruction {
    public mixed $var;
    public mixed $symb;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb = parent::getArg(2, $program->stderr);

        $op1 = $program->extractData($symb);
        $op1_value = $op1["value"];

        if ($op1["type"] !== 'string') {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand", $program->stderr);
        }
        $result = strlen($op1_value);
        $program->storeData($var->nodeValue, $result, "int");
    }
}

class GetChar extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        $op1_value = $op1["value"];
        $op2_value = $op2["value"];

        if ($op1["type"] == 'string' && $op2["type"] === 'int') {
            if($op2_value < 0 || $op2_value >= strlen($op1_value)){
                ErrorHandler::handleError(ErrCode::NON_VALID_STRING, "Index out of range", $program->stderr);
            }
            $result = $op1_value[$op2_value];
            $program->storeData($var->nodeValue, $result, "string");
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
    }
}

class SetChar extends Instruction {
    public mixed $var;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        $string = $program->extractData($var);

        if ($op1["type"] === 'int' && $op2["type"] === 'string' && $string["type"] === 'string') {
            if ($op1["value"] < 0 || $op1["value"] >= strlen($string["value"])) {
                ErrorHandler::handleError(ErrCode::NON_VALID_STRING, "Index out of range", $program->stderr);
            }
            $string["value"][$op1["value"]] = $op2["value"];    
            $program->storeData($var->nodeValue, $string["value"], "string");
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand", $program->stderr);
        }
    }
}

class Type extends Instruction {
    public mixed $var;
    public mixed $symb;
    public static function execute(Program $program): void {
        $var = parent::getArg(1, $program->stderr);
        $symb = parent::getArg(2, $program->stderr);

        $data = $program->extractData($symb);

        switch ($data["type"]) {
            case "int":
                $program->storeData($var->nodeValue, "int", "string");
                break;
            case "string":
                $program->storeData($var->nodeValue, "string", "string");
                break;
            case "bool":
                $program->storeData($var->nodeValue, "bool",  "string");
                break;
            case "nil":
                $program->storeData($var->nodeValue, "nil",  "string");
                break;
            case null:
                $program->storeData($var->nodeValue, "",  "string");
                break;
            default:
                ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Unknown type", $program->stderr);
            }
        
    }
}

class Label extends Instruction {
    public static function execute(Program $program): void {
    }
}

class Jump extends Instruction {
    public String $label;
    public static function execute(Program $program): void {
        $label = parent::getArg(1, $program->stderr);
        
        $program->currPosition = $program->findLabelIndex($label->nodeValue);
    }
}

class JumpIfEq extends Instruction {
    public mixed $label;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $label = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        if ($op1["type"] === "nil" || $op2["type"] === "nil") {
            if ($op1["type"] === "nil" && $op2["type"] === "nil"){
                $program->currPosition = $program->findLabelIndex($label->nodeValue);
            }
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
        if ($label->getAttribute("type") === "label"){
            if  ($op1["type"] === $op2["type"]) {
                if (($op1["value"] == $op2["value"])) {
                    $program->currPosition = $program->findLabelIndex($label->nodeValue);
                }
            } else {
                ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
            }
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Wrong format of arguments", $program->stderr);
        }
    }
}

class JumpIfNeq extends Instruction {
    public mixed $label;
    public mixed $symb1;
    public mixed $symb2;
    public static function execute(Program $program): void {
        $label = parent::getArg(1, $program->stderr);
        $symb1 = parent::getArg(2, $program->stderr);
        $symb2 = parent::getArg(3, $program->stderr);

        $op1 = $program->extractData($symb1);
        $op2 = $program->extractData($symb2);

        if ($op1["type"] === "nil" || $op2["type"] === "nil") {
            if ($op1["type"] === "nil" && $op2["type"] === "nil"){
                $program->currPosition = $program->findLabelIndex($label->nodeValue);
            }
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
        if ($label->getAttribute("type") === "label"){
            if  ($op1["type"] === $op2["type"]) {
                if (($op1["value"] != $op2["value"])) {
                    $program->currPosition = $program->findLabelIndex($label->nodeValue);
                }
            } else {
                ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
            }
        } else {
            ErrorHandler::handleError(ErrCode::WRONG_XML_STRUCTURE, "Wrong format of arguments", $program->stderr);
        }
    }
}

class ExitProg extends Instruction {
    public mixed $symb;
    public static function execute(Program $program): void {
        $symb = parent::getArg(1, $program->stderr);

        $data = $program->extractData($symb);
        $data_value = $data["value"];
        
        if($data["type"] !== "int"){
            ErrorHandler::handleError(ErrCode::WRONG_OPERAND, "Wrong operand type", $program->stderr);
        }
        if ($data_value >= 0 && $data_value <= 9){
            
            exit((int)$data_value);
        } else {
            ErrorHandler::handleError(ErrCode::NON_VALID_VALUE, "Exit code out of range", $program->stderr);
        }
    }
}

class DPrint extends Instruction {
    public mixed $symb;
    public static function execute(Program $program): void {
        $symb = parent::getArg(1, $program->stderr);
        $data = $program->extractData($symb);

        $data_value = $data["value"];

        $output = $program->stringCheck($data_value);

        $program->stderr->writeString($output);
    }
}

class Breakpoint extends Instruction {
    public static function execute(Program $program): void {
        $program->stderr->writeString("BREAK: Caused by instruction " . $program->instructions[$program->currPosition]->nodeValue . "(" . $program->currPosition . ")" . "\n)");
    }
}


