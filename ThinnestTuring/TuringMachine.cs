using System;
using System.Collections.Generic;
using System.Linq;

namespace ThinnestTuring
{
    public class TuringMachine
    {
        public const int STATE_ACCEPT = -1;
        public const int STATE_FAIL = -2;
        private State currentState;

        public TuringMachine(State startingState, uint amountOfBands){
            Mode = TuringMode.Step;
            currentState = startingState;
            if (amountOfBands == 0) {
                throw new ArgumentOutOfRangeException("amountOfBands", "The amount of tapes may not be zero.");
            }
            tapes = new List<TuringTape>();
            while (amountOfBands > 0) {
                tapes.Add(new TuringTape(string.Empty));
                amountOfBands--;
            }
        }

        public TuringMachine(State startingState)
            : this(startingState, 1){}

        public List<TuringTape> tapes {get;}
        public int CalculatedSteps {get; private set;}
        public TuringMode Mode {get; set;}

        private void Step(){
            currentState = currentState.GetNextState(tapes);
            CalculatedSteps++;
            if (Mode == TuringMode.Step) { print(); }
        }

        public bool Compute(List<char> word){
            var tape = tapes.First();
            if (word.Any()) {
                word.Reverse();
                word.GetRange(0, word.Count - 1).ForEach(c => tape.WriteLeft(c));
                tape.WriteStay(word.Last());
            }
            if (Mode == TuringMode.Step) { print(); }
            while (currentState.Index >= 0) {
                Step();
                if (Mode == TuringMode.Step) {
                    //Console.ReadKey();
                }
            }
            if (Mode == TuringMode.Run) { print(); }
            return (currentState.Index == STATE_ACCEPT);
        }

        public bool Compute(string word){
            return Compute(word.ToList());
        }

        public void print(){
            var bandN = 0;
            var origColor = Console.ForegroundColor;
            if (tapes.Count > 1) {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(new string('=', 41));
            }
            foreach (var band in tapes) {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (tapes.Count > 1) { Console.Write("Band " + bandN + ":"); }
                Console.Write("[");
                band.print(currentState.Index);
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