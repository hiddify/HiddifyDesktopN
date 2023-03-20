using System.Diagnostics;

// This program is jsut for restarting main program
try
{
    // Killing

    // Get running main program process id
    int mainProgramProcessID = int.Parse(Environment.GetCommandLineArgs()[1]);
    // Get process
    //var mainProcess = Process.GetProcessById(mainProgramProcessID);
    // Kill process
    //mainProcess.Kill();

    // Running

    // Main program exe path
    string mainProgramPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "HiddifyN.exe"));
    // Command to run program
    string cmd = $"{mainProgramPath}";

    // Run again
    Process.Start(cmd);
}
catch (Exception err)
{
    Console.Error.WriteLine(err);
}