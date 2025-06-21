#if ANDROID

using Android.App;

#endif

namespace WuDingCard;

//本质上就是创建一个特定的二维数组，然后在这个二维数组上连成一条就消元
//我应该把游戏做成纯数字显示的来指向这个抽象本质
public partial class RussionBlock:ContentPage
{
	private GameState GameStateNow { get; set; } = GameState.NotStarted;

	//定义游戏的空间
	private static int[,] GameGridNow { get; set; } = new int[20,10];

	private static int MaxRow { get; set; } = GameGridNow.GetLength(0);
	private static int MaxCol { get; set; } = GameGridNow.GetLength(1);

	//定义方块
	private readonly int[][,] Blocks = new int[7][,] {
		//I形方块
		new int[,] {
		{0,0,0,0},
		{1,1,1,1},
		{0,0,0,0},
		{0,0,0,0},
		},
		//J形方块
		new int[,] {
		{2,0,0,0},
		{2,2,2,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//L形方块
		new int[,] {
		{0,0,3,0},
		{3,3,3,0},
		{0,0,0,0},
		{0,0,0,0}
		},
		//O形方块
		new int[,] {
		{0,4,4,0},
		{0,4,4,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//S形方块
		new int[,] {
		{0,5,5,0},
		{5,5,0,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//T形方块
		new int[,] {
		{0,6,0,0},
		{6,6,6,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//Z形方块
		new int[,] {
		{7,7,0,0},
		{0,7,7,0},
		{0,0,0,0},
		{0,0,0,0},
		},
	};

	//方块颜色
	private readonly Color[] colors = [
	   Colors.Transparent, //
       Colors.Cyan,        //
       Colors.Blue,        //
       Colors.Orange,      //
       Colors.Yellow,      //
       Colors.Green,       //
       Colors.Purple,      //
       Colors.Red		   //
	];

	private TimeSpan _interval = TimeSpan.FromSeconds(0.5);

	//刷新间隔
	private TimeSpan Interval {
		get => _interval;
		set {
			_interval = value;
			if(gameTimer != null) {
				gameTimer.Interval = value;
			}
		}
	}

	//计时器
	private IDispatcherTimer gameTimer;

	private int Score = 0;
	private int Level = 1;

	//当前方块
	private static int CurrentBlockIndex { get; set; }

	private static int CurrentRotation { get; set; }
	private static int CurrentRow { get; set; }
	private static int CurrentCal { get; set; }

	public RussionBlock() {
		InitializeComponent();
		InitializeGameGrid();
	}

	//初始化游戏区
	private void InitializeGameGrid() {
		for(int i = 0;i < MaxRow;i++) {
			GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1,GridUnitType.Star) });
		}
		for(int i = 0;i < MaxCol;i++) {
			GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) });
		}
		for(int row = 0;row < MaxRow;row++) {
			for(int col = 0;col < MaxCol;col++) {
				var cell = new BoxView { Color = Colors.Transparent };
				GameGrid.Add(cell,col,row);
			}
		}
	}

	#region 开始，暂停，结束事件

	private void OnStartClicked(object sender,EventArgs e) {
		StartGame();
	}

	private void OnPauseClicked(object sender,EventArgs e) {
		PauseGame();
	}

	private void OnResetClicked(object sender,EventArgs e) {
		ResetGame();
	}

	#endregion 开始，暂停，结束事件

	#region 手势处理事件

	private void OnSwipeLeft(object sender,SwipedEventArgs e) {
		MoveLeft();
	}

	private void OnSwipeRight(object sender,SwipedEventArgs e) {
		MoveRight();
	}

	private void OnSwipeDown(object sender,SwipedEventArgs e) {
		MoveDown();
	}

	private void OnTap(object sender,TappedEventArgs e) {
		Rotate();
	}

	#endregion 手势处理事件

	#region 开始，暂停，结束

	private void StartGame() {
		GameStateNow = GameState.Playing;
		SpawnNewBlock();
		//创建计时器
		gameTimer = Dispatcher.CreateTimer();
		Interval = TimeSpan.FromSeconds(0.5);
		gameTimer.Tick += (s,e) => {
			if(GameStateNow == GameState.Playing) {
				MoveDown();
			}
			else {
				gameTimer.Stop();
			}
		};
		gameTimer.Start();
	}

	private void PauseGame() {
		GameStateNow = GameState.Paused;
	}

	private void ResetGame() {
		GameStateNow = GameState.NotStarted;
		Score = 0;
		Level = 1;
		ScoreLabel.Text = $"{Score}";
		LevelLabel.Text = $"{Level}";

		for(int row = 0;row < MaxRow;row++) {
			for(int col = 0;col < MaxCol;col++) {
				GameGridNow[row,col] = 0;
			}
		}
		UpdateUI();
		gameTimer?.Stop();
	}

	#endregion 开始，暂停，结束

	//生成新的方块
	private void SpawnNewBlock() {
		Random random = new Random();
		CurrentBlockIndex = random.Next(0,Blocks.Length);
		CurrentRotation = 0;
		CurrentRow = 0;
		CurrentCal = MaxCol / 2 - Blocks[CurrentBlockIndex].GetLength(1) / 2;
		//如果到顶了就结束游戏
		if(CheckCollection()) {
			GameStateNow = GameState.GameOver;
			gameTimer?.Stop();
			DisplayAlert("游戏结束",$"您的得分是{Score}","确定");
		}
		UpdateUI();
	}

	private void UpdateUI() {
	}

	//碰撞检测
	private bool CheckCollection() {
		return false;
	}

	//消除检测
	private void CheckLines() {
	}

	#region 方块控制

	private void MoveLeft() {
	}

	private void MoveRight() {
	}

	private void MoveDown() {
	}

	private void Rotate() {
	}

	#endregion 方块控制

	private enum GameState
	{
		NotStarted,
		Paused,
		Playing,
		GameOver
	}
}