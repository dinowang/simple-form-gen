using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Text.Json;

namespace SimpleFormGen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("UiType: (1) Slack App UI (2) Adaptive Cards [default=2]: ");
            var userInput = Console.ReadLine();
            // var userInput = "1";

            var uiStyle = userInput == "1"
                                ? UiStyle.SlackDialog
                                : UiStyle.AdaptiveCards;

            var yaml = new DeserializerBuilder()
                                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                .Build();

            using var stream = new FileStream("resources.yaml", FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            var resources = yaml.Deserialize<Resources>(reader);

            // var options = new JsonSerializerOptions { WriteIndented = true };
            // var json = JsonSerializer.Serialize(resources, resources.GetType(), options);
            // Console.WriteLine(json);

            foreach (var form in resources.Forms)
            {
                Console.WriteLine($"Form: {form.Name}");

                var layouts = new List<string>();

                foreach (var properties in form.Layouts.Cast<IDictionary<string, object>>())
                {
                    var use = properties["use"].ToString();
                    var component = resources.Components.First(c => c.Name == use);
                    var template = component.GetTemplate(uiStyle, properties);

                    layouts.Add(template);
                }

                var wrapper = uiStyle == UiStyle.SlackDialog
                                            ? @$"{{
                                                ""dialog"": {{
                                                    ""title"": ""{form.Name}"",
                                                    ""submit_label"": ""Submit"",
                                                    ""callback_id"": """",
                                                    ""elements"": [
                                                        {string.Join(",", layouts)}
                                                    ]
                                                }}
                                              }}"
                                            : @$"{{
                                                ""type"": ""AdaptiveCard"",
                                                ""body"": [
                                                    {{
                                                        ""type"": ""TextBlock"",
                                                        ""text"": ""{form.Name}"",
                                                        ""wrap"": true,
                                                        ""style"": ""heading""
                                                    }},
                                                    {string.Join(",", layouts)},
                                                    {{
                                                        ""type"": ""ActionSet"",
                                                        ""actions"": [
                                                            {{
                                                                ""type"": ""Action.Submit"",
                                                                ""title"": ""Submit""
                                                            }}
                                                        ]
                                                    }}
                                                ]
                                              }}";

                Console.WriteLine($"{uiStyle}\r\n{JsonBeautifier(wrapper)}");

                // https://app.slack.com/block-kit-builder/
                // https://adaptivecards.io/designer/
            }
        }

        private static string JsonBeautifier(string json)
        {
            // Console.WriteLine(json);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var obj = JsonSerializer.Deserialize<dynamic>(json);
            return JsonSerializer.Serialize(obj, obj.GetType(), options);
        }
    }
}


