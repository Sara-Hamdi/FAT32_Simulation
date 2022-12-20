using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace asd
{

    public class DirectoryEntry
    {

        public char[] dir_name = new char[11];

        //0x0 ->file
        //0x10->folder
        public byte dir_attr;
        //////////////////////

        public byte[] dir_empty = new byte[12];
        public int dir_firstCluster; //4 bytes
        public int dir_Size;     //4 bytes

        public DirectoryEntry()
        { }

        public DirectoryEntry(char[] name, byte attr, int firCluster, int dirSize)
        {
            string file_name = new string(name);
            if (file_name.Length < 11)
            {
                for (int i = file_name.Length; i < 11; i++)
                    file_name += " ";

            }
            this.dir_name = file_name.ToCharArray();
            this.dir_attr = attr;
            this.dir_firstCluster = firCluster;
            this.dir_Size = dirSize;
        }

        public byte[] GetBytes()
        {
            byte[] record = new byte[32];

            //write dir name
            byte[] buffer = new byte[11];
            buffer = Encoding.ASCII.GetBytes(dir_name);
            for (int i = 0; i < 11; i++)
            {
                if (i < buffer.Length)
                    record[i] = buffer[i];
                else
                    record[i] =(byte)' ';
            }
            ///////////////////////

            //write dir attribute
            record[11] = dir_attr;
            //////////////////////

            //write dir empty
            int j = 0;
            for (int i = 12; i < 24; i++)
            {
                record[i] = dir_empty[j];
                j++;
            }
            //////////////////////

            //write dir size
            byte[] b = new byte[4];
            b = BitConverter.GetBytes(dir_Size);
            int g = 0;
            for (int i = 24; i < 28; i++)
            {
                record[i] = b[g];
                g++;
            }
            //////////////////////////


            //write dir first cluster
            byte[] temp;
            temp = BitConverter.GetBytes(dir_firstCluster);
            int k = 0;
            for (int i = 28; i < 32; i++)
            {
                record[i] = b[k];
                k++;
            }

            return record;

        }
        public static DirectoryEntry getDirectoryEntry(byte[] b)
        {
            DirectoryEntry d = new DirectoryEntry();


            /////dir name
            
            byte[] dir_name_tempp = new byte[12];

            for (int i = 0; i < 11; i++)
            {
                d.dir_name[i] =(char) b[i];
            }
        

            /////dir attr
            d.dir_attr = b[11];
            //////////

            //write dir empty
            int j = 0;
            for (int i = 12; i < 24; i++)
            {
                d.dir_empty[j] = b[i];
                j++;
            }
            //////////////////////



            //write dir size

            int g = 0;
            byte[] temp = new byte[4];
            for (int i = 24; i < 28; i++)
            {
                temp[g] = b[i];
                g++;
            }
            d.dir_Size = BitConverter.ToInt32(temp,0);


            //write dir first cluster
            byte[] templ = new byte[4];

            int k = 0;
            for (int i = 28; i < 32; i++)
            {
                templ[k] = b[i];
                k++;
            }
            d.dir_firstCluster = BitConverter.ToInt32(templ,0);

            return d;
        }



    }
}
