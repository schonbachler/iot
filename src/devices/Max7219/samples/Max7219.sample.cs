// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Device.Spi;
using System.Device.Spi.Drivers;
using System.Threading;

namespace Iot.Device.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Max7219!");

            var connectionSettings = new SpiConnectionSettings(0, 0)
            {
                ClockFrequency = 1_000_000,
                Mode = SpiMode.Mode0
            };
            var spi = new UnixSpiDevice(connectionSettings);
            var device = new Max7219(spi, cascaded: 4);
            using (device)
            {
                Console.WriteLine("Display-Test");
                device.SetRegister(Max7219.Register.DISPLAYTEST, 1);
                Thread.Sleep(1000);
                Console.WriteLine("Init");
                device.Init();
                foreach (Max7219.RotationType rotation in Enum.GetValues(typeof(Max7219.RotationType)))
                {
                    Thread.Sleep(500);
                    device.Rotation = rotation;
                    Console.WriteLine($"Rotation {device.Rotation}");
                    for (var loop = 0; loop < 2; loop++)
                    {
                        for (var value = 1; value < 0x100; value <<= 1)
                        {
                            for (var i = 0; i < device.Cascaded; i++)
                            {
                                for (var digit = 0; digit < 8; digit++)
                                {
                                    device[i, digit] = (byte)value;
                                }
                            }
                            device.Flush();
                            Thread.Sleep(100);
                        }
                        device.ClearAll();
                    }
                }
            }
        }
    }
}