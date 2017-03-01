#pragma once
#include <math.h>
#include <stdlib.h>



#define _OUT 

	struct Connection
	{
		int From;
		int To;
		double Weight;
	};

	struct Node
	{
		int ID;
		double Value;
		unsigned char state;
	};

	struct ListElement
	{
		ListElement *next;
		void *element;
	};

	 struct NeuroNet
	{
		ListElement *nodes;
		ListElement *connections;
		ListElement *outputs;
		int nodesCount;
		int connectionsCount;
		int inputsCount;
		int outputsCount;
	};

	 struct ListEnumerator
	 {

	 };


	ListElement* NewListElement( void *value );

	void FreeListElement( ListElement *list );

	Connection* NewConnection( int from , int to , double weight );

	Node* NewNode( int ID );

	ListElement* FindConnectionsTo( NeuroNet *net , int ToID );

	void ComputeNode( NeuroNet *net , Node *n );



	__declspec(dllexport) ListEnumerator* GetListEnumerator( ListElement* le );
	__declspec(dllexport) void FreeListEnumerator( ListEnumerator *lee );

	__declspec(dllexport) double GetConnectionWeight( Connection *con );
	__declspec(dllexport) void ChangeConnectionWeight( Connection *con , double newWeight );

	__declspec(dllexport) Node * FindNodeByID( NeuroNet *net , int ID );
	__declspec(dllexport) Connection* FindConnection( NeuroNet *net , int from , int to );


	__declspec(dllexport) int RemoveNode(NeuroNet *net, int ID);
	__declspec(dllexport) int RemoveConnection( NeuroNet *net , int from , int to );

	

	__declspec(dllexport) void FreeNeuroNet( NeuroNet* net );
	__declspec(dllexport) double* Compute( NeuroNet* net );
	__declspec(dllexport) Node* AddNode( NeuroNet* net , int ID );
	__declspec(dllexport) Connection* AddConnection( NeuroNet *net , int from , int to , double weight );
	__declspec(dllexport) NeuroNet* NewNeuroNet( int numInputs , int numOutputs );


