using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionNetwork.Teacher
{
    public delegate double TestFunctionDelegate<GT>(GT genome);
    /*where GT : IGenome<GT>;*/


    public interface IGeneticTeacher<GT>
        where GT : IGenome<GT>
    {

        TestFunctionDelegate<IGenome<GT>> TestFunction
        {
            get;
            set;
        }
       

        /// <summary>
        /// Current population of the genome.
        /// </summary>
        IEnumerable<IGenome<GT>> CurrentPopulation
        {
            get;
        }

        /// <summary>
        /// Pass the tests and save the results.
        /// </summary>
        SortedList<double, IGenome<GT>> PassTests();

        /// <summary>
        /// Passing generation, according to the results.
        /// </summary>
        List<IGenome<GT>> PassGeneration();
        
    }
}
