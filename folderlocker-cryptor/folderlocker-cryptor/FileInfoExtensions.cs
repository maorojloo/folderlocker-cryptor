using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace folderlocker_cryptor
{
    public static class FileInfoExtensions
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, newName));
        }
    }
}
