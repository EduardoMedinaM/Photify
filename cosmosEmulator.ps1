$parameters = @(
    "--publish", "8081:8081"
    "--publish", "10250-10255:10250-10255"
    "--memory", "2GB"
    "--interactive"
    "--tty"
)
docker run @parameters mcr.microsoft.com/cosmosdb/windows/azure-cosmos-emulator