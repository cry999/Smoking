# Smoking

## Install

```bash
dotnet publish -c Release -r osx-x64 Smoking.Console
ln -s $(pwd)/Smoking.Console/bin/Release/net5.0/osx-x64/Smoking.Console /usr/local/bin/smoke
```

## TODO

- [ ] move smoking.db -> ~/.smoking/smoking.db
