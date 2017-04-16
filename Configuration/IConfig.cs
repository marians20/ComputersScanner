using System;
using System.Collections.Generic;

namespace Configuration
{
    [Obsolete("IConfigFile is deprecated, please use Config class")]
    public interface IConfigFile
    {
        object Read(string fileName);
        bool Write(object data, string fileName);
        void Seed(string fileName);
    }

    public class Settings : Dictionary<string, Dictionary<string, List<string>>>
    {
        private Settings() { }
        private static Settings _instance;
        public static Settings GetInstance()
        {
            return _instance ?? (_instance = new Settings());
        }

        public new void Clear()
        {
            foreach (var key in Keys)
            {
                var item = this[key];
                foreach (var key1 in item.Keys)
                {
                    var item1 = item[key1];
                    item1.Clear();
                    item.Remove(key1);
                }
                Remove(key);
            }
            base.Clear();
        }
    }
}
