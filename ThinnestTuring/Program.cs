using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;

namespace ThinnestTuring
{
    internal class MainClass
    {
        private static TuringMachine TM = new TuringMachine();
        private static TuringMode mode = TuringMode.Step;

        public static void Main(string[] args){
//            MainAsync(args).Wait();
//        }
//
//        public static async Task MainAsync(string[] args){
            var loop = true;
            var cmndArgs = args.Any();
            var inputWord = string.Empty;
            var turMachInput = string.Empty;
            var outputLoc = string.Empty;


            //Consume possible commandline inputs
            var cmnd = new CommandLineOption();
            if (!Parser.Default.ParseArguments(args, cmnd)) {
                Environment.Exit(Parser.DefaultExitCodeFail);
            }
            if (cmnd.Run) { mode = TuringMode.Run; }
            outputLoc = cmnd.Output;
            turMachInput = cmnd.TuringMachineLocation;
            inputWord = cmnd.Word;

            var loopOnlyOnce = cmndArgs;

            //Setup
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.WriteLine("THE [THIN]NEST TURING");
            Console.WriteLine("=====================");
            if (!cmndArgs) {
                mode = ChooseMode();
                turMachInput = SpecifyInput();
                outputLoc = SpecifyOutput();
            }

            //Loop as long as the user wants to compute words. Except if the word was given in the arguments.
            while (loop) {
                if (string.IsNullOrWhiteSpace(turMachInput)) {
                    //No input file specified. Create the standard Turing Machine.
                    TM = new TuringMachine();
                    CreateOneTapeUnaryMultiplicationStates(TM);
                } else {
                    TM = TuringMachine.FromLaTeXDocument(turMachInput);
                }
                TM.Mode = mode;
                TM.Initialize();

                if (string.IsNullOrEmpty(inputWord)) {
                    Console.WriteLine("Please enter a word.");
                    inputWord = Console.ReadLine();
                }
                if ("exit".Equals(inputWord.Trim()) || "quit".Equals(inputWord.Trim())) { break; }


                //Do the computation
                //var valid = await TM.ComputeAsync(inputWord);
                var valid = TM.Compute(inputWord);

                const int width = 25;
                Console.WriteLine();
                Console.Write("Valid word:".PadRight(width));
                Console.ForegroundColor = valid ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(valid);
                Console.ResetColor();
                Console.WriteLine("Steps:".PadRight(width) + TM.CalculatedSteps);

                #region "MultiplicationOutput"
                if (valid)
                {
                    var reg = Regex.Match(inputWord, "^(0*)1(0*)$");
                    var int1 = reg.Groups[1].Value.Length;
                    var int2 = reg.Groups[2].Value.Length;
                    var res = int1 * int2;
                    var resultCalc = TM.Tapes.Last().tape.Count(c => c.Equals('0'));
                    var match = (resultCalc == res);
                    Console.WriteLine("Attempted calculation:".PadRight(width) + "{0}*{1}={2}", int1, int2, res);
                    Console.WriteLine("Calculated result:".PadRight(width) + resultCalc);
                    Console.Write("Match:".PadRight(width));
                    Console.ForegroundColor = match ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine(match);
                    Console.ResetColor();
                }
                #endregion

                inputWord = string.Empty;
                loop = !loopOnlyOnce;
            }
            if (!string.IsNullOrWhiteSpace(outputLoc)) { TM.ToLaTeXDocument(outputLoc); }
        }

        private static TuringMode ChooseMode(){
            Console.WriteLine("Please select mode [s/r]:");
            while (true) {
                var modeInput = Console.ReadLine();
                if (string.IsNullOrEmpty(modeInput)) { modeInput = string.Empty; }
                switch (modeInput.ToLower()) {
                    case "r":
                    case "run":
                        Console.WriteLine("Entered RUN mode.");
                        return TuringMode.Run;
                    case "s":
                    case "step":
                        Console.WriteLine("Entered STEP mode.");
                        return TuringMode.Step;
                    default:
                        Console.WriteLine("Please select mode [s/r]:");
                        break;
                }
            }
        }

        private static string SpecifyOutput(){
            Console.WriteLine("Please write the path to the output file(leave empty for no output):");
            var outputLoc = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(outputLoc)) {
                Console.WriteLine("No output file will be created.");
            } else {
                Console.WriteLine("The output will be written into \"{0}\".", outputLoc);
            }
            return outputLoc;
        }

        private static string SpecifyInput(){
            Console.WriteLine("Please write the path to the input file(leave empty for no input):");
            var turMachInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(turMachInput)) {
                Console.WriteLine("No input file will be created. The standard Turing Machine will be created.");
            } else {
                Console.WriteLine("The Turing Machine will be created like specified in \"{0}\".", turMachInput);
            }
            return turMachInput;
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

        private static void CreateOneTapeUnaryMultiplicationStates(TuringMachine tm){
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
            var Q11 = tm.CreateState();
            var QE1 = tm.CreateAcceptingState();
            var QE2 = tm.CreateAcceptingState();

            Q0.AddTransition("0/_,R", Q1);
            Q0.AddTransition("1/_,R", QE1);

            Q1.AddTransition("0/_,R", Q2);
            Q1.AddTransition("1/_,R", QE2);

            Q2.AddTransition("0/0,R");
            Q2.AddTransition("1/1,R", Q3);

            Q3.AddTransition("X/X,R");
            Q3.AddTransition("1/1,R");
            Q3.AddTransition("_/_,L", Q10);
            Q3.AddTransition("0/0,R", Q11);

            Q11.AddTransition("0/0,R");
            Q11.AddTransition("X/X,L", Q10);
            Q11.AddTransition("_/_,L", Q10);

            Q10.AddTransition("0/X,R", Q4);

            Q4.AddTransition("X/X,R");
            Q4.AddTransition("0/0,R");
            Q4.AddTransition("_/0,L", Q5);

            Q5.AddTransition("0/0,L");
            Q5.AddTransition("X/X,L", Q6);

            Q6.AddTransition("X/X,L");
            Q6.AddTransition("0/0,L", Q3);
            Q6.AddTransition("1/1,L", Q7);

            Q7.AddTransition("0/0,L");
            Q7.AddTransition("_/_,R", Q1);
        }
    }
}