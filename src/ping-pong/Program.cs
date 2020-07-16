using System;
using System.Threading;

namespace ping_pong
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ссигнализации для сотановки приложения
            // и завершения рабочих потоков
            var stop = new ManualResetEvent(initialState: false);

            // Создание событий для синхронизации потоков.
            //
            // Особеность AutoResetEvent в том что при получении 
            // сигнала и освобождении одного потока он автоматически
            // сбрасывается в несигнальное состояние.
            //
            // ping создаётся в изначально сигнальном состоянии
            // что бы обеспечить выполнение условия что ping
            // выводится первым
            var ping = new AutoResetEvent(initialState:true);
            // pong создаётся в изначально несигнальном состоянии
            // что бы обеспечить выполнение условия что ping
            // выводится первым
            var pong = new AutoResetEvent(initialState: false);

            // Создание рабочего потока для вывода строки "ping"
            var pinger = new Thread(() =>
            {
                while (!stop.WaitOne(0) && ping.WaitOne())
                {
                    Console.WriteLine("ping");
                    pong.Set(); // уведомление для ponger что можно вывести pong
                }
            });

            // Создание рабочего потока для вывода строки "pong"
            var ponger = new Thread(() =>
            {
                while (!stop.WaitOne(0) && pong.WaitOne())
                {
                    Console.WriteLine("pong");
                    ping.Set(); // уведомление для pinger что можно вывести ping
                }
            });

            // Запуск рабочих потоков
            pinger.Start();
            ponger.Start();

            // Ожидание пользовательского ввода для остановки приложения
            Console.ReadLine();

            // Установка флага остановки приложения сигнализирует всем
            // рабочим потокам об остановке
            stop.Set();

            // Ожидание завершения рабочих потоков 
            pinger.Join();
            ponger.Join();
        }
    }
}