FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-build
WORKDIR /app
COPY . .
RUN dotnet new classlib -n XrayRole -o .
RUN dotnet build -c Release -o ./output

FROM python:3.10-slim
WORKDIR /app
COPY --from=dotnet-build /app /app
RUN pip install flask
EXPOSE 5000
CMD ["python", "app.py"]
