#pragma warning disable CS8602
#pragma warning disable CS8603
#pragma warning disable CS8604

using System.Text.RegularExpressions;

namespace SimpleFormGen
{
    public class Component
    {
        public string? Name { get; set; }
        public string? Slack { get; set; }
        public string? AdaptiveCards { get; set; }

        public string? GetTemplate(UiStyle uiStyle, IDictionary<string, object> properties)
        {
            var template = uiStyle == UiStyle.SlackAppUI ? Slack : AdaptiveCards;

            if (template == null)
                return template;

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