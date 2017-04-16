using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Configuration
{
    [Obsolete("Class YamlConfig is deprecated, please use Config instead.")]
    public class YamlConfig : IConfigFile
    {
        public object Read(string fileName)
        {
            var data = Settings.GetInstance();
            data.Clear();
            if(!File.Exists(fileName))
            {
                Seed(fileName);
            }
            var input = new StreamReader(fileName);
            var yaml = new YamlStream();
            yaml.Load(input);
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
            foreach (var computerType in mapping.Children)
            {
                var key = (YamlScalarNode)computerType.Key;
                var values = (YamlMappingNode)computerType.Value;
                foreach (var kpy in values.Children)
                {
                    var fields = (YamlSequenceNode)kpy.Value;
                    new Dictionary<string, List<string>>().Add(kpy.Key.ToString(),
                        fields.Select(field => field.ToString()).ToList());
                }
                data.Add(key.ToString(), new Dictionary<string, List<string>>());
            }
            return data;
        }

        public bool Write(object data, string fileName)
        {
            var serializer = new Serializer();
            using (TextWriter writer = File.CreateText(fileName))
            {
                serializer.Serialize(writer, data);
            }
            return true;
        }

        public void Seed(string fileName)
        {
            var settings = Settings.GetInstance();

            var windows = new Dictionary<string, List<string>>();
            windows.Add("Win32_OperatingSystem", (new[] { "Name", "Manufacturer", "OSArchitecture", "SystemDirectory", "Description" }).ToList());
            windows.Add("Win32_ComputerSystem", (new[] { "Name", "DNSHostName", "Domain", "Manufacturer", "NumberOfLogicalProcessors", "TotalPhysicalMemory" }).ToList());
            windows.Add("Win32_Volume", (new[] { "Name", "DeviceID", "Capacity", "FreeSpace" }).ToList());
            windows.Add("Win32_Process", (new[] { "Name", "ThreadCount", "CommandLine" }).ToList());
            windows.Add("Win32_Product", (new[] { "Name", "Description",
                "Vendor", "Version", "IdentifyingNumber", "InstallDate", "InstallLocation", "InstallSource" }).ToList());
            settings.Add(OperatingSystems.Windows.ToString(), windows);

            var aptitude = new Dictionary<string, List<string>>();
            aptitude.Add("dpkg -l", (new[] { "Name", "Version", "Architecture", "Description" }).ToList());
            settings.Add(OperatingSystems.Aptitude.ToString(), aptitude);

            var cygwin = new Dictionary<string, List<string>>();
            cygwin.Add("cygcheck --check-setup --dump-only *", (new[] { "Name" }).ToList());
            settings.Add(OperatingSystems.Cygwin.ToString(), cygwin);

            var pacman = new Dictionary<string, List<string>>();
            pacman.Add("pacman -Q", (new[] { "Name" }).ToList());
            settings.Add(OperatingSystems.Pacman.ToString(), pacman);

            var pkg = new Dictionary<string, List<string>>();
            pkg.Add("pkg_info", (new[] { "Name" }).ToList());
            settings.Add(OperatingSystems.Pkg.ToString(), pkg);

            var portage = new Dictionary<string, List<string>>();
            portage.Add("equery list", (new[] { "Name" }).ToList());
            settings.Add(OperatingSystems.Portage.ToString(), portage);

            var rpm = new Dictionary<string, List<string>>();
            rpm.Add("rpm -qa", (new[] { "Name" }).ToList());
            settings.Add(OperatingSystems.Rpm.ToString(), rpm);

            var slackware = new Dictionary<string, List<string>>();
            slackware.Add("slapt-get --installed", (new[] { "Name" }).ToList());
            settings.Add(OperatingSystems.Slackware.ToString(), slackware);

            Write(settings, fileName);
        }
    }
}
