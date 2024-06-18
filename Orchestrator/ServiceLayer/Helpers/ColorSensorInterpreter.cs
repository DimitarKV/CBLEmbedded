namespace ServiceLayer.Helpers;

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

public class ColorSensorInterpreter
{
    public List<Cylinder> ColorSpaces { get; set; } = new ()    
    {

        new Cylinder(new Point(1500, 2444, 1611),
            new Point(3000, 4370, 3342),
                1000, "black_disc"),
        new Cylinder(new Point(46000, 64800, 42000),
            new Point(66000, 66000, 66000),
            2500, "white_disc"),
        new Cylinder(new Point(40000, 50000, 32500),
            new Point(47500, 60000, 40000),
            2500, "white_disc"),
        new Cylinder(new Point(0, 0, 0),
            new Point(4000, 4000, 5000),
            1000, "empty")
    };

    public void AddColorSpace(Cylinder cylinder)
    {
        ColorSpaces.Add(cylinder);
    }
    
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
    public string ColorFind(Point P)
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
}