using System;
using System.Globalization;

public static class FindSlope {
	public static void Main (string[] args) {
		int j;
		
		if (args[0] == "a") { // From Slope and Y-Intercept
			if (Int32.TryParse(args[2], out j)) {
				if (j < 0) {
					Console.WriteLine("y = "+ args[1] + "x - " + (j * -1));
				} else {
					Console.WriteLine("y = "+ args[1] + "x + " + j);
				}
			} 
		}
		
		else if (args[0] == "b") { // From Slope and Point
			float slope = float.Parse(args[1], CultureInfo.InvariantCulture.NumberFormat);
			float x = float.Parse(args[2].Split(',')[0], CultureInfo.InvariantCulture.NumberFormat);
			float y = float.Parse(args[2].Split(',')[1], CultureInfo.InvariantCulture.NumberFormat);
			
			FromPointAndSlope(x, y, slope);
		}
		
		else if (args[0] == "c") { // From Two Points
			float x1 = float.Parse(args[1].Split(',')[0], CultureInfo.InvariantCulture.NumberFormat);
			float y1 = float.Parse(args[1].Split(',')[1], CultureInfo.InvariantCulture.NumberFormat);
			float x2 = float.Parse(args[2].Split(',')[0], CultureInfo.InvariantCulture.NumberFormat);
			float y2 = float.Parse(args[2].Split(',')[1], CultureInfo.InvariantCulture.NumberFormat);
			
			FromPointAndSlope(x1, y1, (float)((y2-y1)/(x2-x1)));
		}
	}
	
	public static void FromPointAndSlope(float x, float y, float slope) {
		float new_y_intercept = (float)((slope * (0 - x)) + y);
		
		if (new_y_intercept < 0) {
			Console.WriteLine("y = " + slope + "x - " + (-1 * new_y_intercept) + " | (0," + new_y_intercept + ") and (" + (new_y_intercept * -1) + "/" + (slope) + ", 0)");
		} else if (new_y_intercept == 0) {
			Console.WriteLine("y = " + slope + "x | (0," + new_y_intercept + ") and (" + (new_y_intercept * -1) + "/" + (slope) + ", 0)");
		} else {
			Console.WriteLine("y = " + slope + "x + " + new_y_intercept + " | (0," + new_y_intercept + ") and (" + (new_y_intercept * -1) + "/" + (slope) + ", 0)");
		}
	}
}
