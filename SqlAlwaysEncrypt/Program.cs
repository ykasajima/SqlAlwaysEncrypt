using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Logging;

namespace SqlAlwaysEncrypt
{
    public class Program
    {
        public static void Main(string[] args)
        {


            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((content, builder) =>
                {
                    var configuration = builder.Build();
                    var keyVaultEndpoint = configuration["KeyVaultEndpoint"];

                    if (!string.IsNullOrEmpty(keyVaultEndpoint))
                    {
                        // Azure Key Vaultに接続するための追加の設定を記述する ・・・③
                        // ③-1 アプリをAzureに認証するためのアクセストークン取得プロバイダーのインスタンス化
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();

                        // ③-2 Azure Key Vaultに接続するためのクライアントのインスタンス化
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        // ③-3 Azure Key Vault（キーコンテナー）からシークレットにアクセスするための設定をbuilderに追加
                        builder.AddAzureKeyVault(
                            keyVaultEndpoint,
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager());

                        // register Column Encryption KeyStore Provider
                        var sqlColumnEncryptionAzureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));

                        var providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>
                        {
                            { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, sqlColumnEncryptionAzureKeyVaultProvider },
                        };
                        SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
                    }
                })
                .UseStartup<Startup>();
        }
    }
}
