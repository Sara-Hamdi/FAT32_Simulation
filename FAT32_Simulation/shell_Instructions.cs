using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace asd
{
    public class shell_Instructions
    {
        public void help()
        {
            Console.WriteLine("cls    Clear the screen");
            Console.WriteLine("dir    List the contents of directory");
            Console.WriteLine("quit   Quit the shell​");
            Console.WriteLine("copy   Copies one or more files to another location​​");
            Console.WriteLine("del    Deletes one or more files​");
            Console.WriteLine("help   Provides Help information for commands");
            Console.WriteLine("md     Creates a directory​");
            Console.WriteLine("rd     Removes a directory");
            Console.WriteLine("rename Renames a file​​​");
            Console.WriteLine("type   Displays the contents of a text file​");
            Console.WriteLine("import import text file(s) from your computer​​​​");
            Console.WriteLine("export export text file(s) to your computer");

        }
        public void help(string com)
        {
            if (com == "cls") Console.WriteLine("Clear the screen");
            else if (com == "dir") Console.WriteLine("List the contents of directory");
            else if (com == "quit") Console.WriteLine("Quit the shell​");
            else if (com == "copy") Console.WriteLine("Copies one or more files to another location​​");
            else if (com == "del") Console.WriteLine("Deletes one or more files");
            else if (com == "help") Console.WriteLine("Provides Help information for commands");
            else if (com == "md") Console.WriteLine("Creates a directory");
            else if (com == "rd") Console.WriteLine("Removes a directory");
            else if (com == "rename") Console.WriteLine("Renames a file​​​");
            else if (com == "type") Console.WriteLine("Displays the contents of a text file​​​");
            else if (com == "import") Console.WriteLine("import text file(s) from your computer");
            else if (com == "export") Console.WriteLine("export text file(s) to your computer");
            else if (com == "cd") Console.WriteLine("Changes the current default directory to . If the argument is not present, report the current directory. If the directory does not exist an appropriate error should be reported.");

            else Console.WriteLine("\nThis command is not supported by the help utility.");


        }
        public void cls()
        {
            Console.Clear();
        }
        public void quit()
        {
            Environment.Exit(0);
        }
        public void md(string name)
        {
            //checking if the directory doesn't exist so we can create it
           
            if (FAT32_Simulation.currentDirectory.SearchDirectory(name) == -1)
            {
                //convert string to arr of char to pass it
                char[] temp = new char[11];
                if (name.Length >= 11)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = name[i];
                    }
                }
                else
                {
                    int i;
                    for (i = 0; i < name.Length; i++)
                    {
                        temp[i] = name[i];
                    }
                    for (; i < temp.Length; i++)
                    {
                        temp[i] = ' ';
                    }

                }
                DirectoryEntry newDir = new DirectoryEntry(temp, 0x10,0, 0);
                FAT32_Simulation.currentDirectory.DirectoryTable.Add(newDir);
                FAT32_Simulation.currentDirectory.WriteDirectory();
                //ask your ta
                if (FAT32_Simulation.currentDirectory.Parent != null)
                {
                    FAT32_Simulation.currentDirectory.Parent.UpdateContent(FAT32_Simulation.currentDirectory);
                }

            }
            else
            {
                Console.WriteLine("this folder already exist.");
            }
        }
        public void rd(string name)
        {
            
            int index = FAT32_Simulation.currentDirectory.SearchDirectory(name);
            if (index != -1)
            {
                int firCluster = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_firstCluster;
                Directorry d = new Directorry(FAT32_Simulation.currentDirectory, name.ToCharArray(), 0x10, firCluster, 0);
                d.DeleteDirectory();
            }
            else
            {
                Console.WriteLine("error:the system cannot find the folder specfied");
            }
        }
        public void cd(string name)
        {
            char[] temp = new char[11];
            if (name.Length >= 11)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = name[i];
                }
            }
            else
            {
                int i;
                for (i = 0; i < name.Length; i++)
                {
                    temp[i] = name[i];
                }
                for (; i < temp.Length; i++)
                {
                    temp[i] = ' ';
                }

            }
            int index = FAT32_Simulation.currentDirectory.SearchDirectory(name);
            if (index != -1)
            {
                int firCluster = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_firstCluster;
                Directorry d = new Directorry(FAT32_Simulation.currentDirectory, temp, 0x10, firCluster, 0);
                FAT32_Simulation.currentDirectory = d;
                string Name = new string(FAT32_Simulation.currentDirectory.dir_name);
                FAT32_Simulation.CurrentPath += '\\' + Name;

            }
            else
            {
                Console.WriteLine("this folder does not exist");

            }

        }
        public void dir()
        {
            Console.Write("Directory of :");
            Console.WriteLine(FAT32_Simulation.CurrentPath.Trim()+"\\");
            int numOfFiles = 0;
            int numOfFolders = 0;
            int fileSize = 0;
            for (int i = 0; i < FAT32_Simulation.currentDirectory.DirectoryTable.Count; i++)
            {
                if (FAT32_Simulation.currentDirectory.DirectoryTable[i].dir_attr == 0x0)
                {
                    Console.Write("        ");
                    Console.Write(FAT32_Simulation.currentDirectory.DirectoryTable[i].dir_Size + "  ");

                    Console.WriteLine(FAT32_Simulation.currentDirectory.DirectoryTable[i].dir_name);
                    numOfFiles++;
                    fileSize += FAT32_Simulation.currentDirectory.DirectoryTable[i].dir_Size;
                }
                else if (FAT32_Simulation.currentDirectory.DirectoryTable[i].dir_attr == 0x10)
                {
                    Console.Write("<Dir>  ");
                    Console.WriteLine(FAT32_Simulation.currentDirectory.DirectoryTable[i].dir_name);
                    numOfFolders++;
                }
            }
            Console.Write(numOfFiles);
            Console.Write("  ");
            Console.Write("Files  ");
            Console.Write(fileSize);
            Console.WriteLine(" bytes");
            Console.Write(numOfFolders);
            Console.Write("  ");
            Console.Write("Dires  ");
            Console.Write(FatTable.GetFreeSpace());
            Console.WriteLine(" bytes free");
        }
        public void import(string path)
        {
            if (File.Exists(path))
            {

                int start = path.LastIndexOf('\\');
                string name = "";

                for (int i = start + 1; i < path.Length; i++)
                {
                    name += path[i];
                }
                string content = File.ReadAllText(path);
                int size = content.Length;
   
                char[] temp = new char[11];
                if (name.Length >= 11)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        temp[i] = name[i];
                    }
                }
                else
                {
                    int i;
                    for (i = 0; i < name.Length; i++)
                    {
                        temp[i] = name[i];
                    }
                    for (; i < temp.Length; i++)
                    {
                        temp[i] = ' ';
                    }

                }
                int index = FAT32_Simulation.currentDirectory.SearchDirectory(name);
                FileEntry newFile;
                if (index == -1)
                {
                    if (size > 0)
                    {
                         newFile=new FileEntry(FAT32_Simulation.currentDirectory, temp, 0x0, FatTable.GetAvailableBlock(), size, content);
                    }
                    else
                    {
                        newFile = new FileEntry(FAT32_Simulation.currentDirectory, temp, 0x0, 0, size, content);
                    }
                    newFile.WriteFile();
                    DirectoryEntry dir = new DirectoryEntry(temp, 0x0, 0, size);
                    FAT32_Simulation.currentDirectory.DirectoryTable.Add(dir);
                    FAT32_Simulation.currentDirectory.WriteDirectory();
                }
                else
                {
                    Console.WriteLine("error:this file already exists");
                }



            }
            else
            {
                Console.WriteLine("error:this file does not exist in your pc");
            }
        }
        public void type(string fileName)
        {
            char[] temp = new char[11];
             int i;
                for (i = 0; i < fileName.Length; i++)
                {
                    temp[i] = fileName[i];
                }
                for (; i < temp.Length; i++)
                {
                    temp[i] = ' ';
                }

            
            int index = FAT32_Simulation.currentDirectory.SearchDirectory(fileName);
            if (index != -1)
            {
                int firCluster = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_firstCluster;
                int fileSize = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_Size;
                string content = null;
                FileEntry file = new FileEntry(FAT32_Simulation.currentDirectory, temp, 0x0, firCluster, fileSize, content);
                file.readFile();
                Console.WriteLine(file.content);

            }
            else
            {
                Console.WriteLine("the system cannot find the file spesfied");
            }
        }
        public void export(string source, string destination)
        {
            char[] temp = new char[11];
            if (source.Length >= 11)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = source[i];
                }
            }
            else
            {
                int i;
                for (i = 0; i < source.Length; i++)
                {
                    temp[i] = source[i];
                }
                for (; i < temp.Length; i++)
                {
                    temp[i] = ' ';
                }

            }
            int index = FAT32_Simulation.currentDirectory.SearchDirectory(source);
            if (index != -1)
            {
                bool isExist = System.IO.Directory.Exists(destination);
                if (isExist)
                {
                    int firCluster = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_firstCluster;
                    int fSize = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_Size;
                    string content = null;
                    FileEntry file = new FileEntry(FAT32_Simulation.currentDirectory, temp, 0x0, firCluster, fSize, content);
                    file.readFile();
                    string path = destination + '\\' + source;
                    StreamWriter stream = new StreamWriter(path);
                    stream.Write(file.content);
                    stream.Flush();
                    stream.Close();


                }
                else
                {
                    Console.WriteLine("The system cannot find the path specified.");
                }
            }
            else
            {
                Console.WriteLine("the system cannot find the file specified.");
            }
        }
        public void rename(string oldName, string newName)
        {
            int index = FAT32_Simulation.currentDirectory.SearchDirectory(oldName);
            if (index != -1)
            {
                int index_newName = FAT32_Simulation.currentDirectory.SearchDirectory(newName);
            
                if (index_newName == -1)
                {
                    char[] temp = new char[11];
                    if (newName.Length >= 11)
                    {
                        for (int i = 0; i < temp.Length; i++)
                        {
                            temp[i] = newName[i];
                        }
                    }
                    else
                    {
                        int i;
                        for (i = 0; i < newName.Length; i++)
                        {
                            temp[i] = newName[i];
                        }
                        for (; i < temp.Length; i++)
                        {
                            temp[i] = ' ';
                        }

                    }
                    FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_name = temp;
                    FAT32_Simulation.currentDirectory.WriteDirectory();
                }
                else
                {
                    Console.WriteLine("duplicate file name exists");
                }

            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
        public void del(string name)
        {
            int index = FAT32_Simulation.currentDirectory.SearchDirectory(name);
            if (index != -1)
            {
                if (FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_attr == 0x0)
                {
                    int firCluster = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_firstCluster;
                    int size = FAT32_Simulation.currentDirectory.DirectoryTable[index].dir_Size;
                    //convert the name to an array of chars
                    char[] temp = new char[11];
   
                    temp = name.ToCharArray();
                    FileEntry file = new FileEntry(FAT32_Simulation.currentDirectory, temp, 0x0, firCluster, size, null);
                    file.deleteFile();
                }
                else
                {
                    Console.WriteLine("The system cannot find the path specified.");
                }
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }

        }
        public List<string> parse(string input)
        {

            string temp = "";
            List<string> inputt = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != ' ')
                {
                    temp += input[i];
                }
                else
                {
                    inputt.Add(temp);
                    temp = "";
                }
            }
            inputt.Add(temp);

            return inputt;
        }

    }
}
