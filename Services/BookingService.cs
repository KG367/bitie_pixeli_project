using System.ComponentModel;
using System.Numerics;
using Service;
using Npgsql;

namespace BookingSystem
{
    public class BookingService
    {

        private NpgsqlConnection connection;

        public bool TableIsAvalible(int tableId, in DateTime startTime, in DateTime endTime)
        {
            try
            {
                var sql = new NpgsqlCommand("SELECT NOT EXISTS ( SELECT 1 FROM bookings WHERE @table_id = ANY(assigned_table_id) AND NOT (end_time <= @start_time OR start_time >= @end_time))", this.connection);
                sql.Parameters.AddWithValue("@table_id", tableId);
                sql.Parameters.AddWithValue("@start_time", startTime);
                sql.Parameters.AddWithValue("@end_time", endTime);

                return (bool)sql.ExecuteScalar();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                throw;
            }
        }
        
        
        public bool TryBook(in Book newBook, out int? index) // TODO: Оптимизировать это через SQL...
        {

            foreach (var tableId in newBook.AssignedTableId)
            {
                if (!this.TableIsAvalible(tableId, newBook.StartTime, newBook.EndTime))
                {
                    index = null;
                    return false;
                }
            }
            
            var sql = new NpgsqlCommand(@"INSERT INTO bookings (user_id, client_name, phone_number, start_time, end_time, comment, assigned_table_id) VALUES (@user_id, @client_name, @phone_number, @start_time, @end_time, @comment, @assigned_table_id) RETURNING id", this.connection);

            sql.Parameters.AddWithValue("@user_id", newBook.UserId);
            sql.Parameters.AddWithValue("@client_name", newBook.ClientName);
            sql.Parameters.AddWithValue("@phone_number", newBook.PhoneNumber);
            sql.Parameters.AddWithValue("@start_time", newBook.StartTime);
            sql.Parameters.AddWithValue("@end_time", newBook.EndTime);
            sql.Parameters.AddWithValue("@comment", newBook.Comment);
            sql.Parameters.AddWithValue("@assigned_table_id", newBook.AssignedTableId.ToArray());

            index = (int)sql.ExecuteScalar();
            return true;

        }
    }
}