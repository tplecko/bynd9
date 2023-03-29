using System.Text.RegularExpressions;
using System.ServiceProcess;

namespace Bynd9
{
    internal class Bind
    {
        internal static bool UpdateARecord(string filePath, string recordName, string newIpAddress)
        {
            bool returnValue = false;
            try
            {
                string zoneFileContents = File.ReadAllText(filePath);

                string pattern = $@"^{recordName}\s+[0-9]{{0,32}}\s+IN\s+A\s+(\d+\.\d+\.\d+\.\d+)";
                Match match = Regex.Match(zoneFileContents, pattern, RegexOptions.Multiline);

                if (match.Success)
                {
                    string oldIpAddress = match.Groups[1].Value;
                    string newLine = match.Value.Replace(oldIpAddress, newIpAddress);

                    zoneFileContents = zoneFileContents.Replace(match.Value, newLine);
                    File.WriteAllText(filePath, zoneFileContents);

                    returnValue = true;
                    File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Record update: Regex match success\n");
                }
                else
                {
                    File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Record update: Regex match failed\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Record update failed: {ex}\n");
                return returnValue;
            }
            return returnValue;
        }

        internal static bool IncreaseSerial(string filePath)
        {
            bool returnValue = false;
            try
            {
                string zoneFileContents = File.ReadAllText(filePath);

                string serialNumberPattern = @"\d{10}"; // Assumes a serial number in YYYYMMDDnn format
                Match match = Regex.Match(zoneFileContents, serialNumberPattern);

                if (match.Success)
                {
                    int currentSerialNumber = int.Parse(match.Value);

                    int newSerialNumber = currentSerialNumber + 1;
                    string newZoneFileContents = Regex.Replace(zoneFileContents, serialNumberPattern, newSerialNumber.ToString().PadLeft(10, '0'));

                    File.WriteAllText(filePath, newZoneFileContents);

                    returnValue = true;
                    File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Serial increase: Regex match success\n");
                }
                else
                {
                    File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Serial increase: Regex match failed\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText($"server.log", $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff} => Serial increase failed: {ex}\n");
                return returnValue;
            }
            return returnValue;
        }

        internal static void Restart()
        {
            SystemdService service = new(C.conf.Bind9ServiceName);

            if (service.Status == C.conf.ActiveString)
            {
                service.Stop();
                service.WaitForStatus(C.conf.InactiveString);
            }

            service.Start();
            service.WaitForStatus(C.conf.ActiveString);
        }

    }
}
