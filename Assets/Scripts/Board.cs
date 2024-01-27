using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
  private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]
  {
    KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z
  };

  private Row[] rows;

  private int rowIndex;
  private int colIndex;
  private string[] solutions;
  private string[] validWords;
  private string word;
  private int score = 0;

  [Header("States")]
  public Tile.State emptyState;
  public Tile.State occupiedState;
  public Tile.State correctState;
  public Tile.State wrongSpotState;
  public Tile.State incorrectState;

  [Header("UI")]
  public TextMeshProUGUI invalidWord;
  public Button newWordButton;
  public TextMeshProUGUI solWord;
  public TextMeshProUGUI scoreText;
  public TextMeshProUGUI streakText;

  private void Awake()
  {
    rows = GetComponentsInChildren<Row>();
  }

  private void Start()
  {
    LoadData();
    newGame();
  }

  public void newGame()
  {
    clearBoard();
    setRandomWord();
    enabled = true;

  }

  private void setRandomWord()
  {
    word = solutions[Random.Range(0, solutions.Length)];
    word.ToLower().Trim();
    solWord.text = word;
  }

  private void LoadData()
  {
    TextAsset textfile = Resources.Load("official_wordle_all") as TextAsset;
    validWords = textfile.text.Split('\n');

    textfile = Resources.Load("official_wordle_common") as TextAsset;
    solutions = textfile.text.Split('\n');
  }

  private void Update()
  {
    Row currentRow = rows[rowIndex];

    if (Input.GetKeyDown(KeyCode.Backspace))
    {
      colIndex = Mathf.Max(colIndex - 1, 0);
      currentRow.tiles[colIndex].SetLetter('\0');
      currentRow.tiles[colIndex].setState(emptyState);
      invalidWord.gameObject.SetActive(false);
    }

    else if (colIndex >= currentRow.tiles.Length)
    {
      if (Input.GetKeyDown(KeyCode.Return))
      {
        SubmitRow(currentRow);
      }

    }

    else
    {
      for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
      {
        if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
        {
          currentRow.tiles[colIndex].SetLetter((char)SUPPORTED_KEYS[i]);
          currentRow.tiles[colIndex].setState(occupiedState);
          colIndex++;
          break;
        }
      }
    }
  }

  private void SubmitRow(Row row)
  {
    if (!isValidWord(row.word))
    {
      invalidWord.gameObject.SetActive(true);
      return;
    }

    string remaining = word;

    // Loop to check for correct and incorrect letters
    for (int i = 0; i < row.tiles.Length; i++)
    {
      Tile tile = row.tiles[i];

      if (word[i] == tile.letter)
      {
        tile.setState(correctState);
        remaining = remaining.Remove(i, 1);
        remaining = remaining.Insert(i, " ");
      }
      else if (!word.Contains(tile.letter))
      {
        tile.setState(incorrectState);
      }
    }

    // Loop to handle wrong spot letters
    for (int i = 0; i < row.tiles.Length; i++)
    {
      Tile tile = row.tiles[i];

      if (tile.state != correctState && tile.state != incorrectState)
      {
        if (remaining.Contains(tile.letter))
        {
          tile.setState(wrongSpotState);
          int index = remaining.IndexOf(tile.letter);
          remaining = remaining.Remove(index, 1);
          remaining = remaining.Insert(index, " ");

        }
        else
        {
          tile.setState(incorrectState);
        }
      }

    }

    if (hasWon(row))
    {
      score += 1;
      scoreText.text = score.ToString();
      enabled = false;
    }

    rowIndex++;
    colIndex = 0;

    if (rowIndex >= rows.Length)
    {
      solWord.gameObject.SetActive(true);
      score = 0;
      scoreText.text = score.ToString();
      enabled = false;
    }

  }

  private bool isValidWord(string word)
  {
    foreach (string i in validWords)
    {
      if (i == word)
        return true;
    }

    return false;
  }

  private bool hasWon(Row row)
  {
    if (row.word == word)
      return true;
    else
      return false;
  }

  private void OnEnable()
  {
    newWordButton.gameObject.SetActive(false);
    solWord.gameObject.SetActive(false);

  }

  private void OnDisable()
  {
    newWordButton.gameObject.SetActive(true);
  }

  private void clearBoard()
  {
    for (int row = 0; row < rows.Length; row++)
    {
      for (int col = 0; col < rows[row].tiles.Length; col++)
      {
        rows[row].tiles[col].SetLetter('\0');
        rows[row].tiles[col].setState(emptyState);
      }
    }

    rowIndex = colIndex = 0;

  }

}
