using System;
using System.Globalization;

public static class Parabola {
	public static void Main (string[] args) {
    float a = float.Parse(args[0]);
		float b = float.Parse(args[1]);
		float c = float.Parse(args[2]);
		
    float y_int = c;
    float vert_num = (-b)/(2*a);
    float vertex_y = (a*(vert_num*vert_num))+(b*vert_num)+c;
    
    string opens = "down";
    if (a > 0) { opens = "up";}
    
    Console.WriteLine("Parabola opens " + opens + ", Y-int: (0, " + y_int + "), Vertex: (" + vert_num + ", " + vertex_y + "), AoS: " + vert_num);
    
    Quadratic(a, b, c);
    
	}
  
  public static void FactorTrinomial(float t1, float t2, float t3) {
    float target = t1 * t3;
    float num1 = 0;
    float num2 = 0;
    
    for(int x = 0; x < target; x++) {
      for (int y = 0; y < target; y++) {
        if (x * y == target && x+y == t2) {
         num1 = x;
         num2 = y;
         break;
        }
      }
    }
    
    Console.WriteLine(num1 + ", " + num2);
  }
  
  public static void Quadratic(float a, float b, float c) {
    float sq = (b*b) - (4 *(a*c));
    if (sq == 0) {
      Console.WriteLine("X-Intercept: (" + -b + "/" + (2 *a) + ", 0)");
    } else {
      if (Math.Sqrt(sq) == (int) Math.Sqrt(sq)) {
        Console.WriteLine("X-Intercepts: (" + ((-b + Math.Sqrt(sq))/(2*a)) + ", 0), and (" + ((-b - Math.Sqrt(sq))/(2*a)) + ", 0)");
      } else {
        float bottom = 2 * a;
        
        Console.WriteLine("X-Intercepts: ((" + (-1 * b) + " + sqrt(" + sq + "))/" + bottom + ", 0), and ((" + (-1 * b) + " - sqrt(" + sq + "))/" + bottom + ", 0).");
      }
    }	
  }
}
