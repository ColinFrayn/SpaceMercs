using OpenTK.Mathematics;
using SpaceMercs.Graphics;

namespace SpaceMercs {
    interface IPanelItem {
        float ZDist { get; }
        bool Enabled { get; }
        uint ID { get; }
        GUIPanel? SubPanel { get; }

        void DrawSelectionFrame();
        IPanelItem Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent);
        void SetPos(float x, float y);
        void SetIconSize(float w, float h);
        void SetSubPanel(GUIPanel gpl);
        void SetZDist(float zd);
        void SetOverlay(int _texID, Vector4 texRect, Vector4 dimRect);
    }
}
