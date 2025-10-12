using System.Text.Json;
using Telemetry_Device.Models.Interface.Icd;

namespace Telemetry_Device.Services.Icd
{
    public class IcdFileProvider : IIcdProvider
    {
        private const string FilePath = "Data/icd.json";

        public List<IcdField> LoadIcd()
        {
            string icdJson = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<IcdField>>(icdJson)!;
        }
    }
}
