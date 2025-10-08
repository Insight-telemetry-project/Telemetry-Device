using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Files;

namespace Telemetry_Device.Services.Files
{
    public class FileOperations: IFileOperations
    {

        public string GetFullPath(string fileName)
        {
            return Path.Combine(ConstantPackets.BASE_DIRECTORY_OF_FILES, fileName);
        }

        public bool FileExists(string fileName)
        {
            string fullPath = GetFullPath(fileName);
            return File.Exists(fullPath);
        }
    }
}
