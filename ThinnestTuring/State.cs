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
        }

        public int Index {get;}
        public List<Condition> Conditions {get;}

        public void AddCondition(string condition, State nextState){
            Conditions.Add(Condition.Parse(condition, this, nextState));
        }

        public State GetNextState(List<TuringTape> read){
            var match = Conditions.FirstOrDefault(c => c.IsMatch(read));
            if (match == null) { return new DroppingState(); }
            return match.Apply(read);
        }

        public string ToLaTeX(State fromState){
            return string.Format("\\node[{0}state]({2})[right of = {1}] {$q_{3}$};", _tikzPrefix, this, fromState, Index);
        }
        public static List<State> FromLaTeX(string input) {
            List<State> states = new List<State>();
            foreach (Match mtch in Regex.Matches(input,
                @"\\node\[(?<mod>accepting,|initial,|)[ ]*state\][ ]*\(q(?<index>\d+|e\);",
                RegexOptions.Singleline | RegexOptions.IgnoreCase)) {
                switch (mtch.Groups["index"].Value) {
                    case "accepting,":
                        states.Add(new AcceptingState());
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
            return "q" + Index;
        }
    }
}