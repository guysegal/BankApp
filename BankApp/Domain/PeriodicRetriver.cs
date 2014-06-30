using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BankApp.Domain
{
    public class PeriodicRetriver
    {
        private readonly Timer _timer = new Timer();
        private bool _timerCallbackRunning;
        private readonly object _synchronizingObject = new object();
        private readonly ITransancationsRepository _transancationsRepository;
        private readonly IBank _bank;
        private readonly int _interval;

        public event Action RetrievalCompleted = delegate { }; 

        public PeriodicRetriver(
            IBank bank, 
            ITransancationsRepository transancationsRepository,
            TimeSpan interval)
        {
            _bank = bank;
            _transancationsRepository = transancationsRepository;
            _interval = (int) interval.TotalMilliseconds;

            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = false;
            _timer.Interval = 1;
        }

        public void Run()
        {
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_synchronizingObject)
            {
                if (_timerCallbackRunning) return;
                _timerCallbackRunning = true;
            }

            var cards = _bank.GetCreditCards();
            foreach (var creditCard in cards)
            {
                var lastTransaction = _transancationsRepository.GetLastTransactionDate(creditCard);
                var transancations = _bank.GetTranscationsForCreditCard(creditCard, lastTransaction, DateTime.Now);
                transancations.ForEach(t => _transancationsRepository.TryAdd(t));
            }
            RetrievalCompleted();
            _timer.Interval = _interval;
            _timerCallbackRunning = false;
        }
    }

}
