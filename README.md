# bynd9
Allows Bind9 to be used as a dynamic DNS server as an alternative to popular paid services.

Fully configurable http endpoint. Including key field name, path and port. You can change 'key' header field, listener port and path for each instance.

Configuration is located in several .json files:
- config.json - Holds the service configuration
``` json
{
  "HttpPort": 23432,
  "certificateFilePath": "server.crt",
  "certificatePassword": "password",
  "HttpsPort": 23433,
  "KeyFieldName": "API-key",
  "Path": "/updater/post",
  "ZoneFilePath": "/etc/bind/test.zone",
  "Bind9ServiceName": "bind9",
  "FieldIndex": 2,
  "ActiveString": "(running)",
  "InactiveString": "(dead)"
}
```

- devices.json - Holds the allowed device list
``` json
[
  "Device1",
  "Device2"
]
```
No limit on device name length.

- hosts.json - Holds the API keys with associated hostnames
``` json
{
  "AAAAAAAAAAAAAAA": "host1",
  "BBBBBBBBBBBBBBB": "host2"
}
```
No limit on key length

# To-do
- https (started)
- debug mode (more logging)
- notifications (telegram, discord, zulip and such)
- more clients (python and such)

Example request:
C#
``` c#
var url = "http://192.168.2.142:23432/updater/post";

var httpRequest = (HttpWebRequest)WebRequest.Create(url);
httpRequest.Method = "POST";

httpRequest.Headers["API-key"] = "AAAAAAAAAAAAAAA";
httpRequest.ContentType = "application/json";

var data = @"{
  ""IP"": ""10.0.0.25"",
  ""DeviceID"": ""Device1""
}";

using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
{
   streamWriter.Write(data);
}

var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
{
   var result = streamReader.ReadToEnd();
}

Console.WriteLine(httpResponse.StatusCode);
```

Bash
``` bash
#!/bin/bash

curl -X POST http://192.168.2.142:23432/updater/post -H "API-key: AAAAAAAAAAAAAAA" -H "Content-Type: application/json" --data-binary @- <<DATA
{
  "IP": "10.0.0.25",
  "DeviceID": "Device1"
}
DATA
```

JavaScript
``` javascript
const url = 'http://192.168.2.142:23432/updater/post';

const data = `{
  "IP": "10.0.0.25",
  "DeviceID": "Device1"
}`;

const response = await fetch(url, {
    method: 'POST',
    headers: {
        'API-key': 'AAAAAAAAAAAAAAA',
        'Content-Type': 'application/json',
    },
    body: data,
});

const text = await response.text();

console.log(text);
```

Python
```python
import requests
from requests.structures import CaseInsensitiveDict

url = "http://192.168.2.142:23432/updater/post"

headers = CaseInsensitiveDict()
headers["API-key"] = "AAAAAAAAAAAAAAA"
headers["Content-Type"] = "application/json"

data = """
{
  "IP": "10.0.0.25",
  "DeviceID": "Device1"
}
"""

resp = requests.post(url, headers=headers, data=data)

print(resp.status_code)
```

PHP
``` php
<?php

$url = "http://192.168.2.142:23432/updater/post";

$curl = curl_init($url);
curl_setopt($curl, CURLOPT_URL, $url);
curl_setopt($curl, CURLOPT_POST, true);
curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);

$headers = array(
   "API-key: AAAAAAAAAAAAAAA",
   "Content-Type: application/json",
);
curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);

$data = <<<DATA
{
  "IP": "10.0.0.25",
  "DeviceID": "Device1"
}
DATA;

curl_setopt($curl, CURLOPT_POSTFIELDS, $data);

//for debug only!
curl_setopt($curl, CURLOPT_SSL_VERIFYHOST, false);
curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, false);

$resp = curl_exec($curl);
curl_close($curl);
var_dump($resp);

?>
```
Service responds with JSON encoded data:
- 1 = Success
- 0 = No update needed
- -1 = Error updating zone file
