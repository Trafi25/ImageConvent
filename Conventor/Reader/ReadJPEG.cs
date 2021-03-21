using Conventor.ImageConcept;
using Conventor.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Conventor.Reader
{
    class ReadJPEG : IImageReader
    {
        private byte[] fileData;
        internal void ReadFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            fileData = new byte[fs.Length];
            fs.Read(fileData, 0, fileData.Length);
            fs.Close();
        }

        private string BytesToHexString(byte[] buffer)
        {
            var hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();            
        }

        private byte[] HexString2Bytes(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }
            return bytes;
        }

        public void Analyze(string hex)
        {
            string temp=string.Empty;
            while(hex.Length>0)
            {
                temp = hex.Substring(0,4);
                hex = hex.Substring(4);               
               switch(temp)
               {
                   case "ffd8":
                       temp = string.Empty;
                       break;
                   case "fffe":
                        {                                
                            hex = CommentSection(hex);
                            temp = string.Empty;
                        }
                       break;
                    case "ffe0":
                        {
                            hex = CommentSection(hex);
                            temp = string.Empty;
                        }
                        break;
                   case "ffdb":
                        {
                            hex = QuantizationMatrix(hex);
                            temp = string.Empty;
                        }
                        break;
                    case "ffc0":
                        {
                            hex = BaselineDCT(hex);
                            temp = string.Empty;
                        }
                        break;
                    case "ffc4":
                        {
                            hex = DHTSection(hex);
                            temp = string.Empty;
                        }
                        break;
                    case "ffda":
                        {
                            hex = StartOfScan(hex);
                            temp = string.Empty;
                        }
                        break;
                    case "ffd9":
                        return;
               }                
            }
        }

        private string CommentSection(string hex)
        {
            string temp=hex.Substring(0,4);            
            hex = hex.Substring(int.Parse(temp,System.Globalization.NumberStyles.HexNumber)*2);
            return hex;
        }

        private string QuantizationMatrix(string hex)
        {
            Console.WriteLine();
            string sizestr = hex.Substring(0, 4);
            int size = int.Parse(sizestr, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Size of Quantiztion:{size}");
            string QuantizationMatrix = hex.Substring(6,(size-3)*2);
            int LengthOfValues = (int)Math.Pow(2,int.Parse(hex[4].ToString()));
            int MatrixId = int.Parse(hex[5].ToString());
            Console.WriteLine($"LengthOfValues:{LengthOfValues}");
            Console.WriteLine($"MatrixId:{MatrixId}");
            Console.WriteLine($"QuantizationMatrix:{QuantizationMatrix}");
            hex = hex.Substring(size*2);
            return hex;
        }

        public string BaselineDCT(string hex)
        {
            Console.WriteLine();
            string sizestr = hex.Substring(0, 4);
            int size = int.Parse(sizestr, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Size of Quantiztion:{size}");
            int Precision = int.Parse(hex.Substring(4,2));
            Console.WriteLine($"Precision:{Precision}");
            int heigth = int.Parse(hex.Substring(6,4),System.Globalization.NumberStyles.HexNumber);
            int width = int.Parse(hex.Substring(10, 4), System.Globalization.NumberStyles.HexNumber);
            int NumberOfChannels = int.Parse(hex.Substring(14, 2), System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"heigth:{heigth}");
            Console.WriteLine($"width:{width}");
            Console.WriteLine($"NumberOfChannels:{NumberOfChannels}");
            //add here foreach NumberOfChannels
            hex = hex.Substring(size * 2);
            return hex;
        }

        private string DHTSection(string hex)
        {
            Console.WriteLine();
            string sizestr = hex.Substring(0, 4);
            int size = int.Parse(sizestr, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Size of DHTSection:{size}");
            int HoffmanClass = int.Parse(hex[4].ToString());
            int TableId = int.Parse(hex[5].ToString());
            string DHTSection = hex.Substring(6, (size - 3) * 2);
            Console.WriteLine($"HoffmanClass:{HoffmanClass}");
            Console.WriteLine($"TableId:{TableId}");
            Console.WriteLine($"DHTSection:{DHTSection}");
            hex = hex.Substring(size * 2);
            return hex;
        }

        private string StartOfScan(string hex)
        {
            Console.WriteLine();
            string sizestr = hex.Substring(0, 4);
            int size = int.Parse(sizestr, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Size of StartOfScan:{size}");
            int NumberOfChannels = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //add here foreach NumberOfChannels
            hex = hex.Substring(size * 2);
            string EncodedData = hex.Substring(0,hex.Length-4);
            Console.WriteLine($"EncodedData:{EncodedData}");
            hex = hex.Substring(hex.Length - 4);
            return hex;
        }

        public Image Read(string path)
        {
            try
            {
                ReadFile(path);
                string hex = BytesToHexString(fileData);
                Analyze(hex);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            throw new NotImplementedException();
        }
        
    }
}
