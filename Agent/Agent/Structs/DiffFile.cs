using System.IO;

namespace Agent.Structs
{
    public struct DiffFile // Делимый файл
    {
        public FileInfo data;   // сам файл
        public FileInfo splitExe; // исполняемый файл для разделения
    }
}