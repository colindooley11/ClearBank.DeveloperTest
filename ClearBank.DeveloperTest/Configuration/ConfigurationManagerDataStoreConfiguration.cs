using System.Configuration;

namespace ClearBank.DeveloperTest.Configuration;

public class ConfigurationManagerDataStoreConfiguration : IDataStoreConfiguration
{
    public string DataStoreType { get; } = ConfigurationManager.AppSettings["DataStoreType"];
}