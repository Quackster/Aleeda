using System;
using System.Data;

using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Console.SetWindowSize(100, 40);

            Console.Title = "Aleeda: Habbo Hotel Emulation";
            Console.WindowWidth = 110;
            Console.WindowHeight = 32;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(@"   _   _               _       ");
            Console.WriteLine(@"  /_\ | | ___  ___  __| | __ _ ");
            Console.WriteLine(@" //_\\| |/ _ \/ _ \/ _` |/ _` |");
            Console.WriteLine(@"/  _  \ |  __/  __/ (_| | (_| |");
            Console.WriteLine(@"\_/ \_/_|\___|\___|\__,_|\__,_|" + "\n");
            Console.ResetColor();

            AleedaEnvironment.Initialize();
        }
    }
}
