using System;
using System.Net.Http;
using System.Text;
using Amazon.Lambda.Core;
using Newtonsoft.Json.Linq;

// Allow Lambda to use JSON as input/output
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook
{
    public class Function
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<string> FunctionHandler(string input, ILambdaContext context)
        {
            // Turn the input JSON string into an object
            JObject data = JObject.Parse(input);

            // Get the issue URL from the payload
            string issueUrl = data["issue"]?["html_url"]?.ToString();

            if (string.IsNullOrEmpty(issueUrl))
            {
                return "No issue URL found.";
            }

            // Get your Slack webhook URL from environment variables
            string slackUrl = Environment.GetEnvironmentVariable("SLACK_URL");

            // Create the message to send to Slack
            string messageJson = $"{{\"text\": \"New GitHub Issue: {issueUrl}\"}}";

            // Send the message to Slack
            var content = new StringContent(messageJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(slackUrl, content);

            return "Posted to Slack!";
        }
    }
}
