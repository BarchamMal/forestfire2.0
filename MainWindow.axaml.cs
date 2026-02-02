using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using forestfire;
using Microsoft.VisualBasic;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace forestfire;

public partial class MainWindow : Window
{
    private readonly ForestFireSimulation _sim;
    private readonly WriteableBitmap _bitmap;
    private readonly DispatcherTimer _timer;

    public MainWindow()
    {
        InitializeComponent();

        _sim = new ForestFireSimulation();

        _bitmap = new WriteableBitmap(
            new PixelSize(Constants.ScreenWidth, Constants.ScreenHeight),
            new Vector(96, 96),
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Unpremul);

        Canvas.Source = null;
        Canvas.Source = _bitmap;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1)
        };
        _timer.Tick += (_, _) => Update();
        _timer.Start();
    }

    private unsafe void Update()
    {
        _sim.Step();

        using var fb = _bitmap.Lock();
        var ptr = (byte*)fb.Address;

        for (int x = 0; x < Constants.GridWidth; x++)
        for (int y = 0; y < Constants.GridHeight; y++)
        {
            var cell = _sim.Grid[x, y];
            var color = cell.Type switch
            {   
                CellType.Charred => ((byte)33, (byte)14, (byte)8),
                CellType.Dirt => ((byte)82, (byte)41, (byte)9),
                CellType.Grass => ((byte)75, (byte)165, (byte)56),
                CellType.Brush => ((byte)65, (byte)121, (byte)19),
                CellType.Tree => ((byte)30, (byte)93, (byte)0),
                CellType.ThickTree => ((byte)11, (byte)63, (byte)0),
                CellType.Fire => (
                    (byte)255,
                    (byte)(255 - MathF.Abs(cell.BurnTime / (float)Constants.FireLifeTime - 0.5f) * 2 * 65f),
                    (byte)50
                ),
                _ => ((byte)0, (byte)0, (byte)0)
            };

            DrawCell(ptr, fb.RowBytes, x, y, color);
        }

        Canvas.InvalidateVisual();
    }

    private unsafe static void DrawCell(
        byte* buffer, int stride,
        int gx, int gy,
        (byte r, byte g, byte b) color)
    {
        int startX = gx * Constants.CellSize;
        int startY = gy * Constants.CellSize;

        for (int dx = 0; dx < Constants.CellSize; dx++)
        for (int dy = 0; dy < Constants.CellSize; dy++)
        {
            int px = startX + dx;
            int py = startY + dy;

            byte* p = buffer + py * stride + px * 4;
            p[0] = color.b;
            p[1] = color.g;
            p[2] = color.r;
            p[3] = 255; // Alpha channel - fully opaque
        }
    }
}
