using System.Diagnostics;

namespace Dish
{
    public enum DishCategory
    {
        Drinks, Salads,
        ColdAppetizers, HotAppetizers, Soups, HotDishes, Desserts
    };


    public class Dish
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Ingredients { get; private set; }
        public string Weight { get; private set; }
        public float Price { get; private set; }
        public DishCategory Category { get; private set; }
        public int CookingTime { get; private set; }
        public List<string> Tags { get; private set; }

        public Dish(int id, string name, string ingredients, string weight, float price, DishCategory category, int cookingTime, List<string> tags)
        {
            Id = id;
            Name = name;
            Ingredients = ingredients;
            Weight = weight;
            Price = price;
            Category = category;
            CookingTime = cookingTime;
            Tags = tags;
        }


        // Статический метод-фабрика
        public static Dish CreateDish(int id, string name, string composition, string weight,
                                        float price, DishCategory category, int cookingTime,
                                        List<string> tags)
        {
            return new Dish(id, name, composition, weight, price, category, cookingTime, tags);
        }


        // Редактирование блюда
        public void EditDish(string name = null, string ingredients = null, string weight = null,
                                float? price = null, DishCategory? category = null,
                                int? cookingTime = null, List<string> tags = null)
        {
            if (!string.IsNullOrEmpty(name))
                Name = name;

            if (!string.IsNullOrEmpty(ingredients))
                Ingredients = ingredients;

            if (!string.IsNullOrEmpty(weight))
                Weight = weight;

            if (price.HasValue)
                Price = price.Value;

            if (category.HasValue)
                Category = category.Value;

            if (cookingTime.HasValue)
                CookingTime = cookingTime.Value;

            if (tags != null)
                Tags = tags;
        }


        public void DisplayInfo()
        {
            Console.WriteLine("=== Информация о блюде ===");
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Название: {Name}");
            Console.WriteLine($"Состав: {Ingredients}");
            Console.WriteLine($"Вес: {Weight}");
            Console.WriteLine($"Цена: {Price:F2} руб.");
            Console.WriteLine($"Категория: {Category}");
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
            Category = DishCategory.HotAppetizers;
            CookingTime = 0;
            Tags.Clear();

            Debug.WriteLine($"Блюдо с ID {Id} было удалено.");
        }


        public override string ToString() =>
            $"Блюдо {Id}: {Name} - {Price:F2} руб. ({Category})\nСостав: {Ingredients}\nВес: {Weight}";

        public void AddType(string type)
        {
            if (!string.IsNullOrEmpty(type) && !Tags.Contains(type))
            {
                Tags.Add(type);
            }
        }

        public void RemoveType(string type)
        {
            Tags.Remove(type);
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

    //         dish1.EditDish(price: 480.00f, cookingTime: 12);
    //         dish1.AddType("Веганское");

    //         dish1.DisplayInfo();

    //         dish1.DeleteDish();
    //     }
    // }
}