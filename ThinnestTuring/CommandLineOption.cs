
using CommandLine;
using CommandLine.Text;

namespace ThinnestTuring
{
    public sealed class CommandLineOption
    {
        [Option('r', "run", HelpText = "Starts Turing machine in run mode")]
        public bool ArgRun { get; set; }
        [Option('s', "step", HelpText = "Starts Turing machine in step mode")]
        public bool ArgStep { get; set; }
        [Option('i', "input", HelpText = "Sets the LaTeX input file to parse the Turing machine from")]
        public string ArgInput { get; set; }
        [Option('o', "output", HelpText = "Sets the LaTeX output file to export the Turing machine to")]
        public string ArgOutput { get; set; }
    }
}