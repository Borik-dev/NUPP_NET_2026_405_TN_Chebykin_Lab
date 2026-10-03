using ElectronicsOnlineStore.ElectronicsOnlineStore.Common;
using System;
using System.Text;



namespace ElectronicsOnlineStore.ElectronicsOnlineStore.ConsoleApp
{
    class Class1
    {

        static void Main(string[] args)
        {

            System.Console.OutputEncoding = Encoding.UTF8;
            System.Console.InputEncoding = Encoding.UTF8;
            
            // CREATE CRUD SERVICES
          
            MyProductList<Computer> computerService =
                new MyProductList<Computer>();

            MyProductList<Smartphone> smartphoneService =
                new MyProductList<Smartphone>();
         
            // CREATE - ДОДАВАННЯ КОМП'ЮТЕРІВ           
            Computer computer1 = new Computer
            {
                Name = "Lenovo Legion",
                Price = 50000,
                Weight = 2.5f,
                Processor = "Intel Core i7",
                RAM = 32,
                GraphicsCard = "RTX 4070"
            };

            Computer computer2 = new Computer
            {
                Name = "ASUS ROG",
                Price = 65000,
                Weight = 2.3f,
                Processor = "AMD Ryzen 7",
                RAM = 32,
                GraphicsCard = "RTX 4080"
            };

            computerService.Create(computer1);
            computerService.Create(computer2);

            // READ ALL - ВСІ КОМП'ЮТЕРИ
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("КОМП'ЮТЕРИ");
            System.Console.WriteLine("==================================");
            foreach (Computer computer in computerService.ReadAll())
            {
                computer.DisplayInfo();
                System.Console.WriteLine();
            }

            // READ - ПОШУК КОМП'ЮТЕРА
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("ПОШУК КОМП'ЮТЕРА");
            System.Console.WriteLine("==================================");
            Computer foundComputer = computerService.Read(computer1.Id);
            if (foundComputer != null)
            {
                System.Console.WriteLine($"Знайдено: {foundComputer.Name}, {foundComputer.Price:C}");
            }

            // UPDATE
            System.Console.WriteLine();
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("UPDATE");
            System.Console.WriteLine("==================================");
            computer1.Price = 45000;
            computerService.Update(computer1);
            System.Console.WriteLine($"Нова ціна {computer1.Name}: {computer1.Price:C}");

            // DELETE            
            System.Console.WriteLine();
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("DELETE");
            System.Console.WriteLine("==================================");
            computerService.Remove(computer2);
            System.Console.WriteLine("Після видалення:");
            foreach (Computer computer in computerService.ReadAll())
            {
                System.Console.WriteLine($"{computer.Name} | {computer.Price:C}");
            }
            
            // SMARTPHONES            
            System.Console.WriteLine();
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("СМАРТФОНИ");
            System.Console.WriteLine("==================================");

            Smartphone smartphone1 = new Smartphone
            {
                Name = "Samsung Galaxy S25",
                Price = 35000,
                Weight = 0.2f,
                OperatingSystem = "Android",
                Storage = 256,
                ScreenSize = 6.7
            };

            Smartphone smartphone2 = new Smartphone
            {
                Name = "iPhone 17",
                Price = 45000,
                Weight = 0.18f,
                OperatingSystem = "iOS",
                Storage = 512,
                ScreenSize = 6.3
            };
            smartphoneService.Create(smartphone1);
            smartphoneService.Create(smartphone2);

            // READ ALL
            foreach (Smartphone smartphone in smartphoneService.ReadAll())
            {
                smartphone.DisplayInfo();
                System.Console.WriteLine();
            }

            // READ
            System.Console.WriteLine("Пошук смартфона:");
            Smartphone foundSmartphone = smartphoneService.Read(smartphone1.Id);
            if (foundSmartphone != null)
            {
                System.Console.WriteLine($"Знайдено: {foundSmartphone.Name}");
            }

            // UPDATE
            smartphone1.Price = 32000;
            smartphoneService.Update(smartphone1);
            System.Console.WriteLine($"Нова ціна {smartphone1.Name}: {smartphone1.Price:C}");

            // DELETE
            smartphoneService.Remove(smartphone2);
            System.Console.WriteLine("Після видалення:");
            foreach (Smartphone smartphone in smartphoneService.ReadAll())
            {
                System.Console.WriteLine($"{smartphone.Name} | {smartphone.Price:C}");
            }

            // EXTENSION METHOD
            System.Console.WriteLine();
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("EXTENSION METHOD");
            System.Console.WriteLine("==================================");
            smartphone1.GetPriceWithDiscount(10);
            
            // STATIC PRODUCT MEMBER            
            System.Console.WriteLine();
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("STATIC PRODUCT MEMBER");
            System.Console.WriteLine("==================================");
            System.Console.WriteLine($"Кількість створених продуктів: {Product.GetProductCount()}");
            
            // CUSTOMER           
            System.Console.WriteLine();
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("CUSTOMER");
            System.Console.WriteLine("==================================");
            Customer customer = new Customer
            (
                "Богдан",
                "bohdan@gmail.com"
            );
            customer.DisplayInfo();
            
            // PURCHASE + EVENT           
            System.Console.WriteLine();
            System.Console.WriteLine("==================================");
            System.Console.WriteLine("ПОКУПКА ПРОДУКТУ");
            System.Console.WriteLine("==================================");
            customer.BuyProduct(computer1);
            System.Console.WriteLine();            
        }

    }
}
