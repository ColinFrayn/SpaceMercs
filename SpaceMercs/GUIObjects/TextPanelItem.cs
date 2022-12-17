using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    class TextPanelItem : PanelItem {
        private readonly TextMeasure measure;
        private string Text;
        private const float TextSizeScale = 0.4f;

        public TextPanelItem(string strText, uint _id) : base(null, true, _id) {
            measure = TextRenderer.MeasureText(strText);
            Text = strText;
        }

        public override PanelItem? Draw(ShaderProgram prog, double xx, double yy, GUIPanel gpParent, Vector2 itemPos, Vector2 itemSize, float zdist, float aspect) {
            PanelItem? piHover = null;
            double BorderY = gpParent.BorderY;
            float itemX = itemPos.X;
            float itemY = itemPos.Y;
            float itemW = itemSize.X;
            float itemH = itemSize.Y;

            if (xx >= itemX && xx <= itemX + itemW && yy >= itemY - BorderY && yy <= itemY + itemH + BorderY) {
                piHover = this;
            }

            if (Enabled) {
                if (piHover != null) {
                    prog.SetUniform("flatColour", new Vector4(0.5f, 0.5f, 0.5f, 1f));
                }
                else {
                    prog.SetUniform("flatColour", new Vector4(0.3f, 0.3f, 0.3f, 1f));
                }
            }

            // Draw the background
            prog.SetUniform("textureEnabled", false);
            prog.SetUniform("lightEnabled", false);
            Matrix4 translateM = Matrix4.CreateTranslation(itemX, itemY, zdist);
            Matrix4 scaleM = Matrix4.CreateScale(itemW, itemH, 1f);
            Matrix4 modelM = scaleM * translateM;
            prog.SetUniform("model", modelM);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Textured.BindAndDraw();

            // Draw the text string
            TextRenderOptions tro = new TextRenderOptions() {
                Alignment = Alignment.CentreLeft,
                Aspect = aspect,
                TextColour = Color.White,
                XPos = itemX + (itemH/8f),
                YPos = itemY + (itemH/2f),
                ZPos = zdist + 0.01f,
                Scale = itemH * TextSizeScale
            };
            TextRenderer.DrawWithOptions(Text, tro);

            if (ovTexID != -1) {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texID);
                prog.SetUniform("textureEnabled", true);
                prog.SetUniform("texPos", ovTX, ovTY);
                prog.SetUniform("texScale", ovTW, ovTH);
                GL.BindTexture(TextureTarget.Texture2D, ovTexID);
                translateM = Matrix4.CreateTranslation(itemX + (itemW * ovX), itemY + (itemH * ovY), zdist + 0.001f);
                scaleM = Matrix4.CreateScale(itemW * ovW, itemH * ovH, 1f);
                modelM = scaleM * translateM;
                prog.SetUniform("model", modelM);
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Textured.BindAndDraw();
            }

            if (SubPanel != null) {
                throw new Exception("Text Panel shouldn't have sub panel");
            }
            if (piHover != null) DrawSelectionFrame(prog, itemX, itemY, itemW, itemH, zdist);
            return piHover;
        }
        public override void SetSubPanel(GUIPanel? gpl) {
            SubPanel = gpl;
        }
        public override void SetOverlay(TexSpecs ts, Vector4 dimRect) {
            ovTexID = ts.ID;
            ovTX = ts.X;
            ovTY = ts.Y;
            ovTW = ts.W;
            ovTH = ts.H;
            ovX = dimRect.X;
            ovY = dimRect.Y;
            ovW = dimRect.Z;
            ovH = dimRect.W;
        }
        public override float Width(float tw, float th, float aspect) {
            return measure.Width * th * TextSizeScale / (aspect * TextRenderer.FontSize);
        }
        public override float Height(float tw, float th) {
            return th;
        }
    }
}