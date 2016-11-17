﻿using System;
using System.Collections.Generic;

namespace ThinnestTuring
{
    [Serializable]
    public sealed class TuringTape
    {
        public const int TAPEPRINTOVERHEAD = 15;
        private int headPosition;

        public TuringTape(string baseWord = ""){ //'␣' for empty space doesn't work
            tape = new List<char>(baseWord.ToCharArray());
        }

        public List<char> tape {get;}

        public void WriteLeft(char write){
            WriteStay(write);
            headPosition--;
        }

        public void WriteRight(char write){
            WriteStay(write);
            headPosition++;
        }

        public void WriteLeft(){
            WriteLeft('_');
        }

        public void WriteRight(){
            WriteRight('_');
        }

        public void WriteStay(char write){
            if (headPosition < 0) {
                for (var amountPrefix = Math.Abs(headPosition) - 1; amountPrefix > 0; amountPrefix--) {
                    tape.Insert(0, '_');
                }
                tape.Insert(0, write);
                headPosition = 0;
            } else if (headPosition >= tape.Count) {
                for (var amountSuffix = headPosition - tape.Count; amountSuffix > 0; amountSuffix--) { tape.Add('_'); }
                tape.Add(write);
            } else {
                tape[headPosition] = write;
            }
        }

        public void WriteStay(){
            WriteStay('_');
        }

        public char Read(){
            if (headPosition < 0 || headPosition >= tape.Count) { return '_'; }
            return tape[headPosition];
        }

        public void print(State state, bool final){
            var origColor = Console.ForegroundColor;
            var minIndex = headPosition - TAPEPRINTOVERHEAD;
            var maxIndex = headPosition + TAPEPRINTOVERHEAD;
            for (var index = minIndex; index < maxIndex; index++) {
                if (index == headPosition) {
                    if (state is AcceptingState) {
                        if (final) {
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
                    if (tape[index] != '_') {
                        Console.ForegroundColor = ConsoleColor.White;
                    } else {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write(tape[index]);
                } else {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write('_');
                }
            }
            Console.ForegroundColor = origColor;
        }

        public override string ToString(){
            return string.Join(string.Empty, tape).Insert(headPosition, "q");
        }
    }
}