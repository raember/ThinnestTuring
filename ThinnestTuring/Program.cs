using System;
using System.Collections.Generic;
using System.Linq;
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
                var states = CreateUnaryMultiplicationStates();
                //var states = CreateStates();
                var TM = new TuringMachine(states.First(), 3);
                //TM.Mode = TuringMode.Run;
                var input = Console.ReadLine();
                Console.WriteLine("...Teste auf unäre Multiplikation...");
                var reg = Regex.Match(input, "(0*)1(0*)");
                var int1 = reg.Groups[1].Value.Length;
                var int2 = reg.Groups[2].Value.Length;
                var res = int1*int2;
                Console.WriteLine("Rechne {0}*{1}={2}", int1, int2, res);
                if (TM.Compute(input)) {
                    Console.WriteLine("Das Wort gehört zur Sprache. Berechnung fertig. Benötigte Schritte: " +
                                      TM.CalculatedSteps);
                    Console.WriteLine("Resultat: {0}", TM.tapes.Last().tape.Count);
                } else {
                    Console.WriteLine("Das Wort gehört nicht zur Sprache. Berechnung fertig. Benötigte Schritte: " +
                                      TM.CalculatedSteps);
                }
            }
        }

        private static List<State> CreateUnaryMultiplicationStates(){
            var Q0 = new State(0);
            var Q1 = new State(1);
            var Q2 = new State(2);
            var Q3 = new State(3);
            var QE = new AcceptingState();

            // q0: Übertrage ersten Faktor auf Band 2
            Q0.AddCondition("0**/_0*,RLS", Q0);
            Q0.AddCondition("1**/_**,RRS", Q1);

            // q1: Überprüfe, ob noch eine Stelle des ersten Faktors zu verarbeiten ist
            Q1.AddCondition("0**/0**,RSS", Q2);
            Q1.AddCondition("_**/_**,SSS", QE);

            // q2: Addiere Band 2 auf Band 3
            Q2.AddCondition("*0_/*00,SRR", Q2);
            Q2.AddCondition("*_*/*_*,SLS", Q3);

			// q3: Rücke zum Anfang des ersten Faktors
            Q3.AddCondition("*0*/*0*,SLS", Q3);
            Q3.AddCondition("*_*/*_*,SRS", Q1);
            
            return new[]{Q0, Q1, Q2, Q3, QE}.ToList();
        }

        //private static List<State> CreateStates(){
        //    var Q0 = new State(0);
        //    var Q1 = new State(1);
        //    var Q2 = new State(2);
        //    var Q3 = new State(3);
        //    var Q4 = new State(4);
        //    var Q5 = new State(5);
        //    var Q6 = new State(6);
        //    var Q7 = new State(7);
        //    var Q8 = new State(8);
        //    var Q9 = new State(9);
        //    var Q10 = new State(10);
        //    var QE = new AcceptingState();

        //    // q0
        //    Q0.AddCondition(Q1, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteRight('a');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q1
        //    Q1.AddCondition(Q2, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteRight('a');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });
        //    Q1.AddCondition(Q8, t =>{
        //                             if (t[0].Read().Equals(t[0].emptySlot)) {
        //                                 t[0].WriteLeft();
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q2
        //    Q2.AddCondition(Q2, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteRight('a');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });
        //    Q2.AddCondition(Q3, t =>{
        //                             if (t[0].Read().Equals(t[0].emptySlot)) {
        //                                 t[0].WriteLeft();
        //                                 return true;
        //                             }
        //                             if (t[0].Read().Equals('t')) {
        //                                 t[0].WriteLeft('t');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q3
        //    Q3.AddCondition(Q4, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteLeft('t');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q4
        //    Q4.AddCondition(Q4, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteLeft('a');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });
        //    Q4.AddCondition(Q5, t =>{
        //                             if (t[0].Read().Equals(t[0].emptySlot)) {
        //                                 t[0].WriteRight();
        //                                 return true;
        //                             }
        //                             if (t[0].Read().Equals('t')) {
        //                                 t[0].WriteRight('t');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q5
        //    Q5.AddCondition(Q6, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteRight('t');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q6
        //    Q6.AddCondition(Q7, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteRight('a');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q7
        //    Q7.AddCondition(Q2, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteLeft('a');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });
        //    Q7.AddCondition(Q8, t =>{
        //                             if (t[0].Read().Equals('t')) {
        //                                 t[0].WriteLeft('t');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q8
        //    Q8.AddCondition(Q9, t =>{
        //                             if (t[0].Read().Equals('a')) {
        //                                 t[0].WriteRight('x');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });

        //    // q9
        //    Q9.AddCondition(Q9, t =>{
        //                             if (t[0].Read().Equals('t')) {
        //                                 t[0].WriteRight('t');
        //                                 return true;
        //                             }
        //                             return false;
        //                         });
        //    Q9.AddCondition(Q10, t =>{
        //                              if (t[0].Read().Equals(t[0].emptySlot)) {
        //                                  t[0].WriteLeft();
        //                                  return true;
        //                              }
        //                              return false;
        //                          });

        //    // q10
        //    Q10.AddCondition(Q10, t =>{
        //                               if (t[0].Read().Equals('x')) {
        //                                   t[0].WriteLeft('x');
        //                                   return true;
        //                               }
        //                               if (t[0].Read().Equals('t')) {
        //                                   t[0].WriteLeft('a');
        //                                   return true;
        //                               }
        //                               return false;
        //                           });
        //    Q10.AddCondition(QE, t =>{
        //                              if (t[0].Read().Equals(t[0].emptySlot)) {
        //                                  t[0].WriteRight();
        //                                  return true;
        //                              }
        //                              return false;
        //                          });
        //    return new[]{Q0, Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9, Q10, QE}.ToList();
        //}
    }
}