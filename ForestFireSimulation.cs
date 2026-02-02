using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using Avalonia.Controls;
using Microsoft.VisualBasic;
using Tmds.DBus.Protocol;

namespace forestfire;

public sealed class ForestFireSimulation
{
    public CellState[,] Grid;
    private CellState[,] _nextGrid;
    private readonly Random _rng = new();

    private readonly (int dx, int dy, int distSq)[] _neighborOffsets;
    private int _fireCounter = 0;

    public ForestFireSimulation()
    {
        Grid = new CellState[Constants.GridWidth, Constants.GridHeight];
        _nextGrid = new CellState[Constants.GridWidth, Constants.GridHeight];

        var offsets = new List<(int, int, int)>();
        for (int dx = -Constants.MaxBurnDistance; dx <= Constants.MaxBurnDistance; dx++)
        for (int dy = -Constants.MaxBurnDistance; dy <= Constants.MaxBurnDistance; dy++)
        {
            if (dx == 0 && dy == 0) continue;
            offsets.Add((dx, dy, dx * dx + dy * dy));
        }

        _neighborOffsets = offsets.ToArray();

        // Initialize grid with trees
        for (int x = 0; x < Constants.GridWidth; x++)
        for (int y = 0; y < Constants.GridHeight; y++)
        {
            if (_rng.NextDouble() < Constants.GrowthProbability)
            {
                if (_rng.NextDouble() < Constants.GrowthProbability * 5)
                {
                    if (_rng.NextDouble() < Constants.GrowthProbability * 10)
                        Grid[x, y] = new CellState(CellType.Tree);
                    else
                    {
                        Grid[x, y] = new CellState(CellType.Brush);
                    }
                }
                else
                {
                    Grid[x, y] = new CellState(CellType.Grass);
                }
            }
        }
    }

    public void Step()
    {
        for (int x = 0; x < Constants.GridWidth; x++)
        for (int y = 0; y < Constants.GridHeight; y++)
        {
            var cell = Grid[x, y];
            var INB = IsNeighborBurning(x, y, out int distSq);
            var fireRandom = _rng.NextDouble();
            var growthRandom = _rng.NextDouble();
            var firePause = _fireCounter > 0;

            switch (cell.Type)
            {
                case CellType.Charred:
                    if (growthRandom < Constants.GrowthProbability / Constants.SecondaryGrowthProbability * 4 && !firePause)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Dirt);
                    }
                    else
                    {
                        _nextGrid[x, y] = cell;
                    }
                    break;

                case CellType.Dirt:
                    if (growthRandom < Constants.GrowthProbability / Constants.SecondaryGrowthProbability * 3 && !firePause)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Grass);
                    }
                    else
                    {
                        _nextGrid[x, y] = cell;
                    }
                    break;

                case CellType.Grass:
                    if (INB.found && fireRandom < Constants.FireSpreadProbability / distSq)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Fire, 1, Grid[INB.nx, INB.ny].Fire!);
                        _nextGrid[x, y].Fire!.Increment();
                        _nextGrid[x, y].Fire!.IncrementActive();
                        _fireCounter++;
                    }
                    else if (growthRandom < Constants.GrowthProbability / Constants.SecondaryGrowthProbability * 2 && !firePause)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Brush);
                    }
                    else
                    {
                        _nextGrid[x, y] = cell;
                    }
                    break;

                case CellType.Brush:
                    if (INB.found && fireRandom < Constants.FireSpreadProbability * 2 / distSq)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Fire, 1, Grid[INB.nx, INB.ny].Fire!);
                        _nextGrid[x, y].Fire!.Increment();
                        _nextGrid[x, y].Fire!.IncrementActive();
                        _fireCounter++;
                    }
                    else if (growthRandom < Constants.GrowthProbability / Constants.SecondaryGrowthProbability && !firePause)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Tree);
                    }
                    else
                    {
                        _nextGrid[x, y] = cell;
                    }
                    break;

                case CellType.Tree:

                    if (fireRandom < Constants.LightningProbability && !firePause)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Fire, 1, new ForestFire());
                        _nextGrid[x, y].Fire!.Increment();
                        _nextGrid[x, y].Fire!.IncrementActive();
                        _fireCounter++;
                    }
                    else if (INB.found && fireRandom < Constants.FireSpreadProbability * 3 / distSq)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Fire, 1, Grid[INB.nx, INB.ny].Fire!);
                        _nextGrid[x, y].Fire!.Increment();
                        _nextGrid[x, y].Fire!.IncrementActive();
                        _fireCounter++;
                    }
                    else if (growthRandom < Constants.GrowthProbability / Constants.SecondaryGrowthProbability && !firePause)
                    {
                        _nextGrid[x, y] = new CellState(CellType.ThickTree);
                    }
                    else
                    {
                        _nextGrid[x, y] = cell;
                    }
                    break;

                case CellType.ThickTree:

                    if (fireRandom < Constants.LightningProbability*2 && !firePause)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Fire, 1, new ForestFire());
                        _nextGrid[x, y].Fire!.Increment();
                        _nextGrid[x, y].Fire!.IncrementActive();
                        _fireCounter++;
                    }
                    else if (INB.found && fireRandom < Constants.FireSpreadProbability * 4 / distSq)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Fire, 1, Grid[INB.nx, INB.ny].Fire!);
                        _nextGrid[x, y].Fire!.Increment();
                        _nextGrid[x, y].Fire!.IncrementActive();
                        _fireCounter++;
                    }
                    else
                    {
                        _nextGrid[x, y] = cell;
                    }
                    break;

                case CellType.Fire:
                    cell.BurnTime++;
                    if (cell.BurnTime == Constants.FireSpreadTime * 2)
                    {
                        _fireCounter--;
                    }
                    if (cell.BurnTime >= Constants.FireLifeTime)
                    {
                        cell.Fire!.DecrementActive();
                    }
                    else if (INB.found)
                    {
                        cell.Fire!.AddChildFire(Grid[INB.nx, INB.ny].Fire!);
                    }
                    if (cell.BurnTime >= Constants.FireLifeTime)
                    {
                        _nextGrid[x, y] = new CellState(CellType.Charred);
                    }
                    else
                    {
                        _nextGrid[x, y] = cell;
                    }
                    break;

            }
        }

        SwapBuffers();
    }

    private (bool found, int nx, int ny) IsNeighborBurning(int x, int y, out int distSq)
    {
        foreach (var (dx, dy, d) in _neighborOffsets)
        {
            int nx = x + dx;
            int ny = y + dy;

            if (nx < 0 || ny < 0 ||
                nx >= Constants.GridWidth || ny >= Constants.GridHeight)
                continue;

            if (Grid[nx, ny].Type == CellType.Fire && Grid[nx, ny].BurnTime <= Constants.FireSpreadTime)
            {
                distSq = d;
                return (true, nx, ny);
            }
        }

        distSq = 0;
        return (false, 0, 0);
    }
    private void SwapBuffers()
    {
        (Grid, _nextGrid) = (_nextGrid, Grid);
    }
}
