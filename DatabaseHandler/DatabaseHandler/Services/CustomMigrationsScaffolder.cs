using Microsoft.EntityFrameworkCore.Migrations.Design;

namespace DatabaseHandler.Services;

public class CustomMigrationsScaffolder : MigrationsScaffolder
{
    public CustomMigrationsScaffolder(MigrationsScaffolderDependencies dependencies) : base(dependencies)
    {
    }

    protected override string GetDirectory(string projectDir, string? siblingFileName, string subnamespace)
    {
        return Path.Combine(projectDir, Path.Combine(subnamespace.Split('.')));
    }
}