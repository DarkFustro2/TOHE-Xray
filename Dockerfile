FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-build
WORKDIR /app

# TOHE'nin ihtiyaç duyduğu bağımlılıkları ve repoyu çekiyoruz
RUN apt-get update && apt-get install -y git
RUN git clone https://github.com/Town-Of-Host-Official/TownOfHost-Enhanced.git .

# Xray.cs dosyamızı doğru klasörün içine koyuyoruz
COPY Xray.cs ./TOHE/Roles/Crewmate/Xray.cs

# Projeyi derliyoruz
RUN dotnet build -c Release -o ./output

FROM python:3.10-slim
WORKDIR /app
COPY --from=dotnet-build /app /app
RUN pip install flask
EXPOSE 5000
CMD ["python", "app.py"]
