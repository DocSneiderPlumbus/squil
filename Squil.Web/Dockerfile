﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 80/tcp
EXPOSE 443/tcp
COPY . App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Squil.Web.dll"]
