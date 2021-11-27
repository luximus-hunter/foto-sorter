# Foto Sorter

_Sorts images based on their timestamps. Categorizes by year and month_

## Download

The latest download can be found [here](https://github.com/luximus-hunter/foto-sorter/releases/latest).

## Building executable

Open a terminal and execute the following code:

```cmd
git clone https://github.com/luximus-hunter/foto-sorter.git FotoSorter
cd ./FotoSorter/FotoSorter
dotnet publish -p:PublishSingleFile=true -r win-x64 -c Release --self-contained true -p:EnableCompressionInSingleFile=true -o ../../
```
