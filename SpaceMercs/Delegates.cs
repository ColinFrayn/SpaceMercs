namespace SpaceMercs {
    public class Delegates {
        public delegate void ShowMessageDelegate(string s, Action? a);
        public delegate void ShowMessageDecisionDelegate(string s, Action a, Action? b);
        public delegate void PlaySoundDelegate(string s);
    }
}
