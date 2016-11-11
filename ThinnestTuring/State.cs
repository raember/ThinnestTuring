using System;
using System.Collections.Generic;
using System.Linq;

namespace ThinnestTuring
{
    public class State
    {
        public State(int index){
            Index = index;
            Transitions = new Dictionary<Func<List<TuringTape>, bool>, State>();
        }

        public int Index {get;}
        public Dictionary<Func<List<TuringTape>, bool>, State> Transitions {get;}

        public void AddConnection(State other, Func<List<TuringTape>, bool> transition){
            Transitions.Add(transition, other);
        }

        public State GetNextState(List<TuringTape> read){
            var nextState = Transitions.ToList().FirstOrDefault(kv => kv.Key.Invoke(read)).Value;
            if (nextState == null) {
                return new DroppingState();
            }
            return nextState;
        }
    }
}