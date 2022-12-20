using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace asd
{
    class VirtualDisk
    {

        public static void Initalize()
        {
            string path = @"D:\virtual_disk3.txt";
            if (!File.Exists(path))
            {
                FileStream disk = new FileStream(@"D:\virtual_disk3.txt", FileMode.Create, FileAccess.Write);
                //super block(the first block)
                for (int i = 0; i < 1024; i++)
                {
                    //disk.WriteByte(0);
                    disk.WriteByte((byte)'0');
                }
                ////////////////////////////////////

                //Fat table
                for (int i = 0; i < 4 * 1024; i++)
                {
                    disk.WriteByte((byte)'*');

                }
                //Inialize File Data
                for (int i = 0; i < 1019 * 1024; i++)
                {
                    disk.WriteByte((byte)'#');
                }
                disk.Close();

                FatTable.Iinialize();
                //char[] root_name = new char[1];
                //root_name[0] = 'S';
                Directorry root = new Directorry(null, "S:".ToCharArray(), 0x10, 5, 0);
                root.WriteDirectory();
                // FatTable.WriteFatTable();
                FAT32_Simulation.currentDirectory = root;



            }
            else
            {
                
                FatTable.GetFatTable();
                //  char[] root_name = new char[1];
                //  root_name[0] = 'S';
                Directorry root = new Directorry(null, "S:".ToCharArray(), 0x10, 5, 0);
                root.ReadDirectory();
                //FatTable.WriteFatTable();
                FAT32_Simulation.currentDirectory = root;

            }

        }

        public static void WriteBlock(byte[] data, int index)
        {
            byte[] buffer = new byte[1024];
            FileStream disk = new FileStream(@"D:\virtual_disk3.txt", FileMode.Open, FileAccess.Write);
            disk.Seek(1024 * index, SeekOrigin.Begin);
            disk.Write(buffer, 0, buffer.Length);
            disk.Close();
        }
        public static byte[] GetBlock(int index)
        {
            FileStream disk = new FileStream(@"D:\virtual_disk3.txt", FileMode.Open, FileAccess.Read);
            disk.Seek(1024 * index, SeekOrigin.Begin);
            byte[] BlockData = new byte[1024];
            disk.Read(BlockData, 0, BlockData.Length);
            disk.Close();
            return BlockData;
        }

    }

}
