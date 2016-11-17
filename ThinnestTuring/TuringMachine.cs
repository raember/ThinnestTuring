using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThinnestTuring
{
    public class TuringMachine
    {
        private bool checkedTransitions;

        public TuringMachine(){
            Mode = TuringMode.Step;
            Tapes = new List<TuringTape>();
            States = new List<State>();
        }

        public List<TuringTape> Tapes {get;}
        public int CalculatedSteps {get; private set;}
        public TuringMode Mode {get; set;}
        public List<State> States {get;}
        public State CurrentState {get; set;}

        public bool Initialize(){
            var amountOfTapes = CurrentState.AmountOfTapes;
            if (States.All(s => s.AmountOfTapes == amountOfTapes || s.AmountOfTapes == 0)) {
                for (var i = 0; i < amountOfTapes; i++) { Tapes.Add(new TuringTape()); }
                checkedTransitions = true;
            }
            return checkedTransitions;
        }

        public bool Compute(string word){
            return Compute(word.ToList());
        }

        public async Task<bool> ComputeAsync(List<char> word){
            return await Task.Run(() => { return Compute(word); });
        }

        public async Task<bool> ComputeAsync(string word){
            return await ComputeAsync(word.ToCharArray().ToList());
        }

        public bool Compute(List<char> word){
            if (!checkedTransitions) { throw new Exception("I'm not prepared yet."); }
            var tape = Tapes.First();
            if (word.Any()) {
                word.Reverse();
                word.GetRange(0, word.Count - 1).ForEach(c => tape.WriteLeft(c));
                tape.WriteStay(word.Last());
            }
            if (Mode == TuringMode.Step) { print(false); }
            while (CurrentState.CanStep(Tapes)) {
                CurrentState = CurrentState.GetNextState(Tapes);
                CalculatedSteps++;
                if (Mode == TuringMode.Step) {
                    print(false);
                    //Console.ReadKey();
                }
            }
            var accepted = (CurrentState is AcceptingState);
            if (!accepted) { CurrentState = new DroppingState(int.MinValue); }
            ;
            print(true);
            return (accepted);
        }

        public State CreateState(){
            var newState = new State(States.Count(s => s is State));
            if (CurrentState == null) { CurrentState = newState; }
            States.Add(newState);
            return newState;
        }

        public AcceptingState CreateAcceptingState(){
            var newState = new AcceptingState(-1 - States.Count(s => s is AcceptingState));
            if (CurrentState == null) { CurrentState = newState; }
            States.Add(newState);
            return newState;
        }
        
        public string ToLaTeX(){
            var str = new List<string>();
            if (States.Count > 0) {
                str.Add(States.First().ToLaTeX());
                if (States.Count > 1) {
                    for (int i = 1; i < States.Count; i++) {
                        str.Add(States[i].ToLaTeX(States[i - 1]));
                    }
                }
            }
            str.Add(string.Empty);
            str.Add("\\path[->]");
            var trans = new List<Transition>();
            foreach (var s in States) {
                trans.AddRange(s.Transitions);
            }
            str.AddRange(trans.Distinct().ToList().Select(t => t.ToLaTeX()));
            str.Add(";");
            return string.Join("\n", str);
        }

        public void print(bool final){
            var bandN = 0;
            var origColor = Console.ForegroundColor;
            if (Tapes.Count > 1 && !final) {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(new string('=', 2*TuringTape.TAPEPRINTOVERHEAD + 8
                                                  + bandN.ToString().Length + CurrentState.ToString().Length));
            }
            if (final) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (Tapes.Count > 1) {
                    Console.WriteLine(new string('=', 2*TuringTape.TAPEPRINTOVERHEAD + 9
                                                      + CurrentState.ToString().Length));
                } else {
                    Console.WriteLine(new string('=', 2*TuringTape.TAPEPRINTOVERHEAD + 6
                                                      + bandN.ToString().Length + CurrentState.ToString().Length));
                }
                Console.ForegroundColor = origColor;
                Console.WriteLine("Final state has been reached:");
            }
            foreach (var tape in Tapes) {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (Tapes.Count > 1) { Console.Write("Band " + bandN + ":"); }
                Console.Write("[");
                tape.print(CurrentState, final);
                Console.WriteLine("]");
                bandN++;
            }
            Console.ForegroundColor = origColor;
        }
    }
}

public enum TuringMode
{
    Step,
    Run
}