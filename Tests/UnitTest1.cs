using Xunit;
using BookingSysem;
using Service;
using DishService;
// i need CollectionAssert

namespace Tests
{
    public class BookingServiceTests
    {
        [Fact]
        public void AddNewTable_ShouldAddTable()
        {

            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();


            bool result = service.AddNewTable(1);


            Assert.True(result);
            Assert.Single(service.bookings); // Проверяем что одна таблица
            Assert.True(service.bookings.ContainsKey(1));
        }

        [Fact]
        public void AddNewTable_ExistingTable_ShouldReturnFalse()
        {

            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);


            bool result = service.AddNewTable(1); // Пытаемся добавить ту же таблицу


            Assert.False(result);
        }

        [Fact]
        public void BookTable_FirstBooking_ShouldSuccess()
        {

            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);

            var start = DateTime.Now;
            var end = start.AddHours(2);


            bool result = service.BookTable(1, start, end, 1, "Иван", "+79991112233", "Тестовое бронирование");


            Assert.True(result);
            Assert.Single(service.bookings[1]); // Одно бронирование в таблице
        }

        [Fact]
        public void BookTable_OverlappingBooking_ShouldFail()
        {

            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);

            var start1 = DateTime.Now;
            var end1 = start1.AddHours(2);
            service.BookTable(1, start1, end1, 1, "Иван", "+79991112233", "Первое бронирование");

            var start2 = start1.AddHours(1); // Пересекается!
            var end2 = start2.AddHours(2);


            bool result = service.BookTable(1, start2, end2, 2, "Петр", "+79994445566", "Второе бронирование");


            Assert.False(result); // Должно вернуть false при пересечении
        }

        [Fact]
        public void GetEmptyBookPosition_EmptyList_ShouldReturnZero()
        {

            var service = new BookingService();
            var bookings = new List<Booking>();
            var start = DateTime.Now;
            var end = start.AddHours(1);


            bool result = service.GetEmptyBookPosition(in bookings, in start, in end, out int index);


            Assert.True(result);
            Assert.Equal(0, index);
        }

        [Fact]
        public void CancelBooking_ShouldRemoveBooking()
        {

            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);

            var start = DateTime.Now;
            var end = start.AddHours(2);
            var booking = new Booking(1, "Иван", "+79991112233", start, end, "Тест", 1);
            service.BookTable(booking);


            bool result = service.CancelBooking(booking);


            Assert.True(result);
            Assert.Empty(service.bookings[1]); // Бронирование удалено
        }

        [Fact]
        public void Booking_TryModify_ShouldChangeFields()
        {

            var booking = new Booking(1, "Иван", "+79991112233", DateTime.Now, DateTime.Now.AddHours(2), "Коммент", 1);


            bool result = booking.TryModify(
                (BookingField.ClientName, "Петр"),
                (BookingField.PhoneNumber, "+79994445566")
            );


            Assert.True(result);
            Assert.Equal("Петр", booking.ClientName);
            Assert.Equal("+79994445566", booking.PhoneNumber);
        }

        [Fact]
        public void Booking_TryModifyInvalidId_ShouldReturnFalse()
        {

            var booking = new Booking(1, "Иван", "+79991112233", DateTime.Now, DateTime.Now.AddHours(2), "Коммент", 1);


            bool result = booking.TryModify(
                (BookingField.UserId, "-5") // Невалидный ID
            );


            Assert.False(result);
        }

        [Fact]
        public void DB_TryConnect()
        {
            var serv = new PostgresService(); //либо упадет, либо удачно подключится
        }

        [Fact]
        public void DB_TestParcer()
        {
            string command = "t353 @t fvbnjq @re";
            var parced = Parcer.ParceArgs(command);

            var ex = new List<string> { "@t", "@re" };
            Assert.Equal(ex, parced);
        }
    }


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