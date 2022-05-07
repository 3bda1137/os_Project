using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_project_sh2
{
    public class Directory_Entry
    {
        public char[] dir_name = new char[11];
        public byte dir_attr;
        public byte[] dir_empty = new byte[12];
        public int dir_firstCluster;
        public int dir_filesize;
        public Directory_Entry()
        {

        }
        public Directory_Entry(string name, byte dir_attr, int dir_firstCluster)
        {
            this.dir_attr = dir_attr;
            //regular file
            if (dir_attr == 0x0)
            {
                string[] fileName = name.Split('.');
                assignFileName(fileName[0].ToCharArray(), fileName[1].ToCharArray());
            }
            else if (dir_attr == 0x10)
            {

                assignDIRName(name.ToCharArray());
            }
            this.dir_firstCluster = dir_firstCluster;
        }
        public void assignFileName(char[] name, char[] extension)
        {
            if (name.Length <= 7 && extension.Length == 3)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.dir_name[i] = name[i];
                }
                j++;
                this.dir_name[j] = '.';
                for (int i = 0; i < extension.Length; i++)
                {
                    j++;
                    this.dir_name[j] = extension[i];
                }
                for (int i = ++j; i < dir_name.Length; i++)
                {
                    this.dir_name[i] = ' ';
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    this.dir_name[i] = name[i];
                }
                this.dir_name[7] = '.';
                for (int i = 0, j = 8; i < extension.Length; j++, i++)
                {
                    this.dir_name[j] = extension[i];
                }
            }
        }
        public void assignDIRName(char[] name)
        {
            if (name.Length <= 11)
            {
                int j = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    j++;
                    this.dir_name[i] = name[i];
                }
                for (int i = ++j; i < dir_name.Length; i++)
                {
                    this.dir_name[i] = ' ';
                }
            }
            else
            {
                int j = 0;
                for (int i = 0; i < 11; i++)
                {
                    j++;
                    this.dir_name[i] = name[i];
                }
            }
        }
    }
}
