# Welcome to my coding challenge solution

This Github repository contains the solution and completed installation for PlayStudios Asia Backend Coding Challenge (test 2)


# How to build and run from source
## Prerequisites
You'll need the following tools:

 - [git](https://git-scm.com/)
 - [Dotnet core SDK](https://dotnet.microsoft.com/en-us/download) Version 3.1 or higher
 - [postman](https://www.postman.com/downloads/) for testing purpose
 - [Visual studio](https://visualstudio.microsoft.com/) 2017 or higher (recommend 2019) or [Visual studio code](https://code.visualstudio.com/)
 

## Build an run
**Getting the source**
first, choose a folder that need to clone the repository.
Hit right click mouse to open git bash:
![enter image description here](https://2.pik.vn/202247695fce-73f6-4109-a555-7aa66590b934.png)

Once git bash is opened, type the below command and hit enter to start cloning repository:

        git clone https://github.com/hungphamuy95/ps-coding-challenge.git
If you see the below message, it means that the cloning process is completed successfully:

![enter image description here](https://2.pik.vn/20224c63e726-91eb-4840-b96d-adcb6fb0790f.png)

**Run the source**
If you already installed visual studio (VS), double click the sln file and the solution should be opened in VS.
Please wait few minutes or click restore nuget option.

![enter image description here](https://2.pik.vn/202232e0b1e8-72cd-4b2f-bb6b-33ce3c90b5bc.png)

Click start of hit F5 button to run the project
If the swagger document is loaded, it means that you can can run the source.

![enter image description here](https://2.pik.vn/20224c58c3be-111d-4727-b178-83fab2780c32.png)

You can also run the source by dotnet cli:
Open the folder that contains source code the click right mouse button to open the terminal:

![enter image description here](https://2.pik.vn/20223185341a-28af-47dd-b7c5-335eba573157.png)

Type the following command to restore the source

    dotnet restore
 Open `ps-coding-challenge` folder then type the following command in cmd:
 

    dotnet run
If you see message, it means that you can run the source:

![enter image description here](https://2.pik.vn/202222df2c72-6212-4da9-8289-28846ce8d66c.png)

Open your browser and type urls:

    https://localhost:5001/swagger
    http://localhost:5000/swagger

> If you see security warning, please proceed

