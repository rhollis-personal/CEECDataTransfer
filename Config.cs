using System;
using System.Configuration;
using System.Xml;

namespace CeecDataTransfer
{
    public class Config
    {
       public static void UpdateConfigSettings(string KeyName, string KeyValue)
        {
            XmlDocument XmlDoc = new XmlDocument();

            XmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            foreach (XmlElement xElement in XmlDoc.DocumentElement)
            {
                if (xElement.Name == "appSettings")
                {

                    foreach (XmlNode xNode in xElement.ChildNodes)
                    {
                        if (xNode.Attributes[0].Value == KeyName)
                        {
                            xNode.Attributes[1].Value = KeyValue;
                        }
                    }
                }
            }
            XmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        public static string GetConfigSettings(string KeyName)
        {
            string value = "";
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            foreach (XmlElement xElement in XmlDoc.DocumentElement)
            {
                if (xElement.Name == "appSettings")
                {
                    foreach (XmlNode xNode in xElement.ChildNodes)
                    {
                        if (xNode.Attributes[0].Value == KeyName)
                        {
                            value = xNode.Attributes[1].Value;
                        }
                    }
                }
            }

            return value;
        }

        public static string ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings[key] ?? "Not Found";
        }

        public static void UpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                settings[key].Value = value;

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
        public static void LogError(Exception error)
        {
            //Stub for error logging
        }
    }
}
