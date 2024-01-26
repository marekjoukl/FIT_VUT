/* ******************************* c204.c *********************************** */
/*  Předmět: Algoritmy (IAL) - FIT VUT v Brně                                 */
/*  Úkol: c204 - Převod infixového výrazu na postfixový (s využitím c202)     */
/*  Referenční implementace: Petr Přikryl, listopad 1994                      */
/*  Přepis do jazyka C: Lukáš Maršík, prosinec 2012                           */
/*  Upravil: Kamil Jeřábek, září 2019                                         */
/*           Daniel Dolejška, září 2021                                       */
/* ************************************************************************** */
/*
** Implementujte proceduru pro převod infixového zápisu matematického výrazu
** do postfixového tvaru. Pro převod využijte zásobník (Stack), který byl
** implementován v rámci příkladu c202. Bez správného vyřešení příkladu c202
** se o řešení tohoto příkladu nepokoušejte.
**
** Implementujte následující funkci:
**
**    infix2postfix ... konverzní funkce pro převod infixového výrazu
**                      na postfixový
**
** Pro lepší přehlednost kódu implementujte následující pomocné funkce:
**    
**    untilLeftPar ... vyprázdnění zásobníku až po levou závorku
**    doOperation .... zpracování operátoru konvertovaného výrazu
**
** Své řešení účelně komentujte.
**
** Terminologická poznámka: Jazyk C nepoužívá pojem procedura.
** Proto zde používáme pojem funkce i pro operace, které by byly
** v algoritmickém jazyce Pascalovského typu implemenovány jako procedury
** (v jazyce C procedurám odpovídají funkce vracející typ void).
**
**/

#include "c204.h"

bool solved;

/**
 * Pomocná funkce untilLeftPar.
 * Slouží k vyprázdnění zásobníku až po levou závorku, přičemž levá závorka bude
 * také odstraněna.
 * Pokud je zásobník prázdný, provádění funkce se ukončí.
 *
 * Operátory odstraňované ze zásobníku postupně vkládejte do výstupního pole
 * znaků postfixExpression.
 * Délka převedeného výrazu a též ukazatel na první volné místo, na které se má
 * zapisovat, představuje parametr postfixExpressionLength.
 *
 * Aby se minimalizoval počet přístupů ke struktuře zásobníku, můžete zde
 * nadeklarovat a používat pomocnou proměnnou typu char.
 *
 * @param stack Ukazatel na inicializovanou strukturu zásobníku
 * @param postfixExpression Znakový řetězec obsahující výsledný postfixový výraz
 * @param postfixExpressionLength Ukazatel na aktuální délku výsledného postfixového výrazu
 */
void untilLeftPar( Stack *stack, char *postfixExpression, unsigned *postfixExpressionLength ) {
    while(!Stack_IsEmpty(stack) && stack->array[stack->topIndex] != '(')
    {
        postfixExpression[(*postfixExpressionLength)++] = stack->array[stack->topIndex];
        Stack_Pop(stack);
    }
    if (!Stack_IsEmpty(stack)) {
        Stack_Pop(stack);
    }
}

// my own function for setting priority of operators
int priority(char symbol){
    switch (symbol)
    {
    case '+':
    case '-':
        return 1;   // lower priroity
    case '/':
    case '*':
        return 2;   // higher priority
    default:
        return 0;
    }
}

/**
 * Pomocná funkce doOperation.
 * Zpracuje operátor, který je předán parametrem c po načtení znaku ze
 * vstupního pole znaků.
 *
 * Dle priority předaného operátoru a případně priority operátoru na vrcholu
 * zásobníku rozhodneme o dalším postupu.
 * Délka převedeného výrazu a taktéž ukazatel na první volné místo, do kterého
 * se má zapisovat, představuje parametr postfixExpressionLength, výstupním
 * polem znaků je opět postfixExpression.
 *
 * @param stack Ukazatel na inicializovanou strukturu zásobníku
 * @param c Znak operátoru ve výrazu
 * @param postfixExpression Znakový řetězec obsahující výsledný postfixový výraz
 * @param postfixExpressionLength Ukazatel na aktuální délku výsledného postfixového výrazu
 */
void doOperation( Stack *stack, char c, char *postfixExpression, unsigned *postfixExpressionLength ) {
    while (!Stack_IsEmpty(stack)) {
        char top;
        Stack_Top(stack, &top);
        // Check if the top element is '(' or has lower priority than the current operator 'c'
        if (top == '(' || priority(top) < priority(c)) {
            break;
        }
        postfixExpression[(*postfixExpressionLength)++] = top;
        Stack_Pop(stack);
    }
    Stack_Push(stack, c);
}

/**
 * Konverzní funkce infix2postfix.
 * Čte infixový výraz ze vstupního řetězce infixExpression a generuje
 * odpovídající postfixový výraz do výstupního řetězce (postup převodu hledejte
 * v přednáškách nebo ve studijní opoře).
 * Paměť pro výstupní řetězec (o velikosti MAX_LEN) je třeba alokovat. Volající
 * funkce, tedy příjemce konvertovaného řetězce, zajistí korektní uvolnění zde
 * alokované paměti.
 *
 * Tvar výrazu:
 * 1. Výraz obsahuje operátory + - * / ve významu sčítání, odčítání,
 *    násobení a dělení. Sčítání má stejnou prioritu jako odčítání,
 *    násobení má stejnou prioritu jako dělení. Priorita násobení je
 *    větší než priorita sčítání. Všechny operátory jsou binární
 *    (neuvažujte unární mínus).
 *
 * 2. Hodnoty ve výrazu jsou reprezentovány jednoznakovými identifikátory
 *    a číslicemi - 0..9, a..z, A..Z (velikost písmen se rozlišuje).
 *
 * 3. Ve výrazu může být použit předem neurčený počet dvojic kulatých
 *    závorek. Uvažujte, že vstupní výraz je zapsán správně (neošetřujte
 *    chybné zadání výrazu).
 *
 * 4. Každý korektně zapsaný výraz (infixový i postfixový) musí být uzavřen
 *    ukončovacím znakem '='.
 *
 * 5. Při stejné prioritě operátorů se výraz vyhodnocuje zleva doprava.
 *
 * Poznámky k implementaci
 * -----------------------
 * Jako zásobník použijte zásobník znaků Stack implementovaný v příkladu c202.
 * Pro práci se zásobníkem pak používejte výhradně operace z jeho rozhraní.
 *
 * Při implementaci využijte pomocné funkce untilLeftPar a doOperation.
 *
 * Řetězcem (infixového a postfixového výrazu) je zde myšleno pole znaků typu
 * char, jenž je korektně ukončeno nulovým znakem dle zvyklostí jazyka C.
 *
 * Na vstupu očekávejte pouze korektně zapsané a ukončené výrazy. Jejich délka
 * nepřesáhne MAX_LEN-1 (MAX_LEN i s nulovým znakem) a tedy i výsledný výraz
 * by se měl vejít do alokovaného pole. Po alokaci dynamické paměti si vždycky
 * ověřte, že se alokace skutečně zdrařila. V případě chyby alokace vraťte namísto
 * řetězce konstantu NULL.
 *
 * @param infixExpression vstupní znakový řetězec obsahující infixový výraz k převedení
 *
 * @returns znakový řetězec obsahující výsledný postfixový výraz
 */
char *infix2postfix(const char *infixExpression) {
    // Allocation of space for postfix expression
    char *postfixExpression = (char *)malloc(sizeof(char) * MAX_LEN);
    if (postfixExpression == NULL) Stack_Error(SERR_INIT);
    Stack stack;
    Stack_Init(&stack);

    unsigned infixIndex = 0;
    unsigned postfixIndex = 0;
    char sym;

	// taking parameters from left to right from infixExpression
    while (infixExpression[infixIndex] != '\0') {
        sym = infixExpression[infixIndex];
        switch (sym) {
            case '(':							// if the symbol is '(', push it into the stack
                Stack_Push(&stack, sym);
                break;
            case ')':							// if the symbol is '(', call function untilLeftPar to clear the stack up to '('
                untilLeftPar(&stack, postfixExpression, &postfixIndex);  
                break;
            case '+':
            case '-':
            case '*':
            case '/':							// if the symbol is operand, call function doOperation to process it
                doOperation(&stack, sym, postfixExpression, &postfixIndex);  
                break;
			case '=':							// cycle through the stack and add symbols to postfixExpression until the stack is empty
				while (!Stack_IsEmpty(&stack)) {
					Stack_Top(&stack, &postfixExpression[postfixIndex++]);
					Stack_Pop(&stack);
				}
				postfixExpression[postfixIndex++] = '=';	// after the stack is empty, add '='
				break;

            default:							// if the symbol is id or number, add it to postfixExpression
                if ((sym >= '0' && sym <= '9') || (sym >= 'a' && sym <= 'z') || (sym >= 'A' && sym <= 'Z')) {
                    postfixExpression[postfixIndex++] = sym;
                }
                break;
        } // switch
        infixIndex++;
    } // while
    postfixExpression[postfixIndex] = '\0';		// add null terminator to indicate end of the postfixExpression

    Stack_Dispose(&stack);						// free stack
    return postfixExpression;					// return final expression
}



/**
 * Pomocná metoda pro vložení celočíselné hodnoty na zásobník.
 *
 * Použitá implementace zásobníku aktuálně umožňuje vkládání pouze
 * hodnot o velikosti jednoho byte (char). Využijte této metody
 * k rozdělení a postupné vložení celočíselné (čtyřbytové) hodnoty
 * na vrchol poskytnutého zásobníku.
 *
 * @param stack ukazatel na inicializovanou strukturu zásobníku
 * @param value hodnota k vložení na zásobník
 */
void expr_value_push(Stack *stack, int value) {
    if (Stack_IsFull(stack)) {
        Stack_Error(SERR_PUSH);
    } else {
        stack->topIndex++;
        stack->array[stack->topIndex] = value;
    }
}

/**
 * Pomocná metoda pro extrakci celočíselné hodnoty ze zásobníku.
 *
 * Využijte této metody k opětovnému načtení a složení celočíselné
 * hodnoty z aktuálního vrcholu poskytnutého zásobníku. Implementujte
 * tedy algoritmus opačný k algoritmu použitému v metodě
 * `expr_value_push`.
 *
 * @param stack ukazatel na inicializovanou strukturu zásobníku
 * @param value ukazatel na celočíselnou proměnnou pro uložení
 *   výsledné celočíselné hodnoty z vrcholu zásobníku
 */
void expr_value_pop(Stack *stack, int *value) {
    if (!Stack_IsEmpty(stack)) {
        *value = stack->array[stack->topIndex];
        stack->topIndex--; 
    } else {
        Stack_Error(SERR_TOP);
    }
}

// my own isdigit function
bool isDigit (char c) {
    if ((c >= '0') && (c <= '9')) return true;
    return false;
}

// my own isAlpha functon
bool isAlpha(char c){
    return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ? true : false);
}

// function for finding value of variable
int valueSearch (char c, VariableValue variableValues[], int variableValueCount){
	for (int i = 0; i < variableValueCount; i++)
	{
		if (c == variableValues[i].name)
		{
			return variableValues[i].value;
		}				
	}
	return 0;
}

/**
 * Tato metoda provede vyhodnocení výrazu zadaném v `infixExpression`,
 * kde hodnoty proměnných použitých v daném výrazu jsou definovány
 * v poli `variableValues`.
 *
 * K vyhodnocení vstupního výrazu využijte implementaci zásobníku
 * ze cvičení c202. Dále také využijte pomocných funkcí `expr_value_push`,
 * respektive `expr_value_pop`. Při řešení si můžete definovat libovolné
 * množství vlastních pomocných funkcí.
 *
 * Předpokládejte, že hodnoty budou vždy definovány
 * pro všechy proměnné použité ve vstupním výrazu.
 *
 * @param infixExpression vstpní infixový výraz s proměnnými
 * @param variableValues hodnoty proměnných ze vstupního výrazu
 * @param variableValueCount počet hodnot (unikátních proměnných
 *   ve vstupním výrazu)
 * @param value ukazatel na celočíselnou proměnnou pro uložení
 *   výsledné hodnoty vyhodnocení vstupního výrazu
 *
 * @return výsledek vyhodnocení daného výrazu na základě poskytnutých hodnot proměnných
 */
bool eval( const char *infixExpression, VariableValue variableValues[], int variableValueCount, int *value ) {
    char *postfixExpression = infix2postfix(infixExpression);
    Stack stack;
    Stack_Init(&stack);

    // loop through the postfixExpression until '='
    for (int i = 0; postfixExpression[i] != '='; i++) {
        char sym = postfixExpression[i];

        if (isDigit(sym)) {
            // if the symbol is a digit, convert it to an integer and push it into the stack
			int tmp = (int)(sym - '0');
            expr_value_push(&stack, tmp);
        } else if (isAlpha(sym)) {
            // search for corresponding value of variable and push it into the stack
            expr_value_push(&stack, valueSearch(sym, variableValues, variableValueCount));
        } else {
            int val1 = 0, val2 = 0;
            expr_value_pop(&stack, &val1);
            expr_value_pop(&stack, &val2);
            switch (sym) {
                case '+':
                    expr_value_push(&stack, val2 + val1);
                    break;
                case '-':
                    expr_value_push(&stack, val2 - val1);
                    break;
                case '*':
                    expr_value_push(&stack, val2 * val1);
                    break;
                case '/':
                    if (val1 != 0) {
                        expr_value_push(&stack, val2 / val1);
                    } else {
                        // handle division by zero
                        Stack_Dispose(&stack);
                        free(postfixExpression);
                        return false;
                    }
                    break;
                default:
                    break;
            }
        }
    } // for

    if (!Stack_IsEmpty(&stack)) {
        // pop the final result and return it
        expr_value_pop(&stack, value);
        Stack_Dispose(&stack);
        free(postfixExpression);
        return true;
    } else {
        // handle invalid expression
        Stack_Dispose(&stack);
        free(postfixExpression);
        return false;
    }
}

/* Konec c204.c */
