﻿using System;
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

public record Cylinder(Point A, Point B, int Radius, string ClassificationName);

public class Program
{
    public static List<Cylinder> ColorSpaces { get; set; } = new ()
    {
        new Cylinder(new Point(1600, 2600, 2000),
            new Point(2400, 3600, 2650),
            250, "black_disc"),
        new Cylinder(new Point(48000, 64800, 44000),
            new Point(66000, 66500, 61000),
            2500, "white_disc"),
        new Cylinder(new Point(0, 0, 0),
            new Point(3100, 3600, 3600),
            400, "empty")
    };
    

    /**
     * Method that finds the angle <PAB where AB is the line and P is our point
     */
    public static double AngleBetweenPointAndVector(Point P, Point A, Point B)
    {
        double[] AP = { P.Red - A.Red, P.Green - A.Green, P.Blue - A.Blue };
        double[] AB = { B.Red - A.Red, B.Green - A.Green, B.Blue - A.Blue };

        double dotProduct = DotProduct(AP, AB);
        double magAP = Magnitude(AP);
        double magAB = Magnitude(AB);

        double cosThetaAP = dotProduct / (magAP * magAB);
        double angleRadAP = Math.Acos(Math.Clamp(cosThetaAP, -1.0, 1.0));
        double angleDegAP = RadiansToDegrees(angleRadAP);

        return angleDegAP;
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
        double closestDistance = Double.MaxValue;
        Cylinder currentChoice = null;
        
        foreach (var colorSpace in ColorSpaces)
        {
            double angleDegAP = AngleBetweenPointAndVector(P, colorSpace.A, colorSpace.B);
            double angleDegBP = AngleBetweenPointAndVector(P, colorSpace.B, colorSpace.A);
            if (angleDegAP <= 90 && angleDegBP <= 90)
            {
                double distanceToSpace = PointToLineDistance(P, colorSpace.A, colorSpace.B);
                if (distanceToSpace < closestDistance)
                {
                    currentChoice = colorSpace;
                    closestDistance = distanceToSpace;
                }   
            }
        }

        if (currentChoice is not null)
            return currentChoice.ClassificationName;

        return "none";
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