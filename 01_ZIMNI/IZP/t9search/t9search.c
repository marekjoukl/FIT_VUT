/*
    - VUT FIT, 1BIT
    - IZP PROJEKT 1 - Práce s textem
    - Marek Joukl
*/ 
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

// my str_len function (finfing length of string)
int str_len(char *str) {
    
    int counter = 0;
    for(int i = 0; str[i] != '\0'; i++){
        counter++;
    }
    return counter;
}

// trasfer numbers to euqal letter
int num_to_letter(char letter1, char letter2) {
    if (letter1 == 50)
    {
        letter1 = 97; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 51)
    {
        letter1 = 100; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 52)
    {
        letter1 = 103; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 53)
    {
        letter1 = 106; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 54)
    {
        letter1 = 109; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 55)
    {
        letter1 = 112; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2 || letter1 + 3 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 56)
    {
        letter1 = 116; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 57)
    {
        letter1 = 119; 
        if (letter1 == letter2 || letter1 + 1 == letter2 || letter1 + 2 == letter2 || letter1 + 3 == letter2)
        {
            return 1;
        }
    }
    if (letter1 == 48)
    {
        letter1 = 43; 
        if (letter1 == letter2)
        {
            return 1;
        }
    }
    return 0;
}

//my capital to lower letter function
char lower_key(char letter) {
    int diff = 'a' - 'A';
    if (letter >= 'A' && letter <= 'Z')
    {
        letter += diff;
    }
    return letter;
}

// find equality between input_num and names
int equality_char(char *str, char *num) {
    str[strlen(str) -1] = '\0'; // for printing buffer on the same line
    for (int i = 0; str[i] != '\0'; i++)
    {
        for (int j = 0; j < str_len(num); j++)
        {
            if (num_to_letter(num[j], lower_key(str[i])) || num[j] == str[i])
            {
                i++;
            } else {
                break;
            }
            //if j(=input num) made it to the last check -> equality ends
            if (j == (str_len(num)-1))
            {
                return 1;
            }
        }
    }
    return 0;
}

// my isdigit function (checks if string contains only numbers)
int my_isdigit(char *str) {

    int counter = 0;

    for (int i = 0; str[i] != '\0'; i++) {
        if(str[i] >= '0' && str[i] <= '9') {
            counter++;
        }
    }
    if(counter != str_len(str)) {
        return 0;
    }
    return 1;
}

int main(int argc, char* argv[]) {

    char buffer_name[101];
    char buffer_num[101];
    char *input_num = argv[1];
    int not_found = 0;

    if (argc == 2)
    {
        while (fgets(buffer_name, 101, stdin) != NULL && fgets(buffer_num, 101, stdin) != NULL)
        {
            if (equality_char(buffer_name, input_num) || strstr(buffer_num, input_num) || equality_char(buffer_num, input_num))
            {
                fprintf(stdout, "%s, %s", buffer_name, buffer_num);
                not_found = 1;
            }
            if (!my_isdigit(input_num))
            {
                fprintf(stderr, "%s","not a number");
                return -1;
            }
        }
        if (not_found == 0)
        {
            fprintf(stdout, "%s", "not found");
        }
    }

    if (argc > 2)
    {
        fprintf(stderr, "ERROR: too many inputs\n");
            return -1;
    }

    if (argc < 2)
    {
        while (fgets(buffer_name, 101, stdin) != NULL && fgets(buffer_num, 101, stdin) != NULL) {
            buffer_name[strlen(buffer_name) -1] = '\0';
            buffer_num[strlen(buffer_num) -1] = '\0';
            fprintf(stdout, "%s, %s\n", buffer_name, buffer_num);
        }
    }
    return 0;
}