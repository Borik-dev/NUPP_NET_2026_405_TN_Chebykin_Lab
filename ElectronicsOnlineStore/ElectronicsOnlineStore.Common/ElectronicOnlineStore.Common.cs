using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElectronicsOnlineStore.ElectronicsOnlineStore.Common
{
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
    
    public delegate void ProductPurchasedEventHandler(string productName);

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
        //список продуктів
        private List<T> products;
        //конструктор класу
        public MyProductList()
        {
            products = new List<T>();
        }
        //реалізація методу Create для додавання продукту
        public void Create(T element)
        {
            products.Add(element);
        }
        //реалізація методу Read для отримання продукту за його ідентифікатором
        public T Read(Guid id)
        {
            return products.FirstOrDefault(p => p.Id == id);
        }

        //реалізація методу ReadAll для отримання всіх продуктів
        public IEnumerable<T> ReadAll()
        {
            return products;
        }

        //реалізація методу Update для оновлення продукту
        public void Update(T element)
        {
            var existingProduct = Read(element.Id);
            if (existingProduct != null)
            {
                int index = products.FindIndex(p => p.Id == element.Id);

                products[index] = element;
            }
        }
        //реалізація методу Remove для видалення продукту
        public void Remove(T element)
        {
            products.Remove(element);
        }
    }

    //асинхронний інтерфейс для CRUD операцій
    public interface ICrudServiceAsync<T> : IEnumerable<T>
    {
        Task<bool> CreateAsync(T element);
        Task<T> ReadAsync(Guid id);
        Task<IEnumerable<T>> ReadAllAsync();
        Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
        Task<bool> UpdateAsync(T element);
        Task<bool> RemoveAsync(T element);
        Task<bool> SaveAsync();
    }

    public class MyProductListAsync<T> : ICrudServiceAsync<T> where T : Product 
    {
        private readonly object _lock = new object();
        //потокобезпечна колекція продуктів
        private ConcurrentBag<T> products;
        //конструктор класу
        public MyProductListAsync()
        {
            products = new ConcurrentBag<T>();
        }
        //конструктор з параметром для ініціалізації колекції продуктів
        public MyProductListAsync(IEnumerable<T> initialProducts)
        {
            products = new ConcurrentBag<T>(initialProducts);
        }

        //реалізація асинхронного методу Create для додавання продукту
        public Task<bool> CreateAsync (T element)
        {
            lock (_lock)
            {
                products.Add(element);
                return Task.FromResult(true);
            }

        }

        //реалізація асинхронного методу Read для отримання продукту за його ідентифікатором
        public Task<T> ReadAsync(Guid id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        //реалізація асинхронного методу ReadAll для отримання всіх продуктів
        public Task<IEnumerable<T>> ReadAllAsync()
        {
            return Task.FromResult(products.AsEnumerable());
        }

        //реалізація асинхронного методу ReadAll для отримання продуктів з пагінацією
        public  Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            var pagedProducts = products.Skip((page - 1) * amount).Take(amount);
            return Task.FromResult(pagedProducts);

        }

        //реалізація асинхронного методу Update для оновлення продукту
        public Task<bool> UpdateAsync(T element)
        {
            lock (_lock)
            {
                var existingProduct = products.FirstOrDefault(p => p.Id == element.Id);
                if (existingProduct != null)
                {
                    products = new ConcurrentBag<T>(products.Where(p => p.Id != element.Id));
                    products.Add(element);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            
        }

        //реалізація асинхронного методу Remove для видалення продукту
        public Task<bool> RemoveAsync(T element)
        {
            lock (_lock)
            {
                var existingProduct = products.FirstOrDefault(p => p.Id == element.Id);
                if (existingProduct != null)
                {
                    products = new ConcurrentBag<T>(products.Where(p => p.Id != element.Id));
                    return Task.FromResult(true);
                }
                return Task.FromResult(false)   ;
            }
        }

        //реалізація асинхронного методу Save для збереження продуктів 
        public async Task<bool> SaveAsync()
        {         
            string FilaPath = "products.txt";
            using (StreamWriter writer = new StreamWriter(FilaPath))
            {
                foreach (var product in products)
                {
                    await writer.WriteLineAsync($"Product Name: {product.Name}, Price: {product.Price:C}, Weight: {product.Weight}, Id: {product.Id}");
                }
            }
            return true;         
            
        }

        public IEnumerator<T> GetEnumerator()
        {
            return products.GetEnumerator();
        }        

        System.Collections.IEnumerator
            System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }


    public class Product
    {
        //статична властивість для підрахунку кількості створених продуктів
        public static int ProductCount { get; private set; }
        //статичний конструктор для ініціалізації статичної властивості
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
        //статичний метод для отримання кількості створених продуктів
        public static int GetProductCount()
        {
            return ProductCount;
        }

        //подія для повідомлення про покупку продукту
        public event ProductPurchasedEventHandler ProductPurchased;

        //метод для виклику події покупки продукту
        public void OnProductPurchased()
        {
            ProductPurchased?.Invoke(this.Name);
        }

    }


    public class Computer : Product
    {
        //статичні масиви для генерації випадкових даних комп'ютера
        static string[] processors = { "Intel Core i5", "Intel Core i7", "AMD Ryzen 5", "AMD Ryzen 7" };
        static string[] graphicsCards = { "NVIDIA GeForce GTX 1660", "NVIDIA GeForce RTX 3060", "AMD Radeon RX 5600", "AMD Radeon RX 6700" };
        static int[] ramOptions = { 8, 16, 32, 64 };
        static float[] prices = { 500, 1000, 1500, 2000 };
        static float[] weights = { 1.5f, 2.0f, 2.5f, 3.0f };
        static string[] names = { "Gaming PC", "Workstation", "Home PC", "Laptop" };
        static Random random = new Random();


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
        // стптичний метод для створення нового комп'ютера зі згенерованими даними
        public static Computer CreateRandomComputer() 
        {
            return new Computer(names[random.Next(names.Length)], prices[random.Next(prices.Length)], weights[random.Next(weights.Length)], processors[random.Next(processors.Length)], ramOptions[random.Next(ramOptions.Length)], graphicsCards[random.Next(graphicsCards.Length)]);
        }


    }

    public class Smartphone : Product
    {
        //статичні масиви для генерації випадкових даних смартфона
        static string[] operatingSystems = { "Android", "iOS", "Windows Phone" };
        static int[] storageOptions = { 32, 64, 128, 256, 512 };
        static double[] screenSizes = { 4.7, 5.5, 6.1, 6.7 };
        static float[] prices = { 300, 500, 700, 1000 };
        static float[] weights = { 0.15f, 0.2f, 0.25f, 0.3f };
        static string[] names = { "Smartphone A", "Smartphone B", "Smartphone C", "Smartphone D" };
        static Random random = new Random();

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

        public static Smartphone CreateRandomSmartphone()
        {
            return new Smartphone(names[random.Next(names.Length)], prices[random.Next(prices.Length)], weights[random.Next(weights.Length)], operatingSystems[random.Next(operatingSystems.Length)], storageOptions[random.Next(storageOptions.Length)], screenSizes[random.Next(screenSizes.Length)]);
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
