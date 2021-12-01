using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Text.Json;

namespace SimpleFormGen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var uiStyles = Enum.GetValues(typeof(UiStyle)).Cast<UiStyle>();
            var selectedStyle = uiStyles.First();
            var availableStyles = string.Join(", ", uiStyles.Select(x => $"{(int)x}={x}"));

            Console.Write($"UiStyle: {availableStyles} [default={selectedStyle}]: ");
            var userInput = Console.ReadLine();
            if (Enum.TryParse(userInput, out UiStyle style))
            {
                selectedStyle = style;
            }

            var yaml = new DeserializerBuilder()
                                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                .Build();

            using var stream = new FileStream("resources.yaml", FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            var resources = yaml.Deserialize<Resources>(reader);


            foreach (var form in resources.Forms)
            {
                Console.WriteLine($"Form: {form.Name}");

                var json = form.CreateLayout(resources.Components, selectedStyle, title: "untitled");

                Console.WriteLine($"{selectedStyle}\r\n{json}");

                // https://api.slack.com/dialogs
                // https://app.slack.com/block-kit-builder/
                // https://adaptivecards.io/designer/
            }
        }
    }
}


