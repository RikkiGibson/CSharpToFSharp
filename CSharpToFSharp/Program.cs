using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientUtils.FsharpLib;
using DataContracts;
using System.Reflection;

namespace CSharpToFSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int ans1 = SomeMath.factorial1(6);
            int ans2 = SomeMath.factorial2(6);
            int ans3 = SomeMath.factorial3(6);
            int ans4 = SomeMath.factorial4(6);

            Console.WriteLine("Testing F# factorial(6)!");
            Console.WriteLine("First F# factorial: " + ans1);
            Console.WriteLine("Second F# factorial: " + ans2);
            Console.WriteLine("Third F# factorial: " + ans3);
            Console.WriteLine("Fourth F# factorial: " + ans4);

            Console.WriteLine("Pass in a code to parse:");
            string input = Console.ReadLine();

            var message = Parser.getMessage(input);

            Console.WriteLine("Got a parsed message!");
            Console.WriteLine(message.IsThisTheRealLife);
            Console.WriteLine(message.IsThisJustFantasy);
            Console.WriteLine(message.CaughtInALandSlide);
            
            foreach (var item in message.NoEscapeFromReality)
            {
                Console.Write(item + "\t");
            }
            Console.WriteLine("\n");

            HereICome foo = ReadyOrNot.ChickenBone;
            Console.WriteLine("Now let's try to consume an F# type:");
            Console.WriteLine("Type: {0} Name: {1} Age: {2}\n", foo.GetType().Name, foo.Name, foo.Age);

            Console.WriteLine("Now let's try to instanciate an F# type:");
            HereICome bar = new HereICome("Fred", 80);
            Console.WriteLine("Name: {0} Age: {1}\n", bar.Name, bar.Age);

            Console.WriteLine("Reflection test:");
            foreach (PropertyInfo p in typeof(HereICome).GetProperties())
            {
                Console.Write("{0}: {1}\t", p.Name, p.GetValue(bar, null));
            }

            var derp = FSharpLib.LocationCodeParser.parseLatLng("=30, 40", new LocationFilterMessage());
            Console.ReadLine();
        }
    }
}
