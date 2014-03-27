using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Virion
{
    class MutationView : MenuView
    {
        int currentPlayer;

        static int[] strengthLevel;
        static int[] healthLevel;
        static int[] speedLevel;

        MenuEntry speedMutation;
        MenuEntry strengthMutation;
        MenuEntry healthMutation;
        MenuEntry backMenuEntry;


        public MutationView()
            : base("Mutation Menu")
        {
            strengthLevel = new int[4] { 0, 0, 0, 0 };
            healthLevel = new int[4] { 0, 0, 0, 0 };
            speedLevel = new int[4] { 0, 0, 0, 0 };

            currentPlayer = 1;

            speedMutation = new MenuEntry("Speed");
            strengthMutation = new MenuEntry("Strength");
            healthMutation = new MenuEntry("Health");
            backMenuEntry = new MenuEntry("Back");

            SetMenuEntryText();

            // Hook up menu event handlers.
            speedMutation.Selected += BuySpeed;
            strengthMutation.Selected += BuyStrength;
            healthMutation.Selected += BuyHealth;
            backMenuEntry.Selected += BackToMain;

            // Add entries to the menu.
            MenuEntries.Add(speedMutation);
            MenuEntries.Add(strengthMutation);
            MenuEntries.Add(healthMutation);
            MenuEntries.Add(backMenuEntry);
        }

        void SetMenuEntryText()
        {
            speedMutation.Text = "Speed: " + string.Join("  ", speedLevel);
            healthMutation.Text = "Health: " + healthLevel;
            strengthMutation.Text = "Strength: " + strengthLevel;
        }

        // Upgrades virus speed
        void BuySpeed(object sender, PlayerIndexEventArgs e)
        {
            speedLevel[currentPlayer] += 1;
        }

        // Upgrades virus infection strength
        void BuyStrength(object sender, PlayerIndexEventArgs e)
        {
            strengthLevel[currentPlayer]++;
        }

        // Upgrades virus health
        void BuyHealth(object sender, PlayerIndexEventArgs e)
        {
            healthLevel[currentPlayer]++;
        }

        // Returns to the main menu
        void BackToMain(object sender, PlayerIndexEventArgs e)
        {
            LoadingView.Load(ViewManager, false, null, new BackgroundView(), new MainMenuView());
        }
    }
}
