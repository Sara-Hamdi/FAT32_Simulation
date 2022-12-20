using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
namespace asd
{
    class FAT32_Simulation
    {

        public static Directorry currentDirectory;
        public static string CurrentPath;

        static void Main(string[] args)
        {
            VirtualDisk.Initalize();
            //  FatTable.Iinialize();
            CurrentPath = new string(currentDirectory.dir_name);
            shell_Instructions sh = new shell_Instructions();
           // Console.WriteLine("hhhhh");

            string input;
            while (true)
            {

                Console.Write(CurrentPath.Trim());
                Console.Write(":>");
                input = Console.ReadLine();
                List<string> l;
                l = sh.parse(input);
                if (l[0] == "help" && l.Count == 1)
                {
                    sh.help();
                }
                else if (l[0] == "help" && l.Count > 1)
                {
                    sh.help(l[1]);
                }
                else if (l[0] == "dir")
                {
                    sh.dir();
                }
                else if (l[0] == "cd")
                {
                    if (l.Count == 1)
                        sh.cd(l[0]);
                    else if (l.Count == 2)
                        sh.cd(l[1]);
                    else Console.WriteLine("The filename, directory name, or volume label syntax is incorrect.");
                }
                else if (l[0] == "md" && l.Count > 1)
                {
                    for (int i = 1; i < l.Count; i++)
                    {
                        sh.md(l[i]);
                    }
                }
                else if (l[0] == "rd" && l.Count > 1)
                {
                    for (int i = 1; i < l.Count; i++)
                    {
                        sh.rd(l[i]);
                    }
                }
                else if (l[0] == "rename" && l.Count == 3)
                {
                    sh.rename(l[1], l[2]);
                }
                else if (l[0] == "quit") sh.quit();
                else if (l[0] == "cls") sh.cls();
                else if (l[0] == "import") sh.import(l[1]);
                else if(l[0]=="del"&&l.Count>1)
                {
                    for(int i=1;i<l.Count;i++)
                    {
                        sh.del(l[i]);
                    }
                }
                else if(l[0]=="type"&&l.Count>1)
                {
                    sh.type(l[1]);
                }
                else if(l[0]=="export"&&l.Count==3)
                {
                    sh.export(l[1],l[2]);
                }
                else
                {
                    Console.WriteLine("The syntax of the command is incorrect.");
                }


            }
        }

    }
}



