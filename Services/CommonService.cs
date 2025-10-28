// здесь будут лежать общие методы, их можно вызвать ВЕЗДЕ, как обычный файл .js
using Microsoft.JSInterop;
using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;


namespace Service // пока что тупо прослойка для JS, мб потом что-то более нужное добавим
{
    public class JSService
    {

        public static string loudMessage = "УРА";
        private readonly IJSRuntime _js;

        public JSService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task AlertJS(string message)
            => await _js.InvokeVoidAsync("alert", message);

        public async Task LogJS(string message)
            => await _js.InvokeVoidAsync("console.log", message);

        public async Task CallJSFunc(string functionName, params object[] args)
            => await _js.InvokeVoidAsync(functionName, args);
    }

    public class PostgresService : IDisposable
    {
        private readonly NpgsqlConnection _connection;

        public PostgresService()
        {
            string connectionString = @"
            Host=84.19.3.114;
            Port=5432;
            Database=pks_project;
            Username=project;
            Password=C#_forever!;
            Pooling=true;                           
            Minimum Pool Size=0;                    
            Maximum Pool Size=20;                   
            Connection Idle Lifetime=300;           
            Timeout=30;                             
            Command Timeout=300;                    
            Encoding=UTF8;                          
            Search Path=public;";

            try
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к БД: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }

    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ClientName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Comment { get; set; }
        public int AssignedTableId { get; set; }
    }
}