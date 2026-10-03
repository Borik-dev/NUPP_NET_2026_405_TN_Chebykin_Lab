using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsOnlineStore.ElectronicsOnlineStore.Common
{
    public delegate void ProductPurchasedEventHandler(string  productName);

    public interface ICrudService<T>
    {
        void Create(T element);
        T Read(Guid id);
        IEnumerable<T> ReadAll();
        void Update(T element);
        void Remove(T element);
    }

    public class MyProductList<T> : ICrudService<T> where T : Product
    {
        private List<T> products;
        public MyProductList()
        {
            products = new List<T>();
        }
        public void Create(T element)
        {
            products.Add(element);
        }
        public T Read(Guid id)
        {
            return products.FirstOrDefault(p => p.Id == id);
        }
        public IEnumerable<T> ReadAll()
        {
            return products;
        }
        public void Update(T element)
        {
            var existingProduct = Read(element.Id);
            if (existingProduct != null)
            {
                int index = products.FindIndex(p => p.Id == element.Id);
                
                products[index] = element;
            }
        }

        public void Remove(T element)
        {
            products.Remove(element);
        }
    }

    public class Product 
    {
        //статична властивість для підрахунку кількості створених продуктів
        public static int ProductCount { get; private set; }
        //статичний конструктор для ініціалізації ститочної властивості
        static Product()
        {
            ProductCount = 0;
        }
        //конструктор за замовчуванням
        public Product()
        {
            Name = "Unknown";
            Price = 0.0f;
            Weight = 0.0f;
            ProductCount++;
            Id = Guid.NewGuid();
        }
        //конструктор з параметрами
        public Product(string name, float price, float weight)
        {
            Name = name;
            Price = price;
            Weight = weight;
            ProductCount++;
            Id = Guid.NewGuid();
        }
        //властивості продукту
        public string Name { get; set; }
        public float Price { get; set; }

        public Guid Id { get; set; }

        public float Weight { get; set; }
        //метод для відображення інформації про продукт
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"Product Name: {Name}, Price: {Price:C}");
        }
        public static int GetProductCount()
        {
            return ProductCount;
        }

        //подія для повідомлення про покупку продукту
        public event ProductPurchasedEventHandler ProductPurchased;

        //метод для виклику події покупки продукту
        public void OnProductPurchased( )
        {
            ProductPurchased?.Invoke(this.Name);
        }

    }

    public static class ProductExtensions
    {
        //розширюючий метод для отримання ціни продукту зі знижкою
        public static void GetPriceWithDiscount(this Product product, float discountPercentage)
        {
            float discountedPrice = product.Price - (product.Price * discountPercentage / 100);
            Console.WriteLine($"Продукт {product.Name} зі {discountPercentage}% знижкою матиме ціну: {discountedPrice:C}");
            product.Price = discountedPrice;
        }
    }

    public class Computer : Product
    {
        //конструктор за замовчуванням
        public Computer()
        {
            Processor = "Unknown";
            RAM = 0;
            GraphicsCard = "Unknown";
        }
        //конструктор з параметрами
        public Computer(string name, float price, float weight, string processor, int ram, string graphicsCard)
            : base(name, price, weight)
        {
            Processor = processor;
            RAM = ram;
            GraphicsCard = graphicsCard;
        }
        //властивості комп'ютера
        public string Processor { get; set; }
        public int RAM { get; set; }

        public string GraphicsCard { get; set; }
        //перевизначений метод для відображення інформації про комп'ютер
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Processor: {Processor}, RAM: {RAM}GB, Graphics Card: {GraphicsCard}");
        }

        
    }

    public class Smartphone : Product
    {
        //конструктор за замовчуванням
        public Smartphone()
        {
            OperatingSystem = "Unknown";
            Storage = 0;
            ScreenSize = 0.0;
        }
        //конструктор з параметрами
        public Smartphone(string name, float price, float weight, string operatingSystem, int storage, double screenSize)
            : base(name, price, weight)
        {
            OperatingSystem = operatingSystem;
            Storage = storage;
            ScreenSize = screenSize;
        }
        //властивості смартфона
        public string OperatingSystem { get; set; }
        public int Storage { get; set; }

        public double ScreenSize { get; set; }

        //перевизначений метод для відображення інформації про смартфон
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"Operating System: {OperatingSystem}, Storage: {Storage}GB");
        }

        
       
    }

    public class NotificationService
    {
        //метод, який підписується на подію покупки продукту та відправляє повідомлення

        public static void SendNotification(string message)
        {
            Console.WriteLine($"Вітаємо з покупокю продукту {message}");
        }
    }

    public class Customer
    {
        //конструктор за замовчуванням
        public Customer()
        {
            Name = "Unknown";
            Email = "Unknown";
        }
        //конструктор з параметрами
        public Customer(string name, string email)
        {
            Name = name;
            Email = email;
        }
        //властивості покупця
        public string Name { get; set; }
        public string Email { get; set; }
        //метод для відображення інформації про покупця
        public void DisplayInfo()
        {
            Console.WriteLine($"Customer Name: {Name}, Email: {Email}");
        }
        //метод для покупки продукту
        public void BuyProduct(Product product)
        {
            Console.WriteLine($"{Name} bought {product.Name} for {product.Price:C}");
            product.ProductPurchased += NotificationService.SendNotification;
            product.OnProductPurchased();


        }

    }
}
