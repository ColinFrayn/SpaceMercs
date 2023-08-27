using System.IO;

namespace SpaceMercs {
    public interface IItem {
        string Name { get; } // Name of this thing
        double Mass { get; } // In kg
        double Cost { get; } // In credits
        string Desc { get; } // Textual description
        void SaveToFile(StreamWriter file);
    }
}
