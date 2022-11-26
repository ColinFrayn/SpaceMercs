using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SpaceMercs {
  abstract class HabitableAO : AstronomicalObject {
    public Colony Colony { get; private set; }
    public Colony.BaseType Base { get { if (Colony == null) return Colony.BaseType.None; return Colony.Base; } }
    public int BaseSize {
      get {
        if (Colony == null) return 0;
        return Colony.BaseSize;
      }
    }
    public Planet.PlanetType Type;
    protected List<Mission> _MissionList = null;
    public IEnumerable<Mission> MissionList {  get { return _MissionList?.AsReadOnly(); } }
    public bool Scanned { get { return (_MissionList != null); } }
    public int CountMissions {  get { return _MissionList == null ? 0 : _MissionList.Count; } }

    protected void LoadMissions(XmlNode xml) {
      XmlNode xms = xml.SelectSingleNode("Missions");
      if (xms == null) return;
      _MissionList = new List<Mission>();
      foreach (XmlNode xm in xms.SelectNodes("Mission")) {
        _MissionList.Add(new Mission(xm, this));
      }
    }
    protected void SaveMissions(StreamWriter file) {
      if (_MissionList == null) return;
      file.WriteLine(" <Missions>");
      foreach (Mission m in _MissionList) m.SaveToFile(file);
      file.WriteLine(" </Missions>");
    }
    public void RemoveMission(Mission miss) {
      if (_MissionList == null || miss == null) return;
      if (_MissionList.Contains(miss)) {
        _MissionList.Remove(miss);
        return;
      }
      // In case the mission pointers got messed up, check for an identical mission and delete it
      foreach (Mission m in _MissionList) {
        if (m.IsSameMissionAs(miss)) {
          _MissionList.Remove(m);
          return;
        }
      }
    }
    public void AddMission(Mission miss) {
      if (miss == null) return;
      if (_MissionList == null) _MissionList = new List<Mission>();
      if (_MissionList.Contains(miss)) return;
      _MissionList.Add(miss);
    }

    public double TDiff(Race rc) {
      double tdiff = Temperature - rc.BaseTemp;
      if (tdiff > 0.0) tdiff *= 2.0;
      return Math.Abs(tdiff);
    }
    public void SetColony(Colony cl) {
      Colony = cl;
      cl.Owner.AddColony(cl);
    }
    public void SetupBase(Race rc,int iSize) {
      Colony = new Colony(rc, iSize, Seed, this);
    }
    public void ExpandBase(Colony.BaseType bt) {
      Colony.ExpandBase(bt);
    }
    public int ExpandBase(Race rc, Random rand) {
      if (Colony == null) {
        Colony = new Colony(rc, 1, rand.Next(1000000), this);
        return 1;
      }
      return Colony.ExpandBase(rand);
    }

    // Draw icons showing whether or not this body has a base on it and, if so, then what type.
    public abstract void DrawBaseIcon();

    // Price modifier, based on the relations with the owning race
    public double PriceModifier {
      get {
        return 1.0; // TODO
      }
    }

  }
}
