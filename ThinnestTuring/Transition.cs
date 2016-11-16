using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ThinnestTuring
{
    [Serializable]
    public sealed class Transition
    {
        private Transition(string pattern, string replacement, List<HeadMovement> movement, State fromState,
                           State nextState){
            Pattern = pattern;
            Replacement = replacement;
            Movement = movement;
            NextState = nextState;
            FromState = fromState;
            AmountOfTapes = pattern.Length;
            if (replacement.Length != AmountOfTapes || movement.Count != AmountOfTapes) {
                throw new ArgumentException("The amounts of tapes required differes among the input variables.");
            }
        }

        [DataMember(Name = "Pattern")]
        public string Pattern {get;}

        [DataMember(Name = "Replacement")]
        public string Replacement {get;}

        [DataMember(Name = "Move")]
        public List<HeadMovement> Movement {get;}

        [DataMember(Name = "From")]
        public State FromState {get;}

        [DataMember(Name = "To")]
        public State NextState {get;}

        [DataMember(Name = "AmountOfTapes")]
        public int AmountOfTapes {get;}

        public bool IsMatch(List<TuringTape> tapes){
            return Regex.IsMatch(new string(tapes.ConvertAll(t => t.Read()).ToArray()), Pattern.Replace('*', '.'));
        }

        public static Transition Parse(string input, State fromState, State nextState){
            var mtch = Regex.Match(input, "^(?<Read>.+?)/(?<Replace>.+?),(?<Move>(L|R|S)+)$");
            var pattern = mtch.Groups["Read"].Value;
            var replace = mtch.Groups["Replace"].Value;
            var move = new List<HeadMovement>();
            foreach (var moveMatch in mtch.Groups["Move"].Value.ToCharArray()) {
                switch (moveMatch) {
                    case 'L':
                        move.Add(HeadMovement.Left);
                        break;
                    case 'R':
                        move.Add(HeadMovement.Right);
                        break;
                    case 'S':
                        move.Add(HeadMovement.Stay);
                        break;
                }
            }
            return new Transition(pattern, replace, move, fromState, nextState);
        }

        public State Apply(List<TuringTape> tapes){
            var pattern = string.Join(string.Empty, Pattern.Replace('*', '.').ToList().ConvertAll(c => "(" + c + ")"));
            var replace = string.Empty;
            var index = 1;
            foreach (var c in Replacement.ToCharArray()) {
                if (c == '*') {
                    replace += "$" + index + ",";
                    index++;
                    continue;
                }
                index++;
                replace += c + ",";
            }
            var newKonf = Regex.Replace(new string(tapes.ConvertAll(t => t.Read()).ToArray()), pattern, replace);
            newKonf = newKonf.Replace(",", "");
            for (var i = 0; i < tapes.Count; i++) {
                switch (Movement[i]) {
                    case HeadMovement.Left:
                        tapes[i].WriteLeft(newKonf[i]);
                        break;
                    case HeadMovement.Right:
                        tapes[i].WriteRight(newKonf[i]);
                        break;
                    case HeadMovement.Stay:
                        tapes[i].WriteStay(newKonf[i]);
                        break;
                }
            }
            return NextState;
        }

        public string GetCondition(){
            var pattern = string.Join(string.Empty, Pattern);
            var replace = string.Join(string.Empty, Replacement);
            var move = string.Join(string.Empty, Movement.ConvertAll(m => m.ToString().First()));
            return string.Format("{0}/{1},{2}", pattern, replace, move);
        }

        public string ToLaTeX(){
            if (NextState.Equals(FromState)) { //{${1}$} ({0})
                return string.Format("({0}) edge[out=80,in=100,loop] node[above]{1} ({0})", FromState,
                    "{$" + GetCondition() + "$}");
            }
            return string.Format("({0}) edge node[above]{1} ({2})", FromState, "{$" + GetCondition() + "$}", NextState);
        }

        public static List<Transition> FromLaTeX(string input, List<State> states){
            //(q1) edge node[above]{$b/e,R$} (q4)
            var conds = new List<Transition>();
            foreach (Match mtch in Regex.Matches(input,
                @"\((?<fromState>q.+?)\)[ ]*edge.*?node.*?\{\$(?<condition>.+?)\$\}[ ]*\((?<toState>q.+?)\)",
                RegexOptions.Singleline | RegexOptions.IgnoreCase)) {
                var from = mtch.Groups["fromState"].Value;
                var fromState = states.FirstOrDefault(s => s.ToString().Equals(from));
                var to = mtch.Groups["toState"].Value;
                var toState = states.FirstOrDefault(s => s.ToString().Equals(to));
                if (fromState == null || toState == null) {
                    throw new Exception(string.Format("Couldn'd find either {0} or {1} in states.", from, to));
                }
                fromState.AddTransition(mtch.Groups["condition"].Value, toState);
                conds.Add(fromState.Transitions.Last());
            }
            return conds;
        }

        public override string ToString(){
            return string.Format("{0} => {1}", GetCondition(), NextState);
        }
    }

    public enum HeadMovement
    {
        Left,
        Right,
        Stay
    }
}