using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionNetwork;

namespace EvolutionNetwork.Teacher
{


public class GenericTeacher<GT, Type> : IGeneticTeacher<GT,Type>
        where GT : IGenome<Type>
    {


        double ScoreMK;
        double UsualMK;
        double StructuralMK;
        double ComplexityMK;

        Random random;

        public GenericTeacher(int seed, double scoreMK, double usualMK,
            double structuralMK,double complexityMK, TestFunctionDelegate<GT> testFunction)
        {
            random = new Random(seed);
            ScoreMK = scoreMK;
            UsualMK = usualMK;
            StructuralMK = structuralMK;
            ComplexityMK = complexityMK;

            _generationHistory = new Dictionary<int, List<Tuple<double, GT>>>();
            TestFunction = testFunction;

        }

        List<GT> _currentPopulation = null;

        List<Tuple<double, GT>> _lastTests  = null;

        Dictionary<int, List<Tuple<double, GT>>> _generationHistory;

        int _currentGeneration = 0;
        public int CurrentGeneration
        {
            get { return _currentGeneration; }
        }
        public IReadOnlyDictionary<int, List<Tuple<double, GT>>> History
        {
            get
            {
                return new RODW(_generationHistory);
            }
        }

        public class ReadOnlyDictionary : IReadOnlyDictionary<int, List<Tuple<double, GT>>>
        {
            Dictionary<int, List<Tuple<double, GT>>> _ref;

            protected ReadOnlyDictionary(Dictionary<int, List<Tuple<double, GT>>> Reference)
            {
                _ref = Reference;
            }

            public List<Tuple<double, GT>> this[int key]
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

            public IEnumerable<List<Tuple<double, GT>>> Values
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

            public IEnumerator<KeyValuePair<int, List<Tuple<double, GT>>>> GetEnumerator()
            {
                return _ref.GetEnumerator();
            }

            public bool TryGetValue(int key, out List<Tuple<double, GT>> value)
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
            public RODW(Dictionary<int, List<Tuple<double, GT>>> Reference):base(Reference)
            {

            }
        }
    


        #region IGenericTeacher

        public TestFunctionDelegate<GT> TestFunction
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

        TestFunctionDelegate<GT> _testF;

        public IEnumerable<GT> CurrentPopulation
        {
            get
            {
                return _currentPopulation;
            }
        }

        public List<Tuple<double, GT>> AddNewGeneration(List<GT> Generation)
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


        public List<Tuple<double, GT>> PassTests()
        {
            List<Tuple<double, GT>> result = new List<Tuple<double, GT>>();
            lock (_currentPopulation)
            {
                Parallel.ForEach(_currentPopulation, x =>
                {
                    double testResult = TestFunction(x);
                    x.LastResult = testResult;
                    lock (result)
                    {
                        result.Add(new Tuple<double, GT>(testResult, x));
                    }
                });
            }
            result.Sort((x,y) =>
            {
                if(x.Item1>y.Item1)
                {
                    return -1;
                }else if(x.Item1<y.Item1)
                {
                    return 1;
                }else
                {
                    return 0;
                }
            });
            _lastTests = result;
            //_generationHistory.Add(_currentGeneration, result);
            return result;
        }

        public List<GT> GetNewGenerationPopulation()
        {
            if(_lastTests != null)
            {

                List<GT> newGeneration = new List<GT>();
                    lock (_lastTests)
                    {
                        newGeneration.Add((GT)_lastTests[_lastTests.Count - 1].Item2.Copy());
                        _lastTests.RemoveAt(0);

                        Parallel.ForEach(_lastTests, x =>
                         {
                             GT curGen = x.Item2;
                             double curResults = x.Item1;

                             double structureMutationChance = (Math.Abs(curGen.LastResult - curGen.ParentResult) * ScoreMK + UsualMK + curGen.Complexity * ComplexityMK)
                                / (curGen.StructuralMutations * StructuralMK);

                             if (random.NextDouble() < structureMutationChance)
                            {
                                 lock (newGeneration)
                                 {
                                     newGeneration.Add((GT)curGen.ProceedStructuralMutation());
                                 }
                             }
                             else
                             {
                                 lock (newGeneration)
                                 {
                                     newGeneration.Add((GT)curGen.ProceedNonStructuralMutation());
                                 }
                             }

                         });
                    }
                


                return newGeneration;
            }

            return null;
        }
        #endregion


    }
}
