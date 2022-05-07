using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
    public class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
        public File_Entry(string name, byte dir_attr, int dir_firstCluster, Directory pa) : base(name, dir_attr, dir_firstCluster)
        {
            content = string.Empty;
            if (pa != null)
                parent = pa;
        }
        public Directory_Entry GetDirectoryEntry()
        {
            Directory_Entry opject = new Directory_Entry(new string(this.dir_name), this.dir_attr, this.dir_firstCluster);
            return opject;
        }
        public void writeContent()
        {
            byte[] content_bytes = Converter.StringToBytes(content);
            List<byte[]> bytes_lest = Converter.splitBytes(content_bytes);
            int index;
            if (this.dir_firstCluster != 0)
                index = this.dir_firstCluster;
            else
            {
                index = Mini_FAT.GetAvilableBlock();
                this.dir_firstCluster = index;
            }
            int last_index = -1;
            for (int i = 0; i < bytes_lest.Count; i++)
            {
                if (Mini_FAT.GetAvilableBlocks()>= bytes_lest.Count)
                {
                    Virtual_Disk.WriteBlock(bytes_lest[i], index, 0, bytes_lest[i].Length);
                    Mini_FAT.SetNextBlock(index, -1);
                    if (last_index != -1)
                        Mini_FAT.SetNextBlock(last_index, index);
                    last_index = index;
                    index = Mini_FAT.GetAvilableBlock();
                }
            }
        }
        public void ReadContent()
        {
            if (this.dir_firstCluster != 0)
            {
                content = string.Empty;
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
                content = Converter.BytesToString(lest.ToArray());
            }
        }
        public void DeleteFile()
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
                } while (index != -1);
            }
            if (this.parent != null)
            {
                int index = this.parent.searchDirectory(new string(this.dir_name));
                if (index != -1)
                {
                    this.parent.directory_table.RemoveAt(index);
                    this.parent.WriteDirectory();
                    Mini_FAT.WriteFat();
                }
            }
        }
    }
}
