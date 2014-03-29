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

        MenuEntry speedMutation;
        MenuEntry strengthMutation;
        MenuEntry healthMutation;
        MenuEntry backMenuEntry;
        MenuEntry proteinCountEntry;
        MenuEntry playerIndexEntry;


        public MutationView()
            : base("Mutation Menu")
        {

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
            proteinCountEntry.Text = "Available proteins: " + Main.Instance.players[currentPlayer].Proteins;
            speedMutation.Text = "Speed: " + Main.Instance.players[currentPlayer].Speed;
            healthMutation.Text = "Health: " + Main.Instance.players[currentPlayer].Health;
            strengthMutation.Text = "Strength: " + Main.Instance.players[currentPlayer].Strength;
        }

        // Upgrades virus speed
        void BuySpeed(object sender, PlayerIndexEventArgs e)
        {
            if (Main.Instance.players[currentPlayer].Proteins > 0)
            {
                Main.Instance.players[currentPlayer].Proteins--;
                Main.Instance.players[currentPlayer].Speed++;
                SetMenuEntryText();
            }
        }

        // Upgrades virus infection strength
        void BuyStrength(object sender, PlayerIndexEventArgs e)
        {
            if (Main.Instance.players[currentPlayer].Proteins > 0)
            {
                Main.Instance.players[currentPlayer].Proteins--;
                Main.Instance.players[currentPlayer].Strength++;
                SetMenuEntryText();
            }
        }

        // Upgrades virus health
        void BuyHealth(object sender, PlayerIndexEventArgs e)
        {
            if (Main.Instance.players[currentPlayer].Proteins > 0)
            {
                Main.Instance.players[currentPlayer].Proteins--;
                Main.Instance.players[currentPlayer].Health++;
                SetMenuEntryText();
            }
        }

        void SwitchPlayer(object sender, PlayerIndexEventArgs e)
        {
            if (currentPlayer < Main.Instance.playerCount - 1)
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
