namespace SimpleFormGen
{
    public class Resources
    {
        public IEnumerable<Component>? Components { get; set; }
        public IEnumerable<Form>? Forms { get; set; }
    }

    public class QuickAccessResource
    {
        public static QuickAccessResource Create(Resources resources) => new QuickAccessResource
        {
            Components = resources.Components.ToDictionary(c => c.Name, c => c),
            Forms = resources.Forms.ToDictionary(f => f.Name, f => f)
        };

        public IDictionary<string, Component> Components { get; set; }
        public IDictionary<string, Form> Forms { get; set; }
    }
}
