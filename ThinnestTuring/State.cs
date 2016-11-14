using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ThinnestTuring
{
    public class State
    {
        private string _tikzPrefix;

        public State(int index, string tikzPrefix = ""){
            Index = index;
            _tikzPrefix = tikzPrefix;
            Conditions = new List<Condition>();
			AmountOfTapes = 0;
        }

		public int Index {get;}
        public List<Condition> Conditions {get;}
		public int AmountOfTapes {get; private set;}

        public void AddCondition(string condition, State nextState){
            Conditions.Add(Condition.Parse(condition, this, nextState));
			if (AmountOfTapes == 0) {
				AmountOfTapes = Conditions.First().AmountOfTapes;
				return;
			}
			if (!Conditions.All(c => c.AmountOfTapes == AmountOfTapes)) {
				throw new ArgumentException("The amounts of tapes required differes among the conditions.");
			}
        }

		public bool CanStep(List<TuringTape> read) {
			return Conditions.Any(c => c.IsMatch(read));
		}

        public State GetNextState(List<TuringTape> read){
            var match = Conditions.FirstOrDefault(c => c.IsMatch(read));
            return match.Apply(read);
        }

		public string ToLaTeX(State fromState, string direction){// {$q_{{2}}$};
			return string.Format("\\node[{0}state]({2})[{4} of = {1}] {3}",
				_tikzPrefix, this, fromState, Index, "{$q_{" + this + "}$}", direction);
		}
		public string ToLaTeX(){// {$q_{{3}}$};
			return string.Format("\\node[{0}state]({2}) {3}", _tikzPrefix, this, "{$q_{" + this + "}$}");
		}
        public static List<State> FromLaTeX(string input) {
            List<State> states = new List<State>();
            foreach (Match mtch in Regex.Matches(input,
                @"\\node\[(?<mod>accepting,|initial,|)[ ]*state\][ ]*\(q(?<index>\d+|e|e\d+\);",
                RegexOptions.Singleline | RegexOptions.IgnoreCase)) {
                switch (mtch.Groups["index"].Value) {
                    case "accepting,":
						states.Add(new AcceptingState(-1-states.Count(s => s is AcceptingState)));
                        break;
                    case "initial,":
                        states.Add(new State(0,"initial,"));
                        break;
                    case "":
                        states.Add(new State(int.Parse(mtch.Groups["mod"].Value)));
                        break;
                }
            }
            return states;
        }

        public override string ToString(){
			if (Index == -1) {
				return "qe";
			} else if (Index < -1){
				return "qe" + Math.Abs(Index);
			}
            return "q" + Index;
        }
    }
}