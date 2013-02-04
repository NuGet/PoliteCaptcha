using System.Configuration;

namespace PoliteCaptcha
{
    /// <summary>
    /// Default configuration source that works with app.config/web.config via ConfigurationManager.
    /// </summary>
    internal class DefaultConfigurationSource : IConfigurationSource
    {
        string IConfigurationSource.GetConfigurationValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
