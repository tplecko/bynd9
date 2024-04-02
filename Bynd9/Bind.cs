using System.Text.RegularExpressions;

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
                    File.AppendAllText($"server.log", $"{C.TS} => Record update: Regex match success\n");
                }
                else
                {
                    File.AppendAllText($"server.log", $"{C.TS} => Record update: Regex match failed. Was using pattern: {pattern}\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText($"server.log", $"{C.TS} => Record update failed: {ex}\n");
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

                string pattern = @"\d{10}"; // Assumes a serial number in YYYYMMDDnn format
                Match match = Regex.Match(zoneFileContents, pattern);

                if (match.Success)
                {
                    string newSerialNumber = $"{DateTime.UtcNow:yyyyMMdd}{C.DayProgress}".PadLeft(10, '0');
                    string newZoneFileContents = Regex.Replace(zoneFileContents, pattern, newSerialNumber);

                    File.AppendAllText($"server.log", $"{C.TS} => Serial increase: Day progress: {C.DayProgress}. New serial: {newSerialNumber}\n");

                    File.WriteAllText(filePath, newZoneFileContents);

                    returnValue = true;
                    File.AppendAllText($"server.log", $"{C.TS} => Serial increase: Regex match success\n");
                }
                else
                {
                    File.AppendAllText($"server.log", $"{C.TS} => Serial increase: Regex match failed. Was using pattern: {pattern}\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText($"server.log", $"{C.TS} => Serial increase failed: {ex}\n");
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
