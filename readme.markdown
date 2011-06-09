iTunes Remote for Windows Phone 7
=========================================

This is a demo project I'm putting together for a talk I'm giving on June 23, 2011 at the [Boulder Silverlight Users Group](http://www.meetup.com/Boulder-Silverlight-Users-Group/).

The talk is to illustrate several topics:

1. Embedding simple web servers in Windows services and applications (in this case using [Nancy](http://elegantcode.com/2010/11/28/introducing-nancy-a-lightweight-web-framework-inspired-by-sinatra/))
2. Creating simple REST services that can be hosted in a simple web server for communicating with applications or services 
3. Creating a Silverlight applications for Windows Phone 7 that can talk to these services (in this case, for automating iTunes running on your home theater PC, but ultimately for whatever interesting purposes you can conjure up). 
4. Making the asynchronous communications on WP7 easier and more robust using the [Reactive Extensions](http://msdn.microsoft.com/en-us/data/gg577609).

Requirements
------------

The agent program for controlling iTunes and hosting the REST service just requires a Windows 7 or Vista PC running iTunes (it might work on XP, I haven't tried it yet).

The control application requires a Windows Phone 7 device (or the emulator). This app probably won't be going up on the market (other iTunes remotes for WP7 already exist; this is just for teaching purposes), so you'll need to have an unlocked device to deploy it.

Dependencies
------------
* [MvvmLight](http://mvvmlight.codeplex.com/)
* [Newtonsoft Json](http://james.newtonking.com/pages/json-net.aspx)
* [Nancy](http://elegantcode.com/2010/11/28/introducing-nancy-a-lightweight-web-framework-inspired-by-sinatra/)

Installation
------------
For the agent, build and run the setup project on the machine with iTunes.
For the WP7 application, you'll need to deploy to your phone using Visual Studio 2010.

Disclaimer
----------
This is project for demonstration purposes for the talk. Use of this code is at your own risk. If this thing somehow erases your iTunes library or sets your phone on fire, you were warned.