using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginRegister
{
    struct CfgFileLine
    {
        public string section;
        public string key;
        public string value;
        public CfgFileLine(string section = null,string key = null, string value = null)
        {
            this.section = section;
            this.key = key;
            this.value = value;
        }
    }
    class CfgFile
    {
        private string path;
        public List<CfgFileLine> cfgFile = new List<CfgFileLine>();

        public CfgFile(string path)
        {
            this.path = path;
            ReadToList();
        }

        private void ReadToList()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string currentSection = null;
                    string[] lineSplitted;
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine().Trim();
                        if (!string.IsNullOrWhiteSpace(line) && line[0] == '[' && line[line.Length - 1] == ']')
                        {
                            currentSection = line.Trim(new char[] { '[', ']' });
                        }
                        else if (line.Contains('='))
                        {
                            lineSplitted = line.Split(new char[] { '=' });
                            cfgFile.Add(new CfgFileLine(currentSection, lineSplitted[0], lineSplitted[1]));
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
        public void WriteFromList()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(path, false))
                {
                    string currentSectionName = null;
                    foreach (var line in cfgFile)
                    {
                        if (!String.Equals(currentSectionName, line.section))
                        {
                            currentSectionName = line.section;
                            if (!string.IsNullOrEmpty(line.section))
                                streamWriter.WriteLine("["+ line.section + "]");
                        }
                        streamWriter.WriteLine(line.key+"="+line.value);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void DeleteSection(string Section = null)
        {
            foreach(var line in cfgFile.Reverse<CfgFileLine>()) 
            {
                if (line.section == Section)
                    cfgFile.Remove(line);
            }
        }
        public void DeleteKey(string Key, string Section = null)
        {
            foreach (var line in cfgFile.Reverse<CfgFileLine>())
            {
                if ((line.section == null || String.Equals(line.section, Section, StringComparison.OrdinalIgnoreCase)) && (line.key?.EndsWith(Key, StringComparison.OrdinalIgnoreCase)==true))
                    cfgFile.Remove(line);
            }
        }
        public void AddKey(string Key, string Value, string Section = null)
        {
            int myIndex = cfgFile.FindLastIndex(p => String.Equals(p.section, Section, StringComparison.OrdinalIgnoreCase));
            if (myIndex >= 0)
                myIndex++;
            else
                myIndex = cfgFile.Count;

            cfgFile.Insert(myIndex, new CfgFileLine(Section, Key, Value));
        }
        public void PrintGfgFile()
        {
            foreach (var line in cfgFile)
            {
                Console.WriteLine("Section: " + line.section + " Key: " + line.key + " Value: " + line.value);
            }
        }
    }
}
