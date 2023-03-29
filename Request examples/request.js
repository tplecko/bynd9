onst url = 'http://192.168.2.142:23432/updater/post';

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