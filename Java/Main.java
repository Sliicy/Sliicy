import java.util.Scanner;

public class Main {
  public static void main(String[] args) {

    // This class demonstrates usage of Sliicy Encryption.


    Sliicy sliicy = new Sliicy("password");

    // Load file:


    // Creating new conversation:
    //System.out.println("Results of createChat:");

    //remove when ready to test create function:
    //sliicy.loadChat("-268072;-268071;-268070\n\nfrog\nmice\ncat\n\ncome\nwalk\nbike\nsled");

    //System.out.println(sliicy.createChat("Sliicy\nWalmart\nBest-Buy\n\nfrog\nmice\ncat\n\ncome\nwalk\nbike\nsled\n\n"));

    // Joining a conversation:
    System.out.println("Numerical Password value: " + sliicy.numericPassword);
    sliicy.joinChat("1;2;3\n\na\nb\nc\nd\ne\nf\ng\nh\ni\n\nfrog\nmice\ncat\n\ncome\nwalk\nbike\nsled");
    //System.out.println("Encrypted tostring:");
    System.out.println(sliicy.toString());
    sliicy.changePassword("bob!");
    System.out.println("Numerical Password value: " + sliicy.numericPassword);
    System.out.println(sliicy.toString());
    sliicy.changePassword("password");
    System.out.println("Numerical Password value: " + sliicy.numericPassword);
    System.out.println(sliicy.toString());
    System.out.println("-----------------------------------");
    System.out.println(sliicy.encrypt("an old dog im like theyre everybodys"));
    System.out.println(sliicy.decrypt("a new dog -17"));




    //System.out.println("Now attempting to join encrypted contact:");
    //sliicy.loadChat("-268072;-268071;-268070\n\nfrog\nmice\ncat\n\ncome\nwalk\nbike\nsled");
    //System.out.println("Decrypted Headers:");

    // Sending a message:

    // Receiving a message:

    // Changing the stored password:

  }
}
