using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
    public static class Converter
    {
        public static byte[] ToBytes(int[] array)
        {
            byte[] bytes = null;
            bytes = new byte[array.Length * sizeof(int)];
            int k = 0;
            for (int i = 0; i < array.Length; i++)
            {
                byte[] b = BitConverter.GetBytes(array[i]);
                for (int j = 0; j < b.Length; j++)
                {
                    bytes[k] = b[j];
                    k++;
                }
            }
            //System.Buffer.BlockCopy(array, 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static byte[] StringToBytes(string s)
        {
            byte[] bytes = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                bytes[i] = (byte)s[i];
            }
            return bytes;
        }
        public static string BytesToString(byte[] bytes)
        {
            string s = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                if ((char)bytes[i] != '\0')
                    s += (char)bytes[i];
                else
                    break;
            }
            return s;
        }
        public static byte[] Directory_EntryToBytes(Directory_Entry d)
        {
            byte[] bytes = new byte[32];
            for (int i = 0; i < d.dir_name.Length; i++)
            {
                bytes[i] = (byte)d.dir_name[i];
            }
            bytes[11] = d.dir_attr;
            int j = 12;
            for (int i = 0; i < d.dir_empty.Length; i++)
            {
                bytes[j] = d.dir_empty[i];
                j++;
            }
            byte[] fc = BitConverter.GetBytes(d.dir_firstCluster);
            for (int i = 0; i < fc.Length; i++)
            {
                bytes[j] = fc[i];
                j++;
            }
            byte[] sz = BitConverter.GetBytes(d.dir_filesize);
            for (int i = 0; i < sz.Length; i++)
            {
                bytes[j] = sz[i];
                j++;
            }
            return bytes;
            
        }


        public static Directory_Entry BytesToDirectory_Entry(Byte[] bytes)
        {
            char[] name = new char[11];
            for (int i = 0; i < name.Length; i++)
            {
                name[i] = (char)bytes[i];
            }
            byte attr = bytes[11];
            byte[] empty = new byte[12];
            int j = 12;
            for (int i = 0; i < empty.Length; i++)
            {
                empty[i] = bytes[j];
                j++;
            }
            byte[] fc = new byte[4];
            for (int i = 0; i < fc.Length; i++)
            {
                fc[i] = bytes[j];
                j++;
            }
            int firstcluster = BitConverter.ToInt32(fc, 0);
            byte[] sz = new byte[4];
            for (int i = 0; i < sz.Length; i++)
            {
                sz[i] = bytes[j];
                j++;
            }
            int filesize = BitConverter.ToInt32(sz, 0);
            Directory_Entry d = new Directory_Entry(new string(name), attr, firstcluster);
            d.dir_empty = empty;
            d.dir_filesize = filesize;
            return d;
            
        }
        public static byte[] ToBytes(List<Directory_Entry> array)
        {
            List<byte> ls = new List<byte>();
            for (int i = 0; i < array.Count; i++)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                if (array[i] != null)
                {
                    bf.Serialize(ms, array[i]);
                    ls.AddRange(ms.ToArray());
                }
                else
                {
                    Directory_Entry d = new Directory_Entry("root", 0x10, 5);
                    bf.Serialize(ms, d);
                    ls.AddRange(ms.ToArray());
                }
            }
            return ls.ToArray();

        }
        public static int[] ToInt(byte[] bytes)
        {
            int[] ints = null;
            ints = new int[bytes.Length / sizeof(int)];
            int k = 0;
            for (int i = 0; i < ints.Length; i++)
            {
                byte[] b = new byte[4];
                for (int j = 0; j < b.Length; j++)
                {
                    b[j] = bytes[k];
                    k++;
                }
                ints[i] = BitConverter.ToInt32(b, 0);
            }

            //System.Buffer.BlockCopy(bytes, 0, ints, 0, bytes.Length);
            return ints;
        }
        public static List<Directory_Entry> ToDirectory_Entry(byte[] bytes)
        {
           
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                List<Directory_Entry> ls = ((List<object>)bformatter.Deserialize(stream)).Cast<Directory_Entry>().ToList();
                return ls;
            }
        }
        public static List<byte[]> splitBytes(byte[] bytes)
        {
            List<byte[]> ls = new List<byte[]>();
            int number_of_arrays = bytes.Length / 1024;
            int rem = bytes.Length % 1024;
            for (int i = 0; i < number_of_arrays; i++)
            {
                byte[] b = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    b[k] = bytes[j];
                }
                ls.Add(b);
            }
            if (rem > 0)
            {
                byte[] b1 = new byte[1024];
                for (int i = number_of_arrays * 1024, k = 0; k < rem; i++, k++)
                {
                    b1[k] = bytes[i];
                }
                ls.Add(b1);
            }
            return ls;
        }
    }
}
