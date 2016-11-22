using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ThinnestTuring
{
    public class State
    {
        private readonly string _tikzPrefix;

        public State(int index, string tikzPrefix = ""){
            Index = index;
            if (index == 0) { _tikzPrefix = "initial,"; } else { _tikzPrefix = tikzPrefix; }
            Transitions = new List<Transition>();
            AmountOfTapes = 0;
        }

        public int Index {get;}
        public List<Transition> Transitions {get;}
        public int AmountOfTapes {get; private set;}

        public void AddTransition(string condition, State nextState){
            Transitions.Add(Transition.Parse(condition, this, nextState));
            if (AmountOfTapes == 0) {
                AmountOfTapes = Transitions.First().AmountOfTapes;
                return;
            }
            if (Transitions.Any(c => c.AmountOfTapes != AmountOfTapes)) {
                throw new ArgumentException("The amounts of tapes required differes among the conditions.");
            }
        }

        public void AddTransition(string condition){
            AddTransition(condition, this);
        }

        public bool CanStep(List<TuringTape> read){
            return Transitions.Any(c => c.IsMatch(read));
        }

        public State GetNextState(List<TuringTape> read){
            var match = Transitions.FirstOrDefault(c => c.IsMatch(read));
            return match.Apply(read);
        }

        public string ToLaTeX(State fromState, string direction){ // {$q_{{2}}$};
			var indx = (this is AcceptingState) ? "e" : Index.ToString();
			return string.Format("\\node[{0}state]({2})[{4} of = {1}] {3};",
				_tikzPrefix, fromState, this, "{$q_{" + indx + "}$}", direction);
        }

        public string ToLaTeX(){ // {$q_{{3}}$};
            return string.Format("\\node[{0}state]({1}) {2};", _tikzPrefix, this, "{$q_{" + Index + "}$}");
        }

        public string ToLaTeX(State fromState){
            return string.Format("\\node[{0}state]({1}) [right of={2}] {3};", _tikzPrefix, this, fromState,
                "{$q_{" + Index + "}$}");
        }

        public static List<State> FromLaTeX(string input){
            var states = new List<State>();
            foreach (Match mtch in Regex.Matches(input,
                @"\\node\[(?<mod>accepting,|initial,|)[ ]*state\][ ]*\(q(?<index>\d+|e|e\d+)\);",
                RegexOptions.Singleline | RegexOptions.IgnoreCase)) {
                switch (mtch.Groups["index"].Value) {
                    case "accepting,":
                        states.Add(new AcceptingState(-1 - states.Count(s => s is AcceptingState)));
                        break;
                    case "initial,":
                        states.Add(new State(0, "initial,"));
                        break;
                    case "":
                        states.Add(new State(int.Parse(mtch.Groups["mod"].Value)));
                        break;
                }
            }
            return states;
        }

        public override string ToString(){
            if (Index == -1) { return "qe"; }
            if (Index < -1 && Index > int.MinValue) { return "qe" + Math.Abs(Index); }
            if (Index == int.MinValue) { return "qx"; }
            return "q" + Index;
        }
    }
}