#if ANDROID

using Android.App;

#endif

namespace WuDingCard;

//�����Ͼ��Ǵ���һ���ض��Ķ�ά���飬Ȼ���������ά����������һ������Ԫ
//��Ӧ�ð���Ϸ���ɴ�������ʾ����ָ�����������
public partial class RussionBlock:ContentPage
{
	private GameState GameStateNow { get; set; } = GameState.NotStarted;

	//������Ϸ�Ŀռ�
	private static int[,] GameGridNow { get; set; } = new int[20,10];

	private static int MaxRow { get; set; } = GameGridNow.GetLength(0);
	private static int MaxCol { get; set; } = GameGridNow.GetLength(1);

	//���巽��
	private readonly int[][,] Blocks = new int[7][,] {
		//I�η���
		new int[,] {
		{0,0,0,0},
		{1,1,1,1},
		{0,0,0,0},
		{0,0,0,0},
		},
		//J�η���
		new int[,] {
		{2,0,0,0},
		{2,2,2,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//L�η���
		new int[,] {
		{0,0,3,0},
		{3,3,3,0},
		{0,0,0,0},
		{0,0,0,0}
		},
		//O�η���
		new int[,] {
		{0,4,4,0},
		{0,4,4,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//S�η���
		new int[,] {
		{0,5,5,0},
		{5,5,0,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//T�η���
		new int[,] {
		{0,6,0,0},
		{6,6,6,0},
		{0,0,0,0},
		{0,0,0,0},
		},
		//Z�η���
		new int[,] {
		{7,7,0,0},
		{0,7,7,0},
		{0,0,0,0},
		{0,0,0,0},
		},
	};

	//������ɫ
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

	//ˢ�¼��
	private TimeSpan Interval {
		get => _interval;
		set {
			_interval = value;
			if(gameTimer != null) {
				gameTimer.Interval = value;
			}
		}
	}

	//��ʱ��
	private IDispatcherTimer gameTimer;

	private int Score = 0;
	private int Level = 1;

	//��ǰ����
	private static int CurrentBlockIndex { get; set; }

	private static int CurrentRotation { get; set; }
	private static int CurrentRow { get; set; }
	private static int CurrentCal { get; set; }

	public RussionBlock() {
		InitializeComponent();
		InitializeGameGrid();
	}

	//��ʼ����Ϸ��
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

	#region ��ʼ����ͣ�������¼�

	private void OnStartClicked(object sender,EventArgs e) {
		StartGame();
	}

	private void OnPauseClicked(object sender,EventArgs e) {
		PauseGame();
	}

	private void OnResetClicked(object sender,EventArgs e) {
		ResetGame();
	}

	#endregion ��ʼ����ͣ�������¼�

	#region ���ƴ����¼�

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

	#endregion ���ƴ����¼�

	#region ��ʼ����ͣ������

	private void StartGame() {
		GameStateNow = GameState.Playing;
		SpawnNewBlock();
		//������ʱ��
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

	#endregion ��ʼ����ͣ������

	//�����µķ���
	private void SpawnNewBlock() {
		Random random = new Random();
		CurrentBlockIndex = random.Next(0,Blocks.Length);
		CurrentRotation = 0;
		CurrentRow = 0;
		CurrentCal = MaxCol / 2 - Blocks[CurrentBlockIndex].GetLength(1) / 2;
		//��������˾ͽ�����Ϸ
		if(CheckCollection()) {
			GameStateNow = GameState.GameOver;
			gameTimer?.Stop();
			DisplayAlert("��Ϸ����",$"���ĵ÷���{Score}","ȷ��");
		}
		UpdateUI();
	}

	private void UpdateUI() {
	}

	//��ײ���
	private bool CheckCollection() {
		return false;
	}

	//�������
	private void CheckLines() {
	}

	#region �������

	private void MoveLeft() {
	}

	private void MoveRight() {
	}

	private void MoveDown() {
	}

	private void Rotate() {
	}

	#endregion �������

	private enum GameState
	{
		NotStarted,
		Paused,
		Playing,
		GameOver
	}
}