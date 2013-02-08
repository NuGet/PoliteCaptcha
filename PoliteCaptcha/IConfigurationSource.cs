namespace PoliteCaptcha
{
    public interface IConfigurationSource
    {
        /// <param name="key">name of the configuration value</param>
        /// <returns>null if no such configuration value exists</returns>
        string GetConfigurationValue(string key);
    }
}
