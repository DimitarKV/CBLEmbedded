using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Xml.Schema;
/**
 * Class Point for the points
 * 
 */
public class Point
{
    public double Red { get; set; }
    public double Green { get; set; }
    public double Blue { get; set; }

    public Point(double red, double green, double blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    /**
     * Mai pozvolqva da se izvajdat tochki.
     */
    public static Point operator -(Point p1, Point p2)
    {
        return new Point(p1.Red - p2.Red, p1.Green - p2.Green, p1.Blue - p2.Blue);
    }

    public static double[] ToArray(Point p)
    {
        return new double[] { p.Red, p.Green, p.Blue };
    }

    public override string? ToString()
    {
        return Red + ", " + Green + ", " + Blue;
    }
}
public class Program
{
    /**
     * Method that finds the angle <PAB where AB is the line and P is our point
     */
    public static double AngleBetweenVectorAndPointAP(Point P, Point A, Point B)
    {
        double[] AP = { P.Red - A.Red, P.Green - A.Green, P.Blue - A.Blue };
        double[] AB = { B.Red - A.Red, B.Green - A.Green, B.Blue - A.Blue };

        double dotProductAP = DotProduct(AP, AB);
        double magAP = Magnitude(AP);
        double magAB = Magnitude(AB);

        double cosThetaAP = dotProductAP / (magAP * magAB);
        double angleRadAP = Math.Acos(Math.Clamp(cosThetaAP, -1.0, 1.0));
        double angleDegAP = RadiansToDegrees(angleRadAP);

        return angleDegAP;
    }

    /**
     * Method that finds the angle <PBA where AB is the line and P is our point
     */
    public static double AngleBetweenVectorAndPointBP(Point P, Point A, Point B)
    {
        double[] AB = { B.Red - A.Red, B.Green - A.Green, B.Blue - A.Blue };
        double[] BP = { B.Red - P.Red, B.Green - P.Green, B.Blue - P.Blue };

        double dotProductBP = DotProduct(BP, AB);
        double magAB = Magnitude(AB);
        double magBP = Magnitude(BP);

        double cosThetaBP = dotProductBP / (magBP * magAB);
        double angleRadBP = Math.Acos(Math.Clamp(cosThetaBP, -1.0, 1.0));
        double angleDegBP = RadiansToDegrees(angleRadBP);

        return angleDegBP;
    }

    public static double DotProduct(double[] vec1, double[] vec2)
    {
        return vec1.Zip(vec2, (a, b) => a * b).Sum();
    }

    public static double Magnitude(double[] vec)
    {
        return Math.Sqrt(vec.Select(v => v * v).Sum());
    }

    public static double RadiansToDegrees(double radians)
    {
        return Math.Round(radians * (180.0 / Math.PI), 2);
    }

    /**
     * Finds the perpendicular, the distance from our point to the line.
     * 
     * @param P - our point
     * @param A - first point of the line
     * @param B - second point of the line
     * 
     */

    public static double PointToLineDistance(Point P, Point A, Point B)
    {
        Point AP = P - A;
        Point AB = B - A;

        double[] crossProduct = CrossProduct(Point.ToArray(AP), Point.ToArray(AB));
        double distance = Magnitude(crossProduct) / Magnitude(Point.ToArray(AB));

        return distance;
    }

    public static double[] CrossProduct(double[] vec1, double[] vec2)
    {
        return new double[]
        {
            vec1[1] * vec2[2] - vec1[2] * vec2[1],
            vec1[2] * vec2[0] - vec1[0] * vec2[2],
            vec1[0] * vec2[1] - vec1[1] * vec2[0]
        };
    }

    /**
     * Main method for derermining the color
     * 
     * da, znam che ne e robust xd
     * 
     */
    public static string ColorFind(Point P)
    {
        Point Ablack = new Point(1600, 2600, 2000);
        Point Bblack = new Point(2400, 3600, 2650);
        int rBlack = 250;

        Point Awhite = new Point(48000, 64800, 44000);
        Point Bwhite = new Point(66000, 66500, 61000);
        int rWhite = 2500;

        Point Aconveyor = new Point(0, 0, 0);
        Point Bconveyor = new Point(3100, 3600, 3600);
        int rConveyor = 400;

        string color = "";

        double distBlack = PointToLineDistance(P, Ablack, Bblack);
        double distWhite = PointToLineDistance(P, Awhite, Bwhite);
        double distConveyor = PointToLineDistance(P, Aconveyor, Bconveyor);

        if ((distBlack < distWhite) && (distBlack < distConveyor) && (distBlack <= rBlack))
        {
            double angleDegAPblack = AngleBetweenVectorAndPointAP(P, Ablack, Bblack);
            double angleDegBPblack = AngleBetweenVectorAndPointBP(P, Ablack, Bblack);

            if (angleDegAPblack <= 90 && angleDegBPblack <= 90)
            {
                color = "black";
            }

            else
            {
                color = "other";
            }
        }

        else if ((distWhite < distBlack) && (distWhite < distConveyor) && (distWhite <= rWhite))
        {
            double angleDegAPwhite = AngleBetweenVectorAndPointAP(P, Awhite, Bwhite);
            double angleDegBPwhite = AngleBetweenVectorAndPointBP(P, Awhite, Bwhite);

            if (angleDegAPwhite <= 90 && angleDegBPwhite <= 90)
            {
                color = "white";
            }

            else
            {
                color = "other";
            }
        }

        else if ((distConveyor < distBlack) && (distConveyor < distWhite) && (distConveyor <= rConveyor))
        {
            double angleDegAPconveyor = AngleBetweenVectorAndPointAP(P, Aconveyor, Bconveyor);
            double angleDegBPconveyor = AngleBetweenVectorAndPointBP(P, Aconveyor, Bconveyor);

            if (angleDegAPconveyor <= 90 && angleDegBPconveyor <= 90)
            {
                color = "conveyor";
            }

            else
            {
                color = "other";
            }
        }              
        return color;
    }

    /**
     * Veroqtno shte go mahnem ama e polezen za testvane sega
     */
    public static Point ReadPointFromConsole()
    {
        Console.WriteLine($"Enter the coordinates for point, (separated by commas): ");
        string input = Console.ReadLine();
        double[] coords = input.Split(',').Select(double.Parse).ToArray();

        return new Point(coords[0], coords[1], coords[2]);
    }

    public static void Main()
    {
        Point P = ReadPointFromConsole();

        string diskColor = ColorFind(P);
        Console.WriteLine(diskColor);

    }
}
