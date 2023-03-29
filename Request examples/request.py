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