FROM gcr.io/google-appengine/aspnetcore:
COPY . /app
WORKDIR /app
ENTRYPOINT ["dotnet", "FuegoDeQuasar.dll"]
