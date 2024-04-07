using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceMercs.Graphics;
using SpaceMercs.Graphics.Shapes;

namespace SpaceMercs {
    // Common graphics elements
    static class GraphicsFunctions {
        // A fractional bar
        public static void DisplayBicolourFractBar(ShaderProgram prog, float fTLCX, float fTLCY, float fWidth, float fHeight, float fract, Vector4 col, Vector4 bg) {
            if (fract < 0f) fract = 0f;
            if (fract > 1f) fract = 1f;
            GL.Disable(EnableCap.Blend);
            Matrix4 translateM = Matrix4.CreateTranslation(fTLCX, fTLCY, 0f);
            Matrix4 scaleM = Matrix4.CreateScale(fWidth, fHeight, 1f);
            if (fract < 1f) {
                prog.SetUniform("model", scaleM * translateM);
                prog.SetUniform("flatColour", bg);
                GL.UseProgram(prog.ShaderProgramHandle);
                Square.Flat.BindAndDraw();
            }
            scaleM = Matrix4.CreateScale(fWidth * fract, fHeight, 1f);
            prog.SetUniform("model", scaleM * translateM);
            prog.SetUniform("flatColour", col);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();
        }
        public static void DrawFramedFractBar(ShaderProgram prog, float fTLCX, float fTLCY, float fWidth, float fHeight, float fract, Vector4 col) {
            if (fract < 0f) fract = 0f;
            if (fract > 1f) fract = 1f;
            GL.Disable(EnableCap.Blend);
            Matrix4 translateM = Matrix4.CreateTranslation(fTLCX, fTLCY, 0f);
            Matrix4 scaleM = Matrix4.CreateScale(fWidth * fract, fHeight, 1f);
            prog.SetUniform("model", scaleM * translateM);
            prog.SetUniform("flatColour", col);
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Flat.BindAndDraw();
            scaleM = Matrix4.CreateScale(fWidth, fHeight, 1f);
            prog.SetUniform("model", scaleM * translateM);
            prog.SetUniform("flatColour", new Vector4(1f, 1f, 1f, 1.0f));
            GL.UseProgram(prog.ShaderProgramHandle);
            Square.Lines.BindAndDraw();
        }
    }
}
