using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace iPathAuthService.Util
{


    public class MappingConfiguration : ConfigurationSection
    {
        private static readonly ConfigurationProperty s_property
        = new ConfigurationProperty(string.Empty, typeof(MappingCollection), null,
                                        ConfigurationPropertyOptions.IsDefaultCollection);

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public MappingCollection Mapping
        {
            get
            {
                return (MappingCollection)base[s_property];
            }
        }
    }


    /// <summary>
    /// 短网址映射配置
    /// </summary>
    [ConfigurationCollection(typeof(UrlMappingKeyValueSetting))]
    public class MappingCollection : ConfigurationElementCollection
    {
        public MappingCollection() : base(StringComparer.OrdinalIgnoreCase) { }

        new public UrlMappingKeyValueSetting this[string name]
        {
            get
            {
                return (UrlMappingKeyValueSetting)base.BaseGet(name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new UrlMappingKeyValueSetting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UrlMappingKeyValueSetting)element).Key;
        }
    }

    public class UrlMappingKeyValueSetting : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get { return this["key"].ToString(); }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return this["value"].ToString(); }
        }
    }
}