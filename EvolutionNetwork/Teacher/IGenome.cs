using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionNetwork.Teacher
{

    public interface IGenome<GenomeType>
    {


        /// <summary>
        /// Copies current IGenome
        /// </summary>
        /// <returns>Copy of the IGenome</returns>
        IGenome<GenomeType> Copy();

        /// <summary>
        /// Number of total non structural mutations done.
        /// </summary>
        int NonStructuralMutations
        {
            get;
        }

        /// <summary>
        /// Number of total structural mutations done in the whole family.
        /// </summary>
        int TotalNonStructuralMutations
        {
            get;
        }

        /// <summary>
        /// Number of structural mutations done by this specific genome family.
        /// </summary>
        int StructuralMutations
        {
            get;
        }

        /// <summary>
        /// The structure complexity of the genome.
        /// </summary>
        double Complexity
        {
            get;
        }

        double LastResult
        {
            get;
            set;
        }

        double ParentResult
        {
            get;
            set;
        }



        /// <summary>
        /// Proceeds structural mutation and return the result of it.
        /// </summary>
        /// <returns>The result of mutation.</returns>
        IGenome<GenomeType> ProceedStructuralMutation();

        /// <summary>
        /// Proceeds non structural mutation and return the result. 
        /// </summary>
        /// <returns>The result of mutation.</returns>
        IGenome<GenomeType> ProceedNonStructuralMutation();
    }
}
