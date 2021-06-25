using System;
using System.Globalization;

public static class FactorTrinomial {
	public static void Main (string[] args) {
    int t1 = int.Parse(args[1]);
		int t2 = int.Parse(args[2]);
		int t3 = int.Parse(args[3]);
		
		if (args[0] == "a") { // Nice Trinomial
			
		}
		
		else if (args[0] == "b") { // Not So Nice Trinomial
			 int target = t1 * t3;
			 int num1 = 0;
			 int num2 = 0;
			 
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
		
		 
	}

}
