#r "Newtonsoft.Json"
#r "System.Data.SqlClient"

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;


public class Result {
    public string text;
    public string detectedLanguage;
    public string translation;
    public string targetLanguage;

    public Result(string text, string detectedLanguage, string translation, string targetLanguage)
    {
        this.text = text;
        this.detectedLanguage = detectedLanguage;
        this.translation = translation;
        this.targetLanguage = targetLanguage;
    }

    public override string ToString()
    {
        return $"text = {text} detectedLanguage = {detectedLanguage} translation = {translation} targetLanguage = {targetLanguage}";
    }
}

public static string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTIONSTRING");

public static async Task Run(string myQueueItem, ILogger log)
{
    var result = JsonConvert.DeserializeObject<Result>(myQueueItem);
    log.LogInformation($"C# Queue trigger function processed: {result.ToString()}");
    
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        conn.Open();
        var query = $@"INSERT INTO log(
            text,
            detected_language,
            translation,
            target_language
        ) VALUES (
            '{result.text}',
            '{result.detectedLanguage}',
            '{result.translation}',
            '{result.targetLanguage}'
        )";

        log.LogInformation($"Running query: {query}");

        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            var rows = await cmd.ExecuteNonQueryAsync();
            log.LogInformation($"{rows} rows were updated");
        }
    }
}