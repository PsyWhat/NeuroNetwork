#pragma once
#include "RecursiveNeuroNet.h"


void ClearListElement( ListElement *list )
{
	ListElement *tmp = NULL;
	if ( list != NULL )
	{
		while ( list != NULL )
		{
			tmp = list;
			list = list->next;
			MYFREE( tmp );
		}
	}
}

void FreeListElement( ListElement *list )
{
	ListElement *tmp = NULL;
	if ( list != NULL )
	{
		while ( list != NULL )
		{
			tmp = list;
			list = list->next;
			MYFREE( tmp );
			MYFREE( tmp->element );
		}
	}
}

Node* AddNode( NeuroNet* net , int ID )
{
	ListElement *le = NULL;
	Node *n = NULL;
	if ( net != NULL )
	{
		if ( net->nodes != NULL )
		{
			n = NewNode( ID );
			le = NewListElement( n );
			if ( le == NULL )
				return NULL;
			le->next = net->nodes;
			net->nodes = le;
			net->nodesCount += 1;
		} else
		{
			n = NewNode( ID );
			net->nodes = NewListElement( n );
			if ( net->nodes == NULL )
				return NULL;
			net->nodesCount += 1;
		}
		return n;
	}
	return NULL;
}

Connection* AddConnection( NeuroNet *net , int from , int to , double weight )
{
	ListElement *le = NULL;
	Connection *con = NULL;
	Node *n1 = NULL , *n2 = NULL;
	if ( net != NULL )
	{
		if ( to >= net->inputsCount )
		{
			n1 = FindNodeByID( net , from );
			n2 = FindNodeByID( net , to );
			if ( n1 != NULL && n2 != NULL )
			{
				if ( net->connections != NULL )
				{
					con = NewConnection( n1 , n2 , weight );
					le = NewListElement( con );
					if ( le == NULL )
						return NULL;
					le->next = net->connections;
					net->connections = le;
					net->connectionsCount += 1;
				} else
				{
					con = NewConnection( n1 , n2 , weight );
					net->connections = NewListElement( con );
					if ( net->connections == NULL )
						return NULL;
					net->connectionsCount += 1;
				}
				return con;
			} else
			{
				return NULL;
			}
		}
	}
	return NULL;
}

double* Compute( NeuroNet* net )
{
	double *res = NULL;
	int i;
	ListElement *en;
	Node *n;
	if ( net != NULL )
	{
		if ( net->nodes != NULL )
		{
			en = net->nodes;
			while ( en != NULL )
			{
				n = ((Node*)(en->element));
				n->state = 0;
				en = en->next;
			}

			en = net->nodes;
			while ( en != NULL )
			{
				n = (Node *)(en->element);
				if ( n->ID >= net->inputsCount && 
					n->ID < (net->inputsCount + net->outputsCount) )
				{
					ComputeNode( net , n );
				}
				en = en->next;
			}

			res = (double *)MYALLOC( (sizeof( double ))*net->outputsCount );
			i = 0;

			en = net->outputs;

			while ( en != NULL )
			{
				n = ((Node*)(en->element));
				res[n->ID - net->inputsCount] = n->Value;
				en = en->next;
			}


			return res;
		}
	}
	return NULL;
}

void ComputeNode( NeuroNet *net , Node *n )
{
	Node *toCompute = NULL;
	Connection *c = NULL;
	ListElement *connectionsTo = NULL;
	ListElement *en = NULL;
	ListElement *le  = NULL;
	double sum = 0.0;
	if ( net != NULL )
	{
		if ( net->connections != NULL && net->nodes != NULL )
		{
			en = connectionsTo = FindConnectionsTo( net , n );

			n->state |= (1 << 0);

			en = connectionsTo;
			while ( en != NULL )
			{
				c = (Connection *)(en->element);
				toCompute = (Node*)(c->From);

				if ( toCompute->ID == c->From->ID )
				{
					if ( (toCompute->state & ((1 << 0) | (1 << 1))) || ((toCompute->ID) < (net->inputsCount)) || (toCompute->ID < (net->inputsCount + net->outputsCount)) )
					{
						sum += toCompute->Value * c->Weight;
					} else
					{
						ComputeNode( net , toCompute );
						sum += toCompute->Value * c->Weight;
					}
				}

				en = en->next;
			}
			ClearListElement( connectionsTo );

			n->Value = 1 / (1 + exp( sum ));


			n->state &= ~(1 << 0);
			n->state |= (1 << 1);
		}
	}
}

ListElement* GetConnections( NeuroNet *net )
{
	if ( net != NULL )
	{
		return net->connections;
	}
	return NULL;
}

ListElement* GetNodes( NeuroNet *net )
{
	if ( net != NULL )
	{
		return net->nodes;
	}
	return NULL;
}

int GetListLenght( ListElement *list )
{
	int res = 0;
	while ( list != NULL )
	{
		list = list->next;
		res++;
	}
	return res;
}

ListEnumerator* GetEnumerator( ListElement *list )
{
	ListEnumerator *res = (ListEnumerator*)MYALLOC( sizeof( ListEnumerator ) );
	res->first = list;
	res->current = NULL;
	return res;
}

int EnumeratorGoNext( ListEnumerator* le )
{
	if ( le != NULL && le->first != NULL )
	{
		if ( le->current == NULL )
		{
			le->current = le->first;
		} else
		{
			le->current = le->current->next;
			if ( le->current == NULL )
			{
				return 0;
			}
		}
		return 1;
	}
	return 0;
}

void* GetEnumeratorCurrent( ListEnumerator *le )
{
	if ( le != NULL && le->current != NULL )
	{
		return le->current->element;
	}
	return NULL;
}

void ResetEnumerator( ListEnumerator *le )
{
	if ( le != NULL )
	{
		le->current = NULL;
	}
}

int GetNodeID( Node *n )
{
	if ( n != NULL )
	{
		return n->ID;
	}
	return -1;
}

int GetConnectionTo( Connection *c )
{
	if ( c != NULL )
	{
		return c->To->ID;
	}
	return -1;
}

int GetConnectionFrom( Connection *c )
{
	if ( c != NULL )
	{
		return c->From->ID;
	}
	return -1;
}

Connection* SetConnectionFromTo( Connection *c , Node *from , Node *to )
{
	if ( c != NULL )
	{
		c->From = from;
		c->To = to;
	}
	return c;
}

double GetConnectionWeight( Connection *con )
{
	if ( con != NULL )
	{
		return con->Weight;
	}
	return 0.0;
}



void ChangeConnectionWeight( Connection *con , double newWeight )
{
	if ( con != NULL )
	{
		con->Weight = newWeight;
	}
}

ListElement* FindConnectionsTo( NeuroNet *net , Node *node )
{
	Connection *tmp = NULL;
	ListElement *lr = NULL;
	ListElement *lf = NULL;
	ListElement *en = NULL;
	int i;
	if ( net != NULL )
	{
		if ( net->connections != NULL )
		{
			i = 0;
			en = net->connections;

			while ( en != NULL )
			{
				tmp = (Connection*)(en->element);
				if ( tmp->To == node )
				{
					if ( lr == NULL )
					{
						lf = lr = NewListElement( tmp );//////////////////
					} else
					{
						lr->next = NewListElement( tmp );//////////////////
						lr = lr->next;
					}
				}
				en = en->next;
			}

			return lf;
		}
	}
	return NULL;
}

ListElement* FindConnectionsFrom( NeuroNet *net , int FromID )
{
	Connection *tmp = NULL;
	ListElement *lr = NULL;
	ListElement *lf = NULL;
	ListElement *en = NULL;
	int i;
	if ( net != NULL )
	{
		if ( net->connections != NULL )
		{
			i = 0;
			en = net->connections;

			while ( en != NULL )
			{
				tmp = (Connection*)(en->element);
				if ( tmp->To->ID == FromID )
				{
					if ( lr == NULL )
					{
						lf = lr = NewListElement( tmp );//////////////////
					} else
					{
						lr->next = NewListElement( tmp );//////////////////
						lr = lr->next;
					}
				}
				en = en->next;
			}

			return lf;
		}
	}
	return NULL;
}



Connection* FindConnection( NeuroNet *net , int from , int to )
{
	ListElement *el;
	Connection *con;
	if ( net != NULL && net->connections != NULL )
	{
		el = net->connections;
		while ( el != NULL )
		{
			con = ((Connection*)(el->element));
			if ( con->From->ID == from && con->To->ID == to )
			{
				return con;
			}
			el = el->next;
		}
	}
	return NULL;
}

Node * FindNodeByID( NeuroNet *net , int ID )
{
	ListElement *e;
	if ( net != NULL )
	{
		if ( net->nodes != NULL )
		{
			e = net->nodes;
			do
			{
				if ( ((Node*)(e->element))->ID == ID )
				{
					return (Node*)(e->element);
				}
				e = e->next;
			} while ( e != NULL );

		} else
		{
			return NULL;
		}

	}
	return NULL;
}

void FreeNeuroNet( NeuroNet* net )
{
	ListElement *el;
	if ( net != NULL )
	{
		while ( net->connections != NULL )
		{
			el = net->connections->next;
			MYFREE( net->connections->element );
			MYFREE( net->connections );
			net->connections = el;
		}

		while ( net->nodes != NULL )
		{
			el = net->nodes->next;
			MYFREE( net->nodes->element );
			MYFREE( net->nodes );
			net->nodes = el;
			net->nodesCount--;
		}

		while ( net->outputs != NULL )
		{
			el = net->outputs->next;
			MYFREE( net->outputs );
			net->outputs = el;
		}

		while ( net->inputs != NULL )
		{
			el = net->inputs->next;
			MYFREE( net->inputs );
			net->inputs = el;
		}
		{

	}

		MYFREE( net );
#ifdef USEMYALLOC
		NNAllocs--;
#endif // 

	}
}

Connection* NewConnection( Node *from , Node *to , double weight )
{
	Connection *res = NULL;
	res = (Connection*)MYALLOC( sizeof( Connection ) );
	if ( res != NULL )
	{
		res->From = from;
		res->To = to;
		res->Weight = weight;
	}
	return res;
}





#ifdef USEMYALLOC
#undef  MYALLOC
void* my_malloc( size_t am )
{
	MemBlk b;
	void* ref = malloc( am );
	TotalMem += am;
	b.am = am;
	b.pointer = ref;
	AddMemAlloc( b );
	Allocks++;
	return ref;
}
#define MYALLOC(p) my_malloc(p)


void AddMemAlloc( MemBlk blk )
{
	MLB *n = (MLB*)malloc( sizeof( MLB ) );
	n->next = mems;
	n->val = blk;
	mems = n;
}


size_t PullMemAllocSize( void * block )
{
	MLB *el , *tmp = NULL;
	size_t res;
	el = mems;
	while ( el != NULL )
	{
		if ( el->val.pointer == block )
		{
			res = el->val.am;
			if ( tmp == NULL )
			{
				mems = el->next;
				free( el );
			} else
			{
				tmp->next = el->next;
				free( el );
			}

			return res;

		} else
		{
			tmp = el;
		}
		el = el->next;
	}
	return 0;
}


#undef  MYFREE
void my_free( void* block )
{
	TotalMem -= PullMemAllocSize( block );
	free( block );
	Allocks--;
}
#define MYFREE(p) my_free(p)

unsigned long long GetMemoryAllocated()
{
	return TotalMem;
}

unsigned long long GetNeuroNetsAllocated()
{
	return NNAllocs;
}


#endif

ListElement* NewListElement( void *value )
{
	ListElement *res;
	if ( value != NULL )
	{
		res = NULL;
		res = (ListElement*)MYALLOC( sizeof( ListElement ) );
		if ( res != NULL )
		{
			res->element = value;
			res->next = NULL;
		}
		return res;
	} else
	{
		return NULL;
	}
}

NeuroNet* NewNeuroNet( int numInputs , int numOutputs )
{
	NeuroNet *res;
	ListElement *le, *li;
	Node *n;
	int i;

	res = (NeuroNet*)MYALLOC( sizeof( NeuroNet ) );
	res->inputsCount = numInputs;
	res->outputsCount = numOutputs;



	res->connectionsCount = 0;
	res->nodesCount = 0;
	res->nodes = NULL;
	res->connections = NULL;
	res->outputs = NULL;
	res->inputs = NULL;
	le = NULL;
	li = NULL;

	for ( i = 0; i < numInputs + numOutputs; ++i )
	{

		n = AddNode( res , i );
		if ( i < numInputs )
		{
			if ( li == NULL )
			{
				li = NewListElement( n );
				res->inputs = li;
			} else
			{
				li->next = NewListElement( n );
				li = li->next;
			}
		} else
		{
			if ( le == NULL )
			{
				le = NewListElement( n );
				res->outputs = le;

			} else
			{
				le->next = NewListElement( n );
				le = le->next;
			}
		}
	}

#ifdef USEMYALLOC
	NNAllocs++;
#endif
	return res;
}

Node* NewNode( int ID )
{
	Node *res = NULL;
	res = (Node*)MYALLOC( sizeof( Node ) );
	if ( res != NULL )
	{
		res->ID = ID;
		res->Value = 0.0;
		res->state = 0;
	}
	return res;
}

int RemoveNode( NeuroNet *net , int ID )
{
	Node *n;
	ListElement *el, *tmp = NULL,*ct, *tmpc;
	Connection *c;
	if ( net != NULL && net->nodes != NULL )
	{
		if ( ID > net->outputsCount + net->inputsCount -1)
		{
			el = net->nodes;
			while ( el != NULL )
			{
				n = (Node *)(el->element);



				if ( n->ID == ID )
				{


					ct = net->connections;
					tmpc = NULL;
					while ( ct != NULL )
					{
						c = (Connection*)(ct->element);
						if ( c->From->ID == n->ID || c->To->ID == n->ID )
						{
							if ( tmpc == NULL )
							{
								net->connections = ct->next;

							} else
							{
								tmpc->next = ct->next;
							}

							MYFREE( ct->element );
							MYFREE( ct );

							if ( tmpc == NULL )
							{
								ct = net->nodes;
							} else
							{
								ct = tmpc;
							}
						} else
						{
							tmpc = ct;
						}
						ct = ct->next;
					}


					if ( tmp != NULL )
					{
						tmp->next = el->next;
						MYFREE( el->element );
						MYFREE( el );
					} else
					{
						net->nodes = el->next;
						MYFREE( el->element );
						MYFREE( el );
					}
					net->nodesCount -= 1;

					return 1;
				}

			tmp = el;
				el = el->next;
			}
		}
	}
	return 0;
}

int RemoveConnection( NeuroNet *net , int from , int to )
{
	Connection *c;
	ListElement *el , *tmp = NULL;
	if ( net != NULL && net->connections != NULL )
	{
		el = net->connections;
		while ( el != NULL )
		{
			c = (Connection *)(el->element);

			if ( c->From->ID == from && c->To->ID == to )
			{
				if ( tmp != NULL )
				{
					tmp->next = el->next;
					MYFREE( el->element );
					MYFREE( el );
				} else
				{
					net->connections = el->next;
					MYFREE( el->element );
					MYFREE( el );
				}
				net->connectionsCount -= 1;
				return 1;
			}

				tmp = el;

			el = el->next;
		}
	}
	return 0;
}

void ClearNodes( NeuroNet *net )
{
	ListElement *el, *tmp = NULL, *ct, *tmpc;
	Node *n;
	Connection *c;
	if ( net != NULL )
	{
		el = net->nodes;
		while ( el != NULL )
		{
			n = ((Node *)(el->element));
			if ( n->ID > net->inputsCount + net->outputsCount - 1 )
			{
				ct = net->connections;
				tmpc = NULL;
				while ( ct != NULL )
				{
					c = (Connection*)(ct->element);
					if ( c->From->ID == n->ID || c->To->ID == n->ID )
					{
						if ( tmpc == NULL )
						{
							net->connections = ct->next;

						} else
						{
							tmpc->next = ct->next;
						}

						MYFREE( ct->element );
						MYFREE( ct );

						if ( tmpc == NULL )
						{
							ct = net->nodes;
						} else
						{
							ct = tmpc;
						}
					} else
					{
						tmpc = ct;
					}
					ct = ct->next;
				}




				if ( tmp == NULL )
				{
					net->nodes = el->next;
				} else
				{
					tmp->next = el->next;
				}

				MYFREE( el->element );
				MYFREE( el );

				if ( tmp == NULL )
				{
					el = net->nodes;
				} else
				{
					el = tmp;
				}

			} else
			{
				tmp = el;
			}
			el = el->next;
		}
	}
}

void ClearConnections( NeuroNet *net )
{
	ListElement *el;
	if ( net != NULL )
	{

		while ( net->connections != NULL )
		{
			el = net->connections->next;
			MYFREE( net->connections->element );
			MYFREE( net->connections );
			net->connections = el;
		}
	}
}

int GetNodesCount( NeuroNet *net )
{
	if ( net != NULL )
	{
		return net->nodesCount;
	}
	return -1;
}

int GetConnectionsCount( NeuroNet *net )
{

	if ( net != NULL )
	{
		return net->connectionsCount;
	}
	return -1;
}

void InitInputs( NeuroNet* net , double *values )
{
	ListElement *le;
	Node *n;
	if ( net != NULL && net->inputs != NULL )
	{
		le = net->inputs;
		while ( le != NULL )
		{
			n = (Node*)(le->element);

			if ( n != NULL )
			{
				n->Value = values[n->ID];
			}

			le = le->next;
		}
	}
}

MY_EXPORT void FlushNet( NeuroNet *net )
{
	ListElement *le;
	if ( net != NULL && net->nodes != NULL )
	{
		le = net->nodes;
		while ( le != NULL )
		{
			((Node*)(le->element))->Value = 0;
			le = le->next;
		}
	}
}

