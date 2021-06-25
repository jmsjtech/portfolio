using System;
using System.Globalization;

public static class Quadratic {
	public static void Main (string[] args) {
    float a = float.Parse(args[0]);
		float b = float.Parse(args[1]);
		float c = float.Parse(args[2]);
		
    float sq = (b*b) - (4 *(a*c));
    if (sq == 0) {
      Console.WriteLine("One answer: " + -b + "/" + (2 *a));
    } else {
      if (Math.Sqrt(sq) == (int) Math.Sqrt(sq)) {
        Console.WriteLine("Two answers: " + ((-b + Math.Sqrt(sq))/(2*a)) + ", and " + ((-b - Math.Sqrt(sq))/(2*a)));
      } else {
        float bottom = 2 * a;
        
        Console.WriteLine("Two answers: (" + (-1 * b) + " + sqrt(" + sq + "))/" + bottom + ", and (" + (-1 * b) + " - sqrt(" + sq + "))/" + bottom + ".");
      }
    }	 
	}
}
