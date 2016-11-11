using System;
using System.Collections.Generic;

namespace ThinnestTuring
{
    public class TuringMachine
    {
        private readonly List<TuringTape> tapes = new List<TuringTape>();
        private int state;
        public int CalculatedSteps { get; private set; }

        public static readonly int STATE_ACCEPT = -1;
        public static readonly int STATE_FAIL = -2;

        public TuringMachine(Func<int, List<TuringTape>, int> transitionAction, uint amountOfBands, string baseWord){
            TransitionAction = transitionAction;
            Mode = TuringMode.STEP;
            if (amountOfBands == 0){
                throw new ArgumentOutOfRangeException("amountOfBands", "The amount of tapes may not be zero.");
            }
            tapes.Add(new TuringTape(baseWord));
            while (amountOfBands > 1){
                tapes.Add(new TuringTape(string.Empty));
                amountOfBands--;
            }
        }

        public TuringMachine(Func<int, List<TuringTape>, int> transitionAction, string baseWord = "")
            : this(transitionAction, 1, baseWord){}

        public TuringMode Mode { get; set; }
        public Func<int, List<TuringTape>, int> TransitionAction { get; set; }

        public void Step(){
            state = TransitionAction.Invoke(state, tapes);
            CalculatedSteps++;
            if (Mode == TuringMode.STEP){
                print();
            }
        }

        public bool Compute(){
            while (state >= 0){
                Step();
                //Console.ReadKey();
            }
            if (Mode == TuringMode.RUN) {
                print();
            }
            return (state == STATE_ACCEPT);
        }

        public void print(){
            var bandN = 0;
            var origColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(new string('=', 72));
            foreach (var band in tapes){
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Band " + bandN + ":[");
                band.print(state);
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