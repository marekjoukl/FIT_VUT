//======== Copyright (c) 2022, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     White Box - test suite
//
// $NoKeywords: $ivs_project_1 $black_box_tests.cpp
// $Author:     MAREK JOUKL <xjoukl00@stud.fit.vutbr.cz>
// $Date:       $2023-03-03
//============================================================================//
/**
 * @file white_box_tests.cpp
 * @author MAREK JOUKL
 * 
 * @brief Implementace testu hasovaci tabulky.
 */

#include <vector>

#include "gtest/gtest.h"

#include "white_box_code.h"

//============================================================================//
// ** ZDE DOPLNTE TESTY **
//
// Zde doplnte testy hasovaci tabulky, testujte nasledujici:
// 1. Verejne rozhrani hasovaci tabulky
//     - Vsechny funkce z white_box_code.h
//     - Chovani techto metod testuje pro prazdnou i neprazdnou tabulku.
// 2. Chovani tabulky v hranicnich pripadech
//     - Otestujte chovani pri kolizich ruznych klicu se stejnym hashem 
//     - Otestujte chovani pri kolizich hashu namapovane na stejne misto v 
//       indexu
//============================================================================//
class EmptyHashMap : public ::testing::Test{

protected:
    hash_map_t* map;
    void SetUp(){
        map = hash_map_ctor();
    }
    void TearDown(){
        hash_map_dtor(map);
    }
};

class NonEmptyHashMap : public ::testing::Test {

protected:
    hash_map_t* map;
    void SetUp(){
        map = hash_map_ctor();
        hash_map_state_code_t error1 = hash_map_put(map, "cat", 2);
        hash_map_state_code_t error2 = hash_map_put(map, "dog", 4);
        hash_map_state_code_t error3 = hash_map_put(map, "snake", 6);
    }
    void TearDown(){
        hash_map_dtor(map);
    }
};

//==================NonEmptyHashMap====================//

TEST_F(NonEmptyHashMap, MapConstructor) {
    EXPECT_EQ(map->allocated, HASH_MAP_INIT_SIZE);
    EXPECT_EQ(map->used, 3);
    EXPECT_EQ(map->first->value, 2);
    EXPECT_EQ(map->first->prev, nullptr);
};

TEST_F(NonEmptyHashMap, MapReserve) {
    hash_map_state_code_t error1 = hash_map_reserve(map, 16);
    hash_map_state_code_t error2 = hash_map_reserve(map, -5);
    hash_map_state_code_t error3 = hash_map_reserve(map, 0);

    EXPECT_EQ(map->allocated, 16);
    EXPECT_EQ(error1, OK);
    EXPECT_EQ(error2, VALUE_ERROR); //!!! RETURNS MEMORY_ERROR INSTEAD OF VALUE_ERROR !!!
    EXPECT_EQ(error3, VALUE_ERROR); 
};

TEST_F(NonEmptyHashMap, MapSize) {
    EXPECT_EQ(map->used, hash_map_size(map));
};

TEST_F(NonEmptyHashMap, MapCapacity) {
    EXPECT_EQ(map->allocated, hash_map_capacity(map));
};
TEST_F(NonEmptyHashMap, MapContains){
    EXPECT_EQ(hash_map_contains(map, "cat"), true);
    EXPECT_EQ(hash_map_contains(map, "dog"), true);
    EXPECT_EQ(hash_map_contains(map, "failureKey"), false);
};

TEST_F(NonEmptyHashMap, MapPut) {
    const char* key = "key";
    hash_map_state_code_t code1 = hash_map_put(map, key, 5);
    hash_map_state_code_t code2 = hash_map_put(map, key, 1);
    hash_map_state_code_t code3 = hash_map_put(map, "elephant", 7);
    
    EXPECT_EQ(hash_map_contains(map,key), true);
    EXPECT_EQ(code1, OK);
    EXPECT_EQ(code2, KEY_ALREADY_EXISTS);
    EXPECT_EQ(code3, OK);
    
    EXPECT_EQ(map->used, 5);
    EXPECT_EQ(map->allocated, 16); // !!! Map index should be allocated to 16 as 5 elements are in 2/3 of 8 !!!

    hash_map_state_code_t code4 = hash_map_put(map, "rabbit", 8);
    EXPECT_EQ(code4, OK);
    EXPECT_EQ(map->used, 6);
    EXPECT_EQ(map->allocated, 16); 
};
TEST_F(NonEmptyHashMap, MapGet) {
    int value1, value2, value3;
    hash_map_state_code_t code1 = hash_map_get(map, "cat", &value1);
    hash_map_state_code_t code2 = hash_map_get(map, "dog", &value2);
    hash_map_state_code_t code3 = hash_map_get(map, "xxx", &value3);

    EXPECT_EQ(value1, 2);
    EXPECT_EQ(value2, 4);
    EXPECT_EQ(code1, OK);
    EXPECT_EQ(code2, OK);
    EXPECT_EQ(code3, KEY_ERROR);
}
TEST_F(NonEmptyHashMap, MapPop) {
    const char* key = "key";
    int value1, value2, value3;
    hash_map_state_code_t code1 = hash_map_pop(map, key, &value1);
    hash_map_state_code_t code2 = hash_map_pop(map, "cat", &value2);
    hash_map_state_code_t code3 = hash_map_pop(map, "snake", &value3);

    EXPECT_EQ(code1, KEY_ERROR);
    EXPECT_EQ(code2, OK);
    EXPECT_EQ(value2, 2);
    EXPECT_EQ(code2, OK);
    EXPECT_EQ(value3, 6);
};
TEST_F(NonEmptyHashMap, MapRemove) {
    hash_map_state_code_t code1 = hash_map_remove(map,"cat");
    hash_map_state_code_t code2 = hash_map_remove(map,"failureKey");

    EXPECT_EQ(hash_map_contains(map,"cat"), false);
    EXPECT_EQ(code1, OK);
    EXPECT_EQ(code2, KEY_ERROR);
};

//====================EmptyHashMap=====================//

TEST_F(EmptyHashMap, MapConstructor) {
    EXPECT_EQ(map->allocated, HASH_MAP_INIT_SIZE);
    EXPECT_EQ(map->used, 0);
    EXPECT_EQ(map->first, nullptr);
    EXPECT_EQ(map->last, nullptr);
};

TEST_F(EmptyHashMap, MapReserve) {
    hash_map_state_code_t error1 = hash_map_reserve(map, 16);
    EXPECT_EQ(map->allocated, 16);
    EXPECT_EQ(error1, OK);
    
    hash_map_state_code_t error2 = hash_map_reserve(map, -5);
    EXPECT_EQ(map->allocated, 16);
    EXPECT_EQ(error2, VALUE_ERROR); //!!! RETURNS MEMORY_ERROR INSTEAD OF VALUE_ERROR !!!

    hash_map_state_code_t error3 = hash_map_reserve(map, 16);
    EXPECT_EQ(map->allocated, 16);
    EXPECT_EQ(error3, OK);

    hash_map_state_code_t error4 = hash_map_reserve(map, 0);
    EXPECT_EQ(map->allocated, 16); // Shouldn't execute if new value is lower than index
    EXPECT_EQ(error4, VALUE_ERROR); //Should return VALUE_ERROR
};

TEST_F(EmptyHashMap, MapSize) {
    EXPECT_EQ(map->used, hash_map_size(map));
};

TEST_F(EmptyHashMap, MapCapacity) {
    EXPECT_EQ(map->allocated, hash_map_capacity(map));
};

TEST_F(EmptyHashMap, MapContains){
    EXPECT_EQ(hash_map_contains(map, "failureKey"), false);
};

TEST_F(EmptyHashMap, MapPut) {
    const char* key = "key";
    hash_map_state_code_t code1 = hash_map_put(map, key, 5);
    hash_map_state_code_t code2 = hash_map_put(map, key, 1);
    hash_map_state_code_t code3 = hash_map_put(map, "crocodile", 2);
    hash_map_state_code_t code4 = hash_map_put(map, "zebra", 3);
    hash_map_state_code_t code5 = hash_map_put(map, "fish", 4);
    hash_map_state_code_t code6 = hash_map_put(map, "shark", 5);
    

    EXPECT_EQ(code1, OK);
    EXPECT_EQ(code2, KEY_ALREADY_EXISTS);
    EXPECT_EQ(hash_map_contains(map,key), true);
    EXPECT_EQ(code3, OK);
    EXPECT_EQ(code4, OK);
    EXPECT_EQ(code5, OK);
    EXPECT_EQ(code6, OK);

    EXPECT_EQ(map->used, 5);
    EXPECT_EQ(map->allocated, 16); // !!! Map index should be allocated to 16 as 5 elements are in 2/3 of 8 !!!

    hash_map_state_code_t code7 = hash_map_put(map, "lion", 5);
    EXPECT_EQ(code7, OK);
};

TEST_F(EmptyHashMap, MapKeyFunctions) {
    const char* key = "key";
    int value;
    hash_map_state_code_t code1 = hash_map_get(map, key, &value);
    hash_map_state_code_t code2 = hash_map_pop(map, key, &value);
    hash_map_state_code_t code3 = hash_map_remove(map, key);
    
    EXPECT_EQ(code1, KEY_ERROR);
    EXPECT_EQ(code2, KEY_ERROR);
    EXPECT_EQ(code3, KEY_ERROR);
};

/*** Konec souboru white_box_tests.cpp ***/
