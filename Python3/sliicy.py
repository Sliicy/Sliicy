#!/usr/bin/python3

from tkinter.filedialog import askopenfilename
from tkinter.filedialog import askdirectory
from tkinter import *

import sys
import os
import re
import random
from random import randint

# Lookup IP address of URL:
from urllib.parse import urlparse
import socket

# Temporarily store typed text in memory for autocomplete and ease of use:
import readline
import rlcompleter
readline.parse_and_bind('tab: complete')
del readline, rlcompleter

const_Numbers = 0
const_Decimals = 1
const_IPv4 = 2 # Maximum is 4 places such as 192.168.0.1
const_IPv6 = 6 # Maximum is 8 places such as 0.0.0.0.0.0.0.0
numSign = 14 # Maximum is 12 numbers which are 1234567890.-
symSign = 26 # Maximum is 33 symbols which are ,./|\;':"[]{}<>?-=_+`~!@#$%^&*() and [space]
letterSign = 59 # Maximum is 52 letters which are A-Z and a-z
const_CountOrder = 111 # Maximum is 256 words currently
const_NumSignOrder = const_CountOrder
const_CountTotal = 367 # Maximum is 256 words currently
countSymbol = 623 # Maximum is 32 unique symbols in foundSymbols, 9 total which are .,;!?()+= 
const_Total = 654 # Maximum allocation of space needed for everything besides the wordlists
const_Types = const_Total + 1 # Start position of the wordlists

def isNumber(s):
    try:
        float(s)
        return True
    except ValueError:
        pass

def modIt(x, m):
    r = x % m
    if r < 0:
        return r + m
    else:
        return r

def clearScreen():
    if os.name == 'nt':
        absolutely_unused_variable = os.system("cls")
        absolutely_unused_variable = os.system("cls")    
    elif os.name == 'posix':
        absolutely_unused_variable = os.system("clear")
        absolutely_unused_variable = os.system("clear")

# The main Sliicy funtion:
def sliicy(command, user, password, message = None, friendName = None, newPassword = None, readMode = False):
    
    # The main wordlist that will hold all the words together (from the contact file's dictionary). Both wordList1 and wordList2 are 2D lists:
    wordList1 = []
    
    # A dual-purpose wordlist (when creating a wordlist, used as the shuffler; when (en/de)crypting, used to sort and save to the contact file:
    wordList2 = []
    
    # A list of words that didn't exist in the contact file will fill up here (and will be added to the contact file):
    addedWordList = []
    
    # A list format of the password found in the first line of the contact file:
    passwordChunk = []
    
    # 2 lists of every number from 0 to 255 or 65535, used to define as an IP address:
    IPv4 = list(range(255))
    IPv6 = list(range(65535))
    
    # Get the numerical value of the user's personal password:
    charCount = 1
    sumTotal = 0
    for c in password:
        sumTotal = sumTotal + ord(c) * charCount
        if c.isalpha():
            charCount = charCount + 1
    if charCount % 2 > 0:
        sumTotal = -sumTotal
    password = sumTotal * charCount
    
    # Start checking the arguments for Sliicy (check the lowercase of the command for compatibility with uppercase):
    if command.lower() == 'create':
        
        # Check if any fields are left blank:
        if not user or not password or not friendName:
            clearScreen()
            print("Operation failed because nothing can be left empty!\n")
            return
                
        # Create a list of strings based on Sliicy's Wordlists which is another file called 'wordlists':
        
        if not os.path.isfile("wordlists"):
            clearScreen()
            print("Wordlists file wasn't found. Please ensure the file is in the same directory as this script!\n")
            return
        lineSplit = open("wordlists", "r").read().split("\n\n")
        
        # Adds each subcontents of lineSplit into wordList1, list by list:
        for i in range(0, len(lineSplit) - 1):
            wordList1.append(lineSplit[i].split("\n"))
        
        # Create a shuffled list of numbers = total amount of word groups. This list will map out where each word group will go:
        randomList = list(range(0, len(lineSplit) - 1))
        random.shuffle(randomList)
        lastList = []
        for i in randomList:
            
            # Reserve the proper noun group for last (most new words are proper nouns) and saved to lastList:
            temp = wordList1[i]
            random.shuffle(temp)
            if not "Sliicy" in wordList1[i]:
                wordList2.append(temp)
            else:
                lastList = temp
        
        # Create an array from A-Z,a-z used for encrypting all new words (bit-wise style):
        abcs = []
        for c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz":
            abcs.append(c)
        
        # Insert the randomized ABC list at the beginning:
        random.shuffle(abcs)
        wordList2.insert(0, abcs)
        
        # Shuffle and add the missing proper noun list to the end:
        wordList2.insert(len(wordList2), lastList)
        
        # For each number in the line split including the total number of password segments used (see the constants listed at top of code):
        newPass = ""
        for i in range(0, len(lineSplit) + const_CountTotal + 10000):
            newPass = str(newPass) + str(randint(-100000, 100000)) + ";"
        
        # If the first number is a zero, keep repeating the process again (because it can compromise Sliicy):
        while (newPass[:2] == "0;"):
            newPass = str(randint(-100000, 100000)) + ";" + newPass[2]
        
        # If there are any other zeros, keep repeating the process again (because it can compromise Sliicy):
        while (";0" in newPass):
            newPass = newPass.replace(";0", ";" + str(randint(-100000, 100000)))
        
        # Remove the last character of the new password (it's an extra semicolon):
        newPass = newPass[:-1]
        
        # Make a final list that will have break points between each wordlist:
        finalList = []
        
        # For every wordlist:
        for i in range(0, len(wordList2)):
        
            # For every sub-wordlist:
            for j in range(0, len(wordList2[i])):
            
                # Add all words to the final list:
                finalList.append(wordList2[i][j])
            
            # Add a blank space to separate one group from the next:
            finalList.append("") #test
        
        # Remove the last empty string of final list:
        finalList = finalList[:-1]
        
        # Finally, write all changes to the contact file just created saved as a .slii (this file should be shared securely), and not encrypted by any password (until they join it):
        with open(user + ".slii", "w") as text_file:
            text_file.write(newPass + "\n\n" + "\n".join(map(str, finalList)))
        
        # Encrypt the user's own contact file used by their personal password:
        answer = ""
        
        # For each of the numbers found in the contact file's first line:
        for passFound in newPass.split(';'):
        
            # Add the user's password and the current number found in the contact file's first line to the answer:
            answer = answer + str(int(passFound) + int(password)) + ";"
        
        # Write all changes from the answer to the user's own contact file saved as a .txt file (NOT to be shared with anyone), and encrypted by the user's own password:
        with open(friendName + ".txt", "w") as text_file:
            text_file.write(answer[:-1] + "\n\n" + "\n".join(map(str, finalList)))
        clearScreen()
        print("Successfully created 2 files. Please share " + user + ".slii with the recipient over a secure line! Do not give " + friendName + ".txt to anyone!\n")
        
    # If Sliicy was asked to join a conversation:
    elif command.lower() == 'join':
        
        # Check if any fields are left blank:
        if not user or not password:
            clearScreen()
            print("Operation failed because nothing can be left empty!\n")
            return
        
        # Remove redundant quotes from contact file location supplied:
        #Commented out to be tested on Windows only
        #user = user.replace('""', None)
        
        # Get the entire contact file into a split string:
        lineSplit = open(user, "r").read().split("\n\n")
        
        # Temporary holding string:
        answer = ""
        
        # Using the numerical value of the user's password, every number is added to this number to encrypt it:
        for passFound in lineSplit[0].split(';'):
            
            # Encrypt passwords:
            answer = answer + str(int(passFound) + int(password)) + ";"
        
        # Set the first string of the line split to the numbers just encrypted and remove the last semicolon:
        lineSplit[0] = answer[:-1]
        
        # Make a new file so that the new password along with the wordlists get "joined" and the user can then use this file:
        # It's important that the wordlists have a separating blank line separating each wordlist.
        # This contact file is encrypted with the user's password. If this gets into the wrong hands, it still will be extremely hard to guess each number.
        with open(user.replace(".slii", ".txt"), "w") as text_file:
            text_file.write("\n\n".join(map(str, lineSplit)))
        
        # Delete the old contact file ending in a .slii (no longer needed and a major security flaw if not deleted):
        os.remove(user)
        clearScreen()
        print("Successfully joined contact!\n")
        
    # If Sliicy received an encrypt or decrypt request:
    elif command.lower() == 'encrypt' or command.lower() == 'decrypt':
        
        # Check if any fields are left blank:
        if not user or not password or not message:
            clearScreen()
            print("Operation failed because nothing can be left empty!\n")
            return
        
        # Check if message has no words:
        if command.lower() == 'encrypt' and not re.search('[a-zA-Z]', message):
            clearScreen()
            print("Operation failed because message must contain at least 1 word for security purposes!\n")
            return
            
        # Remove leading and trailing spaces from message:
        message.strip()
        
        # Remove periods from numbers (they are seen as decimals):
        if "." in message:
            
            # redFlag indicates if any number has a trailing dot:
            redFlag = 0
            countChars = 0
            locationOfDot = []
            
            for c in message:
                if c == "." and redFlag == 1 and countChars == len(message) - 1:
                    locationOfDot.append(countChars)
                    redFlag = 0
                elif isNumber(c):
                    redFlag = 1
                elif c == "." and redFlag == 1:
                    redFlag = 2
                elif c == "." and redFlag == 2:
                    locationOfDot.append(countChars - 1)
                    redFlag = 0
                elif not isNumber(c) and c != "." and redFlag == 2:
                    locationOfDot.append(countChars - 1)
                    redFlag = 0
                elif not isNumber(c) and c != ".":
                    redFlag = 0
                countChars = countChars + 1
            
            # Remove any dots connecting to numbers starting backwards:
            if len(locationOfDot) > 0:
                for i in range(len(locationOfDot) - 1, 0, -1):
                    message = message.insert(locationOfDot[i], " ")
            
        # Split contact file into sections of lineSplits():
        lineSplit = open(user, "r").read().split("\n\n")
        
        # Set the Master Password to the first line of contact:
        masterPassword = lineSplit[0]
        
        # Decrypt the Master Password based on the numerical value of the user's own password:
        # Computation will temporarily be the string builder (it will hold our results):
        computation = ""
        
        # For each of the numbers found in the contact file's first line:
        for passFound in masterPassword.split(";"):
            
            # Decrypt number and append a semicolon:
            computation = computation + str(int(passFound) - int(password)) + ";"
        
        # Finally add again all decrypted numbers back to Master Password (since they were encrypted and unreadable before):
        masterPassword = computation[:-1]
        
        # Add each wordlist found from the contact to WordLists 1 & 2 (start at index 2 because index 0 are numbers, and index 1 are letters):
        for i in range(2, len(lineSplit) - 1):
            wordList1.append(lineSplit[i].split("\n\n"))
            wordList2.append(lineSplit[i].split("\n\n"))
        
        # Create a list dedicated to letter-based encryption for new words (starting at 1 because that is where the letters are located):
        letterList = lineSplit[1].split("\n\n")
        
        # Flip all numbers from + to - and vice versa if decrypting:
        if command.lower() == "decrypt":
            for passFound in masterPassword.split(";"):
                if "-" in passFound:
                    
                    # Remove existing - sign:
                    passwordChunk.append(passFound[1])
                else:
                    
                    # Add a - sign:
                    passwordChunk.append("-" + passFound)
        
        else:
            for passFound in masterPassword.split(";"):
                passwordChunk.append(passFound)
        
        # Remove fancy quotation marks (’ ‘ into ' and “ ” into "):
        message = message.replace("’", "'").replace("‘", "'").replace('“', '"').replace('”', '"')
        
        # Anti-forgery:
        signature = 0
        verifySignature = 0
        
        if command.lower() == "decrypt":
            verifySignature = message[message.rfind(' '):len(message) - message.rfind(' ')].strip()
            message = message[:message.rfind(' ')]
        
        # Constantly add or subtract each word's position from the next word's position to create the unique signature:
        flipPositiveNegative = True
        
        # Count the number of words in the message:
        occurrences = len(message) - len(message.replace(" ", "")) + 1
        
        # Warn if too many words are being sent:
        if occurrences > 256 and not command.lower() == "decrypt":
            choice = input("\nThe message has more than 256 words. Any extra words won't have a unique encryption. Continue?")
            if not(choice.lower()=='y' or choice.lower()=='yes'):
                return
        
        # Assign variables:
        countOrder = const_CountOrder
        numSignOrder = const_CountOrder
        countTotal = const_CountTotal
        
        # Strength represents how strong the requested message is from being cracked:
        strength = 1
        
        # Define a list of characters to find in the message (colons (:) excluded due to IPv6 conflicts, also minus sign (-) not included due to numbers):
        foundSymbols = [".", ",", ";", "!", "?", "(", ")", "+", "="]
        
        # Get total number of all occurrences of symbols found:
        total = 0
        for i in range (0, len(foundSymbols) - 1):
            total = total + len(message) - len(message.replace(foundSymbols[i], ""))
        
        # 32 = punctuation on average there can be in short messages:
        if total > 32:
            total = 32
        
        # Delimiters are various symbols (they are ignored in message processing):
        
        delimiters = list(filter(None, re.split(r'[ ~!@#$%^&*()_=+,<>/?;"{}\|\n]+', message)))
        
        
        for word in message.split(' '):
            if word == '':
                continue
            
            # Replace URL contained in word with regular IPv4 address (ie: 192.168.1.1):
            p = '(?:http.*://)?(?P<host>[^:/ ]+).?(?P<port>[0-9]*).*'
            try:
                if "://" in word:
                    message = message.replace(word, socket.gethostbyname(re.search(p, urlparse(str(word))[1]).group('host')))
                elif "/" in word and "://" not in word:
                    message = message.replace(word, socket.gethostbyname(re.search(p, urlparse('http://' + str(word))[1]).group('host')))
                else:
                    message = message.replace(word, socket.gethostbyname(str(word)))
            except:
                pass
            del p
            # Every word will add to the total count of words in the message:
            if countTotal < const_CountTotal + 256:
                countTotal = countTotal + 1
        # If the above code found website URLs, they are replaced with their actual IP address counterparts.
        
        output = re.split(r'[ ~!@#$%^&*()_=+,<>/?;"{}\|\n]+', message)
        print("message: " + str(message))
        print(*output, sep=" ")
        
        # Continue
        
    elif command.lower() == 'change':
        
        # Check if any fields are left blank:
        if not user or not password:
            clearScreen()
            print("Operation failed because nothing can be left empty!\n")
            return
        
        # Set charCount and sumTotal to 0 to calculate the new password's value:
        charCount = 0
        sumTotal = 0
        
        # Calculate the new password:
        for c in newPassword:
            sumTotal = sumTotal + ord(c) * charCount
            if c.isalpha():
                charCount = charCount + 1
        if charCount % 2 > 0:
            sumTotal = -sumTotal
        newPassword = sumTotal * charCount
        
        # Detect if folder exists:
        if os.path.isdir(user) == True:
            
            # The following code will warn the user of all text files which will be modified:
            listOfFiles = 0
            for filename in os.listdir(user):
                if ".txt" in filename:
                    lineSplit = open(os.path.join(user, filename), "r").read().split("\n\n")
                    if "Sliicy" in open(os.path.join(user, filename), "r").read() and ";" in open(os.path.join(user, filename), "r").read():
                        listOfFiles = listOfFiles + 1
                        print(str(listOfFiles) + ": " + filename)
            if listOfFiles == 0:
                clearScreen()
                print('No contact files were found in the specified folder.\n')
                return
            else:
                print('\nThe above files located in "' + user + '" WILL BE MODIFIED! This cannot be undone. Proceed?')
            
            # If the user proceeds:
            choice2 = input()
            if choice2.lower()=='y' or choice2.lower()=='yes':
                for filename in os.listdir(user):
                
                    # If the file has a .txt extension:
                    if ".txt" in filename:
                        
                        # Read contents of file:
                        lineSplit = open(os.path.join(user, filename), "r").read().split("\n\n")
                        
                        # If file contains "Sliicy" and semicolons:
                        if "Sliicy" in open(os.path.join(user, filename), "r").read() and ";" in open(os.path.join(user, filename), "r").read():
                        
                            # masterPassword will hold the first line of numbers in the contact file:
                            masterPassword = lineSplit[0]
                                                        
                            # Computation will hold the decrypted numbers:
                            computation = ""
                            
                            # For each of the numbers found in the contact file's first line:
                            for passFound in masterPassword.split(';'):
                            
                                # Decrypt number from old password:
                                computation = computation + str((int(passFound) - int(password)) + int(newPassword)) + ";"
                            
                            # Trim the last extra semicolon:
                            lineSplit[0] = computation[:-1]
                            
                            # Write all changes to each contact found:
                            with open(os.path.join(user, filename), "w") as text_file:
                                text_file.write("\n\n".join(map(str, lineSplit)))
            
                # Set the new password:
                password = newPassword
                clearScreen()
                print("Password successfully changed in selected folder!")
            else:
                clearScreen()
                print("Nothing was changed!")
        # End of Sliicy function.

# Handle arguments:
if len(sys.argv) == 1:
    temp = None
elif len(sys.argv) == 3:
    sliicy(sys.argv[1], sys.argv[2], sys.argv[3])
elif len(sys.argv) == 4:
    sliicy(sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4])
elif len(sys.argv) == 6:
    sliicy(sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4], sys.argv[5], sys.argv[6])
else:
    print("Error") # script needs to be fixed help goes here ???????????????????

#sliicy("create", "user", "password", "message", "friendName") # Delete this line (only meant for debugging)

# Handle Ctrl + C:
try:
    # Retrieve password and clear screen twice for full privacy:
    print('Please enter your password:\n')
    # Input is typed plain-text because Sliicy can't verify if the password is correct. If the user mistypes the password, all subsequent messages will be corrupted.
    password = input()
    clearScreen()
    attempt = 0
    while password == '':
        attempt = attempt + 1
        if attempt >= 3:
            clearScreen()
            print("User entered no password. Not sure why. History erased for privacy.")
            sys.exit()
        clearScreen()
        print("Password can't be empty!\nPlease enter your current or new password!\n")
        password = input()
    clearScreen()
    
    # Main screen:
    print('\nWelcome to Sliicy!\n')
    while True:
        print('Select one of the following:\n')
        print('1:  Start a new group')
        print('2:  Join another chat')
        print('3:  Encrypt a message')
        print('4:  Decrypt a message')
        print('5:  Change a password')
        print('6:  Help, about, logs')
        print('7:  Exit\n')
        choice = input()
        if choice=='1' or choice.lower()=='n' or choice.lower()=='new' or choice.lower()=='start':
            print('Enter your name:')
            user = input()
            print("Enter your friend's name:")
            friendName = input()
            sliicy("create", user, password, None, friendName)
        elif choice=='2' or choice.lower()=='j' or choice.lower()=='join' or choice.lower()=='existing':
            print("Loading, please wait!")
            root = Tk()
            root.filename = filedialog.askopenfilename(title = "Choose existing contact to join",filetypes = (("Slii Files","*.slii"),("All Files","*.*")))
            root.withdraw()
            if root.filename == "":
                clearScreen()
                print("No file selected!\n")
            else:
                sliicy("join", root.filename, password)
        elif choice=='3' or choice.lower()=='e' or choice.lower()=='encrypt':
            print('Select a contact:')
            root = Tk()
            root.filename = filedialog.askopenfilename(title = "Choose contact",filetypes = (("Text Files","*.txt"),("All Files","*.*")))
            root.withdraw()
            if root.filename == "":
                clearScreen()
                print("No file selected!\n")
            else:
                print("Enter your message:")
                message = input()
                sliicy("encrypt", root.filename, password, message, None, None, False)
        elif choice=='4' or choice.lower()=='d' or choice.lower()=='decrypt':
            print('Select a contact:')
            root = Tk()
            root.filename = filedialog.askopenfilename(title = "Choose contact",filetypes = (("Text Files","*.txt"),("All Files","*.*")))
            root.withdraw()
            if root.filename == "":
                clearScreen()
                print("No file selected!\n")
            else:
                print("Enter the message to decrypt:")
                message = input()
                sliicy("decrypt", root.filename, password, message, None, None, False)
        elif choice=='5' or choice.lower()=='c' or choice.lower()=='change' or choice.lower()=='password':
            print('Please select the folder containing all of your contacts:')
            root = Tk()
            root.filename = filedialog.askdirectory()
            print("\nYou have selected: " + root.filename + "\n")
            root.withdraw()
            if not root.filename:
                clearScreen()
                print("No folder selected!\n")
            else:
                print("Enter your new password:")
                newPassword = input()
                print("Re-enter your new password:")
                comparePassword = input()
                clearScreen()
                if newPassword == comparePassword:
                    sliicy("change", root.filename, password, None, None, newPassword)
                else:
                    print("Passwords don't match!")
        elif choice=='6' or choice.lower()=='h' or choice.lower()=='help':
            clearScreen()
            print('\nSliicy encrypts your conversations by scrambling words together.\n')
            print('Only trust the official Sliicy app from Sliicy.com or Github.com/Sliicy\n')
            print('Created by Sliicy.\n')
        elif choice=='7' or choice.lower()=='exit' or choice.lower()=='q' or choice.lower()=='quit' or choice.lower()=='end' or choice.lower()=='stop' or choice.lower()=='x':
            clearScreen()
            print("Goodbye! History erased for privacy.")
            break
        else:
            clearScreen()
            print('\nSorry, please try again!\n')
except KeyboardInterrupt:
    clearScreen()
    print("Control-C was used to abort. History erased for privacy.")
    sys.exit()
