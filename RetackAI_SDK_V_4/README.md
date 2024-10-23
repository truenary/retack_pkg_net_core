# Retack-AI SDK (1.0.1) for ASP.NET

## Overview

This SDK provides integration with Retack-AI for ASP.NET projects, allowing you to report errors to Retack-AI for analysis and monitoring.

## Installation

To install the Retack-AI SDK in your ASP.NET project, follow these steps:

1. Install the `Retack-AI` package from NuGet:

   ```
   dotnet add package RetackAI.NuGet.AspNetSdk 1.0.1
   ```

2. how to configure the SDK with the Retack-AI API key and how to use the ReportErrorAsync method to report errors.

   ```
        using RetackAISDK;

        class Program
        {
            static async Task Main(string[] args)
            {
                // Configure Retack-AI SDK
                var retackConfig = new RetackConfig("YOUR_API_KEY");
                var retackClient = new RetackClient(retackConfig);

                try
                {
                    // Your application code that may throw exceptions
                }
                catch (Exception ex)
                {
                    // Construct user context
                    var userContext = new UserContext
                    {
                        UserName = "user@retack.io",
                        Extras = new Dictionary<string, string>
                        {
                            { "key1", "value1" },
                            { "key2", "value2" }
                        }
                    };
                    // Report error to Retack-AI
                    var success = await retackClient.ReportErrorAsync(ex.Message, ex.StackTrace, userContext);
                    if (success)
                    {
                        Console.WriteLine("Error reported successfully to Retack-AI.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to report error to Retack-AI.");
                    }
                }
            }
        }

   ```



## Usage

Once the Retack-AI SDK is installed and configured in your ASP.NET project, you can use it to report errors to Retack-AI for analysis and monitoring. To report an error, simply make a POST request to the `/report-error` endpoint with the appropriate error information in the request body.

## Contributing

Contributions are welcome! If you encounter any issues with the SDK or have suggestions for improvements, please feel free to open an issue or submit a pull request on the GitHub repository.




