/*
 * Tabulka s rozptýlenými položkami
 *
 * S využitím datových typů ze souboru hashtable.h a připravených koster
 * funkcí implementujte tabulku s rozptýlenými položkami s explicitně
 * zretězenými synonymy.
 *
 * Při implementaci uvažujte velikost tabulky HT_SIZE.
 */

#include "hashtable.h"
#include <stdlib.h>
#include <string.h>

int HT_SIZE = MAX_HT_SIZE;

/*
 * Rozptylovací funkce která přidělí zadanému klíči index z intervalu
 * <0,HT_SIZE-1>. Ideální rozptylovací funkce by měla rozprostírat klíče
 * rovnoměrně po všech indexech. Zamyslete sa nad kvalitou zvolené funkce.
 */
int get_hash(char *key) {
  int result = 1;
  int length = strlen(key);
  for (int i = 0; i < length; i++) {
    result += key[i];
  }
  return (result % HT_SIZE);
}

/*
 * Inicializace tabulky — zavolá sa před prvním použitím tabulky.
 */
void ht_init(ht_table_t *table) {
  for (int i = 0; i < HT_SIZE; i++)
  {
    (*table)[i] = NULL;
  }
}

/*
 * Vyhledání prvku v tabulce.
 *
 * V případě úspěchu vrací ukazatel na nalezený prvek; v opačném případě vrací
 * hodnotu NULL.
 */
ht_item_t *ht_search(ht_table_t *table, char *key) {
  if (table == NULL || key == NULL) return NULL;
  int index = get_hash(key);
  ht_item_t *tmp = (*table)[index];
  while (tmp != NULL) {
    if (strcmp(tmp->key, key) == 0)
    {
      return tmp;
    } else {
      tmp = tmp->next;
    }
  }
  return NULL;
}

/*
 * Vložení nového prvku do tabulky.
 *
 * Pokud prvek s daným klíčem už v tabulce existuje, nahraďte jeho hodnotu.
 *
 * Při implementaci využijte funkci ht_search. Pri vkládání prvku do seznamu
 * synonym zvolte nejefektivnější možnost a vložte prvek na začátek seznamu.
 */
void ht_insert(ht_table_t *table, char *key, float value) {
  int index = get_hash(key);
  ht_item_t *tmp = ht_search(table, key);
  if (tmp == NULL)
  {
    // prvek neexistuje -> vytvorit novy
    ht_item_t *item = (ht_item_t *)malloc(sizeof(ht_item_t));
    item->key = key;
    item->value = value;
    item->next = (*table)[index];
    (*table)[index] = item;
  } else {
    // prven existuje -> nahradit hodnotu
    tmp->value = value;
  }
}

/*
 * Získání hodnoty z tabulky.
 *
 * V případě úspěchu vrací funkce ukazatel na hodnotu prvku, v opačném
 * případě hodnotu NULL.
 *
 * Při implementaci využijte funkci ht_search.
 */
float *ht_get(ht_table_t *table, char *key) {
  ht_item_t *tmp = ht_search(table, key);
  int index = get_hash(key);
  if (tmp->value == (*table)[index]->value)
  {
    return &tmp->value;
  }
  return NULL;
}

/*
 * Smazání prvku z tabulky.
 *
 * Funkce korektně uvolní všechny alokované zdroje přiřazené k danému prvku.
 * Pokud prvek neexistuje, funkce nedělá nic.
 *
 * Při implementaci NEPOUŽÍVEJTE funkci ht_search.
 */
void ht_delete(ht_table_t *table, char *key) {
  int index = get_hash(key);
  ht_item_t *tmp = (*table)[index];
  ht_item_t *prev = NULL;
  if (tmp == NULL) {
    // zadne prvky
    return;
  } else { 
    while (tmp != NULL) {
      if (strcmp(tmp->key, key) == 0) {
        if (prev != NULL) {
          prev->next = tmp->next;
        } else {
          (*table)[index] = tmp->next;
        }
        free(tmp);
        return;
      }
      prev = tmp;
      tmp = tmp->next;
    }
  } 
}

/*
 * Smazání všech prvků z tabulky.
 *
 * Funkce korektně uvolní všechny alokované zdroje a uvede tabulku do stavu po 
 * inicializaci.
 */
void ht_delete_all(ht_table_t *table) {
  // iterace pres tabulku
  for (int i = 0; i < HT_SIZE; i++){
    // iterace pres synonyma
    for (ht_item_t *tmp = (*table)[i]; tmp != NULL;) {
      ht_item_t *next = tmp->next;
      free(tmp);
      tmp = next;
    }
    (*table)[i] = NULL;
  }
}
