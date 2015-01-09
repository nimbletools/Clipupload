http://clipupload.net/

ClipUpload is made by Angelo Geels.
Hit me up on Twitter: @Ansjh / http://twitter.com/ansjh
Or visit my website: http://angelog.nl/


=======================================
   CONTENTS
=======================================

 - Installing
 - Troubleshooting
 - More info
 - Legal

=======================================
   INSTALLING
=======================================

 - Extract this RAR to some place on your harddrive. I recommend having a folder somewhere on your C: drive, like C:\ClipUpload, but of course this is personal preference.
 - Open ClipUpload4.exe. Done!

OR

 - Simply run the installer.

=======================================
   TROUBLESHOOTING
=======================================

Q: How do I change addon settings?
A: Right click an addon in the main window and click on Settings.

Q: Can I make ClipUpload start at launch?
A: Yes. In ClipUpload, go to Settings, then check the "Start with Windows" checkbox. Click OK.

Q: ClipUpload doesn't launch. Why?
A: You're gonna be a bit more specific than that, but I'll bite. You might not have installed the .Net Framework 4.0. This will cause ClipUpload to not be able to start, as it depends on that framework. Get it here: https://www.microsoft.com/download/en/details.aspx?id=17851

Q: The application crashes when I launch it. Why?
A: Some general cause may be that you didn't extract all the files, and you forgot some. Try re-installing. (See INSTALLING in this readme.)

Q: So many Addons, but I only want FTP! How do I do that?
A: Just right click on any addon you want to disable/enable and click on Disable or Enable. If you wanna get rid of an addon alltogether so it won't even show up anymore on the main window, just go into your installation folder and remove the directory of the addon in the Addons folder. For example, remove the Addons/Imgur folder if you want to get rid of Imgur.

Q: How do I report a broken addon?
A: If the addon is made by me, please see the question below on reporting bugs. If it's not, please do not contact me about it, because I can't help you in that case.

Q: How do I report a bug?
A: Please send a bug report to angelo@clipupload.net. (Only if it's not on the below list of known bugs!) Thanks!

Q: Can I donate?
A: Yes, you can! You can do this through Sourceforge via PayPal on the following page: http://sourceforge.net/donate/index.php?group_id=340379

Q: My anti-virus detects a virus in ClipUpload! Am I infected?
A: No. ClipUpload uses a .Net library which allows ClipUpload to make use of global shortcuts. Your anti-virus just says it might be dangerous as it detects stuff in the code regarding keyboard interaction. It is not a virus, however.

Q: Animation encoding takes way too long! What do I do?
A: The encoding of gif animations can take a rather long time, depending on the amount of colors you are trying to cram into the animation. Generating the palette to use can be a very CPU intensive task. Multithreading of generating this palette will be coming soon, as well as a general performance boost for the generation of these.


=======================================
   MORE INFO
=======================================

More info on developing addons and ClipUpload can be found at:

 - http://wiki.clipupload.net/
 - http://clipupload.net/

You can contact me for support like bugs or feature requests at angelo@clipupload.net.

=======================================
   LEGAL
=======================================

Some icons are made by http://pixel-mixer.com/, http://dryicons.com and http://www.fatcow.com/free-icons
Included icons for Imgur, Pastebin, Dropbox and Facebook are copyrighted by their respective owners.

SFTP addon uses SSH.NET library:
Copyright (c) 2010, RENCI
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of RENCI nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
