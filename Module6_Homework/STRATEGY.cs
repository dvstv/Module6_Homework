using System;
class Program
{
    static void Main(string[] args)
    {
        PaymentContext context = new PaymentContext();
        while (true)
        {
            Console.WriteLine("\n=== система оплаты ===");
            Console.WriteLine("1 - банковская карта");
            Console.WriteLine("2 - PayPal");
            Console.WriteLine("3 - криптовалюта");
            Console.WriteLine("4 - наличные");
            Console.WriteLine("0 - выход");
            Console.Write("выберите способ оплаты: ");
            string? choice = Console.ReadLine();
            if (choice == "0")
                break;
            Console.Write("введите сумму: ");
            string? amountStr = Console.ReadLine();
            double amount = double.Parse(amountStr ?? "0");
            IPaymentStrategy? strategy = null;
            switch (choice)
            {
                case "1":
                    Console.Write("введите номер карты: ");
                    string? cardNumber = Console.ReadLine();
                    strategy = new CreditCardPaymentStrategy(cardNumber ?? "");
                    break;
                case "2":
                    Console.Write("введите email PayPal: ");
                    string? email = Console.ReadLine();
                    strategy = new PayPalPaymentStrategy(email ?? "");
                    break;
                case "3":
                    Console.Write("введите адрес криптокошелька: ");
                    string? walletAddress = Console.ReadLine();
                    strategy = new CryptoPaymentStrategy(walletAddress ?? "");
                    break;
                case "4":
                    strategy = new CashPaymentStrategy();
                    break;
                default:
                    Console.WriteLine("неверный выбор");
                    continue;
            }
            if (strategy != null)
            {
                context.SetPaymentStrategy(strategy);
                context.ExecutePayment(amount);
            }
        }
    }
}
public interface IPaymentStrategy
{
    void Pay(double amount);
}
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private string cardNumber;   
    public CreditCardPaymentStrategy(string cardNumber)
    {
        this.cardNumber = cardNumber;
    }
    public void Pay(double amount)
    {
        Console.WriteLine($"\n--- оплата банковской картой ---");
        Console.WriteLine($"номер карты: {MaskCardNumber(cardNumber)}");
        Console.WriteLine($"сумма: {amount} тг");
        Console.WriteLine($"комиссия банка: {amount * 0.02} тг");
        Console.WriteLine($"итого списано: {amount * 1.02} тг");
        Console.WriteLine("оплата успешно выполнена!\n");
    }
    private string MaskCardNumber(string cardNumber)
    {
        if (cardNumber.Length >= 4)
        {
            return "****-****-****-" + cardNumber.Substring(cardNumber.Length - 4);
        }
        return "****";
    }
}
public class PayPalPaymentStrategy : IPaymentStrategy
{
    private string email;   
    public PayPalPaymentStrategy(string email)
    {
        this.email = email;
    }
    public void Pay(double amount)
    {
        Console.WriteLine($"\n--- оплата через PayPal ---");
        Console.WriteLine($"email: {email}");
        Console.WriteLine($"сумма: {amount} тг");
        Console.WriteLine($"комиссия PayPal: {amount * 0.03} тг");
        Console.WriteLine($"итого списано: {amount * 1.03} тг");
        Console.WriteLine("оплата успешно выполнена!\n");
    }
}
public class CryptoPaymentStrategy : IPaymentStrategy
{
    private string walletAddress;   
    public CryptoPaymentStrategy(string walletAddress)
    {
        this.walletAddress = walletAddress;
    }
    public void Pay(double amount)
    {
        Console.WriteLine($"\n--- оплата криптовалютой ---");
        Console.WriteLine($"адрес кошелька: {walletAddress}");
        Console.WriteLine($"сумма: {amount} тг");
        double btcAmount = amount / 15000000;
        Console.WriteLine($"в BTC: {btcAmount:F8} BTC");
        Console.WriteLine($"комиссия сети: {amount * 0.01} тг");
        Console.WriteLine("транзакция отправлена в блокчейн");
        Console.WriteLine("оплата успешно выполнена!\n");
    }
}
public class CashPaymentStrategy : IPaymentStrategy
{
    public void Pay(double amount)
    {
        Console.WriteLine($"\n--- оплата наличными ---");
        Console.WriteLine($"сумма: {amount} тг");
        Console.WriteLine("оплата наличными при получении");
        Console.WriteLine("оплата успешно выполнена!\n");
    }
}
public class PaymentContext
{
    private IPaymentStrategy? paymentStrategy;   
    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        this.paymentStrategy = strategy;
    }
    public void ExecutePayment(double amount)
    {
        if (paymentStrategy == null)
        {
            Console.WriteLine("стратегия оплаты не установлена!");
            return;
        }   
        if (amount <= 0)
        {
            Console.WriteLine("некорректная сумма!");
            return;
        }
        paymentStrategy.Pay(amount);
    }
}