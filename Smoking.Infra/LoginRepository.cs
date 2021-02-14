using System;
using System.IO;
using System.Threading.Tasks;
using Smoking.Domain;

namespace Smoking.Infra
{
    public class FileLoginRepository : LoginRepository
    {
        private string _settingDirectory;
        private const string LOGIN_FILE = "login";
        private string LoginFile { get => Path.Join(_settingDirectory, LOGIN_FILE); }

        public FileLoginRepository(string settingDirectory)
        {
            this._settingDirectory = settingDirectory;
        }

        public async Task<Guid> GetLoggedinUser()
        {
            try
            {
                var content = await File.ReadAllTextAsync(this.LoginFile);
                var maybeID = content.Trim();
                var loginID = Guid.Parse(maybeID);
                return loginID;
            }
            catch (Exception error)
            {
                throw new Exception("Not Found", error);
            }
        }

        public async Task Login(Guid userID)
        {
            await File.WriteAllTextAsync(LoginFile, userID.ToString());
        }

        public async Task Logout()
        {
            await Task.Run(() => File.Delete(LoginFile));
        }
    }
}
