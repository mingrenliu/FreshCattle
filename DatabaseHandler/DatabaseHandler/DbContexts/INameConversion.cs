namespace DatabaseHandler.DbContexts;

internal interface INameConversion
{
    string? TableName(string? tableName);

    string? ColumnName(string? columnName);
}

internal class OracleNameConversion : INameConversion
{
    public string? ColumnName(string? columnName)
    {
        return columnName?.ToUpper();
    }

    public string? TableName(string? tableName)
    {
        return tableName?.ToUpper();
    }
}

internal class DefaultNameConversion : INameConversion
{
    public string? ColumnName(string? columnName)
    {
        return columnName;
    }

    public string? TableName(string? tableName)
    {
        return tableName;
    }
}

internal class NameConversionFactory
{
    public static INameConversion Create(DbType type)
    {
        return type switch
        {
            DbType.MySql => new DefaultNameConversion(),
            DbType.Oracle => new OracleNameConversion(),
            _ => new DefaultNameConversion(),
        };
    }
}

internal enum DbType
{
    Undefined = 0,
    MySql = 1,
    Oracle = 2
}