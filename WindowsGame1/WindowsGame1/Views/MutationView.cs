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

        static int[] proteins;
        static int[] strengthLevel;
        static int[] healthLevel;
        static int[] speedLevel;

        MenuEntry speedMutation;
        MenuEntry strengthMutation;
        MenuEntry healthMutation;
        MenuEntry backMenuEntry;
        MenuEntry proteinCountEntry;
        MenuEntry playerIndexEntry;


        public MutationView()
            : base("Mutation Menu")
        {

            proteins = new int[4] { 3, 3, 3, 3 };

            strengthLevel = new int[4] { 0, 0, 0, 0 };
            healthLevel = new int[4] { 0, 0, 0, 0 };
            speedLevel = new int[4] { 0, 0, 0, 0 };

            currentPlayer = 0;

            speedMutation = new MenuEntry("Speed");
            strengthMutation = new MenuEntry("Strength");
            healthMutation = new MenuEntry("Health");
            proteinCountEntry = new MenuEntry("Back");
            backMenuEntry = new MenuEntry("Back");
            playerIndexEntry = new MenuEntry("Current Player");

            SetMenuEntryText();

            // Hook up menu event handlers.
            playerIndexEntry.Selected += SwitchPlayer;
            speedMutation.Selected += BuySpeed;
            strengthMutation.Selected += BuyStrength;
            healthMutation.Selected += BuyHealth;
            backMenuEntry.Selected += BackToMain;

            // Add entries to the menu.
            MenuEntries.Add(playerIndexEntry);
            MenuEntries.Add(proteinCountEntry);
            MenuEntries.Add(speedMutation);
            MenuEntries.Add(strengthMutation);
            MenuEntries.Add(healthMutation);
            MenuEntries.Add(backMenuEntry);
        }

        void SetMenuEntryText()
        {
            playerIndexEntry.Text = "Current player: " + (currentPlayer+1);
            proteinCountEntry.Text = "Available proteins: " + proteins[currentPlayer];
            speedMutation.Text = "Speed: " + speedLevel[currentPlayer];
            healthMutation.Text = "Health: " + healthLevel[currentPlayer];
            strengthMutation.Text = "Strength: " + strengthLevel[currentPlayer];
        }

        // Upgrades virus speed
        void BuySpeed(object sender, PlayerIndexEventArgs e)
        {
            if (proteins[currentPlayer] > 0)
            {
                proteins[currentPlayer]--;
                speedLevel[currentPlayer]++;
                SetMenuEntryText();
            }
        }

        // Upgrades virus infection strength
        void BuyStrength(object sender, PlayerIndexEventArgs e)
        {
            if (proteins[currentPlayer] > 0)
            {
                proteins[currentPlayer]--;
                strengthLevel[currentPlayer]++;
                SetMenuEntryText();
            }
        }

        // Upgrades virus health
        void BuyHealth(object sender, PlayerIndexEventArgs e)
        {
            if (proteins[currentPlayer] > 0)
            {
                proteins[currentPlayer]--;
                healthLevel[currentPlayer]++;
                SetMenuEntryText();
            }
        }

        void SwitchPlayer(object sender, PlayerIndexEventArgs e)
        {
            if (currentPlayer < 3)
            {
                currentPlayer++;
            }
            else
            {
                currentPlayer = 0;
            }
            SetMenuEntryText();
        }

        // Returns to the main menu
        void BackToMain(object sender, PlayerIndexEventArgs e)
        {
            LoadingView.Load(ViewManager, false, null, new BackgroundView(), new MainMenuView());
        }
    }
}
