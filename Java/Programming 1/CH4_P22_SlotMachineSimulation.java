import java.util.Random;
import java.util.Scanner;

public class CH4_P22_SlotMachineSimulation {
	public static void main(String args[]) {
		int moneySpent = 0, moneyWon = 0;
		String[] symbols = { "Cherries", "Oranges", "Plums", "Bells", "Melons", "Bars" };
		Scanner kb = new Scanner(System.in);
		Random rand = new Random();
		
		
		while (true) {
			System.out.print("How much would you like to bet? ");
			int currentBet = kb.nextInt();
			moneySpent += currentBet;
			
			int matches = 0;
			
			String slot1 = symbols[rand.nextInt(symbols.length)];
			String slot2 = symbols[rand.nextInt(symbols.length)];
			String slot3 = symbols[rand.nextInt(symbols.length)];
			
			System.out.println(slot1 + " | " + slot2 + " | " + slot3);
			
			if (slot1 == slot2) { matches++; }
			if (slot1 == slot3) { matches++; }
			if (slot2 == slot3) { matches++; }
			
			if (matches == 0) { System.out.println("No matches, you lose!"); }
			if (matches == 1) { 
				System.out.println("You won " + (currentBet * 2) + "!");
				moneyWon += currentBet * 2;
			} if (matches >= 2) { 
				System.out.println("You won " + (currentBet * 3) + "!");
				moneyWon += currentBet * 3;
			}
			
			System.out.print("Play again? ");
			String response = kb.nextLine();
			response = kb.nextLine();
			if (response.charAt(0) == 'n') {
				break;
			}
		}
		
		System.out.println("You spent " + moneySpent + ", and won " + moneyWon + "!");
		
		kb.close();
	}

}
