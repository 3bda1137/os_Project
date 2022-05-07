using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
    public static class Mini_FAT
    {
        public static int[] FAT = new int[1024];
        public static void CreatFat()
        {
            for (int i = 0; i < FAT.Length; i++)
            {
                if (i == 0 || i == 4)
                    FAT[i] = -1;
                else if (i > 0 && i <= 3)
                    FAT[i] = i + 1;
                else
                    FAT[i] = 0;
            }
        }
        public static void WriteFat()
        {
            byte[] fat_bytes = Converter.ToBytes(Mini_FAT.FAT);
            List<byte[]> lest = Converter.splitBytes(fat_bytes);
            for (int i = 0; i < lest.Count; i++)
                Virtual_Disk.WriteBlock(lest[i], i + 1, 0, lest[i].Length);
        }
        public static void ReadFat()
        {
            List<byte> lest = new List<byte>();
            for (int i = 1; i <= 4; i++)
                lest.AddRange(Virtual_Disk.ReadBlock(i));
            FAT = Converter.ToInt(lest.ToArray());
        }
        public static void PrintFat()
        {
            for (int i = 0; i < FAT.Length; i++)
                Console.WriteLine("FAT[" + i + "] = " + FAT[i]);
        }
        public static int GetAvilableBlock()
        {
            for (int i = 0; i < FAT.Length; i++)
                if (FAT[i] == 0)
                    return i;
            return -1;
        }
        public static int GetAvilableBlocks()
        {
            int cont=0;
            for (int i = 0; i < FAT.Length; i++)
                if (FAT[i] == 0)
                    cont++;
            return cont;
        }
        public static void SetNextBlock(int index, int next)
        {
            FAT[index] = next;
        }
        public static int GetNextBlock(int index)
        {
                return FAT[index];
        }
    }
}
