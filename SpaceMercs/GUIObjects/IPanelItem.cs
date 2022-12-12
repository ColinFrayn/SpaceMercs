using OpenTK.Mathematics;

namespace SpaceMercs {
  interface IPanelItem {
    double ZDist { get; }
    bool Enabled { get; }
    uint ID { get; }
    GUIPanel? SubPanel { get; }

    void DrawSelectionFrame();
    IPanelItem Draw(double xx, double yy, GUIPanel gpParent);
    void SetPos(double x, double y);
    void SetIconSize(double w, double h);
    void SetSubPanel(GUIPanel gpl);
    void SetZDist(double zd);
    void SetOverlay(int _texID, Vector4d texRect, Vector4d dimRect);
  }
}
