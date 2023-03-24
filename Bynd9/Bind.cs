using System.Text.RegularExpressions;
using System.ServiceProcess;

namespace Bynd9
{
    internal class Bind
    {
        internal static bool UpdateARecord(string filePath, string recordName, string newIpAddress)
        {
            bool returnValue = false;

            string zoneFileContents = File.ReadAllText(filePath);

            string pattern = $@"^{recordName}\s+IN\s+A\s+(\d+\.\d+\.\d+\.\d+)";
            Match match = Regex.Match(zoneFileContents, pattern, RegexOptions.Multiline);

            if (match.Success)
            {
                string oldIpAddress = match.Groups[1].Value;
                string newLine = match.Value.Replace(oldIpAddress, newIpAddress);

                zoneFileContents = zoneFileContents.Replace(match.Value, newLine);
                File.WriteAllText(filePath, zoneFileContents);

                returnValue = true;
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
