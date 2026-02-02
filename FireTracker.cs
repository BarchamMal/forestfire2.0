using System;
using System.Text;

namespace forestfire;

public struct FireTracker
{
    private StringBuilder _tracker;
    private int lines;

    public static FireTracker INSTANCE = new FireTracker();

    public FireTracker()
    {
        _tracker = new StringBuilder();
    }

    public void Log(int fireAmount)
    {
        _tracker.AppendLine(fireAmount.ToString());
        lines++;
        if (lines >= 100)
        {
            WriteToFile();
        }
    }

    public void WriteToFile()
    {
        System.IO.File.AppendAllText("fire-sizes.txt", _tracker.ToString());
        _tracker.Clear();
        lines = 0;
    }
}