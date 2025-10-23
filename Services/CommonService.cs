// здесь будут лежать общие методы, их можно вызвать ВЕЗДЕ, как обычный файл .js
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Service // пока что тупо прослойка для JS, мб потом что-то более нужное добавим
{
    public class CommonService
    {

        public string loudMessage = "УРА";
        private readonly IJSRuntime _js;

        public CommonService(IJSRuntime js)
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
}