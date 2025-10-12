namespace Telemetry_Device.Models.Interface.Files
{
    public interface IFileOperations
    {
        string GetFullPath(string fileName);

        bool FileExists(string fileName);
    }
}
