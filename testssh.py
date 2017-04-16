#pip install paramiko
import paramiko, base64
import re

host = "host name or ip address"
username = "username"
password = "password"

class SshSession:
    def __init__(self, Machine, UserName="", Password="", PrivateKey=""):
        self.Machine = Machine
        self.UserName = UserName
        self.Password = Password
        self.PrivateKey = PrivateKey
        self.client = None

    def Connect(self):
        self.client = paramiko.SSHClient()
        self.client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        self.client.load_system_host_keys()
        self.client.connect(self.Machine, username=self.UserName, password=self.Password)

    def Execute(self, command: str = ""):
        if self.client == None:
            self.Connect()
        stdin, stdout, stderr = self.client.exec_command(command)
        result = []
        for line in stdout:
            result.append(line.strip('\n'))
        return result

    def __del__(self):
        if self.client:
            self.client.close()
            self.client = None

class InstalledSoftwareScanner:
    Fields = ["Name", "Vesion", "Architecture", "Description"]
    def __init__(self, Machine, UserName="", Password="", PrivateKey=""):
        self.sshSession = SshSession(Machine, UserName, Password, PrivateKey)
    def GetInstalledSoftware(self):
        result = self.sshSession.Execute("dpkg -l")
        items = []
        markers = None
        for line in result:
            if line.startswith("+++"):
                markers = [m.start() for m in re.finditer("-", line)]
                print(markers)
            elif markers:
                item = \
                {
                    "Name": (line[markers[0]:markers[1]]).strip(),
                    "Version": (line[markers[1]:markers[2]]).strip(),
                    "Architecture": (line[markers[2]:markers[3]]).strip(),
                    "Description": (line[markers[3]:]).strip(),
                }
                items.append(item)

        return items

s = InstalledSoftwareScanner(host, username, password)

result = s.GetInstalledSoftware()

for item in result:
    print(json.dumps(item))