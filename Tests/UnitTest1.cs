using Xunit;
using Server;  // Твое пространство имен!

namespace Tests
{
    public class BookingServiceTests
    {
        [Fact]
        public void AddNewTable_ShouldAddTable()
        {
            // Arrange
            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            
            // Act
            bool result = service.AddNewTable(1);
            
            // Assert
            Assert.True(result);
            Assert.Single(service.bookings); // Проверяем что одна таблица
            Assert.True(service.bookings.ContainsKey(1));
        }

        [Fact]
        public void AddNewTable_ExistingTable_ShouldReturnFalse()
        {
            // Arrange
            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);
            
            // Act
            bool result = service.AddNewTable(1); // Пытаемся добавить ту же таблицу
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void BookTable_FirstBooking_ShouldSuccess()
        {
            // Arrange
            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);
            
            var start = DateTime.Now;
            var end = start.AddHours(2);
            
            // Act
            bool result = service.BookTable(1, start, end, 1, "Иван", "+79991112233", "Тестовое бронирование");
            
            // Assert
            Assert.True(result);
            Assert.Single(service.bookings[1]); // Одно бронирование в таблице
        }

        [Fact]
        public void BookTable_OverlappingBooking_ShouldFail()
        {
            // Arrange
            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);
            
            var start1 = DateTime.Now;
            var end1 = start1.AddHours(2);
            service.BookTable(1, start1, end1, 1, "Иван", "+79991112233", "Первое бронирование");
            
            var start2 = start1.AddHours(1); // Пересекается!
            var end2 = start2.AddHours(2);
            
            // Act
            bool result = service.BookTable(1, start2, end2, 2, "Петр", "+79994445566", "Второе бронирование");
            
            // Assert
            Assert.False(result); // Должно вернуть false при пересечении
        }

        [Fact]
        public void GetEmptyBookPosition_EmptyList_ShouldReturnZero()
        {
            // Arrange
            var service = new BookingService();
            var bookings = new List<Booking>();
            var start = DateTime.Now;
            var end = start.AddHours(1);
            
            // Act
            bool result = service.GetEmptyBookPosition(in bookings, in start, in end, out int index);
            
            // Assert
            Assert.True(result);
            Assert.Equal(0, index);
        }

        [Fact]
        public void CancelBooking_ShouldRemoveBooking()
        {
            // Arrange
            var service = new BookingService();
            service.bookings = new Dictionary<int, List<Booking>>();
            service.AddNewTable(1);
            
            var start = DateTime.Now;
            var end = start.AddHours(2);
            var booking = new Booking(1, "Иван", "+79991112233", start, end, "Тест", 1);
            service.BookTable(booking);
            
            // Act
            bool result = service.CancelBooking(booking);
            
            // Assert
            Assert.True(result);
            Assert.Empty(service.bookings[1]); // Бронирование удалено
        }

        [Fact]
        public void Booking_TryModify_ShouldChangeFields()
        {
            // Arrange
            var booking = new Booking(1, "Иван", "+79991112233", DateTime.Now, DateTime.Now.AddHours(2), "Коммент", 1);
            
            // Act
            bool result = booking.TryModify(
                (BookingField.ClientName, "Петр"),
                (BookingField.PhoneNumber, "+79994445566")
            );
            
            // Assert
            Assert.True(result);
            Assert.Equal("Петр", booking.ClientName);
            Assert.Equal("+79994445566", booking.PhoneNumber);
        }

        [Fact]
        public void Booking_TryModifyInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var booking = new Booking(1, "Иван", "+79991112233", DateTime.Now, DateTime.Now.AddHours(2), "Коммент", 1);
            
            // Act
            bool result = booking.TryModify(
                (BookingField.ClientId, "-5") // Невалидный ID
            );
            
            // Assert
            Assert.False(result);
        }
    }
}