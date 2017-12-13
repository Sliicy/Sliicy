# Sliicy

<img src="logo.png" width=150 align=right>

Sliicy is a honeypot encryption algorithm that substitutes words instead of characters to obfuscate messages sent between parties. The messages are "sliced" into groups of nouns, verbs, adjectives, adverbs, past and present tense, and others with one another. It requires a secure line for only the initial setup between parties involved. Once set up, everything sent is gibberish but grammatically accurate. It's basically a huge game of [Madlibs](http://www.madlibs.com/)!

# Sliicy can be used alongside anything! Including: SMS, Email, Snail mail, Phone calls, WhatsApp, even paper airplanes!

## Usage
Use Sliicy at own risk! I'm not responsible for misuse or illegal use!
Referring to Sliicy in VB.NET example: (Code ~~is~~ _will be_ compatible with C#, Java, Python...)
```vbnet
Sliicy.ChangePassword(oldPassword, newPassword, file)
Sliicy.Decrypt(password, file, message, optional readonly)
Sliicy.Encrypt(password, file, message, optional readonly)
Sliicy.Join(password, file)
Sliicy.New(password, file)
```

An example of a Windows console running Sliicy:
_Alice encrypts her message with her own **secretpassword** (known only to her) with the **contact_Bob.txt** file (which has the saved password for talking to Bob with). The unencrypted message is, **"Ok, Bin Laden was assassinated."**_
```
Sliicy.exe -encrypt secretpassword contact_Bob.txt Ok, Bin Laden was assassinated.
```
_Bob decrypts the message with his own **secretpassword** with the **contact_Alice.txt** (that has the saved password for talking with Alice). The encrypted message is, **"So, Elmo Gerald was overjoyed."**_
```
Sliicy.exe -decrypt secretpassword contact_Alice.txt So, Elmo Gerald was overjoyed.
```

## Motivation

I created Sliicy because I was tired of [NSA tactics](https://en.wikipedia.org/wiki/Vault_7) and the [weakness of AES and other ciphers against Quantum Computers](https://security.stackexchange.com/questions/116596/will-quantum-computers-render-aes-obsolete). Honey encryption is a relatively new method [currently being implemented](https://www.technologyreview.com/s/523746/honey-encryption-will-bamboozle-attackers-with-fake-secrets/). It replaces encrypted data with false data if the password is incorrect. Why not try this with messages? I admit I would not have figured out this idea without Talmudic Study. It helped sharpen my mind to fix flaws with my algorithm.

## Installation

Simply download the Sliicy application relative to your OS and run! No installation needed. Security-minded individuals should check the SHA1sum of their downloaded copies to match the ones posted in this Repository.
You can upload your copy of Sliicy to verify the SHA1sum signatures and check for malware at [VirusTotal](https://www.virustotal.com/).

## API Reference

Creating a new contact:
Sliicy.exe -n password contact

Joining a created contact:
Sliicy.exe -j password contact.slii

Encrypting a message:
Sliicy.exe -e password contact message

Decrypting a message:
Sliicy.exe -d password contact message

Changing the user's password:
Sliicy.exe -c oldPassword newPassword file/folder

Help:
Sliicy.exe -h

Read-only mode is used for messaging **without** changing the password. It should only be used for testing:
Sliicy.exe -e -r password contact message
Sliicy.exe -d -r password contact message

## Contributors

Anyone who can help find mistakes or suggest words to be added to the wordlist would really speed things up.

## License

Icons and images in this work is copyrighted. The code is not yet available for the public. License is GNU AGPLv3.

## Acknowledgements
[Hashem](http://www.simpletoremember.com/articles/a/seven-laws-of-noah/) for helping me.

[Google 10000](https://github.com/first20hours/google-10000-english) helped generate thousands of words.

[WordNet](http://wordnet.princeton.edu) helped sort out Parts of Speech of many words.

[Humanizer](https://github.com/Humanizr/Humanizer) for helping sort out plural and singular word types.

Judy Stern for her help with negative modulos (Microsoft [decided to frustrate VB programmers](http://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain) when finding the modulo of negative values).

## Further Reading
[Sliicy Documentation](https://github.com/Sliicy/Sliicy/blob/master/Sliicy%20Documentation.pdf)

[Official Sliicy Website](http://www.sliicy.com/)
<p>Copyright <span>&copy; 2017&nbsp;</span>All rights reserved.</p>
