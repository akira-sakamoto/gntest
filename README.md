# gntest

Gracenote Web API test program wrriten by c#

## configuration

You should add both your Gracenote Client ID and Client tag into gnsetting.xml.
Below is a sample of gnsetting.xml.

```xml
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

## to make doc
```
mcs -doc:gntest.xml gntest.cs
```

# How to use
1. Run gntest
```
mono gntest.exe
```
Gntest shows as following;
```
ApiUrl: https://cXXXXXXXXXX.web.cddbp.net/webapi/xml/1.0/
UserID: 
Lang:   
cmd: 1: Regist, 2: Enter userId, 3: Enter Language, 4: Query, 5: Fetch, 0: Exit
> 
```

2. Register your client ID

  Select command "1".  Gntest posts your client ID to the server, then receive the result.
  If some errors occure, you should check gnsetting.xml.

3. Enter result language, if you want

  Select command "3".  Enter 3 character language code: 'eng', 'jpn', 'twn', etc.
  The default is 'eng'.

4. Do Track Search

  Select command "4", and answer questions.  Gntest queries by Track Search method, then show the result as CSV format.

5. Do Album Fetch

  Select command "5", and enter GNID which was displayed at the result of Track Search.

# License
MIT
