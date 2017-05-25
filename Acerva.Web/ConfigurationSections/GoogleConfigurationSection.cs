using System.Configuration;

namespace Acerva.Web.ConfigurationSections
{
    public class GoogleConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("clientId")]
        public string ClientId
        {
            get { return (string)base["clientId"]; }
            set { base["clientId"] = value; }
        }

        [ConfigurationProperty("clientSecret")]
        public string ClientSecret
        {
            get { return (string)base["clientSecret"]; }
            set { base["clientSecret"] = value; }
        }

        public GoogleConfigurationSection()
        {
        }

    }
}