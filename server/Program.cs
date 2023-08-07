using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Helpers;
using SecureNotes.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
        {
            var storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var keyVaultUrl = Environment.GetEnvironmentVariable("KeyVaultUrl");


            s.AddSingleton(_ => new TableStorageHelper<User>(storageConnectionString!, "users"));
            s.AddSingleton(_ => new TableStorageHelper<Note>(storageConnectionString!, "notes"));

            s.AddSingleton(_ => new SecretHelper(keyVaultUrl!));
            s.AddSingleton(_ => new JwtHelper(
                s.BuildServiceProvider().GetRequiredService<SecretHelper>().GetSecretAsync("jwt-secret").Result
            ));

            s.AddSingleton<UserService>();
            s.AddSingleton<NoteService>();
        }
    )
    .Build();

host.Run();