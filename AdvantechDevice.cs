using Advantech.Motion;
using ashqtech.Utility;
using System;

namespace ashqtech
{
    public sealed class AdvantechDevice : IAdvantechDevice
    {
        private IntPtr handler = IntPtr.Zero;
        public string Name { get; private set; } = string.Empty;
        public bool IsVirtual { get; }

        private Axis[] axes;
        public int AxesCount => axes.Length;
        public Axis this[int index] { get { return axes[index]; } }

        public AxesGroup Group { get; }

        public AdvantechDevice(string queryName, uint axesCount, string[] axisNames = null)
        {
            if (axisNames?.Length < axesCount)
                throw new ArgumentException("Недостаточно имён для заданного количества осей.");
            BuildDeviceByName(queryName);

            if (axisNames is null)
            {
                axisNames = new string[axesCount];
                for (uint i = 0; i < axesCount; i++)
                    axisNames[i] = $"{Name}, Axis {i}";
            }
            InitializeAxesByCount(axesCount, axisNames);
            Group = new AxesGroup(Name);
        }

        private void BuildDeviceByName(string name)
        {
            IntPtr deviceHandler = IntPtr.Zero;

            DEV_LIST[] curAvailableDevs = GetDevices();
            if (curAvailableDevs.Length == 0)
                throw new Exception($"Не обнаружено подключенных устройств Advantech c именем, содержащим '{name}'.");

            foreach (var device in curAvailableDevs)
                if (device.DeviceName.Contains(name))
                {
                    uint actionResult = Motion.mAcm_DevOpen(device.DeviceNum, ref deviceHandler);
                    Name = device.DeviceName;
                    string errorPrefix = $"{Name}: Получение обработчика устройства ({deviceHandler})";
                    ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                    break;
                }

            handler = deviceHandler;
        }

        private DEV_LIST[] GetDevices()
        {
            DEV_LIST[] curAvailableDevs = new DEV_LIST[Motion.MAX_DEVICES];
            uint deviceCount = new uint();
            string errorPrefix = $"Получение списка доступных устройств Advantech...";
            uint actionResult = (uint)Motion.mAcm_GetAvailableDevs(curAvailableDevs, Motion.MAX_DEVICES, ref deviceCount);
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            return curAvailableDevs;
        }

        private void InitializeAxesByCount(uint axesCount, string[] axisNames)
        {
            uint maxAxesCount = GetMaxAxesCount();

            if (axesCount > maxAxesCount)
                throw new ArgumentException($"{Name}: Устройство не поддерживает {axesCount} осей. Максимальное количество - {maxAxesCount}.");

            axes = new Axis[axesCount];
            for (int i = 0; i < axesCount; i++)
            {
                axes[i] = new Axis(handler, i, axisNames[i]);
            }
        }

        private uint GetMaxAxesCount()
        {
            uint AxisPerDev = 0;
            uint actionResult = Motion.mAcm_GetU32Property(handler, (uint)PropertyID.FT_DevAxesCount, ref AxisPerDev);
            string errorPrefix = $"{Name}: Получение количества доступных осей ({AxisPerDev})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            return AxisPerDev;
        }

        public void Close()
        {
            Group.Close();
            foreach (var axis in axes)
                axis.Close();
            uint actionResult = Motion.mAcm_DevClose(ref handler);
            string errorPrefix = $"{Name}: Закрытие устройства";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void LoadConfig(string path)
        {
            uint actionResult = Motion.mAcm_DevLoadConfig(handler, path);
            string errorPrefix = $"{Name}: Загрукзка файла конфигурации: {path}";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

    }
}
