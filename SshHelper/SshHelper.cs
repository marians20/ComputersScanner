using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
using Logger;

namespace SshHelper
{
    public class SshHelper: IDisposable
    {
        #region properties
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SudoPassword { get; set; }
        public string Host { get; set; }
        public string PrivateKeyFileName { get; set; }
        public bool CheckConnected
        {
            get
            {
                if(SshClient == null)
                {
                    SshClient = BuildConnection();
                }
                if(SshClient == null)
                {
                    return false;
                }
                if (SshClient.IsConnected) return SshClient != null && SshClient.IsConnected;
                try
                {
                    SshClient.Connect();
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
                return SshClient != null && SshClient.IsConnected;
            }
        }

        protected SshClient SshClient;
        #endregion

        #region ctor
        #endregion

        #region methods
        protected SshClient BuildConnection()
        {
            try
            {
                if (!string.IsNullOrEmpty(PrivateKeyFileName) && File.Exists(PrivateKeyFileName))
                {
                    return new SshClient(Host, UserName, new PrivateKeyFile(PrivateKeyFileName));
                }
                else
                {
                    return new SshClient(Host, UserName, Password);
                }
            }
            catch(Exception ex)
            {
                Log.Exception(ex);
                return null;
            }

        }

        /// <summary>
        /// Executes a command over ssh
        /// </summary>
        /// <param name="strCommand">The command that should be executed.</param>
        /// <returns></returns>
        public string Exec(string strCommand)
        {
            if (!CheckConnected)
            {
                return null;
            }
            var result = string.Empty;
            var testValue = Guid.NewGuid().ToString();
            var command = SshClient.RunCommand(string.Format(strCommand, testValue));
            result = command.Result;
            return result;
        }

        /// <summary>
        /// Executes a command over ssh as sudo
        /// </summary>
        /// <param name="strCommand">The command that should be executed. It should not start with sudo.</param>
        /// <returns></returns>
        public string ExecSudo(string strCommand)
        {
            ///http://stackoverflow.com/questions/27953227/sudo-command-in-the-c-sharp-ssh-net-library
            ///
            if (!CheckConnected)
            {
                return null;
            }

            var result = new StringBuilder();

            var id = Guid.NewGuid().ToString();
            var strSudoCommand = string.Format("sudo -p {0} {1}", id, strCommand);

            IDictionary<Renci.SshNet.Common.TerminalModes, uint> termkvp =
                new Dictionary<Renci.SshNet.Common.TerminalModes, uint>();
            //I don't want echo active
            termkvp.Add(Renci.SshNet.Common.TerminalModes.Echo, 0);
            var shell = SshClient.CreateShellStream("vt100", 800, 250, 640, 480, 4096, termkvp);

            shell.WriteLine(strCommand);
            var expectSudoPrompt = shell.Expect(id);
            shell.WriteLine(SudoPassword);
            string output;
            do
            {
                output = shell.ReadLine(TimeSpan.FromSeconds(2));
                result.Append(output);
            } while (output != null);

            return result.ToString();
        }

        public void Dispose()
        {
            if (SshClient == null) return;

            if(SshClient.IsConnected)
            {
                SshClient.Disconnect();
            }
            SshClient.Dispose();
        }
        #endregion

    }
}
