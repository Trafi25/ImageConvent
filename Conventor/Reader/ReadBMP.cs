using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Conventor.ImageConcept;
using Conventor.Interfaces;
using Conventor.Writer;

namespace Conventor.Reader
{
    class ReadBMP : IImageReader
    {

        private byte[] fileData;
        private int PixelStartAddress;
        private byte[] tablePixel;
        private int height = 0;
        private int width = 0;
        private uint fileSize = 0;
        private int reservedFirst = 0;
        private int reservedSecond = 0;
        private ImageHeader imgHeadBMP;

        private ImageHeader ReadFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            fileData = new byte[fs.Length];
            fs.Read(fileData, 0, fileData.Length);


            fs.Close();
            imgHeadBMP = new ImageHeader();
            BinaryReader br = new BinaryReader(File.OpenRead(path));
            br.BaseStream.Position = 18;

            byte[] widthBbytes = new byte[sizeof(int)];
            for (int i = 0; i < widthBbytes.Length; i++)
            {
                widthBbytes[i] = br.ReadByte();
            }
            width = BitConverter.ToInt32(widthBbytes, 0);
            imgHeadBMP.Width = width;

            br.BaseStream.Position = 22;

            byte[] heightBytes = new byte[sizeof(int)];
            for (int i = 0; i < widthBbytes.Length; i++)
            {
                heightBytes[i] = br.ReadByte();
            }
            height = BitConverter.ToInt32(heightBytes, 0);
            imgHeadBMP.Height = height;





            return imgHeadBMP;
        }

        private byte[] BMPReadPixel()
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


            fileSize = BMPGetFileSize(ref fileHeaderInfo);

            byte[] fileReservedInfo = new byte[4];

            for (int i = 6, j = 0; j < fileReservedInfo.Length; i++, j++)
            {
                fileReservedInfo[j] = fileData[i];
            }

            int[] reservedResult = BMPGetReservedHeader(ref fileReservedInfo);
            reservedFirst = reservedResult[0];
            reservedSecond = reservedResult[1];



            // Getting the address of the starting byte at which the bitmap data (array of pixels) can be found

            byte[] filePixelArrayAddress = new byte[4];

            for (int i = 10, j = 0; j < filePixelArrayAddress.Length; i++, j++)
            {
                filePixelArrayAddress[j] = fileData[i];
            }

            PixelStartAddress = BMPGetStartAddress(ref filePixelArrayAddress);


            tablePixel = new byte[fileData.Length - PixelStartAddress];
            byte[] bufferTable = new byte[3];
           

            int tempNumber = 0;
            for(int i=height-1;i>=0;i--)
            {
                for(int j=0;j<width;j++)
                {

                    for(int k=0;k<3;k++)
                    {
                        bufferTable[k] = fileData[tempNumber];
                        tempNumber++;
                    }
                    for (int k = 2; k >= 0; k--)
                    {
                        tablePixel[i * width*3 + j * 3 + 2 - k] = bufferTable[k];
                    }
                }
            }

            return tablePixel;

        }

        private uint BMPGetFileSize(ref byte[] headerPart)
        {
            uint fileSize = (uint)fileData[6] << 32
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

        private int BMPGetStartAddress(ref byte[] headerPart)
        {
            int resultAddress = 0;

            for (int i = 0; i < headerPart.Length; i++) resultAddress += headerPart[i];

            return resultAddress;
        }

        private int[] BMPGetReservedHeader(ref byte[] headerPart)
        {
            int reservedFirst = headerPart[0] + headerPart[1];
            int reservedSecond = headerPart[2] + headerPart[3];

            int[] resultReserved = new int[2];
            resultReserved[0] = reservedFirst;
            resultReserved[1] = reservedSecond;

            return resultReserved;
        }

        public Image Read(string path)
        {
            Image img = new Image(path);

            try
            {
                img.Header = ReadFile(path);
                string hex = BytesToHexString(fileData);
                img.RGBPixel = BMPReadPixel();
                //maybe need big refactor
                Ppm_writer wr = new Ppm_writer();                
                wr.Write("C:/Users/user/Desktop/Комп графика/ImageConvent/Image/1.ppm", img);
                //-------------------
                return img;

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return img;
        }
    }
}