#!/usr/bin/python3

version = 1.1

# Message boxes:
from tkinter.filedialog import askopenfilename
from tkinter.filedialog import askdirectory
from tkinter import *
from tkinter import messagebox
root = Tk()
root.withdraw()

# Get username of user (username will be suggested when creating a new contact):
import getpass

import sys
import os
import re
import random
from random import randint
import argparse

# Lookup IP address of URL:
from urllib.parse import urlparse
import socket

# Time how long encryption takes:
import timeit

# Add coloring:
class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

# Constants which control the position of the numbers on the contact file:
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

# Find if string is number:
def isNumber(s):
    try:
        float(s)
        return True
    except ValueError:
        pass

# Find modulo function:
def modIt(x, m):
    r = x % m
    if r < 0:
        return r + m
    else:
        return r

# Removes duplicates from list, case-sensitive:
def removeDuplicatesSensitive(values):
    output = []
    seen = set()
    for value in values:
        if value not in seen:
            output.append(value)
            seen.add(value)
    return output

# Removes duplicates from list, case-insensitive:
def removeDuplicates(values):
    output = []
    helperset = set()
    for x in values:
        if x.lower() not in helperset:
            output.append(x)
            helperset.add(x.lower())
    return output

# Find if IPv4 address is legal:
def validIPv4(address):
    try:
        socket.inet_pton(socket.AF_INET, address)
    except AttributeError:  # no inet_pton here, sorry
        try:
            socket.inet_aton(address)
        except socket.error:
            return False
        return address.count('.') == 3
    except socket.error:  # not a valid address
        return False
    return True

# Find if IPv6 address is legal:
def validIPv6(address):
    try:
        socket.inet_pton(socket.AF_INET6, address)
    except socket.error:  # not a valid address
        return False
    return True

# Clear the screen twice to prevent history:
def clearScreen():
    if os.name == 'nt':
        absolutely_unused_variable = os.system("cls")
        absolutely_unused_variable = os.system("cls")    
    elif os.name == 'posix':
        absolutely_unused_variable = os.system("clear")
        absolutely_unused_variable = os.system("clear")

# The main Sliicy funtion:
def sliicy(command, user, password, message = None, friendName = None, newPassword = None, readMode = False):
    messagebox.showinfo(command, 'user:' + user + '\npassword:' + password +'\nmessage:' + str(message) + '\nfriendName:' + str(friendName) + '\nnewPassword:' + str(newPassword) + '\nreadMode:' + str(readMode))
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
            print(bcolors.FAIL + "Operation failed because nothing can be left empty!\n" + bcolors.ENDC)
            return
                
        # Create a list of strings based on Sliicy's Wordlists which is another file called 'wordlists':
        
        if not os.path.isfile("wordlists"):
            clearScreen()
            print(bcolors.FAIL + "Wordlists file wasn't found. Please ensure the file is in the same directory as this script!\n" + bcolors.ENDC)
            return
        lineSplit = open("wordlists", 'r', encoding='utf-8').read().split('\n\n')
        
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
        
        # Create an array from A-Z, a-z used for encrypting all new words (bit-wise style), including a hyphen for special words:
        abcs = []
        for c in "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-":
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
        
        # If the first number is a zero, keep repeating the process again, because it can compromise security:
        while (newPass[:2] == "0;"):
            newPass = str(randint(-100000, 100000)) + ";" + newPass[2]
        
        # If there are any other zeros, keep repeating the process again, because it can compromise security:
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
        with open(user + ".slii", 'w', encoding='utf-8') as text_file:
            text_file.write(newPass + "\n\n" + "\n".join(map(str, finalList)))
        
        # Encrypt the user's own contact file used by their personal password:
        answer = ""
        
        # For each of the numbers found in the contact file's first line:
        for passFound in newPass.split(';'):
        
            # Add the user's password and the current number found in the contact file's first line to the answer:
            answer = answer + str(int(passFound) + int(password)) + ";"
        
        # Write all changes from the answer to the user's own contact file saved as a .txt file (NOT to be shared with anyone), and encrypted by the user's own password:
        with open(friendName + ".txt", 'w', encoding='utf-8') as text_file:
            text_file.write(answer[:-1] + "\n\n" + "\n".join(map(str, finalList)))
        clearScreen()
        print(bcolors.OKGREEN + "Successfully created 2 files. " + bcolors.WARNING + "Please share " + user + ".slii with the recipient over a secure line! " + bcolors.FAIL + "Do not give " + friendName + ".txt to anyone!\n" + bcolors.ENDC)
        
    # If Sliicy was asked to join a conversation:
    elif command.lower() == 'join':
        
        # Check if any fields are left blank:
        if not user or not password:
            clearScreen()
            print(bcolors.FAIL + "Operation failed because nothing can be left empty!\n" + bcolors.ENDC)
            return
        
        # Get the entire contact file into a split string:
        lineSplit = open(user, 'r', encoding='utf-8').read().split("\n\n")
        
        # Temporary holding string:
        answer = ""
        
        # Using the numerical value of the user's password, every number is added to this number to encrypt it:
        for passFound in lineSplit[0].split(';'):
            
            # Encrypt passwords:
            answer = answer + str(int(passFound) + int(password)) + ";"
        
        # Set the first string of the line split to the encrypted numbers and remove the last semicolon:
        lineSplit[0] = answer[:-1]
        
        # Make a new file so that the new password along with the wordlists get "joined" and the user can then use this file:
        # It's important that the wordlists have a separating blank line separating each wordlist.
        # This contact file is encrypted with the user's password. If this gets into the wrong hands, it still will be extremely hard to guess each number.
        with open(user.replace(".slii", ".txt"), 'w', encoding='utf-8') as text_file:
            text_file.write("\n\n".join(map(str, lineSplit)))
        
        # Delete the old contact file ending in a .slii (no longer needed and a major security flaw if not deleted):
        os.remove(user)
        clearScreen()
        print(bcolors.OKGREEN + "Successfully joined contact!\n" + bcolors.ENDC)
        
    # If Sliicy received an encrypt or decrypt request:
    elif command.lower() == 'encrypt' or command.lower() == 'decrypt':
        
        # Measure how long encryption takes:
        start_time = timeit.default_timer()
        
        # Check if any fields are left blank:
        if not user or not password or not message:
            clearScreen()
            print(bcolors.FAIL + "Operation failed because nothing can be left empty!\n" + bcolors.ENDC)
            return
        
        # Check if message has no words:
        if command.lower() == 'encrypt' and not re.search('[a-zA-Z]', message):
            clearScreen()
            print(bcolors.FAIL + "Operation failed because message must contain at least a single word for security purposes!\n" + bcolors.ENDC)
            return
        
        # Remove leading and trailing spaces from message:
        message = message.strip(' ')
        
        # Remove double spacing:
        while '  ' in message:
            message = message.replace('  ', ' ')
        
        # Separate all numbers from letters which can't be handled (example: 20th, 5e10, 3D):
        message = re.sub(r'(\d)([A-z])|([A-z])(\d)', r'\1 \2', message)
        
        # Remove periods from numbers (they are seen as decimals):
        if '.' in message:
            
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
        lineSplit = open(user, 'r', encoding='utf-8').read().split('\n\n')
        
        # Set the Master Password to the first line of contact:
        masterPassword = lineSplit[0]
        
        # Phrase will hold the final output of the message:
        phrase = ''
        
        # Decrypt the Master Password based on the numerical value of the user's own password. Computation will temporarily be the string builder (it will hold the results):
        computation = ""
        
        # For each of the numbers found in the contact file's first line:
        for passFound in masterPassword.split(";"):
            
            # Decrypt number and append a semicolon:
            computation = computation + str(int(passFound) - int(password)) + ";"
        
        # Finally add again all decrypted numbers back to Master Password (since they were encrypted and unreadable before):
        masterPassword = computation[:-1]
        
        # Add each wordlist found from the contact to WordLists 1 & 2 (start at index 2 because index 0 are numbers, and index 1 are letters):
        for i in range(2, len(lineSplit)):
            wordList1.append(lineSplit[i].split('\n'))
            wordList2.append(lineSplit[i].split('\n'))
        
        # Create a list dedicated to letter-based encryption for new words (starting at 1 because that is where the letters are located):
        letterList = lineSplit[1].split('\n')
        
        # Flip all numbers from + to - and vice versa if decrypting:
        if command.lower() == "decrypt":
            for passFound in masterPassword.split(";"):
                if "-" in passFound:
                    
                    # Remove existing - sign:
                    passwordChunk.append(passFound[1:])
                else:
                    
                    # Add a - sign:
                    passwordChunk.append("-" + passFound)
        
        else:
            for passFound in masterPassword.split(";"):
                passwordChunk.append(passFound)
        
        # Remove fancy quotation marks (’ ‘ into ' and “ ” into "):
        message = message.replace("’", "'").replace("‘", "'").replace('“', '"').replace('”', '"')
        
        # Signature will hold the calculated value of the message while verifySignature will be set to the trailing numbers in each encrypted message. This prevents forgery:
        signature = 0
        verifySignature = 0
        
        # Set verifySignature to the trailing number in the message and trim that number from the message itself:
        if command.lower() == "decrypt":
            if ' ' not in message:
                clearScreen()
                print(bcolors.FAIL + "Messages require both content and a numerical signature.\n" + bcolors.ENDC)
                return
            verifySignature = message[message.rindex(' ') + 1:len(message)]
            message = message[:message.rfind(' ')]
            if not isNumber(verifySignature):
                clearScreen()
                print(bcolors.FAIL + "Messages require a numerical signature.\n" + bcolors.ENDC)
                return
        
        # Constantly add or subtract each word's position from the next word's position to create the unique signature:
        flipPositiveNegative = True
        
        # Count the number of words in the message:
        occurrences = len(message) - len(message.replace(' ', '')) + 1
        
        # Warn if too many words are being sent:
        if occurrences > 256 and not command.lower() == "decrypt":
            choice = input(bcolors.WARNING + "\nThe message has more than 256 words. Any extra words won't have a unique encryption. Continue?\n" + bcolors.ENDC)
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
            total = total + len(message) - len(message.replace(foundSymbols[i], ''))
        
        # Total will hold how much punctuation is present in the message. The average amount of punctuation in short messages usually doesn't exceed 32 characters:
        if total > 32: total = 32
        
        # Replace all URLs in the message with IP addresses:
        for word in message.split(' '):
            if word == '':
                continue
            
            # Replace URL contained in word with regular IPv4 address (ie: 192.168.1.1):
            p = '(?:http.*://)?(?P<host>[^:/ ]+).?(?P<port>[0-9]*).*'
            try:
                
                # URL has http, https, ftp, ftps, etc:
                if '://' in word:
                    message = message.replace(word, socket.gethostbyname(re.search(p, urlparse(str(word))[1]).group('host'))).replace('https://', '').replace('http://', '').replace('ftps://', '').replace('ftp://', '')
                    
                # URL has page in it, and no 'http' prefix (eg: google.com/home):
                elif '/' in word and '://' not in word:
                    message = message.replace(word, socket.gethostbyname(re.search(p, urlparse('http://' + str(word))[1]).group('host')))
                    
                # URL is bare (eg: google.com):
                else:
                    
                    # Catch if numbers are detected:
                    if word.count('.') >= 2 or not isNumber(word):
                        message = message.replace(word, socket.gethostbyname(str(word)))
            except:
                pass
            del p
            # Every word will add to the total count of words in the message:
            if countTotal < const_CountTotal + 256:
                countTotal = countTotal + 1
        
        output = re.split(r'(\ |~|!|@|\#|\$|%|\^|&|\*|\(|\)|_|=|\+|,|<|>|/|\?|;|"|\[|\{|]|}|\\|\||\n)', message)
        
        # This will hold all the words:
        bigListOfAllWords = []
        
        # Fill the big list with all words found in the contact file (everything must be combined into one large list):
        for i in range(0, len(wordList1)):
            for j in range(0, len(wordList1[i])):
                bigListOfAllWords.append(wordList1[i][j])
        
        # Catch if no words left:
        if len(output) == 0:
            return
                
        # For each word in the message:
        for word in output:
            
            # If word is blank, skip:
            if word == '': continue
            
            # Any dots and colons found in the word will help us know if the address is IPv4 or IPv6:
            dotFound = 0
            colonFound = 0
            
            # If the word actually is a number (ie: 5, 3.14, -70), it is dealt with separately:
            if isNumber(word) or (command.lower() == 'decrypt' and '.' in word and word.endswith('-')):
                if 'e' in word.lower():
                    clearScreen()
                    print(bcolors.FAIL + "Message can't contain any exponents." + bcolors.ENDC)
                    return
                negativeDecimal = False
                if '.' in word and word.endswith('-'):
                    negativeDecimal = True
                    word = word[:-1]
                if not isNumber(word):
                    clearScreen()
                    print(bcolors.FAIL + "Message contains semi-numeric words that can't be evaluated. Integers, decimals and floats are allowed." + bcolors.ENDC)
                    return
                if '.' in word:
                    word = word.rstrip('0')
                    if word.endswith('.'): word = word + '0'
                originalPhrase = word
                cryptedPhrase = str(float(originalPhrase) + int(passwordChunk[const_Numbers]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]))
                if '.' in str(originalPhrase):
                    originalAnswer = str(originalPhrase).split('.')
                    cryptedAnswer = str(originalPhrase).split('.')
                    cryptedAnswer[1] = cryptedAnswer[1][::-1]
                    cryptedAnswer[0] = int(cryptedAnswer[0]) + int(passwordChunk[const_Numbers]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total])
                    
                    # Reversed so .07 becomes 70, and -90 becomes .09-:
                    if negativeDecimal:
                        cryptedAnswer[1] = str(int(cryptedAnswer[1]) + -(int(passwordChunk[const_Decimals]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total])))[::-1].replace('-', '')
                    else:
                        cryptedAnswer[1] = str(int(cryptedAnswer[1]) + int(passwordChunk[const_Decimals]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]))[::-1]
                    cryptedPhrase =  str(cryptedAnswer[0]) + '.' + str(cryptedAnswer[1])
                if 'e' in cryptedPhrase.lower():
                    clearScreen()
                    print(bcolors.FAIL + "Message can't contain any exponents." + bcolors.ENDC)
                    return
                if cryptedPhrase.endswith('.0'):
                    cryptedPhrase = cryptedPhrase[:-2]
                phrase = phrase + cryptedPhrase
                signature2 = 0
                flipPositiveNegative2 = True
                if command.lower() == 'encrypt':
                    for c in str(word):
                        temp = 0
                        if isNumber(c):
                            temp = int(passwordChunk[numSign + int(c)]) + int(passwordChunk[numSignOrder])
                        elif c == '-':
                            temp = int(passwordChunk[numSign + 10]) + int(passwordChunk[numSignOrder])
                        elif c == '.':
                            temp = int(passwordChunk[numSign + 11]) + int(passwordChunk[numSignOrder])
                        if signature2 == 0:
                            signature2 = temp
                        else:
                            if flipPositiveNegative2:
                                signature2 = signature2 + temp
                            else:
                                signature2 = signature2 - temp
                        numSignOrder =  numSignOrder + 1
                        flipPositiveNegative2 = not flipPositiveNegative2
                    numSignOrder = const_CountOrder
                    
                    # The signature will be empty the first time. Here is where it is assigned:
                    if signature == 0:
                        signature = signature2
                    else:
                        
                        # If positive, add the new word's positional number by positive, or else subtract it:
                        if flipPositiveNegative:
                            signature = signature + signature2
                        else:
                            signature = signature - signature2
                    
                    # Flip the value of manipulating the signature by + or - as being True or False respectively:
                    flipPositiveNegative = not flipPositiveNegative
                elif command.lower() == 'decrypt':
                    for c in cryptedPhrase:
                        temp = 0
                        if isNumber(c):
                            temp = -int(passwordChunk[numSign + int(c)]) - int(passwordChunk[numSignOrder])
                        elif c == '-':
                            temp -int(passwordChunk[numSign + 10]) - int(passwordChunk[numSignOrder])
                        elif c == '.':
                            temp = -int(passwordChunk[numSign + 11]) - int(passwordChunk[numSignOrder])
                        if signature2 == 0:
                            signature2 = temp
                        else:
                            if flipPositiveNegative2:
                                signature2 = signature2 + temp
                            else:
                                signature2 = signature2 - temp
                        numSignOrder = numSignOrder + 1
                        flipPositiveNegative2 = not flipPositiveNegative2
                    numSignOrder = const_CountOrder
                    if signature == 0:
                        signature = signature2
                    else:
                        
                        # If positive, add the new word's positional number by positive, or else subtract it:
                        if flipPositiveNegative:
                            signature = signature + signature2
                        else:
                            signature = signature - signature2
                    
                    # Flip the value of manipulating the signature by + or - as being True or False respectively:
                    flipPositiveNegative = not flipPositiveNegative
                
            # If the word is an IPv4 address:
            elif validIPv4(word) and ':' not in word:
                
                # Count number of dots found (since IPv4 addresses only have dots):
                dotCount = word.count('.')
                
                # If 3 dots found (ie: 192.168.1.1):
                if dotCount == 3:
                    
                    # Iterate 4x to keep track:
                    tempCount = 0
                    
                    # For every number (4 total in an IPv4) found in the IPv4 address:
                    for item in word.split('.'):
                        
                        # Positive and negative results are split to prevent out of bounds. If the number in the contact password located at [1 + how many times this code ran] is a positive number:
                        cryptedPhrase = 0
                        if int(passwordChunk[const_IPv4 + tempCount]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]) >= 0:
                            cryptedPhrase = (int(item) + int(passwordChunk[const_IPv4 + tempCount]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total])) % len(IPv4)
                        else:
                            answer = int(item) + modIt(int(passwordChunk[const_IPv4 + tempCount]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]), len(IPv4))
                            if answer >= len(IPv4): answer = answer - len(IPv4)
                            cryptedPhrase = IPv4[answer]
                        phrase = phrase + str(cryptedPhrase) + '.'
                        
                        signature2 = 0
                        flipPositiveNegative2 = True
                        if command.lower() == 'encrypt':
                            for c in str(item):
                                temp = int(passwordChunk[numSign + int(c)]) + int(passwordChunk[numSignOrder])
                                if signature2 == 0:
                                    signature2 = temp
                                else:
                                    if flipPositiveNegative2:
                                        signature2 = signature2 + temp
                                    else:
                                        signature2 = signature2 - temp
                                numSignOrder = numSignOrder + 1
                                flipPositiveNegative2 = not flipPositiveNegative2
                            numSignOrder = const_CountOrder
                            
                            # The signature will be empty the first time. Here is where it is assigned:
                            if signature == 0:
                                signature = signature2
                            else:
                                
                                # If positive, add the new word's positional number by positive, or else subtract it:
                                if flipPositiveNegative:
                                    signature = signature + signature2
                                else:
                                    signature = signature - signature2
                            
                            # Flip the value of manipulating the signature by + or - as being True or False respectively:
                            flipPositiveNegative = not flipPositiveNegative
                            
                        elif command.lower() == 'decrypt':
                            for c in str(cryptedPhrase):
                                temp = -1 * int(passwordChunk[numSign + int(c)]) - int(passwordChunk[numSignOrder])
                                if signature2 == 0:
                                    signature2 = temp
                                else:
                                    if flipPositiveNegative2:
                                        signature2 = signature2 + temp
                                    else:
                                        signature2 = signature2 - temp
                                numSignOrder = numSignOrder + 1
                                flipPositiveNegative2 = not flipPositiveNegative2
                            numSignOrder = const_CountOrder
                            if signature == 0:
                                signature = signature2
                            else:
                                
                                # If positive, add the new word's positional number by positive, or else subtract it:
                                if flipPositiveNegative:
                                    signature = signature + signature2
                                else:
                                    signature = signature - signature2
                            
                            # Flip the value of manipulating the signature by + or - as being True or False respectively:
                            flipPositiveNegative = not flipPositiveNegative
                        tempCount = tempCount + 1
                    phrase = phrase[:-1]
                    continue
                
            # The word is IPv6:
            elif validIPv6(word) and ':' in word:
                colonCount = word.count(':')
                if colonCount >= 2 and colonCount <= 7:
                    phrase0 = ''
                    neededZeros = 9 - len(word.split(':'))
                    
                    # Convert to longhand decimal:
                    for item in word.split(':'):
                        if item == '':
                            if neededZeros == 1: phrase0 = phrase0 + ':0'
                            if neededZeros == 2: phrase0 = phrase0 + ':0:0'
                            if neededZeros == 3: phrase0 = phrase0 + ':0:0:0'
                            if neededZeros == 4: phrase0 = phrase0 + ':0:0:0:0'
                            if neededZeros == 5: phrase0 = phrase0 + ':0:0:0:0:0'
                            if neededZeros == 6: phrase0 = phrase0 + ':0:0:0:0:0:0'
                            if neededZeros == 7: phrase0 = phrase0 + ':0:0:0:0:0:0:0'
                        else:
                            phrase0 = phrase0 + ':' + str(int(item, 16))
                    phrase1 = ''
                    tempCount = 0
                    
                    # Add substring 1 because of a : at the beginning:
                    for item in phrase0[1:].split(':'):
                        
                        # Cannot add 'countSymbol' and 'total' because the IPv6 colons always change their size:
                        cryptedPhrase = 0
                        if int(passwordChunk[const_IPv6 + tempCount]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) >= 0:
                            cryptedPhrase = (int(item) + int(passwordChunk[const_IPv6 + tempCount]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal])) % len(IPv6)
                        else:
                            answer = int(item) + modIt(int(passwordChunk[const_IPv6 + tempCount]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]), len(IPv6))
                            if answer >= len(IPv6): answer = answer - len(IPv6)
                            cryptedPhrase = IPv6[answer]
                        phrase1 = phrase1 + str(cryptedPhrase) + ':'
                        signature2 = 0
                        flipPositiveNegative2 = True
                        if command.lower() == 'encrypt':
                            for c in str(item):
                                temp = int(passwordChunk[numSign + int(c)]) + int(passwordChunk[numSignOrder])
                                if signature2 == 0:
                                    signature2 = temp
                                else:
                                    if flipPositiveNegative2:
                                        signature2 = signature2 + temp
                                    else:
                                        signature2 = signature2 - temp
                                numSignOrder = numSignOrder + 1
                                flipPositiveNegative2 = not flipPositiveNegative2
                            numSignOrder = const_CountOrder
                            
                            # The signature will be empty the first time. Here is where it is assigned:
                            if signature == 0:
                                signature = signature2
                            else:
                                
                                # If positive, add the new word's positional number by positive, or else subtract it:
                                if flipPositiveNegative:
                                    signature = signature + signature2
                                else:
                                    signature = signature - signature2
                            
                            # Flip the value of manipulating the signature by + or - as being True or False respectively:
                            flipPositiveNegative = not flipPositiveNegative
                            
                        elif command.lower() == 'decrypt':
                            for c in str(cryptedPhrase):
                                temp = -int(passwordChunk[numSign + int(c)]) - int(passwordChunk[numSignOrder])
                                if signature2 == 0:
                                    signature2 = temp
                                else:
                                    if flipPositiveNegative2:
                                        signature2 = signature2 + temp
                                    else:
                                        signature2 = signature2 - temp
                                numSignOrder = numSignOrder + 1
                                flipPositiveNegative2 = not flipPositiveNegative2
                            numSignOrder = const_CountOrder
                            if signature == 0:
                                signature = signature2
                            else:
                                
                                # If positive, add the new word's positional number by positive, or else subtract it:
                                if flipPositiveNegative:
                                    signature = signature + signature2
                                else:
                                    signature = signature - signature2
                            
                            # Flip the value of manipulating the signature by + or - as being True or False respectively:
                            flipPositiveNegative = not flipPositiveNegative
                        tempCount = tempCount + 1
                    phrase2 = ''
                    for item in phrase1[:len(phrase1) - 1].split(':'):
                        
                        # Convert to shorthand hex:
                        phrase2 = phrase2 + ':' + str(hex(int(item)))[2:]
                    phrase0 = phrase2[1:].lower()
                    tempValue = True
                    while tempValue:
                        if ':0:0:0:0:0:0:' in phrase0:
                            phrase0 = phrase0.replace(':0:0:0:0:0:0:', '::')
                            break
                        elif ':0:0:0:0:0:' in phrase0:
                            phrase0 = phrase0.replace(':0:0:0:0:0:', '::')
                            break
                        elif ':0:0:0:0:' in phrase0:
                            phrase0 = phrase0.replace(':0:0:0:0:', '::')
                            break
                        elif ':0:0:0:' in phrase0:
                            phrase0 = phrase0.replace(':0:0:0:', '::')
                            break
                        elif ':0:0:' in phrase0:
                            phrase0 = phrase0.replace(':0:0:', '::')
                            break
                        tempValue = False
                    phrase = phrase + phrase0
                    continue
            
            # The word in the message is not a number or an IP address (assuming it is now a word):
            else:
                
                # Count the number of dots found (since dots are excluded from the delimiter):
                amountOfDots = word.count('.')
                if '.' in word:
                    if word.startswith('.'):
                        dotFound = 1
                    else:
                        dotFound = 2
                    word = word.replace('.', '')
                if command.lower() == 'encrypt' and ':::' in word:
                    clearScreen()
                    print(bcolors.FAIL + 'Sliicy uses ":::" to distinguish between new words and regular words. The message cannot contain three (3) consecutive colons.\n' + bcolors.ENDC)
                    return
                
                # Count the number of colons found (since colons are excluded from the delimiter):
                amountOfColons = word.count(':')
                if ':' in word:
                    
                    # If the word is a new word and also additional colons:
                    if ':::' in word and len(word.split(':')) - 1 > 3:
                        if word.endswith(':'):
                            colonFound = 2
                        else:
                            colonFound = 1
                    
                    # If the word only has 3 colons:
                    elif ':::' in word and len(word.split(':')) - 1 == 3:
                        colonFound = 0
                    
                    # If colons exist without a new word:
                    else:
                        if word.startswith(':'):
                            colonFound = 1
                        else:
                            colonFound = 2
                    if ':::' not in word:
                        word = word.replace(':', '')
                
                # Replace 'an' with 'a':
                if word.lower() == 'an': word = 'a'
                
                # Check if the word is misspelled (ie: Ive instead of I've) categorically by its character length, and fix the spelling:
                if len(word) == 2:
                    word = re.sub("im", "I'm", word, flags=re.I)
                elif len(word) == 3:
                    word = re.sub("ive", "I've", word, flags=re.I)
                    word = re.sub("hed", "he'd", word, flags=re.I)
                    word = re.sub("hes", "he's", word, flags=re.I)
                    word = re.sub("itd", "it'd", word, flags=re.I)
                elif len(word) == 4:
                    word = re.sub("aint", "ain't", word, flags=re.I)
                    word = re.sub("cant", "can't", word, flags=re.I)
                    word = re.sub("dont", "don't", word, flags=re.I)
                    word = re.sub("isnt", "isn't", word, flags=re.I)
                    word = re.sub("itll", "it'll", word, flags=re.I)
                    word = re.sub("lets", "let's", word, flags=re.I)
                    word = re.sub("shes", "she's", word, flags=re.I)
                    word = re.sub("weve", "we've", word, flags=re.I)
                    word = re.sub("whos", "who's", word, flags=re.I)
                    word = re.sub("wont", "won't", word, flags=re.I)
                    word = re.sub("yall", "y'all", word, flags=re.I)
                    word = re.sub("youd", "you'd", word, flags=re.I)
                elif len(word) == 5:
                    word = re.sub("arent", "aren't", word, flags=re.I)
                    word = re.sub("didnt", "didn't", word, flags=re.I)
                    word = re.sub("hadnt", "hadn't", word, flags=re.I)
                    word = re.sub("hasnt", "hasn't", word, flags=re.I)
                    word = re.sub("thats", "that's", word, flags=re.I)
                    word = re.sub("theyd", "they'd", word, flags=re.I)
                    word = re.sub("wasnt", "wasn't", word, flags=re.I)
                    word = re.sub("youll", "you'll", word, flags=re.I)
                    word = re.sub("youre", "you're", word, flags=re.I)
                    word = re.sub("youve", "you've", word, flags=re.I)
                elif len(word) == 6:
                    word = re.sub("doesnt", "doesn't", word, flags=re.I)
                    word = re.sub("havent", "haven't", word, flags=re.I)
                    word = re.sub("mustve", "must've", word, flags=re.I)
                    word = re.sub("mustnt", "mustn't", word, flags=re.I)
                    word = re.sub("theres", "there's", word, flags=re.I)
                    word = re.sub("theyll", "they'll", word, flags=re.I)
                    word = re.sub("theyre", "they're", word, flags=re.I)
                    word = re.sub("theyve", "they've", word, flags=re.I)
                    word = re.sub("werent", "weren't", word, flags=re.I)
                    word = re.sub("wheres", "where's", word, flags=re.I)
                elif len(word) == 7:
                    word = re.sub("couldve", "could've", word, flags=re.I)
                    word = re.sub("couldnt", "couldn't", word, flags=re.I)
                    word = re.sub("mightve", "might've", word, flags=re.I)
                    word = re.sub("nobodys", "nobody's", word, flags=re.I)
                    word = re.sub("wouldve", "would've", word, flags=re.I)
                    word = re.sub("wouldnt", "wouldn't", word, flags=re.I)
                elif len(word) == 8:
                    word = re.sub("shouldve", "should've", word, flags=re.I)
                    word = re.sub("shouldnt", "shouldn't", word, flags=re.I)
                    word = re.sub("someones", "someone's", word, flags=re.I)
                elif len(word) == 9:
                    word = re.sub("everyones", "everyone's", word, flags=re.I)
                    word = re.sub("somebodys", "somebody's", word, flags=re.I)
                elif len(word) == 10:
                    word = re.sub("everybodys", "everybody's", word, flags=re.I)
                
                # Create a variable that will indicate if the word was found yet in the wordlists:
                found = False
                
                tempValue = True
                while tempValue:
                    if len(word) == 1 and not word.isalpha(): break
                    countTypes = const_Types
                    for inspectedGroup in range(0, len(wordList1)):
                        for inspectedWord in range(0, len(wordList1[inspectedGroup])): # - 1):
                            if wordList1[inspectedGroup][inspectedWord].lower() == word.lower():
                                if command.lower() == 'encrypt':
                                    
                                    # The signature will be empty the first time. Here is where it is assigned:
                                    if signature == 0:
                                        
                                        # Set signature to the index of the current word in the big list of words:
                                        signature = next(x for x, v in enumerate(bigListOfAllWords) if v.lower() == word.lower())
                                    else:
                                        
                                        # If positive, add the new word's positional number by positive, or else subtract it:
                                        if flipPositiveNegative:
                                            signature = signature + next(x for x, v in enumerate(bigListOfAllWords) if v.lower() == word.lower())
                                        else:
                                            signature = signature - next(x for x, v in enumerate(bigListOfAllWords) if v.lower() == word.lower())
                                    
                                    # Flip the value of manipulating the signature by + or - as being True or False respectively:
                                    flipPositiveNegative = not flipPositiveNegative
                                addDots = ''
                                for i in range(0, amountOfDots): addDots = addDots + '.'
                                addColons = ''
                                for i in range(0, amountOfColons): addColons = addColons + ':'
                                ss1 = ''
                                ss2 = ''
                                if dotFound == 1:
                                    ss1 = ss1 + addDots
                                elif dotFound == 2:
                                    ss2 = ss2 + addDots
                                if colonFound == 1:
                                    ss1 = ss1 + addColons
                                elif colonFound == 2:
                                    ss2 = ss2 + addColons
                                cryptedPhrase = ''
                                try:
                                    if int(passwordChunk[countTypes]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]) >= 0:
                                        cryptedPhrase = wordList1[inspectedGroup][int(inspectedWord + int(passwordChunk[countTypes]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total])) % len(wordList1[inspectedGroup])]
                                        
                                        # Determines if the word was UPPERCASE, Normal, lowercase or neither:
                                        if word.isupper():
                                            phrase = phrase + ss1 + cryptedPhrase.upper() + ss2
                                        else:
                                            phrase = phrase + ss1 + cryptedPhrase + ss2
                                            
                                    # If the password yielded a negative result:
                                    else:
                                        answer = inspectedWord + modIt(int(passwordChunk[countTypes]) + int(passwordChunk[countOrder]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]), len(wordList1[inspectedGroup]))
                                        if answer >= len(wordList1[inspectedGroup]): answer = answer - len(wordList1[inspectedGroup])
                                        cryptedPhrase = wordList1[inspectedGroup][answer]
                                        
                                        # Determines if the word was UPPERCASE, Normal, lowercase or neither:
                                        if word.isupper():
                                            phrase = phrase + ss1 + cryptedPhrase.upper() + ss2
                                        else:
                                            phrase = phrase + ss1 + cryptedPhrase + ss2
                                except:
                                    clearScreen()
                                    print(bcolors.FAIL + 'The selected contact is either corrupt or not valid.' + bcolors.ENDC)
                                    return
                                
                                # Word was found in the list:
                                found = True
                                if command.lower() == 'decrypt':
                                    if signature == 0:
                                        signature = next(x for x, v in enumerate(bigListOfAllWords) if v.lower() == cryptedPhrase.lower())
                                    else:
                                        
                                        # If positive, add the new word's positional number by positive, or else subtract it:
                                        if flipPositiveNegative:
                                            signature = signature + next(x for x, v in enumerate(bigListOfAllWords) if v.lower() == cryptedPhrase.lower())
                                        else:
                                            signature = signature - next(x for x, v in enumerate(bigListOfAllWords) if v.lower() == cryptedPhrase.lower())
                                    # Flip the value of manipulating the signature by + or - as being True or False respectively:
                                    flipPositiveNegative = not flipPositiveNegative
                                
                                # Strength of the encryption:
                                strength = strength * len(wordList1[inspectedGroup])
                                if not readMode and not command.lower() == 'decrypt':
                                    if word.lower() in wordList2[inspectedGroup]: wordList2[inspectedGroup].remove(word.lower())
                                    wordList2[inspectedGroup].insert(0, word)
                                break
                        countTypes = countTypes + 1
                    tempValue = False
                if not found:
                    if len(word) > 1:
                        
                        # Colonized will be true if ':::' is in the word:
                        colonized = False
                        if ':::' in word: colonized = True
                        word = word.replace(':::', '')
                        cryptedPhrase = ''
                        temp2 = 0
                        for c in word:
                            for i in range(0, len(letterList)):
                                if letterList[i] == c:
                                    if int(passwordChunk[const_CountOrder + temp2]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]) >= 0:
                                        cryptedPhrase = cryptedPhrase + letterList[int(i + int(passwordChunk[const_CountOrder + temp2]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total])) % len(letterList)]
                                    
                                    # If the password yielded a negative result:
                                    else:
                                        answer = int(i + modIt(int(passwordChunk[const_CountOrder + temp2]) + int(passwordChunk[countTotal]) + int(passwordChunk[countSymbol + total]), len(letterList)))
                                        if answer >= len(letterList): answer = answer - len(letterList)
                                        cryptedPhrase = cryptedPhrase + letterList[answer]
                                    if temp2 < 256: temp2 = temp2 + 1
                        addDots = ''
                        addColons = ''
                        if dotFound == 0 and colonFound == 0:
                            if command.lower() == 'encrypt':
                                phrase = phrase + ':::' + cryptedPhrase
                            else:
                                phrase = phrase + cryptedPhrase
                        else:
                            for j in range(0, amountOfDots):
                                addDots = addDots + '.'
                            if colonized:
                                
                                # -3 due to ::: being present and -1 due to index length (-4 total):
                                for j in range(0, amountOfColons - 3):
                                    addColons = addColons + ':'
                            else:
                                for j in range(0, amountOfColons):
                                    addColons = addColons + ':'
                            if dotFound == 1:
                                if command.lower() == 'encrypt':
                                    phrase = phrase + addDots + ':::' + cryptedPhrase
                                else:
                                    phrase = phrase + addDots + cryptedPhrase
                            elif dotFound == 2:
                                if command.lower() == 'encrypt':
                                    phrase = phrase + ':::' + cryptedPhrase + addDots
                                else:
                                    phrase = phrase + cryptedPhrase + addDots
                            if colonFound == 1:
                                if command.lower() == 'encrypt':
                                    phrase = phrase + addColons + ':::' + cryptedPhrase
                                else:
                                    phrase = phrase + addColons + cryptedPhrase
                            elif colonFound == 2:
                                if command.lower() == 'encrypt':
                                    phrase = phrase + ':::' + cryptedPhrase + addColons
                                else:
                                    phrase = phrase + cryptedPhrase + addColons
                        signature2 = 0
                        flipPositiveNegative2 = True
                        if command.lower() == 'encrypt':
                            
                            # Add new unknown word to custom dictionary in contact file:
                            addedWordList.append(word)
                            
                            # Add new unknown word to the end of the big list of words:
                            bigListOfAllWords.append(word)
                            for c in str(word):
                                characterStart = 0
                                if str(c).upper() == str(c):
                                    characterStart = ord(c) - 65
                                else:
                                    characterStart = ord(c) - 71
                                temp = int(passwordChunk[letterSign + characterStart]) + int(passwordChunk[numSignOrder])
                                if signature2 == 0:
                                    signature2 = temp
                                else:
                                    if flipPositiveNegative2:
                                        signature2 = signature2 + temp
                                    else:
                                        signature2 = signature2 - temp
                                
                                numSignOrder = numSignOrder + 1
                                flipPositiveNegative2 = not flipPositiveNegative2
                            numSignOrder = const_CountOrder
                            
                            # The signature will be empty the first time. Here is where it is assigned:
                            if signature == 0:
                                signature = signature2
                            else:
                                
                                # If positive, add the new word's positional number by positive, or else subtract it:
                                if flipPositiveNegative:
                                    signature = signature + signature2
                                else:
                                    signature = signature - signature2
                            
                            # Flip the value of manipulating the signature by + or - as being True or False respectively:
                            flipPositiveNegative = not flipPositiveNegative
                        elif command.lower() == 'decrypt':
                            
                            # Add new unknown word to custom dictionary in contact file:
                            addedWordList.append(cryptedPhrase)
                            
                            # Add new unknown word to the end of the big list of words:
                            bigListOfAllWords.append(cryptedPhrase)
                            
                            for c in str(cryptedPhrase):
                                characterStart = 0
                                if str(c).upper() == str(c):
                                    characterStart = ord(c) - 65
                                else:
                                    characterStart = ord(c) - 71
                                temp = -int(passwordChunk[letterSign + characterStart]) - int(passwordChunk[numSignOrder])
                                if signature2 == 0:
                                    signature2 = temp
                                else:
                                    if flipPositiveNegative2:
                                        signature2 = signature2 + temp
                                    else:
                                        signature2 = signature2 - temp
                                numSignOrder = numSignOrder + 1
                                flipPositiveNegative2 = not flipPositiveNegative2
                            numSignOrder = const_CountOrder
                            if signature == 0:
                                signature = signature2
                            else:
                                
                                # If positive, add the new word's positional number by positive, or else subtract it:
                                if flipPositiveNegative:
                                    signature = signature + signature2
                                else:
                                    signature = signature - signature2
                            
                            # Flip the value of manipulating the signature by + or - as being True or False respectively:
                            flipPositiveNegative = not flipPositiveNegative
                        addedWordList = removeDuplicates(addedWordList)
                        bigListOfAllWords = removeDuplicates(bigListOfAllWords)
                    else:
                        tempValue = True
                        while tempValue:
                            temp = 0
                            charTemp = 0
                            if word is '`':
                                charTemp = 0
                            elif word is '~':
                                charTemp = 1
                            elif word is '!':
                                charTemp = 2
                            elif word is '@':
                                charTemp = 3
                            elif word is '#':
                                charTemp = 4
                            elif word is '$':
                                charTemp = 5
                            elif word is '%':
                                charTemp = 6
                            elif word is '^':
                                charTemp = 7
                            elif word is '&':
                                charTemp = 8
                            elif word is '*':
                                charTemp = 9
                            elif word is '(':
                                charTemp = 10
                            elif word is ')':
                                charTemp = 11
                            elif word is '_':
                                charTemp = 12
                            elif word is '+':
                                charTemp = 13
                            elif word is '-':
                                charTemp = 14
                            elif word is '=':
                                charTemp = 15
                            elif word is '{':
                                charTemp = 16
                            elif word is '}':
                                charTemp = 17
                            elif word is '|':
                                charTemp = 18
                            elif word is '[':
                                charTemp = 19
                            elif word is ']':
                                charTemp = 20
                            elif word is '\\':
                                charTemp = 21
                            elif word is ':':
                                charTemp = 22
                            elif word is '"':
                                charTemp = 23
                            elif word is ';':
                                charTemp = 24
                            elif word is "'":
                                charTemp = 25
                            elif word is '<':
                                charTemp = 26
                            elif word is '>':
                                charTemp = 27
                            elif word is '?':
                                charTemp = 28
                            elif word is ',':
                                charTemp = 29
                            elif word is '.':
                                charTemp = 30
                            elif word is '/':
                                charTemp = 31
                            elif word is ' ':
                                charTemp = 32
                            else:
                                break
                            if command.lower() == 'encrypt':
                                temp = int(passwordChunk[symSign + charTemp]) + int(passwordChunk[numSignOrder])
                            elif command.lower() == 'decrypt':
                                temp = -1 * int(passwordChunk[symSign + charTemp]) - int(passwordChunk[numSignOrder])
                            if signature == 0:
                                signature = temp
                            else:
                                if flipPositiveNegative:
                                    signature = signature + temp
                                else:
                                    signature = signature - temp
                            flipPositiveNegative = not flipPositiveNegative
                            tempValue = False
                        if dotFound == 0 and colonFound == 0:
                            phrase = phrase + word
                        else:
                            addDots = ''
                            for i in range(0, amountOfDots):
                                addDots = addDots + '.'
                            if dotFound == 1:
                                phrase = phrase + addDots + word
                            elif dotFound == 2:
                                phrase = phrase + word + addDots
                            addColons = ''
                            for i in range(0, amountOfColons):
                                addColons = addColons + ':'
                            if colonFound == 1:
                                phrase = phrase + addColons + word
                            elif colonFound == 2:
                                phrase = phrase + word + addColons
            if countOrder < 268: countOrder = countOrder + 1
            
        # Autocorrect the crypted message. Finds if vowel following the 'a' word:
        if phrase[0:2].lower() == 'a ' and (phrase[2:1].lower() == 'a' or phrase[2:1].lower() == 'e' or phrase[2:1].lower() == 'i' or phrase[2:1].lower() == 'o' or phrase[2:1].lower() == 'u'):
            phrase = 'An' + phrase[1:]
        
        # Replace 'a' with 'an' and keep capitalization:
        phrase = phrase.replace(" a A", " an A")
        phrase = phrase.replace(" a E", " an E")
        phrase = phrase.replace(" a I", " an I")
        phrase = phrase.replace(" a O", " an O")
        phrase = phrase.replace(" a U", " an U")
        
        # Replace 'a' with 'an' without capitalization:
        phrase = re.sub(" a a", " an a", phrase, flags=re.I)
        phrase = re.sub(" a e", " an e", phrase, flags=re.I)
        phrase = re.sub(" a i", " an i", phrase, flags=re.I)
        phrase = re.sub(" a o", " an o", phrase, flags=re.I)
        phrase = re.sub(" a u", " an u", phrase, flags=re.I)
        
        if command.lower() == 'decrypt':
            
            # Verify signatures:
            if not int(signature) == int(verifySignature):
                print(bcolors.WARNING + 'Signature in message: ' + str(verifySignature) + bcolors.ENDC)
                print(bcolors.FAIL + 'Signature calculated: ' + str(signature) + '\n')
                print('The signature was forged or corrupted! It is HIGHLY likely that the message was tampered with. Proceed anyways?' + bcolors.ENDC)
                choice2 = input()
                if not (choice2.lower()=='y' or choice2.lower()=='yes'):
                    clearScreen()
                    print(bcolors.FAIL + 'The decryption was aborted. Nothing was changed.\n' + bcolors.ENDC)
                    return
            else:
                print(bcolors.OKGREEN + 'Signature in message: ' + str(verifySignature))
                print('Signature calculated: ' + str(signature) + '\n' + bcolors.ENDC)
        else:
            phrase = phrase + ' ' + str(signature)
        print(bcolors.HEADER + '\nMessage:\n' + bcolors.ENDC + phrase.strip(' ') + '\n')
        if command.lower() == 'decrypt' and not readMode:
            print(bcolors.WARNING + 'Does the message look legitimate?' + bcolors.ENDC)
            choice3 = input()
            if not (choice3.lower()=='y' or choice3.lower()=='yes'):
                clearScreen()
                print(bcolors.FAIL + 'The decryption was aborted. Nothing was changed.\n' + bcolors.ENDC)
                return
        
        # Copy message to clipboard if supported by the OS:
        copiedStatus = 'Message copied to clipboard!'
        try:
            if phrase: pyperclip.copy(phrase)
        except:
            copiedStatus = "OS doesn't support clipboard copying."
        
        
        # Display how strong encryption is:
        print(bcolors.BOLD + '\nChances of breaking this encryption: 1/' + str(strength) + bcolors.ENDC)
        
        # Print how long encryption took:
        print(bcolors.OKBLUE + 'Encryption took ' + bcolors.OKGREEN + bcolors.BOLD + str(round(timeit.default_timer() - start_time, 2)) + bcolors.ENDC + bcolors.OKBLUE + ' seconds to complete.\n' + copiedStatus + bcolors.ENDC)
        
        messagebox.showinfo('STOPSTOPSTOP', 'STOPSTOPSTOP')
        messagebox.showinfo('STOPSTOPSTOP', 'STOPSTOPSTOP')
        if not readMode:
            if command.lower() == 'decrypt':
                output2 = re.split(r'[ ~`!@#$%^&*()_=+,.<>/?;:"{}\|\n\[|\]]+', phrase)
                for word in output2:
                    for l in range(0, len(wordList1) - 1):
                        for m in range(0, len(wordList1[l]) - 1):
                            if wordList1[l][m].lower() == word.lower():
                                if word.lower() in wordList2[l]: wordList2[l].remove(word.lower())
                                wordList2[l].insert(0, word)
            final = ''
            for i in range(0, len(wordList2)):
                wordList2[i] = removeDuplicates(wordList2[i])
                final = final + '\n\n' + '\n'.join(map(str, wordList2[i]))
            final = final.lstrip()
            if len(addedWordList) > 0:
                addedWordList = removeDuplicates(addedWordList)
                final = final + '\n' + '\n'.join(map(str, addedWordList))
            with open(user, 'w', encoding='utf-8') as text_file:
                text_file.write(lineSplit[0] + '\n\n' + lineSplit[1] + '\n\n' + final)
    elif command.lower() == 'change':
        
        # Check if any fields are left blank:
        if not user or not password:
            clearScreen()
            print(bcolors.FAIL + "Operation failed because nothing can be left empty!\n" + bcolors.ENDC)
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
                    lineSplit = open(os.path.join(user, filename), 'r', encoding='utf-8').read().split("\n\n")
                    if "Sliicy" in open(os.path.join(user, filename), 'r', encoding='utf-8').read() and ";" in open(os.path.join(user, filename), 'r', encoding='utf-8').read():
                        listOfFiles = listOfFiles + 1
                        print(str(listOfFiles) + ": " + filename)
            if listOfFiles == 0:
                clearScreen()
                print(bcolors.FAIL + 'No contacts were found in the specified folder.\n' + bcolors.ENDC)
                return
            else:
                print(bcolors.WARNING + bcolors.BOLD + '\nThe above files located in "' + user + '" WILL BE MODIFIED! This cannot be undone. Proceed?' + bcolors.ENDC)
            
            # If the user proceeds:
            choice2 = input()
            if choice2.lower()=='y' or choice2.lower()=='yes':
                for filename in os.listdir(user):
                
                    # If the file has a .txt extension:
                    if ".txt" in filename:
                        
                        # Read contents of file:
                        lineSplit = open(os.path.join(user, filename), 'r', encoding='utf-8').read().split("\n\n")
                        
                        # If file contains "Sliicy" and semicolons:
                        if "Sliicy" in open(os.path.join(user, filename), 'r', encoding='utf-8').read() and ";" in open(os.path.join(user, filename), 'r', encoding='utf-8').read():
                        
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
                            with open(os.path.join(user, filename), 'w', encoding='utf-8') as text_file:
                                text_file.write("\n\n".join(map(str, lineSplit)))
            
                # Set the new password:
                password = newPassword
                clearScreen()
                print("Password successfully changed in selected folder!")
            else:
                clearScreen()
                print(bcolors.FAIL + 'Nothing was changed!' + bcolors.ENDC)
# End of Sliicy function.

# Handle arguments passed to script:
if len(sys.argv) > 1:
    parser = argparse.ArgumentParser(description='Symmetrically encrypt/decrypt messages based on parts of speech. Cannot be cracked by humans or Quantum Computers.', usage='sliicy.py [Optional Arguments]')
    parser.add_argument('integers', metavar='N', type=int, nargs='+', help='an integer for the accumulator')
    parser.add_argument('--new', dest='newcontact', action='store_const', const=sum, default=max, help='start a new chat')
    parser.add_argument('--join', dest='joincontact', action='store_const', const=sum, default=max, help='join an existing chat')
    parser.add_argument('--encrypt', dest='encrypt', action='store_const', const=sum, default=max, help='encrypt a message')
    parser.add_argument('--decrypt', dest='decrypt', action='store_const', const=sum, default=max, help='decrypt a message')
    parser.add_argument('--change', dest='changepassword', action='store_const', const=sum, default=max, help='change the password')
    
    args = parser.parse_args()
    print(args.accumulate(args.integers))

# Copy results to clipboard:
try:
    import pyperclip
except:
    print("Pyperclip not installed on current OS. Messages won't be copied to clipboard.")

# Handle Ctrl + C:
try:
    # Retrieve password and clear screen twice for full privacy:
    print(bcolors.BOLD + 'Welcome to Sliicy!\n' + bcolors.HEADER + 'If you have an existing password, enter it now. Otherwise, please choose a password:\n' + bcolors.WARNING + 'Please note: Sliicy will never be able to verify if the password is typed correctly!' + bcolors.ENDC)
    
    # Input is typed plain-text because Sliicy can't verify if the password is correct. If the user mistypes the password, all subsequent messages will be corrupted.
    password = input()
    clearScreen()
    attempt = 0
    while password == '':
        attempt = attempt + 1
        if attempt >= 3:
            clearScreen()
            print(bcolors.FAIL + "User entered no password. Not sure why. History erased for privacy." + bcolors.ENDC)
            sys.exit()
        clearScreen()
        print(bcolors.FAIL + "Password can't be empty!\nPlease enter your current or new password!\n" + bcolors.ENDC)
        password = input()
    clearScreen()
    
    # Temporarily store typed text in memory for autocomplete and ease of use, after the password is entered:
    # Will not work on Windows, because readline is not included in Windows:
    try:
        import readline
        import rlcompleter
        readline.parse_and_bind('tab: complete')
        del readline, rlcompleter
    except:
        print(bcolors.WARNING + "\nAutocomplete and temporary history won't work because the current OS doesn't support it. Read more here:\n" + bcolors.UNDERLINE + "https://stackoverflow.com/questions/6024952/readline-functionality-on-windows-with-python-2-7\n" + bcolors.ENDC)
    
    # Main screen:
    print(bcolors.BOLD + bcolors.OKGREEN + '\nWelcome to Sliicy!\n' + bcolors.ENDC)
    while True:
        print(bcolors.HEADER + 'Select a choice below:\n' + bcolors.ENDC)
        print('1:  Start a ' + bcolors.UNDERLINE + 'n' + bcolors.ENDC + 'ew group')
        print('2:  ' + bcolors.UNDERLINE + 'J' + bcolors.ENDC + 'oin another chat')
        print('3:  ' + bcolors.UNDERLINE + 'E' + bcolors.ENDC + 'ncrypt a message')
        print('4:  ' + bcolors.UNDERLINE + 'D' + bcolors.ENDC + 'ecrypt a message')
        print('5:  ' + bcolors.UNDERLINE + 'C' + bcolors.ENDC + 'hange a password')
        print('6:  ' + bcolors.UNDERLINE + 'H' + bcolors.ENDC + 'elp, about, logs')
        print('7:  E' + bcolors.UNDERLINE + 'x' + bcolors.ENDC + 'it\n')
        choice = input()
        if choice=='1' or choice.lower()=='n' or choice.lower()=='new' or choice.lower()=='start':
            print('Enter your name (' + getpass.getuser() + '):')
            user = input()
            print("Enter recipient's name:")
            sliicy("create", user, password, None, input())
        elif choice=='2' or choice.lower()=='j' or choice.lower()=='join' or choice.lower()=='existing':
            print('Loading, please wait...')
            root.filename = filedialog.askopenfilename(title = "Choose existing contact to join", filetypes = (("Slii Files","*.slii"), ("All Files","*.*")))
            root.withdraw()
            if root.filename:
                sliicy("join", root.filename, password)                
            else:
                clearScreen()
                print(bcolors.FAIL + "No chat selected!\n" + bcolors.ENDC)
        elif choice=='3' or choice.lower()=='e' or choice.lower()=='encrypt':
            print('Select a contact:')
            root.filename = filedialog.askopenfilename(title = "Choose contact", filetypes = (("Text Files","*.txt"), ("All Files","*.*")))
            root.withdraw()
            if root.filename:
                if not root.filename.lower().endswith('.slii'):
                    print('Enter your message:')
                    sliicy("encrypt", root.filename, password, input(), None, None, False)
                else:
                    clearScreen()
                    print(bcolors.FAIL + "Selected contact isn't in a chat yet! Please first join the chat!\n" + bcolors.ENDC)
            else:
                clearScreen()
                print(bcolors.FAIL + "No contact selected!\n" + bcolors.ENDC)
        elif choice=='4' or choice.lower()=='d' or choice.lower()=='decrypt':
            print('Select a contact:')
            root.filename = filedialog.askopenfilename(title = "Choose contact", filetypes = (("Text Files","*.txt"), ("All Files","*.*")))
            root.withdraw()
            if root.filename:
                if not root.filename.lower().endswith('.slii'):
                    print('Enter your message:')
                    sliicy("decrypt", root.filename, password, input(), None, None, False)
                else:
                    clearScreen()
                    print(bcolors.FAIL + "Selected contact isn't in a chat yet! Please first join the chat!\n" + bcolors.ENDC)
            else:
                clearScreen()
                print(bcolors.FAIL + "No contact selected!\n" + bcolors.ENDC)
        elif choice == '5' or choice.lower() == 'c' or choice.lower() == 'change' or choice.lower() == 'password':
            print('Please select the folder containing all of your contacts:')
            root.filename = filedialog.askdirectory()
            print('\nYou have selected: ' + root.filename + '\n')
            root.withdraw()
            if not root.filename:
                clearScreen()
                print(bcolors.FAIL + 'No folder selected!\n' + bcolors.ENDC)
            else:
                print('Enter your new password:')
                newPassword = input()
                print('Re-enter your new password:')
                comparePassword = input()
                clearScreen()
                if newPassword == comparePassword:
                    sliicy('change', root.filename, password, None, None, newPassword)
                else:
                    print(bcolors.FAIL + "Passwords don't match!" + bcolors.ENDC)
        elif choice=='6' or choice.lower()=='h' or choice.lower()=='help':
            clearScreen()
            print('\n' + 'Version: ' + bcolors.OKBLUE + bcolors.BOLD + str(version) + bcolors.ENDC + '\nSliicy encrypts your conversations by scrambling words together.')
            print('Only trust the official Sliicy app from ' + bcolors.OKGREEN + 'Sliicy.com' + bcolors.ENDC + ' or ' + bcolors.UNDERLINE + bcolors.OKGREEN + 'https://github.com/Sliicy' + bcolors.ENDC)
            print('\nCreated by Sliicy.\n')
        elif choice=='7' or choice.lower()=='exit' or choice.lower()=='q' or choice.lower()=='quit' or choice.lower()=='end' or choice.lower()=='stop' or choice.lower()=='x':
            clearScreen()
            foo = ['Goodbye', 'Adios', 'See you later', 'Take care', 'Exiting', 'Have a good day', 'Come back next time', 'Pleasure doing business', "Quick! Someone's coming"]
            print(bcolors.OKGREEN + random.choice(foo) + "! History erased for privacy." + bcolors.ENDC)
            break
        else:
            clearScreen()
            print(bcolors.FAIL + '\nSorry, please try again!\n' + bcolors.ENDC)
except KeyboardInterrupt:
    clearScreen()
    print(bcolors.FAIL + "Control + C was used to abort. History erased for privacy." + bcolors.ENDC)
    sys.exit()
