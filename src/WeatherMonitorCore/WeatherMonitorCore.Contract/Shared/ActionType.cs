namespace WeatherMonitorCore.Contract.Shared;
public enum ActionType
{
    NoAccess = 0,
    Read = 1,
    Write = 2,
    ReadWrite = 3,
    Subscribe = 4,
    ReadAndSubscribe = 5,
    WriteAndSubscribe = 6,
    ReadWriteAndSubscribe = 7
}