#region Using declarations
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using ForgeBoard.Core;
#endregion

//This namespace holds GUI items and is required.
namespace NinjaTrader.Gui.NinjaScript
{
    internal class HotReloader
	{
		internal void Init()
		{
            // Create a new FileSystemWatcher and set its properties

            FileSystemWatcher watcher = new FileSystemWatcher();

			var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            watcher.Path = documentPath + @"\ForgeBoard\Extenstions";


            watcher.IncludeSubdirectories = true;


            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName |
            NotifyFilters.DirectoryName;


            // Add event handlers for the events raised by the FileSystemWatcher
            watcher.Changed += Watcher_Created;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Created;
            watcher.Deleted += Watcher_Created;
            // Start monitoring the directory


            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            MessageBox.Show(e.Name);
        }

        private void LoadAndInspectDLLs(string directoryPath)
        {
            // Check all DLL files in the specified directory
            foreach (string dllFile in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllFile);

                    // Loop through types in the assembly to find types that inherit from a specific base class
                    foreach (Type type in assembly.GetTypes())
                    {
                        // Check if the type is a class and inherits from your specified base class
                        if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BarExtension)))
                        {
                            // Perform actions with types that inherit from YourBaseClass
                            // For example, you could create instances or execute methods
                            // Instantiate the class or perform any desired action
                            BarExtension instance = (BarExtension)Activator.CreateInstance(type);
                            // Do something with the instance...

                            instance.Init();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions, e.g., log or display an error message
                    Console.WriteLine($"Error loading DLL {dllFile}: {ex.Message}");
                }
            }
        }
    }
}