using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BankApp.Domain;
using BankApp.Infrastructure;
using BankApp.Infrastructure.Login;
using FakeItEasy;
using NUnit.Framework;

namespace BankApp.Tests.Integration
{
    [TestFixture]
    public class BankAppIntegrationTests
    {
        [Test]
        public void Login_To_Fibi_Bank_And_Get_All_The_Transactions_Within_Time_Span()
        {
            var fibiEncryptionService = new FibiEncryptionService();
            var fibiLoginService = new FibiWebSiteLoginService(fibiEncryptionService);
            var fibiHtmlParser = new FibiHtmlPagesParser();
            var fibiBankProxy = new FibiWebSiteProxy(fibiLoginService, fibiHtmlParser);

            var myBank = new Bank(fibiBankProxy);
            myBank.Login("I378SLE", "qyw2j3ka");
            var creditCards = myBank.GetCreditCards();
            var card = creditCards.First();
            var transactions = myBank.GetTranscationsForCreditCard(card, new DateTime(2014, 5, 1), new DateTime(2014, 5, 24));
            
            Assert.That(transactions.Count == 27);
        }

        [Test]
        public void Login_To_Otzar_Bank_And_Get_All_The_Transactions_Within_Time_Span()
        {
            var fibiEncryptionService = new FibiEncryptionService();
            var fibiLoginService = new FibiWebSiteLoginService(fibiEncryptionService);
            var otzarHtmlPagesParser = new OtzarHtmlPagesParser();
            var fibiBankProxy = new FibiWebSiteProxy(fibiLoginService, otzarHtmlPagesParser);

            var myBank = new Bank(fibiBankProxy);
            myBank.Login("VZ766DL", "qyw2j3kb");
            var creditCards = myBank.GetCreditCards();
            var card = creditCards.First();
            var transactions = myBank.GetTranscationsForCreditCard(card, new DateTime(2014, 5, 1), new DateTime(2014, 5, 24));

            Assert.That(transactions.Count == 11);
        }

        [Test]
        public void Periodic_Retriver_Runs_Periodically_And_Insets_Only_New_Transancation_To_The_Repository()
        {
            var bank = A.Fake<IBank>();
            var creditCard = new CreditCard("1234");

            var transactionRespository = new TransacntionRepository();
            transactionRespository.TryAdd(new CreditCardTransaction
            {
                Id = "7846434",
                Date = new DateTime(2014, 4, 30),
                Amount = 100,
                Description = "movie",
                PaymentsTransaction = false,
                CreditCard = creditCard
            });

            var transcations1 = new List<CreditCardTransaction>
            {
                new CreditCardTransaction
                {
                    Id = "424265",
                    Date = new DateTime(2014, 5, 1),
                    Amount = 100,
                    Description = "restuarant",
                    PaymentsTransaction = false,
                    CreditCard = creditCard
                },
                new CreditCardTransaction
                {
                    Id = "534534645",
                    Date = new DateTime(2014, 5, 2),
                    Amount = 100,
                    Description = "kiosk",
                    PaymentsTransaction = false,
                    CreditCard = creditCard
                },
                new CreditCardTransaction
                {
                    Id = "9653636",
                    Date = new DateTime(2014, 5, 20),
                    Amount = 100,
                    Description = "vending machine",
                    PaymentsTransaction = false,
                    CreditCard = creditCard
                }
            };

            var transcations2 = new List<CreditCardTransaction>
            {
                new CreditCardTransaction
                {
                    Id = "9653636",
                    Date = new DateTime(2014, 5, 20),
                    Amount = 100,
                    Description = "vending machine",
                    PaymentsTransaction = false,
                    CreditCard = creditCard
                },
                new CreditCardTransaction
                {
                    Id = "653464567",
                    Date = new DateTime(2014, 5, 20),
                    Amount = 100,
                    Description = "vending machine",
                    PaymentsTransaction = false,
                    CreditCard = creditCard
                }
            };

            A.CallTo(() => bank.GetCreditCards()).Returns(new List<CreditCard> {creditCard});

            A.CallTo(() =>
                bank.GetTranscationsForCreditCard(creditCard, new DateTime(2014, 4, 30), A<DateTime>.That.Matches(d => d > new DateTime(2014, 5, 20))))
                .Returns(transcations1);

            A.CallTo(() =>
                bank.GetTranscationsForCreditCard(creditCard, new DateTime(2014, 5, 20), A<DateTime>.That.Matches(d => d > new DateTime(2014, 5, 20))))
                .Returns(transcations2);

            var retrivalsCount = 0;
            var wait = new AutoResetEvent(false);
            var periodicRetriver = new PeriodicRetriver(bank, transactionRespository, TimeSpan.FromMilliseconds(50));
            periodicRetriver.RetrievalCompleted += () => { if (++retrivalsCount == 2) wait.Set(); };

            periodicRetriver.Run();
            wait.WaitOne();
            periodicRetriver.Dispose();

            var savedTransancations = transactionRespository.GetAll(creditCard);
            Assert.That(savedTransancations.Count == 5);
        }
    }
}
