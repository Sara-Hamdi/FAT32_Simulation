using System;
using System.Collections.Generic;
using System.Text;

namespace asd
{
    public class FileEntry : DirectoryEntry
    {

        public Directorry Parent;
        public string content;
        public FileEntry(Directorry p, char[] name, byte attr, int firCluster, int fileSize, string cont)
          : base(name, attr, firCluster, fileSize)
        {
            if (p != null)
            {
                this.Parent = p;
            }
            this.content = cont;
        }
        public void WriteFile()
        {
            ///content in bytes to write it
            byte[] bytes = Encoding.ASCII.GetBytes(content);


            double numOfRequiredBlock = Math.Ceiling(content.Length / 1024.0);
            int numOfFullBlocks = (content.Length / 1024);
            int reminder = content.Length % 1024;
            int fat_index = 0;
            int lastIndex = -1;
            if (numOfRequiredBlock <= FatTable.getAvailableBlocks())
            {
                List<byte[]> blocks = new List<byte[]>();

                byte[] a;
                if (numOfFullBlocks > 0)
                {
                    int c = 0;
                    for(int i=0;i<numOfFullBlocks;i++)
                    {
                        a = new byte[1024];
                        for(int j=0;j<1024;j++)
                        {
                            a[j] = bytes[c];
                            c++;
                        }
                        blocks.Add(a);
                    }

                }
                if(reminder>0)
                {
                    int x = 1024 * numOfFullBlocks;
                    byte[] aa = new byte[1024];
                    for(int i=0;i<reminder;i++)
                    {
                        aa[i] = bytes[x];
                        x++;
                    }
                    blocks.Add(aa);
                }
                if (dir_firstCluster != 0)
                {
                    fat_index = dir_firstCluster;
                }
                else
                {
                    fat_index = FatTable.GetAvailableBlock();
                    dir_firstCluster = fat_index;
                }

                for (int i = 0; i < blocks.Count; i++)
                {
                    VirtualDisk.WriteBlock(blocks[i], fat_index);
                    FatTable.SetNext(fat_index, -1);
                    if (lastIndex != -1)
                    {
                        FatTable.SetNext(lastIndex, fat_index);
                    }
                    
                    lastIndex = fat_index;
                    fat_index = FatTable.GetAvailableBlock();
                }

                FatTable.WriteFatTable();
            }

        }
        public void readFile()
        {
            int fatIndex = 0;
            int next = 0;
            List<byte> Bytes = new List<byte>();
            if (dir_firstCluster != 0 && FatTable.GetNext(dir_firstCluster) != 0)
            {
                fatIndex = dir_firstCluster;
                next = FatTable.GetNext(fatIndex);
                do
                {
                    Bytes.AddRange(VirtualDisk.GetBlock(fatIndex));
                    fatIndex = next;
                    if (fatIndex != -1)
                        next = FatTable.GetNext(fatIndex);


                }
                while (next != -1);
                //convert list of bytes to array then to string and assgin it to content

                content = "";
                for(int i=0;i<Bytes.Count;i++)
                {
                    content += (char)Bytes[i];
                }
            }
        }


        public void updateContent(FileEntry file)
        {
            readFile();
            this.content = file.content;
            WriteFile();

        }
        public void deleteFile()
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
                string temp = new string(dir_name);
                int I = Parent.SearchDirectory(temp);
                if (I != -1)
                {
                    Parent.DirectoryTable.RemoveAt(I);
                    Parent.WriteDirectory();
                    
                }
                Console.WriteLine("hhh");
            }
            FatTable.WriteFatTable();
        }

    }
}
