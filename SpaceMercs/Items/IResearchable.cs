
namespace SpaceMercs.Items {
    public interface IResearchable {
        public string Name { get; }
        public Requirements? Requirements { get; }
        public string Description { get; }
        public bool CanBuild(Race? race);
    }
}
