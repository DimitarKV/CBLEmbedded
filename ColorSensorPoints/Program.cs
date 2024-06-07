using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Schema;

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

    public override string? ToString()
    {
        return Red + ", " + Green + ", " + Blue;
    }
}
public class Program
{
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


    public static void RadiusFind(Point A, Point B)
    {
        string color;

        List<Point> dfBlackPoints = new List<Point>
        {
            new Point(2018, 3262, 2348),
            new Point(2051, 3293, 2351),
            new Point(1813, 2881, 2156),
            // Add more points as needed
        };

        List<double> anglesDegAP = dfBlackPoints.Select(p => AngleBetweenVectorAndPointAP(p, A, B)).ToList();
        List<double> anglesDegBP = dfBlackPoints.Select(p => AngleBetweenVectorAndPointBP(p, A, B)).ToList();

        var dfAnglesBlack = dfBlackPoints
            .Select((p, index) => new
            {
                Point = p,
                AP_Angle = anglesDegAP[index],
                BP_Angle = anglesDegBP[index]
            })
            .Where(x => x.AP_Angle <= 90 && x.BP_Angle <= 90)
            .ToList();

        foreach (var item in dfAnglesBlack)
        {
            Console.WriteLine($"Point(Red: {item.Point.Red}, Green: {item.Point.Green}, Blue: {item.Point.Blue}), AP-Angle: {item.AP_Angle}, BP-Angle: {item.BP_Angle}");
        }
    }

    public static Point ReadPointFromConsole()
    {
        string input = Console.ReadLine();
        double[] coords = input.Split(',').Select(double.Parse).ToArray();

        return new Point(coords[0], coords[1], coords[2]);
    }
    public static void Main()
    {
        Point Ablack = new Point(1600, 2600, 2000);
        Point Bblack = new Point(2400, 3600, 2650);

        Point Awhite = new Point(48000, 64800, 44000);
        Point Bwhite = new Point(66000, 66500, 61000);

        Point Aconveyor = new Point(0, 0, 0);
        Point Bconveyor = new Point(3100, 3600, 3600);

        var P = ReadPointFromConsole();

        //RadiusFind(Ablack, Bblack);
        //RadiusFind(Awhite, Bwhite);
        //RadiusFind(Aconveyor, Bconveyor);

    }
}
