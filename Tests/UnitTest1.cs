using Xunit;
using BookingSystem;
using Service;

namespace Tests
{
    public class BookingServiceTests
    {
        
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
}