# HorizonTCBGviewer
# Sync Background Images with an FTP-Server and write system- and room-information’s to an Background Image 

![Alt text](DemoDesktopt.jpg?raw=true "Demo Sreen Shoot")

HorizonTCBGviewer was designed for thin client computer rooms on university campuses. 

It is a simple tool to write system information’s (available free hard drive space, the network status and uptime) directly inside an windows background image. The tool decodes with the windows Device name (e.g. “TC-D11-158-2”) the Room Number (here: “D11-158”) and the PC Number (here: “2”) and writes them also to the Background image. 
After a restart of the tool is the Background image synced with an FTP Server and only change if a new image is available (This is detected by the modification date of the image file).  The App can detect if someone changed the Windows Background Image manually and then update the image automatically back. The Background image is stored as an JPEG-File and can for example contain a Manuel of the usage of the computer room and a logo.   
HorizonTCBGviewer never sends any wired messages to a user. Instead status messages about the FTP connection are only written to an Info Window. This Window is accessible by a click on the Windows Minibar Icon of the App.
The App was originally designed for small Windows 10 IoT embedded Thin-Clients running Vmware Horizon desktop virtualization. 

To porting this app your infrastructure following code changes are required: 
1.	“*DynamicVals.cs*” --> Change here your FTP Server Name, Address and Path to the Image
2.	 “DynamicVals.cs : DecodeComputerName()*” --> This Method is responsible to decode the room- and PC- Number from a Windows Device name. If you use a different Device name format please change this method    
3.	“*FTPdowloadHandler.cs*” --> Select here your FTP User Name and Password 
4.	“*ViewModels/RoomAndNoModel.cs*” --> Edit this file to add or change information’s to show on the background Image

Recompile and publish the App, put an Image to a FTP-Server, install the app with Windows Click once on your clients, put the application on your clients to the startup folder and **enjoy**!   

# Author
* **Robin Sebastian**

*HorizonTCBGviewer* is a project, that I have fully developed on my own. No companies are involved in this project.
Today I'm a Master Student of electronic engineering with the major embedded systems. 

[![Ask Me Anything !](https://img.shields.io/badge/Ask%20me-anything-1abc9c.svg)](mailto:mail@robseb.de)

