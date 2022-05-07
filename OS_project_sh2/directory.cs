using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{

    public class Directory : Directory_Entry
    {
        public List<Directory_Entry> directory_table;
        public Directory parent;
        public Directory(string name, byte dir_attr, int dir_firstCluster, Directory pa) : base(name, dir_attr, dir_firstCluster)
        {
            directory_table = new List<Directory_Entry>();
            if (pa != null)
                parent = pa;
        }
        public void UpdateContent(Directory_Entry d)
        {
            int index = searchDirectory(new string(d.dir_name));
            if (index != -1)
            {
                directory_table.RemoveAt(index);
                directory_table.Insert(index, d);
            }
        }
        public Directory_Entry GetDirectoryEntry()
        {
            Directory_Entry opject = new Directory_Entry(new string(this.dir_name), this.dir_attr, this.dir_firstCluster);
            return opject;
        }
        public void WriteDirectory()
        {
            byte[] dirsory_bytes = new byte[directory_table.Count * 32];
            for (int i = 0; i < directory_table.Count; i++)
            {
                byte[] b = Converter.Directory_EntryToBytes(this.directory_table[i]);
                for (int j = i * 32, k = 0; k < b.Length; k++, j++)
                    dirsory_bytes[j] = b[k];
            }
            List<byte[]> bytes_lest = Converter.splitBytes(dirsory_bytes);
            int index;
            if (this.dir_firstCluster != 0)
                index = this.dir_firstCluster;
            else
            {
                index = Mini_FAT.GetAvilableBlock();
                this.dir_firstCluster = index;
            }
            int next = -1;
            for (int i = 0; i < bytes_lest.Count; i++)
            {
                if (index != -1)
                {
                    Virtual_Disk.WriteBlock(bytes_lest[i], index, 0, bytes_lest[i].Length);
                    Mini_FAT.SetNextBlock(index, -1);
                    if (next != -1)
                        Mini_FAT.SetNextBlock(next, index);
                    next = index;
                    index = Mini_FAT.GetAvilableBlock();
                }
            }
            if (this.parent != null)
            {
                this.parent.UpdateContent(this.GetDirectoryEntry());
                this.parent.WriteDirectory();
            }
            Mini_FAT.WriteFat();
        }
        public void ReadDirectory()
        {
            if (this.dir_firstCluster != 0)
            {
                directory_table = new List<Directory_Entry>();
                int index = this.dir_firstCluster;
                int next = Mini_FAT.GetNextBlock(index);
                List<byte> lest = new List<byte>();
                do
                {
                    lest.AddRange(Virtual_Disk.ReadBlock(index));
                    index = next;
                    if (index != -1)
                        next = Mini_FAT.GetNextBlock(index);
                } while (next != -1);
                for (int i = 0; i < lest.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < lest.Count; m++, k++)
                        b[m] = lest[k];
                    if (b[0] == 0)
                        break;
                    directory_table.Add(Converter.BytesToDirectory_Entry(b));
                }
            }
        }
        public void DeleteDirectory()
        {
            if (this.dir_firstCluster != 0)
            {
                int index = this.dir_firstCluster;
                int next = Mini_FAT.GetNextBlock(index);
                do
                {
                    Mini_FAT.SetNextBlock(index, 0);
                    index = next;
                    if (index != -1)
                        next = Mini_FAT.GetNextBlock(index);
                }
                while (index != -1);
            }
            if (this.parent != null)
            {
                int index = this.parent.searchDirectory(new string(this.dir_name));
                if (index != -1)
                {
                    this.parent.directory_table.RemoveAt(index);
                    this.parent.WriteDirectory();
                }
            }
            if (Program.current == this)
            {
                if (this.parent != null)
                {
                    Program.current = this.parent;
                    Program.currentPath = Program.currentPath.Substring(0, Program.currentPath.LastIndexOf('\\'));
                    Program.current.ReadDirectory();
                }
            }
            Mini_FAT.WriteFat();
        }
        public int searchDirectory(string name)
        {
            if (name.Length < 11)
            {
                name += "\0";
                for (int i = name.Length + 1; i < 12; i++)
                    name += " ";
            }
            else
            {
                name = name.Substring(0, 11);
            }
            for (int i = 0; i < directory_table.Count; i++)
            {
                string n = new string(directory_table[i].dir_name);
                //int l = n.Length;
                //int ln = name.Length;
                if (n.Equals(name))
                    return i;
            }
            return -1;
        }
    }
}
