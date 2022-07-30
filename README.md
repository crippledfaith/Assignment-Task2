# Assignment-Task2
Please Update Seed Data in Task2.Infrastructure.Context.ServerContext class in Seed() and Run Update-Database before running.

Configuration Explaination

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Infrastructure": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    },
    "Console": {
      "FormatterName": "json",
      "FormatterOptions": {
        "SingleLine": true,
        "IncludeScopes": false,
        "TimestampFormat": "HH:mm:ss",
        "UseUtcTimestamp": true,
        "JsonWriterOptions": {
          "Indented": true
        }
      }
    }
  },
  "ServiceSetting": {
    "ServiceInterval": 60,
    "ServiceLocalPath": "D:\\TestDownload", //Local Download Path
    "UseFlatDirectory": true //Use Flat download folder as in Download all files in the root directory, But with hash the FileName so as to avoid having conflict of name
  },
  "ConnectionStrings": {
    "Default": "Server=127.0.0.1;Port=5432;Database=Task2;User Id=postgres;Password=Test@123;" //Postgras Connection String 
  }
}
