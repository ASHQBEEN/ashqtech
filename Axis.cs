using Advantech.Motion;
using ashqtech.Enums;
using ashqtech.Utility;
using System;

namespace ashqtech
{
    public sealed class Axis
    {
        public string Name { get; } = string.Empty;
        internal IntPtr Handler => _handler;
        private IntPtr _handler = IntPtr.Zero;
        private readonly int _index;
        private readonly IntPtr _deviceHandler;

        private bool _servo = false;

        public Axis(IntPtr deviceHandler, int axisIndex)
        {
            _index = axisIndex;
            _deviceHandler = deviceHandler;
            Open(_deviceHandler, _index);
            ResetPosition();
        }

        public Axis(IntPtr deviceHandler, int axisIndex, string axisName) : this(deviceHandler, axisIndex) => Name = axisName;

        public OutLogic Output4 { get => GetOutputBit(4); set => SetOutputBit(4, value); }

        public double CommandPosition
        {
            get
            {
                double currentComandPosition = 0;
                uint actionResult = Motion.mAcm_AxGetCmdPosition(Handler, ref currentComandPosition);
                string errorPrefix = $"({Name}): Получение командной позиции ({currentComandPosition})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return currentComandPosition;
            }
            set
            {
                uint actionResult = Motion.mAcm_AxSetCmdPosition(Handler, value);
                string errorPrefix = $"({Name}): Задание командной позиции ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public double ActualPosition
        {
            get
            {
                double currentActPosition = 0;
                uint actionResult = Motion.mAcm_AxGetActualPosition(Handler, ref currentActPosition);
                string errorPrefix = $"({Name}): Получение актуальной позиции ({currentActPosition})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return currentActPosition;
            }
            set
            {
                uint actionResult = Motion.mAcm_AxSetActualPosition(Handler, value);
                string errorPrefix = $"({Name}): Задание актуальной позиции ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public AxisState State
        {
            get
            {
                ushort stateNum = 0;
                AxisState state;
                uint actionResult = Motion.mAcm_AxGetState(Handler, ref stateNum);
                state = (AxisState)stateNum;
                string errorPrefix = $"{Name}: Получение состояния ({state})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return state;
            }
        }

        public double HighVelocity
        {
            get
            {
                double vel = 0;
                uint actionResult = Motion.mAcm_GetF64Property(Handler, (uint)PropertyID.PAR_AxVelHigh, ref vel);
                string errorPrefix = $"{Name}: Получение конечной скорости ({vel})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return vel;
            }
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(Handler, (uint)PropertyID.PAR_AxVelHigh, value);
                string errorPrefix = $"{Name}: Задание конечной скорости ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public double Acceleration
        {
            get
            {
                double acc = 0;
                uint actionResult = Motion.mAcm_GetF64Property(Handler, (uint)PropertyID.PAR_AxAcc, ref acc);
                string errorPrefix = $"{Name}: Получение ускорения ({acc})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return acc;
            }
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(Handler, (uint)PropertyID.PAR_AxAcc, value);
                string errorPrefix = $"{Name}: Задание ускорения ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public double Deceleration
        {
            get
            {
                double dec = 0;
                uint actionResult = Motion.mAcm_GetF64Property(Handler, (uint)PropertyID.PAR_AxDec, ref dec);
                string errorPrefix = $"{Name}: Получение замедления ({dec})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return dec;
            }
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(Handler, (uint)PropertyID.PAR_AxDec, value);
                string errorPrefix = $"{Name}: Задание замедления ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public double LowVelocity
        {
            get
            {
                double vel = 0;
                uint actionResult = Motion.mAcm_GetF64Property(Handler, (uint)PropertyID.PAR_AxVelLow, ref vel);
                string errorPrefix = $"{Name}: Получение начальной скорости ({vel})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return vel;
            }
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(Handler, (uint)PropertyID.PAR_AxVelLow, value);
                string errorPrefix = $"{Name}: Задание начальной скорости ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        public Jerk Jerk
        {
            get
            {
                double jerk = 0;
                uint actionResult = Motion.mAcm_GetF64Property(Handler, (uint)PropertyID.PAR_AxJerk, ref jerk);
                Jerk varJerk = (Jerk)jerk;
                string errorPrefix = $"{Name}: Получение плавности ({varJerk})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return varJerk;
            }
            set
            {
                uint actionResult = Motion.mAcm_SetF64Property(Handler, (uint)PropertyID.PAR_AxJerk, (int)value);
                string errorPrefix = $"{Name}: Задание плавности ({value})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        /// <summary>
        /// Gets/Sets Servodrivers state
        /// </summary>
        /// <remarks>
        /// Get method is unsafe since it is based on variable and not on any Advantech.Motion method to get state
        /// </remarks>
        public bool Servo
        {
            get
            {
                return _servo;
            }
            set
            {
                _servo = value;
                uint servoState = 0;
                if (_servo) servoState = 1;
                uint actionResult = Motion.mAcm_AxSetSvOn(Handler, servoState);
                string errorPrefix = $"{Name}: Переключение сервопривода в состояние {_servo}";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            }
        }

        private uint _ioStatus
        {
            get
            {
                uint ioStatus = 0;
                uint actionResult = Motion.mAcm_AxGetMotionIO(Handler, ref ioStatus);
                string errorPrefix = $"{Name}: Получение статуса I/O ({ioStatus})";
                ApiErrorChecker.CheckForError(actionResult, errorPrefix);
                return ioStatus;
            }
        }

        public bool IsHardwareLimitP => (_ioStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTP) > 0;

        public bool IsHardwareLimitN => (_ioStatus & (uint)Ax_Motion_IO.AX_MOTION_IO_LMTN) > 0;

        private void Open(IntPtr deviceHandler, int axisIndex)
        {
            uint actionResult = Motion.mAcm_AxOpen(deviceHandler, (ushort)axisIndex, ref _handler);
            string errorPrefix = $"{Name}: Открытие оси (обработчик: {_handler})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void ResetPosition()
        {
            ActualPosition = 0;
            CommandPosition = 0;
        }

        private OutLogic GetOutputBit(ushort chanell)
        {
            byte bitDo = 0;
            uint actionResult = Motion.mAcm_AxDoGetBit(_handler, chanell, ref bitDo);
            var bit = (OutLogic)bitDo;
            string errorPrefix = $"{Name}: Получение бита порта вывода на канал {chanell} ({bit})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            return bit;
        }

        private void SetOutputBit(ushort channel, OutLogic bit)
        {
            uint actionResult = Motion.mAcm_AxDoSetBit(_handler, channel, (byte)bit);
            string errorPrefix = $"{Name}: Задание бита порта вывода на канале {channel} ({bit})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public byte GetInputBit(ushort chanell)
        {
            byte bit = 0;
            uint actionResult = Motion.mAcm_AxDiGetBit(_handler, chanell, ref bit);
            string errorPrefix = $"{Name}: Получение бита порта ввода на канале {chanell} ({bit})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
            return bit;
        }

        public void MoveRelative(double distance)
        {
            uint actionResult = Motion.mAcm_AxMoveRel(Handler, distance);
            string errorPrefix = $"{Name}: Относительное движение на расстояние {distance}";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void EmergencyStop()
        {
            uint actionResult = Motion.mAcm_AxStopEmg(Handler);
            string errorPrefix = $"{Name}: Незамедлительная остановка";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void ContinousMovement(MOTION_DIRECTION direction)
        {
            uint actionResult = Motion.mAcm_AxMoveVel(Handler, (ushort)direction);
            string errorPrefix = $"{Name}: Постоянное движение в направлении {direction}";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void MoveHome(HomeMode homeMode, MOTION_DIRECTION dirMode)
        {
            uint actionResult = Motion.mAcm_AxHome(Handler, (uint)homeMode, (uint)dirMode);
            string errorPrefix = $"{Name}: Движение в начало координат ({homeMode}, {dirMode})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void MoveToPoint(double position)
        {
            uint actionResult = Motion.mAcm_AxMoveAbs(Handler, position);
            string errorPrefix = $"{Name}: Движение в точку ({position})";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void ResetError()
        {
            uint actionResult = Motion.mAcm_AxResetError(Handler);
            string errorPrefix = $"{Name}: Сброс ошибки)";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }

        public void Close()
        {
            uint actionResult = Motion.mAcm_AxClose(ref _handler);
            string errorPrefix = $"{Name}: Закрытие оси";
            ApiErrorChecker.CheckForError(actionResult, errorPrefix);
        }
    }
}
