using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LeaFramework.PlayGround.Misc
{
    public struct Vector3Double
    {
        public double X;
        public double Y;
        public double Z;

        static Vector3Double()
        {

            Zero = new Vector3Double();
            One = new Vector3Double(1f, 1f, 1f);
            UnitX = new Vector3Double(1f, 0f, 0f);
            UnitY = new Vector3Double(0f, 1f, 0f);
            UnitZ = new Vector3Double(0f, 0f, 1f);
            Up = new Vector3Double(0f, 1f, 0f);
            Down = new Vector3Double(0f, -1f, 0f);
            Right = new Vector3Double(1f, 0f, 0f);
            Left = new Vector3Double(-1f, 0f, 0f);
            Forward = new Vector3Double(0f, 0f, -1f);
            Backward = new Vector3Double(0f, 0f, 1f);
        }

        public Vector3Double(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3Double(double value)
        {
            X = Y = Z = value;
        }

        public static Vector3Double Zero { get; private set; }

        public static Vector3Double One { get; private set; }

        public static Vector3Double UnitX { get; private set; }

        public static Vector3Double UnitY { get; private set; }

        public static Vector3Double UnitZ { get; private set; }

        public static Vector3Double Up { get; private set; }

        public static Vector3Double Down { get; private set; }

        public static Vector3Double Right { get; private set; }

        public static Vector3Double Left { get; private set; }

        public static Vector3Double Forward { get; private set; }

        public static Vector3Double Backward { get; private set; }


        public bool Equals(Vector3Double other)
        {
            return (((X == other.X) && (Y == other.Y)) && (Z == other.Z));
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj is Vector3Double)
            {
                isEqual = Equals((Vector3Double)obj);
            }
            return isEqual;
        }

        public override int GetHashCode()
        {
            return ((X.GetHashCode() + Y.GetHashCode()) + Z.GetHashCode());
        }

        public double Length()
        {
            double lengthSquared = (X * X) + (Y * Y) + (Z * Z);
            return System.Math.Sqrt(lengthSquared);
        }

        public double LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        public static double Distance(Vector3Double value1, Vector3Double value2)
        {
            double xDistance = value1.X - value2.X;
            double yDistance = value1.Y - value2.Y;
            double zDistance = value1.Z - value2.Z;
            double distanceSquared = ((xDistance * xDistance) + (yDistance * yDistance)) + (zDistance * zDistance);
            return Math.Sqrt(distanceSquared);
        }

        public static double DistanceSquared(Vector3Double value1, Vector3Double value2)
        {
            double xDistance = value1.X - value2.X;
            double yDistance = value1.Y - value2.Y;
            double zDistance = value1.Z - value2.Z;
            double distanceSquared = ((xDistance * xDistance) + (yDistance * yDistance)) + (zDistance * zDistance);
            return distanceSquared;
        }

        public static double Dot(Vector3Double vector1, Vector3Double vector2)
        {
            return (vector1.X * vector2.X) + (vector1.Y * vector2.Y) + (vector1.Z * vector2.Z);
        }

        public void Normalize()
        {
            double lengthSquared = (X * X) + (Y * Y) + (Z * Z);
            double lengthReciprocal = 1.0 / Math.Sqrt(lengthSquared);

            X *= lengthReciprocal;
            Y *= lengthReciprocal;
            Z *= lengthReciprocal;
        }

        public static Vector3Double Normalize(Vector3Double value)
        {
            Vector3Double normalizedVector;
            double lengthSquared = (value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z);
            double lengthReciprocal = 1.0 / Math.Sqrt(lengthSquared);

            normalizedVector.X = value.X * lengthReciprocal;
            normalizedVector.Y = value.Y * lengthReciprocal;
            normalizedVector.Z = value.Z * lengthReciprocal;

            return normalizedVector;
        }

        public static Vector3Double Cross(Vector3Double vector1, Vector3Double vector2)
        {
            Vector3Double product;

            product.X = (vector1.Y * vector2.Z) - (vector1.Z * vector2.Y);
            product.Y = (vector1.Z * vector2.X) - (vector1.X * vector2.Z);
            product.Z = (vector1.X * vector2.Y) - (vector1.Y * vector2.X);

            return product;
        }

        public static Vector3Double Reflect(Vector3Double vector, Vector3Double normal)
        {
            double reflection = ((vector.X * normal.X) + (vector.Y * normal.Y)) + (vector.Z * normal.Z);

            Vector3Double reflectedVector;
            reflectedVector.X = vector.X - ((2.0 * reflection) * normal.X);
            reflectedVector.Y = vector.Y - ((2.0 * reflection) * normal.Y);
            reflectedVector.Z = vector.Z - ((2.0 * reflection) * normal.Z);

            return reflectedVector;
        }

        public static Vector3Double Min(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double minimumVector;
            minimumVector.X = (value1.X < value2.X) ? value1.X : value2.X;
            minimumVector.Y = (value1.Y < value2.Y) ? value1.Y : value2.Y;
            minimumVector.Z = (value1.Z < value2.Z) ? value1.Z : value2.Z;

            return minimumVector;
        }

        public static Vector3Double Max(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double maximumVector;
            maximumVector.X = (value1.X > value2.X) ? value1.X : value2.X;
            maximumVector.Y = (value1.Y > value2.Y) ? value1.Y : value2.Y;
            maximumVector.Z = (value1.Z > value2.Z) ? value1.Z : value2.Z;

            return maximumVector;
        }

        public static Vector3Double Clamp(Vector3Double value, Vector3Double min, Vector3Double max)
        {
            double x = value.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            double y = value.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            double z = value.Z;
            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            Vector3Double clampedVector;
            clampedVector.X = x;
            clampedVector.Y = y;
            clampedVector.Z = z;

            return clampedVector;
        }

        public static Vector3Double Lerp(Vector3Double value1, Vector3Double value2, double amount)
        {
            Vector3Double lerpedVector;
            lerpedVector.X = value1.X + (value2.X - value1.X) * amount;
            lerpedVector.Y = value1.Y + (value2.Y - value1.Y) * amount;
            lerpedVector.Z = value1.Z + (value2.Z - value1.Z) * amount;

            return lerpedVector;
        }

        public static Vector3Double Barycentric(Vector3Double value1, Vector3Double value2, Vector3Double value3, double amount1, double amount2)
        {
            Vector3Double coordinates;
            coordinates.X = value1.X + (amount1 * (value2.X - value1.X)) + (amount2 * (value3.X - value1.X));
            coordinates.Y = value1.Y + (amount1 * (value2.Y - value1.Y)) + (amount2 * (value3.Y - value1.Y));
            coordinates.Z = value1.Z + (amount1 * (value2.Z - value1.Z)) + (amount2 * (value3.Z - value1.Z));

            return coordinates;
        }

        public static Vector3Double SmoothStep(Vector3Double value1, Vector3Double value2, double amount)
        {
            amount = (amount > 1.0) ? 1.0 : ((amount < 0.0) ? 0.0 : amount);
            amount = (amount * amount) * (3.0 - (2.0 * amount));

            Vector3Double steppedVector;
            steppedVector.X = value1.X + ((value2.X - value1.X) * amount);
            steppedVector.Y = value1.Y + ((value2.Y - value1.Y) * amount);
            steppedVector.Z = value1.Z + ((value2.Z - value1.Z) * amount);

            return steppedVector;
        }

        public static Vector3Double CatmullRom(Vector3Double value1, Vector3Double value2, Vector3Double value3, Vector3Double value4, double amount)
        {
            double amountSquared = amount * amount;
            double amountCubed = amount * amountSquared;

            Vector3Double interpolatedVector;
            interpolatedVector.X = 0.5 * ((((2.0 * value2.X) + ((-value1.X + value3.X) * amount)) + (((((2.0 * value1.X) - (5.0 * value2.X)) + (4.0 * value3.X)) - value4.X) * amountSquared)) + ((((-value1.X + (3.0 * value2.X)) - (3.0 * value3.X)) + value4.X) * amountCubed));
            interpolatedVector.Y = 0.5 * ((((2.0 * value2.Y) + ((-value1.Y + value3.Y) * amount)) + (((((2.0 * value1.Y) - (5.0 * value2.Y)) + (4.0 * value3.Y)) - value4.Y) * amountSquared)) + ((((-value1.Y + (3.0 * value2.Y)) - (3.0 * value3.Y)) + value4.Y) * amountCubed));
            interpolatedVector.Z = 0.5 * ((((2.0 * value2.Z) + ((-value1.Z + value3.Z) * amount)) + (((((2.0 * value1.Z) - (5.0 * value2.Z)) + (4.0 * value3.Z)) - value4.Z) * amountSquared)) + ((((-value1.Z + (3.0 * value2.Z)) - (3.0 * value3.Z)) + value4.Z) * amountCubed));
            return interpolatedVector;
        }

        public static Vector3Double Hermite(Vector3Double value1, Vector3Double tangent1, Vector3Double value2, Vector3Double tangent2, double amount)
        {
            double amountSquared = amount * amount;
            double amountCubed = amount * amountSquared;
            double basis1 = (2.0 * amountCubed) - (3.0 * amountSquared) + 1.0;
            double basis2 = (-2.0 * amountCubed) + (3.0 * amountSquared);
            double basis3 = amountCubed - (2.0 * amountSquared) + amount;
            double basis4 = amountCubed - amountSquared;

            Vector3Double interpolatedVector;
            interpolatedVector.X = (((value1.X * basis1) + (value2.X * basis2)) + (tangent1.X * basis3)) + (tangent2.X * basis4);
            interpolatedVector.Y = (((value1.Y * basis1) + (value2.Y * basis2)) + (tangent1.Y * basis3)) + (tangent2.Y * basis4);
            interpolatedVector.Z = (((value1.Z * basis1) + (value2.Z * basis2)) + (tangent1.Z * basis3)) + (tangent2.Z * basis4);

            return interpolatedVector;
        }

        public static Vector3Double Transform(Vector3Double position, Matrix matrix)
        {
            double x = (((position.X * matrix.M11) + (position.Y * matrix.M21)) + (position.Z * matrix.M31)) + matrix.M41;
            double y = (((position.X * matrix.M12) + (position.Y * matrix.M22)) + (position.Z * matrix.M32)) + matrix.M42;
            double z = (((position.X * matrix.M13) + (position.Y * matrix.M23)) + (position.Z * matrix.M33)) + matrix.M43;

            Vector3Double transformedVector;
            transformedVector.X = x;
            transformedVector.Y = y;
            transformedVector.Z = z;

            return transformedVector;
        }

        public static Vector3Double TransformNormal(Vector3Double normal, Matrix matrix)
        {
            double x = ((normal.X * matrix.M11) + (normal.Y * matrix.M21)) + (normal.Z * matrix.M31);
            double y = ((normal.X * matrix.M12) + (normal.Y * matrix.M22)) + (normal.Z * matrix.M32);
            double z = ((normal.X * matrix.M13) + (normal.Y * matrix.M23)) + (normal.Z * matrix.M33);

            Vector3Double transformedVector;
            transformedVector.X = x;
            transformedVector.Y = y;
            transformedVector.Z = z;
            return transformedVector;
        }

        public static void Transform(Vector3Double[] sourceArray, ref Matrix matrix, Vector3Double[] destinationArray)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if (destinationArray.Length < sourceArray.Length)
            {
                throw new ArgumentException("Destination is not long enough.");
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                double x = sourceArray[i].X;
                double y = sourceArray[i].Y;
                double z = sourceArray[i].Z;

                destinationArray[i].X = (((x * matrix.M11) + (y * matrix.M21)) + (z * matrix.M31)) + matrix.M41;
                destinationArray[i].Y = (((x * matrix.M12) + (y * matrix.M22)) + (z * matrix.M32)) + matrix.M42;
                destinationArray[i].Z = (((x * matrix.M13) + (y * matrix.M23)) + (z * matrix.M33)) + matrix.M43;
            }
        }

        public static void Transform(Vector3Double[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3Double[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if (sourceArray.Length < (sourceIndex + length))
            {
                throw new ArgumentException("Source is not long enough.");
            }
            if (destinationArray.Length < (destinationIndex + length))
            {
                throw new ArgumentException("Target is not long enough.");
            }

            while (length > 0)
            {
                double x = sourceArray[sourceIndex].X;
                double y = sourceArray[sourceIndex].Y;
                double z = sourceArray[sourceIndex].Z;

                destinationArray[destinationIndex].X = (((x * matrix.M11) + (y * matrix.M21)) + (z * matrix.M31)) + matrix.M41;
                destinationArray[destinationIndex].Y = (((x * matrix.M12) + (y * matrix.M22)) + (z * matrix.M32)) + matrix.M42;
                destinationArray[destinationIndex].Z = (((x * matrix.M13) + (y * matrix.M23)) + (z * matrix.M33)) + matrix.M43;

                sourceIndex++;
                destinationIndex++;
                length--;
            }
        }

        public static void TransformNormal(Vector3Double[] sourceArray, ref Matrix matrix, Vector3Double[] destinationArray)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if (destinationArray.Length < sourceArray.Length)
            {
                throw new ArgumentException("Target is not long enough.");
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                double x = sourceArray[i].X;
                double y = sourceArray[i].Y;
                double z = sourceArray[i].Z;

                destinationArray[i].X = ((x * matrix.M11) + (y * matrix.M21)) + (z * matrix.M31);
                destinationArray[i].Y = ((x * matrix.M12) + (y * matrix.M22)) + (z * matrix.M32);
                destinationArray[i].Z = ((x * matrix.M13) + (y * matrix.M23)) + (z * matrix.M33);
            }
        }

        public static void TransformNormal(Vector3Double[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3Double[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if (sourceArray.Length < (sourceIndex + length))
            {
                throw new ArgumentException("Source is not long enough.");
            }
            if (destinationArray.Length < (destinationIndex + length))
            {
                throw new ArgumentException("Target is not long enough.");
            }

            while (length > 0)
            {
                double x = sourceArray[sourceIndex].X;
                double y = sourceArray[sourceIndex].Y;
                double z = sourceArray[sourceIndex].Z;

                destinationArray[destinationIndex].X = ((x * matrix.M11) + (y * matrix.M21)) + (z * matrix.M31);
                destinationArray[destinationIndex].Y = ((x * matrix.M12) + (y * matrix.M22)) + (z * matrix.M32);
                destinationArray[destinationIndex].Z = ((x * matrix.M13) + (y * matrix.M23)) + (z * matrix.M33);

                sourceIndex++;
                destinationIndex++;
                length--;
            }
        }

        public Vector3Double ProjectUnitPlaneToUnitSphere()
        {
            // http://mathproofs.blogspot.com/2005/07/mapping-cube-to-sphere.html
            Vector3Double sphereUnitVector;

            var xSquared = X * X;
            var ySquared = Y * Y;
            var zSquared = Z * Z;

            sphereUnitVector.X = X * Math.Sqrt(1.0 - (ySquared / 2) - (zSquared / 2) + (ySquared * zSquared / 3));
            sphereUnitVector.Y = Y * Math.Sqrt(1.0 - (zSquared / 2) - (xSquared / 2) + (zSquared * xSquared / 3));
            sphereUnitVector.Z = Z * Math.Sqrt(1.0 - (xSquared / 2) - (ySquared / 2) + (xSquared * ySquared / 3));

            return sphereUnitVector;
        }

        public static Vector3Double Negate(Vector3Double value)
        {
            Vector3Double negatedVector;
            negatedVector.X = -value.X;
            negatedVector.Y = -value.Y;
            negatedVector.Z = -value.Z;

            return negatedVector;
        }

        public static Vector3Double Add(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double sum;
            sum.X = value1.X + value2.X;
            sum.Y = value1.Y + value2.Y;
            sum.Z = value1.Z + value2.Z;

            return sum;
        }

        public static Vector3Double Subtract(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double difference;
            difference.X = value1.X - value2.X;
            difference.Y = value1.Y - value2.Y;
            difference.Z = value1.Z - value2.Z;

            return difference;
        }

        public static Vector3Double Multiply(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double product;
            product.X = value1.X * value2.X;
            product.Y = value1.Y * value2.Y;
            product.Z = value1.Z * value2.Z;

            return product;
        }

        public static Vector3Double Multiply(Vector3Double value1, double scaleFactor)
        {
            Vector3Double product;
            product.X = value1.X * scaleFactor;
            product.Y = value1.Y * scaleFactor;
            product.Z = value1.Z * scaleFactor;

            return product;
        }


        public static Vector3Double Divide(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double quotient;
            quotient.X = value1.X / value2.X;
            quotient.Y = value1.Y / value2.Y;
            quotient.Z = value1.Z / value2.Z;

            return quotient;
        }

        public static Vector3Double Divide(Vector3Double value1, double value2)
        {
            double reciprocal = 1.0 / value2;

            Vector3Double quotient;
            quotient.X = value1.X * reciprocal;
            quotient.Y = value1.Y * reciprocal;
            quotient.Z = value1.Z * reciprocal;

            return quotient;
        }

        public static Vector3Double operator -(Vector3Double value)
        {
            Vector3Double negatedVector;
            negatedVector.X = -value.X;
            negatedVector.Y = -value.Y;
            negatedVector.Z = -value.Z;

            return negatedVector;
        }

        public static bool operator ==(Vector3Double value1, Vector3Double value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(Vector3Double value1, Vector3Double value2)
        {
            return !value1.Equals(value2);
        }

        public static Vector3Double operator +(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double sum;
            sum.X = value1.X + value2.X;
            sum.Y = value1.Y + value2.Y;
            sum.Z = value1.Z + value2.Z;

            return sum;
        }

        public static Vector3Double operator -(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double difference;
            difference.X = value1.X - value2.X;
            difference.Y = value1.Y - value2.Y;
            difference.Z = value1.Z - value2.Z;

            return difference;
        }

        public static Vector3Double operator *(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double product;
            product.X = value1.X * value2.X;
            product.Y = value1.Y * value2.Y;
            product.Z = value1.Z * value2.Z;

            return product;
        }

        public static Vector3Double operator *(Vector3Double value, double scaleFactor)
        {
            Vector3Double product;
            product.X = value.X * scaleFactor;
            product.Y = value.Y * scaleFactor;
            product.Z = value.Z * scaleFactor;

            return product;
        }

        public static Vector3Double operator *(double scaleFactor, Vector3Double value)
        {
            Vector3Double product;
            product.X = value.X * scaleFactor;
            product.Y = value.Y * scaleFactor;
            product.Z = value.Z * scaleFactor;

            return product;
        }

        public static Vector3Double operator /(Vector3Double value1, Vector3Double value2)
        {
            Vector3Double quotient;
            quotient.X = value1.X / value2.X;
            quotient.Y = value1.Y / value2.Y;
            quotient.Z = value1.Z / value2.Z;

            return quotient;
        }

        public static Vector3Double operator /(Vector3Double value, double divider)
        {
            Vector3Double quotient;
            double reciprocal = 1.0 / divider;
            quotient.X = value.X * reciprocal;
            quotient.Y = value.Y * reciprocal;
            quotient.Z = value.Z * reciprocal;

            return quotient;
        }

        public static implicit operator Vector3(Vector3Double Vector3Double)
        {
            return new Vector3((float)Vector3Double.X, (float)Vector3Double.Y, (float)Vector3Double.Z);
        }

        public static implicit operator Vector3Double(Vector3 vector3)
        {
            return new Vector3Double(vector3.X, vector3.Y, vector3.Z);
        }

        public override string ToString()
        {
            return "X: " + X.ToString() +" Y: "  + Y.ToString() + " Z: "+ Z.ToString();

        }

        public Vector3 ToVector3(Vector3Double value1)
        {
            Vector3 tmp;

            tmp.X = (float)value1.X;
            tmp.Y = (float)value1.Y;
            tmp.Z = (float)value1.Z;

            return tmp;
        }

    }
}
