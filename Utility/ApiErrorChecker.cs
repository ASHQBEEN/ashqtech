using Advantech.Motion;
using System.Text;
using System;

namespace ashqtech.Utility
{
    internal class ApiErrorChecker
    {
        public static void CheckForError(uint actionResult, string errorPrefix)
        {
            if (actionResult == (uint)ErrorCode.SUCCESS)
                return;
            StringBuilder errorDescription = new StringBuilder(string.Empty, 100);
            //Get the error message according to error code returned from API
            Motion.mAcm_GetErrorMessage(actionResult, errorDescription, 100);
            throw new Exception($"{errorPrefix} завершено ошибкой с кодом: {actionResult}\r\n{errorDescription}");
        }
    }
}
