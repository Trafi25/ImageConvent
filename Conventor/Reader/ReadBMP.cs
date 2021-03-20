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
        }

        public Image Read(string path)
        {
            try
            {
                ReadBMP objDataInfo = new ReadBMP();
                objDataInfo.ReadFile(path);
                objDataInfo.BMPGetHeader();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            throw new NotImplementedException();
        }
    }
}