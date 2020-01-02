using System;
using System.Collections.Generic;
using System.IO;

namespace Lib
{
    public delegate void BinaryWriterFunction(BinaryWriter w);
    public delegate void BinaryReaderFunction(BinaryReader r);
    public static class FFunc
    {
        static public void WriteTextFile(string path, string text, bool newLine = true, bool append = true)
        {
            string writePath = path;
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, append, System.Text.Encoding.UTF8))
                {
                    if (newLine)
                    {
                        sw.WriteLine("");
                        sw.Write(text);
                    }
                    else sw.Write(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static public async void WriteTextFileAsync(string path, string text, bool newLine = true, bool append = true)
        {
            string writePath = path;

            try
            {

                using (StreamWriter sw = new StreamWriter(writePath, append, System.Text.Encoding.UTF8))
                {
                    if (newLine)
                    {
                        await sw.WriteLineAsync("");
                        await sw.WriteAsync(text);
                    }
                    else await sw.WriteAsync(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static public List<string> ReadTextFileRows(string path, bool filterNull = true)
        {
            List<string> rows = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (filterNull)
                        {
                            if (line == "") continue;
                        }
                        rows.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return rows;
        }
        static public string ReadTextFileAll(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public void WriteByteFile(string path, BinaryWriterFunction func )
        {
            try
            {
                // создаем объект BinaryWriter
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                {
                    func(writer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        static public void ReadByteFile(string path, BinaryReaderFunction func)
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {                    
                    func(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
