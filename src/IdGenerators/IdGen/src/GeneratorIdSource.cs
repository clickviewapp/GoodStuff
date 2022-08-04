namespace ClickView.GoodStuff.IdGenerators.IdGen;

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

internal class GeneratorIdSource
{
    /// <summary>
    /// Gets a generator id for use with IdGen
    /// </summary>
    /// <param name="bits">The number of bits the generated id should use</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static int GetId(byte bits)
    {
        if (bits <= 0)
            throw new ArgumentException("Value must be greater than 0", nameof(bits));

        if(bits > 32)
            throw new ArgumentException("Value must be less than 32", nameof(bits));

        if (TryGetIdFromPrivateIpV4(bits, out var generatorId))
            return generatorId;

        throw new Exception("Failed to get Generator Id");
    }

    private static int GetMask(byte bits) => (1 << bits) - 1;

    /// <summary>
    /// Try and get a generator id from the local network interface
    /// </summary>
    /// <param name="bits"></param>
    /// <param name="generatorId"></param>
    /// <returns></returns>
    private static bool TryGetIdFromPrivateIpV4(byte bits, out int generatorId)
    {
        var ipAddress = GetPrivateIpV4();

        if (ipAddress == null)
        {
            generatorId = 0;
            return false;
        }

        Trace.WriteLine($"Using IP Address {ipAddress} for Id Generator");

        var bytes = ipAddress.GetAddressBytes();

        // If we are on a LittleEndian system we need to swap the bits around
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        var address = BitConverter.ToInt32(bytes, 0);

        // Mask the value to get the correct length
        var mask = GetMask(bits);
        generatorId = address & mask;

        return true;
    }

    internal static IPAddress? GetPrivateIpV4()
    {
        // Get all the ethernet (or wireless) interfaces
        var interfaces = NetworkInterface.GetAllNetworkInterfaces()
            .Where(x => x.NetworkInterfaceType is NetworkInterfaceType.Ethernet
                            or NetworkInterfaceType.GigabitEthernet
                            or NetworkInterfaceType.Wireless80211
                            or NetworkInterfaceType.FastEthernetFx
                            or NetworkInterfaceType.FastEthernetT
                        && x.OperationalStatus == OperationalStatus.Up
            );

        // Get all the InterNetwork (IPv4) Unicast addresses
        var addresses = interfaces.SelectMany(x => x.GetIPProperties().UnicastAddresses)
            .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(x => x.Address);

        // Get the first
        return addresses.FirstOrDefault();
    }
}
