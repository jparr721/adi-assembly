using System.Reflection;

public class ConfigHunter
{
    public static void Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
        {
            PrintUsage();
            return;
        }

        string targetDir = null;
        bool quiet = false;
        bool listAll = false;

        foreach (var a in args)
        {
            if (a == "-q") quiet = true;
            else if (a == "-a") listAll = true;
            else if (Directory.Exists(a)) targetDir = a;
        }

        if (targetDir == null)
        {
            Console.Error.WriteLine("[!] Please specify a directory to scan.");
            PrintUsage();
            return;
        }

        if (!quiet)
        {
            Console.WriteLine($"Scanning {targetDir}");
        }

        FindVulnerableBinaries(targetDir, listAll, quiet);
    }

    private static void PrintUsage()
    {
        Console.WriteLine(
@"Usage: adi_enum.exe [options] <directory>

Options:
  -q          Suppress normal output(only show findings)
  -a          Also list non‑vulnerable binaries
  -h, --help  Show this help message
");
    }

    private static bool DirectoryIsWritable(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            return false;
        }

        try
        {
            var testFile = Path.Combine(dirPath, Path.GetRandomFileName());
            using FileStream fs = File.Create(testFile, 1, FileOptions.DeleteOnClose);
            fs.Close();
            // writable
            return true;
        } catch
        {
            // Any other error means not writable
            return false;
        }
    }

    private static bool IsDotNetExe(string exePath)
    {
        try
        {
            var asmName = AssemblyName.GetAssemblyName(exePath);
            return true;
        } catch
        {
            return false;
        }
    }

    public static void FindVulnerableBinaries(string path, bool listAll, bool quiet)
    {
        //string[] frameworkPaths = {
        //    @"C:\Windows\Microsoft.NET\",
        //    @"C:\Program Files\dotnet\shared\Microsoft.NETCore.App"
        //};

        if (!quiet)
        {
            Console.WriteLine("[*] Searching for .NET binaries without .config files\n");
        }

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Path {path} does not exist or is not a directory.");
        }

        if (!quiet)
        {
            Console.WriteLine($"[*] Checking: {path}");
        }

        var enumOptions = new EnumerationOptions
        {
            RecurseSubdirectories = true
        };

        var exeFiles = Directory.GetFiles(path, "*.exe", enumOptions);


        foreach (string exe in exeFiles)
        {
            var parentDir = Path.GetDirectoryName(exe);
            if (parentDir == null)
            {
                continue;
            }
            string configFile = exe + ".config";
            bool missingConfig = !File.Exists(configFile);
            bool isDotNet = IsDotNetExe(exe);
            bool isVulnerable = missingConfig && isDotNet;
            bool isWritable = DirectoryIsWritable(parentDir);

            if (!listAll && !isVulnerable) continue;

            if (!quiet)
            {
                if (listAll || (isWritable && isVulnerable))
                {
                    Console.WriteLine($"[+] Vulnerable: {Path.GetFileName(exe)}");
                    Console.WriteLine($"    Path: {exe}");
                    Console.WriteLine($"    Missing: {configFile}\n");
                    Console.WriteLine($"    Writable: {isWritable}\n");
                    Console.WriteLine($"    IsDotNet: {isDotNet}\n");
                }
            } else if (isVulnerable)
            {
                Console.WriteLine(exe);
            }
        }
    }
}