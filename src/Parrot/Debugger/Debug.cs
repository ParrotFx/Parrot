// -----------------------------------------------------------------------
// <copyright file="Debug.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Debugger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Debug
    {
        private static readonly ILogger Instance = new Logger();

        private class Logger : ILogger
        {
            public ILogger WriteLine(string format, params object[] values)
            {
                System.Console.WriteLine(format, values);
                System.Diagnostics.Debug.WriteLine(format, values);
                return this;
            }

            public ILogger WriteLine(object value)
            {
                System.Console.WriteLine(value);
                System.Diagnostics.Debug.Write(value);

                return this;
            }

            public ILogger Write(string format, params object[] values)
            {
                System.Console.Write(format, values);
                System.Diagnostics.Debug.Write(string.Format(format, values));

                return this;
            }

            public ILogger Write(object value)
            {
                System.Console.Write(value);
                System.Diagnostics.Debug.Write(value);

                return this;
            }
        }
        
        public static ILogger WriteLine(object value)
        {
            return Instance.WriteLine(value);
        }

        public static ILogger WriteLine(string format, params object[] values)
        {
            return Instance.WriteLine(format, values);
        }
    }

    public interface ILogger
    {
        ILogger WriteLine(string format, params object[] values);
        ILogger WriteLine(object value);
        ILogger Write(string format, params object[] values);
        ILogger Write(object value);
    }
}
