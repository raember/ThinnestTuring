using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThinnestTuring
{
    internal class MainClass
    {
        private static TuringMachine TM = new TuringMachine();
        private static bool modeSet;
        private static TuringMode mode = TuringMode.Step;
        private static bool latexOutputSet;

        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args){
            var inputWord = string.Empty;
            var outputLaTeX = false;
            var loop = true;
            var loopOnlyOnce = false;
            foreach (var arg in args) {
                switch (arg) {
                    case "-s":
                        EnterStepMode();
                        break;
                    case "-r":
                        EnterRunMode();
                        break;
                    case "-latex":
                        outputLaTeX = true;
                        latexOutputSet = true;
                        break;
                    default:
                        latexOutputSet = true;
                        inputWord = arg;
                        break;
                }
                loopOnlyOnce = true;
            }
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("THE THINNEST TURING");
            Console.WriteLine("===================");
            Console.ResetColor();
            if (!modeSet) {
                Console.WriteLine("Please select mode [s/r]:");
                while (!modeSet) {
                    var modeInput = Console.ReadLine();
                    if (string.IsNullOrEmpty(modeInput)){modeInput = string.Empty;}
                    switch (modeInput.ToLower()) {
                        case "r":
                            EnterRunMode();
                            Console.WriteLine("Entered RUN mode.");
                            break;
                        case "s": case "step":
                            EnterStepMode();
                            Console.WriteLine("Entered STEP mode.");
                            break;
                        default:
                            Console.WriteLine("Please select mode [s/r]:");
                            break;
                    }
                }
            }
            var correctOutput = latexOutputSet;
            if (!latexOutputSet) {
				Console.WriteLine("Do you wish a LaTeX output?(writes into \"output.tex\") [y/n]:");
                while (!correctOutput) {
                    var modeInput = Console.ReadLine().ToLower();
                    correctOutput = true;
                    switch (modeInput) {
                        case "y":
                            outputLaTeX = true;
                            latexOutputSet = true;
                            break;
                        case "n":
                            latexOutputSet = true;
                            break;
                        default:
                            Console.WriteLine("Do you wish a LaTeX output?(writes into \"output.tex\") [y/n]:");
                            correctOutput = false;
                            break;
                    }
                }
            }
            while (loop) {
                if (string.IsNullOrEmpty(inputWord)) { Console.WriteLine("Please enter a word."); }
                TM = new TuringMachine();
                TM.Mode = mode;
                CreateUltimateUnaryMultiplicationStates(TM);
                TM.Initialize();

                if (string.IsNullOrEmpty(inputWord)) { inputWord = Console.ReadLine(); }
                if ("exit".Equals(inputWord) || "quit".Equals(inputWord)) { break; }
                var reg = Regex.Match(inputWord, "(0*)1(0*)");
                var int1 = reg.Groups[1].Value.Length;
                var int2 = reg.Groups[2].Value.Length;
                var res = int1*int2;

                var valid = await TM.ComputeAsync(inputWord);
                Console.WriteLine();
                var resultCalc = TM.Tapes.Last().tape.Count(c => c.Equals('0'));
                var match = (resultCalc == res);
                const int width = 25;
                var whitespaceOffset = int1.ToString().Length + int2.ToString().Length + 2;
                Console.Write("Valid word:".PadRight(width + whitespaceOffset));
                Console.ForegroundColor = valid ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(valid);
                Console.ResetColor();
                Console.WriteLine("Steps:".PadRight(width + whitespaceOffset) + TM.CalculatedSteps);
                Console.WriteLine("Attempted calculation:".PadRight(width) + "{0}*{1}={2}", int1, int2, res);
                Console.WriteLine("Calculated result:".PadRight(width + whitespaceOffset) + resultCalc);
                Console.Write("Match:".PadRight(width + whitespaceOffset));
                Console.ForegroundColor = match ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(match);
                Console.ResetColor();
                inputWord = string.Empty;
                loop = !loopOnlyOnce;
            }
			if (outputLaTeX) { TM.ToLaTeXDocument(); }
        }

        private static void EnterStepMode()
        {
            mode = TuringMode.Step;
            modeSet = true;
        }

        private static void EnterRunMode()
        {
            mode = TuringMode.Run;
            modeSet = true;
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

        private static void CreateBestUnaryMultiplicationStates(TuringMachine tm){
            var Q0 = tm.CreateState();
            var Q1OnePlusTimesX = tm.CreateState();
            var Q2OneTimesX = tm.CreateState();
            var Q3YTimesX = tm.CreateState();
            var Q4YTimesX = tm.CreateState();
            var Q5 = tm.CreateState();
            var Q6YTimesOne = tm.CreateState();
            var Q7 = tm.CreateState();
            var Q8 = tm.CreateState();
            var Q9 = tm.CreateState();
            var Q10 = tm.CreateState();
            var QE = tm.CreateAcceptingState();

            //0*x=0
            Q0.AddTransition("1**/1**,SSS", QE);

            //1*x=x
            Q0.AddTransition("0**/_1*,RLS", Q1OnePlusTimesX);
            Q1OnePlusTimesX.AddTransition("1**/_**,RSL", Q2OneTimesX);
            Q2OneTimesX.AddTransition("0**/0*0,RSL");
            Q2OneTimesX.AddTransition("_**/_**,SSR", QE);

            //y*x  --  1 not yet consumed.
            Q1OnePlusTimesX.AddTransition("0**/_0*,RLS", Q3YTimesX);
            Q3YTimesX.AddTransition("0**/_0*,RLS");
            Q3YTimesX.AddTransition("1**/_**,RRS", Q4YTimesX);

            //y*0=0
            Q4YTimesX.AddTransition("_**/_**,SSS", QE);

            //y*(1+?)
            Q4YTimesX.AddTransition("00*/010,RRL", Q5);

            //y*1=y
            Q5.AddTransition("_**/_**,SSS", Q6YTimesOne);
            Q6YTimesOne.AddTransition("*0*/*00,SRL");
            Q6YTimesOne.AddTransition("*1*/*10,SRL", QE);

            //Trivial cases covered
            //y*x| y>1 & x>1
            Q5.AddTransition("00*/000,SRL");
            Q5.AddTransition("01*/010,SSL", Q9);
            Q7.AddTransition("01*/010,RRL", Q8);
            Q7.AddTransition("_1*/_1*,SRR", QE);
            Q8.AddTransition("*0*/*00,SRL");
            Q8.AddTransition("*1*/*10,SSL", Q9);
            Q9.AddTransition("01*/010,RLL", Q10);
            Q9.AddTransition("_1*/_1*,SRR", QE);
            Q10.AddTransition("*0*/*00,SLL");
            Q10.AddTransition("01*/*10,SSL", Q7);
            Q10.AddTransition("_1*/_10,SRL", QE);
        }

        private static void CreateUltimateUnaryMultiplicationStates(TuringMachine tm){
            var Q0 = tm.CreateState();
            var Q1 = tm.CreateState();
            var Q2 = tm.CreateState();
            var Q3 = tm.CreateState();
            var Q4 = tm.CreateState();
            var Q5 = tm.CreateState();
            var Q6 = tm.CreateState();
            var Q7 = tm.CreateState();
            var Q8 = tm.CreateState();
            var Q9 = tm.CreateState();
            var Q10 = tm.CreateState();
            var QE = tm.CreateAcceptingState();

            //0*x=0
            Q0.AddTransition("1**/_**,RSS", Q1);
            Q1.AddTransition("0**/0**,RSS");
            Q1.AddTransition("_**/_**,SSS", QE);

            //1*x=x
            Q0.AddTransition("0**/_1*,RLS", Q2);
            Q2.AddTransition("1**/_**,RSS", Q3);
            Q3.AddTransition("0**/0*0,RSL");
            Q3.AddTransition("_**/_**,SRR", QE);

            //y*x  --  1 not yet consumed.
            Q2.AddTransition("0**/_00,RLL", Q4);
            Q4.AddTransition("0**/_00,RLL");

            //y*0=0
            Q4.AddTransition("1**/_*0,RRL", Q5);
            Q5.AddTransition("_**/_**,SSR", Q6);
            Q6.AddTransition("**0/**_,SSR");
            Q6.AddTransition("**_/**_,SSS", QE);

            //y*(1+?)
            Q5.AddTransition("00*/01*,RSS", Q7);

            //y*1=y
//            Q5.AddTransition("_**/_**,SSS", Q6);
//            Q6.AddTransition("*0*/*00,SRL");
//            Q6.AddTransition("*1*/*10,SRS", QE);

            //Trivial cases covered
            //y*x| y>1 & x>1
//            Q5.AddTransition("00*/000,SRL");
//            Q5.AddTransition("01*/010,SSL", Q9);
            Q7.AddTransition("01*/010,RRL", Q8);
            Q7.AddTransition("_1*/_1*,SSR", QE);
            Q8.AddTransition("*0*/*00,SRL");
            Q8.AddTransition("01*/010,SSL", Q9);
            Q8.AddTransition("_1*/_10,SRS", QE);
            Q9.AddTransition("01*/010,RLL", Q10);
            Q9.AddTransition("_1*/_1*,SRR", QE);
            Q10.AddTransition("*0*/*00,SLL");
            Q10.AddTransition("01*/*10,SSL", Q7);
            Q10.AddTransition("_1*/_10,SSS", QE);
        }
    }
}