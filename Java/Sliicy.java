import java.util.List;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Random;
import java.util.Collections;
import java.util.regex.*;

public class Sliicy {

  // Constants used throughout:
  final int NUMBERS = 0;
  final int DECIMALS = 1;
  final int IPV4 = 2;
  final int IPV6 = 6;
  final int NUM_SIGN = 14;
  final int SYMBOL_SIGN = 26;
  final int LETTER_SIGN = 59;
  final int COUNT_ORDER = 111;
  final int NUM_SIGN_ORDER = COUNT_ORDER;
  final int COUNT_TOTAL = 367;
  final int COUNT_SYMBOL = 623;
  final int TOTAL = 654;
  final int TYPES = TOTAL + 1;

  final int MAX_ALLOWED_WORDS = 256;
  final int MAX_ALLOWED_PUNCTUATION = 32;

  // headerKeys stores the first line of values:
  private ArrayList<Integer> headerKeys;

  // bodyDictionary stores the entire list of wordlists found in the rest of the lines (excludes headerKeys).
  // each wordlist contains a list of words per that category:
  private ArrayList<ArrayList<String>> bodyDictionary;

  // Numerical value of password (which is added to all headerKeys):
  protected int numericPassword = 0;

  Sliicy(String userPassword) {

    // Set up the password:
    numericPassword = passwordToInt(userPassword);

    // Initialize the arrays:
    resetKeys();
  }

  // Return whether a character is a letter:
  private static boolean isLetter(char c) {
      return (c >= 'a' && c <= 'z') ||
             (c >= 'A' && c <= 'Z');
  }

  // Return a string with specified amount trimmed from the end:
  private static String trimEnd(String input, int amount) {
    return input.substring(0, input.length() - amount);
  }

  // Shuffle characters of string into an arraylist of strings:
  private ArrayList<String> shuffleChars(String input) {
    List<Character> characters = new ArrayList<Character>();
    for (char c : input.toCharArray()) {
        characters.add(c);
    }
    ArrayList<String> output = new ArrayList<String>();
    while (characters.size() != 0) {
        int randPicker = (int) (Math.random() * characters.size());
        output.add(characters.remove(randPicker).toString());
    }
    return output;
  }

  // Remove whitespace from input:
  private String trimWhitespace(String input) {
    input.trim();
    while (input.contains("  ")) {
      input = input.replace("  ", " ");
    }
    input = input.replace("’", "'").replace("‘", "'").replace('“', '"').replace('”', '"');
    return input;
  }

  // Method to split numbers from letters (3D -> 3 D)
  private String separateLettersFromNumbers(String input) {
    input = input.replaceAll("([A-z])(\\d)","$1 $2");
    input = input.replaceAll("(\\d)([A-z])","$1 $2");
    //input = StringUtils.replacePattern(input, "(\\d)\\1{14}", "$1");
    return input;
  }

  // TODO Unknown why needed:
  private String removeDotsfromNumbers(String input) {
    if (input.contains(".")) {
      int redFlag = 0, characterCount = 0;
      ArrayList<String> locationOfDot = new ArrayList<String>();
      for (int i = 0; i < input.length(); i++) {
        char c = input.charAt(i);
        if (c == '.' && redFlag == 1 && characterCount == input.length() - 1) {
          locationOfDot.add(Integer.toString(characterCount));
          redFlag = 0;
        } else if (Character.isDigit(c)) {
          redFlag = 1;
        } else if (c == '.' && redFlag == 1) {
          redFlag = 2;
        } else if (c == '.' && redFlag == 2) {
          locationOfDot.add(Integer.toString(characterCount - 1));
          redFlag = 0;
        } else if (!Character.isDigit(c) && c != '.' && redFlag == 2) {
          locationOfDot.add(Integer.toString(characterCount - 1));
          redFlag = 0;
        } else if (!Character.isDigit(c) && c != '.') {
          redFlag = 0;
        }
        characterCount++;
      }
      if (locationOfDot.size() > 0) {
        for (int i = locationOfDot.size() - 1; i == 0 ; i--) {
          input = new StringBuilder(input).insert(input.length() - 1, " ").toString();
        }
      }
    }
    return input;
  }

  private int passwordToInt(String password) {
    // this method converts the alphanumeric password of the user into an integer
    int charCount = 1;
    int sumTotal = 0;
    char[] passwordChars = password.toCharArray();

    for (char c : passwordChars) {
      sumTotal += (int) c * charCount;
      if (isLetter(c)) charCount++;
    }

    // introduce possibility of negative integers to enhance security:
    if (charCount % 2 > 0) sumTotal *= -1;

    return sumTotal * charCount;
  }

  private static boolean isNumeric(String strNum) {
      if (strNum == null) {
          return false;
      }
      try {
          double d = Double.parseDouble(strNum);
      } catch (NumberFormatException nfe) {
          return false;
      }
      return true;
  }

  private boolean isVowel(char letter) {
    letter = Character.toLowerCase(letter);
    System.out.println("NAY:" + letter);
    return letter == 'a' || letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u' ? true : false;
  }

  private static final String IPV4_REGEX =
                      "^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\." +
                      "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\." +
                      "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\." +
                      "(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

  private static final String IPV6_REGEX =
                      "(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|" +
                      "([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:)" +
                      "{1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}" +
                      "(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}" +
                      "(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}" +
                      "(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}" +
                      "(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4})" +
                      "{1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4})" +
                      "{0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|" +
                      "(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}" +
                      "[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}" +
                      "[0-9]){0,1}[0-9])\\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))";

  private static final Pattern IPv4_PATTERN = Pattern.compile(IPV4_REGEX);
  private static final Pattern IPv6_PATTERN = Pattern.compile(IPV6_REGEX);

  private boolean validIPv4(String input) {
    if (input == null) {
        return false;
    }
    Matcher matcher = IPv4_PATTERN.matcher(input);
    return matcher.matches();
  }

  private boolean validIPv6(String input) {
    if (input == null) {
        return false;
    }
    Matcher matcher = IPv6_PATTERN.matcher(input);
    return matcher.matches();
  }

  private String autocorrect(String input) {
    if (input.toLowerCase().substring(0,2) == "a " && isVowel(input.substring(2,1).charAt(0))) {
      input = new StringBuilder(input).insert(1, "n").toString();
    }

    return input;
  }

  private String apostrophize(String input) {
    if (input.toLowerCase() == "an") input = "a";
    switch (input.length()) {
      case 2:
        input = input.toLowerCase().replace("im", "I'm");
        break;
      case 3:
        input = input.toLowerCase().replace("ive", "I've");
        input = input.toLowerCase().replace("hed", "he'd");
        input = input.toLowerCase().replace("hes", "he's");
        input = input.toLowerCase().replace("itd", "it'd");
        break;
      case 4:
        input = input.toLowerCase().replace("aint", "ain't");
        input = input.toLowerCase().replace("cant", "can't");
        input = input.toLowerCase().replace("dont", "don't");
        input = input.toLowerCase().replace("isnt", "isn't");
        input = input.toLowerCase().replace("itll", "it'll");
        input = input.toLowerCase().replace("lets", "let's");
        input = input.toLowerCase().replace("shes", "she's");
        input = input.toLowerCase().replace("weve", "we've");
        input = input.toLowerCase().replace("whos", "who's");
        input = input.toLowerCase().replace("wont", "won't");
        input = input.toLowerCase().replace("yall", "y'all");
        input = input.toLowerCase().replace("youd", "you'd");
        break;
      case 5:
        input = input.toLowerCase().replace("arent", "aren't");
        input = input.toLowerCase().replace("didnt", "didn't");
        input = input.toLowerCase().replace("hadnt", "hadn't");
        input = input.toLowerCase().replace("hasnt", "hasn't");
        input = input.toLowerCase().replace("thats", "that's");
        input = input.toLowerCase().replace("theyd", "they'd");
        input = input.toLowerCase().replace("wasnt", "wasn't");
        input = input.toLowerCase().replace("youll", "you'll");
        input = input.toLowerCase().replace("youre", "you're");
        input = input.toLowerCase().replace("youve", "you've");
        break;
      case 6:
        input = input.toLowerCase().replace("doesnt", "doesn't");
        input = input.toLowerCase().replace("havent", "haven't");
        input = input.toLowerCase().replace("mustve", "must've");
        input = input.toLowerCase().replace("mustnt", "mustn't");
        input = input.toLowerCase().replace("theres", "there's");
        input = input.toLowerCase().replace("theyll", "they'll");
        input = input.toLowerCase().replace("theyre", "they're");
        input = input.toLowerCase().replace("theyve", "they've");
        input = input.toLowerCase().replace("werent", "weren't");
        input = input.toLowerCase().replace("wheres", "where's");
        break;
      case 7:
        input = input.toLowerCase().replace("couldve", "could've");
        input = input.toLowerCase().replace("couldnt", "couldn't");
        input = input.toLowerCase().replace("mightve", "might've");
        input = input.toLowerCase().replace("nobodys", "nobody's");
        input = input.toLowerCase().replace("wouldve", "would've");
        input = input.toLowerCase().replace("wouldnt", "wouldn't");
        break;
      case 8:
        input = input.toLowerCase().replace("shouldve", "should've");
        input = input.toLowerCase().replace("shouldnt", "shouldn't");
        input = input.toLowerCase().replace("someones", "someone's");
        break;
      case 9:
        input = input.toLowerCase().replace("everyones", "everyone's");
        input = input.toLowerCase().replace("somebodys", "somebody's");
        break;
      case 10:
        input = input.toLowerCase().replace("everybodys", "everybody's");
        break;
    }
    return input;
  }
  // Convert a 2-Dimensional string delimited by \n\n and then by \n to an ArrayList:
  private ArrayList<ArrayList<String>> twoDimStringTo2DArrayList(String twoDimString) {
    ArrayList<ArrayList<String>> twoDimList = new ArrayList<ArrayList<String>>();
    ArrayList<String> listOfLists = new ArrayList<String>();
    for (String subSection : twoDimString.split("\n\n")) {
      listOfLists.add(subSection);
    }
    for (int i = 0; i < listOfLists.size(); i++) {
      ArrayList<String> subList = new ArrayList<String>();
      for (String subString : listOfLists.get(i).split("\n")) {
        subList.add(subString);
      }
      twoDimList.add(subList);
    }
    return twoDimList;
  }

  /**
  * createChat will generate a bunch of random numbers, create a small array
  * of characters from a-Z, using the masterDictionary as a word-template to
  * generate a list of shuffled wordlists. The last wordlist will always be pronouns.
  * This method effectively loads a new conversation into memory, as well as returns
  * the recipient file.
  * @param  masterDictionary The destination path to save the sender's file to
  * @return      String to be saved and sent to recipient securely
  * @see         joinChat
  */
  public String createChat(String masterDictionary) {

    // Generate random numbers:
    ArrayList<ArrayList<String>> dictionaryList = twoDimStringTo2DArrayList(masterDictionary);
    for (int i = 0; i < dictionaryList.size() + COUNT_TOTAL + 10000; i++) {
      Random r = new Random();
      int low = -100000;
      int high = 100000;
      int result = r.nextInt(high - low) + low;

      // Prevent zeros in the headerKeys:
      while (result == 0) result = r.nextInt(high - low) + low;
      headerKeys.add(result);
    }

    // Insert A-z randomly into body:
    bodyDictionary.add(shuffleChars("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-"));

    String shuffledDictionary = "jake\npirate\n\nbean\ncake";

    // Shuffle all lists of words:
    Collections.shuffle(dictionaryList);

    // Shuffle each wordlist individually:
    for (ArrayList<String> list : dictionaryList) {
      Collections.shuffle(list);
    }

    // Move pronoun wordlist to end of all lists:
    for (int i = 0; i < dictionaryList.size(); i++) {
      if (dictionaryList.get(i).contains("Sliicy")) {
        dictionaryList.add(dictionaryList.size(), dictionaryList.get(i));

        // Remove from current position:
        dictionaryList.remove(i);
        break;
      }
    }

    for (ArrayList<String> list : dictionaryList) {
      bodyDictionary.add(list);
    }

    // Representation of recipient without a password:
    Sliicy recipientSliicy = new Sliicy("");

    // Insert plaintext representation of newly created info into recipient object:
    recipientSliicy.joinChat(toString(false));
    return recipientSliicy.toString();
  }

  public void loadChat(String encryptedSliicyFile) {
    joinChat(encryptedSliicyFile, false);
  }

  public void joinChat(String contentsOfContactFile) {
    joinChat(contentsOfContactFile, true);
  }

  /**
  * joinChat receives an existing recipient file, and commits the
  * headerKeys from the first line, as well as the bodyDictionary
  * from the rest of the file, to memory. It returns nothing.
  * joinChat also functions as a loadChat method, optionally decrypting the file.
  * @param  contentsOfContactFile Contents of recipient file
  * @param  firstTimeJoining Boolean to determine if headerKeys need to be decrypted or not
  * @see         loadChat createChat
  */
  private void joinChat(String contentsOfContactFile, boolean firstTimeJoining) {
    resetKeys();

    // If encrypted, this will reverse all keys based on numericPassword:
    int numericPasswordifEncrypted = (firstTimeJoining) ? 0 : numericPassword;

    ArrayList<String> listOfLists = new ArrayList<String>();
    for (String subSection : contentsOfContactFile.split("\n\n")) {
      listOfLists.add(subSection);
    }

    for (String headerKey : listOfLists.get(0).split(";")) {
      headerKeys.add(Integer.parseInt(headerKey) + numericPasswordifEncrypted * -1);
    }

    // Start from 1 to skip the headerKeys:
    for (int i = 1; i < listOfLists.size(); i++) {
      ArrayList<String> subList = new ArrayList<String>();
      for (String subString : listOfLists.get(i).split("\n")) {
        subList.add(subString);
      }
      bodyDictionary.add(subList);
    }
  }

  private void resetKeys() {
    headerKeys = new ArrayList<Integer>();
    bodyDictionary = new ArrayList<ArrayList<String>>();
  }

  public void changePassword(String newPassword) {
    // Change the password used to locally encrypt the headerKeys:
    numericPassword = passwordToInt(newPassword);
  }

  public String toString() {
    // Return the entire list of lists as an encrypted (by default) single string ready to be saved to a file.
    // This should be the last thing called before disposing a Sliicy object, or else all changes are unsaved.
    // Used for creating a new chat.
    return toString(true);
  }

  private String toString(boolean encrypted) {
    // Return the entire list of lists as a single string ready to be saved to a file.
    // This should be the last thing called before disposing a Sliicy object, or else all changes are unsaved.
    int numericPasswordifEncrypted = (encrypted) ? numericPassword : 0;

    String builder = "";
    for (int header : headerKeys) {
      builder += Integer.toString(header + numericPasswordifEncrypted) + ";";
    }
    builder = trimEnd(builder, 1) + "\n\n";

    for (ArrayList<String> chunkofLines : bodyDictionary) {
      for (String listOfWords : chunkofLines) {
        builder += listOfWords + "\n";
      }
      builder = trimEnd(builder, 1);
      builder += "\n\n";
    }
    builder = trimEnd(builder, 2);
    return builder;
  }

  public String encrypt(String message) {
    return encrypt(message, false);
  }

  public String decrypt(String message) {
    return encrypt(message, true);
  }

  private String encrypt(String message, boolean decrypt) {
    /*
    Requirements:

    message must have minimum 1 word

    return -1 if failed
    */

    // Message must have at least 1 word:
    if (!Pattern.compile("[a-zA-Z]").matcher(message).find()) {
        return "-1"; // TODO throw an exception
    }

    message = trimWhitespace(message);
    message = separateLettersFromNumbers(message);
    message = removeDotsfromNumbers(message);

    // Retrieve letters from dictionary:
    ArrayList<String> letterList = new ArrayList<String>(bodyDictionary.get(0));

    int signature = 0;
    int verifySignature = 0;

    // Trim off signature from message:
    if (decrypt) {
      String firstHalf = message.substring(0, message.lastIndexOf(" "));
      verifySignature = Integer.parseInt(message.replace(firstHalf + " ", ""));
      message = firstHalf;
    }

    boolean flipPositiveNegative = true;

    if (message.length() - message.replace(" ", "").length() + 1 > MAX_ALLOWED_WORDS) {
      return "-2"; // TODO throw an exception
    }

    int countOrder = COUNT_ORDER;
    int numSignOrder = NUM_SIGN_ORDER;
    int countTotal = COUNT_TOTAL;

    ArrayList<String> punctuationSymbolList = new ArrayList<String>(Arrays.asList(".", ",", ";", "!", "?", "(", ")", "+", "="));

    int totalNumAllSymbols = 0;
    for (String symbol : punctuationSymbolList) {
      totalNumAllSymbols += message.length() - message.replace(symbol, "").length();
    }

    // Cap the maximum to MAX_ALLOWED_PUNCTUATION:
    if (totalNumAllSymbols > MAX_ALLOWED_PUNCTUATION) totalNumAllSymbols = MAX_ALLOWED_PUNCTUATION;


    // TODO add URL to IPv4 conversion


    ArrayList<String> words = new ArrayList<String>(Arrays.asList(message.split(" |~|!|@|\\#|\\$|%|\\^|&|\\*|\\(|\\)|_|=|\\+|,|<|>|/|\\?|;|\"|\\[|\\{|]|}|\\|\\||\\n")));

    // Holds outputted message:
    String output = "";

    for (String word : words) {
      if (isNumeric(word) || (decrypt && word.contains(".") && word.substring(word.length() - 1) == "-")) {
        // TODO Handle numbers
      } else if (validIPv4(word) && !word.contains(":")) {
        // TODO Handle IPv4
      } else if (validIPv6(word) && word.contains(":")) {
        // TODO Handle IPv6
      } else {
        // Handle anything else

        // Correct spelling mistakes:
        word = apostrophize(word);

        // Holds whether the word was found yet in the wordlists:
        boolean found = false;

        boolean temporaryValue = true;
        while (temporaryValue) {

        }
        if (!found) {
          if (word.length() > 1) {

          } else {
            temporaryValue = true;
            temporaryloop:
            while (temporaryValue) {
              int temp = 0;
              int charTemp = 0;
              switch (word) {
                case "`":
                  charTemp = 0;
                  break;
                case "~":
                  charTemp = 1;
                  break;
                case "!":
                  charTemp = 2;
                  break;
                case "@":
                  charTemp = 3;
                  break;
                case "#":
                  charTemp = 4;
                  break;
                case "$":
                  charTemp = 5;
                  break;
                case "%":
                  charTemp = 6;
                  break;
                case "^":
                  charTemp = 7;
                  break;
                case "&":
                  charTemp = 8;
                  break;
                case "*":
                  charTemp = 9;
                  break;
                case "(":
                  charTemp = 10;
                  break;
                case ")":
                  charTemp = 11;
                  break;
                case "_":
                  charTemp = 12;
                  break;
                case "+":
                  charTemp = 13;
                  break;
                case "-":
                  charTemp = 14;
                  break;
                case "=":
                  charTemp = 15;
                  break;
                case "{":
                  charTemp = 16;
                  break;
                case "}":
                  charTemp = 17;
                  break;
                case "|":
                  charTemp = 18;
                  break;
                case "[":
                  charTemp = 19;
                  break;
                case "]":
                  charTemp = 20;
                  break;
                case "\\":
                  charTemp = 21;
                  break;
                case ":":
                  charTemp = 22;
                  break;
                case "\"":
                  charTemp = 23;
                  break;
                case ";":
                  charTemp = 24;
                  break;
                case "'":
                  charTemp = 25;
                  break;
                case "<":
                  charTemp = 26;
                  break;
                case ">":
                  charTemp = 27;
                  break;
                case "?":
                  charTemp = 28;
                  break;
                case ",":
                  charTemp = 29;
                  break;
                case ".":
                  charTemp = 30;
                  break;
                case "/":
                  charTemp = 31;
                  break;
                case " ":
                  charTemp = 32;
                  break;
                default:
                  break temporaryloop;
              }


            }
          }
        }
      }
      if (countOrder < 268) countOrder++;
    }
    output = autocorrect(output);


    // Use this code and multiply it against headerKeys.get(i) to perform en/decryption
    // (decrypt) ? -1 : 1

    return output;
  }
}
