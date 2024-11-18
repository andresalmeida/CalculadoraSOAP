using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Iniciando la aplicación de calculadora...");
        Console.WriteLine("Consumiendo el servicio SOAP de la calculadora...");

        await CallSoapService("Sumar", 10, 5);        
        await CallSoapService("Restar", 10, 5);      
        await CallSoapService("Multiplicar", 10, 5);  
        await CallSoapService("Dividir", 10, 5);  


        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();
    }

    static async Task CallSoapService(string operacion, int intA, int intB)
    {
        // Mapeo de nombres de operación
        string operacionSoap = GetSoapOperationName(operacion);

        string soapEnvelope = $@"
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
              <soap:Body>
                <{operacionSoap} xmlns=""http://tempuri.org/"">
                  <intA>{intA}</intA>
                  <intB>{intB}</intB>
                </{operacionSoap}>
              </soap:Body>
            </soap:Envelope>";

        HttpClient httpClient = new HttpClient();
        try
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://www.dneonline.com/calculator.asmx"),
                Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
            };

            httpRequest.Headers.Add("SOAPAction", $"http://tempuri.org/{operacionSoap}");

            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Resultado de {operacion}: {ExtractResult(responseBody, operacionSoap)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al realizar la operación {operacion}: {ex.Message}");
        }
        finally
        {
            httpClient.Dispose(); // Liberar recursos manualmente
        }
    }

    static string GetSoapOperationName(string operacion)
    {
        if (operacion == "Sumar") return "Add";
        if (operacion == "Restar") return "Subtract";
        if (operacion == "Multiplicar") return "Multiply";
        if (operacion == "Dividir") return "Divide";
        throw new ArgumentException("Operación no válida");
    }

    static string ExtractResult(string responseXml, string operacionSoap)
    {
        var startTag = $"<{operacionSoap}Result>";
        var endTag = $"</{operacionSoap}Result>";
        var startIndex = responseXml.IndexOf(startTag) + startTag.Length;
        var endIndex = responseXml.IndexOf(endTag);
        return responseXml.Substring(startIndex, endIndex - startIndex);
    }
}
