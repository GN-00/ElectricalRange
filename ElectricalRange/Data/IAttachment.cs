namespace ProjectsNow.Data
{
    public interface IAttachment
    {
        int Id { get; set; }
        string Name { get; set; }
        byte[] Data { get; set; }
    }
}
