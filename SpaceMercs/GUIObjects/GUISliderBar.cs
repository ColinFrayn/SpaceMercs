using OpenTK.Graphics.OpenGL;

namespace SpaceMercs {
  class GUISliderBar : GUIObject {
    public double BarX;
    public double BarY;
    public double BarHeight;
    public double BarLength;
    public double SlideFrac;
    public double ButtonGap;
    public double GapSz;
    public double ArrowSz;
    public double SlideGap;
    public bool DragBar {get; set;}
    double SlideWidth;
    double ButtonX;
    double ButtonY;
    double SlideX;
    double SlideSz;
    public double dMinValue = 0;
    public double dMaxValue = 0;
    public double dValue;
    public double BarStep;
    public bool bMarkZero;

    public GUISliderBar () {
      BarX = 0.0;
      BarY = 0.0;
      BarHeight = 0.07;
      BarLength = 0.3;
      SlideFrac = 0.15;
      ButtonGap = 0.04;
      GapSz = 0.006;
      ArrowSz = 0.03;
      SlideGap = 0.004;
      DragBar = false;
      Active = true;
      BarStep = 8.0;
      Alpha = 0.4f;
      bMarkZero = false;
    }

    // Display the slider bars
    public override void Display(int x, int y) {
      int WindowWidth = Window.Size.X;
      int WindowHeight = Window.Size.Y;
      double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;
      double xstart, dx, dy;

      if (!Active) return;

      // Set up transparency
      GL.Disable(EnableCap.Lighting);
      GL.Disable(EnableCap.Texture2D); 
      GL.Enable(EnableCap.Blend);
      GL.Disable(EnableCap.DepthTest);
      //GL.DepthMask(false);
      GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

      // Clamp to range
      if (dValue < dMinValue) dValue = dMinValue;
      if (dValue > dMaxValue) dValue = dMaxValue;

      // Draw the arrows
      if (xpos >= BarX && xpos <= BarX + ArrowSz && ypos >= BarY && ypos <= BarY + BarHeight) {
        // Now test to see if we're in the arrow itself
        dx = xpos - BarX;
        if (dx == 0.0f) dx = 0.0001f;
        dy = ypos - (BarY + BarHeight * 0.5f);
        if (Math.Abs(dy / dx) < ArrowSz / BarHeight) GL.Color4(0.9f, 0.9f, 0.9f, 0.9f);
        else GL.Color4(0.6f, 0.6f, 0.6f, Alpha);
      }
      else GL.Color4(0.6f, 0.6f, 0.6f, Alpha);
      GL.Begin(BeginMode.Triangles);
      GL.Vertex3(BarX, BarY + BarHeight * 0.5f, 0.0f);
      GL.Vertex3(BarX + ArrowSz, BarY, 0.0f);
      GL.Vertex3(BarX + ArrowSz, BarY + BarHeight, 0.0f);
      GL.End();

      if (xpos >= BarX + BarLength - ArrowSz && xpos <= BarX + BarLength && ypos >= BarY && ypos <= BarY + BarHeight) {
        // Now test to see if we're in the arrow itself
        dx = (BarX + BarLength) - xpos;
        if (dx == 0.0f) dx = 0.0001f;
        dy = ypos - (BarY + BarHeight * 0.5f);
        if (Math.Abs(dy / dx) < ArrowSz / BarHeight) GL.Color4(0.9f, 0.9f, 0.9f, 0.9f);
        else GL.Color4(0.6f, 0.6f, 0.6f, Alpha);
      }
      else GL.Color4(0.6f, 0.6f, 0.6f, Alpha);

      GL.Begin(BeginMode.Triangles);
      GL.Vertex3(BarX + BarLength, BarY + BarHeight * 0.5f, 0.0f);
      GL.Vertex3(BarX + BarLength - ArrowSz, BarY + BarHeight, 0.0f);
      GL.Vertex3(BarX + BarLength - ArrowSz, BarY, 0.0f);
      GL.End();

      // Draw the bar frame
      GL.Color4(0.6f, 0.6f, 0.6f, Alpha);
      GL.Begin(BeginMode.LineStrip);
      GL.Vertex3(SlideX, BarY, 0.0f);
      GL.Vertex3(SlideX, BarY + BarHeight, 0.0f);
      GL.Vertex3(SlideX + SlideWidth + SlideGap * 2.0f, BarY + BarHeight, 0.0f);
      GL.Vertex3(SlideX + SlideWidth + SlideGap * 2.0f, BarY, 0.0f);
      GL.Vertex3(SlideX, BarY, 0.0f);
      GL.End();
      if (bMarkZero && dMinValue <= 0.0 && dMaxValue >= 0.0) {
        double zx = SlideX + SlideGap + ((0.0 - dMinValue) / (dMaxValue - dMinValue)) * SlideWidth;
        GL.Begin(BeginMode.Lines);
        GL.Vertex3(zx, BarY, 0.0f);
        GL.Vertex3(zx, BarY - BarHeight / 4.0, 0.0f);
        GL.Vertex3(zx, BarY + BarHeight, 0.0f);
        GL.Vertex3(zx, BarY + BarHeight * 5.0 / 4.0, 0.0f);
        GL.End();

      }

      // Calculate bar position
      xstart = SlideX + (SlideWidth * (1.0f - SlideFrac) * ((dValue - dMinValue) / (dMaxValue - dMinValue))) + SlideGap;
      // Draw the bar itself
      if (DragBar || (xpos >= xstart && xpos <= (xstart + SlideSz) && ypos >= (BarY + SlideGap) && ypos <= (BarY + BarHeight - SlideGap))) GL.Color4(0.9f, 0.9f, 0.9f, 0.9f);
      else GL.Color4(0.6f, 0.6f, 0.6f, Alpha);
      GL.Begin(BeginMode.Quads);
      GL.Vertex3(xstart, BarY + SlideGap, 0.0f);
      GL.Vertex3(xstart + SlideSz, BarY + SlideGap, 0.0f);
      GL.Vertex3(xstart + SlideSz, BarY + BarHeight - SlideGap, 0.0f);
      GL.Vertex3(xstart, BarY + BarHeight - SlideGap, 0.0f);
      GL.End();
      GL.Enable(EnableCap.DepthTest);
    }

    // See if there's anything that needs to be done for the slider bar after a L-click
    public override bool CaptureClick(int x, int y) {
      if (!Active) return false;

      int WindowWidth = Window.Size.X;
      int WindowHeight = Window.Size.Y;
      double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;
      double dx, dy;

      // Are we hovering over the left arrow?
      // First check the bounding box
      if (xpos >= BarX && xpos <= BarX + ArrowSz && ypos >= BarY && ypos <= BarY + BarHeight) {
        // Now test to see if we're in the arrow itself
        dx = xpos - BarX;
        if (dx == 0.0f) dx = 0.0001f;
        dy = ypos - (BarY + BarHeight * 0.5f);
        if (Math.Abs(dy / dx) < ArrowSz / BarHeight) {
          dValue -= (dMaxValue - dMinValue) / BarStep;
          if (dValue < dMinValue) dValue = dMinValue;
          return true;
        }
      }

      // Are we hovering over the right arrow?
      if (xpos >= BarX + BarLength - ArrowSz && xpos <= BarX + BarLength && ypos >= BarY && ypos <= BarY + BarHeight) {
        // Now test to see if we're in the arrow itself
        dx = (BarX + BarLength) - xpos;
        if (dx == 0.0f) dx = 0.0001f;
        dy = ypos - (BarY + BarHeight * 0.5f);
        if (Math.Abs(dy / dx) < ArrowSz / BarHeight) {
          dValue += (dMaxValue - dMinValue) / BarStep;
          if (dValue > dMaxValue) dValue = dMaxValue;
          return true;
        }
      }
      if (DragBar) {
        DragBar = false;
        return true;
      }
      return false;
    }

    // See if there's anything that needs to be done for the slider bar after a L-click
    public void CaptureDrag(int x, int y) {
      if (!Active) return;

      int WindowWidth = Window.Size.X;
      int WindowHeight = Window.Size.Y;
      double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;
      double xstart;
      DragBar = false;

      // Are we hovering over the bar itself?
      xstart = SlideX + (SlideWidth * (1.0f - SlideFrac) * ((dValue - dMinValue) / (dMaxValue - dMinValue))) + SlideGap;
      if (xpos >= xstart && xpos <= (xstart + SlideSz) && ypos >= (BarY + SlideGap) && ypos <= (BarY + BarHeight - SlideGap)) {
        DragBar = true;
      }
    }

    // Track the slider bar
    public override void TrackMouse(int x, int y) {
      if (!DragBar || !Active) return;
      int WindowWidth = Window.Size.X;
      double xpos = (double)x / (double)WindowWidth;
      xpos -= (SlideX + SlideGap + SlideSz/2.0);
      xpos /= (SlideWidth * (1.0f - SlideFrac));
      xpos = xpos * (dMaxValue - dMinValue) + dMinValue;
      if (xpos < dMinValue) xpos = dMinValue;
      if (xpos > dMaxValue) xpos = dMaxValue;
      dValue = xpos;
    }

    // Set the default (calculated) values
    public override void Initialise() {
      ButtonX    = 1.0 - ButtonGap;
      ButtonY    = BarY + ButtonGap;
      SlideX     = BarX + ArrowSz + GapSz;
      SlideWidth = BarLength - 2.0*(ArrowSz + GapSz + SlideGap);
      SlideSz    = SlideWidth * SlideFrac;
    }

    // See if we're hovering over this bar
    public override bool IsHover(int x, int y) {
      if (!Active) return false;

      int WindowWidth = Window.Size.X;
      int WindowHeight = Window.Size.Y;
      double xpos = (double)x / (double)WindowWidth, ypos = (double)y / (double)WindowHeight;
      double xstart, dx, dy;

      // Are we hovering over the left arrow?
      // First check the bounding box
      if (xpos >= BarX && xpos <= BarX + ArrowSz && ypos >= BarY && ypos <= BarY + BarHeight) {
        // Now test to see if we're in the arrow itself
        dx = xpos - BarX;
        if (dx == 0.0f) dx = 0.0001f;
        dy = ypos - (BarY + BarHeight * 0.5f);
        if (Math.Abs(dy / dx) < ArrowSz / BarHeight) {
          return true;
        }
      }

      // Are we hovering over the right arrow?
      if (xpos >= BarX + BarLength - ArrowSz && xpos <= BarX + BarLength && ypos >= BarY && ypos <= BarY + BarHeight) {
        // Now test to see if we're in the arrow itself
        dx = (BarX + BarLength) - xpos;
        if (dx == 0.0f) dx = 0.0001f;
        dy = ypos - (BarY + BarHeight * 0.5f);
        if (Math.Abs(dy / dx) < ArrowSz / BarHeight) {
          return true;
        }
      }

      // Are we hovering over the bar itself?
      xstart = SlideX + (SlideWidth * (1.0f - SlideFrac) * ((dValue - dMinValue) / (dMaxValue - dMinValue))) + SlideGap;
      if (xpos >= xstart && xpos <= (xstart + SlideSz) && ypos >= (BarY + SlideGap) && ypos <= (BarY + BarHeight - SlideGap)) {
        return true;
      }
      return false;
    }
  }
}
