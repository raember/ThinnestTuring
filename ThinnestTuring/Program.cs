using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;

namespace ThinnestTuring
{
    internal class MainClass
    {
        public static void Main(string[] args){
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.WriteLine("THE TURING TEST");
            while (true) {
                Console.WriteLine("===============");
                Console.WriteLine();
                Console.WriteLine("Bitte ein Wort angeben.");
                var states = CreateStateNetwork();
                var TM = new TuringMachine(states.First(), 3);
                //TM.Mode = TuringMode.RUN;
                var input = Console.ReadLine();
                var reg = Regex.Match(input, "(0*)1(0*)");
                var int1 = reg.Groups[1].Value.Length;
                var int2 = reg.Groups[2].Value.Length;
                var res = int1 * int2;
                Console.WriteLine("...Teste auf unäre Multiplikation...");
                if (TM.Compute(input)) {
                    Console.WriteLine("Das Wort gehört zur Sprache. Berechnung fertig. Benötigte Schritte: " +
                                      TM.CalculatedSteps);
                } else {
                    Console.WriteLine("Das Wort gehört nicht zur Sprache. Berechnung fertig. Benötigte Schritte: " +
                                      TM.CalculatedSteps);
                }
            }
        }

        private static List<State> CreateStateNetwork(){
            var Q0 = new State(0);
            var Q1 = new State(1);
            var Q2 = new State(2);
            var Q3 = new State(3);
            var QE = new AcceptingState();

            // q0: Übertrage ersten Faktor auf Band 2
            Q0.AddConnection(Q0, t =>{ // 0**/_0*, RLS => q0
                                     if (t[0].Read().Equals('0')) {
                                         t[0].WriteRight();
                                         t[1].WriteLeft('0');
                                         return true;
                                     }
                                     return false;
                                 });
            Q0.AddConnection(Q1, t =>{ // 1**/_**,RSS => q1
                                     if (t[0].Read().Equals('1')) {
                                         t[0].WriteRight();
                                         t[1].WriteRight();
                                         return true;
                                     }
                                     return false;
                                 });

            // q1: Überprüfe, ob noch eine Stelle des ersten Faktors zu verarbeiten ist
            Q1.AddConnection(Q2, t =>{ // 0**/0**, RSS => q2
                                     if (t[0].Read().Equals('0')) {
                                         t[0].WriteRight('0');
                                         return true;
                                     }
                                     return false;
                                 });
            Q1.AddConnection(QE, t =>{ // _**/_**, SSS => qe
                                     if (t[0].Read().Equals(t[0].emptySlot)) {
                                         t[0].WriteRight('0');
                                         return true;
                                     }
                                     return false;
                                 });

            // q2: Addiere Band 2 auf Band 3
            Q2.AddConnection(Q2, t =>{ // *0*/*00,SRR => q2
                                     if (t[1].Read().Equals('0')) {
                                         t[1].WriteRight('0');
                                         t[2].WriteRight('0');
                                         return true;
                                     }
                                     return false;
                                 });
            Q2.AddConnection(Q3, t =>{ // *_*/*_*,SLS => q3
                                     if (t[1].Read().Equals(t[1].emptySlot)) {
                                         t[1].WriteLeft();
                                         return true;
                                     }
                                     return false;
                                 });

            // q3: Rücke zum Anfang des zweiten Faktors
            Q3.AddConnection(Q3, t =>{ // *0*/*0*,SLS => q3
                                     if (t[1].Read().Equals('0')) {
                                         t[1].WriteLeft('0');
                                         return true;
                                     }
                                     return false;
                                 });
            Q3.AddConnection(Q1, t =>{ // *_*/*_*,SRS => q1
                                     if (t[1].Read().Equals(t[1].emptySlot)) {
                                         t[1].WriteRight();
                                         return true;
                                     }
                                     return false;
                                 });

            return new[]{Q0, Q1, Q2, Q3, QE}.ToList();
        }
    }
}