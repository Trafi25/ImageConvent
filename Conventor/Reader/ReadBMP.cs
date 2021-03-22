using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Conventor.ImageConcept;
using Conventor.Interfaces;



namespace Conventor.Reader
{
    class ReadBMP : IImageReader
    {
        byte[] fileData;
        int PixelStartAddress;
        internal byte[] ReadFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            fileData = new byte[fs.Length];
            fs.Read(fileData, 0, fileData.Length);
            fs.Close();
            return fileData;
        }

        internal void BMPGetHeader()
        {
            // initializing the array size of 14 bytes for the BMP header

            byte[] headerBMP = new byte[14];

            for (int i = 0; i < headerBMP.Length; i++)
            {
                headerBMP[i] = fileData[i];
            }

            // Getting file size from BMP heade

            byte[] fileHeaderInfo = new byte[4];

            //needed fix for the correct byte shift (data[i] = i << shiftValue)
            for (int i = 2, j = 0; j < fileHeaderInfo.Length; i++, j++)
            {
                fileHeaderInfo[j] = fileData[i];
            }


            uint fileSize;  // Ай нужно будет поменять в будующем
            fileSize = BMPGetFileSize(ref fileHeaderInfo);
            

            // Getting the address of the starting byte at which the bitmap data (array of pixels) can be found

            byte[] filePixelArrayAddress = new byte[4];

            for (int i = 10, j = 0; j < filePixelArrayAddress.Length; i++, j++)
            {
                filePixelArrayAddress[j] = fileData[i];
            }

          PixelStartAddress = BMPGetStartAddress(ref filePixelArrayAddress);



        }

        uint BMPGetFileSize(ref byte[] headerPart)
        {
            uint fileSize = (uint)fileData[5] << 32
                            | (uint)fileData[5] << 24
                            | (uint)fileData[4] << 16
                            | (uint)fileData[3] << 8
                            | (uint)fileData[2];

            return fileSize;
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

        int BMPGetStartAddress(ref byte[] headerPart)
        {
            int resultAddress = 0;

            for (int i = 0; i < headerPart.Length; i++)   resultAddress += headerPart[i];

            return resultAddress;
        }

        public Image Read(string path)
        {
            try
            {               
                ReadFile(path);
                string hex = BytesToHexString(fileData);               
                BMPGetHeader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            throw new NotImplementedException();
        }
    }
}