using System;
using System.IO;
using System.Reflection;
using GOLD;

namespace Parrot.Parser
{
    public static class ParserFactory
    {
        private static readonly object Locker = new object();
        static bool _init;

        private static GOLD.Parser _parser;

        private static BinaryReader GetResourceReader(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return new BinaryReader(stream);
        }

        public static void InitializeFactoryFromResource(string resourceName)
        {
            lock (Locker)
            {
                if (!_init)
                {
                    _parser = new GOLD.Parser();
                    _parser.LoadTables(GetResourceReader("Parrot.parrot.egt"));
                    _init = true;
                }
            }
        }

        public static GOLD.Parser CreateParser(TextReader reader)
        {
            if (_init)
            {
                return _parser;
            }
            throw new Exception("You must first Initialize the Factory before creating a parser!");
        }
    }
}