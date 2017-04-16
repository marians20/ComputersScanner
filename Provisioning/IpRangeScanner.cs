using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logger;

namespace Provisioning
{
    public class IpRangeScanner : IScanner
    {
        public string Ip { get; set; }
        public string Subnet { get; set; }

        public List<string> GetComputers()
        {
            var ip = new IpSegment(Ip, Subnet);

            Log.Debug("Number of hosts = {0}", ip.NumberOfHosts);
            Log.Debug("Network address = {0}", ip.NetworkAddress.ToIpString());
            Log.Debug("Broadcast address = {0}", ip.BroadcastAddress.ToIpString());

            return ip.Hosts().Select(host => host.ToIpString()).ToList();
        }
    }

    internal class IpSegment
    {

        private readonly uint _ip;
        private readonly uint _mask;

        public IpSegment(string ip, string mask)
        {
            _ip = ip.ParseIp();
            _mask = mask.ParseIp();
        }

        public uint NumberOfHosts => ~_mask + 1;

        public uint NetworkAddress => _ip & _mask;

        public uint BroadcastAddress => NetworkAddress + ~_mask;

        public IEnumerable<uint> Hosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                yield return host;
            }
        }

    }

    public static class IpHelpers
    {
        public static string ToIpString(this uint value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = (value & bitmask) >> ((3 - i) * 8);
                bitmask >>= 8;
                parts[i] = masked.ToString(CultureInfo.InvariantCulture);
            }
            return string.Join(".", parts);
        }

        public static uint ParseIp(this string ipAddress)
        {
            var splitted = ipAddress.Split('.');
            uint ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + uint.Parse(splitted[i]);
            }
            return ip;
        }
    }
}
