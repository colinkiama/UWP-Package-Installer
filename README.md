# UWP-Package-Installer
An UWP installer for appx/appxbundle packages

You can download the installer here: https://github.com/colinkiama/UWP-Package-Installer/releases

Need help or have questions? Contact me at colinkiama@gmail.com
 
![App Demonstration](screenshot.gif)


The App installs the packages with or without their dependencies and displays errors in case something goes wrong.

Devices on Windows 10 Creators Update and Later get their install progress in the action center. Devices on older versions of Windows 10 have their install progress inside the installer's app UI and a few notifications in the app.

This is fully capable of replacing the buillt in app installer on Windows 10 and Windows Device Portal (for sideloading apps onto your device).

Feel free to use this code for your own projects too. WARNING: The methods used in the Package Manager class in this app do require you to manually add the following restricted capablities into your package.appxmanifest file: "packageManagement". You can learn more about restricted capablities in UWP apps here (You might need to scroll down a bit): https://docs.microsoft.com/en-us/windows/uwp/packaging/app-capability-declarations


Enjoy😁!

## How to install an uncertified UWP App
![GIF Tutorial](uncertified-uwp-install.gif)

## Roadmap:
- [ ] Install Certificates before packages
- [ ] Try installing the package without dependencies when app fails to install the first time
