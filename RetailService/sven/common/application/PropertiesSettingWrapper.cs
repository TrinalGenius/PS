using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using RetailService.Properties;

namespace WebSpiderOfPostcode.sven.common.application
{
    class PropertiesSettingWrapper : DynamicObject
    {

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string key = binder.Name;
            if (Settings.Default.Properties[key] != null)
            {
                
                result = Settings.Default.Properties[key].DefaultValue;
            }
            else
            {
                result = null;
            }
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string key = binder.Name;
            if (Settings.Default.Properties[key] == null)
            {
                var property = new SettingsProperty(key);
                property.DefaultValue = value;
                property.IsReadOnly = false;
                property.Provider = Settings.Default.Providers["LocalFileSettingsProvider"];
                property.Attributes.Add(typeof(System.Configuration.UserScopedSettingAttribute), new System.Configuration.UserScopedSettingAttribute());
                Settings.Default.Properties.Add(property);
                Settings.Default.Save();
            }
            else
            {
                Settings.Default.Properties[key].DefaultValue = value;
                Settings.Default.Save();
            }
            return true;
        }

        public static void main(String[] args)
        {
            dynamic d = new PropertiesSettingWrapper();
            d.test = 1;
            


        }
    }
}
