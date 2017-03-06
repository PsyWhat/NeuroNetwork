using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionNetwork.Teacher
{
    public delegate double TestFunctionDelegate<GT>(GT genome);
    /*where GT : IGenome<GT>;*/


    public interface IGeneticTeacher<GT,Type>
        where GT : IGenome<Type>
    {

        TestFunctionDelegate<GT> TestFunction
        {
            get;
            set;
        }
       

        /// <summary>
        /// Current population of the genome.
        /// </summary>
        IEnumerable<GT> CurrentPopulation
        {
            get;
        }



        /// <summary>
        /// Pass the tests and save the results.
        /// </summary>
        List<Tuple<double, GT>> PassTests();

        /// <summary>
        /// Passing generation, according to the results.
        /// </summary>
        List<GT> GetNewGenerationPopulation();
        
    }
}
