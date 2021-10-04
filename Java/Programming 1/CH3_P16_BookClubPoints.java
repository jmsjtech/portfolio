import java.util.Scanner;

public class CH3_P16_BookClubPoints {
	public static void main(String args[]) {
		Scanner kb = new Scanner(System.in);
		
		
		System.out.print("How many books purchased this month? ");
		int books = kb.nextInt();
		
		if (books <= 0) {
			System.out.println("You earned 0 points.");
		} else if (books == 1) {
			System.out.println("You earned 5 points.");
		} else if (books == 2) {
			System.out.println("You earned 15 points.");
		} else if (books == 3) {
			System.out.println("You earned 30 points.");
		} else {
			System.out.println("You earned 60 points!");
		}
		
		
		kb.close();
	}

}
