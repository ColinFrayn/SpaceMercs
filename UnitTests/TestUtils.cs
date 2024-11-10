using SpaceMercs;
using SpaceMercs.Dialogs;

namespace UnitTests {
    internal class TestUtils {
        internal static Team TestTeam() {
            NewGame ng = new NewGame();
            ng.PlayerName = "Test Name";
            Race rc = StaticData.GetRaceByName("Human") ?? throw new Exception("Could not ID Human race");
            return new Team(ng, rc);
        }
    }
}
