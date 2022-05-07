using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace OS_project_sh2
{
    class Program
    {
        //opject from class Directory
        public static Directory current;
        // public variable
        public static string currentPath;
        // method main  the project started from  this method
        static void Main(string[] args)
        {
            //call method  initalize  from class virtual
            //the 'FILE' is name of the file  
            Virtual_Disk.Initalize("FILE");

            currentPath = new string(current.dir_name);
            //remove the all " " in the path
            currentPath = currentPath.Trim(new char[] { '\0', ' ' });
            while (true)
            {
                //print the path
                Console.Write(currentPath + "\\" + ">");
                current.ReadDirectory();
               string input;
                //input the command from the user
                input = Console.ReadLine();
                //pass the input  to method parse in class praser
                Parser.parse(input);
            }
        }
    }
}
