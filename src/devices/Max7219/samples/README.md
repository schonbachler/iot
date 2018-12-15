# Driver for 8x8 Dot Matrix Module

You can use .NET Core to drive MAX7219 based DOt Matrix Modules.

These Modules can be 


## Accessing the MAX7219 via SPI

The Raspberry Pi has support for SPI. You need to [enable the SPI interface on the Raspberry Pi](https://www.raspberrypi-spy.co.uk/2014/08/enabling-the-spi-interface-on-the-raspberry-pi/) since it is not enabled by default.


```csharp
var connection = new SpiConnectionSettings(0,0);
connection.ClockFrequency = 1000000;
connection.Mode = SpiMode.Mode0;
var spi = new UnixSpiDevice(connection);
var max = new Max7219(spi);
```

The following pin layout can be used (also shown in a [fritzing diagram](rpi-trimpot-spi.fzz)):

* MAX7219 VCC to RPi 5V
* MAX7219 GND to RPi GND
* MAX7219 DIN to RPi GPIO 10 (MOSI)
* MAX7219 CS to RPi GPIO 8 (SPI CSO)
* MAX7219 CLK to RPi GPIO11 (SPI CLK)

## Writing to the Matrix

Independent of the way in which you access the MCP3008 chip, the code to process its results is the same, which follows.

```csharp
while (true)
{
    double value = mcp.Read(0);
    value = value / 10.24;
    value = Math.Round(value);
    Console.WriteLine(value);
    Thread.Sleep(500);
}
```

The chip is 10-bit, which means that it will generate values from 0-1023 (recall that 2^10 is 1024). We can transform the value to a more familiar 0-100 scale by dividing the 10-bit value by 10.24.

## Hardware elements

The following elements are used in this sample:

## References


See [Using .NET Core for IoT Scenarios](../README.md) for more samples.
