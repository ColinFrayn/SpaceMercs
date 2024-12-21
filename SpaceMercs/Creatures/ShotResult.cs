namespace SpaceMercs {
    public class ShotResult {
        public bool Hit { get; init; }
        public IEntity? Source { get; init; }

        public ShotResult(IEntity? src, bool hit) {
            Hit = hit;
            Source = src;
        }
    }
}
