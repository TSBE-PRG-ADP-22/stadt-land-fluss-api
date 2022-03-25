namespace StadtLandFussApi.Persistence.Entities
{
    public record ToDo
    {
        public int Id { get; set; } = default!;

        public string Key { get; set; } = default!;
    }
}
