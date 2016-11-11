using System;
using System.Collections.Generic;
using System.Linq;

namespace ThinnestTuring
{
    public class TuringMachine
    {
        public const int STATE_ACCEPT = -1;
        public const int STATE_FAIL = -2;
        private readonly List<TuringTape> tapes = new List<TuringTape>();
        private State currentState;

        public TuringMachine(State startingState, uint amountOfBands){
            Mode = TuringMode.STEP;
            currentState = startingState;
            if (amountOfBands == 0) {
                throw new ArgumentOutOfRangeException("amountOfBands", "The amount of tapes may not be zero.");
            }
            while (amountOfBands > 0) {
                tapes.Add(new TuringTape(string.Empty));
                amountOfBands--;
            }
        }

        public TuringMachine(State startingState)
            : this(startingState, 1){}

        public int CalculatedSteps {get; private set;}
        public TuringMode Mode {get; set;}

        private void Step(){
            currentState = currentState.GetNextState(tapes);
            CalculatedSteps++;
            if (Mode == TuringMode.STEP) {
                print();
            }
        }

        public bool Compute(List<char> word){
            var tape = tapes.First();
            word.Reverse();
            word.GetRange(0, word.Count - 1).ForEach(c => tape.WriteLeft(c));
            tape.WriteStay(word.Last());
            if (Mode == TuringMode.STEP) {
                print();
            }
            while (currentState.Index >= 0) {
                Step();
                if (Mode == TuringMode.STEP) {
                    //Console.ReadKey();
                }
            }
            if (Mode == TuringMode.RUN) {
                print();
            }
            return (currentState.Index == STATE_ACCEPT);
        }

        public bool Compute(string word){
            return Compute(word.ToList());
        }

        public void print(){
            var bandN = 0;
            var origColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(new string('=', 72));
            foreach (var band in tapes) {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Band " + bandN + ":[");
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
    STEP,
    RUN
}