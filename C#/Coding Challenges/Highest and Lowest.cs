using System;

public static class HighestLowest {
	public static void Main (string[] args) {
		int[] nums = Array.ConvertAll<string, int>(args, Int32.Parse);
		
		Console.WriteLine(Solution(nums));
	}


	public static string Solution(int[] nums) {
		int lowest = Int32.MaxValue;
		int highest = Int32.MinValue;

		for (int i = 0; i < nums.Length; i++) {
			if (nums[i] < lowest) lowest = nums[i];
			if (nums[i] > highest) highest = nums[i];
		}

		return highest + " " + lowest;
	}
}