public class Main {
  public static void main(String[] args) {

    // This class demonstrates usage of Sliicy Encryption.


    Sliicy sliicy = new Sliicy("user's own password");

    // Load file:


    // Creating new conversation:
    //sliicy.createChat("destination sender path with filename", "destination receiver path with filename", );

    // Joining a conversation:
    System.out.println(sliicy.joinChat("1;2;3\n\nfrog\nmice"));
    // Sending a message:

    // Receiving a message:

    // Changing the stored password:





  }
}
