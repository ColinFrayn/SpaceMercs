using System.Windows.Media;

namespace SpaceMercs {
    internal static class SoundEffects {
        private static readonly Dictionary<string, MediaPlayer> Players = new Dictionary<string, MediaPlayer>();
        private static readonly object oLock = new object();

        public static void PlaySound(string strSound) {
            lock (oLock) {
                if (Players.ContainsKey(strSound)) {
                    Players[strSound].Stop();
                    Players[strSound].Play();
                }
                else {
                    MediaPlayer mp = new MediaPlayer();
                    mp.Open(new Uri(@"Sounds/" + strSound + ".wav", UriKind.Relative));
                    mp.Play();
                    Players.Add(strSound, mp);
                }
            }
        }
    }
}
