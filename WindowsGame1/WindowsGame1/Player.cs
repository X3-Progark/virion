using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virion
{
    public class Player
    {
        private int proteins;
        private int strengthLevel;
        private int healthLevel;
        private int speedLevel;

        private string model;
        private string name;

        public int Proteins
        {
            get { return this.proteins; }
            set { this.proteins = value; }
        }

        public int Strength
        {
            get { return this.strengthLevel; }
            set { this.strengthLevel = value; }
        }

        public int Health
        {
            get { return this.healthLevel; }
            set { this.healthLevel = value; }
        }

        public int Speed
        {
            get { return this.speedLevel; }
            set { this.speedLevel = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Model
        {
            get { return this.model; }
            set { this.model = value; }
        }



        public Player(string name)
        {
            this.name = name;

            proteins = 3;
            strengthLevel = 1;
            healthLevel = 1;
            speedLevel = 1;
        }

        public void buyStrength()
        {
            if (proteins > 0)
            {
                proteins--;
                strengthLevel++;
            }
        }

        public void buyHealth()
        {
            if (proteins > 0)
            {
                proteins--;
                healthLevel++;
            }
        }

        public void buySpeed()
        {
            if (proteins > 0)
            {
                proteins--;
                speedLevel++;
            }
        }
    }
}
