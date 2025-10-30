// здесь будут лежать общие методы, их можно вызвать ВЕЗДЕ, как обычный файл .js
using Microsoft.JSInterop;
using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;
using System.Data;


namespace Service //базовая прослойка JS плюс БД
{

    public interface CheckingConnection //интерфейс для проверки открытости подключения к БД
    {
        void Check();
    }

    public static class ConnectionExtension
    {
        public static void Check(this NpgsqlConnection connection) //реализация интерфейса для Npgsql соединения
        {
            if (connection.State != ConnectionState.Open) //перестраховка, что канал жив, в противном случае "перезагружаем"
            {
                    connection.Close();
                    connection.Open();
            }   
        }
    }


    public class Parcer
    {
        public static List<string> ParceArgs(string str) //парсит аргументы (необходим в силу специфики Npgsql) для быстрых SQL-запросов. Перед аргументами в запросе должен быть @!
        {
            List<string> args = new List<string>();
            string buff = "";
            bool isArg = false;

            foreach (char c in str)
            {
                if (isArg)
                {
                    if (c == ' ')
                    {
                        args.Add(buff);
                        buff = "@";
                        isArg = false;
                    }
                    else
                    {
                        buff += c;
                    }
                }
                else
                {
                    if (c == '@')
                    {
                        isArg = true;
                        buff = "@";
                    }
                }
            }

            if (buff != "@" && buff != "")
            {
                args.Add(buff);
            }

            return args;
        }
    }
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




    public enum BookingField
    {
        UserId,
        ClientName,
        PhoneNumber,
        Comment,
    }
    public class Book
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ClientName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Comment { get; set; }
        public List<int> AssignedTableId { get; set; }


        public Book(int userId, string clientName, string phoneNumber, DateTime startTime, DateTime endTime, string comment, in List<int> assignedTableId)
        {
            UserId = userId;
            ClientName = clientName;
            PhoneNumber = phoneNumber;
            StartTime = startTime;
            EndTime = endTime;
            Comment = comment;
            AssignedTableId = assignedTableId;
        }

        public override string ToString()
        {
            return $"Клиент: {this.UserId}\nИмя: {this.ClientName}\nТелефон: {this.PhoneNumber}\nНачало: {this.StartTime}\nКонец: {this.EndTime}\nКомментарий: {this.Comment}\n";
        }

        public bool TryModify(params (BookingField, string)[] fields)
        {
            foreach ((var field, var value) in fields)
            {
                switch (field)
                {
                    case BookingField.UserId:
                        int clearId = 0;
                        if (int.TryParse(value, out clearId) && clearId >= 0)
                        {
                            this.UserId = clearId;
                            break;
                        }
                        return false;

                    case BookingField.ClientName:
                        this.ClientName = value;
                        break;

                    case BookingField.PhoneNumber:
                        this.PhoneNumber = value;
                        break;

                    case BookingField.Comment:
                        this.Comment = value;
                        break;

                }

            }
            return true;
        }
    }


    public class Tables
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public int Seats { get; set; }
    }

    public class Orders
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public List<int> Dishes { get; set; }
        public string Comment { get; set; }
        public DateTime OrderTime { get; set; }
        public int WaiterId { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Tags
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PostgresService : IDisposable
    {
        private static NpgsqlConnection _connection;

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

        public static List<List<object>> ExecSQL(string command, params object[] args) //это функция написана по большому счету для быстрых тестов KG367. В продакшне не использовать, т.к. objectы череваты ошибками в рантайме и множестввенными оверхедами
        {
            List<List<object>> result = new List<List<object>>();

            try
            {
                using var projectCommand = new NpgsqlCommand(command, _connection);

                if (args.Length != 0)
                {
                    List<string> parcedArgs = Parcer.ParceArgs(command); 

                    if (parcedArgs.Count != args.Length)
                    {
                        throw new Exception($"Количество аргументов не соответствует. Ожидалось: {parcedArgs.Count}, получено: {args.Length}");
                    }

                    for (int ind = 0; ind < parcedArgs.Count; ind++)
                    {
                        projectCommand.Parameters.AddWithValue(parcedArgs[ind], args[ind] ?? DBNull.Value);
                    }
                }

                using var reader = projectCommand.ExecuteReader();

                // Читаем результат
                while (reader.Read())
                {
                    var row = new List<object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.IsDBNull(i) ? null : reader.GetValue(i));
                    }
                    result.Add(row);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"Ошибка выполнения запроса: {err.Message}");
                Console.WriteLine($"Запрос: {command}");
                throw;
            }

            return result;
        }

        public static int ExecuteSQLNonQuery(string command, params object[] args) //Для SQL, не предполагающего возврат чего-то
        {
            try
            {
                using var cmd = new NpgsqlCommand(command, _connection);

                if (args.Length != 0)
                {
                    List<string> parcedArgs = Parcer.ParceArgs(command);

                    if (parcedArgs.Count != args.Length)
                    {
                        throw new Exception($"Количество аргументов не соответствует. Ожидалось: {parcedArgs.Count}, получено: {args.Length}");
                    }

                    for (int ind = 0; ind < parcedArgs.Count; ind++)
                    {
                        cmd.Parameters.AddWithValue(parcedArgs[ind], args[ind] ?? DBNull.Value);
                    }
                }

                return cmd.ExecuteNonQuery();
            }
            catch (Exception err)
            {
                Console.WriteLine($"Ошибка выполнения команды: {err.Message}");
                throw;
            }
        }

        public static NpgsqlConnection GetConnection()
        {

            _connection.Check();
            return _connection;
        }      

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}