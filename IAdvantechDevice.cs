namespace ashqtech
{
    public interface IAdvantechDevice
    {
        string Name { get; }
        Axis this[int index] { get; }
        AxesGroup Group { get; }
        void Close();
        void LoadConfig(string path);
        int AxesCount { get; }
    }
}
