using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
    public static class Virtual_Disk
    {
        public static FileStream File_Disk;
        public static void CreatFile(string path)
        {
            File_Disk = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
        public static int getFreeSpace()
        {
            return (1024 * 1024) - (int)File_Disk.Length;
        }
        public static void In()
        {
            byte[] b = new byte[1024];
            for (int i = 0; i < b.Length; i++)
                b[i] = 0;
            WriteBlock(b, 0);
        }
        public static void Initalize(string path)
        {
            if (!File.Exists(path))
            {
                CreatFile(path);
                In();
                Mini_FAT.CreatFat();
                Directory root = new Directory("A:", 0x10, 5, null);
                root.WriteDirectory();
                Mini_FAT.SetNextBlock(5, -1);
                Program.current = root;
                Mini_FAT.WriteFat();
            }
            else
            {
                CreatFile(path);
                Mini_FAT.ReadFat();
                Directory root = new Directory("A:", 0x10, 5, null);
                root.ReadDirectory();
                Program.current = root;
            }
        }
        public static void WriteBlock(byte[] block, int index, int offset = 0, int count = 1024)
        {
            File_Disk.Seek(index * 1024, SeekOrigin.Begin);
            File_Disk.Write(block, offset, count);
            File_Disk.Flush();
        }
        public static byte[] ReadBlock(int index)
        {
            File_Disk.Seek(index * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            File_Disk.Read(bytes, 0, 1024);
            return bytes;
        }
    }
}
