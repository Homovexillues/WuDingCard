namespace WuDingCard;
//这里是根据词法分析器和解析器的代码片段，构建一个简单的数学表达式解析器

/// <summary>
/// 词法单元类型
/// </summary>
public enum TokenType
{
	Number, // 数字
	Identifier, // 标识符
	Operator, // 运算符
	LeftParenthesis, // 左括号
	RightParenthesis, // 右括号
	Comma, // 逗号
}

/// <summary>
/// 词法单元结构
/// </summary>
public class Token
{
	public TokenType Type { get; set; } // 词法单元类型
	public string Value { get; set; } // 词法单元值

	public override string ToString() => $"{Type}: {Value}";
}

/// <summary>
/// 简单的词法分析器
/// </summary>
public class SimpleLexer(string input)
{
	private readonly string _input = input; // 输入字符串
	private int _pos = 0; // 当前解析位置

	public List<Token> Tokenize() {
		var tokens = new List<Token>(); // 存储词法单元的列表
		while(_pos < _input.Length) {
			char current = _input[_pos];
			if(char.IsWhiteSpace(current)) {
				_pos++; // 跳过空白字符
				continue;
			}
			else if(char.IsDigit(current)) {
				tokens.Add(ReadNumber()); // 读取数字
				continue;
			}
			else if(char.IsLetter(current)) {
				tokens.Add(ReadIdentifier()); // 读取标识符
				continue;
			}
			switch(current) {
				case '+':
				case '-':
				case '*':
				case '^':
				case '/':
					tokens.Add(new Token { Type = TokenType.Operator,Value = current.ToString() });
					_pos++;
					break;

				case '(':
					tokens.Add(new Token { Type = TokenType.LeftParenthesis,Value = current.ToString() });
					_pos++;
					break;

				case ')':
					tokens.Add(new Token { Type = TokenType.RightParenthesis,Value = current.ToString() });
					_pos++;
					break;

				case ',':
					tokens.Add(new Token { Type = TokenType.Comma,Value = current.ToString() });
					_pos++;
					break;

				default:
					throw new Exception($"未知字符: {current}");
			}
		}
		return tokens; // 返回词法单元列表
	}

	private Token ReadNumber() {
		int start = _pos; // 记录数字开始位置
		while(_pos < _input.Length && (char.IsDigit(_input[_pos]) || _input[_pos] == '.')) {
			_pos++; // 继续读取数字
		}
		string number = _input.Substring(start,_pos - start); // 提取数字字符串
		return new Token { Type = TokenType.Number,Value = number }; // 返回数字词法单元
	}

	private Token ReadIdentifier() {
		int start = _pos; // 记录标识符开始位置
		while(_pos < _input.Length && (char.IsLetter(_input[_pos]))) {
			_pos++; // 继续读取标识符
		}
		string identifier = _input.Substring(start,_pos - start); // 提取标识符字符串
		return new Token { Type = TokenType.Identifier,Value = identifier }; // 返回标识符词法单元
	}
}

/// <summary>
/// AST节点基类
/// </summary>
public interface IAstNode
{
	public double Evaluate(); // 抽象方法，用于计算节点的值
}

/// <summary>
/// 数字节点
/// </summary>
public class NumberNode(double value):IAstNode
{
	public double Value { get; set; } = value; // 数字节点的值

	public double Evaluate() => Math.Round(Value,6); // 计算数字节点的值
}

/// <summary>
/// 二元运算符节点
/// </summary>
public class BinaryOperatorNode(string op,IAstNode left,IAstNode right):IAstNode
{
	public string Operator = op; // 运算符
	public IAstNode Left = left, Right = right;

	public double Evaluate() {
		double l = Left.Evaluate(); // 计算左子节点的值
		double r = Right.Evaluate(); // 计算右子节点的值
		return Operator switch {
			"+" => l + r, // 加法
			"-" => l - r, // 减法
			"*" => l * r, // 乘法
			"/" => l / r, // 除法
			"%" => l % r, // 取模
			"^" => Math.Pow(l,r), // 幂运算
			_ => throw new Exception($"未知运算符: {Operator}") // 抛出异常
		};
	}
}

/// <summary>
/// 函数调用节点
/// </summary>
public class FunctionCallNode(string name,List<IAstNode> args):IAstNode
{
	public string FunctionName = name;// 函数名
	public List<IAstNode> Arguments = args; // 函数参数列表

	public double Evaluate() {
		// 根据函数名调用不同的函数
		double result = FunctionName.ToLower() switch {
			"log" => Arguments.Count == 2
			? Math.Log(Arguments[1].Evaluate(),Arguments[0].Evaluate())
			: Math.Log(Arguments[0].Evaluate()), // 对数函数
			"sin" => Math.Sin(Arguments[0].Evaluate()), // 正弦函数
			"cos" => Math.Cos(Arguments[0].Evaluate()), // 余弦函数
			"tan" => Math.Tan(Arguments[0].Evaluate()), // 正切函数
			_ => throw new Exception($"未知函数: {FunctionName}") // 抛出异常
		};
		return Math.Round(result,6); // 返回计算结果，保留6位小数
	}
}

/// <summary>
/// 解析器类，用于解析词法单元并构建AST
/// </summary>
public class Parser(List<Token> tokens)
{
	private readonly List<Token> _tokens = tokens; // 词法单元列表
	private int _pos = 0; // 当前解析位置
	private Token Current => _pos < _tokens.Count ? _tokens[_pos] : null; // 当前词法单元

	private Token Eat() => _tokens[_pos++]; // 获取当前词法单元并前进

	private static readonly Dictionary<string,int> Precedence = new() {
			{ "+",1 }, // 加法
			{ "-",1 }, // 减法
			{ "*",2 }, // 乘法
			{ "/",2 }, // 除法
			{ "^",3 } // 幂运算
		};

	private IAstNode ParseBinary(int minPrec) {
		IAstNode left = ParsePrimary(); // 解析初始的左操作数
		while(Current != null && Current.Type == TokenType.Operator && Precedence.TryGetValue(Current.Value,out int prec) && prec >= minPrec) {
			string op = Eat().Value; // 获取当前运算符
			int nextMinPrec = op == "^" ? prec + 1 : prec; // 确定下一个最小优先级
			IAstNode right = ParseBinary(nextMinPrec); // 递归解析右操作数
			left = new BinaryOperatorNode(op,left,right); // 创建二元运算符节点
		}
		return left; // 返回解析后的节点
	}

	private IAstNode ParsePrimary() {
		if(Current == null) throw new Exception("意外的结束，缺少表达式");
		if(Current.Type == TokenType.Number) {
			double value = double.Parse(Eat().Value); // 解析数字
			return new NumberNode(value); // 返回数字节点
		}
		else if(Current.Type == TokenType.Identifier) {
			string identifier = Eat().Value; // 解析标识符
			if(Current != null && Current.Type == TokenType.LeftParenthesis) {
				Eat(); // 吃掉左括号
				List<IAstNode> args = new();
				if(Current != null && Current.Type != TokenType.RightParenthesis) {
					args.Add(ParseBinary(0)); // 解析函数参数
					while(Current != null && Current.Type == TokenType.Comma) {
						Eat(); // 吃掉逗号
						args.Add(ParseBinary(0)); // 继续解析下一个参数
					}
				}
				if(Current == null || Current.Type != TokenType.RightParenthesis) throw new Exception("缺少右括号");
				Eat(); // 吃掉右括号
				return new FunctionCallNode(identifier,args); // 返回函数调用节点
			}
			else {
				throw new Exception($"未知标识符: {identifier}"); // 抛出异常
			}
		}
		else if(Current.Type == TokenType.LeftParenthesis) {
			Eat(); // 吃掉左括号
			IAstNode expr = ParseBinary(0); // 解析括号内的表达式
			if(Current == null || Current.Type != TokenType.RightParenthesis) throw new Exception("缺少右括号");
			Eat(); // 吃掉右括号
			return expr; // 返回解析后的表达式节点
		}
		else {
			throw new Exception($"未知词法单元: {Current}"); // 抛出异常
		}
	}

	public IAstNode ParseExpression() {
		return ParseBinary(0);
	}
}