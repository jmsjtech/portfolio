package montyHallProof;

import java.util.Random;

public class main {

	public static void main(String[] args) {
		int correctDoor = 0;
		int numberCorrect = 0;
		int numberCorrect2 = 0;
		int numberTries = 10000;
		int firstChoice = 0;
		int secondChoice = 0;
		Random rn = new Random();
		int n = 3 - 1 + 1;
		boolean door1 = false, door2 = false, door3 = false;
		boolean door1Open = false, door2Open = false, door3Open = false;
		
		
		for (int i = 0; i < numberTries; i++) {
			correctDoor  = (rn.nextInt() % 4);
			
			while (correctDoor < 1 | correctDoor > 3) {
				correctDoor  = (rn.nextInt() % 4);
			}
			
			switch (correctDoor) {
				case 1:
					door1 = true;
					break;
				case 2:
					door2 = true;
					break;
				case 3:
					door3 = true;
					break;
				default:
					i--;
					break;
			}
			
			firstChoice = rn.nextInt() % 4;
			
			while (firstChoice < 1 | firstChoice > 3) {
				firstChoice = rn.nextInt() % 4;
			}

			 if (firstChoice == 1) {
				 if (door2 == false) {
					secondChoice = 3;
				 }
				 
				 else {
					 secondChoice = 2;
				 }
			 }
			 
			 else if (firstChoice == 2) {
				 if (door1 == false) {
					 secondChoice = 3;
				 }
				 else {
					 secondChoice = 1;
				 }
			 }
			 
			 else if (firstChoice == 3) {
				 if (door1 == false) {
					 secondChoice = 2;
				 }
				 
				 else {
					 secondChoice = 1;
				 }
			 }
			 
			 if (secondChoice == correctDoor) { numberCorrect++; }
			 if (firstChoice == correctDoor) { numberCorrect2++; }
			 
		}
		
		float solution = numberCorrect*100/numberTries;
		float solution2 = numberCorrect2*100/numberTries;
		
		System.out.println("When changing: " + solution);
		System.out.println("When not changing: " + solution2);
	}
}
