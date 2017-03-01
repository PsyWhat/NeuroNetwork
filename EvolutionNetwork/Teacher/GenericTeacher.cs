using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionNetwork;

namespace EvolutionNetwork.Teacher
{


public class GenericTeacher<GT> : IGeneticTeacher<GT>
		where GT : IGenome<GT>
    {


        double ScoreMK;
        double UsualMK;
        double StructuralMK;
        double ComplexityMK;

        Random random;

        List<IGenome<GT>> _currentPopulation;

        SortedList<double, IGenome<GT>> _lastTests;

        Dictionary<int, SortedList<double, IGenome<GT>>> _generationHistory;

        int _currentGeneration;

        public IReadOnlyDictionary<int,SortedList<double,IGenome<GT>>> History
        {
            get;
        }


        #region IGenericTeacher

        public TestFunctionDelegate<IGenome<GT>> TestFunction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IGenome<GT>> CurrentPopulation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public SortedList<double, IGenome<GT>> PassTests()
        {
            SortedList<double, IGenome<GT>> result = new SortedList<double, IGenome<GT>>();
            Parallel.ForEach(_currentPopulation, x =>
             {
                 double testResult = TestFunction(x);
                 x.LastResult = testResult;
                 lock (result)
                 {
                     result.Add(testResult, x);
                 }
             });
            _lastTests = result;
            _generationHistory.Add(_currentGeneration, result);
            return result;
        }

        public List<IGenome<GT>> PassGeneration()
        {
            if(_lastTests != null)
            {

                List<IGenome<GT>> newGeneration = new List<IGenome<GT>>();

                Parallel.ForEach(_lastTests, x =>
                 {
                     IGenome<GT> curGen = x.Value;
                     double curResults = x.Key;

                     double structureMutationChance = Math.Abs(curGen.LastResult - curGen.ParentResult) * ScoreMK + UsualMK / curGen.StructuralMutations * StructuralMK + curGen.Complexity * ComplexityMK;

                     if(random.NextDouble() < structureMutationChance)
                     {
                         newGeneration.Add(curGen.ProceedStructuralMutation());
                     }else
                     {
                         newGeneration.Add(curGen.ProceedNonStructuralMutation());
                     }


                 });


                return newGeneration;
            }

            return null;
        }
        #endregion


    }
}
