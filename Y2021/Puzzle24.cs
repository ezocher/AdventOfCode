using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle24 : ASolver 
    {
        private List<Instruction> program;

        private class Instruction
        {
            public const int OperandIsLiteral = -1;

            public string Operation;
            public int DestRegister;
            public int SourceRegister;
            public int SourceLiteral;

            public Instruction(string line)
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Operation = parts[0];
                DestRegister = ALU.RegisterIndexOf(parts[1][0]);
                if (parts.Length > 2)
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
                }
            }

            public override string ToString()
            {
                string instruction = $"{Operation} {ALU.RegisterNameOf(DestRegister)} ";
                if (ALU.IsThreeOperandInstruction(Operation))
                    instruction += (SourceRegister == OperandIsLiteral) ? SourceLiteral.ToString() : ALU.RegisterNameOf(SourceRegister).ToString();
                return instruction;
            }
        }

        private class ALU
        {
            public const char FirstRegisterName = 'w';
            public const int NumRegisters = 4;
            public const int InitialRegisterValue = 0;

            public long[] Registers;
            public int InputIndex;

            public ALU()
            {
                Registers = new long[NumRegisters];
                Reset();
            }

            private void Reset()
            {
                for (int i = 0; i < NumRegisters; i++)
                    Registers[i] = InitialRegisterValue;
                InputIndex = 0;
            }

            public long Execute(List<Instruction> program, string input, char outputRegister)
            {
                Reset();
                foreach (Instruction instruction in program)
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
                        default:
                            throw new ArgumentException("Unknow operation", $"{instruction.Operation}");
                            break;
                    }
                }
                return Registers[RegisterIndexOf(outputRegister)];
            }

            public override string ToString()
            {
                return $"w = {Registers[0]}, x = {Registers[1]}, y = {Registers[2]}, z = {Registers[3]}, II = {InputIndex}";
            }

            public int GetNextInput(string input) => int.Parse(input[InputIndex++].ToString());
            public long SecondOperand(Instruction ins) => (ins.SourceRegister == Instruction.OperandIsLiteral) ? (long)ins.SourceLiteral : Registers[ins.SourceRegister];

            public static int RegisterIndexOf(char c) => (int)c - FirstRegisterName;
            public static char RegisterNameOf(int i) => (char)(i + FirstRegisterName);
            internal static bool IsThreeOperandInstruction(string operation) => operation != "inp";

            //inp a - Read an input value and write it to variable a.
            //add a b - Add the value of a to the value of b, then store the result in variable a.
            //mul a b - Multiply the value of a by the value of b, then store the result in variable a.
            //div a b - Divide the value of a by the value of b, truncate the result to an integer, then store the result in variable a. (Here, "truncate" means to round the value toward zero.)
            //mod a b - Divide the value of a by the value of b, then store the remainder in variable a. (This is also called the modulo operation.)
            //eql a b - If the value of a and b are equal, then store the value 1 in variable a.Otherwise, store the value 0 in variable a.
        }
        public Puzzle24(string input) : base(input) { Name = "Arithmetic Logic Unit"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            program = new();
            foreach (string line in lines)
                program.Add(new Instruction(line));
        }

        public long LargestValidSerialNumber()
        {
            const long ValidModelNumber = 0;
            ALU alu = new();


            for (long i = 99999999999999; i >= 11111111111111; i--)
            {
                string candidateModelNum = i.ToString();
                if (!candidateModelNum.Contains('0'))
                {
                    long output = alu.Execute(program, candidateModelNum, 'z');
                    if (output == ValidModelNumber)
                        return i;
                }
            }
            return 0;
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart1()
        {

            return LargestValidSerialNumber().ToString();
        }

        [Description("What is the largest model number accepted by MONAD?")]
        public override string SolvePart2()
        {
            //Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}