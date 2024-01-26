//======== Copyright (c) 2023, FIT VUT Brno, All rights reserved. ============//
//
// Purpose:     Test Driven Development - graph
//
// $NoKeywords: $ivs_project_1 $black_box_tests.cpp
// $Author:     MAREK JOUKL <xjoukl00@stud.fit.vutbr.cz>
// $Date:       $2023-03-03
//============================================================================//
/**
 * @file tdd_code.cpp
 * @author Martin Dočekal
 * @author Karel Ondřej
 *
 * @brief Implementace metod tridy reprezentujici graf.
 */

#include "tdd_code.h"


Graph::Graph(){}

Graph::~Graph(){
    //free allocated nodes
    for (auto node : myNodes) {
        delete node;
    }
}

std::vector<Node*> Graph::nodes() {
    std::vector<Node*> nodes;
    //pushes nodes to my own vector myNodes
    for (int i = 0; i < myNodes.size(); i++) {
        nodes.push_back(myNodes[i]);
    }
    return nodes;
}

std::vector<Edge> Graph::edges() const{
    std::vector<Edge> edges;
    //pushes edges to my own vector myEdges
    for (int i = 0; i < myEdges.size(); i++)
    {
        edges.push_back(myEdges[i]);
    }
    return edges;
}

Node* Graph::addNode(size_t nodeId) {
    for (int i = 0; i < myNodes.size(); i++) {
        if (myNodes[i]->id == nodeId) {
        return nullptr;
        }
    }
    //allocates new node
    Node* newNode = new Node();
    newNode->id = nodeId;
    myNodes.push_back(newNode);
    return newNode;
}

bool Graph::addEdge(const Edge& edge){
    size_t nodeX = edge.a;
    size_t nodeY = edge.b;
    //checks for duplicities
    if (nodeX == nodeY) {
        return false;
    }
    //checks for loops
    for (size_t i = 0; i < myEdges.size(); i++) {
        if ((myEdges[i].a == nodeX && myEdges[i].b == nodeY) || (myEdges[i].a == nodeY && myEdges[i].b == nodeX)) {
            return false;
        }
    }
    myEdges.push_back(edge);
    return true;
}

void Graph::addMultipleEdges(const std::vector<Edge>& edges) {
    for (size_t i = 0; i < edges.size(); ++i) {
        const Edge edge = edges[i];
        
        if (edge.a == edge.b || containsEdge(edge)) {
            continue;
        }
        Node* nodeA = getNode(edge.a);
        if (!nodeA) {
            nodeA = addNode(edge.a);
        }
        Node* nodeB = getNode(edge.b);
        if (!nodeB) {
            nodeB = addNode(edge.b);
        }
        myEdges.push_back(edge);
    }
}

Node* Graph::getNode(size_t nodeId){
    for (int i = 0; i < myNodes.size(); i++)
    {
        if (myNodes[i]->id == nodeId) return myNodes[i];  
    }
    return nullptr;
}

bool Graph::containsEdge(const Edge& edge) const{
    size_t nodeX = edge.a;
    size_t nodeY = edge.b;
    for (int i = 0; i < myEdges.size(); i++)
    {
        if ((myEdges[i].a == nodeX && myEdges[i].b == nodeY) || (myEdges[i].a == nodeY && myEdges[i].b == nodeX))
        {
            return true;
        }
    }
    return false;
}

void Graph::removeNode(size_t nodeId){
    int nodeExists = 0;

    for (int i = 0; i < myEdges.size(); i++)
    {
        if (nodeId == myEdges[i].a || nodeId == myEdges[i].b)
        {   
            removeEdge(myEdges[i]);
            i--;
        }
    }
    for (int i = 0; i < myNodes.size(); i++)
    {
        if (myNodes[i]->id == nodeId)
        {   
            delete myNodes[i];
            myNodes.erase(myNodes.begin() + i);
            nodeExists = 1;
            break;
        }
    }
    
    if (!nodeExists){
        throw std::out_of_range("OUT_OF_RANGE: Node doesn't exist!");
    }   
}

void Graph::removeEdge(const Edge& edge){
    size_t nodeX = edge.a;
    size_t nodeY = edge.b;
    int edgeExists = 0;
    
    for (size_t i = 0; i < myEdges.size(); i++) {
        if ((myEdges[i].a == nodeX && myEdges[i].b == nodeY) || (myEdges[i].a == nodeY && myEdges[i].b == nodeX)) {
            myEdges.erase(myEdges.begin() + i);
            edgeExists = 1;
            break;
        }
    }
    if (!edgeExists){
        throw std::out_of_range("OUT_OF_RANGE: Edge doesn't exist!");
    }
}

size_t Graph::nodeCount() const{
    return myNodes.size();
}

size_t Graph::edgeCount() const{
    return myEdges.size();
}

size_t Graph::nodeDegree(size_t nodeId) const{
    size_t count = 0;
    int nodeExists = 0;

    for (int i = 0; i < myEdges.size(); i++){
        if((myEdges[i].a == nodeId) || (myEdges[i].b == nodeId)) {
        count++;
        nodeExists = 1;
        }
    }
    if (nodeExists) {return count;}
    else  {throw std::out_of_range("OUT_OF_RANGE: Node doesn't exist!");}
}

size_t Graph::graphDegree() const{
    size_t maxNodeDegree = 0;
    for (int i = 0; i < myNodes.size(); i++)
    {
        size_t currentNodeDegree = nodeDegree(myNodes[i]->id);
        if (maxNodeDegree < currentNodeDegree)
        {
            maxNodeDegree = currentNodeDegree;
        }
    }
    return maxNodeDegree;
}

void Graph::coloring(){
    for (int i = 0; i < myNodes.size(); i++)
    {
        myNodes[i]->color = i+1;
        for (int j = 0; j < myEdges.size(); j++)
        {
            if (myEdges[j].a == myNodes[i]->id)
            {
                Node* tmpNode = getNode(myEdges[j].a);
                tmpNode->color = i+2;
                break;
            }else if (myEdges[j].b == myNodes[i]->id)
            {
                Node* tmpNode = getNode(myEdges[j].b);
                tmpNode->color = i+2;
                break;
            }else {
                break;
            }
        }
    }
}

void Graph::clear() {
    for (int i = 0; i < myNodes.size(); i++)
    {
        delete myNodes[i];
    }
    myNodes.clear();
    myEdges.clear();
}

/*** Konec souboru tdd_code.cpp ***/
