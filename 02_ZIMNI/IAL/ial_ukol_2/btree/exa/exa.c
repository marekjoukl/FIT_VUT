/*
 * Použití binárních vyhledávacích stromů.
x *
 * S využitím Vámi implementovaného binárního vyhledávacího stromu (soubory ../iter/btree.c a ../rec/btree.c)
 * implementujte funkci letter_count. Výstupní strom může být značně degradovaný (až na úroveň lineárního seznamu) 
 * a tedy implementujte i druhou funkci (bst_balance), která strom, na požadavek uživatele, vybalancuje.
 * Funkce jsou na sobě nezávislé a tedy automaticky NEVOLEJTE bst_balance v letter_count.
 * 
 */

#include "../btree.h"
#include <stdio.h>
#include <stdlib.h>


/**
 * Vypočítání frekvence výskytů znaků ve vstupním řetězci.
 * 
 * Funkce inicilializuje strom a následně zjistí počet výskytů znaků a-z (case insensitive), znaku 
 * mezery ' ', a ostatních znaků (ve stromu reprezentováno znakem podtržítka '_'). Výstup je v 
 * uložen ve stromu.
 * 
 * Například pro vstupní řetězec: "abBccc_ 123 *" bude strom po běhu funkce obsahovat:
 * 
 * key | value
 * 'a'     1
 * 'b'     2
 * 'c'     3
 * ' '     2
 * '_'     5
 * 
 * Pro implementaci si můžete v tomto souboru nadefinovat vlastní pomocné funkce.
*/
#include <stdbool.h>

// moje isAplha funkce
bool isAlpha(char c) {
    if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) {
        return true;
    }
    return false;
}

// funkce pro prevod na validni znaky
char letter_transfer(char c) {
    // pokud je znak velke pismeno, preved na male
    bool alpha = isAlpha(c);
    if (alpha) {
        if ((c >= 'a' && c <= 'z')) {
            return c;
        } else {
            return (c - 'A' + 'a');
        }   
    } else if(c == 32){
        return c;           // pokud je znak mezera
    } else {
        return ('_');       // vse ostatni
    }
}

void letter_count(bst_node_t **tree, char *input) {
    int tmp_int = 1;
    char tmp_char;
    bst_init(&(*tree));
    for (size_t i = 0; input[i] != '\0'; i++)
    {
        tmp_char = letter_transfer(input[i]);
        if ((*tree) == NULL) {
            bst_insert(&(*tree), tmp_char, 1);          // prvni symbol
        } else {
            bool isInside = bst_search((*tree), tmp_char, &tmp_int);
            if (isInside)
            {
                bst_insert(&(*tree), tmp_char, ++tmp_int);
            } else {
                bst_insert(&(*tree), tmp_char, 1);
            }    
        }
    }
}

/**
 * Vyvážení stromu.
 * 
 * Vyvážený binární vyhledávací strom je takový binární strom, kde hloubka podstromů libovolného uzlu se od sebe liší maximálně o jedna.
 * 
 * Předpokládejte, že strom je alespoň inicializován. K získání uzlů stromu využijte vhodnou verzi vámi naimplmentovaného průchodu stromem.
 * Následně můžete například vytvořit nový strom, kde pořadím vkládaných prvků zajistíte vyváženost.
 *  
 * Pro implementaci si můžete v tomto souboru nadefinovat vlastní pomocné funkce. Není nutné, aby funkce fungovala *in situ* (in-place).
*/

bst_items_t* init_items(void) {
  bst_items_t* items = (bst_items_t *)malloc(sizeof(bst_items_t));
  items->capacity = 0;
  items->nodes = NULL;
  items->size = 0;
  return items;
}

void reset_items (bst_items_t *items) {
  if(items != NULL) {
    if (items->capacity > 0)
    {
      free(items->nodes);
    }
    items->capacity = 0;
    items->size = 0;
  }
}

void build_balanced_tree(bst_node_t **tree, bst_items_t *items, int left, int right) {
    if(left <= right) {
        int middle = (left + right) / 2;
        *tree = (bst_node_t *)malloc(sizeof(bst_node_t));
        if (*tree == NULL) return;
        (*tree)->key = items->nodes[middle]->key;
        (*tree)->value = items->nodes[middle]->value;
        build_balanced_tree(&(*tree)->left, items, left, middle-1);
        build_balanced_tree(&(*tree)->right, items, middle+1, right);
    } else {
        *tree = NULL;
    }
}

void bst_balance(bst_node_t **tree) {
    if (*tree == NULL) return;
    bst_node_t *tree_to_replace;
    bst_init(&tree_to_replace);                                             // inicializace noveho stromu
    bst_items_t *items = init_items();                                      // inicializace pole uzlu
    bst_inorder(*tree, items);                                              // naplneni inorder pruchodem
    build_balanced_tree(&tree_to_replace, items, 0, items->size - 1);
    bst_dispose(&(*tree));                                                  // uvolneni puvodniho stromu
    *tree = tree_to_replace;
    reset_items(items);
    free(items);
}
