using Agent.Enums;

namespace Agent.Model
{
    public delegate void RefreshData(); // делегат изменения представления
    public delegate void SetStat(StatusMachine status); // делегат изменение статуса
    public delegate void UpdateProgressBar(double percent, string text); // делегат изменения представления
    public delegate void GetMessage(ILockeded sender, string message); // делегат получение сообщения
}