# gntest

Gracenote Web API test program for c#

## configuration

You should add both your Gracenote Client ID and Client tag into gnsetting.xml.
Below is a sample of gnsetting.xml.

```
<?xml version="1.0" encoding="utf-8"?>
<Settings xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <clientId>XXXXXXXXXX</clientId>
  <clientTag>YYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY</clientTag>
</Settings>
```

## compile
```
mcs gntest.cs
```

## run
```
mono gntest.exe
```
