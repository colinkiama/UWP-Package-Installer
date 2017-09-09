# UWP-Package-Installer
An UWP installer for appx/appxbundle packages (Runs on all Windows 10 Devices but was originally made for Mobile devices)

The App installs the packages with or without their dependencies and displays errors in case something goes wrong.

Devices on Windows 10 Creators Update and Later get their install progress in the action center. Devices on older version of have their install progress inside the installer's app UI and a few notifications in the app.

This is fully capable of replacing the buillt in app installer on Windows 10 and Windows Device Portal (for sideloading apps onto your device).

Feel free to use this code for your own projects too. WARNING: The methods used in the Package Manager class in this app do require you to manually add the following restricted capablities into your package.appxmanifest file: "packageManagement". You can learn more about restricted capablities in UWP apps here (You might need to scroll down a bit): https://docs.microsoft.com/en-us/windows/uwp/packaging/app-capability-declarations

If you you need help, or want to talk to me about something, contact me at colinkiama@gmail.com

Enjoy😁!
