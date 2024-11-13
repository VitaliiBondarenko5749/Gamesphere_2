namespace CatalogOfGames.BAL.DTOs
{
    public class GameEditDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Icon { get; set; } = default!;
        public PublisherInfoDTO Publisher { get; set; } = default!;
        public ICollection<GameImageInfoDTO>? GameImages { get; set; }
        public ICollection<GameVideoInfoDTO>? VideoLinks { get; set; }
        public ICollection<CategoryInfoDTO>? Categories { get; set; }
        public ICollection<DeveloperInfoDTO>? Developers { get; set; }
        public ICollection<LanguageInfoDTO>? Languages { get; set; }
        public ICollection<PlatformInfoDTO>? Platforms { get; set; }
    }
}