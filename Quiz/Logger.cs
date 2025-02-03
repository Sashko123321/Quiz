using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz;
class Logger
{
    private string path;

    public Logger(string filePath)
    {
        path = filePath;

        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) { }
    }

    public void Info(string message)
    {
        WriteLog("INFO", message);
    }

    public void Warn(string message)
    {
        WriteLog("WARNING", message);
    }

    public void Error(string message)
    {
        WriteLog("ERROR", message);
    }

    private void WriteLog(string type, string message)
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
        {
            using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                streamWriter.WriteLine($"{DateTime.Now} [{type}] {message}");
            }
        }
    }
}


