namespace TFU002.Interfaces.Services
{
    public interface IDirectoryProvider
    {
        string AssemblyDirectory { get; }
        string DefaultPath { get; }
    }
}