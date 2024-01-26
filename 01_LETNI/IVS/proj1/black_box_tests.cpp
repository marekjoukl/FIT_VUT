//======== Copyright (c) 2017, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     Red-Black Tree - public interface tests
//
// $NoKeywords: $ivs_project_1 $black_box_tests.cpp
// $Author:     MAREK JOUKL <xjoukl00@stud.fit.vutbr.cz>
// $Date:       $2023-03-03
//============================================================================//
/**
 * @file black_box_tests.cpp
 * @author MAREK JOUKL
 * 
 * @brief Implementace testu binarniho stromu.
 */

#include <vector>

#include "gtest/gtest.h"

#include "red_black_tree.h"

//============================================================================//
// ** ZDE DOPLNTE TESTY **
//
// Zde doplnte testy Red-Black Tree, testujte nasledujici:
// 1. Verejne rozhrani stromu
//    - InsertNode/DeleteNode a FindNode
//    - Chovani techto metod testuje pro prazdny i neprazdny strom.
// 2. Axiomy (tedy vzdy platne vlastnosti) Red-Black Tree:
//    - Vsechny listove uzly stromu jsou *VZDY* cerne.
//    - Kazdy cerveny uzel muze mit *POUZE* cerne potomky.
//    - Vsechny cesty od kazdeho listoveho uzlu ke koreni stromu obsahuji
//      *STEJNY* pocet cernych uzlu.
//============================================================================//

class EmptyTree : public ::testing::Test{

protected:
    BinaryTree tree;
};

class NonEmptyTree : public ::testing::Test{

protected:

    void SetUp() {
        int values[] = {2, 1, 5, 7, 3, 6, 9, 3};
        for (auto value: values)
        {
            tree.InsertNode(value);
        }   
    }
    BinaryTree tree;
};
class TreeAxioms : public ::testing::Test{

protected:

    void SetUp() {
        int values[] = {2, 1, 5, 7, 3, 6, 9, 3};
        for (auto value: values)
        {
            tree.InsertNode(value);
        }   
    }
    BinaryTree tree;
};

//====================EmptyTree====================//

TEST_F(EmptyTree, InsertNode){
    auto res = tree.InsertNode(4);

    EXPECT_TRUE(res.first);
    EXPECT_EQ(res.second->key, 4);

    auto res2 = tree.InsertNode(4);
    EXPECT_FALSE(res2.first);
    EXPECT_EQ(res.second->key, res2.second->key);
};

TEST_F(EmptyTree, DeleteNode){
    auto res = tree.DeleteNode(2);

    EXPECT_FALSE(res);
};
TEST_F(EmptyTree, FindNode){
    auto res = tree.FindNode(2);

    EXPECT_EQ(res, nullptr);
};

//===================NonEmptyTree===================//

TEST_F(NonEmptyTree, InsertNode){
    auto res = tree.InsertNode(4);

    EXPECT_TRUE(res.first);
    EXPECT_EQ(res.second->key, 4);

    auto res2 = tree.InsertNode(4);
    EXPECT_FALSE(res2.first);
    EXPECT_EQ(res2.second->key, res.second->key);

};
TEST_F(NonEmptyTree, DeleteNode){
    auto res = tree.DeleteNode(2);
    
    EXPECT_TRUE(res);
    EXPECT_EQ(tree.FindNode(2), nullptr);

    auto res2 = tree.DeleteNode(20);
    EXPECT_FALSE(res2);
};
TEST_F(NonEmptyTree, FindNode){
    auto testNode = tree.InsertNode(11);
    auto res = tree.FindNode(11);
    
    EXPECT_EQ(res, testNode.second);

    auto res2 = tree.FindNode(20);
    EXPECT_EQ(res2, nullptr);
};

//====================Axioms====================//

TEST_F(TreeAxioms, Axiom1){
    std::vector<Node_t *>leafNodes;
    tree.GetLeafNodes(leafNodes);
    
    for (int i = 0; i < leafNodes.size(); i++)
    {
        EXPECT_EQ(leafNodes[i]->color, BLACK);
    }
    
}
TEST_F(TreeAxioms, Axiom2){
    std::vector<Node_t *>nonLeafNodes;
    tree.GetNonLeafNodes(nonLeafNodes);

    for (int i = 0; i < nonLeafNodes.size(); i++)
    {
        if (nonLeafNodes[i]->color == RED)
        {
            EXPECT_EQ(nonLeafNodes[i]->pLeft->color, BLACK);
            EXPECT_EQ(nonLeafNodes[i]->pRight->color, BLACK);
        }   
    }   
}
TEST_F(TreeAxioms, Axiom3){
    std::vector<Node_t*> leafNodes;
    tree.GetLeafNodes(leafNodes);
    std::vector<int>blackNodes;

    for (auto node : leafNodes) {
        int blackCount = 0;
        for (auto temp = node; temp != tree.GetRoot(); temp = temp->pParent){
            if (temp->color != RED) blackCount++;
        }
        blackNodes.push_back(blackCount);
    }
    
    //check if all elements in blackNodes vector are the same value
    for (int i = 0; i < blackNodes.size(); i++)
    {
        for (int j = i + 1; j < blackNodes.size(); j++)
        {
            EXPECT_EQ(blackNodes[i], blackNodes[j]);
        }
    }
}

/*** Konec souboru black_box_tests.cpp ***/
