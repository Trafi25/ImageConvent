using Conventor.ImageConcept;
using Conventor.Interfaces;
using Conventor.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Conventor.Reader
{
    class ReadJPEG : IImageReader
    {
        private byte[] fileData;
        private List<int> Hi=new List<int>();
        private List<int> Vi = new List<int>();
        private List<int> QuantizationId = new List<int>();
        private List<int> ChannelDCcoef = new List<int>();
        private List<int> ChannelACcoef = new List<int>();
        private List<HuffmanTree> DC = new List<HuffmanTree>() { new HuffmanTree(),new HuffmanTree() };
        private List<HuffmanTree> AC = new List<HuffmanTree>() { new HuffmanTree(), new HuffmanTree() };
        private List<int[,]> QuantMatrix = new List<int[,]>() { new int[8,8], new int[8,8] };
        private List<int[,]> Luminance = new List<int[,]>();
        private List<int[,]> Chrominance = new List<int[,]>();
        private int Height;
        private int Width;

        private void ReadFile(string path)
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
            ZigZagQuantization(QuantizationMatrix, LengthOfValues, MatrixId);
            hex = hex.Substring(size*2);
            return hex;
        }

        private void ZigZagQuantization(string QuantizationMatrix, int LengthOfValues, int MatrixId)
        {
            int[,] Matrix = new int[8,8];
            int TempI = 0;
            int TempJ = 0;
            LengthOfValues *= 2;
            string temp = QuantizationMatrix.Substring(0, LengthOfValues);
            QuantizationMatrix = QuantizationMatrix.Substring(LengthOfValues);
            Matrix[TempI, TempJ] = int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
            while (QuantizationMatrix.Length>0)
            {                
                temp = QuantizationMatrix.Substring(0,LengthOfValues);
                QuantizationMatrix = QuantizationMatrix.Substring(LengthOfValues);

                if(TempJ!=7)
                {
                    Matrix[TempI, ++TempJ] = int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    Matrix[++TempI, TempJ] = int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
                }
               
                while(TempJ>0 && TempI<7 && TempJ<8)
                {
                    temp = QuantizationMatrix.Substring(0, LengthOfValues);
                    QuantizationMatrix = QuantizationMatrix.Substring(LengthOfValues);
                    Matrix[++TempI,--TempJ]= int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
                }
                temp = QuantizationMatrix.Substring(0, LengthOfValues);
                QuantizationMatrix = QuantizationMatrix.Substring(LengthOfValues);

                if (TempI == 7)
                {
                    Matrix[TempI, ++TempJ] = int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    Matrix[++TempI, TempJ] = int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
                }

                while (TempI > 0 && TempI < 8 && TempJ < 7)
                {
                    temp = QuantizationMatrix.Substring(0, LengthOfValues);
                    QuantizationMatrix = QuantizationMatrix.Substring(LengthOfValues);
                    Matrix[--TempI, ++TempJ] = int.Parse(temp, System.Globalization.NumberStyles.HexNumber);
                }
            }
            QuantMatrix[MatrixId] = Matrix;
        }

        public string BaselineDCT(string hex)
        {
            Console.WriteLine();
            string sizestr = hex.Substring(0, 4);
            int size = int.Parse(sizestr, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Size of Quantiztion:{size}");
            int Precision = int.Parse(hex.Substring(4,2));
            Console.WriteLine($"Precision:{Precision}");
            Height = int.Parse(hex.Substring(6,4),System.Globalization.NumberStyles.HexNumber);
            Width = int.Parse(hex.Substring(10, 4), System.Globalization.NumberStyles.HexNumber);
            int NumberOfChannels = int.Parse(hex.Substring(14, 2), System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"heigth:{Height}");
            Console.WriteLine($"width:{Width}");
            Console.WriteLine($"NumberOfChannels:{NumberOfChannels}");            
            for(int i=0;i<NumberOfChannels;i++)
            {
                string ChannelsInfo = hex.Substring(16+6*i,6);
                Hi.Add(int.Parse(ChannelsInfo[2].ToString()));
                Vi.Add(int.Parse(ChannelsInfo[3].ToString()));
                QuantizationId.Add(int.Parse(ChannelsInfo.Substring(4),System.Globalization.NumberStyles.HexNumber));
            }            
            hex = hex.Substring(size * 2);
            return hex;
        }

        private string DHTSection(string hex)
        {
            Console.WriteLine();
            string sizestr = hex.Substring(0, 4);
            int size = int.Parse(sizestr, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Size of DHTSection:{size}");
            int HaffmanClass = int.Parse(hex[4].ToString());
            int TableId = int.Parse(hex[5].ToString());
            string DHTSection = hex.Substring(6, (size - 3) * 2);
            string NumberOfCodes = DHTSection.Substring(0, 32);
            string MeaningOfCodes = DHTSection.Substring(32);
            Console.WriteLine($"HoffmanClass:{HaffmanClass}");
            Console.WriteLine($"TableId:{TableId}");
            Console.WriteLine($"NumberOfCodes:{NumberOfCodes}");
            Console.WriteLine($"MeaningOfCodes:{MeaningOfCodes}");
            HexToHuffmanTree(HaffmanClass,TableId,NumberOfCodes,MeaningOfCodes);
            hex = hex.Substring(size * 2);
            return hex;
        }

        private void HexToHuffmanTree(int HaffmanClass,int TableId,string NumberOfCodes,string MeaningOfCodes)
        {
            if(HaffmanClass==0)
            {
                for(int i=0;i<16;i++)
                {
                    string NumberOfCode = NumberOfCodes.Substring(0+2*i,2);
                    int counter = int.Parse(NumberOfCode, System.Globalization.NumberStyles.HexNumber);
                    while (counter > 0)
                    {
                        string CodeValue = MeaningOfCodes.Substring(0, 2);
                        MeaningOfCodes = MeaningOfCodes.Substring(2);
                        DC[TableId].PushFromHex(CodeValue,i+1);
                        DC[TableId].ResetCurrentNode();
                        counter--;
                    }
                    if(MeaningOfCodes.Length==0)
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    string NumberOfCode = NumberOfCodes.Substring(0 + 2 * i, 2);
                    int counter = int.Parse(NumberOfCode, System.Globalization.NumberStyles.HexNumber);
                    while (counter > 0)
                    {
                        string CodeValue = MeaningOfCodes.Substring(0, 2);
                        MeaningOfCodes = MeaningOfCodes.Substring(2);
                        AC[TableId].PushFromHex(CodeValue, i + 1);
                        AC[TableId].ResetCurrentNode();
                        counter--;
                    }
                    if (MeaningOfCodes.Length == 0)
                    {
                        break;
                    }
                }
            }
        }

        private string StartOfScan(string hex)
        {
            Console.WriteLine();
            string sizestr = hex.Substring(0, 4);
            int size = int.Parse(sizestr, System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine($"Size of StartOfScan:{size}");
            int NumberOfChannels = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);            
            for(int i=0; i< NumberOfChannels; i++)
            {
                string ChannelsInfo = hex.Substring(6 + 4 * i, 4);
                ChannelDCcoef.Add(int.Parse(ChannelsInfo[2].ToString()));
                ChannelACcoef.Add(int.Parse(ChannelsInfo[3].ToString()));
            }            
            hex = hex.Substring(size * 2);
            string EncodedData = hex.Substring(0,hex.Length-4);
            Console.WriteLine($"EncodedData:{EncodedData}");
            string binarystring = String.Join(String.Empty,
                  EncodedData.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                  )
                );
            Encoding(binarystring);
            hex = hex.Substring(hex.Length - 4);
            return hex;
        }

        private void Encoding(string EncodedData)
        {
            while (EncodedData.Length > 0)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < Hi[j] * Vi[j]; i++)
                    {
                        int[,] temp = new int[8, 8];
                        int TempI = 0;
                        int TempJ = 0;
                        DC[ChannelDCcoef[j]].FindCoef(EncodedData);
                        EncodedData = EncodedData.Substring(DC[ChannelDCcoef[j]].GetCode().Length);
                        int NodeValue = int.Parse(DC[ChannelDCcoef[j]].GetValue(), System.Globalization.NumberStyles.HexNumber);
                        if (NodeValue==0)
                        {
                            temp[0, 0] = 0;
                        }
                        else
                        {
                            string binaryDCcoef = EncodedData.Substring(0, NodeValue);
                            EncodedData= EncodedData.Substring(NodeValue);
                            if(binaryDCcoef[0]=='1')
                            {
                                temp[0, 0] = Convert.ToInt32(binaryDCcoef, 2);
                            }
                            else
                            {
                                temp[0, 0] = (int)(Convert.ToInt32(binaryDCcoef, 2) - Math.Pow(2, binaryDCcoef.Length) + 1);
                            }
                        }
                        DC[ChannelDCcoef[j]].ResetCurrentNode();
                        bool Direction = false;
                        for(int t=0;t<63;t++)
                        {                            
                            AC[ChannelACcoef[j]].FindCoef(EncodedData);
                            string Node = AC[ChannelACcoef[j]].GetValue();
                            if (int.Parse(Node, System.Globalization.NumberStyles.HexNumber)==0)
                            {
                                EncodedData = EncodedData.Substring(AC[ChannelACcoef[j]].GetCode().Length);
                                AC[ChannelACcoef[j]].ResetCurrentNode();
                                break;
                            }

                            int NumberOfZeros=int.Parse(Node[0].ToString(),System.Globalization.NumberStyles.HexNumber);
                            int CoefSize= int.Parse(Node[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                            while(NumberOfZeros>0)
                            {
                                if(TempI==0 && TempJ!=7 && !Direction)
                                {
                                    ++TempJ;
                                    Direction = true;
                                }
                                else if(TempJ==7 && !Direction)
                                {
                                    ++TempI;
                                    Direction = true;
                                }
                                else if(TempJ==0 && TempI!=7 && Direction)
                                {
                                    ++TempI;
                                    Direction = false;
                                }
                                else if(TempI==7 && Direction)
                                {
                                    ++TempJ;
                                    Direction = false;
                                }
                                else if (Direction)
                                {
                                    ++TempI;
                                    --TempJ;
                                }
                                else if(!Direction)
                                {
                                    --TempI;
                                    ++TempJ;
                                }
                                t++;
                                NumberOfZeros--;
                            }
                            EncodedData = EncodedData.Substring(AC[ChannelACcoef[j]].GetCode().Length);
                            AC[ChannelACcoef[j]].ResetCurrentNode();
                            Node = EncodedData.Substring(0,CoefSize);
                            EncodedData = EncodedData.Substring(CoefSize);
                            int ACcoef=0;
                            if(Node[0]=='1')
                            {
                                ACcoef= Convert.ToInt32(Node, 2);
                            }
                            else if(Node[0]=='0')
                            {
                                ACcoef = (int)(Convert.ToInt32(Node, 2) - Math.Pow(2, Node.Length) + 1);
                            }
                            if (TempI == 0 && TempJ != 7 && !Direction)
                            {
                                temp[TempI,++TempJ]=ACcoef;
                                Direction = true;
                            }
                            else if (TempJ == 7 && !Direction)
                            {
                                temp[++TempI,TempJ]=ACcoef;
                                Direction = true;
                            }
                            else if (TempJ == 0 && TempI != 7 && Direction)
                            {
                                temp[++TempI, TempJ] = ACcoef;
                                Direction = false;
                            }
                            else if (TempI == 7 && Direction)
                            {
                                temp[TempI, ++TempJ] = ACcoef;
                                Direction = false;
                            }
                            else if (Direction)
                            {
                                temp[++TempI, --TempJ] = ACcoef;                                
                            }
                            else if (!Direction)
                            {
                                temp[--TempI, ++TempJ] = ACcoef;                                
                            }
                        }
                        if(j==0)
                        {
                            if(i>0)
                            {
                                temp[0, 0] = temp[0, 0] + Luminance[Luminance.Count - 1][0, 0];
                            }
                            Luminance.Add(temp);
                        }
                        else
                        {
                            if (i > 0)
                            {
                                temp[0, 0] = temp[0, 0] + Chrominance[Chrominance.Count - 1][0, 0];
                            }
                            Chrominance.Add(temp);
                        }
                    }
                }
            }            
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
