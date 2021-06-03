using System;
using System.Collections.Generic;
using System.Threading;
using CowinPro.DataTypes;
using Telegram;
using Telegram.Bot;

namespace CowinPro
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            Console.WriteLine("Enter Pincode");
            string pincode = Console.ReadLine();          
            CheckAvailability checkAvailability = new CheckAvailability(pincode);
            var result =  checkAvailability.Check(token);
            String realResult = "Available Slots";
            foreach(var res in result)
               {
                realResult = realResult + "\nAddress :" + res.Address + "\nAvailable Quantity :" + res.available + "\nDate :" + res.date + "\nVaccine Name :" + res.Vaccinename;
               }
            var bot = new TelegramBotClient("1881981952:AAHOcy2LmhTRGzUHieIoN3YGsNn-_EA_6EI");
            var s = await bot.SendTextMessageAsync("895209023", realResult);
            Console.WriteLine(realResult);
            
        }
    }
}
