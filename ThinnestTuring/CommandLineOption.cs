using System;
using CommandLine;
using CommandLine.Text;

namespace ThinnestTuring
{
    public sealed class CommandLineOption
    {
        [Option('r', "run", HelpText = "Starts Turing machine in run mode")]
        public bool Run {get; set;}

        [Option('i', "input", HelpText = "Creates the Turing Machine like specified in the given LaTeX file")]
        public string TuringMachineLocation {get; set;}

        [Option('o', "output", DefaultValue = "output.tex",
            HelpText = "Sets the LaTeX output file to export the Turing machine to")]
        public string Output {get; set;}

        [ValueOption(0)]
        public string Word {get; set;}

        [HelpOption('h',"help")]
        public string GetUsage(){
            var help = new HelpText {
                Heading = new HeadingInfo("The Thinnest Turing", "v0.1"),
                Copyright = new CopyrightInfo("Raphael Emberger", 2016),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: {[-r|--run]|[(-i|--input) <filename>]|[(-o|--output) <filename>]} <inputword>");
            help.AddOptions(this);
            return help;
        }
    }
}