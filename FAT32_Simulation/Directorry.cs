using System;
using System.Collections.Generic;
using System.Text;
namespace asd
{
    public class Directorry : DirectoryEntry
    {
        public List<DirectoryEntry> DirectoryTable;
        public Directorry Parent;

        public Directorry(Directorry p, char[] name, byte attr, int firCluster, int dirSize)
            : base(name, attr, firCluster, dirSize)
        {
            DirectoryTable = new List<DirectoryEntry>();
            if (p != null)
            {
                this.Parent = p;
            }

        }

        public void WriteDirectory()
        {
            byte[] dirTableBytes = new byte[32 * DirectoryTable.Count];
            byte[] dirEntryBytes = new byte[32];
            for (int i = 0; i < DirectoryTable.Count; i++)
            {
               
                dirEntryBytes = DirectoryTable[i].GetBytes();
                for (int j = i * 32, c = 0; c < 32; j++, c++)
                {
                    dirTableBytes[j] = dirEntryBytes[c];
                }

            }

        
            double num_Of_Required_Blocks = Math.Ceiling(dirTableBytes.Length / 1024.0);
            int numOfFullBlocks = (dirTableBytes.Length/ 1024);
            int  reminder = dirTableBytes.Length % 1024;
           // int xx = (int)num_Of_Required_Blocks;
            if (num_Of_Required_Blocks <= FatTable.getAvailableBlocks())
            {
                List<byte[]> blocks = new List<byte[]>();
                byte[] aa = new byte[1024];

                for (int i = 0; i < numOfFullBlocks; i++)
                {
                    for(int j=i*1024,c=0;c<1024;c++,j++)
                    aa[c]= dirTableBytes[j];
                    blocks.Add(aa);

                }
                if (reminder > 0)
                {
                    aa = new byte[1024];
                    int start = numOfFullBlocks * 1024;
                    for (int j = start; j < (start + reminder); j++)
                        aa[j%1024]= dirTableBytes[j];
                    blocks.Add(aa);

                }

                int fatIndex, lastIndex = -1;
                if (dir_firstCluster != 0)
                {
                    fatIndex = dir_firstCluster;
                }
                else
                {
                    fatIndex = FatTable.GetAvailableBlock();
                    dir_firstCluster = fatIndex;

                }

                for (int i = 0; i < blocks.Count; i++)
                {
                    VirtualDisk.WriteBlock(blocks[i], fatIndex);
                    FatTable.SetNext(fatIndex, -1);
                    if (lastIndex != -1)
                    {
                        FatTable.SetNext(lastIndex, fatIndex);
                    }
                    lastIndex = fatIndex;

                    fatIndex = FatTable.GetAvailableBlock();
                }

                FatTable.WriteFatTable();

            }
            if (dir_firstCluster != 0)
            {
                if (DirectoryTable.Count == 0)
                {
                    FatTable.SetNext(dir_firstCluster, 0);
                    dir_firstCluster = 0;

                }

            }
            else
            {
                Console.WriteLine("There are no available blocks");
            }
        }
        public void ReadDirectory()
        {

            List<byte> ls = new List<byte>();
            int fatIndex, next;
            if (dir_firstCluster != 0 && FatTable.GetNext(dir_firstCluster)!=0)
            {
                fatIndex = dir_firstCluster;
                next = FatTable.GetNext(fatIndex);
                do
                {
                    ls.AddRange(VirtualDisk.GetBlock(fatIndex));

                    fatIndex = next;
                    if (fatIndex != -1)
                        next = FatTable.GetNext(fatIndex);

                }
                while (fatIndex != -1);
            }
            byte[] temp = new byte[32];
            //int k = 0;
            for (int i = 0; i < ls.Count; i++)
            {
                temp[i%32]= ls[i];
                if ((i + 1) % 32 == 0)
                {
                    DirectoryEntry d = getDirectoryEntry(temp);
                    if (d.dir_name[0] != '\0')
                        DirectoryTable.Add(DirectoryEntry.getDirectoryEntry(temp));


                }
            }



        }
        public int SearchDirectory(string file_name)
        {
            if (file_name.Length < 11)
            {
                for (int i = file_name.Length; i < 11; i++)
                    file_name += ' ';

            }
            for (int i = 0; i < DirectoryTable.Count; i++)
            {
                string name = new string(DirectoryTable[i].dir_name);
               
                if (name==file_name)
                {
                   
                    return i;
                }

            }
            return -1;
        }
        public void UpdateContent(DirectoryEntry d)
        {
            ReadDirectory();
            string name = new string(d.dir_name);
            if (SearchDirectory(name) != -1)
            {
                int index = SearchDirectory(name);
                DirectoryTable.RemoveAt(index);
                DirectoryTable.Insert(index, d);

            }

        }
        public void DeleteDirectory()
        {
            if (dir_firstCluster != 0)
            {
                int index = dir_firstCluster;
                int next = FatTable.GetNext(index);
                do
                {
                    FatTable.SetNext(index, 0);
                    index = next;
                    if (index != -1)
                    {
                        next = FatTable.GetNext(index);
                    }

                }
                while (index != -1);
            }
            if (Parent != null)
            {
                Parent.ReadDirectory();
                string name = new string(dir_name);
                int index = Parent.SearchDirectory(name);
                if (index != -1)
                {
                    Parent.DirectoryTable.RemoveAt(index);
                    Parent.WriteDirectory();
                }


            }
            FatTable.WriteFatTable();
        }

    }
}
