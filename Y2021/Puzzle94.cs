using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

// Keeping the ALU from Puzzle 24 because it's cute and fun
// Ended up solving Puzzzle 24 a completely different way

namespace AdventOfCode.Y2021
{
    public class Puzzle94 : ASolver 
    {
        private Program program;
        private Program[] programs;

        private class Program
        {
            public List<Instruction> Instructions;

            public Program()
            {
                Instructions = new();
            }
        }

        // Added new "set" instruction to replace mul 0 instructions
        private class Instruction
        {
            public const int OperandIsLiteral = -1;
            public static string Comment = "//";

            public string Operation;
            public int DestRegister;
            public int SourceRegister;
            public int SourceLiteral;

            public Instruction(string line)
            {
                string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                Operation = parts[0];
                if (Operation == Comment)
                    return;

                DestRegister = ALU.RegisterIndexOf(parts[1][0]);
                if (IsThreeOperandInstruction(Operation))
                {
                    int literal = 0;
                    if (int.TryParse(parts[2], out literal))
                    {
                        SourceRegister = OperandIsLiteral;
                        SourceLiteral = literal;
                    }
                    else
                    {
                        SourceRegister = ALU.RegisterIndexOf(parts[2][0]);
                        SourceLiteral = 0;
                    }
                    OptimizeMul0();
                    OptimizeNoops();
                }
            }

            // Change mul 0 instructions to set 0
            private void OptimizeMul0()
            {
                if ((Operation == "mul") && (SourceRegister == OperandIsLiteral) && (SourceLiteral == 0))
                    Operation = "set";
            }

            // Eliminate add 0, mul 1, div 1, and mod 1 instructions
            private void OptimizeNoops()
            {
                if (SourceRegister == OperandIsLiteral)
                {
                    if (SourceLiteral == 0)
                    {
                        if (Operation == "add")
                            Operation = "nop";
                    }
                    else if (SourceLiteral == 1)
                    {
                        if ((Operation == "mul") || (Operation == "div") || (Operation == "mod"))
                            Operation = "nop";
                    }          
                }
            }

            public override string ToString()
            {
                if (Operation == Comment)
                    return Comment;
                else
                {
                    string instruction = $"{Operation} {ALU.RegisterNameOf(DestRegister)} ";
                    if (IsThreeOperandInstruction(Operation))
                        instruction += (SourceRegister == OperandIsLiteral) ? SourceLiteral.ToString() : ALU.RegisterNameOf(SourceRegister).ToString();
                    return instruction;
                }
            }

            public static bool IsThreeOperandInstruction(string operation) => operation != "inp";

        }

        private class ALU
        {
            public const char FirstRegisterName = 'w';
            public const int NumRegisters = 4;
            public const int InitialRegisterValue = 0;

            public long[] Registers;
            public int InputIndex;
            public string CumulativeInput;

            public ALU()
            {
                Registers = new long[NumRegisters];
                Reset();
            }

            public ALU(ALU alu)
            {
                Registers = new long[NumRegisters];
                Array.Copy(alu.Registers, Registers, NumRegisters);
                InputIndex = alu.InputIndex;
                CumulativeInput = alu.CumulativeInput;
            }

            private void Reset()
            {
                for (int i = 0; i < NumRegisters; i++)
                    Registers[i] = InitialRegisterValue;
                InputIndex = 0;
                CumulativeInput = string.Empty;
            }

            public long Execute(Program program, string input, char outputRegister)
            {
                Reset();
                foreach (Instruction instruction in program.Instructions)
                {
                    switch (instruction.Operation)
                    {
                        case "inp":
                            Registers[instruction.DestRegister] = GetNextInput(input);
                            break;
                        case "add":
                            Registers[instruction.DestRegister] += SecondOperand(instruction);
                            break;
                        case "mul":
                            Registers[instruction.DestRegister] *= SecondOperand(instruction);
                            break;
                        case "div":
                            Registers[instruction.DestRegister] /= SecondOperand(instruction);
                            break;
                        case "mod":
                            Registers[instruction.DestRegister] %= SecondOperand(instruction);
                            break;
                        case "eql":
                            Registers[instruction.DestRegister] = (Registers[instruction.DestRegister] == SecondOperand(instruction)) ? 1 : 0;
                            break;
                        case "set":
                            Registers[instruction.DestRegister] = SecondOperand(instruction);
                            break;
                        case "nop":
                            break;
                        default:
                            throw new ArgumentException("Unknow operation", $"{instruction.Operation}");
                            break;
                    }
                }
                return Registers[RegisterIndexOf(outputRegister)];
            }

            public ALU ExecuteOne(Program program, int input)
            {
                foreach (Instruction instruction in program.Instructions)
                {
                    switch (instruction.Operation)
                    {
                        case "inp":
                            Registers[instruction.DestRegister] = input;
                            InputIndex++;
                            CumulativeInput += input.ToString();
                            break;
                        case "add":
                            Registers[instruction.DestRegister] += SecondOperand(instruction);
                            break;
                        case "mul":
                            Registers[instruction.DestRegister] *= SecondOperand(instruction);
                            break;
                        case "div":
                            Registers[instruction.DestRegister] /= SecondOperand(instruction);
                            break;
                        case "mod":
                            Registers[instruction.DestRegister] %= SecondOperand(instruction);
                            break;
                        case "eql":
                            Registers[instruction.DestRegister] = (Registers[instruction.DestRegister] == SecondOperand(instruction)) ? 1 : 0;
                            break;
                        case "set":
                            Registers[instruction.DestRegister] = SecondOperand(instruction);
                            break;
                        case "nop":
                            break;
                        default:
                            throw new ArgumentException("Unknow operation", $"{instruction.Operation}");
                            break;
                    }
                }
                return this;
            }

            public override string ToString()
            {
                return $"i = {CumulativeInput} => w = {Registers[0]}, x = {Registers[1]}, y = {Registers[2]}, z = {Registers[3]}, ii = {InputIndex}";
            }

            public int GetNextInput(string input) => int.Parse(input[InputIndex++].ToString());
            public long SecondOperand(Instruction ins) => (ins.SourceRegister == Instruction.OperandIsLiteral) ? (long)ins.SourceLiteral : Registers[ins.SourceRegister];

            public static int RegisterIndexOf(char c) => (int)c - FirstRegisterName;
            public static char RegisterNameOf(int i) => (char)(i + FirstRegisterName);
        }
        
        public Puzzle94(string input) : base(input) { Name = "Arithmetic Logic Unit"; }

        const int NumInputDigits = 14;
        public override void Setup()
        {
            // TODO: Break the program into sub-programs starting with each inp statement
            // Run each sub-program on inputs of 1-9 and look at ALU state
            List<string> lines = Tools.GetLines(Input);

            program = new();

            programs = new Program[NumInputDigits];
            for (int i = 0; i < NumInputDigits; i++)
                programs[i] = new();

            int inputStatementNumber = 0;
            bool firstInputStatement = true;

            foreach (string line in lines)
            {
                Instruction instruction = new Instruction(line);
                if (instruction.Operation == "inp")
                {
                    if (!firstInputStatement)
                        inputStatementNumber++;
                    else
                        firstInputStatement = false;
                    programs[inputStatementNumber].Instructions.Add(instruction);
                    program.Instructions.Add(instruction);
                }
                else if (instruction.Operation != Instruction.Comment)
                {
                    programs[inputStatementNumber].Instructions.Add(instruction);
                    program.Instructions.Add(instruction);
                }
            }
        }

        public long OldInnerLoop()
        {
            const long ValidModelNumber = 0;
            long smallestOuput = Int64.MaxValue;
            ALU alu = new();

            // a solution (not largest) = 44671911181712

            //for (long i = 99999999999999; i >= 11111111111111; i--)
            for (long i = 49999999981712; i >= 11111111111111; i -= 100000)
            // for (long i = 11111911181712; i <= 99999999999999; i += 1000000000)
            {
                string candidateModelNum = i.ToString();
                if (!candidateModelNum.Contains('0'))
                {
                    long output = alu.Execute(program, candidateModelNum, 'z');
                    if (Math.Abs(output) < smallestOuput)
                    {
                        Console.WriteLine($"new smallest = {output}, input = {i}");
                        smallestOuput = Math.Abs(output);
                    }
                    if (output == ValidModelNumber)
                        return i;
                }
            }
            return -1; 
        }

        public long LargestValidSerialNumber()
        {
            return OldInnerLoop();
        }

        private void RunInputs(ALU alu, int stage, int depth)
        {
            ALU root;
            if (stage == 0)
                root = new();
            else
                root = new ALU(alu);

            for (int digit = 1; digit <= 9; digit++)
            {
                ALU a = new ALU(root);
                a.ExecuteOne(programs[stage], digit);
                if (a.Registers[3] < root.Registers[3])
                    Console.WriteLine($"{new String(' ', stage * 3)}{a}");
                if (stage < depth)
                    RunInputs(a, stage + 1, depth);
             }
        }

        private void WriteAllSubprograms()
        {
            string[] lines = new string[18];

            for (int lnum = 0; lnum < 18; lnum++)
            {
                lines[lnum] = string.Empty;
                for (int prog = 0; prog < NumInputDigits; prog++)
                    lines[lnum] += $"{programs[prog].Instructions[lnum]}\t";
            }
            File.WriteAllLinesAsync("Programs.txt", lines);
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart1()
        {
            Console.WriteLine("\n");

            RunInputs(null, 0, 4 );

            return string.Empty; //  LargestValidSerialNumber().ToString();
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart2()
        {
            //Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}