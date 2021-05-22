using System;

public static class SpinningWords {
	public static void Main (string[] args) {
		string combined = "";
		
		for (int i = 0; i < args.Length; i++) {
			combined += args[i];
			
			if (i != args.Length - 1) combined += " ";
		}
		
		Console.WriteLine(Solution(combined));
	}

	public static string Solution(string sentence) {
		string[] words = sentence.Split();
		string output = "";

		for (int i = 0; i < words.Length; i++) {
			char[] charArray = words[i].ToCharArray();
			Array.Reverse(charArray);

			if (words[i].Length > 5) output += new string (charArray);
			else output += words[i];

			if (i != words.Length-1) output += " ";
		}

		return output;
	}
}