using System.Net;
using System.Text.Json;

internal class WordleSolution
{
    private class WordleJSON
    {
        public int id { set; get; }
        public string? solution { set; get; }
        public string? print_date { set; get; }
        public int days_since_launch { set; get; }
        public string? editor { set; get; }
    }
    public async Task<bool> print(string date)
    {
        string url = string.Format("https://www.nytimes.com/svc/wordle/v2/{0}.json", date);
        System.Net.ServicePointManager.Expect100Continue = true;
        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13
            | SecurityProtocolType.Tls
            | SecurityProtocolType.Tls11
            | SecurityProtocolType.Tls12;
        HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
            };

            string responseBody = await response.Content.ReadAsStringAsync();
            WordleJSON? wordle = JsonSerializer.Deserialize<WordleJSON>(responseBody, options);

            if (wordle == null)
            {
                Console.WriteLine("Error");
                return false;
            }
            else
            {
                Console.WriteLine("Wordle Solution for {0}: {1}", date, wordle.solution);
                return true;
            }
        }

        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return false;
        }
    }
}
internal static class Program
{
    static bool checkDate(string date)
    {
        DateTime result;
        return DateTime.TryParseExact(date,
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out result);
    }
    static void Main(string[] args)
    {
        WordleSolution solution = new WordleSolution();
        string date;
        if (args.Length == 0)
            date = DateTime.Today.ToString("yyyy-MM-dd");
        else
            date = args[0];
        if (checkDate(date))
            solution.print(date).Wait();
        else
            Console.WriteLine("Date format error, should by yyyy-MM-dd");
    }
}
