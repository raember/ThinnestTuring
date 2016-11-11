using System;
using System.Collections.Generic;
using System.Text;

namespace ThinnestTuring
{
    internal class MainClass
    {
        public static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.ReadLine();
            Console.WriteLine("THE TURING TEST");
            var M = new TuringMachine(Transition, 3, "000010");
            M.print();
            if (M.Compute()){
                Console.WriteLine("Das Wort gehört zur Sprache. Berechnung fertig. Benötigte Schritte: " + M.CalculatedSteps);
            }
            Console.ReadLine();
        }

        private static int Transition(int state, List<TuringTape> tapes){
            var factor1 = tapes[0];
            var factor2 = tapes[1];
            var result = tapes[2];
            switch (state){
                case 0: //Übertrage ersten Faktor auf Band 2
                    if (factor1.Read().Equals('0')) {// 0**/_0*, RLS => q0
                        factor1.WriteRight();
                        factor2.WriteLeft('0');
                        return 0;
                    }
                    if (factor1.Read().Equals('1')) {// 1**/_**,RSS => q1
                        factor1.WriteRight(factor1.emptySlot);
                        factor2.WriteRight();
                        return 1;
                    }
                    return TuringMachine.STATE_FAIL; // Abfallen
                case 1: //Überprüfe, ob noch eine Stelle des ersten Faktors zu verarbeiten ist
                    if (factor1.Read().Equals('0')){// 0**/0**, RSS => q2
                        factor1.WriteRight('0');
                        return 2;
                    }
                    if (factor1.Read().Equals(factor1.emptySlot)){// _**/_**, SSS => qe
                        return TuringMachine.STATE_ACCEPT;
                    }
                    return TuringMachine.STATE_FAIL; // Abfallen
                case 2: //Addiere Band 2 auf Band 3
                    if (factor2.Read().Equals('0')) {// *0*/*00,SRR => q2
                        factor2.WriteRight('0');
                        result.WriteRight('0');
                        return 2;
                    }
                    if (factor2.Read().Equals(factor2.emptySlot)){// *_*/*_*,SLS => q3
                        factor2.WriteLeft();
                        return 3;
                    }
                    return TuringMachine.STATE_FAIL; // Abfallen
                case 3: //Rücke zum Anfang des zweiten Faktors
                    if (factor2.Read().Equals('0')) {// *0*/*0*,SLS => q3
                        factor2.WriteLeft('0');
                        return 3;
                    }
                    if (factor2.Read().Equals(factor2.emptySlot)) {// *_*/*_*,SRS => q1
                        factor2.WriteRight();
                        return 1;
                    }
                    return TuringMachine.STATE_FAIL; // Abfallen
                default:
                    throw new ArgumentOutOfRangeException("state");
            }
        }
    }
}