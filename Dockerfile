FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-build
WORKDIR /app

# Apt tarihi hatasını bypass edip git kuruyoruz
RUN apt-get -o Acquire::Check-Valid-Until=false -o Acquire::Check-Date=false update && apt-get install -y git

# TOHE reposunu çekiyoruz
RUN git clone https://github.com/Town-Of-Host-Official/TownOfHost-Enhanced.git .

# Xray.cs dosyamızı ekliyoruz
COPY Xray.cs ./TOHE/Roles/Crewmate/Xray.cs

# Derliyoruz
RUN dotnet build -c Release -o ./output

FROM python:3.10-slim
WORKDIR /app
COPY --from=dotnet-build /app /app
RUN pip install flask
EXPOSE 5000
CMD ["python", "app.py"]
