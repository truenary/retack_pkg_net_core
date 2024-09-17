using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RetackAI_SDK_V_4
{
    public class RetackClient
    {
        private readonly RetackConfig _retackConfig;

        public RetackClient(RetackConfig retackConfig)
        {
            _retackConfig = retackConfig ?? throw new ArgumentNullException(nameof(retackConfig));
        }

        public async Task<bool> ReportErrorAsync(string error, object stackTrace, UserContext userContext = null)
        {
            var baseUrl = "https://api.retack.ai";
            var endpoint = "/observe/error-log/";

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "ENV-KEY", _retackConfig.EnvKey }
            };

            var body = new Dictionary<string, dynamic>
            {
                { "title", error },
                { "stack_trace", stackTrace?.ToString() },
                { "user_context", userContext?.ToJson() }
            };

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + endpoint))
                {
                    request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
                    request.Headers.TryAddWithoutValidation("ENV-KEY", _retackConfig.EnvKey);

                    var json = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    using (var response = await httpClient.SendAsync(request))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Unable to report error to Retack AI.");
                            Console.WriteLine(await response.Content.ReadAsStringAsync());
                        }
                        return response.IsSuccessStatusCode;
                    }
                }
            }
        }
    }

    public class ErrorReportRequest
    {
        public string Error { get; set; }
        public string StackTrace { get; set; }
        public UserContext UserContext { get; set; }
    }

    public class RetackConfig
    {
        public string EnvKey { get; set; }

        public RetackConfig(string EnvKey)
        {
            EnvKey = EnvKey;
        }
    }

    public class UserContext
    {
        public string UserName { get; set; }
        public Dictionary<string, dynamic> Extras { get; set; }

        public UserContext(string userName, Dictionary<string, dynamic> extras)
        {
            UserName = userName;
            Extras = extras;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
