using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Configuration
{

    public class Field
    {
        public string Name { get; set; }
        public string MapTo { get; set; }
        #region ctor
        public Field()
        {
            Name = string.Empty;
            MapTo = string.Empty;
        }
        public Field(string name, string mapTo = "")
        {
            Name = name;
            MapTo = string.IsNullOrEmpty(mapTo) ? name : mapTo;
        }
        #endregion
    }

    public class Kpi
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public List<Field> Fields { get; set; }
        #region ctor
        public Kpi()
        {
            Name = string.Empty;
            Command = string.Empty;
            Fields = new List<Field>();
        }

        public Kpi(string name, string command)
        {
            Name = name;
            Command = command;
            Fields = new List<Field>();
        }

        public Kpi(string name, string command, List<Field> fields)
            :this(name, command)
        {
            Fields = fields;
        }
        #endregion
        public void AddField(Field field)
        {
            Fields.Add(field);
        }

        public void AddField(string name, string mapTo = "")
        {
            Fields.Add(new Field(name, mapTo));
        }

        public void AddFields(Field[] fields)
        {
            foreach(var field in fields)
            {
                Fields.Add(field);
            }
        }
        public void AddFields(string[] fieldsNames)
        {
            foreach(var fieldName in fieldsNames)
            {
                AddField(fieldName);
            }
        }
    }

    public class ConfigItem
    {
        public OperatingSystems OperatingSystem { get; set; }
        public List<Kpi> Kpis { get; set; }

        public ConfigItem()
        {
            Kpis = new List<Kpi>();
        }

        public ConfigItem(OperatingSystems operatingSystem)
            :this()
        {
            OperatingSystem = operatingSystem;
        }

        public void AddKpi(Kpi kpi)
        {
            Kpis.Add(kpi);
        }
    }

    public class Config : List<ConfigItem>
    {
        protected static ISerializer<Config> Serializer;
        protected static object SerializerLock = new object();
        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config();
                }
                return _instance;
            }
        }

        public Config()
        {
            Serializer = SerializerFactory<Config>.GetSerializer(Serializers.Yaml);
        }

        public void Save(string fileName)
        {
            lock (SerializerLock)
            {
                Serializer.SetFileName(fileName);
                Serializer.Save(this);
            }
        }

        public static Config Load(string fileName)
        {
            if(!File.Exists(fileName))
            {
                Seed(fileName);
            }

            lock (SerializerLock)
            {
                Serializer.SetFileName(fileName);
                return Serializer.Load();
            }
        }

        public static void Seed(string fileName)
        {
            Config cfg = new Config();
            Kpi kpi;

            var rpm = new ConfigItem(OperatingSystems.Rpm);
            kpi = new Kpi("Software", "dpkg -l");
            kpi.AddFields(new[] { "Name", "Version", "Architecture", "Description" });
            rpm.AddKpi(kpi);

            cfg.Add(rpm);

            var windows = new ConfigItem(OperatingSystems.Windows);
            kpi = new Kpi("OperatingSystem", "Win32_OperatingSystem");
            kpi.AddFields(new[] {
                "Name", "Manufacturer", "OSArchitecture",
                "SystemDirectory", "Caption"
            });
            windows.AddKpi(kpi);

            kpi = new Kpi("ComputerSystem", "Win32_ComputerSystem");
            kpi.AddFields(new[] {
                "Name", "DNSHostName", "Domain", "Manufacturer",
                "NumberOfLogicalProcessors", "TotalPhysicalMemory"
            });
            windows.AddKpi(kpi);

            kpi = new Kpi("Software", "Win32_Product");
            kpi.AddFields(new[] {
                "Name", "Vendor", "Caption", "Version"
            });
            windows.AddKpi(kpi);

            cfg.Add(windows);

            var aptitude = new ConfigItem(OperatingSystems.Aptitude);
            kpi = new Kpi("Software", "dpkg -l");
            kpi.AddFields(new[] { "Name", "Version", "Architecture", "Description" });
            aptitude.AddKpi(kpi);

            cfg.Add(aptitude);

            cfg.Save(fileName);
        }

        public override string ToString()
        {
            string result;
            var serializer = new Serializer();
            using (var str = new MemoryStream())
            {
                using (var writer = new StreamWriter(str))
                {
                    serializer.Serialize(writer, this);
                    result = Encoding.UTF8.GetString(str.ToArray(), 0, (int)str.Length);
                }
            }
            return result;
        }
    }
}
