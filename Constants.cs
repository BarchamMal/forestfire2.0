namespace forestfire;

public static class Constants
{
    public const int ScreenWidth = 1600;
    public const int ScreenHeight = 1600;
    public const int CellSize = 8;

    public const int GridWidth = ScreenWidth / CellSize;
    public const int GridHeight = ScreenHeight / CellSize;

    public const double GrowthProbability = 0.1;
    public const double SecondaryGrowthProbability = 100;
    public const double LightningProbability = 0.00001;
    public const double FireSpreadProbability = 0.2;

    public const int MaxBurnDistance = 1;
    public const int FireLifeTime = 24;
    public const int FireSpreadTime = 1;
}
