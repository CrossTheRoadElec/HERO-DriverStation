/*
 *  Software License Agreement
 *
 * Copyright (C) Cross The Road Electronics.  All rights
 * reserved.
 * 
 * Cross The Road Electronics (CTRE) licenses to you the right to 
 * use, publish, and distribute copies of CRF (Cross The Road) binary firmware files (*.crf) 
 * and software example source ONLY when in use with Cross The Road Electronics hardware products.
 * 
 * THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT
 * WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT
 * LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS FOR A
 * PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT SHALL
 * CROSS THE ROAD ELECTRONICS BE LIABLE FOR ANY INCIDENTAL, SPECIAL, 
 * INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF
 * PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR SERVICES, ANY CLAIMS
 * BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE
 * THEREOF), ANY CLAIMS FOR INDEMNITY OR CONTRIBUTION, OR OTHER
 * SIMILAR COSTS, WHETHER ASSERTED ON THE BASIS OF CONTRACT, TORT
 * (INCLUDING NEGLIGENCE), BREACH OF WARRANTY, OR OTHERWISE
 */

using System;
using System.Collections;
using CTRE.HERO;
using Microsoft.SPOT;

namespace CTRE.FRC
{
    public class ESPModule : IRobotStateProvider, Controller.IGameControllerValuesProvider
    {
        //public HERO.Module.WiFiESP12F _wifiModule;
        byte[] _data;//Data for decoder
        bool _connected;//Connected to wifi module or not
        bool _updateFlag;//New data
        byte[] _sendingMessage;//Byte array to module
        static byte _len= 0;
        static int _iterator = 0;
        static UInt16 _eofA = 0;
        static UInt16 _checksum = 0;
        static byte[] _tx = new byte[255];//Ring buffer
        static uint _txCnt = 0;//Ring buffer size
        static uint _txIn = 0;//Ring buffer in _iterator
        static uint _txOut = 0;//Ring buffer out _iterator
        /** Cache for reading out bytes in serial driver. */
        Stopwatch _timeout = new Stopwatch();
        Stopwatch _initialization = new Stopwatch();
        
        System.IO.Ports.SerialPort _uart;
        Microsoft.SPOT.Hardware.OutputPort _restart;
        Microsoft.SPOT.Hardware.OutputPort _flashPin;

        static Processing processing = Processing.header;
        private enum Processing
        {
            header,
            length,
            payload,
            eof1,
            eof2,
            process
        }

        public ESPModule(PortDefinition wifiPort)
        {
            if (wifiPort is IPortUart)
            {
                IPortUart p = (IPortUart)wifiPort;
                _uart = new System.IO.Ports.SerialPort(p.UART, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                _restart = new Microsoft.SPOT.Hardware.OutputPort(p.Pin6, true);
                //Need to define the flash pin to ensure it's held true
                if (wifiPort is Port1Definition) _flashPin = new Microsoft.SPOT.Hardware.OutputPort(((Port1Definition)wifiPort).Pin3, true);
                if (wifiPort is Port4Definition) _flashPin = new Microsoft.SPOT.Hardware.OutputPort(((Port4Definition)wifiPort).Pin3, true);
                if (wifiPort is Port6Definition) _flashPin = new Microsoft.SPOT.Hardware.OutputPort(((Port6Definition)wifiPort).Pin3, true);
            }
            else
            {
                throw new ArgumentException("Port is not UART compatible");
            }


            _uart.Open();
            _uart.Flush();

            
            _connected = false;
            _data = new byte[255];
            _timeout.Start();
            _sendingMessage = new byte[1] { 0x00 };
            _enableTimeout.Start();
            _initialization.Start();
        }

        public void update()
        {
            _sendingMessage = combine(new byte[1] { 0x00 }, _sendingMessage);
            _uart.Write(_sendingMessage, 0, _sendingMessage.Length);
            _sendingMessage = new byte[0];
            _updateFlag = false;
            _sendingMessage = new byte[1];

            if (_uart.BytesToRead > 0)
            {
                _updateFlag = true;

                for (int i = 0; i < _uart.BytesToRead; i++)
                {
                    PushByte((byte)_uart.ReadByte());
                }
            }


            //Process data until buffer is empty to ensure newest data
            while (_txCnt > 0)
            {
                switch (processing)
                {
                    //Header frame, make sure I've got an AA
                    case Processing.header:
                        _checksum = 0;
                        if (PopByte() == 0xAA)
                        {
                            _checksum += 0xAA;
                            processing = Processing.length;
                        }
                        break;

                    //Length frame, find out how long the payload is
                    case Processing.length:
                        _len = PopByte();
                        _checksum += _len;
                        _iterator = 0;
                        processing = Processing.payload;
                        break;

                    //Payload frame, assign payload to data array
                    case Processing.payload:
                        while (_txCnt > 0 && _iterator < _len)
                        {
                            _data[_iterator] = PopByte();
                            _checksum += _data[_iterator];
                            _iterator++;
                        }
                        if (_iterator == _len)
                        {
                            processing = Processing.eof1;
                        }
                        break;

                    //End of frame 1, record first checksum byte
                    case Processing.eof1:
                        _eofA = PopByte();
                        processing = Processing.eof2;
                        break;

                    //End of frame 2, record second checksum byte, combine two, and ensure it's 0
                    case Processing.eof2:
                        _checksum += (UInt16)((UInt16)(_eofA << 8) + (UInt16)PopByte());
                        if (_checksum == 0x00)
                        {
                            processing = Processing.process;
                            goto case Processing.process;
                        }
                        else
                        {
                            //Bad _checksum
                            processing = Processing.header;
                        }
                        break;

                    //Process frame, assign payload data to joysticks


                    //If anything else, set to header
                    default:
                        processing = Processing.header;
                        break;
                }
                _timeout.Start();
            }

            if (_timeout.DurationMs > 5000)
            {
                _restart.Write(false);//Restart module if no data is coming
                _timeout.Start();
                _connected = false;
            }
            else
            {
                _restart.Write(true);
                _connected = true;
            }
        }

        public void SendIP(byte[] moduleIP, byte[] targetIP)
        {
            _sendingMessage = combine(_sendingMessage, new byte[] { 0x33, (byte)'i', (byte)(moduleIP.Length + targetIP.Length) });
            _sendingMessage = combine(_sendingMessage, moduleIP);
            _sendingMessage = combine(_sendingMessage, targetIP);
        }

        public void SendUDP(UInt16 port, byte[] data)
        {
            if(data.Length < 200)
            {
                _sendingMessage = combine(_sendingMessage, new byte[] { 0x33, (byte)'u', (byte)(data.Length + 2) });

                byte[] b = BitConverter.GetBytes(port);
                _sendingMessage = combine(_sendingMessage, b);
                _sendingMessage = combine(_sendingMessage, data);
            }
            else
            {
                throw new Exception("Data length too long");
            }
        }

        public bool IsConnected()
        {
            return _connected;
        }

        private byte[] combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            Array.Copy(a, c, a.Length);
            Array.Copy(b, 0, c, a.Length, b.Length);
            return c;
        }

        /** @param received byte to push into ring buffer */
        private static void PushByte(byte datum)
        {
            _tx[_txIn] = datum;
            if (++_txIn >= _tx.Length)
                _txIn = 0;
            ++_txCnt;
        }
        /** 
         * Pop the oldest byte out of the ring buffer.
         * Caller must ensure there is at least one byte to pop out by checking _txCnt.
         * @return the oldest byte in buffer.
         */
        private static byte PopByte()
        {
            byte retval = _tx[_txOut];
            if (++_txOut >= _tx.Length)
                _txOut = 0;
            --_txCnt;
            return retval;
        }
    }
}
