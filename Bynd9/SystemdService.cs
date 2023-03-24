using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bynd9
{
    internal class SystemdService
    {
        private readonly string _serviceName;
        private string _status;

        internal SystemdService(string serviceName)
        {
            _serviceName = serviceName;
            _status = GetStatus();
        }

        internal string Status
        {
            get { return _status; }
        }

        internal void Start()
        {
            ExecuteCommand($"systemctl start {_serviceName}");
            _status = GetStatus();
        }

        internal void Stop()
        {
            ExecuteCommand($"systemctl stop {_serviceName}");
            _status = GetStatus();
        }

        internal void Restart()
        {
            ExecuteCommand($"systemctl restart {_serviceName}");
            _status = GetStatus();
        }

        internal void Reload()
        {
            ExecuteCommand($"systemctl reload {_serviceName}");
            _status = GetStatus();
        }

        internal void WaitForStatus(string desiredStatus)
        {
            while (_status != desiredStatus)
            {
                Thread.Sleep(1000);
                _status = GetStatus();
            }
        }

        private string GetStatus()
        {
            string output = ExecuteCommand($"systemctl status {_serviceName} --no-pager");
            string[] lines = output.Split('\n');

            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("Active:"))
                {
                    string[] fields = line.Trim().Split(' ');
                    return fields[C.conf.FieldIndex];
                }
            }

            return "unknown";
        }

        private static string ExecuteCommand(string command)
        {
            Process process = new();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
    }
}
