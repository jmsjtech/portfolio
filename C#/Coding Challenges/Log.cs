using System;
using System.Globalization;

public static class Log {
	public static void Main (string[] args) {
    double a = 0;
    double b = 0;
    double c = 0;
    double t = 0;
    
    int decimals = int.Parse(args[1]);
    
    if (args[2] == "pi") { a = Math.PI; } else if (args[2] == "-pi") { a = Math.PI * -1; }
    else if (args[2].Contains("/")) {
      int x = int.Parse(args[2].Split('/')[0]);
      int y = int.Parse(args[2].Split('/')[1]);
      a = (float) x / y;
    } else {
      a = double.Parse(args[2]);
    }
    
    if (args.Length > 3) {
      if (args[3] == "pi") { b = Math.PI; } else if (args[3] == "-pi") { b = Math.PI * -1; }
      else if (args[3].Contains("/")) {
        int x = int.Parse(args[3].Split('/')[0]);
        int y = int.Parse(args[3].Split('/')[1]);
        b = (double) x / y;
      } else {
        b = double.Parse(args[3]);
      }
    }
    
    if (args.Length > 4) {
      if (args[4] == "pi") { c = Math.PI; } else if (args[4] == "-pi") { c = Math.PI * -1; }
      if (args[4].Contains("/")) {
        int x = int.Parse(args[4].Split('/')[0]);
        int y = int.Parse(args[4].Split('/')[1]);
        c = (double) x / y;
      } else {
        c = double.Parse(args[4]);
      }
    }
    
    if (args.Length > 5) {
      if (args[5] == "pi") { c = Math.PI; } else if (args[5] == "-pi") { c = Math.PI * -1; }
      else if (args[5].Contains("/")) {
        int x = int.Parse(args[5].Split('/')[0]);
        int y = int.Parse(args[5].Split('/')[1]);
        t = (double) x / y;
      } else {
        t = double.Parse(args[5]);
      }
    }
    
    if (args[0] == "a") { // f(x) = a^b
      Console.WriteLine(Math.Round(Math.Pow(a, b), decimals));
    }
    
    if (args[0] == "b") { // f(x) = e^a
      Console.WriteLine(Math.Round(Math.Exp(a), decimals));
    }
    
    if (args[0] == "c") { // f(x) = a*e^(b*c)
      Console.WriteLine(Math.Round(a * Math.Exp((b * c)), decimals));
    }
    
		
    if (args[0] == "d") { // Interest - Compounded Incrementally. a = Principal, b = interest rate in decimal, c = compoundings per year, t = years
      double interest = 1 + (b / c);
      double totalChange = Math.Pow(interest, (c * t));
      
      Console.WriteLine(Math.Round(a * totalChange, decimals));
    }
    
    if (args[0] == "e") { // Interest - Compounded Continually. a = Principal, b = interest rate in decimal, c = years
      Console.WriteLine(Math.Round(a * Math.Exp(b*c), decimals));
    }
    
    if (args[0] == "f") { // Base 10 Log
      Console.WriteLine(Math.Round(Math.Log10(a), decimals));
    }
    
    if (args[0] == "g") { // Natural Log fx(x) = ln(a)
      Console.WriteLine(Math.Round(Math.Log(a), decimals));
    }
	}

}
