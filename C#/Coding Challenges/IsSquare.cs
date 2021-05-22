using System;

public static class IsSquare {
	public static void Main (string[] args) {
		int j = 0;
		
		if (Int32.TryParse(args[0], out j)) {
			Console.WriteLine(Solution(j));
		} else {
			Console.WriteLine("Enter a number.");
		}
	}

	public static bool Solution(int n) {
		if (n < 0) { return false; }
		if (n == 0 || n == 1 || n == 4 || n == 9) { return true; }


		int i = 0;
		if (n % 2 != 0) { i++; }

		for (i = i; i < n / 3; i += 2) {
			if (i * i == n) { return true; }
		}

		return false;
	}
}