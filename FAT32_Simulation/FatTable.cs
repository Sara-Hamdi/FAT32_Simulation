using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace asd
{
    public static class FatTable
    {

        public static int[] Fat_Table = new int[1024];


        // Fat_Table = new int[1024];


        public static void Iinialize()
        {
            //super block
            Fat_Table[0] = -1;
            //fat table////////////////
            Fat_Table[1] = -1;

            Fat_Table[2] = -1;
            Fat_Table[3] = -1;
            Fat_Table[4] = -1;
            /////////////////////////// 

        }
        public static void WriteFatTable()
        {
            FileStream stream = new FileStream(@"D:\virtual_disk3.txt", FileMode.Open, FileAccess.Write,FileShare.ReadWrite);
            stream.Seek(1024, SeekOrigin.Begin);
            for (int i = 0; i < Fat_Table.Length; i++)
            {
                byte[] buffer = new byte[4];
                buffer = BitConverter.GetBytes(Fat_Table[i]);
                stream.Write(buffer, 0, buffer.Length);

            }
            stream.Close();

        }
        public static int[] GetFatTable()
        {

            FileStream stream = new FileStream(@"D:\virtual_disk3.txt", FileMode.Open, FileAccess.Read);
            stream.Seek(1024, SeekOrigin.Begin);
            byte[] buffer = new byte[4 * 1024];
            int[] arr = new int[1024];
            stream.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, arr, 0, buffer.Length);
            stream.Close();
            return arr;
        }
        public static void PrintFatTable()
        {
            int[] arr = new int[1024];
            arr = GetFatTable();
            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i]);
            }
        }
        public static int GetAvailableBlock()
        {
            for (int i = 5; i < Fat_Table.Length; i++)
            {
                if (Fat_Table[i] == 0) return i;
            }

            return -1;
        }
        public static int GetNext(int i)
        {
            return Fat_Table[i];
        }
        public static void SetNext(int index, int value)
        {
            Fat_Table[index] = value;
        }
        public static int getAvailableBlocks()
        {
            int counter = 0;
            for (int i = 0; i < Fat_Table.Length; i++)
            {
                if (Fat_Table[i] == 0)
                {
                    counter++;
                }

            }
            return counter;
        }
        public static int GetFreeSpace()
        {
            int space = getAvailableBlocks() * 1024;
            return space;
        }
    }

}
