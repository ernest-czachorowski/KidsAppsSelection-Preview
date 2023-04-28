# KidsAppsSelection

KidsAppsSelection is an application that I am creating as part of my portfolio using MAUI Blazor and Blazor WebAssembly. The application is designed to provide a list of games and applications suitable for children.

### Note:
 
The application is **not yet finished**, and the data included in it is only for testing purposes. Nevertheless, I have decided to make the unfinished application public for **demonstration purposes**.

### Features:

The application contains a list of games and applications for children. Users can add games (currently only for Android) and search for games.

### Technologies Used:

KidsAppsSelection is a .NET Blazor MAUI application that can be used as a client application for mobile devices or as a web service developed in Blazor WebAssembly. The application server is developed in .Net Core using Entity Framework SQL-Lite and AutoMapper. The user interface is created using MudBlazor.

### Installation:

To run the application, you need to clone the repository and run it in the latest version of Visual Studio. It should work straight away after pressing Run button in Visual Studio.

If only server project start, without any client interface visible, it means you need to add other projects to be auto started by Visual Studio in **Properties/Run/Configuration** on MacOS or **Properties/Common Properties/Startup Projest/Multiple startup projects** on Windows.

Sometimes, proper functioning of the application may require changing the IP and port numbers in the **MauiProgram.cs** and **Program.cs** files on the client app and **/Properties/launchSettings.json** files in every project. Additionally, you will need properly configured simulators/emulators for iOS and Android to ensure full functionality of the application.

### Conclusion:

KidsAppsSelection is a work in progress, and I will continue to improve and develop it over time. I hope that it will become a useful resource for parents and guardians looking for fun and engaging apps for their children. If you have any feedback or suggestions, feel free to contact me.

### Demo: (you may need to wait a while for demo.gif to load...)
<img src="https://github.com/ernest-czachorowski/KidsAppsSelection-Preview/blob/main/demo.gif" width="800" />
