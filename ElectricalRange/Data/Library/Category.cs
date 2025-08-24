
namespace ProjectsNow.Data.Library
{
    public class Category : Base
    {
        private List<Property> _properties;

        public string Id { get; }
        public int Sort { get; }
        public List<Property> Properties
        {
            get => _properties;
            set => SetValue(ref _properties, value)
                  .UpdateProperties(this, nameof(Width));
        }

        public int Width
        {
            get
            {
                int properties = Properties != null ? Properties.Count : 0;
                if (properties > 8)
                    return 1050;
                else if (properties > 6)
                    return 840;
                else if (properties > 4)
                    return 630;
                else if (properties > 2)
                    return 420;
                else
                    return 210;
            }
        }
        public override string ToString()
        {
            return $"{Sort}:{Id}";
        }
    }
}
