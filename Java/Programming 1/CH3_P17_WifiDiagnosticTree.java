import java.util.Scanner;

public class CH3_P17_WifiDiagnosticTree {
	public static void main(String args[]) {
		Scanner kb = new Scanner(System.in);
		
		
		System.out.println("Reboot the computer and try to connect.");
		System.out.print("Did that fix the problem? ");
		char response = kb.nextLine().charAt(0);
		
		if (response == 'n') {
			System.out.println("Reboot the router and try to connect.");
			System.out.print("Did that fix the problem? ");
			response = kb.nextLine().charAt(0);
			
			if (response == 'n') {
				System.out.println("Make sure the cables between the router and the modem are plugged in firmly.");
				System.out.print("Did that fix the problem? ");
				response =  kb.nextLine().charAt(0);
				
				if (response == 'n') {
					System.out.println("Move the router to a new location and try to connect.");
					System.out.print("Did that fix the problem? ");
					response = kb.nextLine().charAt(0);
					
					if (response == 'n') {
						System.out.println("Get a new router.");
					}
				}
			}
		}		
		
		kb.close();
	}

}
