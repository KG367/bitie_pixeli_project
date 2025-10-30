using System.Diagnostics;

namespace DishService
{
    public enum DishCategory
    {
        Drinks, Salads, ColdAppetizers, HotAppetizers, Soups, HotDishes, Desserts
    };

    public enum DishField
    {
        Id, Name, Ingredients, Weight, Price, Categories, CookingTime, Tags
    }


    public class Dish
    {
        /*
        Id [int] - ид блюда.
        Name [string] - имя блюда.
        Ingredients [string] - состав блюда. Делать списком не вижу смысла, т.к если и менять, то всё сразу
        Weight [text] - вес блюда. Блюда бывают и граммовые и килограммовые. Ладно, это сова на глобус, оно такое какое оно есть
        Price [decimal] - цена блюда. С деньгами надо быть осторожными, так что Decimal
        Categories [List<string>] - Категории блюда. Есть функции по добавлению/удалению категорий. По идее в бд хранится как массив целых-индексов, в обёртке конвертируется в массив строк. (TODO: по тз тип - перечисление, уточнить)
        CookingTime [int] - время приготовления, в каких-то (TODO: Уточнить) единицах измерения.
        Tags [List<string>] - доп. теги, напр. веганское, острое. В бд хранится опять же как массив индексов.
        */
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Ingredients { get; private set; }
        public string Weight { get; private set; }
        public decimal Price { get; private set; }
        public List<string> Categories { get; private set; }
        public int CookingTime { get; private set; }
        public List<string> Tags { get; private set; }

        public Dish(int id, string name, string ingredients, string weight, decimal price, List<string> category, int cookingTime, List<string> tags)
        {
            Id = id;
            Name = name;
            Ingredients = ingredients;
            Weight = weight;
            Price = price;
            Categories = category;
            CookingTime = cookingTime;
            Tags = tags;
        }


        // Функция-заглушка, обозначающая транзакцию в бд
        private static void db() {}


        // Статический метод-фабрика
        public static Dish CreateDish(int id, string name, string composition, string weight,
                                        decimal price, List<string> categories, int cookingTime,
                                        List<string> tags)
        {
            return new Dish(id, name, composition, weight, price, categories, cookingTime, tags);
        }


        // Редактирование блюда
        public bool TryModify(string? name = null, string? ingredients = null, string? weight = null,
                                decimal? price = null, List<string>? categories = null,
                                int? cookingTime = null, List<string>? tags = null)
        {
            //Return true if succesfully modified
            // TODO желательно добавить флаги редактирования, чтобы обновлять в бд только те поля что отредактированы здесь
            if (!string.IsNullOrEmpty(name))
                Name = name;

            if (!string.IsNullOrEmpty(ingredients))
                Ingredients = ingredients;

            if (!string.IsNullOrEmpty(weight))
                Weight = weight;

            if (price.HasValue)
                Price = price.Value;

            // TODO Добавить проверку, чтобы можно было добавить только существующие категории
            if (categories != null)
                Categories = categories;

            if (cookingTime.HasValue)
                CookingTime = cookingTime.Value;

            // TODO аналогично categories
            if (tags != null)
                Tags = tags;

            db();
            return true;
        }


        public void DisplayInfo()
        {
            Console.WriteLine("=== Информация о блюде ===");
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Название: {Name}");
            Console.WriteLine($"Состав: {Ingredients}");
            Console.WriteLine($"Вес: {Weight}");
            Console.WriteLine($"Цена: {Price:F2} руб.");
            Console.WriteLine($"Категории: {string.Join(", ", Categories)}");
            Console.WriteLine($"Время готовки: {CookingTime} мин.");
            Console.WriteLine($"Типы: {string.Join(", ", Tags)}");
            Console.WriteLine("===========================");
        }


        public void DeleteDish()
        {
            Name = "Удаленное блюдо";
            Ingredients = string.Empty;
            Weight = "0/0/0";
            Price = 0;
            Categories.Clear();
            CookingTime = 0;
            Tags.Clear();

            Debug.WriteLine($"Блюдо с ID {Id} было удалено.");
        }


        public override string ToString() =>
            $"Блюдо {Id}: {Name} - {Price:F2} руб.\nСостав: {Ingredients}\nВес: {Weight}";


        public bool AddTags(params string[] newTags)
        {
            // Возвращает 0 если всё ок, иначе возвращает номер (не индекс) неверного аргумента
            foreach (string tag in newTags)
            {
                db(); // Проверка на существование в базе такого типа.
                if (!string.IsNullOrEmpty(tag) && !Tags.Contains(tag))
                {
                    Tags.Add(tag);
                }
            }
            db(); // Закрепление
            return false;
        }


        public bool RemoveTags(params string[] oldTags)
        {
            foreach (string tag in oldTags)
            {
                Tags.Remove(tag);
            }
            db();
            return false;
        }


        public bool AddCategories(params string[] newCategories)
        {
            foreach (string category in newCategories)
            {
                db();
                if (!string.IsNullOrEmpty(category) && !Categories.Contains(category))
                {
                    Categories.Add(category);
                }
            }
            db();
            return false;
        }


        public bool RemoveCategories(params string[] oldCategories)
        {
            foreach (string category in oldCategories)
            {
                Categories.Remove(category);
            }
            db();
            return false;
        }
    }


    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var tags = new List<string> { "Острое", "Халяль" };

    //         Dish dish1 = Dish.CreateDish(
    //             id: 1,
    //             name: "Салат Цезарь",
    //             composition: "Курица, салат, сухарики, соус",
    //             weight: "300/50/25",
    //             price: 450.00f,
    //             category: DishCategory.Salads,
    //             cookingTime: 15,
    //             tags: tags
    //         );

    //         dish1.DisplayInfo();

    //         dish1.TryModify(price: 480.00f, cookingTime: 12);
    //         dish1.AddType("Веганское");

    //         dish1.DisplayInfo();

    //         dish1.DeleteDish();
    //     }
    // }
}