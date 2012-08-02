using System;
using System.IO;
using System.Reflection;
using GoldParser;

namespace Parrot.Parser
{
    public static class ParserFactory
    {
        static Grammar _grammar;
        private static readonly object Locker = new object();
        static bool _init;

        private static BinaryReader GetResourceReader(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return new BinaryReader(stream);
        }

        public static void InitializeFactoryFromFile(string fullCGTFilePath)
        {
            if (!_init)
            {
                BinaryReader reader = new BinaryReader(new FileStream(fullCGTFilePath, FileMode.Open));

                _grammar = new Grammar(reader);
                _init = true;
            }
        }

        public static void InitializeFactoryFromResource(string resourceName)
        {
            lock (Locker)
            {
                if (!_init)
                {
                    BinaryReader reader = GetResourceReader(resourceName);
                    _grammar = new Grammar(reader);
                    _init = true;
                }
            }
        }

        public static GoldParser.Parser CreateParser(TextReader reader)
        {
            if (_init)
            {
                return new GoldParser.Parser(reader, _grammar);
            }
            throw new Exception("You must first Initialize the Factory before creating a parser!");
        }
    }
}