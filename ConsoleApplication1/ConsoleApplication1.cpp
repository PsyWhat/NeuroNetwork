// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

int main()
{
	srand( 647134123 );
	NeuroNet *simple = NewNeuroNet( 50 , 50 );

	NeuroNet *n = NewNeuroNet( 12 , 12 );
	for ( int i = 0; i < 70; ++i )
	{
		AddNode( n , (n->inputsCount) + (n->outputsCount) + i );
	}
	for ( int j = 0; j < 1000000; ++j )
	{
		AddConnection( n , rand() % n->nodesCount , rand() % n->nodesCount , ((rand() /(double )RAND_MAX) - 0.5)*4);
	}

	double *res = Compute( n );
	for ( int i = 0; i < n->outputsCount; ++i )
	{
		printf( "%f\n" , res[i] );
	}
    return 0;
}

