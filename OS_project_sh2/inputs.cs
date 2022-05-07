using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
    public enum TypeOfInput
    {
        DirName, PathToFile, Command, NotCommand, PathToDirectory, FileName
    }
    public struct IputAsKeyValue
    {
        public TypeOfInput key;
        public string value;
    }
    public static class INPUTS
    {
        
        static IputAsKeyValue generateIput(string arg, TypeOfInput TypeOfInput)
        {
            IputAsKeyValue IputAsKeyValue;
            IputAsKeyValue.key = TypeOfInput;
            IputAsKeyValue.value = arg;
            return IputAsKeyValue;
        }
        static bool IsCommand(string arg)
        {
            string[] command = { "cd", "help", "dir", "quit", "copy", "cls", "del", "md", "rd", "rename", "type", "import", "export" };
            foreach (string i in command)
            {
                if (i == arg)
                    return true;
            }
            return false;
        }
        static bool IsPathToDirectory(string arg)
        {
            return (arg.Contains(':') || arg.Contains('\\')) && !arg.Contains('.');
        }
        static bool IsPathToFile(string arg)
        {
            return (arg.Contains(':') || arg.Contains('\\')) && arg.Contains('.');
        }
        static bool IsFileName(string arg)
        {
            return arg.Contains('.') && !arg.Contains(':') && !arg.Contains('\\');
        }
        public static List<IputAsKeyValue> GetInput(string input)
        {
            List<IputAsKeyValue> IputAsKeyValues = new List<IputAsKeyValue>();

            if (input.Length == 0)
            {
                return null;
            }
            string[] inputs = input.Split(' ');
            List<string> lest_input = new List<string>();
            for (int i = 0; i < inputs.Length; i++)
                if (inputs[i] != "" && inputs[i] != " ")
                    lest_input.Add(inputs[i]);
            string[] arguments = lest_input.ToArray();
            arguments[0] = arguments[0].ToLower();
            switch (arguments[0])
            {
                case "cd":
                    if (arguments.Length == 1)
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    else if (arguments.Length == 2)
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        if (IsPathToDirectory(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToDirectory));
                        else if (IsPathToFile(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToFile));
                        else if (IsFileName(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.FileName));
                        else
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.DirName));
                    }
                    else
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        for (int i = 1; i < arguments.Length; i++)
                            IputAsKeyValues.Add(generateIput(arguments[i], TypeOfInput.NotCommand));
                    }
                    break;
                case "cls":
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    if (arguments.Length > 1)
                        for (int i = 1; i < arguments.Length; i++)
                            IputAsKeyValues.Add(generateIput(arguments[i], TypeOfInput.NotCommand));
                    break;
                case "dir":
                    if (arguments.Length == 1)
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    else if (arguments.Length == 2)
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        if (IsPathToDirectory(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToDirectory));
                        else if (IsPathToFile(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToFile));
                        else if (IsFileName(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.FileName));
                        else
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.DirName));
                    }
                    else
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        for (int i = 1; i < arguments.Length; i++)
                            IputAsKeyValues.Add(generateIput(arguments[i], TypeOfInput.NotCommand));
                    }
                    break;
                case "quit":
                    if (arguments.Length == 1)
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    else
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        for (int i = 1; i < arguments.Length; i++)
                            IputAsKeyValues.Add(generateIput(arguments[i], TypeOfInput.NotCommand));
                    }
                    break;
                case "copy":
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    break;
                case "del":
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    break;
                case "help":
                    if (arguments.Length == 1)
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    else if (arguments.Length == 2)
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        arguments[1] = arguments[1].ToLower();
                        if (IsCommand(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.Command));
                        else
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.NotCommand));
                    }
                    else
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        for (int i = 1; i < arguments.Length; i++)
                            IputAsKeyValues.Add(generateIput(arguments[i], TypeOfInput.NotCommand));
                    }
                    break;
                case "md":
                    if (arguments.Length == 1)
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    else if (arguments.Length == 2)
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        if (IsPathToDirectory(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToDirectory));
                        else if (IsPathToFile(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToFile));
                        else if (IsFileName(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.FileName));
                        else
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.DirName));
                    }
                    else
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        for (int i = 1; i < arguments.Length; i++)
                            IputAsKeyValues.Add(generateIput(arguments[i], TypeOfInput.NotCommand));
                    }
                    break;
                case "rd":
                    if (arguments.Length == 1)
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    else if (arguments.Length == 2)
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        if (IsPathToDirectory(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToDirectory));
                        else if (IsPathToFile(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.PathToFile));
                        else if (IsFileName(arguments[1]))
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.FileName));
                        else
                            IputAsKeyValues.Add(generateIput(arguments[1], TypeOfInput.DirName));
                    }
                    else
                    {
                        IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                        for (int i = 1; i < arguments.Length; i++)
                            IputAsKeyValues.Add(generateIput(arguments[i], TypeOfInput.NotCommand));
                    }
                    break;
                case "rename":
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    break;
                case "type":
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    break;
                case "import":
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    break;
                case "export":
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.Command));
                    break;
                default:
                    IputAsKeyValues.Add(generateIput(arguments[0], TypeOfInput.NotCommand));
                    break;
            }
            return IputAsKeyValues;
        }
    }
}
