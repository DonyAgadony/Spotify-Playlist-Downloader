using System.Net.Http;

class Program
{
    static void Main()
    {
        MakeARequest();

        Console.ReadKey();
    }

    static async void MakeARequest()
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        request.Headers.Accept.Add("Content-Type", "application/x-www-form-urlencoded");
        request.Content = new StringContent(
            "grant_type=client_credentials" +
            "&client_id=e6e85ffc945b460191628ba52ccdf388" +
            "&client_secret=0c907d7a782b4ccdbfebb44fbaaed8db"
        );

        var response = await client.SendAsync(request);

        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}