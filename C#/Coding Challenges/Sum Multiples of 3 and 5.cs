using System;

public static class SumMultiples35 {
	public static void Main (string[] args) {
		int j;
		
		if (Int32.TryParse(args[0], out j)) {
			if (j < 0) {
				Console.WriteLine("Enter a positive number.");
			} else if (j == 0) {
				Console.WriteLine("Zero is not a positive number.");
			} else {
				Console.WriteLine(Solution(j));
			}
		} else {
			Console.WriteLine("Enter a number.");
		}
	}


	public static int Solution(int value) {
		int sum = 0;
		for (int i = 0; i < value; i++) {
			if (i % 3 == 0 || i % 5 == 0) sum += i;
		}

		return sum;
	}
}