using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BinSplitter
{
    class Program
    {
        static Stream bin;
        [STAThread]
        static void Main(string[] args)
        {
            var x = new System.Windows.Forms.OpenFileDialog();
            x.ShowDialog();
            while (x.FileNames.Length == 0) { }
            bin = x.OpenFile();
            byte[] buf = new byte[1024 * 25];
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin1.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin2.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin3.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin4.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin5.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin6.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin7.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin8.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin9.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin10.bin", buf);
            bin.Read(buf, 0, buf.Length);
            File.WriteAllBytes(@"c:\ArduinoBin\bin11.bin", buf);
        }
    }
}
