# Assignment-Task2
Please Update Seed Data in Task2.Infrastructure.Context.ServerContext class in Seed() and Run Update-Database before running.

Configuration Explaination<br>

{<br>
  "Logging": {<br>
    "LogLevel": {<br>
      "Default": "Information",<br>
      "Microsoft.AspNetCore": "Warning",<br>
      "Microsoft.EntityFrameworkCore.Infrastructure": "Warning",<br>
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"<br>
    },<br>
    "Console": {<br>
      "FormatterName": "json",<br>
      "FormatterOptions": {<br>
        "SingleLine": true,<br>
        "IncludeScopes": false,<br>
        "TimestampFormat": "HH:mm:ss",<br>
        "UseUtcTimestamp": true,<br>
        "JsonWriterOptions": {<br>
          "Indented": true<br>
        }<br>
      }<br>
    }<br>
  },<br>
  "ServiceSetting": {<br>
    "ServiceInterval": 60,<br>
    "ServiceLocalPath": "D:\\TestDownload", //Local Download Path<br>
    "UseFlatDirectory": true //Use Flat download folder as in Download all files in the root directory, But with hash the FileName so as to avoid having conflict of name<br>
  },<br>
  "ConnectionStrings": {<br>
    "Default": "Server=127.0.0.1;Port=5432;Database=Task2;User Id=postgres;Password=Test@123;" //Postgras Connection String <br>
  }<br>
}<br>
