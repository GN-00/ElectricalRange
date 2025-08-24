namespace ProjectsNow.Data.Library
{
    public class ItemType
    {
        public string Id { get; set; }

        public List<Linked> LinkedProperties { get; set; } = [];

        public override string ToString()
        {
            return Id;
        }
    }
}
