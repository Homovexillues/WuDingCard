using SQLite;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace WuDingCard;

public static class QuickPlaySpell
{
	#region 数据库

	public static class Database
	{
		public static readonly string DbPath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"WuDingCard","wudingcard.db");

		internal static SQLiteAsyncConnection Dbcnn = new(DbPath);
		private static List<Type> Tables = [typeof(Notebook),typeof(Section),typeof(Note)];

		public static async Task CreateTablesAsync() {
			foreach(var t in Tables) {
				var tableInfo = Dbcnn.GetTableInfoAsync(t.Name).Result;
				if(int.Equals(tableInfo.Count,0)) { await Dbcnn.CreateTableAsync(t); }
			}
		}

		/// <summary>
		/// 将数据库文件从软件中提取出来
		/// </summary>
		/// <returns> </returns>
		public static async Task InitializeAsync() {
			// 确保目录存在
			var dirPath = Path.GetDirectoryName(DbPath);
			if(!Directory.Exists(dirPath)) {
				Directory.CreateDirectory(dirPath);
			}

			// 检查是否已存在数据库文件
#if WINDOWS
			if(!File.Exists(DbPath)) {
#endif
			try {
				// 从应用资源复制预置数据库
				using var inStream = await FileSystem.OpenAppPackageFileAsync("Resources/Database/wudingcard.db");
				using var outStream = File.Create(DbPath);
				await inStream.CopyToAsync(outStream);
			}
			catch(Exception ex) {
				await Log.WarnAsync($"复制预置数据库失败，将创建空库: {ex.Message}");
				// 创建空表
				await CreateTablesAsync();
			}
#if WINDOWS
			}
			else {
				await CreateTablesAsync();
			}
#endif
		}

		//这里为啥要加个new()呢，为了限制T必须没有构造函数的参数，因为FindAsync方法要求传入的只是一个Poco类
		//必须是无构造函数的
		//几个常见的约束:
		//new()T有公共无参构造函数
		//where T : BaseClass 要求T必须继承自BaseClass
		//IInterface必须实现指定接口
		//struct 限制T为值类型
		//class 限制T为引用类型
		//多重约束逗号隔开，new()在最后
		public static T Get<T>(int primaryKey) where T : class, new() {
			try {
				return Dbcnn.FindAsync<T>(primaryKey).Result;
			}
			catch(Exception ex) {
				throw new Exception(ex.Message);
			}
		}
	}

	#endregion 数据库

	/// <summary>
	/// 计算一段文本的MD5值
	/// </summary>
	internal static string CalculateMD5(string input) {
		byte[] inputBytes = Encoding.UTF8.GetBytes(input);
		byte[] hashBytes = MD5.HashData(inputBytes);
		StringBuilder sb = new();
		for(int i = 0;i < hashBytes.Length;i++) {
			sb.Append(hashBytes[i].ToString("x2")); // 转换为十六进制字符串
		}
		return sb.ToString();
	}
}

#region 日志

public static class Log
{
	private static string LogPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"WuDingCard","Log",$"{DateTime.Now:yyyy-MM-dd}.log");

	public static void Warn(this Exception ex,string message) {
		string logText = $"Warn {DateTimeOffset.Now}\n" + message;
		Write(logText);
	}

	public static void Warn(this string message) {
		string logText = $"Warn {DateTimeOffset.Now}\n" + message;
		Write(logText);
	}

	public static async Task WarnAsync(this string message) {
		string logText = $"Warn {DateTimeOffset.Now}\n" + message;
		await WriteAsync(logText);
	}

	public static void Debug(this string message) {
		string logText = $"Debug {DateTimeOffset.Now}\n" + message;
		Write(logText);
	}

	public static void Info(this string message) {
		string logText = $"Info {DateTimeOffset.Now}\n" + message;
		Write(logText);
	}

	public static void Write(string logText) {
		string logFilePath = LogPath();
		string directoryPath = Path.GetDirectoryName(logFilePath);
		if(!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
		if(!File.Exists(logFilePath)) {
			using var writer = new StreamWriter(logFilePath);
			writer.WriteLine(logText);
		}
		else {
			using var writer = File.AppendText(logFilePath);
			writer.WriteLine(logText);
		}
	}

	public static async Task WriteAsync(string logText) {
		string logFilePath = LogPath();
		string directoryPath = Path.GetDirectoryName(logFilePath);
		if(!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
		if(!File.Exists(logFilePath)) {
			using var writer = new StreamWriter(logFilePath);
			await writer.WriteLineAsync(logText);
		}
		else {
			using var writer = File.AppendText(logFilePath);
			await writer.WriteLineAsync(logText);
		}
	}
}

#endregion 日志

public readonly struct Result<T, E> where E : Exception
{
	private readonly T? _value;
	private readonly E? _error;

	public bool IsOk { get; }
	public bool IsError => !IsOk;

	private Result(T value) {
		_value = value;
		_error = default;
		IsOk = true;
	}

	private Result(E error) {
		_value = default;
		_error = error;
		IsOk = false;
	}

	public static Result<T,E> Ok(T value) => new(value);

	public static Result<T,E> Err(E error) => new(error);

	public T Unwrap() {
		if(IsOk) return _value!;
		throw new InvalidOperationException("Cannot unwrap an error result.");
	}

	public E UnwrapErr() {
		if(IsError) return _error!;
		throw new InvalidOperationException("Cannot unwrap an ok result.");
	}

	public T UnwrapOr(T defaultValue) => IsOk ? _value! : defaultValue;

	public override string ToString() => IsOk ? $"Ok({_value})" : $"Err({_error})";
}