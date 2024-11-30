using SpaceMercs;

namespace UnitTests {
    public class TestConst {

        // Nothing much here
        // Just want to make sure debugging switches are turned off before releasing.

        [Test]
        public void Test_MakeSureDebuggingSwitchesAreOff() {
            Assert.False(Const.DEBUG_ALL_ENCOUNTERS_INACTIVE);
            Assert.False(Const.DEBUG_MORE_BOARDERS);
            Assert.False(Const.DEBUG_VIEW_ALL_CIVS);
            Assert.False(Const.DEBUG_VISIBLE_ALL);
            Assert.False(Const.DEBUG_RANDOMISE_VENDORS);
            Assert.False(Const.DEBUG_SHOW_SELECTED_ENTITY_VIS);
            Assert.False(Const.DEBUG_MAP_CREATION);
            Assert.Equals(1.0f, Const.DEBUG_ENCOUNTER_FREQ_MOD);
            Assert.Equals(1, Const.DEBUG_WEAPON_SKILL_MOD);
            Assert.Equals(1, Const.DEBUG_EXPERIENCE_MOD);
            Assert.Equals(0, Const.DEBUG_ADDITIONAL_STARTING_CASH);
        }
    }
}