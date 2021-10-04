import java.util.Scanner;

public class CH3_P18_RestaurantSelector {
	public static void main(String args[]) {
		Scanner kb = new Scanner(System.in);
		
		int mask = 0;
		
		System.out.print("Is anyone in your party vegetarian? ");
		if (kb.nextLine().charAt(0) == 'y') { mask += 1; }
		
		System.out.print("Is anyone in your party vegan? ");
		if (kb.nextLine().charAt(0) == 'y') { mask += 2; }
		
		System.out.print("Is anyone in your party gluten-free? ");
		if (kb.nextLine().charAt(0) == 'y') { mask += 4; }
		
		if (mask == 0) { System.out.println("Joe's Gourmet Burgers"); }
		if (mask <= 5) { System.out.println("Main Street Pizza Company"); }
		if (mask <= 7) { System.out.println("Corner Cafe"); }
		if (mask <= 1) { System.out.println("Mama's Fine Italian"); }
		if (mask <= 7) { System.out.println("The Chef's Kitchen"); }
		
		
		kb.close();
	}

}
