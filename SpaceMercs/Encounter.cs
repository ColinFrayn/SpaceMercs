namespace SpaceMercs {
    static class Encounter {

        // Check if we get intercepted when journeying
        public static Mission? CheckForInterception(AstronomicalObject aoFrom, AstronomicalObject aoTo, double dJourneyTime, Team PlayerTeam, double dFract) {
            if (dJourneyTime < Const.SecondsPerDay / 2.0) return null; // Very fast drive in system, or very short hop
            if (Const.DEBUG_ENCOUNTER_FREQ_MOD <= 0.0) return null; // Turn off encounters if debugging

            // Calculate interception chance
            double dDanger = 40.0;
            int iFromSize = (aoFrom is HabitableAO haof) ? haof.BaseSize : 0;
            int iToSize = (aoTo is HabitableAO haot) ? haot.BaseSize : 0;

            if (iFromSize + iToSize > 8) return null; // No chance of pirates when travelling between large bases, as these routes are well policed.

            // Travelling between two locations in the same system
            if (aoFrom.GetSystem() == aoTo.GetSystem()) {
                // One or the other is a colony, so it's safer
                dDanger -= (double)(iFromSize + iToSize);
                // Check if we're travelling from a planet to one of its satellites, or vice versa, and double the safety bonus
                if (aoFrom.AOType == AstronomicalObject.AstronomicalObjectType.Planet && aoTo.AOType == AstronomicalObject.AstronomicalObjectType.Moon && ((Moon)aoTo).Parent == aoFrom) dDanger -= (double)(iFromSize + iToSize);
                else if (aoFrom.AOType == AstronomicalObject.AstronomicalObjectType.Moon && aoTo.AOType == AstronomicalObject.AstronomicalObjectType.Planet && ((Moon)aoFrom).Parent == aoTo) dDanger -= (double)(iFromSize + iToSize);
            }

            // Greater risk if you're not very friendly with the target system
            if (PlayerTeam.GetRelations(aoFrom) <= 2) dDanger -= (PlayerTeam.GetRelations(aoFrom) - 3) * 2.0;
            if (PlayerTeam.GetRelations(aoTo) <= 2) dDanger -= (PlayerTeam.GetRelations(aoTo) - 3) * 2.0;

            // Take into account journey time (if short). We should really look at distance here, but if we're travelling fast then it's harder to attack us, too.
            double dDays = dJourneyTime / Const.SecondsPerDay;
            if (dDays < 5.0) dDanger -= (5.0 - dDays) * 2.0;

            // Did we get intercepted?
            Random rand = new Random();
            double dIntercept = rand.NextDouble() * Const.BaseEncounterScarcity / Const.DEBUG_ENCOUNTER_FREQ_MOD;

            // If not intercepted then return
            if (dIntercept > dDanger) return null;

            // Work out which species we're dealing with
            Race? rc = null; // Unknown alien race

            // Work out which race this is, biased by how far through the journey you are
            if (rand.NextDouble() < dFract) rc = aoTo.GetSystem().GetRandomRace(rand);
            else rc = aoFrom.GetSystem().GetRandomRace(rand);
            if (rc is null && aoFrom.GetSystem().Owner is not null) rc = aoFrom.GetSystem().Owner;
            if (rc is null && aoTo.GetSystem().Owner is not null) rc = aoTo.GetSystem().Owner;

            // No luck with system owners so test sector owners
            if (rc is null) {
                if (aoFrom.GetSystem().Sector.Inhabitant == null) rc = aoTo.GetSystem().Sector.Inhabitant;
                else if (aoTo.GetSystem().Sector.Inhabitant == null) rc = aoFrom.GetSystem().Sector.Inhabitant;
                else if (rand.NextDouble() > 0.5) rc = aoFrom.GetSystem().Sector.Inhabitant;
                else rc = aoTo.GetSystem().Sector.Inhabitant;
            }

            // Still not found a suitable owning race?
            if (rc is null) {
                //double rF = aoFrom.GetMapLocation().Length; // Distance from origin of map (proportional to danger rating)
                //double rT = aoTo.GetMapLocation().Length; // Distance from origin of map (proportional to danger rating)
                //double r = (rF + rT) / 2.0;
                //if (rand.NextDouble() < 0.5 || (rand.NextDouble() * 100.0) > r) {
                //    return null;
                //}
                // Unidentified alien race
                // TODO: Does this mean anything? For now just abort the encounter. I don't want Race ever to be null
                return null;
            }

            // Calculate the difficulty of this mission, based on the distance from home
            //Vector3d Pos = aoFrom.GetMapLocation() + ((aoTo.GetMapLocation() - aoFrom.GetMapLocation()) * dFract);
            int iDiff = (int)((aoFrom.GetRandomMissionDifficulty(rand) * (1.0 - dFract)) + (aoTo.GetRandomMissionDifficulty(rand) * dFract));
            ShipEngine minDrive = StaticData.GetMinimumDriveByDistance(AstronomicalObject.CalculateDistance(aoFrom, aoTo)) ?? throw new Exception("Mission not reachable!");

            // Check for the various intercept types:
            if (dDanger - dIntercept < 15.0 + (Const.DEBUG_ALL_ENCOUNTERS_INACTIVE ? 100000.0 : 0.0)) return InactiveEncounter(rc, dDanger - dIntercept, rand, iDiff, PlayerTeam, minDrive);
            return ActiveEncounter(rc, iDiff, PlayerTeam, minDrive);
        }

        // Do an inactive encounter
        private static Mission InactiveEncounter(Race rc, double dDanger, Random rand, int iDiff, Team PlayerTeam, ShipEngine minDrive) {
            string strDesc = rc.Known ? rc.Name : "unidentified alien";
            if (MessageBox.Show(new Form { TopMost = true }, $"You have detected a distress signal from a nearby {strDesc} vessel. Do you want to investigate?", "Distress Signal", MessageBoxButtons.YesNo) != DialogResult.Yes) { // REPLACE WITH msgBox
                return Mission.CreateIgnoreMission();
            }

            // If race != null then it could turn out to be trap (->Active)
            if (rand.NextDouble() * 50.0 < dDanger && !Const.DEBUG_ALL_ENCOUNTERS_INACTIVE) return ActiveEncounter(rc, iDiff, PlayerTeam, minDrive);

            // Scan for life forms. If none then can just collect resources.
            bool bLifeForms = (rand.NextDouble() > 0.3);
            if (bLifeForms) {
                // Ship might be friendly - they could offer cash to help them with repairs (which takes time)
                bool bFriendly = (rand.NextDouble() * 20.0 > dDanger);
                if (bFriendly) {
                    double dTime = Math.Round(2.0 + rand.NextDouble() * iDiff, 2);
                    double dReward = Math.Round((dTime * 5.0) + (rand.NextDouble() * (iDiff + 2.0) / 2.0), 2);
                    string strMessage = $"You have discovered a stranded {strDesc} freighter. They request your help for repairs. Time = {dTime} days; Reward = {dReward} credits. Will you help?";
                    if (MessageBox.Show(new Form { TopMost = true }, strMessage, "Stranded Freighter", MessageBoxButtons.YesNo) == DialogResult.No) return Mission.CreateIgnoreMission(); // REPLACE WITH msgBox
                    return Mission.CreateRepairMission(rc, (float)(dTime * Const.SecondsPerDay), dReward);
                }
            }
        
            // No life forms - let's just get salvage
            if (!bLifeForms) {
                double dTime = Math.Round(2.0 + rand.NextDouble() * iDiff, 2);
                string strMessage = $"You have discovered a stranded {strDesc} freighter. No life forms have been detected. Do you want to salvage usable items (" + dTime + " days)?";
                if (MessageBox.Show(new Form { TopMost = true }, strMessage, "Abandoned Freighter", MessageBoxButtons.YesNo) == DialogResult.No) return Mission.CreateIgnoreMission(); // REPLACE WITH msgBox
                return Mission.CreateSalvageMission(rc, iDiff, (float)(dTime * Const.SecondsPerDay));
            }

            // Otherwise you will need a hostile boarding party
            string strMessage2 = $"You have discovered a stranded {strDesc} vessel. Scans have detected hostile life forms. Do you wish to board?";
            if (MessageBox.Show(strMessage2, "Stranded Freighter", MessageBoxButtons.YesNo) == DialogResult.No) return Mission.CreateIgnoreMission(); // REPLACE WITH msgBox

            // Create a mission for the landing party scenario
            return Mission.CreateBoardingPartyMission(rc, iDiff);
        }

        // Do an active encounter
        private static Mission ActiveEncounter(Race rc, int iDiff, Team PlayerTeam, ShipEngine minDrive) {
            // Generate a mission, including the random ship
            Mission miss = Mission.CreateShipCombatMission(rc, iDiff, minDrive);
            if (miss.ShipTarget is null) throw new Exception("ShipCombat Mission does not have a target ship");

            // Can attempt to flee (if faster accel) or fight. If slower/equal speed then must fight.
            string strRace = rc.Known ? rc.Name : "unidentified alien";
            string strMessage = $"You have been ambushed by a hostile {strRace} vessel.";
            if (PlayerTeam.PlayerShip.CanOutrun(miss.ShipTarget)) {
                if (MessageBox.Show(new Form { TopMost = true }, strMessage + " You can outrun the ambushers. Do you want to flee?", "Ambush", MessageBoxButtons.YesNo) == DialogResult.Yes) { // REPLACE WITH msgBox
                    return Mission.CreateIgnoreMission();
                }
            }
            else {
                MessageBox.Show(new Form { TopMost = true }, strMessage + " Prepare to fight!"); // REPLACE WITH msgBox
            }

            return miss;
        }
    }
}
