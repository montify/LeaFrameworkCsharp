using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaFramework.PlayGround.Misc;
using SharpDX;

namespace LeaFramework.PlayGround.TerrainSrc
{
    public class QuadNodeExtents
    {
        public double West { get; }
        public double East { get; }
        public double North { get; }
        public double South { get; }
        public Vector3Double uVector { get; }
        public Vector3Double vVector { get; }
        public Vector3Double upVector { get; }
        public float Radius { get; set; }

        public QuadNodeExtents(float radius, double west, double east, double north, double south, Vector3Double uVector,
            Vector3Double vVector, Vector3 upVector)
        {
            this.West = west;
            this.East = east;
            this.North = north;
            this.South = south;
            this.uVector = uVector;
            this.vVector = vVector;
            this.upVector = upVector;
            this.Radius = radius;
        }

        public double Width => this.East - this.West;


        public List<QuadNodeExtents> Split()
        {
            return new List<QuadNodeExtents>()
            {
                new QuadNodeExtents(this.Radius, this.West, this.East - (this.Width*0.5f), this.North,
                    this.South - (this.Width*0.5f), uVector, vVector, upVector),
                new QuadNodeExtents(this.Radius, this.West + (this.Width*0.5f), this.East, this.North,
                    this.South - (this.Width*0.5f), uVector, vVector, upVector),
                new QuadNodeExtents(this.Radius, this.West, this.East - (this.Width *0.5f), this.North + (this.Width*0.5f),
                    this.South, uVector, vVector, upVector),
                new QuadNodeExtents(this.Radius, this.West + (this.Width*0.5f), this.East, this.North + (this.Width*0.5f),
                    this.South, uVector, vVector, upVector)
            };
        }
    }
}
