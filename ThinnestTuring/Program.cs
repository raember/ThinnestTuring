using System;
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
            Console.WriteLine("===============");
            Console.WriteLine();
            Console.WriteLine("Bitte wählen zwischen Step-Modus[s] und Run-Modus[r]:");
            var doRun = false;
            var correctInput = false;
            while (!correctInput) {
                var modeInput = Console.ReadLine().ToLower();
                correctInput = true;
                switch (modeInput) {
                    case "r":
                        doRun = true;
                        Console.WriteLine("RUN-Modus gewählt.");
                        break;
                    case "s":
                        Console.WriteLine("STEP-Modus gewählt.");
                        break;
                    default:
                        correctInput = false;
                        Console.WriteLine("Bitte entweder [s] oder [r] eingeben:");
                        break;
                }
            }
            while (true) {
                Console.WriteLine("Bitte ein Wort angeben.");
                //var states = CreateStates();
                var TM = new TuringMachine();
                if (doRun) { TM.Mode = TuringMode.Run; }
                CreateImprovedUnaryMultiplicationStates(TM);
                TM.Initialize();
                //TM.Mode = TuringMode.Run;
                var input = Console.ReadLine();
                if ("exit".Equals(input) || "quit".Equals(input)) { return; }
                Console.WriteLine("...Teste auf unäre Multiplikation...");
                var reg = Regex.Match(input, "(0*)1(0*)");
                var int1 = reg.Groups[1].Value.Length;
                var int2 = reg.Groups[2].Value.Length;
                var res = int1*int2;
                Console.WriteLine("Rechne {0}*{1}={2}", int1, int2, res);
                if (TM.Compute(input)) {
                    Console.WriteLine("Das Wort gehört zur Sprache. Berechnung fertig. Benötigte Schritte: " +
                                      TM.CalculatedSteps);
                    var resultCalc = TM.Tapes.Last().tape.Count(c => c.Equals('0'));
                    Console.WriteLine("Resultat: {0}, Erwartet: {1}, Match: {2}", resultCalc, res, resultCalc == res);
                    Console.WriteLine("===============");
                    Console.WriteLine(TM.ToLaTeX());
                    Console.WriteLine("===============");
                } else {
                    Console.WriteLine("Das Wort gehört nicht zur Sprache. Berechnung fertig. Benötigte Schritte: " +
                                      TM.CalculatedSteps);
                }
            }
        }

        private static void CreateUnaryMultiplicationStates(TuringMachine tm){
            var Q0 = tm.CreateState();
            var Q1 = tm.CreateState();
            var Q2 = tm.CreateState();
            var Q3 = tm.CreateState();
            var QE = tm.CreateAcceptingState();

            // q0: Übertrage ersten Faktor auf Band 2
            Q0.AddTransition("0**/_0*,RLS", Q0);
            Q0.AddTransition("1**/_**,RRS", Q1);

            // q1: Überprüfe, ob noch eine Stelle des ersten Faktors zu verarbeiten ist
            Q1.AddTransition("0**/0**,RSS", Q2);
            Q1.AddTransition("_**/_**,SSS", QE);

            // q2: Addiere Band 2 auf Band 3
            Q2.AddTransition("*0_/*00,SRR", Q2);
            Q2.AddTransition("*_*/*_*,SLS", Q3);

            // q3: Rücke zum Anfang des ersten Faktors
            Q3.AddTransition("*0*/*0*,SLS", Q3);
            Q3.AddTransition("*_*/*_*,SRS", Q1);
        }

        private static void CreateImprovedUnaryMultiplicationStates(TuringMachine tm){
            var Q0 = tm.CreateState();
            var Q1OnePlusTimesX = tm.CreateState();
            var Q2OneTimesX = tm.CreateState();
            var Q3YTimesX = tm.CreateState();
            var Q4YTimesX = tm.CreateState();
            var Q5 = tm.CreateState();
            var Q6YTimesOne = tm.CreateState();
            var Q7FromLeft = tm.CreateState();
            var Q8FromRight = tm.CreateState();
            var QE = tm.CreateAcceptingState();

            //0*x=0
            Q0.AddTransition("1**/1**,SSS", QE);

            //1*x=x
            Q0.AddTransition("0**/_0*,RLS", Q1OnePlusTimesX);
            Q1OnePlusTimesX.AddTransition("1**/_**,RSL", Q2OneTimesX);
            Q2OneTimesX.AddTransition("0**/0*0,RSL");
            Q2OneTimesX.AddTransition("_**/_**,SSR", QE);

            //y*x  --  1 not yet consumed.
            Q1OnePlusTimesX.AddTransition("0**/_0*,RLS", Q3YTimesX);
            Q3YTimesX.AddTransition("0**/_0*,RLS");
            Q3YTimesX.AddTransition("1**/_**,RSS", Q4YTimesX);

            //y*0=0
            Q4YTimesX.AddTransition("_**/_**,SSS", QE);

            //y*(1+?)
            Q4YTimesX.AddTransition("0_*/0_*,RRS", Q5);

            //y*1=y
            Q5.AddTransition("_**/_**,SSS", Q6YTimesOne);
            Q6YTimesOne.AddTransition("*0*/*00,SRL");
            Q6YTimesOne.AddTransition("*_*/*_*,SLR", QE);

            //Trivial cases covered
            //y*x| y>1 & x>1

            Q5.AddTransition("*0*/*00,SRL", Q7FromLeft);

            //Left 2 right
            Q7FromLeft.AddTransition("__*/__*,SSR", QE);
            Q7FromLeft.AddTransition("*0*/*00,SRL");
            Q7FromLeft.AddTransition("0_*/0_*,RLS", Q8FromRight);

            //Right 2 left
            Q8FromRight.AddTransition("__*/__*,SSR", QE);
            Q8FromRight.AddTransition("*0*/*00,SLL");
            Q8FromRight.AddTransition("0_*/0_*,RRS", Q7FromLeft);
        }
    }
}