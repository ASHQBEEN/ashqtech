using Advantech.Motion;
using ashqtech.Utility;
using System;
using System.Text;

namespace ashqtech
{
    public sealed class AxesGroup
    {
        private readonly string deviceName;
        private IntPtr handler = IntPtr.Zero;

        public AxesGroup(string deviceName) => this.deviceName = deviceName;

        public double HighVelocity
        {
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(handler, (uint)PropertyID.PAR_GpVelHigh, value);
                string errorPrefix = $"{deviceName}: Задание конечной скорости группы ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public double Acceleration
        {
            set
            {

                uint actionResult = Motion.mAcm_SetF64Property(handler, (uint)PropertyID.PAR_GpAcc, value);
                string errorPrefix = $"{deviceName}: Установка ускорения группы ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public double LowVelocity
        {
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(handler, (uint)PropertyID.PAR_GpVelLow, value);
                string errorPrefix = $"{deviceName}: Задание начальной скорости группы ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public double Deceleration
        {
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(handler, (uint)PropertyID.PAR_GpDec, value);
                string errorPrefix = $"{deviceName}: Задание замедления группы ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public void Remove(Axis axis)
        {
            uint actionResult = Motion.mAcm_GpRemAxis(handler, axis.Handler);
            string errorPrefix = $"{deviceName}: Удаление оси {axis.Name} из группы";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public GroupState State
        {
            get
            {
                ushort GpState = 0;
                GroupState state;
                uint actionResult = Motion.mAcm_GpGetState(handler, ref GpState);
                state = (GroupState)GpState;
                string errorPrefix = $"{deviceName}: Получение состояния группы ({state})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return state;
            }
        }

        public void Add(Axis axis)
        {
            uint actionResult = Motion.mAcm_GpAddAxis(ref handler, axis.Handler);
            string errorPrefix = $"{deviceName}: Добавление оси {axis.Name} в группу";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void Close()
        {
            if (handler == IntPtr.Zero) return;
            uint actionResult = Motion.mAcm_GpClose(ref handler);
            string errorPrefix = $"{deviceName}: Закрытие группы";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void MoveAbsolute(double[] positions)
        {
            uint actionResult = Motion.mAcm_GpMoveLinearAbs(handler, positions);
            StringBuilder position = new StringBuilder();
            for (int i = 0; i < positions.Length; i++)
            {
                position.Append(positions[i]);
                position.Append("; ");
            }
            position.Remove(position.Length - 2, 2);
            string errorPrefix = $"{deviceName}: Движение группы в точку ({position})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void EmergencyStop()
        {
            if (handler == IntPtr.Zero) return;
            uint actionResult = Motion.mAcm_GpStopEmg(handler);
            string errorPrefix = $"{deviceName}: Незамедлительная остановка группы";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }
    }
}
