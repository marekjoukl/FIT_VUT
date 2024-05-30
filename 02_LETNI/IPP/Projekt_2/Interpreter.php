<?php

namespace IPP\Student;

use IPP\Core\AbstractInterpreter;

include "InstructionParser.php";

class Interpreter extends AbstractInterpreter
{
    public function execute(): int
    {   
        $dom = $this->source->getDOMDocument();
        
        // Extract root element from XML input file
        $root = $dom->documentElement;

        // Regroup instructions based on their order
        $newRoot = InstructionChecker::regroupInstructions($root, $this->stderr);

        // Check the instruction validity and structure of the XML file
        $instructions = InstructionChecker::checkInstructions($newRoot, $this->stderr);

        // Execute the program
        $program = new Program($instructions, $this->stderr, $this->input);
        $program->execute($program);

        exit(0);
    }
}
