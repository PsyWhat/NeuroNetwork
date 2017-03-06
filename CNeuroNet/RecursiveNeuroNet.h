#pragma once
#include <math.h>
#include <stdlib.h>
#include <stdio.h>

#define MY_EXPORT __declspec(dllexport)

#define MYALLOC(p) malloc(p)
#define MYFREE(p) free(p)


#ifdef USEMYALLOC

struct MemBlk
{
	void* pointer;
	size_t am;
};

struct MLB
{
	MemBlk val;
	MLB *next;
};

MLB *mems = NULL;

void AddMemAlloc( MemBlk blk );

size_t PullMemAllocSize( void * block );

extern "C"
{

	MY_EXPORT unsigned long long TotalMem = 0;
	MY_EXPORT unsigned NNAllocs = 0;
	MY_EXPORT unsigned Allocks = 0;
	MY_EXPORT unsigned long long GetMemoryAllocated();
	MY_EXPORT unsigned long long GetNeuroNetsAllocated();
}

void* my_malloc( size_t am );

void my_free( void* block );

#endif






MY_EXPORT struct Node
{
	int ID;
	double Value;
	unsigned char state;
};

MY_EXPORT struct Connection
{
	Node *From;
	Node *To;
	double Weight;
};

MY_EXPORT struct ListElement
{
	ListElement *next;
	void *element;
};

MY_EXPORT struct NeuroNet
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

MY_EXPORT struct ListEnumerator
{
	ListElement *first;
	ListElement *current;

};






ListElement* NewListElement( void *value );

void ClearListElement( ListElement *list );

Connection* NewConnection( Node *from , Node *to , double weight );

Node* NewNode( int ID );

ListElement* FindConnectionsTo( NeuroNet *net , Node *ToID );

void ComputeNode( NeuroNet *net , Node *n );

/*#define malloc(am) my_malloc(am)
#define free(val) my_free(val)*/

extern "C"
{

	MY_EXPORT ListElement* GetConnections( NeuroNet *net );
	MY_EXPORT ListElement* GetNodes( NeuroNet *net );

	MY_EXPORT int GetNodesCount( NeuroNet *net );
	MY_EXPORT int GetConnectionsCount( NeuroNet *net );

	MY_EXPORT int GetListLenght( ListElement *list );
	MY_EXPORT ListEnumerator* GetEnumerator( ListElement *list );
	MY_EXPORT int EnumeratorGoNext( ListEnumerator* le );
	MY_EXPORT void* GetEnumeratorCurrent( ListEnumerator *le );
	MY_EXPORT void ResetEnumerator( ListEnumerator *le );

	MY_EXPORT int GetNodeID( Node *n );


	MY_EXPORT int GetConnectionTo( Connection *c );

	MY_EXPORT int GetConnectionFrom( Connection *c );

	MY_EXPORT Connection* SetConnectionFromTo( Connection *c , Node *from , Node *to );


	MY_EXPORT double GetConnectionWeight( Connection *con );
	MY_EXPORT void ChangeConnectionWeight( Connection *con , double newWeight );

	MY_EXPORT Node * FindNodeByID( NeuroNet *net , int ID );
	MY_EXPORT Connection* FindConnection( NeuroNet *net , int from , int to );


	MY_EXPORT int RemoveNode( NeuroNet *net , int ID );
	MY_EXPORT int RemoveConnection( NeuroNet *net , int from , int to );


	MY_EXPORT void ClearNodes( NeuroNet *net );

	MY_EXPORT void ClearConnections( NeuroNet *net );

	MY_EXPORT void FreeNeuroNet( NeuroNet* net );

	MY_EXPORT void InitInputs( NeuroNet* net, double *values);

	MY_EXPORT double* Compute( NeuroNet* net );

	MY_EXPORT void FlushNet( NeuroNet *net );

	MY_EXPORT Node* AddNode( NeuroNet* net , int ID );
	MY_EXPORT Connection* AddConnection( NeuroNet *net , int from , int to , double weight );
	MY_EXPORT NeuroNet* NewNeuroNet( int numInputs , int numOutputs );


}