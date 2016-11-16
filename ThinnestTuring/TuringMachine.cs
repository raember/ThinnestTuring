using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ThinnestTuring
{
    [Serializable]
    public class TuringMachine
    {
        [DataMember(Name = "TransitionsChecked")] private bool checkedTransitions;

        public TuringMachine(){
            Mode = TuringMode.Step;
            Tapes = new List<TuringTape>();
            States = new List<State>();
        }

        [DataMember(Name = "From")]
        public List<TuringTape> Tapes {get;}

        [DataMember(Name = "CalculatedSteps")]
        public int CalculatedSteps {get; private set;}

        [DataMember(Name = "Mode")]
        public TuringMode Mode {get; set;}

        [DataMember(Name = "States")]
        public List<State> States {get;}

        [DataMember(Name = "CurrentState")]
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

        //TODO: Implement LaTeX export
        public string ToLaTeX(){
            var str = string.Empty;
            foreach (var s in States) {
                for (var i = 0; i < s.Transitions.Count - 1; i++) {
                    switch (i) {
                        case 0:
                            break;
                    }
                }
            }
            return null;
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