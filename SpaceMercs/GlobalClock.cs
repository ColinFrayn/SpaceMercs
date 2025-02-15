namespace SpaceMercs {
    public class GlobalClock {
        public DateTime CurrentTime { get; set; }

        public GlobalClock(DateTime dt) {
            CurrentTime = dt;
        }

        public float ElapsedSeconds() {
            return (float)(CurrentTime - Const.StartingDate).TotalSeconds;
        }

        public void SetTime(DateTime dt) {
            CurrentTime = dt;
        }

        public void AddMilliseconds(long ms) {
            CurrentTime = CurrentTime.AddMilliseconds(ms);
        }

        public void AddSeconds(double seconds) {
            CurrentTime = CurrentTime.AddSeconds(seconds);
        }

        public void AddHours(double hours) {
            CurrentTime = CurrentTime.AddHours(hours);
        }
    }
}
