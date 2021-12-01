using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SimpleFormGen
{
    public class Component
    {
        public string Name { get; set; }
        public string Slack { get; set; }
        public string AdaptiveCards { get; set; }

        public string GetTemplate(UiStyle uiStyle, IDictionary<string, object> properties)
        {
            var template = uiStyle == UiStyle.SlackDialog
                            ? Slack
                            : AdaptiveCards;

            if (template == null)
                return template;

            template = Regex.Replace(template, @"\$\{([^:\}]+):([^}]+)\}", match =>
            {
                var cmd = match.Groups[1].Value;
                var key = match.Groups[2].Value;

                switch (cmd)
                {
                    case "select":
                        var json = JsonSerializer.Serialize(properties[key]);

                        if (uiStyle == UiStyle.SlackDialog)
                            json = Regex.Replace(json, "\"text\":", "\"label\":");
                        else
                            json = Regex.Replace(json, "\"text\":", "\"title\":");
                        return json;
                }

                return "[]";
            });


            template = Regex.Replace(template, @"\$\{([^}]+)\}", match =>
            {
                var key = match.Groups[1].Value;

                if (properties.TryGetValue(key, out var value))
                    return value.ToString();

                return match.Value;
            });

            return template;
        }
    }
}