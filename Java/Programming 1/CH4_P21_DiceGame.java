import java.util.Random;

public class CH4_P21_DiceGame {
	public static void main(String args[]) {
		int playerWins = 0, computerWins = 0;
		Random rand = new Random();
		
		for (int i = 0; i < 10; i++) {
			int playerDie = rand.nextInt(6) + 1;
			int computerDie = rand.nextInt(6) + 1;
			
			if (playerDie > computerDie) { playerWins++; }
			if (playerDie < computerDie) { computerWins++; }
		}
		
		System.out.println("Final Score: Player " + playerWins + " - Computer " + computerWins + ".");
		
		if (playerWins > computerWins) { System.out.println("Player wins!"); }
		if (playerWins < computerWins) { System.out.println("Computer wins!"); }
		if (playerWins == computerWins) { System.out.println("It's a draw!"); }
	}

}
