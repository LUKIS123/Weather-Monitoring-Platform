using Dapper;
using System.Data;

namespace WeatherMonitor.Server.Infrastructure.Utility;
internal class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToDateTime(new TimeOnly(0, 0));
        parameter.DbType = DbType.Date;
    }

    public override DateOnly Parse(object value)
    {
        return DateOnly.FromDateTime((DateTime)value);
    }
}
