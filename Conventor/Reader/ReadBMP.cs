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

            byte[] fileSizeHeaderInfo = new byte[4];

            //needed fix for the correct byte shift (data[i] = i << shiftValue)
            for (int i = 2, j = 0; j < fileSizeHeaderInfo.Length; i++, j++)
            {
                fileSizeHeaderInfo[j] = fileData[i];
            }


            uint fileSize;  // Ай нужно будет поменять в будующем
            fileSize = BMPGetFileSize(ref fileSizeHeaderInfo);         
                    }

        uint BMPGetFileSize(ref byte[] headerPart)
        {
            uint fileSize = (uint)fileData[5] << 24
                            | (uint)fileData[4] << 16
                            | (uint)fileData[3] << 8
                            | (uint)fileData[2];

            return fileSize;
        }



        public Image Read(string path)
        {
            try
            {               
                ReadFile(path);
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