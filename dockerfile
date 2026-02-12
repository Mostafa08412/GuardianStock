FROM mcr.microsoft.com/dotnet/nightly/sdk as build

WORKDIR /build

COPY ["./Directory.Packages.props" , "./"]
COPY ["./IMS.slnx" , "./"]
COPY ["./src/IMS.API/IMS.API.csproj","./src/IMS.API/IMS.API.csproj"]
COPY ["./src/IMS.Application/IMS.Application.csproj","./src/IMS.Application/IMS.Application.csproj"]
COPY ["./src/IMS.Domain/IMS.Domain.csproj","./src/IMS.Domain/IMS.Domain.csproj"]
COPY ["./src/IMS.Infrastructure/IMS.Infrastructure.csproj","./src/IMS.Infrastructure/IMS.Infrastructure.csproj"]
COPY ["./tests/Domain.UnitTests/Domain.UnitTests.csproj","./tests/Domain.UnitTests/Domain.UnitTests.csproj"]


RUN dotnet restore "IMS.slnx"

COPY . .

RUN dotnet publish --no-restore "./src/IMS.API/IMS.API.csproj" -c Release -o ../release


FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0 as final

WORKDIR /app

COPY --from=build ../release . 

ENV ASPNETCORE_URLS=http://+:5020
EXPOSE 5020

ENTRYPOINT [ "dotnet","IMS.API.dll" ]
