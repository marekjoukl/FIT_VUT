#####################################
#           IPP Project 1           #
#   author: Marek Joukl (xjoukl00)  #
#        date: 13. 2. 2024          #
#         file: parse.py            #
#####################################

import sys
import re

############### PRINT FUNCTIONS ###############


def print_help():
    print("INFO:            The filter script reads the source code in IPPcode24 from the standard input,")
    print("                 checks the lexical and syntactic correctness of the code and prints it to the ")
    print("                 standard output as standard XML representation of the program.\n")
    print("USAGE:           python3.10 parse.py input output")

def add_to_output(string):
    global output_list
    output_list.append(string)

def print_output():
    for i, string in enumerate(output_list):
        if i == len(output_list) - 1:
            print(string, end='')
        else:
            print(string) 

# prints symbol argument
def print_symb(string, arg_num):
    if string.split("@")[1] == '':
            add_to_output(f'\t\t<arg{arg_num} type="{string.split("@")[0]}"/>')
    else:
        # check whether the symb is var or constant
        if re.match(r'^(LF|TF|GF)$', string.split("@")[0]):
            add_to_output(f'\t\t<arg{arg_num} type="var">{string}</arg{arg_num}>')
        else:
            if string.split("@")[1].upper() in {'TRUE', 'FALSE'}:
                add_to_output(f'\t\t<arg{arg_num} type="{string.split("@")[0]}">{"".join(string.split("@")[1:]).lower()}</arg{arg_num}>')
            else:
                add_to_output(f'\t\t<arg{arg_num} type="{string.split("@")[0]}">{"@".join(string.split("@")[1:])}</arg{arg_num}>')

############### INPUT CHECK FUNCTIONS ###############

# checks if variable has correct format
def var_check(string) -> bool:
    pattern = r'^(LF|TF|GF)@[a-zA-Z_\-$&%\*!\?][\w\-$&%\*!\?]*$'    # pattern that represents variable
    return True if re.match(pattern, string) else False

# checks validity of oppcode
def check_opcode(string) -> int:
    if string in {'RETURN', 'BREAK', 'CREATEFRAME', 'PUSHFRAME', 'POPFRAME', 'DEFVAR', 'POPS', 'CALL', 'LABEL', 'JUMP', 'PUSHS', 'WRITE', 'EXIT', 'DPRINT',
                  'MOVE', 'INT2CHAR', 'STRLEN', 'TYPE', 'ADD', 'SUB', 'MUL', 'IDIV', 'LT', 'GT', 'EQ', 'AND', 'OR', 'STRI2INT', 'CONCAT', 'GETCHAR', 'SETCHAR',
                  'NOT', 'READ', 'JUMPIFEQ', 'JUMPIFNEQ'}:
        return 1
    elif string == ".IPPCODE24":
        return 2
    else:
        return 0

# checks if symbol has correct format
def symb_check(string) -> bool:
    if "@" in string:
        match string.split("@")[0].upper():
            case 'STRING':
                new_str = "".join(string.split("@")[1:])
                pattern = r'^(\d{3})'
                pattern2 = r'[a-zA-Z0-9]*'
                parts = re.split(r'\\', new_str)
                if len(parts) == 1 and re.match(pattern2, parts[0]):
                    return True
                else:
                    for part in parts[1:]:
                        return True if re.match(pattern, part) else False
            case 'NIL':
                return True if string.split("@")[1].upper() == 'NIL' else False
            case 'INT':
                pattern = r'^([+-]?(0x[0-9a-fA-F]+|\d+|0o[0-7]+))$'
                return True if re.match(pattern, string.split("@")[1]) else False
            case 'BOOL':
                return True if string.split("@")[1].upper() in {'TRUE', 'FALSE'} else False
            case _:
                return False
    else:
        return False
        
# checks if type has correct format
def type_check(string) -> bool:
    return True if string in {'int', 'bool', 'nil', 'string'} else False

############### FUNCTIONS FOR INSTRUCTIONS ###############

# no arguments required
def no_args(keyword, count, num_of_args):
    if num_of_args == 1:
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')
        add_to_output("</instruction>")
    else:
        print("ERROR: Wrong number of operands!", file=sys.stderr)
        sys.exit(23)

# <var>
def var_arg(keyword, count, num_of_args, keywords):
    if (num_of_args == 2 and var_check(keywords[1])):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')
        add_to_output(f'\t\t<arg1 type="var">{keywords[1]}</arg1>')

        keywords[1] = keywords[1].replace('&', '&amp;')
        keywords[1] = keywords[1].replace('<', '&lt;')
        keywords[1] = keywords[1].replace('>', '&gt;')
        keywords[1] = keywords[1].replace("\'", '&apos;')
        keywords[1] = keywords[1].replace('"', '&quot;')

        add_to_output('\t</instruction>')
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

# <var> <symb>
def var_symb(keyword, count, num_of_args, keywords):
    if (num_of_args == 3 and var_check(keywords[1]) and (symb_check(keywords[2]) or var_check(keywords[2]))):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')
        add_to_output(f'\t\t<arg1 type="var">{keywords[1]}</arg1>')
        
        keywords[2] = keywords[2].replace('&', '&amp;')
        keywords[2] = keywords[2].replace('<', '&lt;')
        keywords[2] = keywords[2].replace('>', '&gt;')
        keywords[2] = keywords[2].replace("\'", '&apos;')
        keywords[2] = keywords[2].replace('"', '&quot;')

        print_symb(keywords[2], 2)

        add_to_output("\t</instruction>")
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

# <label>
def label_arg(keyword, count, num_of_args, keywords):
    if num_of_args == 2 and not re.search("@",keywords[1]):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')
        add_to_output(f'\t\t<arg1 type="label">{keywords[1]}</arg1>')
        add_to_output("\t</instruction>")
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

# <symb>
def symb_arg(keyword, count, num_of_args, keywords):
    if (num_of_args == 2 and (symb_check(keywords[1]) or var_check(keywords[1]))):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')
        keywords[1] = keywords[1].replace('&', '&amp;')
        keywords[1] = keywords[1].replace('<', '&lt;')
        keywords[1] = keywords[1].replace('>', '&gt;')
        keywords[1] = keywords[1].replace("\'", '&apos;')
        keywords[1] = keywords[1].replace('"', '&quot;')
        print_symb(keywords[1], 1)

        add_to_output("\t</instruction>")
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

# <var> <symb1> <symb2>
def var_2symb(keyword, count, num_of_args, keywords):
    if (num_of_args == 4 and var_check(keywords[1]) and (symb_check(keywords[2]) or var_check(keywords[2])) and (symb_check(keywords[3]) or var_check(keywords[3]))):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')

        add_to_output(f'\t\t<arg1 type="var">{keywords[1]}</arg1>')
        keywords[2] = keywords[2].replace('&', '&amp;')
        keywords[2] = keywords[2].replace('<', '&lt;')
        keywords[2] = keywords[2].replace('>', '&gt;')
        keywords[2] = keywords[2].replace("\'", '&apos;')
        keywords[2] = keywords[2].replace('"', '&quot;')
        keywords[3] = keywords[3].replace('&', '&amp;')
        keywords[3] = keywords[3].replace('<', '&lt;')
        keywords[3] = keywords[3].replace('>', '&gt;')
        keywords[3] = keywords[3].replace("\'", '&apos;')
        keywords[3] = keywords[3].replace('"', '&quot;')
        print_symb(keywords[2], 2)
        print_symb(keywords[3], 3)
        
        add_to_output("\t</instruction>")
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

# <var> <symb>
def not_arg(keyword, count, num_of_args, keywords):
    if (num_of_args == 3 and var_check(keywords[1]) and (symb_check(keywords[2]) or var_check(keywords[2]))):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')

        add_to_output(f'\t\t<arg1 type="var">{keywords[1]}</arg1>')
        
        print_symb(keywords[2], 2)
        
        add_to_output("\t</instruction>")
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

    
# <var> <type>
def var_type(keyword, count, num_of_args, keywords):
    if (num_of_args == 3 and var_check(keywords[1]) and type_check(keywords[2])):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')
        add_to_output(f'\t\t<arg1 type="var">{keywords[1]}</arg1>')
        add_to_output(f'\t\t<arg2 type="type">{keywords[2]}</arg2>')
        add_to_output("\t</instruction>")
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

# <label> <symb1> <symb2>
def label_2symb(keyword, count, num_of_args, keywords):
    if (num_of_args == 4 and not re.search("@", keywords[1]) and (symb_check(keywords[2]) or var_check(keywords[2])) and (symb_check(keywords[3]) or var_check(keywords[3]))):
        add_to_output(f'\t<instruction order="{count}" opcode="{keyword}">')
        add_to_output(f'\t\t<arg1 type="label">{keywords[1]}</arg1>')

        keywords[2] = keywords[2].replace('&', '&amp;')
        keywords[2] = keywords[2].replace('<', '&lt;')
        keywords[2] = keywords[2].replace('>', '&gt;')
        keywords[2] = keywords[2].replace("\'", '&apos;')
        keywords[2] = keywords[2].replace('"', '&quot;')
        keywords[3] = keywords[3].replace('&', '&amp;')
        keywords[3] = keywords[3].replace('<', '&lt;')
        keywords[3] = keywords[3].replace('>', '&gt;')
        keywords[3] = keywords[3].replace("\'", '&apos;')
        keywords[3] = keywords[3].replace('"', '&quot;')

        print_symb(keywords[2], 2)
        print_symb(keywords[3], 3)

        add_to_output("\t</instruction>")
    else:
        print("ERROR: Wrong number of operands or mistake in parameters!", file=sys.stderr)
        sys.exit(23)

############### MAIN BODY ###############

if __name__ == "__main__":

    if (len(sys.argv) == 2 and sys.argv[1] == "--help" and sys.argv[0] == "parse.py"):
        print_help()
        sys.exit(0)
    elif len(sys.argv) == 1 and sys.argv[0] == "parse.py":
        ### file handle ###
        output_list = []
        lines = sys.stdin.readlines()

        ### remove comments ###
        for i in range(len(lines)):
            lines[i] = re.sub(r'#.*$', '', lines[i].strip())
        lines = [line for line in lines if line.strip()]

        ### check for header ###
        if not lines or lines[0].strip().upper() != ".IPPCODE24":
            print("Error: File does not start with '.IPPcode24'.", file=sys.stderr)
            sys.exit(21)
        else:
            add_to_output('<?xml version="1.0" encoding="UTF-8"?>')
            add_to_output("<program language=\"IPPcode24\">")
            count = 1
            for line in lines[1:]:
                if check_opcode(line.split()[0].upper()) == 0:
                    print("Error: Wrong opcode", file=sys.stderr)
                    sys.exit(22)
                elif check_opcode(line.split()[0].upper()) == 2:
                    print("Error: Too many headers", file=sys.stderr)
                    sys.exit(23)

            for line in lines[1:]:
                keywords = line.split()
                ### TOKENS HANDLE ###
                # no arguments
                if keywords[0].upper() in {'RETURN', 'BREAK', 'CREATEFRAME', 'PUSHFRAME', 'POPFRAME'}:
                    no_args(keywords[0].upper(), count, len(keywords))

                # <var>
                elif keywords[0].upper() in {'DEFVAR', 'POPS'}:
                    var_arg(keywords[0].upper(), count, len(keywords), keywords)

                # <label>
                elif keywords[0].upper() in {'CALL', 'LABEL', 'JUMP'}:
                    label_arg(keywords[0].upper(), count, len(keywords), keywords)

                # <symb>
                elif keywords[0].upper() in {'PUSHS', 'WRITE', 'EXIT', 'DPRINT'}:

                    symb_arg(keywords[0].upper(), count, len(keywords), keywords)

                # <var> <symb>
                elif keywords[0].upper() in {'MOVE', 'INT2CHAR', 'STRLEN', 'TYPE'}:
                    var_symb(keywords[0].upper(), count, len(keywords), keywords)

                # <var> <symb1> <symb2>
                elif keywords[0].upper() in {'ADD', 'SUB', 'MUL', 'IDIV', 'LT', 'GT', 'EQ', 'AND', 'OR', 'STRI2INT', 'CONCAT', 'GETCHAR', 'SETCHAR'}:
                    var_2symb(keywords[0].upper(), count, len(keywords), keywords)
                
                # <var> <not>
                elif keywords[0].upper() in {'NOT'}:
                    not_arg(keywords[0].upper(), count, len(keywords), keywords)

                # <var> <type>
                elif keywords[0].upper() in {'READ'}:
                    var_type(keywords[0].upper(), count, len(keywords), keywords)
                
                # <label> <symb1> <symb2>
                elif keywords[0].upper() in {'JUMPIFEQ', 'JUMPIFNEQ'}:
                    label_2symb(keywords[0].upper(), count, len(keywords), keywords)

                else:
                    print("ERROR: Unknown instruction!", file=sys.stderr)
                    sys.exit(23)                

                count += 1

        add_to_output("</program>")
        print_output()
    else:
        print("ERROR: Wrong arguments. Use --help for info.", file=sys.stderr)
        sys.exit(10)
