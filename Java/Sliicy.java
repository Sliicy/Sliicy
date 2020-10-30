import java.util.List;
import java.util.ArrayList;

public class Sliicy {

  // headerKeys stores the first line of values:
  private ArrayList<int> headerKeys;
  // bodyDictionary stores the entire list of wordlists found in the rest of the lines.
  // each wordlist contains a list of words per that category:
  private ArrayList<ArrayList<String>> bodyDictionary;

  private int numericPassword = 0;

  Sliicy(String userPassword) {

    // Set up the password:
    numericPassword = passwordToInt(userPassword);

    headerKeys[];

  }

  // Return whether a character is a letter:
  private static boolean isLetter(char c) {
      return (c >= 'a' && c <= 'z') ||
             (c >= 'A' && c <= 'Z');
  }

  // Sanitize input:
  private String sanitize(String input) {
    return "";
  }

  private int passwordToInt(String password) {
    // this method converts the alphanumeric password of the user into an integer
    int charCount = 1;
    int sumTotal = 0;

    for (char c : password) {
      sumTotal += (int) c * charCount;
      if (isLetter(c)) charCount++;
    }

    // introduce possibility of negative integers (to enhance security?):
    if (charCount % 2 > 0) sumTotal *= -1;

    return sumTotal * charCount;
  }

  /**
  * TODO add description
  *
  * @param  destinationSource The destination path to save the sender's file to
  * @param  destinationRecipient The destination path to save the recipient to
  * @return      the image at the specified URL
  * @see         joinChat
  */
  public void createChat(String destinationSource, String destinationRecipient) {

  }

  public String joinChat(String contentsOfContactFile) {

    String builder = "";

    ArrayList<String> arrayOfFields = new ArrayList<String>();
    arrayOfFields = Arrays.asList(contentsOfContactFile.split("\n\n"));

    for (int headerKey : arrayOfFields[0].split(';')) {
      builder += Integer.toString(headerKey + numericPassword) + ";";
    }

    // Remove trailing semi-colon:
    arrayOfFields[0] = builder.substring(0, builder.length() - 1);

    //fix
    headerKeys = arrayOfFields[0];
    for (int headerKey : arrayOfFields[0].split(';')) {
      headerKeys
      builder += Integer.toString(headerKey + numericPassword) + ";";

    }
    bodyDictionary = arrayOfFields[1 to N];

    List<Integer> oldList = ...
    /* Specify the size of the list up front to prevent resizing. */
    List<String> newList = new ArrayList<>(oldList.size());
    for (Integer myInt : oldList) {
      newList.add(String.valueOf(myInt));
    }

    return String.join("\n\n", arrayOfFields);
  }

  public String encrypt() {
    // Encrypt message
  }

  public String decrypt() {
    // Decrypt message
  }

  public void changePassword() {
    // Change the password used to locally encrypt the headers.
  }

  public ArrayList<ArrayList<String>> getLists() {
    // Returns all the lists in array form.
  }

  public ArrayList<int> getPlainHeaders() {
    // Return the first row of raw original numbers.
  }

  public ArrayList<int> getEncryptedHeaders() {
    // Return the first row of numbers fully encrypted with password.
  }

  public void setLists(ArrayList<ArrayList<String>> input) {
    // Sets the entire 2D list from an ArrayList.
  }

  public String toString() {
    // Return the entire list of lists as a single string ready to be saved to a file.
    // This should be the last thing called before disposing a Sliicy object, or else all changes are unsaved.
  }
}
