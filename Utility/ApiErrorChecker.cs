namespace ashqtech.Utility
{
    internal class ApiErrorChecker
    {
        public static void CheckForError(uint actionResult, string errorPrefix)
        {
            if (actionResult == (uint)Advantech.Motion.ErrorCode.SUCCESS)
                return;
            System.Text.StringBuilder errorDescription = new System.Text.StringBuilder(string.Empty, 100);
            //Get the error message according to error code returned from API
            Advantech.Motion.Motion.mAcm_GetErrorMessage(actionResult, errorDescription, 100);
            throw new System.Exception($"{errorPrefix} завершено ошибкой с кодом: {actionResult}\r\n{errorDescription}");
        }
    }
}
