using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Ecliptic.World
{
    public class Orbit
    {
        //for now, main reference on kepler equation :
        //https://www.youtube.com/watch?v=obRLUC_o_HQ

        //Orbit parameters
        public CelestialBody mainCenter; //at position 0, 0, 0
        public Vector3 offCenter; //position of the second center (main center is (0, 0))
        public Vector3 orbitNormal;

        //orbit parameters
        public float semiMajorAxis;
        public float averageAngularSpeed;
        public float eccentricity;
        public float worldTimeAtPeriapsis;

        
        public Orbit(Vector3 position, Vector3 velocity)
        {
            //compute trajectory from initial pos and vel
        }

        public Orbit(CelestialBody mainCenter, Vector3 offCenter, float semiMajorAxis, int parentMass)
        {
            //create an orbit trajectory from given parameters
            this.mainCenter = mainCenter;
            this.offCenter = offCenter;
            this.semiMajorAxis = semiMajorAxis;
            averageAngularSpeed = 5; // MathF.Sqrt(GameConstants.G * parentMass / (semiMajorAxis * semiMajorAxis * semiMajorAxis));
            //eccentricity is c / a
            eccentricity = (Vector3.Distance(mainCenter.position, offCenter) / 2) / semiMajorAxis;
        }

        public Vector3 GetPositionAt(float t)
        {
            //returns the position of the object along the ellipse trajectory
            float meanAnomaly = (averageAngularSpeed * (t - worldTimeAtPeriapsis)) % (2 * MathF.PI);
            float eccentricAnomaly = NewtonSolveKeplerEquation(meanAnomaly, eccentricity);
            //from eccentric anomaly, get theta and r :
            float theta = 2 * MathF.Atan(MathF.Sqrt((1 + eccentricity) / (1 - eccentricity)) * MathF.Tan(eccentricAnomaly / 2));
            float r = (semiMajorAxis * (1 - eccentricity * eccentricity)) / (1 + eccentricity * MathF.Cos(theta));
            //get the position relative to the parent from r, theta, normal, offcenter
            //for now : normal is up, to simplify
            Vector3 result = new Vector3(r * MathF.Cos(theta), 0, r * MathF.Sin(theta));
            return result;
        }

        private float NewtonSolveKeplerEquation(float M, float e, float minError = 0.1f)
        {
            //we solve for E the equation M = E - e sin(E)
            //start at M
            float E = M;
            //iterate until we have a close enought solution
            while(MathF.Abs(M - E + e * MathF.Sin(E)) > minError)
            {
                E = M + e * MathF.Sin(E); //E - (E - e * MathF.Sin(E) - M) / (1 - e * MathF.Cos(e));
            }
            //return result
            return E;
        }

    }
}
