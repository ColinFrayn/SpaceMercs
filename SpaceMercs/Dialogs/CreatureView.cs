using System;

namespace SpaceMercs.Dialogs {
    public partial class CreatureView : Form {
        private readonly int locX, locY;

        internal CreatureView(IEntity ent, int X, int Y) {
            InitializeComponent();
            locX = X + 10;
            locY = Y + 10;
            this.Text = ent.Name;

            // Display primary stats
            lbLevel.Text = ent.Level.ToString();
            lbAttack.Text = ent.Attack.ToString();
            lbDefence.Text = ent.Defence.ToString();
            lbArmour.Text = ent.BaseArmour.ToString();
            lbHealth.Text = ent.Health.ToString();
            lbStamina.Text = ent.Stamina.ToString();
            if (Math.Abs(ent.Shred) > 0.01) {
                lbShred.Visible = true;
                lbShred.Text = $"-{ent.Shred:N2} shred";
            }
            else lbShred.Visible = false;
            if (ent.MaxShields > 0) {
                lbShields.Visible = true;
                label4.Visible = true;
                lbShields.Text = ent.Shields.ToString();
            }
            else {
                lbShields.Visible = false;
                label4.Visible = false;
            }

            // Display all effects
            lbEffects.Items.Clear();
            if (!ent.Effects.Any()) {
                lbEffects.Items.Add("No Active Effects");
            }
            else {
                foreach (Effect e in ent.Effects) {
                    string str = e.Name + " [" + e.Duration + " turn" + ((e.Duration == 1) ? "" : "s") + "]";
                    lbEffects.Items.Add(str);
                }
            }

            Load += new EventHandler(CreatureView_Load);
        }

        private void CreatureView_Load(object? sender, EventArgs e) {
            this.SetDesktopLocation(locX, locY);
        }
    }
}
