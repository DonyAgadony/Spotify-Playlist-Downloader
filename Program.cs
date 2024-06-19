using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

class Program
{
    static async Task Main()
    {
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = "AIzaSyAKzeo5aqxu0c9OxZ7qSN-hYQzaZ31s41Y",
            ApplicationName = "Playtify"
        });

        var searchListRequest = youtubeService.Search.List("snippet");
        searchListRequest.Q = "Crazy Frog";
        searchListRequest.MaxResults = 50;

        var searchListResponse = await searchListRequest.ExecuteAsync();

        foreach (var searchResult in searchListResponse.Items)
        {

            switch (searchResult.Id.Kind)
            {
                case "youtube#video":
                    Console.WriteLine(searchResult.Snippet.Title + " | " + searchResult.Id.VideoId);
                    break;

                case "youtube#channel":
                    Console.WriteLine(searchResult.Snippet.Title + " | " + searchResult.Id.ChannelId);
                    break;

                case "youtube#playlist":
                    Console.WriteLine(searchResult.Snippet.Title + " | " + searchResult.Id.PlaylistId);
                    break;
            }
        }

        // string link = GetId("https://open.spotify.com/track/7lwBfnN8IVlP6WW702pGH0?si=a9282604306b415e");
        // MakeASpotifyRequest().Wait();
    }

    static async Task MakeASpotifyRequest()
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
        var spotifyClient_id = File.ReadLines("secrets.txt").First();
        var spotifyClient_secret = File.ReadLines("secrets.txt").Skip(1).First();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var Data = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", spotifyClient_id },
            { "client_secret", spotifyClient_secret }
        };
        request.Content = new FormUrlEncodedContent(Data);

        var response = await client.SendAsync(request);
        string path = @".\index.html";
        File.WriteAllText(path, await response.Content.ReadAsStringAsync());
        var responseJson = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponseSpotify>(responseJson)!;


        // Use the access token to make authenticated requests
#pragma warning disable CS8604 // Possible null reference argument.
        await GetTrackInfo(tokenResponse.AccessToken, GetId("https://" + "open.spotify.com" + "/track/0LgOdrf5gtT4Q4lAgYABlC?si=19267a58568b4d12"));
#pragma warning restore CS8604 // Possible null reference argument.

        await GetPlaylistInfo(tokenResponse.AccessToken, GetId("https://open.spotify.com/playlist/6MGrEDCs915admcjWJJ6hf?si=538a4cd97ab94933"));
    }

    // Separates the id from the link of the song
    public static string GetId(string url)
    {
        string id = url.Split('/').Last();
        id = id.Substring(0, id.IndexOf('?'));
        return id;
    }

    static async Task GetTrackInfo(string accessToken, string id)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetAsync($"https://api.spotify.com/v1/tracks/{id}");

        var responseJson = await response.Content.ReadAsStringAsync();
        var track = JsonSerializer.Deserialize<Track>(responseJson);

        File.WriteAllText(@".\TrackInfo.txt", responseJson);


#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Console.WriteLine($"Track Name: {track.name}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    static async Task GetPlaylistInfo(string accessToken, string id)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetAsync($"https://api.spotify.com/v1/playlists/{id}");

        var responseJson = await response.Content.ReadAsStringAsync();
        var playlist = JsonSerializer.Deserialize<Playlist>(responseJson);

        File.WriteAllText(@".\PlaylistInfo.txt", responseJson);


#pragma warning disable CS8602 // Dereference of a possibly null reference.
        if (playlist.name != null)
        {
            Console.WriteLine($"Playlist Name: {playlist.name}");
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Console.WriteLine($"Playlist First Track: {playlist.tracks.items[0].track.name}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    // public static async Task MakeAYoutubeRequest()
    // {
    //     var client = new HttpClient();
    //     var APIKey = File.ReadLines(@".\secrets.txt").Skip(2).First();
    //     var request = new HttpRequestMessage(HttpMethod.Get, $"https://accounts.google.com/o/oauth2/v2/auth");
    // }
    // public static async Task SearchAVideoYoutube(string accessToken)
    // {
    //     var client = new HttpClient();
    //     var APIKey = File.ReadLines(@".\secrets.txt").Skip(2).First();
    //     var request = new HttpRequestMessage(HttpMethod.Post, $"https://youtube.googleapis.com/youtube/v3/search?part=snippet&maxResults=25&q=surfing&key={APIKey}");

    // }

}


public class TokenResponseSpotify
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
public class Playlist
{
    public bool collaborative { get; set; }
    public string? description { get; set; }
    public ExternalUrls? external_urls { get; set; }
    public Followers? followers { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public List<Image>? images { get; set; }
    public string? name { get; set; }
    public Owner? owner { get; set; }
    public bool @public { get; set; }
    public string? snapshot_id { get; set; }
    public Tracks? tracks { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

public class ExternalUrls
{
    public string? spotify { get; set; }
}

public class Followers
{
    public object? href { get; set; }
    public int total { get; set; }
}

public class Image
{
    public object? height { get; set; }
    public string? url { get; set; }
    public object? width { get; set; }
}

public class ExternalUrls2
{
    public string? spotify { get; set; }
}

public class Owner
{
    public ExternalUrls2? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

public class ExternalUrls3
{
    public string? spotify { get; set; }
}

public class AddedBy
{
    public ExternalUrls3? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

public class ExternalUrls4
{
    public string? spotify { get; set; }
}

public class Image2
{
    public int height { get; set; }
    public string? url { get; set; }
    public int width { get; set; }
}

public class Album
{
    public string? album_type { get; set; }
    public List<object>? available_markets { get; set; }
    public ExternalUrls4? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public List<Image2>? images { get; set; }
    public string? name { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

public class ExternalUrls5
{
    public string? spotify { get; set; }
}

public class Artist
{
    public ExternalUrls5? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public string? name { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

public class ExternalIds
{
    public string? isrc { get; set; }
}

public class ExternalUrls6
{
    public string? spotify { get; set; }
}

public class Track
{
    public Album? album { get; set; }
    public List<Artist>? artists { get; set; }
    public List<object>? available_markets { get; set; }
    public int disc_number { get; set; }
    public int duration_ms { get; set; }
    public bool @explicit { get; set; }
    public ExternalIds? external_ids { get; set; }
    public ExternalUrls6? external_urls { get; set; }
    public string? href { get; set; }
    public string? id { get; set; }
    public string? name { get; set; }
    public int popularity { get; set; }
    public string? preview_url { get; set; }
    public int track_number { get; set; }
    public string? type { get; set; }
    public string? uri { get; set; }
}

public class Item
{
    public string? added_at { get; set; }
    public AddedBy? added_by { get; set; }
    public bool is_local { get; set; }
    public Track? track { get; set; }
}

public class Tracks
{
    public string? href { get; set; }
    public List<Item>? items { get; set; }
    public int limit { get; set; }
    public object? next { get; set; }
    public int offset { get; set; }
    public object? previous { get; set; }
    public int total { get; set; }
}
