import java.io.*;
import java.util.Scanner;


public class CH4_P23_PersonalWebPage {
	public static void main(String args[]) throws IOException {
		File file = new File("index.html");
		
		Scanner kb = new Scanner(System.in);
		
		System.out.print("Enter your name: ");
		String name = kb.nextLine();
		
		System.out.print("Describe yourself: ");
		String desc = kb.nextLine();
		
		PrintWriter outputFile = new PrintWriter(file);
		
		outputFile.println("<html>");
		outputFile.println("<head>\n</head>");
		outputFile.println("<body>");
		outputFile.println("\t<center>");
		outputFile.println("\t\t<h1>" + name + "</h1>");
		outputFile.println("\t</center>");		
		outputFile.println("\t<hr />");
		outputFile.println("\t" + desc);
		outputFile.println("\t<hr />");
		outputFile.println("</body>");
		outputFile.println("</html>");	
		
		outputFile.close();
		kb.close();
	}

}
