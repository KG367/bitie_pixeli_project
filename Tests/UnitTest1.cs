using Xunit;
using BookingSystem;
using Service;
using DishService;
// i need CollectionAssert

namespace Tests
{


    public class DishServiceTests
    {
        [Fact]
        public void DishCreateCheck()
        {
            var dish = Dish.CreateDish(
                id: 1,
                name: "Салат Цезарь",
                composition: "Курица, салат, сухарики, соус",
                weight: "300/50/25",
                price: 450.0m,
                categories: new List<string> { "Салаты" },
                cookingTime: 15,
                tags: new List<string> { "Острое", "Халяль" }
            );
            dish.DeleteDish();
        }

        [Fact]
        public void DishModifyCheck()
        {
            var dish = Dish.CreateDish(
                id: 1,
                name: "Салат Цезарь",
                composition: "Курица, салат, сухарики, соус",
                weight: "300/50/25",
                price: 450.0m,
                categories: new List<string> { "Салаты" },
                cookingTime: 15,
                tags: new List<string> { "Острое", "Халяль" }
            );

            dish.TryModify(name: "Курица", categories: new List<string> { "Жаркое" });
            Assert.Equal("Курица", dish.Name);
            Assert.True(dish.Categories.Equals(new List<string> { "Жаркое" }));
            Assert.Equal(450.0m, dish.Price);
        }

        [Fact]
        public void DishAddRemoveCheck()
        {
            var dish = Dish.CreateDish(
                id: 1,
                name: "Салат Цезарь",
                composition: "Курица, салат, сухарики, соус",
                weight: "300/50/25",
                price: 450.0m,
                categories: new List<string> { "Салаты" },
                cookingTime: 15,
                tags: new List<string> { "Острое", "Халяль" }
            );

            dish.AddTags("Вкусное");
            Assert.Contains("Вкусное", dish.Tags);
            dish.RemoveTags("Вкусное");
            Assert.DoesNotContain("Вкусное", dish.Tags);

            dish.AddCategories("Вкусное");
            Assert.Contains("Вкусное", dish.Categories);
            dish.RemoveCategories("Вкусное");
            Assert.DoesNotContain("Вкусное", dish.Categories);
        }
    }
}