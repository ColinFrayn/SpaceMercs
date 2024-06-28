namespace SpaceMercs.Graphics {
    public struct TexSpecs {
        public float X, Y, W, H;
        public int ID;

        public TexSpecs(int id, float x, float y, float w, float h) {
            ID = id;
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
