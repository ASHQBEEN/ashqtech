namespace ashqtech
{
    public interface IAdvantechDevice
    {
        string Name { get; }
        bool IsVirtual { get; }
        Axis this[int index] { get; }
        AxesGroup Group { get; }
        void Close();
        void LoadConfig(string path);
        int AxesCount { get; }
    }
}
