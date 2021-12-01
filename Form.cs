using System.Dynamic;

namespace SimpleFormGen
{
    public class Form
    {
        public string Name { get; set; }

        public IEnumerable<ExpandoObject> Layouts { get; set; }
    }
}