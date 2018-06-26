using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

namespace MZ.MongoProvider
{
    public class ConfigurationHelper
    {
        #region 数据库连接字符串
        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public static string GetConnectionStringsConfig(string connectionName)
        {
            if (ConfigurationManager.ConnectionStrings[connectionName] == null)
            {
                throw new ArgumentNullException(string.Format("不存在 {0} 的数据库连接配置项"), connectionName);
            }
            string connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString.ToString();
            return connectionString;
        }

        /// <summary>
        /// 更新连接字符串
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="newConString"></param>
        /// <param name="newProviderName"></param>
        public static void UpdateConnectionStringsConfig(string newName, string newConString, string newProviderName)
        {
            // 记录该连接串是否已经存在
            bool isModified = false;
            // 如果要更改的连接串已经存在
            if (ConfigurationManager.ConnectionStrings[newName] != null)
            {
                isModified = true;
            }
            //新建一个连接字符串实例
            ConnectionStringSettings mySettings =
                new ConnectionStringSettings(newName, newConString, newProviderName);
            // 打开可执行的配置文件*.exe.config
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // 如果连接串已存在，首先删除它
            if (isModified)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(newName);
            }
            // 将新的连接串添加到配置文件中.
            config.ConnectionStrings.ConnectionStrings.Add(mySettings);
            // 保存对配置文件所作的更改
            config.Save(ConfigurationSaveMode.Modified);
            // 强制重新载入配置文件的ConnectionStrings配置节
            ConfigurationManager.RefreshSection("ConnectionStrings");
        }
        #endregion

        #region AppSetting配置节点
        /// <summary>
        /// 获取AppSetting的配置节点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String GetConfigAppSetting(String key)
        {
            return ConfigurationHelper.GetConfigAppSetting(key, string.Empty);
        }
        /// <summary>
        /// 获取AppSetting的配置节点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">如果找不到配置节点，返回的默认值</param>
        /// <returns></returns>
        public static string GetConfigAppSetting(string key, string defaultValue)
        {
            string setting = ConfigurationManager.AppSettings[key];

            return setting == null ? defaultValue : setting;
        }
        #endregion

        #region 自定义节点
        /// <summary>
        /// 获取自定义节点的配置
        /// </summary>
        /// <param name="sectionName">自定义节点名称</param>
        /// <returns></returns>
        public static IDictionary GetConfigSection(string sectionName)
        {
            return (IDictionary)ConfigurationManager.GetSection(sectionName);
        }
        public static string GetConfigSection(string sectionGroupName, string sectionName, string sectionKey)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ConfigurationSectionGroup applicationSectionGroup = config.GetSectionGroup(sectionGroupName);
            ConfigurationSection applicationConfigSection = applicationSectionGroup.Sections[sectionName];
            ClientSettingsSection clientSection = (ClientSettingsSection)applicationConfigSection;

            return clientSection.Settings.Get(sectionKey).Value.ValueXml.InnerText;
        }

 

        #endregion
    }
}
