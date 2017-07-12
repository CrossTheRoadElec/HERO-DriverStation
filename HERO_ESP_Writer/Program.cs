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
using System.Threading;
using Microsoft.SPOT;
using CTRE;
using CTRE.HERO;
using Microsoft.SPOT.Hardware;

namespace HERO_ESP_Writer
{
    public class Program
    {
        //baud rate of 460800 is too fast
        static System.IO.Ports.SerialPort uart = new System.IO.Ports.SerialPort(IO.Port1.UART, 256000, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        static OutputPort FLASH = new OutputPort(IO.Port1.Pin3, false);
        static OutputPort RESET = new OutputPort(IO.Port1.Pin6, false);

        static byte[] fullBin;
        static byte[] bin;
        static int segment = 0;
        static byte[] returnData;

        public static void Main()
        {
            uart.Open();
            Thread.Sleep(1000);
            RESET.Write(true);
            Thread.Sleep(500);
            FLASH.Write(true);
            bool goodFlash = true; ;

            Debug.Print("Module in bootloader mode");

            //Sync esp baud rate to desired baud rate
            while (true)
            {
                //Debug.Print("Syncing");
                sync();
                returnData = syncRead();
                int syncNumber = 0;

                if (returnData != null)
                {
                    foreach (byte b in returnData)
                    {
                        if (b == 0xC0) syncNumber++;
                    }
                }
                if (syncNumber == 16) break;
            }

            Debug.Print("Sync complete, erasing");
            //Erase the flash on the esp
            Thread.Sleep(1000);
            returnData = null;
            erase();
            while (returnData == null)
                returnData = read();

            Debug.Print("Erase successful, writing first bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin1);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if(returnData.Length > 9)
                {
                    if(returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 1 successful, writing second bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin2);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 2 successful, writing third bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin3);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 3 successful, writing fourth bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin4);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 4 successful, writing fifth bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin5);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 5 successful, writing sixth bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin6);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 6 successful, writing seventh bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin7);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 7 successful, writing eighth bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin8);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 8 successful, writing ninth bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin9);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 9 successful, writing tenth bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin10);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }

            Debug.Print("Bin 10 successful, writing eleventh bin");
            //Assign first bin file to byte array
            fullBin = Properties.Resources.GetBytes(Properties.Resources.BinaryResources.bin11);
            for (int i = 0; i < 25; i++)
            {
                bin = new byte[1024];
                //Take first bin file and start assigning a kilobyte to write buffer
                Array.Copy(fullBin, i * 1024, bin, 0, 1024);
                write();
                returnData = read();
                if (returnData.Length > 9)
                {
                    if (returnData[9] == 0x01 && returnData[10] == 0x05)
                    {
                        //Bad flash, do something here
                        goodFlash = false;
                    }
                }
                Debug.GC(true);
                //Debug.Print("Written segment " + segment);
            }


            //End of data writing, execute final command
            end();
            //Congrats, you've got a good module
            Debug.Print(goodFlash ? "Successful flash" : "Bad Flash, try again");
        }


        static void end()
        {
            byte[] end = new byte[14];
            end[0] = 0xC0;
            end[1] = 0x00;
            end[2] = 0x04;
            end[3] = 0x04;
            end[4] = 0x00;
            end[5] = 0x7F;
            end[6] = end[7] = end[8] = 0x00;
            end[9] = 0x01;
            end[10] = end[11] = end[12] = 0x00;
            end[13] = 0xC0;
            uart.BaseStream.Write(end, 0, 14);
        }

        static bool write()
        {
            while (uart.BytesToRead > 0) { uart.ReadByte(); }
            while (uart.BytesToWrite > 0) { }


            byte[] segmentA = BitConverter.GetBytes(segment);
            byte[] write = new byte[24];
            write[0] = 0x00;
            write[1] = 0x03;
            write[2] = 0x10;
            write[3] = 0x04;
            write[8] = 0x00;
            write[9] = 0x04;
            for (int i = 10; i <= 23; i++)
                write[i] = 0x00;
            write[12] = segmentA[0];
            write[13] = segmentA[1];
            write[14] = segmentA[2];
            write[15] = segmentA[3];
            
            Thread.Sleep(5);

            byte[] chksm = BitConverter.GetBytes(chksum(bin));

            write[4] = chksm[0];
            write[5] = chksm[1];
            write[6] = chksm[2];
            write[7] = chksm[3];

            byte[] total = new byte[write.Length + bin.Length];
            Array.Copy(write, 0, total, 0, write.Length);
            Array.Copy(bin, 0, total, write.Length, bin.Length);
                
            byte[] outTotal = convert(total);

            uart.Write(new byte[] { 0xC0 }, 0, 1);
            Thread.Sleep(1);
            uart.Write(outTotal, 0, outTotal.Length);
            Thread.Sleep(1);
            uart.Write(new byte[] { 0xC0 }, 0, 1);

            segment++;

            return false;
        }

        static byte[] read()
        {
            System.Collections.ArrayList ret = new System.Collections.ArrayList();
            long t = DateTime.Now.Ticks;
            while (uart.BytesToRead == 0 && DateTime.Now.Ticks - t < TimeSpan.TicksPerSecond * 5) { }
            if (uart.BytesToRead == 0) return null;

            t = DateTime.Now.Ticks;
            byte end = 0x00;
            while (ret.Count < 5 || end != 0xC0)
            {
                while (uart.BytesToRead > 0)
                {
                    end = (byte)uart.ReadByte();
                    ret.Add(end);
                }
                if (DateTime.Now.Ticks - t > TimeSpan.TicksPerMillisecond * 20) break;
            }
            return (byte[])ret.ToArray(typeof(byte));
        }

        static byte[] syncRead()
        {
            System.Collections.ArrayList ret = new System.Collections.ArrayList();
            long t = DateTime.Now.Ticks;
            while (uart.BytesToRead == 0 && DateTime.Now.Ticks - t < TimeSpan.TicksPerSecond) { }
            if (uart.BytesToRead == 0) return null;
            t = DateTime.Now.Ticks;
            while (uart.BytesToRead > 0)
            {
                ret.Add((byte)uart.ReadByte());
                if (uart.BytesToRead == 0) while (DateTime.Now.Ticks - t < TimeSpan.TicksPerMillisecond / 10) { }
            }
            return (byte[])ret.ToArray(typeof(byte));
        }

        static void erase()
        {

            byte[] erase = new byte[26];
            erase[0] = 0xC0;//Header
            erase[1] = 0x00;//Request
            erase[2] = 0x02;//Command
            erase[3] = 0x10;//Size of packet
            erase[4] = 0x00;
            erase[5] = 0xA8;
            erase[6] = 0x00;
            erase[7] = 0x00;
            erase[8] = 0x00;
            erase[9] = 0x00;//Number of sectors to erase
            erase[10] = 0x00;
            erase[11] = 0x04;
            erase[12] = 0x00;
            erase[13] = 0x11;//Number of transmitting packets?
            erase[14] = 0x01;
            erase[15] = 0x00;
            erase[16] = 0x00;
            erase[17] = 0x00;//Packet size
            erase[18] = 0x04;
            erase[19] = 0x00;
            erase[20] = 0x00;
            erase[21] = 0x00;//Address
            erase[22] = 0x00;
            erase[23] = 0x00;
            erase[24] = 0x00;
            erase[25] = 0xC0;//Footer


            foreach (byte b in erase)
            {
                uart.Write(new byte[] { b }, 0, 1);
            }
        }

        static void sync()
        {
            byte[] sync = new byte[46];
            sync[0] = sync[45] = 0xC0;//Header/Footer
            sync[1] = 0x00;//Requesting
            sync[2] = 0x08;//Command
            sync[3] = 0x24;//Size of packet
            sync[4] = 0x00;
            sync[5] = 0xDD;//Chksum
            sync[6] = sync[7] = sync[8] = 0x00;
            sync[9] = sync[10] = 0x07;//Sync frame
            sync[11] = 0x12;
            sync[12] = 0x20;
            for (int i = 13; i < 45; i++)
                sync[i] = 0x55;


            uart.Write(sync, 0, sync.Length);
        }

        static byte[] convert(byte[] data)
        {
            System.Collections.ArrayList temp = new System.Collections.ArrayList();
            temp.Capacity = 1100;
            foreach (byte b in data)
            {
                if (b == 0xC0)
                {
                    temp.Add((byte)0xDB);
                    temp.Add((byte)0xDC);
                }
                else if (b == 0xDB)
                {
                    temp.Add((byte)0xDB);
                    temp.Add((byte)0xDD);
                }
                else
                {
                    temp.Add(b);
                }
            }
            return (byte[])temp.ToArray(typeof(byte));
        }

        static UInt32 chksum(byte[] data)
        {
            UInt32 ret = 0xEF;
            foreach (byte b in data)
            {
                ret = ret ^ b;
            }
            return ret;
        }
    }
}
