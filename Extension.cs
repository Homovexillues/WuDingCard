using SQLite;

namespace WuDingCard;

internal static class Extension
{
	#region 数据库

	private static string DbPath = "Resources/Database/wanxiangcard.db";
	private static SQLiteAsyncConnection _database = new(DbPath);

	#endregion 数据库
}