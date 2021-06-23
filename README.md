# MaximoChatBE
Hello jobSity tester!
Here you will find the back end for my chat web app

In order to run it you need the .NET 5.0 & .NET Core 3.1 SDKs (or just runtimes) => https://dotnet.microsoft.com/download 

Then:

1) Clone this repo => git clone https://github.com/maximojgonzalezc/MaximoChatBE.git
2) Open ChatService.sln solution with Microsoft Visual Studio 2019 IDE (https://visualstudio.microsoft.com/en/)
3) Restore dependencies (this proccess is triggered when open in visual studio IDE, otherwise execute 'dotnet restore' command within solution folder)
4) Go to solution properties, then select 'Multiple startup projects' and in the Action dropdown select 'Start' in both ChatBot and ChatService
5) Run both ChatService and ChatBot project (multiple startup projects) by clicking 'Start'
