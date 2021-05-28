FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY src/Functions.Sidecar/bin/Release/netcoreapp3.1/publish /app

ENV ASPNETCORE_URLS=http://+:5000

CMD ["dotnet", "Functions.Sidecar.dll"]
