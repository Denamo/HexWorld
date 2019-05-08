using UnityEngine;

namespace logic.debug
{
    public static class Debugger
    {
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }

        public static void DoAssert(bool test, string message)
        {
            if(!test)
                Error(message);
        }

        public static void print(string message)
        {
            Log(message);
        }

        public static void error(string message)
        {
            Error(message);
        }

        public static void warning(string message)
        {
            Warning(message);
        }

        public static void doAssert(bool test, string message)
        {
            DoAssert(test, message);
        }

    }

}

