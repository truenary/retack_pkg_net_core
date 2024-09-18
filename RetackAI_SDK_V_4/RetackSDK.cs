using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RetackAI_SDK_V_4
{
    /// <summary>
    /// Main client class to interact with the Retack AI API. Provides methods to report errors to the Retack service.
    /// </summary>
    public class RetackClient
    {
        // Configuration object that stores environment-related settings like API keys
        private readonly RetackConfig _retackConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetackClient"/> class.
        /// </summary>
        /// <param name="retackConfig">Configuration for the Retack client, such as environment keys.</param>
        /// <exception cref="ArgumentNullException">Thrown if the configuration is null.</exception>
        public RetackClient(RetackConfig retackConfig)
        {
            // Ensures that retackConfig is not null
            _retackConfig = retackConfig ?? throw new ArgumentNullException(nameof(retackConfig));
        }

        /// <summary>
        /// Asynchronously reports an error with an optional stack trace and user context to Retack AI.
        /// </summary>
        /// <param name="error">A brief title or description of the error.</param>
        /// <param name="stackTrace">The stack trace object (if available).</param>
        /// <param name="userContext">Additional context about the user (optional).</param>
        /// <returns>A boolean value indicating if the report was successful.</returns>
        public async Task<bool> ReportErrorAsync(string error, object stackTrace, UserContext userContext = null)
        {
            // Define the base API URL and endpoint
            var baseUrl = "https://api.retack.ai";
            var endpoint = "/observe/error-log/";

            // Define required headers for the HTTP request
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },  // Specifies the format of the request body
                { "ENV-KEY", _retackConfig.EnvKey }      // Include the environment key for authentication
            };

            // Create the payload (body) of the request, which contains the error details
            var body = new Dictionary<string, dynamic>
            {
                { "title", error },                                  // Title or brief description of the error
                { "stack_trace", stackTrace.ToString() },            // Stack trace in json format
                { "user_context", userContext?.ToJson() }            // Optional user context in JSON format
            };

            // Use HttpClient to send an asynchronous POST request to the Retack API
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + endpoint))
                {
                    // Add headers to the request
                    request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
                    request.Headers.TryAddWithoutValidation("ENV-KEY", _retackConfig.EnvKey);

                    // Serialize the body dictionary to JSON format and attach it as the request content
                    var json = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Send the HTTP request and wait for the response
                    using (var response = await httpClient.SendAsync(request))
                    {
                        // Check if the response indicates success (status code 200-299)
                        if (!response.IsSuccessStatusCode)
                        {
                            // If not successful, log the failure
                            Console.WriteLine("Unable to report error to Retack AI.");
                            Console.WriteLine(await response.Content.ReadAsStringAsync());
                        }

                        // Return true if successful, otherwise false
                        return response.IsSuccessStatusCode;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents the structure of an error report request.
    /// </summary>
    public class ErrorReportRequest
    {
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public UserContext UserContext { get; set; }
    }

    /// <summary>
    /// Configuration class that holds the environment key needed to authenticate API requests.
    /// </summary>
    public class RetackConfig
    {
        public string EnvKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetackConfig"/> class.
        /// </summary>
        /// <param name="EnvKey">The environment key used for API authentication.</param>
        public RetackConfig(string EnvKey)
        {
            this.EnvKey = EnvKey;
        }
    }

    /// <summary>
    /// Contains user-specific context information that can be sent with error reports.
    /// </summary>
    public class UserContext
    {
        public string UserName { get; set; }
        public Dictionary<string, dynamic> Extras { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserContext"/> class.
        /// </summary>
        /// <param name="userName">The username of the user experiencing the error.</param>
        /// <param name="extras">Additional data related to the user's session or environment.</param>
        public UserContext(string userName, Dictionary<string, dynamic> extras)
        {
            UserName = userName;
            Extras = extras;
        }

        /// <summary>
        /// Converts the user context object to a JSON-formatted string.
        /// </summary>
        /// <returns>A JSON string representation of the user context.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}