import javax.swing.JOptionPane;


public class Pizza_Part1 {
	public static void main(String args[]) {
		int num = Integer.parseInt(JOptionPane.showInputDialog("How many pizzas do you want?"));
		
		double cost = (num * Math.PI);
		double tax = cost * 0.1;
		double total = cost + tax;
		
		JOptionPane.showMessageDialog(null, "The pizzas cost " + cost + "\nTax is " + tax + "\nTotal cost: " + total);
		
		
		System.exit(0);
	}

}
