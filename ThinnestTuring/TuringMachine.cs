using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace ThinnestTuring
{
    public class TuringMachine
    {
        private bool checkedTransitions;

        public TuringMachine(){
            Mode = TuringMode.Step;
            Tapes = new List<TuringTape>();
            States = new List<State>();
        }

        public List<TuringTape> Tapes {get;}
        public int CalculatedSteps {get; private set;}
        public TuringMode Mode {get; set;}
        public List<State> States {get;}
        public State CurrentState {get; set;}

        public bool Initialize(){
            var amountOfTapes = CurrentState.AmountOfTapes;
            if (States.All(s => s.AmountOfTapes == amountOfTapes || s.AmountOfTapes == 0)) {
                for (var i = 0; i < amountOfTapes; i++) { Tapes.Add(new TuringTape()); }
                checkedTransitions = true;
            }
            return checkedTransitions;
        }

        public bool Compute(string word){
            return Compute(word.ToList());
        }

        public async Task<bool> ComputeAsync(List<char> word){
            return await Task.Run(() => { return Compute(word); });
        }

        public async Task<bool> ComputeAsync(string word){
            return await ComputeAsync(word.ToCharArray().ToList());
        }

        public bool Compute(List<char> word){
            if (!checkedTransitions) { throw new Exception("I'm not prepared yet."); }
            var tape = Tapes.First();
            if (word.Any()) {
                word.Reverse();
                word.GetRange(0, word.Count - 1).ForEach(c => tape.WriteLeft(c));
                tape.WriteStay(word.Last());
            }
            if (Mode == TuringMode.Step) { print(false); }
            while (CurrentState.CanStep(Tapes)) {
                CurrentState = CurrentState.GetNextState(Tapes);
                CalculatedSteps++;
                if (Mode == TuringMode.Step) {
                    print(false);
                    //Console.ReadKey();
                }
            }
            var accepted = (CurrentState is AcceptingState);
            if (!accepted) { CurrentState = new DroppingState(int.MinValue); }
            ;
            print(true);
            return (accepted);
        }

        public State CreateState(){
            var newState = new State(States.Count(s => s is State));
            if (CurrentState == null) { CurrentState = newState; }
            States.Add(newState);
            return newState;
        }

        public AcceptingState CreateAcceptingState(){
            var newState = new AcceptingState(-1 - States.Count(s => s is AcceptingState));
            if (CurrentState == null) { CurrentState = newState; }
            States.Add(newState);
            return newState;
        }

        public string ToLaTeX(){
			var str = new List<string>();
			var strStates = recursiveStateToLaTeX(States.First());

			str.Add("\\begin{tikzpicture}[shorten >=0.5pt,node distance=3.5cm,>=latex,semithick,bend angle=15,every node/.style={font=\\footnotesize}]");
			str.AddRange(strStates);
            str.Add(string.Empty);
            str.Add("\\path[->]");
            var trans = new List<Transition>();
            foreach (var s in States) { trans.AddRange(s.Transitions); }
            str.AddRange(trans.Distinct().ToList().Select(t => t.ToLaTeX()));
            str.Add(";");
			str.Add("\\end{tikzpicture}");
            return string.Join("\n", str);
		}

		private List<string> recursiveStateToLaTeX(State from) {
			var str = new List<string>();
			var transitions = from.Transitions.Where(t => !t.FromState.Equals(t.ToState)).ToList();
			var transEn = transitions.GetEnumerator();
			var count = transitions.Count;
			if (!transitions.Any()) {
				return str;
			}
			while (transEn.MoveNext()) {
				var to = transEn.Current.ToState;
				switch (count) {
					case 1:
						str.Add(to.ToLaTeX(from, "right"));
						break;
					case 2:
						str.Add(to.ToLaTeX(from, "above right"));
						break;
					case 3:
						str.Add(to.ToLaTeX(from, "below right"));
						break;
					case 4:
						str.Add(to.ToLaTeX(from, "above"));
						break;
					case 5:
						str.Add(to.ToLaTeX(from, "below"));
						break;
					case 6:
						str.Add(to.ToLaTeX(from, "above left"));
						break;
					case 7:
						str.Add(to.ToLaTeX(from, "below left"));
						break;
					default:
						str.Add(to.ToLaTeX(from, "right"));
						break;
				}
				str.AddRange(recursiveStateToLaTeX(to));
				count--;
			}
			return str;
		}

		public void ToLaTeXDocument(string filename = "output.tex") {
			var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().FullName), filename);
			using (var strWrtr = new StreamWriter(new FileStream(path, FileMode.Create))) {
				strWrtr.WriteLine("\\documentclass{standalone}");

				strWrtr.WriteLine("\\usepackage[utf8]{inputenc}");
				strWrtr.WriteLine("\\usepackage[german]{babel}");
				strWrtr.WriteLine("\\usepackage[T1]{fontenc}");
				strWrtr.WriteLine("\\usepackage{amsmath}");
				strWrtr.WriteLine("\\usepackage{amsfonts}");
				strWrtr.WriteLine("\\usepackage{amssymb}");
				strWrtr.WriteLine("\\usepackage{graphicx}");
				strWrtr.WriteLine("\\usepackage{pifont}");
				strWrtr.WriteLine("\\usepackage[normalem]{ulem}");
				strWrtr.WriteLine("\\usepackage{tikz}");
				strWrtr.WriteLine("\\usetikzlibrary{automata,arrows,decorations.pathreplacing}");
				strWrtr.WriteLine("\\usepackage{tikz}");
				strWrtr.WriteLine("\\tikzstyle{automat}=[>=triangle 45,node distance=5cm,auto]");
				strWrtr.WriteLine("\\tikzstyle{every initial by arrow}=[initial text={}]");
				strWrtr.WriteLine("\\tikzstyle{every state}=[semithick]");
				strWrtr.WriteLine("\\tikzstyle{accepting by double}=[double distance=.5mm,outer sep=.3pt+.25mm]");
				strWrtr.WriteLine("\\tikzstyle{state}=[draw,circle,fill=white,minimum size=23pt,font=\\small]");
				strWrtr.WriteLine("\\begin{document}");
				strWrtr.WriteLine(ToLaTeX());
				strWrtr.Write("\\end{document}");
			}
		}

        public void print(bool final){
            var tapeN = 0;
            if (Tapes.Count > 1 && !final && Mode != TuringMode.Run) {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(new string('=', 2*TuringTape.TAPEPRINTOVERHEAD + 8
                                                  + tapeN.ToString().Length + CurrentState.ToString().Length));
            }
            if (final && Mode != TuringMode.Run) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (Tapes.Count > 1) {
                    Console.WriteLine(new string('=', 2*TuringTape.TAPEPRINTOVERHEAD + 9
                                                      + CurrentState.ToString().Length));
                } else {
                    Console.WriteLine(new string('=', 2*TuringTape.TAPEPRINTOVERHEAD + 6
                                                      + tapeN.ToString().Length + CurrentState.ToString().Length));
                }
                Console.ResetColor();
            }
            foreach (var tape in Tapes) {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (Tapes.Count > 1) { Console.Write("Tape " + tapeN + ":"); }
                Console.Write("[");
                tape.print(CurrentState, final);
                Console.WriteLine("]");
                tapeN++;
            }
            Console.ResetColor();
        }
    }

    public enum TuringMode
    {
        Step,
        Run
    }
}