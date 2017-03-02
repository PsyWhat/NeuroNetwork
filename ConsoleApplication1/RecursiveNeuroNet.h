#pragma once
#include <math.h>
#include <stdlib.h>
#include <stdio.h>



#define MY_IMPORT __declspec(dllimport)





extern "C"
{
	MY_IMPORT struct Connection
	{
		int From;
		int To;
		double Weight;
	};

	MY_IMPORT struct Node
	{
		int ID;
		double Value;
		unsigned char state;
	};

	MY_IMPORT struct ListElement
	{
		ListElement *next;
		void *element;
	};

	MY_IMPORT struct NeuroNet
	{
		ListElement *nodes;
		ListElement *connections;
		ListElement *outputs;
		ListElement *inputs;
		int nodesCount;
		int connectionsCount;
		int inputsCount;
		int outputsCount;
	};

	MY_IMPORT struct ListEnumerator
	{
		ListElement *first;
		ListElement *current;

	};
}


extern "C"
{
	MY_IMPORT unsigned long long GetMemoryAllocated();
	MY_IMPORT unsigned long long GetNeuroNetsAllocated();
}

/*#define malloc(am) my_malloc(am)
#define free(val) my_free(val)*/

extern "C"
{

	MY_IMPORT ListElement* GetConnections( NeuroNet *net );
	MY_IMPORT ListElement* GetNodes( NeuroNet *net );

	MY_IMPORT int GetListLenght( ListElement *list );
	MY_IMPORT ListEnumerator* GetEnumerator( ListElement *list );
	MY_IMPORT int EnumeratorGoNext( ListEnumerator* le );
	MY_IMPORT void* GetEnumeratorCurrent( ListEnumerator *le );
	MY_IMPORT void ResetEnumerator( ListEnumerator *le );

	MY_IMPORT int GetNodeID( Node *n );


	MY_IMPORT int GetConnectionTo( Connection *c );

	MY_IMPORT int GetConnectionFrom( Connection *c );

	MY_IMPORT Connection* SetConnectionFromTo( Connection *c , int from , int to );


	MY_IMPORT double GetConnectionWeight( Connection *con );
	MY_IMPORT void ChangeConnectionWeight( Connection *con , double newWeight );

	MY_IMPORT Node * FindNodeByID( NeuroNet *net , int ID );
	MY_IMPORT Connection* FindConnection( NeuroNet *net , int from , int to );


	MY_IMPORT int RemoveNode( NeuroNet *net , int ID );
	MY_IMPORT int RemoveConnection( NeuroNet *net , int from , int to );


	MY_IMPORT void ClearNodes( NeuroNet *net );

	MY_IMPORT void ClearConnections( NeuroNet *net );

	MY_IMPORT void FreeNeuroNet( NeuroNet* net );
	MY_IMPORT double* Compute( NeuroNet* net );
	MY_IMPORT Node* AddNode( NeuroNet* net , int ID );
	MY_IMPORT Connection* AddConnection( NeuroNet *net , int from , int to , double weight );
	MY_IMPORT NeuroNet* NewNeuroNet( int numInputs , int numOutputs );


}