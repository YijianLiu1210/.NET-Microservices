# VM: hardware-level virtualization, each app runs its own OS, fully isolated
# hypervisor: VM Monitor, a software that creates and runs VMs
#             allows one host computer to support multiple guest VMs by virtually sharing its resources

# Docker: OS-level virtualization, all apps share the host OS, process-level isolation
# cGroup (control group): allocate resources (CPU, RAM, IOPS, network) to process or container
# namespace: provide isolation for running processes, limit their access to system resources
# UFS (Union File System): a layer cake with each layer baked independently
#                          Linux kernel (base layer) --> OS --> app --> writable layer (top layer)
#                          Each Docker image file is made up of a series of layers, every time a user specifies a command (eg. run, copy), a new layer gets created

# set the baseImage to use for subsequent instructions
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
# define the working directory for the rest of the instructions in the Dockerfile
WORKDIR /app

# copy files to the image's filesystem
COPY *.csproj ./
# execute the command on top of the current image as a new layer and commit the results
RUN dotnet restore

COPY . ./
# compile the application, read through its dependencies specified in the project file, and publish the resulting set of files to a directory
# publish the application and its dependencies to a folder for deployment to a hosting system
# the output includes: IL (Intermediate Language) code in an assembly with a dll extension
#                      a .deps.json file that includes all of the dependencies of the project
#                      a .runtimeconfig.json file
#                      the application's dependencies, which are copied from the NuGet cache itno the output folder
RUN dotnet publish -c Release -o out

# set the baseImage to use for subsequent instructions
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
# configure the container to be run as an executable
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]

# "docker build -t docker_user_id/image_name .": build the image
# "docker push docker_user_id/image_name": push to Docker Hub
# "docker run -p external_port:internal_port -d docker_user_id/image_name": start a new container (the image is stored locally)
# "docker start container_id": restart the existing container