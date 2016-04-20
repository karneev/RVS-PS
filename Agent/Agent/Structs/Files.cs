using System;

namespace Agent.Structs
{
    [Serializable]
    public struct HandleFile // заголовок файла
    {
        public string fileName; // имя файла
        public long size;       // его размерность
    }
    [Serializable]
    public struct PartFile // часть файла
    {
        public int len; // длина части файла
        public byte[] part; // часть файла
    }
}