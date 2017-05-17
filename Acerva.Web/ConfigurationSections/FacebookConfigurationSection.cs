using System.Configuration;

namespace Acerva.Web.ConfigurationSections
{
    public class FacebookConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("appId")]
        public string AppId
        {
            get { return (string)base["appId"]; }
            set { base["appId"] = value; }
        }

        [ConfigurationProperty("appSecret")]
        public string AppSecret
        {
            get { return (string)base["appSecret"]; }
            set { base["appSecret"] = value; }
        }

        public FacebookConfigurationSection()
        {
        }

    }
}