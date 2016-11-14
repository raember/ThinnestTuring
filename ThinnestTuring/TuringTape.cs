using System;
using System.Collections.Generic;

namespace ThinnestTuring
{
    public sealed class TuringTape
    {
        public const int BANDOVERHEAD = 15;
        private int headPosition;

        public TuringTape(string baseWord, char emptySlot = '_'){ //'␣' doesn't work
            if (emptySlot.Equals('_')) { emptySlot = char.Parse("_"); }
            tape = new List<char>(baseWord.ToCharArray());
            this.emptySlot = emptySlot;
        }

        public TuringTape() : this(string.Empty){}
        public List<char> tape {get;}
        public char emptySlot {get;}

        public void WriteLeft(char write){
            WriteStay(write);
            headPosition--;
        }

        public void WriteRight(char write){
            WriteStay(write);
            headPosition++;
        }

        public void WriteLeft(){
            WriteLeft(emptySlot);
        }

        public void WriteRight(){
            WriteRight(emptySlot);
        }

        public void WriteStay(char write){
            if (headPosition < 0) {
                for (var amountPrefix = Math.Abs(headPosition) - 1; amountPrefix > 0; amountPrefix--) {
                    tape.Insert(0, emptySlot);
                }
                tape.Insert(0, write);
                headPosition = 0;
            } else if (headPosition >= tape.Count) {
                for (var amountSuffix = headPosition - tape.Count; amountSuffix > 0; amountSuffix--) {
                    tape.Add(emptySlot);
                }
                tape.Add(write);
            } else {
                tape[headPosition] = write;
            }
        }

        public void WriteStay(){
            WriteStay(emptySlot);
        }

        public char Read(){
            if (headPosition < 0 || headPosition >= tape.Count) { return emptySlot; }
            return tape[headPosition];
        }

		public void print(State state, bool accepted){
            var origColor = Console.ForegroundColor;
            var minIndex = headPosition - BANDOVERHEAD;
            var maxIndex = headPosition + BANDOVERHEAD;
            for (var index = minIndex; index < maxIndex; index++) {
                if (index == headPosition) {
					if (state is AcceptingState) {
						if (accepted) {
							Console.ForegroundColor = ConsoleColor.Green;
							Console.Write(state);
						} else {
							Console.ForegroundColor = ConsoleColor.DarkGreen;
							Console.Write(state);
						}
					} else if (state is DroppingState) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("X");
                    } else {
                        Console.ForegroundColor = ConsoleColor.Yellow;
						Console.Write(state);
                    }
                }
                if (index >= 0 && index < tape.Count) {
                    if (tape[index] != emptySlot) {
                        Console.ForegroundColor = ConsoleColor.White;
                    } else {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write(tape[index]);
                } else {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(emptySlot);
                }
            }
            Console.ForegroundColor = origColor;
        }

		public override string ToString()
		{
			return string.Join(string.Empty,tape).Insert(headPosition,"q");
		}
    }
}