#!/bin/bash

curl -X POST http://192.168.2.142:23432/updater/post -H "API-key: AAAAAAAAAAAAAAA" -H "Content-Type: application/json" --data-binary @- <<DATA
{
  "IP": "10.0.0.25",
  "DeviceID": "Device1"
}
DATA