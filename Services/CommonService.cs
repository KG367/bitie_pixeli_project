// здесь будут лежать общие методы, их можно вызвать ВЕЗДЕ, как обычный файл .js
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;


namespace Service //базовая прослойка JS плюс БД
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


        public Book() 
        {
            ClientName = string.Empty;
            PhoneNumber = string.Empty;
            Comment = string.Empty;
            AssignedTableId = new List<int>();
        }

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
    
    public class TokenResponse
    {
        public string Access_Token { get; set; } = string.Empty;
        public string Token_Type { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

    public class SqlResponse
    {
        public List<Dictionary<string, object>> Data { get; set; } = new();
        public int Row_Count { get; set; }
    }


    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        // Конструктор — задаём IP или базовый адрес API
        public ApiClient(HttpClient http)
        {
            _http = http;
            _baseUrl = "http://84.19.3.114:1624";
            ; 
        }
        
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            // Собираем полный адрес запроса
            var url = _baseUrl + endpoint;

            // Отправляем GET-запрос
            var response = await _http.GetAsync(url);

            // Если не 200 OK — выбрасываем исключение
            response.EnsureSuccessStatusCode();

            // Читаем JSON и превращаем в объект T
            return await response.Content.ReadFromJsonAsync<T>();
        }
        
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body)
        {
            var url = _baseUrl + endpoint;

            // Преобразуем объект в JSON-строку
            var json = JsonSerializer.Serialize(body);

            // Упаковываем в StringContent с заголовком application/json
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            // Читаем ответ (JSON → объект)
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

       
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest body)
        {
            var url = _baseUrl + endpoint;

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        
        public async Task<bool> DeleteAsync(string endpoint)
        {
            var url = _baseUrl + endpoint;

            var response = await _http.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }
        
        public async Task<TokenResponse?> LoginAsync(string username, string password)
        {
            var body = new { username, password };
            return await PostAsync<object, TokenResponse>("/login", body);
        }

        public async Task<SqlResponse?> ExecuteSqlAsync(string token, string sql)
        {
            var body = new { token, sql };
            return await PostAsync<object, SqlResponse>("/exec_sql_dict", body);
        }

    }
}