# Instructions

### Demo 1 Set up your development environment
- Open a browser and navigate to https://visualstudio.microsoft.com/
- Scroll down, and choose the version you wish to install
    - Recommended is Visual Studio Community
    - Linux users can download Visual Studio Code
        - This is a lightweight version of the IDE and will require console commands to create and run the applications
- If you chose Visual Studio Community (Windows or Mac)
    - When the download is done, the Visual Studio Installer will appear on the screen
    - Make sure to select  'ASP.NET and web development' on the Workloads tab
    - Select 'Install while downloading'
    - Click continue and wait for the installation to complete
    - Once it's done, open Visual Studio
    - Select new Web application and create a new ASP.NET Core MVC application
    - Run the application by clicking on the green 'play' menu at the top
    - Verify application is running on Localhost
- If you chose Visual Studio Code (Linux)
    - Open the installation file and follow the wizard to complete the installation
    - On a browser, navigate to https://dotnet.microsoft.com/en-us/ and download the latest version of .NET
    - After the installation of both components is done:
        - Open Visual Studio Code
        - Click File > Open Folder and select an empty folder in your computer
        - Click on Terminal > New Terminal and run the following command
            □ dotnet new mvc
        - Wait for the command above to complete, and verify that the project files where generated successfully
        - Back on the terminal, run the following command
            □ dotnet run
        - Verify that the application is running on Localhost
    - Refer to this CLI command cheat sheet for a guide to different commands: [dotnet cli Cheat Sheet by oba](https://cheatography.com/oba/cheat-sheets/dotnet-cli/)

### Demo 2 Sign up for GitHub
- Refer to the following video [GitHub Desktop Tutorial - How to install and create a new repository in GitHub using GitHub Desktop?](https://www.youtube.com/watch?v=sObZ61W66GU)
- Open a browser and navigate to https://desktop.github.com/
    - Download the latest version
    - Follow the wizard to complete the installation
- Open another tab in your browser and navigate to https://github.com/
    - Sign up for an account OR login with an existing one
- Following the instructions in the video
    - Open GitHub Desktop on your computer
    - Sign in with your GitHub account
    - Create a new repository called COMP2084
    - Create a new folder called lesson01 and copy over the application you created in part 1
    - Commit your changes and publish your repository as PRIVATE
Invite me as a collaborator