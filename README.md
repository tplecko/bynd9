# bynd9
Allows Bind9 to be used as a dynamic DNS server as an alternative to popular paid services.

Fully configurable http endpoint. Including key field name, path and port. You can change 'key' header field, listener port and path for each instance.

# Config files
Configuration is located in several .json files:
- config.json - Holds the service configuration
- devices.json - Holds the allowed device list. There is no limit on device name length.
- hosts.json - Holds the API keys with associated hostnames. There is no limit on key length

## config.json - Server
```
{
    "Interval": 60,
    "HttpPort": 23432,
    "ServerIP": "",
    "certificateFilePath": "server.crt",
    "certificatePassword": "password",
    "HttpsPort": 23433,
    "KeyFieldName": "API-key",
    "Path": "/updater/post",
    "ZoneFilePath": "C:\\temp\\test.zone",
    "Bind9ServiceName": "bind9",
    "FieldIndex": 2,
    "ActiveString": "(running)",
    "InactiveString": "(dead)",
    "FQDNSuffix": "example.com",
    "Discord": "",
    "DiscordIconURL": "internal",
    "DiscordAvatarURL": "internal",
    "TelegramUser": "",
    "WhatsappNumber": "",
    "WhatsappKey": ""
}
```
ServerIP and HttpPort are used when DiscordIconURL or DiscordAvatarURL are set to "internal"

## devices.json
```
[
    "device-name-1",
    "device-name-2"    
]
```

## hosts.json
```
{
    "api-key-1": "domain-name-prefix-1",
    "api-key-2": "domain-name-prefix-2"
}
```

## config.json - Client
```
{
  "Server": "127.0.0.1",
  "Port": 23432,
  "Path": "/updater/post",
  "KeyFieldName": "API-key",
  "KeyFieldValue": "asdfasdfASDFASDF",
  "DeviceID": "Gateway",
  "Discord": "",
  "DiscordIconURL": "internal",
  "DiscordAvatarURL": "internal",
  "TelegramUser": "",
  "WhatsappNumber": "",
  "WhatsappKey": "",
  "Interval": 30
}
```
Server and Port are used when DiscordIconURL or DiscordAvatarURL are set to "internal"

# To-do
- https (started)
- more notifications (telegram, discord, zulip and such)
- more clients (python, mikrotik and such)

Server responds with plain text codes:
- 1 = Success
- 0 = No update needed
- -1 = Error updating zone file

# How-to
Edit the config.json file to your liking

Place the .service file in /etc/systemd/system/
After you have copied the .service file, execute the following:
```
systemctl daemon-reload
systemctl enable bynd9
```

# Zone file format
Record must be in the following format:
- `host          IN    A    1.2.3.4`
- `host    30    IN    A    1.2.3.4`

If the record is in any other format (has additional fields), regex matching must be modified

# Supported notifications
- Discord - Webhook 
- Telegram - CallMeBot direct message
