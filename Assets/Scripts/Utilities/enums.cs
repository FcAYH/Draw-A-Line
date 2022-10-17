namespace DrawALine.Enums
{
    public enum LineState
    {
        ChooseStart, ChooseEnd, KeepDraw
    }

    public enum DrawAlgorithm
    {
        Interpolation, Scanning,
        EFLA_Division, EFLA_Multiplication,
        EFLA_Addition
    }
}