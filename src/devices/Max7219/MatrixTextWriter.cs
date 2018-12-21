// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading;

namespace Iot.Device.Max7219
{

    /// <summary>
    /// Matrix text writer.
    /// </summary>
    public class MatrixTextWriter
    {
        readonly Max7219 _device;

        public MatrixTextWriter(Max7219 device, Font font)
        {
            _device = device;
            Font = font;
        }

        public Font Font { get; set; }

        /// <summary>
        /// Writes a char to the given device with the specified font.
        /// </summary>
        public void WriteLetter(int deviceId, char chr, bool flush = true)
        {
            var bitmap = Font.GetItem(chr);
            var end = Math.Min(bitmap.Length, Max7219.NumDigits);
            for (int col = 0; col < end; col++)
            {
                _device[deviceId, col] = bitmap[col];
            }
            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        ///  Scrolls the underlying buffer (for all cascaded devices) up one pixel
        /// </summary>
        public void ScrollUp(bool flush)
        {
            for (var i = 0; i < _device.Length; i++)
                _device[i] = (byte)(_device[i] >> 1);
            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Scrolls the underlying buffer (for all cascaded devices) down one pixel
        /// </summary>
        public void ScrollDown(bool flush)
        {
            for (var i = 0; i < _device.Length; i++)
            {
                _device[i] = (byte)((_device[i] << 1) & 0xff);
            }
            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Scrolls the underlying buffer (for all cascaded devices) to the left
        /// </summary>
        public void ScrollLeft(byte value, bool flush)
        {
            for (var i = 1; i < _device.Length; i++)
            {
                _device[i - 1] = _device[i];
            }
            _device[_device.Length - 1] = value;
            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Scrolls the underlying buffer (for all cascaded devices) to the right
        /// </summary>
        public void ScrollRight(byte value, bool flush)
        {
            for (var i = _device.Length - 1; i > 0; i--)
            {
                _device[i] = _device[i - 1];
            }
            _device[0] = value;
            if (flush)
            {
                _device.Flush();
            }
        }

        /// <summary>
        /// Shows a message on the device. 
        /// If it's longer then the total width (or <see paramref="alwaysScroll"/> == true), 
        /// it transitions the text message across the devices from right-to-left.
        /// </summary>
        public void ShowMessage(string text, float delay = 0.05f, bool alwaysScroll = false)
        {
            var displayLength = Max7219.NumDigits * _device.CascadedDevices;
            var src = text.Select(Font.GetItem);
            var srcLength = src.Sum(x => x.Length) + text.Length - 1;

            bool scroll = alwaysScroll || srcLength > displayLength;
            if (scroll)
            {
                var d = TimeSpan.FromSeconds(delay);
                var pos = _device.Length - 1;
                _device.ClearAll(false);
                foreach (var arr in src)
                {
                    foreach (var b in arr)
                    {
                        ScrollLeft(b, true);
                        Thread.Sleep(d);

                    }
                    ScrollLeft(0, true);
                    Thread.Sleep(d);

                }
                for (; pos > 0; pos--)
                {
                    ScrollLeft(0, true);
                    Thread.Sleep(d);
                }
            }
            else
            {
                //calculate margin to display text centered
                var margin = (displayLength - srcLength) / 2;
                _device.ClearAll(false);
                var pos = margin;
                foreach (var arr in src)
                {
                    foreach (var b in arr)
                    {
                        _device[pos++] = b;
                    }
                    pos++;
                }
                _device.Flush();
            }
        }
    }
}