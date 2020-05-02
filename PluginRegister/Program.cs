using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace PluginRegister
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            DirectoryInfo d = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files"));

            IniFile pathMorrowindINI = new IniFile("morrowind.ini");
            CfgFile launcherCFG = new CfgFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "OpenMW", "launcher.cfg"));
            CfgFile openmwCFG = new CfgFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games","OpenMW","openmw.cfg"));

            if (d.Exists)
            {

                openmwCFG.DeleteKey("fallback-archive", null);
                openmwCFG.DeleteKey("data", null);
                openmwCFG.DeleteKey("content", null);
                launcherCFG.DeleteKey("content", "Profiles");
                launcherCFG.DeleteKey("currentprofile", "Profiles");
                pathMorrowindINI.DeleteSection("Archives");

                Console.WriteLine("Adding new BSA to Morrowind.ini:");
                int i = 0;
                if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", "Morrowind.bsa")))
                {
                    File.SetLastWriteTime(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", "Morrowind.bsa"), new DateTime(2002, 6, 21, 17, 31, 46));
                    openmwCFG.AddKey("fallback-archive", "Morrowind.bsa",null);
                    pathMorrowindINI.Write("Archive " + i, "Morrowind.bsa", "Archives");
                    Console.WriteLine("Archive " + i + "=Morrowind.bsa");
                    i++;
                }
                if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", "Tribunal.bsa")))
                {
                    File.SetLastWriteTime(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", "Tribunal.bsa"), new DateTime(2002, 10, 29, 21, 22, 06));
                    openmwCFG.AddKey("fallback-archive", "Tribunal.bsa", null);
                    pathMorrowindINI.Write("Archive " + i, "Tribunal.bsa", "Archives");
                    Console.WriteLine("Archive " + i + "=Tribunal.bsa");
                    i++;
                }
                if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", "Bloodmoon.bsa"))) 
                {
                    File.SetLastWriteTime(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", "Bloodmoon.bsa"), new DateTime(2003, 5, 1, 14, 37, 30));
                    openmwCFG.AddKey("fallback-archive", "Bloodmoon.bsa", null);
                    pathMorrowindINI.Write("Archive " + i, "Bloodmoon.bsa", "Archives");
                    Console.WriteLine("Archive " + i + "=Bloodmoon.bsa");
                    i++;
                }

                string[] vanillaArchives = new string[] { "morrowind.bsa", "tribunal.bsa", "bloodmoon.bsa" };
                foreach (var file in d.GetFiles("*.bsa"))
                {
                    if (!vanillaArchives.Any(s => s.Equals(file.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        openmwCFG.AddKey("fallback-archive", file.Name, null);
                        pathMorrowindINI.Write("Archive " + i, file.Name, "Archives");
                        Console.WriteLine("Archive " + i + "="+ file.Name);
                        i++;
                    }
                }

                //openmw
                openmwCFG.AddKey("data","\""+ Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+ "\"", null);
                openmwCFG.AddKey("data", "\"" + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files") + "\"", null);
                launcherCFG.AddKey("currentprofile", "My", "Profiles");

                //Plugins timestamp sorting, Morrowind.ini
                Console.WriteLine("Plugins timestamp sorting:");
                pathMorrowindINI.DeleteSection("Game Files");
                string line;
                i = 0;
                DateTime startDateTime = new DateTime(2002, 1, 1);
                if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LoadOrder.txt")))
                    using (StreamReader file = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LoadOrder.txt")))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            if (!(string.IsNullOrWhiteSpace(line) || line.StartsWith("//") || line.StartsWith("\\")))
                            {
                                if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", line)))
                                {
                                     if (line.EndsWith(".ESM", StringComparison.OrdinalIgnoreCase) || line.EndsWith(".ESP", StringComparison.OrdinalIgnoreCase))
                                    {
                                        Console.WriteLine(line + " " + startDateTime.ToShortDateString());
                                        File.SetLastWriteTime(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data files", line), startDateTime);
                                        startDateTime = startDateTime.AddDays(1);
                                        pathMorrowindINI.Write("GameFile" + i, line, "Game Files");
                                        i++;
                                        openmwCFG.AddKey("content", line, null);
                                        launcherCFG.AddKey("My/content", line, "Profiles");
                                    }
                                    else if (line.EndsWith(".omwaddon", StringComparison.OrdinalIgnoreCase))
                                    {
                                        openmwCFG.AddKey("content", line, null);
                                        launcherCFG.AddKey("My/content", line, "Profiles");
                                        Console.WriteLine(line + " -");
                                    }
                                }
                                if(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "OpenMW", "data", line)))
                                {
                                    if (line.EndsWith(".ESM", StringComparison.OrdinalIgnoreCase) || line.EndsWith(".ESP", StringComparison.OrdinalIgnoreCase) || line.EndsWith(".omwaddon", StringComparison.OrdinalIgnoreCase))
                                    {
                                        openmwCFG.AddKey("content", line, null);
                                        launcherCFG.AddKey("My/content", line, "Profiles");
                                        Console.WriteLine(line + " -");
                                    }
                                }
                            }
                        }
                        Console.WriteLine(i + @" ESM\ESP plugins found");
                    }
            }
            launcherCFG.WriteFromList();
            openmwCFG.WriteFromList();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Press any key...");
            Console.ReadKey();

        }
    }
}
