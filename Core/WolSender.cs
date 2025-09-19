using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ShutdownScheduler.Core
{
    public static class WolSender
    {
        /// <summary>
        /// Sends a Wake-on-LAN magic packet to the given MAC address.
        /// </summary>
        /// <param name="macAddress">MAC address in format "00:11:22:33:44:55".</param>
        public static void Wake(string macAddress)
        {
            try
            {
                byte[] macBytes = macAddress.Split(':')
                    .Select(s => Convert.ToByte(s, 16))
                    .ToArray();

                // Build magic packet (6x FF + 16x MAC)
                byte[] packet = new byte[102];
                for (int i = 0; i < 6; i++) packet[i] = 0xFF;
                for (int i = 1; i <= 16; i++)
                    Buffer.BlockCopy(macBytes, 0, packet, i * 6, macBytes.Length);

                using (UdpClient client = new UdpClient())
                {
                    client.EnableBroadcast = true;
                    client.Connect(IPAddress.Broadcast, 9); // Port 9 (discard)
                    client.Send(packet, packet.Length);
                }

                Console.WriteLine($"✅ Sent WOL packet to {macAddress}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send WOL packet: {ex.Message}");
            }
        }
    }
}
