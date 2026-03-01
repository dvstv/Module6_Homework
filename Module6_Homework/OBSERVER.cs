using System;
using System.Collections.Generic;
class Program
{
    static void Main(string[] args)
    {
        CurrencyExchange exchange = new CurrencyExchange();
        BankObserver bank = new BankObserver("Народный Банк");
        TradingAppObserver tradingApp = new TradingAppObserver("TradingPro");
        NewsAgencyObserver newsAgency = new NewsAgencyObserver("Financial Times");
        exchange.Attach(bank);
        exchange.Attach(tradingApp);
        exchange.Attach(newsAgency);
        Console.WriteLine("=== обновление курса USD ===");
        exchange.SetCurrency("USD", 475.50);
        Console.WriteLine("\n=== обновление курса EUR ===");
        exchange.SetCurrency("EUR", 520.30);
        Console.WriteLine("\n=== обновление курса RUB ===");
        exchange.SetCurrency("RUB", 5.10);
        Console.WriteLine("\n=== отписка банка ===");
        exchange.Detach(bank);       
        Console.WriteLine("\n=== обновление курса USD после отписки ===");
        exchange.SetCurrency("USD", 480.00);
    }
}
public interface IObserver
{
    void Update(string currency, double rate);
}
public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(string currency);
}
public class CurrencyExchange : ISubject
{
    private List<IObserver> observers = new List<IObserver>();
    private Dictionary<string, double> currencyRates = new Dictionary<string, double>();
    public void Attach(IObserver observer)
    {
        observers.Add(observer);
        Console.WriteLine("наблюдатель подписан на обновления курсов валют");
    }
    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
        Console.WriteLine("наблюдатель отписан от обновлений");
    }
    public void Notify(string currency)
    {
        Console.WriteLine($"\nуведомление всех наблюдателей о {currency}...\n");
        double rate = currencyRates[currency];   
        foreach (IObserver observer in observers)
        {
            observer.Update(currency, rate);
        }
    }
    public void SetCurrency(string currency, double rate)
    {
        currencyRates[currency] = rate;
        Console.WriteLine($"курс {currency} обновлен: {rate} тг");
        Notify(currency);
    }   
    public double GetCurrency(string currency)
    {
        if (currencyRates.ContainsKey(currency))
        {
            return currencyRates[currency];
        }
        return 0;
    }
}
public class BankObserver : IObserver
{
    private string bankName;
    public BankObserver(string bankName)
    {
        this.bankName = bankName;
    }   
    public void Update(string currency, double rate)
    {
        Console.WriteLine($"[Банк {bankName}] получил обновление:");
        Console.WriteLine($"  {currency} = {rate} тг");
        Console.WriteLine($"  обновляем табло курсов валют...");
        Console.WriteLine($"  корректируем комиссии для обменных операций\n");
    }
}
public class TradingAppObserver : IObserver
{
    private string appName;
    private Dictionary<string, double> previousRates = new Dictionary<string, double>();
    public TradingAppObserver(string appName)
    {
        this.appName = appName;
    }
    public void Update(string currency, double rate)
    {
        Console.WriteLine($"[Приложение {appName}] получило обновление:");
        Console.WriteLine($"  {currency} = {rate} тг");
        if (previousRates.ContainsKey(currency))
        {
            double change = rate - previousRates[currency];
            string trend = change > 0 ? "↑" : "↓";
            Console.WriteLine($"  изменение: {trend} {Math.Abs(change):F2} тг ({(change / previousRates[currency] * 100):F2}%)");   
            if (Math.Abs(change) > 5)
            {
                Console.WriteLine($"  ВНИМАНИЕ: значительное изменение курса!");
                Console.WriteLine($"  отправка push-уведомления пользователям...");
            }
        }       
        previousRates[currency] = rate;
        Console.WriteLine();
    }
}
public class NewsAgencyObserver : IObserver
{
    private string agencyName;
    public NewsAgencyObserver(string agencyName)
    {
        this.agencyName = agencyName;
    }   
    public void Update(string currency, double rate)
    {
        Console.WriteLine($"[Новостное агентство {agencyName}] получило обновление:");
        Console.WriteLine($"  {currency} = {rate} тг");
        Console.WriteLine($"  подготовка новостной заметки:");
        Console.WriteLine($"  'Курс {currency} составил {rate} тенге на {DateTime.Now:HH:mm}'");
        Console.WriteLine($"  публикация в ленте новостей...\n");
    }
}
public class IndividualTraderObserver : IObserver
{
    private string traderName;
    private double buyThreshold;
    private double sellThreshold;
    public IndividualTraderObserver(string traderName, double buyThreshold, double sellThreshold)
    {
        this.traderName = traderName;
        this.buyThreshold = buyThreshold;
        this.sellThreshold = sellThreshold;
    }
    public void Update(string currency, double rate)
    {
        Console.WriteLine($"[Трейдер {traderName}] получил обновление:");
        Console.WriteLine($"  {currency} = {rate} тг");       
        if (rate <= buyThreshold)
        {
            Console.WriteLine($"  ДЕЙСТВИЕ: Покупаем {currency}! Цена ниже {buyThreshold}");
        }
        else if (rate >= sellThreshold)
        {
            Console.WriteLine($"  ДЕЙСТВИЕ: Продаем {currency}! Цена выше {sellThreshold}");
        }
        else
        {
            Console.WriteLine($"  Удерживаем позицию по {currency}");
        }
        Console.WriteLine();
    }
}