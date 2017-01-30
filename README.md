# Why 
There are too many utilities to convert XML schema to entity class, but few tool to convert DTD. This is why I wrote this tool.

# Enviorment
.NET Core on Windows/Linux/OSX

# How to use
Simply execute command:
```shell
cd src/app/
dotnet restore
dotnet build
dotnet run app -i ~/dtd-to-entity/src/app/sample.dtd -o ~/dtd-to-entity/src/app/output
```