/*
MIT License
Copyright (c) 2016 Marco Silipo (X39)
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;


namespace CommandLineParameterParser
{
    public class Parser
    {
        private IEnumerable<Command> CmdList;
        private TextWriter Out;
        public Parser(IEnumerable<Command> CmdList)
        {
            this.CmdList = CmdList;
            this.Out = Console.Out;
        }
        public Parser(IEnumerable<Command> CmdList, TextWriter StdOut)
        {
            this.CmdList = CmdList;
            this.Out = StdOut;
        }


        /// <summary>
        /// Displays help text on output stream
        /// </summary>
        public void DisplayHelp()
        {
            this.Out.WriteLine("Available flags:");
            this.Out.WriteLine("\thelp\tDisplays this help text");
            this.Out.WriteLine("\t?\tDisplays this help text");
            foreach (var cmd in this.CmdList)
            {
                if (cmd.Kind != Command.EKind.Flag)
                    continue;
                this.Out.WriteLine(string.Concat('\t', cmd.Name, '\t', cmd.Description));
            }
            this.Out.WriteLine("Available Properties:");
            foreach (var cmd in this.CmdList)
            {
                if (cmd.Kind != Command.EKind.Property)
                    continue;
                if(string.IsNullOrWhiteSpace(cmd.DefaultValue))
                {
                    this.Out.WriteLine(string.Concat('\t', cmd.Name, '\t', cmd.Description));
                }
                else
                {
                    this.Out.WriteLine(string.Concat('\t', cmd.Name, "(=", cmd.DefaultValue, ')', '\t', cmd.Description));
                }
            }
        }

        /// <summary>
        /// Checks provided arguments for commands and executes them.
        /// </summary>
        /// <param name="args">string array passed to the tool</param>
        public void Check(string[] args)
        {
            var enumerator = args.GetEnumerator();
            List<Command> PropertyCommands = new List<Command>();
            foreach(var it in this.CmdList)
            {
                if(it.Kind == Command.EKind.Property && !string.IsNullOrWhiteSpace(it.DefaultValue))
                {
                    PropertyCommands.Add(it);
                }
            }
            while (enumerator.MoveNext())
            {
                string arg = enumerator.Current as string;
                if (string.IsNullOrWhiteSpace(arg))
                    continue;
                char firstChar = arg[0];
                bool hadHit = false;
                foreach (var cmd in this.CmdList)
                {
                    switch (cmd.Kind)
                    {
                        case Command.EKind.Flag:
                            {
                                if (firstChar != '-' && firstChar != '/')
                                    continue;
                                if(arg.Substring(1).Equals(cmd.Name))
                                {
                                    cmd.Action(this.Out, string.Empty);
                                    hadHit = true;
                                }
                            }
                            break;
                        case Command.EKind.Path:
                            {
                                if (firstChar == '-' || firstChar == '/')
                                    continue;
                                cmd.Action(this.Out, arg);
                                hadHit = true;
                            }
                            break;
                        case Command.EKind.Property:
                            {
                                if (firstChar != '-' && firstChar != '/')
                                    continue;
                                if (arg.Substring(1).Equals(cmd.Name))
                                {
                                    enumerator.MoveNext();
                                    if(PropertyCommands.Contains(cmd))
                                    {
                                        PropertyCommands.Remove(cmd);
                                    }
                                    cmd.Action(this.Out, enumerator.Current as string);
                                    hadHit = true;
                                }
                            }
                            break;
                        default: throw new NotImplementedException();
                    }
                    if (hadHit)
                        break;
                }
                if(!hadHit)
                {
                    if(arg.Substring(1).Equals("help") || arg.Substring(1).Equals("?"))
                    {
                        this.DisplayHelp();
                    }
                }
            }
            foreach (var cmd in PropertyCommands)
            {
                cmd.Action(this.Out, cmd.DefaultValue);
            }
        }
    }
}
