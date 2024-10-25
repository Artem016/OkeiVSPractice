namespace ElinKleymenov
{
    class Program
    {
        static bool stopThread = false; // Флаг для аварийного завершения потока
        static AutoResetEvent autoResetEvent = new AutoResetEvent(false); // Синхронизация потоков
        static double criticalValue = 1000; // Критическое значение
        static double progressionValue = 1; // Начальное значение прогрессии
        static double progressionFactor = 2; // Множитель для геометрической прогрессии

        static void Main(string[] args)
        {
            Thread t1 = new Thread(Thread1Method);
            Thread t2 = new Thread(Thread2Method);

            t1.Start();
            t2.Start();

            // Ожидаем завершения работы потоков
            t1.Join();
            t2.Join();

            Console.WriteLine("Все потоки завершены.");
        }

        // Поток 1: вывод данных от потока 2
        static void Thread1Method()
        {
            while (!stopThread)
            {
                // Ожидаем сигнал от потока 2 для вывода значения
                autoResetEvent.WaitOne();
                Console.WriteLine($"Thread1 получает значение от Thread2: {progressionValue}");
            }
        }

        // Поток 2: генерация геометрической прогрессии
        static void Thread2Method()
        {
            try
            {
                while (!stopThread)
                {
                    // Генерация следующего элемента прогрессии
                    progressionValue *= progressionFactor;

                    // Проверка на критическое значение
                    if (progressionValue > criticalValue)
                    {
                        Console.WriteLine("Критическое значение достигнуто. Поток 2 завершен аварийно.");
                        stopThread = true;
                        return;
                    }

                    // Сигнал для потока 1 о наличии нового значения
                    autoResetEvent.Set();

                    // Блокировка потока 2 при достижении значения 64 (пример)
                    if (progressionValue == 64)
                    {
                        Console.WriteLine("Thread2 временно приостановлен.");
                        Thread.Sleep(2000); // Время блокировки
                        Console.WriteLine("Thread2 возобновил работу.");
                    }

                    Thread.Sleep(500); // Задержка для имитации работы
                }
            }
            catch (ThreadAbortException ex)
            {
                Console.WriteLine("Thread2 был аварийно завершен.");
                Thread.ResetAbort(); // Отмена завершения
            }
        }
    }
}
