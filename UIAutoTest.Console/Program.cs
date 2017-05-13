using System;
using System.Xml;

using UIAutoTest.Core;

namespace UIAutoTest.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument DomTest = new XmlDocument();
            DomTest.Load(args[0]);
            Engine UITestEngine = new Engine(DomTest);
            try
            {
                UITestEngine.Run(10000);
                System.Console.WriteLine("OK");
            }
            catch (Exception excep)
            {
                System.Console.WriteLine(excep.ToString());
            }
            System.Console.ReadLine();
        }
    }
}
