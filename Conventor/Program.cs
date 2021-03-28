using Conventor.Reader;
using System;
using Conventor.Exceptions;
using Conventor.ImageConcept;

namespace Conventor
{
    class Program
    {
        static void Main(string[] args)
        {
            string source=string.Empty;
            string goalformat="ppm";
            string output = string.Empty;
            string[] input;
            try
            {                
                foreach(var a in args)
                {
                    input = a.Split('=');
                    switch(input[0])
                    {
                        case "--source":
                            { 
                                source = input[1];
                                string[] temp = source.Split('.');
                                if(temp[1]!="bmp")
                                {
                                    throw new FormatSupportedException("Формат ввода не поддерживется", temp[1]);
                                }
                            }                            
                            break;
                        case "--goal-format":
                            {
                                goalformat = input[1];
                                if(goalformat!="ppm")
                                {
                                    throw new FormatSupportedException("Формат вывода не поддерживется",goalformat);
                                }
                            }
                            break;
                        case "--output":
                            {
                                output = input[1];
                            }
                            break;
                        default:
                            throw new UnknowCommandException("Неизвестная команда",input[0]);
                    }

                }
            }
            catch(UnknowCommandException ex)
            {
                Console.WriteLine(ex.ErrorDetails);
            }
            catch(FormatSupportedException ex)
            {
                Console.WriteLine(ex.ErrorDetails);
            }
            if (output.Length == 0)
            {
                string[] temp = source.Split('.');
                output = temp[0] + "." + goalformat;
            }
            Conventor conventor = new Conventor();
            conventor.Convert(output, source);
        }
    }
}
