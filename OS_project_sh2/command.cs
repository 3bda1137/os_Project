using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
    public static class Commands
    {
        public static void clear(List<IputAsKeyValue> IputAsKeyValues)
        {
            if (IputAsKeyValues.Count > 1)
                Console.WriteLine("Error: " + IputAsKeyValues[0].value + " command syntax is \n cls \n function: Clear the screen.");
            else
                Console.Clear();
        }
        public static void quit(List<IputAsKeyValue> IputAsKeyValues)
        {
            if (IputAsKeyValues.Count > 1)
                Console.WriteLine("Error: " + IputAsKeyValues[0].value + " command syntax is \n quit \n function: Quit the shell.");
            else
            {
                Mini_FAT.WriteFat();
                Virtual_Disk.File_Disk.Close();
                Environment.Exit(0);
            }
        }
        public static void createDirectory(List<IputAsKeyValue> IputAsKeyValues)
        {
            if (IputAsKeyValues.Count <= 1 || IputAsKeyValues.Count > 2)
                Console.WriteLine("Error: " + IputAsKeyValues[0].value + "command syntax is \n md [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            else
            {
                if (IputAsKeyValues[1].key == TypeOfInput.DirName)
                {
                    if (Program.current.searchDirectory(IputAsKeyValues[1].value) == -1)
                    {
                        if (Mini_FAT.GetAvilableBlock() != -1)
                        {
                            Directory_Entry d = new Directory_Entry(IputAsKeyValues[1].value, 0x10, 0);
                            Program.current.directory_table.Add(d);
                            Program.current.WriteDirectory();
                            if (Program.current.parent != null)
                            {
                                Program.current.parent. UpdateContent(Program.current.GetDirectoryEntry());
                                Program.current.parent.WriteDirectory();
                            }
                            Mini_FAT.WriteFat();
                        }
                        else
                            Console.WriteLine($"Error : sorry the disk is full!");
                    }
                    else
                        Console.WriteLine($"Error : this directory \" {IputAsKeyValues[1].value} \" is already exists!");
                }
                else if (IputAsKeyValues[1].key == TypeOfInput.PathToDirectory)
                {
                    Directory dir = moveTodir(IputAsKeyValues[1], false);
                    if (dir == null)
                        Console.WriteLine($"Error : this path \" {IputAsKeyValues[1].value} \" is not exists!");
                    else
                    {
                        if (Mini_FAT.GetAvilableBlock() != -1)
                        {
                            string[] ss = IputAsKeyValues[1].value.Split('\\');
                            Directory_Entry d = new Directory_Entry(ss[ss.Length - 1], 0x10, 0);
                            dir.directory_table.Add(d);
                            dir.WriteDirectory();
                            dir.parent.UpdateContent(dir.GetDirectoryEntry());
                            dir.parent.WriteDirectory();
                            Mini_FAT.WriteFat();
                        }
                        else
                            Console.WriteLine($"Error : sorry the disk is full!");
                    }
                }
                else
                    Console.WriteLine("Error: " + IputAsKeyValues[0].value + "command syntax is \n md [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            }
        }

        internal static void createFile(List<IputAsKeyValue> IputAsKeyValues)
        {
            throw new NotImplementedException();
        }

        public static File_Entry createFile(IputAsKeyValue IputAsKeyValue)
        {
            if (IputAsKeyValue.key == TypeOfInput.FileName)
            {
                if (Program.current.searchDirectory(IputAsKeyValue.value) == -1)
                {
                    if (Mini_FAT.GetAvilableBlock() != -1)
                    {
                        Directory_Entry d = new Directory_Entry(IputAsKeyValue.value, 0x0, 0);
                        Program.current.directory_table.Add(d);
                        Program.current.WriteDirectory();
                        if (Program.current.parent != null)
                        {
                            Program.current.parent.UpdateContent(Program.current.GetDirectoryEntry());
                            Program.current.parent.WriteDirectory();
                        }
                        Mini_FAT.WriteFat();
                        File_Entry f = new File_Entry(IputAsKeyValue.value, 0x0, 0, Program.current);
                        return f;
                    }
                    else
                        Console.WriteLine($"Error : sorry the disk is full!");
                }
                else
                    Console.WriteLine($"Error : this file name \" {IputAsKeyValue.value} \" is already exists!");
            }
            else if (IputAsKeyValue.key == TypeOfInput.PathToFile)
            {
                IputAsKeyValue t = new IputAsKeyValue();
                t.value = IputAsKeyValue.value.Substring(0, IputAsKeyValue.value.LastIndexOf('\\'));
                t.key = IputAsKeyValue.key;
                Directory dir = moveTodir(t, false);
                if (dir == null)
                    Console.WriteLine($"Error : this path \" {IputAsKeyValue.value} \" is not exists!");
                else
                {
                    if (Mini_FAT.GetAvilableBlock() != -1)
                    {
                        string[] ss = IputAsKeyValue.value.Split('\\');
                        Directory_Entry d = new Directory_Entry(ss[ss.Length - 1], 0x10, 0);
                        dir.directory_table.Add(d);
                        dir.WriteDirectory();
                        dir.parent.UpdateContent(dir.GetDirectoryEntry());
                        dir.parent.WriteDirectory();
                        Mini_FAT.WriteFat();
                        File_Entry f = new File_Entry(ss[ss.Length - 1], 0x0, 0, dir);
                        return f;
                    }
                    else
                        Console.WriteLine($"Error : sorry the disk is full!");
                }
            }
            return null;
        }
        private static Directory moveTodir(IputAsKeyValue IputAsKeyValue, bool changedirFlag)
        {
            Directory d = null;
            string path;
            if (IputAsKeyValue.key == TypeOfInput.DirName)
            {
                if (IputAsKeyValue.value != "..")
                {
                    int i = Program.current.searchDirectory(IputAsKeyValue.value);
                    if (i == -1)
                        return null;
                    else
                    {
                        string n = new string(Program.current.directory_table[i].dir_name);
                        byte at = Program.current.directory_table[i].dir_attr;
                        int fc = Program.current.directory_table[i].dir_firstCluster;
                        d = new Directory(n, at, fc, Program.current);
                        d.ReadDirectory();
                        path = Program.currentPath;
                        path += "\\" + n.Trim(new char[] { '\0', ' ' });
                        if (changedirFlag)
                            Program.currentPath = path;
                    }
                }
                else
                {
                    if (Program.current.parent != null)
                    {
                        d = Program.current.parent;
                        d.ReadDirectory();
                        path = Program.currentPath;
                        path = path.Substring(0, path.LastIndexOf('\\'));
                        if (changedirFlag)
                            Program.currentPath = path;
                    }
                    else
                    {
                        d = Program.current;
                        d.ReadDirectory();
                    }
                }
            }
            else if (IputAsKeyValue.key == TypeOfInput.PathToDirectory)
            {
                string[] s = IputAsKeyValue.value.Split('\\');
                List<string> ss = new List<string>();
                for (int i = 0; i < s.Length; i++)
                    if (s[i] != "")
                        ss.Add(s[i]);
                Directory rootd = new Directory("K:", 0x10, 5, null);
                rootd.ReadDirectory();
                if (ss[0].ToLower().Equals("k:"))
                {
                    path = "k:";
                    int ll = (changedirFlag) ? ss.Count : ss.Count - 1;
                    for (int i = 1; i < ll; i++)
                    {
                        int j = rootd.searchDirectory(ss[i]);
                        if (j != -1)
                        {
                            Directory pa = rootd;
                            string n = new string(rootd.directory_table[j].dir_name);
                            byte at = rootd.directory_table[j].dir_attr;
                            int fc = rootd.directory_table[j].dir_firstCluster;
                            rootd = new Directory(n, at, fc, pa);
                            rootd.ReadDirectory();
                            path += "\\" + n.Trim(new char[] { '\0', ' ' });
                        }
                        else
                        {
                            return null;
                        }
                    }
                    d = rootd;
                    if (changedirFlag)
                        Program.currentPath = path;
                }
                else if (ss[0] == "..")
                {
                    d = Program.current;
                    for (int i = 0; i < ss.Count; i++)
                    {
                        if (d.parent != null)
                        {
                            d = d.parent;
                            d.ReadDirectory();
                            path = Program.currentPath;
                            path = path.Substring(0, path.LastIndexOf('\\'));
                            if (changedirFlag)
                                Program.currentPath = path;
                        }
                        else
                        {
                            d = Program.current;
                            d.ReadDirectory();
                            break;
                        }
                    }
                }
                else
                    return null;
            }
            return d;
        }
        public static void changeDirectory(List<IputAsKeyValue> IputAsKeyValues)
        {
            if (IputAsKeyValues.Count == 1)
                Console.WriteLine(Program.currentPath);
            else if (IputAsKeyValues.Count == 2)
            {
                if (IputAsKeyValues[1].key == TypeOfInput.DirName || IputAsKeyValues[1].key == TypeOfInput.PathToDirectory)
                {
                    Directory dir = moveTodir(IputAsKeyValues[1], true);
                    if (dir != null)
                    {
                        dir.ReadDirectory();
                        Program.current = dir;
                    }
                    else
                    {
                        Console.WriteLine($"Error : this path \" {IputAsKeyValues[1].value} \" is not exists!");
                    }
                }
                else
                    Console.WriteLine("Error: " + IputAsKeyValues[0].value + "command syntax is \n cd \n or \n cd [directory]\n[directory] can be a new directory name or fullpath of a directory");
            }
            else
                Console.WriteLine("Error: " + IputAsKeyValues[0].value + "command syntax is \n cd \n or \n cd [directory]\n[directory] can be a new directory name or fullpath of a directory");
        }
        public static void help(List<IputAsKeyValue> IputAsKeyValues)
        {
            if (IputAsKeyValues.Count > 2)
            {
                Console.WriteLine("Error: " + IputAsKeyValues[0].value + " command syntax is \n help \n or \n help [command] \n function:Provides Help information for commands.");
            }
            else if (IputAsKeyValues.Count == 2)
            {
                if (IputAsKeyValues[1].key == TypeOfInput.Command)
                {
                    switch (IputAsKeyValues[1].value)
                    {
                        case "cd":
                            Console.WriteLine("Change the current default directory to the directory given in the argument.");
                            Console.WriteLine("If the argument is not present, report the current directory.");
                            Console.WriteLine("If the directory does not exist an appropriate error should be reported.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n cd \n or \n cd [directory]");
                            Console.WriteLine("[directory] can be directory name or fullpath of a directory");
                            break;
                        case "cls":
                            Console.WriteLine("Clear the screen.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n cls");
                            break;
                        case "dir":
                            Console.WriteLine("List the contents of directory given in the argument.");
                            Console.WriteLine("If the argument is not present, list the content of the current directory.");
                            Console.WriteLine("If the directory does not exist an appropriate error should be reported.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n dir \n or \n dir [directory]");
                            Console.WriteLine("[directory] can be directory name or fullpath of a directory");
                            break;
                        case "quit":
                            Console.WriteLine("Quit the shell.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n quit");
                            break;
                        case "copy":
                            Console.WriteLine("Copies one or more files to another location.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n copy [source]+ [destination]");
                            Console.WriteLine("+ after [source] represent that you can pass more than file Name (or fullpath of file) or more than directory Name (or fullpath of directory)");
                            Console.WriteLine("[source] can be file Name (or fullpath of file) or directory Name (or fullpath of directory)");
                            Console.WriteLine("[destination] can be directory name or fullpath of a directory");
                            break;
                        case "del":
                            Console.WriteLine("Deletes one or more files.");
                            Console.WriteLine("NOTE: it confirms the user choice to delete the file before deleting");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n del [file]+");
                            Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file)");
                            Console.WriteLine("[file] can be file Name (or fullpath of file)");
                            break;
                        case "help":
                            Console.WriteLine("Provides Help information for commands.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n help \n or \n For more information on a specific command, type help [command]");
                            Console.WriteLine("command - displays help information on that command.");
                            break;
                        case "md":
                            Console.WriteLine("Creates a directory.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n md [directory]");
                            Console.WriteLine("[directory] can be a new directory name or fullpath of a new directory");
                            break;
                        case "rd":
                            Console.WriteLine("Removes a directory.");
                            Console.WriteLine("NOTE: it confirms the user choice to delete the directory before deleting");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n rd [directory]");
                            Console.WriteLine("[directory] can be a directory name or fullpath of a directory");
                            break;
                        case "rename":
                            Console.WriteLine("Renames a file.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n rd [fileName] [new fileName]");
                            Console.WriteLine("[fileName] can be a file name or fullpath of a filename ");
                            Console.WriteLine("[new fileName] can be a new file name not fullpath ");
                            break;
                        case "type":
                            Console.WriteLine("Displays the contents of a text file.");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n type [file]");
                            Console.WriteLine("NOTE: it displays the filename before its content for every file");
                            Console.WriteLine("[file] can be file Name (or fullpath of file) of text file");
                            break;
                        case "import":
                            Console.WriteLine("– import text file(s) from your computer ");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n import [destination] [file]+");
                            Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file) of text file");
                            Console.WriteLine("[file] can be file Name (or fullpath of file) of text file");
                            Console.WriteLine("[destination] can be directory name or fullpath of a directory in your implemented file system");
                            break;
                        case "export":
                            Console.WriteLine("– export text file(s) to your computer ");
                            Console.WriteLine(IputAsKeyValues[1].value + " command syntax is \n export [destination] [file]+");
                            Console.WriteLine("+ after [file] represent that you can pass more than file Name (or fullpath of file) of text file");
                            Console.WriteLine("[file] can be file Name (or fullpath of file) of text file in your implemented file system");
                            Console.WriteLine("[destination] can be directory name or fullpath of a directory in your computer");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Error: " + IputAsKeyValues[1].value + " => This command is not supported by the help utility.");
                }
            }
            else if (IputAsKeyValues.Count == 1)
            {
                Console.WriteLine("cd       - Change the current default directory to .");
                Console.WriteLine("           If the argument is not present, report the current directory.");
                Console.WriteLine("           If the directory does not exist an appropriate error should be reported.");
                Console.WriteLine("cls      - Clear the screen.");
                Console.WriteLine("dir      - List the contents of directory .");
                Console.WriteLine("quit     - Quit the shell.");
                Console.WriteLine("copy     - Copies one or more files to another location");
                Console.WriteLine("del      - Deletes one or more files.");
                Console.WriteLine("help     - Provides Help information for commands.");
                Console.WriteLine("md       - Creates a directory.");
                Console.WriteLine("rd       - Removes a directory.");
                Console.WriteLine("rename   - Renames a file.");
                Console.WriteLine("type     - Displays the contents of a text file.");
                Console.WriteLine("import   – import text file(s) from your computer");
                Console.WriteLine("export   – export text file(s) to your computer");
            }
        }

        public static void listDirectory(List<IputAsKeyValue> IputAsKeyValues)
        {
            if (IputAsKeyValues.Count == 1)
            {
                int filecount = 0;
                int directorycount = 0;
                int filesSizesSum = 0;
                Console.WriteLine(" Directory of " + Program.currentPath);
                Console.WriteLine();
                int start = 1;
                if (Program.current.parent != null)
                {
                    start = 2;
                    Console.WriteLine("{0}{1:11}", "\t<DIR>    ", ".");
                    directorycount++;
                    Console.WriteLine("{0}{1:11}", "\t<DIR>    ", "..");
                    directorycount++;
                }
                for (int i = start; i < Program.current.directory_table.Count; i++)
                {
                    if (Program.current.directory_table[i].dir_attr == 0x0)
                    {
                        Console.WriteLine("\t{0:9}{1:11}", Program.current.directory_table[i].dir_filesize, new string(Program.current.directory_table[i].dir_name));
                        filecount++;
                        filesSizesSum += Program.current.directory_table[i].dir_filesize;
                    }
                    else if (Program.current.directory_table[i].dir_attr == 0x10)
                    {
                        Console.WriteLine("{0}{1:11}", "\t<DIR>    ", new string(Program.current.directory_table[i].dir_name));
                        directorycount++;
                    }
                }
                Console.WriteLine($"{"              "}{filecount} File(s)    {filesSizesSum} bytes");
                Console.WriteLine($"{"              "}{directorycount} Dir(s)    {Virtual_Disk.getFreeSpace()} bytes free");
            }
            else if (IputAsKeyValues.Count == 2)
            {
                if (IputAsKeyValues[1].key == TypeOfInput.DirName || IputAsKeyValues[1].key == TypeOfInput.PathToDirectory)
                {
                    Directory dir = moveTodir(IputAsKeyValues[1], false);
                    if (dir != null)
                    {
                        dir.ReadDirectory();
                        int filecount = 0;
                        int directorycount = 0;
                        int filesSizesSum = 0;
                        if (IputAsKeyValues[1].key == TypeOfInput.DirName)
                            Console.WriteLine(" Directory of " + Program.currentPath + "\\" + IputAsKeyValues[1].value);
                        else
                            Console.WriteLine(" Directory of " + IputAsKeyValues[1].value);
                        Console.WriteLine();
                        int start = 1;
                        if (dir.parent != null)
                        {
                            start = 2;
                            Console.WriteLine("{0}{1:11}", "\t<DIR>    ", ".");
                            directorycount++;
                            Console.WriteLine("{0}{1:11}", "\t<DIR>    ", "..");
                            directorycount++;
                        }
                        for (int i = start; i < dir.directory_table.Count; i++)
                        {
                            if (dir.directory_table[i].dir_attr == 0x0)
                            {
                                Console.WriteLine("\t{0:9} {1:11}", dir.directory_table[i].dir_filesize, new string(dir.directory_table[i].dir_name));
                                filecount++;
                                filesSizesSum += dir.directory_table[i].dir_filesize;
                            }
                            else if (dir.directory_table[i].dir_attr == 0x10)
                            {
                                Console.WriteLine("{0}{1:11}", "\t<DIR>    ", new string(dir.directory_table[i].dir_name));
                                directorycount++;
                            }
                        }
                        Console.WriteLine($"{"              "}{filecount} File(s)    {filesSizesSum} bytes");
                        Console.WriteLine($"{"              "}{directorycount} Dir(s)    {Virtual_Disk.getFreeSpace()} bytes free");
                    }
                    else
                        Console.WriteLine($"Error : this path \" {IputAsKeyValues[1].value} \" is not exists!");
                }
                else
                    Console.WriteLine("Error: " + IputAsKeyValues[0].value + " command syntax is \n dir \n or \n dir [directory]\n[directory] can be a new directory name or fullpath of a directory");
            }
            else
                Console.WriteLine("Error: " + IputAsKeyValues[0].value + " command syntax is \n dir \n or \n dir [directory]\n[directory] can be a new directory name or fullpath of a directory");
        }
        public static void removeDirectory(List<IputAsKeyValue> IputAsKeyValues)
        {
            if (IputAsKeyValues.Count <= 1 || IputAsKeyValues.Count > 2)
                Console.WriteLine("Error: " + IputAsKeyValues[0].value + "command syntax is \n rd [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            else
            {
                if (IputAsKeyValues[1].key == TypeOfInput.DirName || IputAsKeyValues[1].key == TypeOfInput.PathToDirectory)
                {
                    Directory dir = moveTodir(IputAsKeyValues[1], false);
                    if (dir != null)
                    {
                        Console.Write($"Are you sure that you want to delete {new string(dir.dir_name).Trim(new char[] { '\0', ' ' })} , please enter Y for yes or N for no:");
                        string s = Console.ReadLine().ToLower();
                        if (s.Equals("y"))
                            dir.DeleteDirectory();
                    }
                    else
                        Console.WriteLine($"Error : this directory \" {IputAsKeyValues[1].value} \" is not exists!");
                }
                else
                    Console.WriteLine("Error: " + IputAsKeyValues[0].value + "command syntax is \n rd [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            }
        }
    }
}
