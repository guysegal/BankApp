using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BankApp;
using BankApp.Domain;
using BankApp.Infrastructure;
using BankApp.Infrastructure.Login;

namespace Tester
{
    class Program
    {
        private static TransacntionRepository _transancationRepository;
        private static IBank _bank;

        static void Main(string[] args)
        {
            _transancationRepository = new TransacntionRepository();

            Console.WriteLine("Choose Bank:");
            Console.WriteLine("1. Fibi");
            Console.WriteLine("2. Otzar");
            var bankChosed = Console.ReadLine();
            if (bankChosed == "1")
            {
                var fibiEncryptionService = new FibiEncryptionService();
                var fibiLoginService = new FibiWebSiteLoginService(fibiEncryptionService);
                var fibiHtmlParser = new FibiHtmlPagesParser();
                var fibiBankProxy = new FibiWebSiteProxy(fibiLoginService, fibiHtmlParser);
                _bank = new Bank(fibiBankProxy);
            }
            else if (bankChosed == "2")
            {
                var fibiEncryptionService = new FibiEncryptionService();
                var fibiLoginService = new FibiWebSiteLoginService(fibiEncryptionService);
                var otzarHtmlParser = new OtzarHtmlPagesParser();
                var fibiBankProxy = new FibiWebSiteProxy(fibiLoginService, otzarHtmlParser);
                _bank = new Bank(fibiBankProxy);
            }
            else
            {
                Console.WriteLine("Invalid");               
            }

            Console.WriteLine("Enter username:");
            var username = Console.ReadLine();
            Console.WriteLine("Enter password:");
            var password = Console.ReadLine();

            _bank.Login(username, password);
            Console.Clear();
            Console.WriteLine("Credit Card transancation retrival actiaved..");

            
            var perioicRetriver = new PeriodicRetriver(_bank, _transancationRepository, TimeSpan.FromHours(1));
            perioicRetriver.RetrievalCompleted += OnRetrievalCompleted;
            perioicRetriver.Run();
                                  
            Console.ReadLine();
        }

        static void OnRetrievalCompleted()
        {
            Console.WriteLine("Retrival completed - " + DateTime.Now);
            using (var fs = File.Create("transactions.txt"))
            {
                using (var sw = new StreamWriter(fs))
                {
                    var transactions = _transancationRepository.GetAll();
                    transactions = transactions.OrderBy(x => x.Date).ToList();
                    transactions.ForEach(sw.WriteLine);
                }
            } 
        }        
    }
}
