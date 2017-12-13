## Sliicy
Sliicy is an attempt to create an original encryption algorithm.

<img src="logo.png" width=150 align=right>
## The problem

Nearly all encryption standards share a common problem: with enough time or processing, [**any** encryption will be broken](https://www.smithandcrown.com/8655/).

## Why?

When decrypting a message, one will either end up with the correct message, an illegible message, or an error message.

Given the [advancements](https://web.archive.org/web/20170728233004/https://www.nbcnews.com/mach/tech/quantum-computers-just-moved-step-closer-reality-ncna787481) quantum computers have made, it is certain that [**no encryption will be safe**](https://www.howtogeek.com/166832/brute-force-attacks-explained-how-all-encryption-is-vulnerable/amp/) at some point in the near future. A cracking software only needs to check the decryption for errors and/or legibility, which is a very redundant and repeating task. **Not even [RSA](https://security.stackexchange.com/questions/87345/how-many-qubits-are-needed-to-factor-2048-bit-rsa-keys-on-a-quantum-computer) is safe.**

[WhatsApp](https://www.whatsapp.com/) claims to be [end-to-end encrypted](https://www.whatsapp.com/security/). However, they are the ***sole*** distributor of the password keys, which by definition violates the concept of end-to-end, and has [proven in the past to retain backups](https://www.theguardian.com/technology/2017/jan/13/whatsapp-design-feature-encrypted-messages). WhatsApp is [just as vulnurable to being cracked](https://www.whatsapp.com/security/WhatsApp-Security-Whitepaper.pdf) by time and processing **as is** anything else.

[Signal](https://whispersystems.org/), although open source, also risks being broken into by repetitive cracking.

Both WhatsApp and Signal [depend on servers](https://en.wikipedia.org/wiki/Signal_(software)) to transfer messages. They are prone to [unreliability](https://motherboard.vice.com/en_us/article/padnvm/200-terabyte-proof-demonstrates-the-potential-of-brute-force-math) in the long run.

[Comparison of other Instant Messaging protocols.](https://en.wikipedia.org/wiki/Comparison_of_instant_messaging_protocols)

## What makes Sliicy any different?

Sliicy doesn't change the message itself; it changes the ***meaning*** of the message by splitting up the words into alternative parts of speech. Anyone without the correct password will have an awkward looking message, not necessarily right or wrong. With so much ambiguity per message, even people will find it impossible to predict the original meaning behind the message. Both A.I. and human-beings will have **no clue** what they are looking for.

For example:

_**"Eve sold the gun to Herbert."**_

In this message, we have two (2) names, a past tense verb, a singular noun, and two (2) connecting words. Sliicy switches all the words around, noun for noun, verb for verb, etc, to form a new message based on the user's password, such as:

_**"Jerry ate a chair with Drake."**_

This new message is sent out to the recipient. Without the correct passwords, it is statistically impossible to guess the original context/meaning.

A.I. can be trained to look for patterns, such as "killing", "murder", etc. It can looking for the most common phrases, and it will still need to know language quirks, exaggerations and exceptions to produce a viable list of possible messages. All of this would only be able to produce an enormous list of possible permutations which the bad guys would need to go through. In the end, the meaning will never be recoverable, such as if the sender meant "red" or "blue"; "hot" or "cold"; "tomorrow" or "today".

The obvious limitation for Sliicy is the fact that both users need to securely share the password one (1) time ([symmetric encryption](https://support.microsoft.com/en-us/help/246071/description-of-symmetric-and-asymmetric-encryption)). Sliicy also only works for words and numbers; this doesn't yet work for images, audio, video or raw data.

Sliicy is a honeypot 🍯. A luring encryption which can yield anything as the decryption. It's [Madlibs](http://www.madlibs.com/) on steroids!

## Sliicy is compatible with anything! WhatsApp, SMS, Email, Phone Calls, Snail mail, even paper airplanes!
💬 📧 📬 📞 📄 ✈️

Because of the nature of this encryption, one can use it alongside anything else. This means it is decentralized, no online data stored anywhere, and no servers or logins.

## Motivation

I created Sliicy out of concern for the [weakness of AES and other ciphers against Quantum Computers](https://security.stackexchange.com/questions/116596/will-quantum-computers-render-aes-obsolete). [Honey encryption](https://en.wikipedia.org/wiki/Honey_Encryption) is a relatively new method [currently being implemented](https://www.technologyreview.com/s/523746/honey-encryption-will-bamboozle-attackers-with-fake-secrets/). An incorrect password will yield false data. I attempted to try the same with messages. I admit that I wouldn't have figured out this idea without Talmudic Study. It helped sharpen my mind to fix flaws with my algorithm.

## Installation

Simply download the Sliicy app and run! No installation. Security-minded individuals should check the SHA256 checksum of their downloaded copies to match the ones posted in this Repository.
Please upload your copy of Sliicy to [VirusTotal.com](https://www.virustotal.com/) to verify the SHA256 signatures and check for malware.

## How it works

There are 4 main functions:

* Creating a contact

* Joining a contact

* Encrypting / decrypting a message

* Changing a password

The full steps can be read in the Documentation.

## Contributing

Anyone who can help find mistakes or suggest words to be added to the wordlists is crucial to the encryption.

Help is needed to translate this project into other languages.

I look forward to people trying to test the included program and break it. Comments are welcome.

Offering a reward for anyone who breaks the encryption.
