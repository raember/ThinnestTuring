using System;
using System.Collections.Generic;
using System.Linq;

namespace ThinnestTuring
{
    public class TuringMachine
    {
		private bool checkedConditions;

        public TuringMachine(){
            Mode = TuringMode.Step;
            Tapes = new List<TuringTape>();
			States = new List<State>();
        }

        public List<TuringTape> Tapes {get;}
        public int CalculatedSteps {get; private set;}
        public TuringMode Mode {get; set;}
		public List<State> States {get; private set;}
		public State CurrentState { get; set;}

		public bool Prepare() {
			var amountOfTapes = CurrentState.AmountOfTapes;
			if (States.All(s => s.AmountOfTapes == amountOfTapes || s.AmountOfTapes == 0)) {
				for (int i = 0; i < amountOfTapes; i++) {
					Tapes.Add(new TuringTape());
				}
				checkedConditions = true;
			}
			return checkedConditions;
		}

		public bool Compute(string word){
			return Compute(word.ToList());
		}

        public bool Compute(List<char> word){
			if (!checkedConditions) {
				throw new Exception("I'm not prepared yet.");
			}
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
			if (!accepted) {CurrentState = new DroppingState(int.MinValue);}
            print(accepted);
			return (accepted);
        }

		public State CreateState(){
			var newState = new State(States.Count(s => s is State));
			if (CurrentState == null) {
				CurrentState = newState;
			}
			States.Add(newState);
			return newState;
		}

		public AcceptingState CreateAcceptingState() {
			var newState = new AcceptingState(-1-States.Count(s => s is AcceptingState));
			if (CurrentState == null) {
				CurrentState = newState;
			}
			States.Add(newState);
			return newState;
		}

		//TODO: Implement LaTeX export
		public string ToLaTeX(){
			var str = string.Empty;
			foreach (var s in States) {
				for (int i = 0; i < s.Conditions.Count - 1; i++){
					switch (i) {
						case 0:
							break;
					}
				}
			}
			return null;
		}

		public void print(bool accepted){
            var bandN = 0;
            var origColor = Console.ForegroundColor;
            if (Tapes.Count > 1) {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(new string('=', 41));
            }
			foreach (var tape in Tapes) {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (Tapes.Count > 1) { Console.Write("Band " + bandN + ":"); }
                Console.Write("[");
				tape.print(CurrentState, accepted);
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