#include "RecursiveNeuroNet.h"



void FreeListElement( ListElement *list )
{
	ListElement *tmp = NULL;
	if ( list != NULL )
	{
		while ( list != NULL )
		{
			tmp = list;
			list = list->next;
			free( tmp );
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
	if ( net != NULL )
	{
		if ( to > net->inputsCount )
		{

			if ( net->connections != NULL )
			{
				con = NewConnection( from , to , weight );
				le = NewListElement( con );
				if ( le == NULL )
					return NULL;
				le->next = net->connections;
				net->connections = le;
				net->connectionsCount += 1;
			} else
			{
				con = NewConnection( from , to , weight );
				net->connections = NewListElement( con );
				if ( net->connections == NULL )
					return NULL;
				net->connectionsCount += 1;
			}
			return con;
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
				((Node*)(en->element))->state = 0;
				en = en->next;
			}

			en = net->nodes;
			while ( en != NULL )
			{
				n = (Node *)(en->element);
				if ( n->ID >= net->inputsCount
					&& n->ID < net->inputsCount + net->outputsCount )
				{
					ComputeNode( net , n );
				}
			}

			res = (double *)malloc( (sizeof( double ))*net->outputsCount );
			i = 0;
			en = net->outputs;

			while ( en != NULL )
			{
				res[i++] = ((Node*)(en->element))->Value;
			}


			return res;
		}
	}
	return NULL;
}

void ComputeNode( NeuroNet *net , Node *n )
{
	Node *toCompute = NULL;
	ListElement *connectionsTo = NULL;
	ListElement *en = NULL;
	double sum = 0.0;
	if ( net != NULL )
	{
		if ( net->connections != NULL && net->nodes != NULL )
		{
			en = connectionsTo = FindConnectionsTo( net , n->ID );

			n->state |= (1 << 0);

			while ( en != NULL )
			{
				toCompute = (Node *)(en->element);

				if ( toCompute->state & ((1 << 0) | (1 << 1)) )
				{
					sum += toCompute->Value;
				} else
				{
					ComputeNode( net , toCompute );
					sum += toCompute->Value;
				}

				en = en->next;
			}

			n->Value = 1 / (1 + exp( sum ));


			n->state &= ~(1 << 0);
			n->state |= (1 << 1);
		}
	}
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

ListElement* FindConnectionsTo( NeuroNet *net , int ToID )
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
				if ( tmp->To == ToID )
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
			if ( con->From == from && con->To == to )
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
}

void FreeNeuroNet( NeuroNet* net )
{
	ListElement *el;
	if ( net != NULL )
	{
		while ( net->connections != NULL )
		{
			el = net->connections->next;
			free( net->connections->element );
			free( net->connections );
			net->connections = el;
		}

		while ( net->nodes != NULL )
		{
			el = net->nodes->next;
			free( net->nodes->element );
			free( net->nodes );
			net->nodes = el;
		}

		while ( net->nodes != NULL )
		{
			el = net->outputs->next;
			free( net->nodes );
			net->outputs = el;
		}
	}
}

Connection* NewConnection( int from , int to , double weight )
{
	Connection *res = NULL;
	res = (Connection*)malloc( sizeof( Connection ) );
	if ( res != NULL )
	{
		res->From = from;
		res->To = to;
		res->Weight = weight;
	}
	return res;
}

ListElement* NewListElement( void *value )
{
	if ( value != NULL )
	{
		ListElement *res = NULL;
		res = (ListElement*)malloc( sizeof( ListElement ) );
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
	ListElement *le;
	Node *n;
	int i;

	res = (NeuroNet*)malloc( sizeof( NeuroNet ) );
	res->inputsCount = numInputs;
	res->outputsCount = numOutputs;



	res->connectionsCount = 0;
	res->nodesCount = 0;
	res->nodes = NULL;
	res->connections = NULL;
	res->outputs = NULL;
	le = NULL;

	for ( i = 0; i < numInputs + numOutputs - 1; ++i )
	{

		n = AddNode( res , i );
		if ( i >= numInputs )
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


	return res;
}

Node* NewNode( int ID )
{
	Node *res = NULL;
	res = (Node*)malloc( sizeof( Node ) );
	if ( res != NULL )
	{
		res->ID = ID;
		res->Value = 0.0;
	}
	return res;
}

int RemoveNode( NeuroNet *net , int ID )
{
	Node *n;
	ListElement *el, *tmp = NULL;
	if ( net != NULL && net->nodes != NULL )
	{
		el = net->nodes;
		while ( el != NULL )
		{
			n = (Node *)(el->element);
			if ( n->ID == ID )
			{
				if ( tmp != NULL )
				{
					tmp->next = el->next;
					free( el->element );
					free( el );				
				} else
				{
					net->nodes = el->next;
					free( el->element );
					free( el );
				}
				net->nodesCount -= 1;
				return 1;
			}
			tmp = el;
			el = el->next;
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

			if ( c->From == from && c->To == to )
			{
				if ( tmp != NULL )
				{
					tmp->next = el->next;
					free( el->element );
					free( el );
				} else
				{
					net->connections = el->next;
					free( el->element );
					free( el );
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
