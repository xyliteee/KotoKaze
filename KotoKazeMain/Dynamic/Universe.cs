using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KotoKaze.Dynamic
{
    



    public class Universe
    {
        private static readonly double G = 6.67430e-11;

        public class Stellar(double mass, Double3 position)
        {
            public double mass = mass;
            public Double3 position = position;
            public Double3 velocity = new(0,0,0);
        }

        public static void GetTrackStep(Stellar star, Stellar[] planets, double dt)
        {
            foreach (var planet in planets)
            {
                Double3 force = new(0,0,0);
                foreach (Stellar otherPlanet in planets)
                {
                    if (otherPlanet != planet)
                    {
                        Double3 r = otherPlanet.position - planet.position;
                        double distance = r.Length();
                        force += G * otherPlanet.mass * planet.mass / (distance * distance * distance) * r;
                    }
                }

                Double3 rStar = star.position - planet.position;
                double distanceStar = rStar.Length();
                force += G * star.mass * planet.mass / (distanceStar * distanceStar * distanceStar) * rStar;
                Double3 acceleration = force/planet.mass;
                planet.velocity += acceleration * dt;
                planet.position += planet.velocity * dt;
            }
        }
    }
}
