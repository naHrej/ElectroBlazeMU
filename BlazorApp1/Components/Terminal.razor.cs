using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorApp1.Components;

public partial class Terminal
{
    public static Terminal Instance { get; private set; }
    private ElementReference _terminal;
    private string _input = string.Empty;
    private string _data = string.Empty;
    private byte[] _buffer = new byte[1024];
    TcpClient _client = new();
    private bool _isReading = false;

    public Terminal()
    {
        Instance = this;
    }

    private async Task StartReading(NetworkStream stream)
    {
        while (_client.Connected)
        {
            int bytesRead = await stream.ReadAsync(_buffer, 0, _buffer.Length);
            if (bytesRead > 0)
            {
                string data = Encoding.ASCII.GetString(_buffer, 0, bytesRead);
                await HandleData(data);
            }
        }
    }


    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            NetworkStream stream = _client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(_input + "\n");
            stream.Write(data, 0, data.Length);
            //Logger.LogInformation("Data sent: " + _input);

            _input = string.Empty;
            InvokeAsync(StateHasChanged);
        }
        else
        {
            InvokeAsync(StateHasChanged);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        //Logger.LogInformation("OnInitialized is being called.");
        try
        {
            if ((circuitHandler as CustomCircuitHandler).IsConnected && _client.Connected == false)
            {
                await _client.ConnectAsync("code.deanpool.net", 1701);
                NetworkStream stream = _client.GetStream();
                _ = StartReading(stream); // Fire and forget

                // send a blank message to the server to get the initial prompt
                byte[] data = Encoding.ASCII.GetBytes(" \n");
                stream.Write(data, 0, data.Length);

                await InvokeAsync(StateHasChanged);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred in OnInitialized.");
        }

        // Scroll to bottom of Terminal
    }

    [JSInvokable]
    public static async Task SendCommandToServer(string command)
    {
        if (Instance._client.Connected)
        {
            NetworkStream stream = Instance._client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(command + "\n");
            await stream.WriteAsync(data, 0, data.Length);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("import", "./Components/Terminal.razor.js");
        }

        await JSRuntime.InvokeVoidAsync("scrollToBottom", _terminal);
        await JSRuntime.InvokeVoidAsync("addClickEvents");
    }


    private async Task HandleData(string data)
    {
        int styleIndex = data.IndexOf("!@style:url:", StringComparison.Ordinal);
        if (styleIndex != -1)
        {
            // Find the end of the URL
            int urlEndIndex = data.IndexOf(".less", styleIndex, StringComparison.Ordinal);
            if (urlEndIndex == -1)
            {
                // If .less is not found in the data, log an error and return
                Logger.LogError("Invalid style URL: .less not found");
                //return;
            }

            urlEndIndex += ".less".Length; // Include the .less in the URL

            // Extract the URL from the data
            string url = data.Substring(styleIndex + "!@style:url:".Length,
                urlEndIndex - (styleIndex + "!@style:url:".Length));

            // Sanitize the URL
            Uri uri = new Uri(url);
            url = uri.AbsoluteUri;

            //Logger.LogInformation("URL: " + url);

            // Download the LESS file
            HttpClient httpClient = new HttpClient();
            string less = await httpClient.GetStringAsync(url);

            // Compile the LESS file into CSS
            dotless.Core.ILessEngine lessEngine = new dotless.Core.LessEngine();
            string css = lessEngine.TransformToCss(less, null);

            // Inject the CSS into the document
            await JSRuntime.InvokeVoidAsync("addStyles", css);
        }


        // remove any line returns or new lines from the data
        data = data.Replace("\n", "<br/>").Replace("\r", "");


        _data += data;
        //Logger.LogInformation("Data received: " + data);


        await InvokeAsync(StateHasChanged);
    }
}