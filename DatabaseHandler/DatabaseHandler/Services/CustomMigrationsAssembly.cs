using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace DatabaseHandler.Services;

/// <summary>
///
/// </summary>
public class CustomMigrationsAssembly : IMigrationsAssembly
{
    private readonly IMigrationsIdGenerator _idGenerator;
    private readonly IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;
    private IReadOnlyDictionary<string, TypeInfo>? _migrations;
    private ModelSnapshot? _modelSnapshot;
    private readonly Type _contextType;
    private readonly IDbContextOptions _options;

    /// <summary>
    /// custom migration assembly
    /// </summary>
    /// <param name="currentContext"></param>
    /// <param name="options"></param>
    /// <param name="idGenerator"></param>
    /// <param name="logger"></param>
    public CustomMigrationsAssembly(
        ICurrentDbContext currentContext,
        IDbContextOptions options,
        IMigrationsIdGenerator idGenerator,
        IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
    {
        _contextType = currentContext.Context.GetType();
        _options = options;
        var assemblyName = RelationalOptionsExtension.Extract(options).MigrationsAssembly;
        Assembly = assemblyName == null
            ? _contextType.Assembly
            : Assembly.Load(new AssemblyName(assemblyName));

        _idGenerator = idGenerator;
        _logger = logger;
    }

    /// <summary>
    /// all migrations
    /// </summary>
    public virtual IReadOnlyDictionary<string, TypeInfo> Migrations
    {
        get
        {
            IReadOnlyDictionary<string, TypeInfo> Create()
            {
                var result = new SortedList<string, TypeInfo>();

                var items
                    = from t in Assembly.GetConstructibleTypes()
                      where t.IsSubclassOf(typeof(Migration))
                          && t.GetCustomAttribute<DbContextAttribute>()?.ContextType == _contextType
                          && (t.GetCustomAttribute<ExtensionAttribute>() == null || _options.Extensions.Select(x => x.GetType().Name).Contains(t.GetCustomAttribute<ExtensionAttribute>()?.ExtensionName))
                      let id = t.GetCustomAttribute<MigrationAttribute>()?.Id
                      orderby id
                      select (id, t);

                foreach ((string id, TypeInfo t) in items)
                {
                    if (id == null)
                    {
                        _logger.MigrationAttributeMissingWarning(t);

                        continue;
                    }

                    result.Add(id, t);
                }

                return result;
            }

            return _migrations ??= Create();
        }
    }

    /// <summary>
    /// Model Snapshot
    /// </summary>
    public virtual ModelSnapshot? ModelSnapshot
        => _modelSnapshot
            ??= (from t in Assembly.GetConstructibleTypes()
                 where t.IsSubclassOf(typeof(ModelSnapshot))
                     && t.GetCustomAttribute<DbContextAttribute>()?.ContextType == _contextType
                          && (t.GetCustomAttribute<ExtensionAttribute>() == null || _options.Extensions.Select(x => x.GetType().Name).Contains(t.GetCustomAttribute<ExtensionAttribute>()?.ExtensionName))
                 select (ModelSnapshot)Activator.CreateInstance(t.AsType())!)
            .FirstOrDefault();

    public virtual Assembly Assembly { get; }

    /// <summary>
    /// Find Migration Id
    /// </summary>
    /// <param name="nameOrId"></param>
    /// <returns></returns>
    public virtual string? FindMigrationId(string nameOrId)
        => Migrations.Keys
            .Where(
                _idGenerator.IsValidId(nameOrId)
                    // ReSharper disable once ImplicitlyCapturedClosure
                    ? id => string.Equals(id, nameOrId, StringComparison.OrdinalIgnoreCase)
                    : id => string.Equals(_idGenerator.GetName(id), nameOrId, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();

    /// <summary>
    /// create new migrration
    /// </summary>
    /// <param name="migrationClass"></param>
    /// <param name="activeProvider"></param>
    /// <returns></returns>
    public virtual Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
    {
        var migration = (Migration)Activator.CreateInstance(migrationClass.AsType())!;
        migration.ActiveProvider = activeProvider;

        return migration;
    }
}