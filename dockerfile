FROM mcr.microsoft.com/dotnet/nightly/sdk as build

WORKDIR /build

COPY ["./Directory.Packages.props" , "./"]
COPY ["./GuardianStock.slnx" , "./"]
COPY ["./src/GuardianStock.API/GuardianStock.API.csproj","./src/GuardianStock.API/GuardianStock.API.csproj"]
COPY ["./src/GuardianStock.Application/GuardianStock.Application.csproj","./src/GuardianStock.Application/GuardianStock.Application.csproj"]
COPY ["./src/GuardianStock.Domain/GuardianStock.Domain.csproj","./src/GuardianStock.Domain/GuardianStock.Domain.csproj"]
COPY ["./src/GuardianStock.Infrastructure/GuardianStock.Infrastructure.csproj","./src/GuardianStock.Infrastructure/GuardianStock.Infrastructure.csproj"]
COPY ["./tests/Domain.UnitTests/Domain.UnitTests.csproj","./tests/Domain.UnitTests/Domain.UnitTests.csproj"]


RUN dotnet restore "GuardianStock.slnx"

COPY . .

RUN dotnet publish --no-restore "./src/GuardianStock.API/GuardianStock.API.csproj" -c Release -o ../release


FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0 as final

WORKDIR /app

COPY --from=build ../release . 

ENV ASPNETCORE_URLS=http://+:5020
EXPOSE 5020

ENTRYPOINT [ "dotnet","GuardianStock.API.dll" ]
