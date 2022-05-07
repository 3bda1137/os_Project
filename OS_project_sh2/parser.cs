using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
   
    public static class Parser
    {
        public static void parse(string input)
        {
            List<IputAsKeyValue> IputAsKeyValues = INPUTS.GetInput(input);
            if (IputAsKeyValues != null)
            {
                if (IputAsKeyValues[0].key == TypeOfInput.NotCommand)
                    Console.WriteLine(IputAsKeyValues[0].value + " is not recognized as an internal or external command,operable program or batch file.");
                else
                {
                    if (IputAsKeyValues[0].key == TypeOfInput.Command)
                    {
                        switch (IputAsKeyValues[0].value)
                        {
                            case "cd":
                                Commands.changeDirectory(IputAsKeyValues);
                                break;
                            case "cls":
                                Commands.clear(IputAsKeyValues);
                                break;
                            case "dir":
                                Commands.listDirectory(IputAsKeyValues);
                                break;
                            case "quit":
                                Commands.quit(IputAsKeyValues);
                                break;
                            case "help":
                                Commands.help(IputAsKeyValues);
                                break;
                            case "md":
                                Commands.createDirectory(IputAsKeyValues);
                                break;
                            case "rd":
                                Commands.removeDirectory(IputAsKeyValues);
                                break;
                            
                        }
                    }
                }
            }
        }

    }
}
