using System;
using System.Collections;
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

        List<IGenome<GT>> _currentPopulation = null;

        SortedList<double, IGenome<GT>> _lastTests  = null;

        Dictionary<int, SortedList<double, IGenome<GT>>> _generationHistory;

        int _currentGeneration = 0;

        public IReadOnlyDictionary<int,SortedList<double,IGenome<GT>>> History
        {
            get
            {
                return new RODW(_generationHistory);
            }
        }

        public class ReadOnlyDictionary : IReadOnlyDictionary<int, SortedList<double, IGenome<GT>>>
        {
            Dictionary<int, SortedList<double, IGenome<GT>>> _ref;

            protected ReadOnlyDictionary(Dictionary<int, SortedList<double, IGenome<GT>>> Reference)
            {
                _ref = Reference;
            }

            public SortedList<double, IGenome<GT>> this[int key]
            {
                get
                {
                    return _ref[key];
                }
            }

            public int Count
            {
                get
                {
                    return _ref.Count;
                }
            }

            public IEnumerable<int> Keys
            {
                get
                {
                    return _ref.Keys;
                }
            }

            public IEnumerable<SortedList<double, IGenome<GT>>> Values
            {
                get
                {
                    return _ref.Values;
                }
            }

            public bool ContainsKey(int key)
            {
                return _ref.ContainsKey(key);
            }

            public IEnumerator<KeyValuePair<int, SortedList<double, IGenome<GT>>>> GetEnumerator()
            {
                return _ref.GetEnumerator();
            }

            public bool TryGetValue(int key, out SortedList<double, IGenome<GT>> value)
            {
                return _ref.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _ref.GetEnumerator();
            }
        }

        private class RODW : ReadOnlyDictionary
        {
            public RODW(Dictionary<int, SortedList<double, IGenome<GT>>> Reference):base(Reference)
            {

            }
        }
    


        #region IGenericTeacher

        public TestFunctionDelegate<IGenome<GT>> TestFunction
        {
            get
            {
                return _testF;
            }
            set
            {
                _testF = value;
            }
        }

        TestFunctionDelegate<IGenome<GT>> _testF;

        public IEnumerable<IGenome<GT>> CurrentPopulation
        {
            get
            {
                return _currentPopulation;
            }
        }

        public SortedList<double, IGenome<GT>> AddNewGeneration(List<IGenome<GT>> Generation)
        {
            if(_lastTests != null)
            {
                _generationHistory.Add(_currentGeneration, _lastTests);
                _lastTests = null;
            }
            _currentPopulation = Generation;
            _currentGeneration++;
            return PassTests();
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
            //_generationHistory.Add(_currentGeneration, result);
            return result;
        }

        public List<IGenome<GT>> GetNewGenerationPopulation()
        {
            if(_lastTests != null)
            {

                List<IGenome<GT>> newGeneration = new List<IGenome<GT>>();

                newGeneration.Add(_lastTests[_lastTests.Count-1].Copy());

                _lastTests.RemoveAt(0);

                Parallel.ForEach(_lastTests, x =>
                 {
                     IGenome<GT> curGen = x.Value;
                     double curResults = x.Key;

                     double structureMutationChance = (Math.Abs(curGen.LastResult - curGen.ParentResult) * ScoreMK + UsualMK)
                        / (curGen.StructuralMutations * StructuralMK + curGen.Complexity * ComplexityMK);

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
