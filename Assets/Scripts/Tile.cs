using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
  [System.Serializable]
  public class State
  {
    public Color fillColor;
    public Color outlineColor;
  }

  private TextMeshProUGUI text;
  private Image fill;
  private Outline outline;

  public char letter { get; private set; }
  public State state { get; private set; }

  private void Awake()
  {
    text = GetComponentInChildren<TextMeshProUGUI>();
    fill = GetComponent<Image>();
    outline = GetComponent<Outline>();

  }

  public void SetLetter(char letter)
  {
    this.letter = letter;
    text.text = letter.ToString();
  }

  public void setState(State state)
  {
    this.state = state;
    fill.color = state.fillColor;
    outline.effectColor = state.outlineColor;
  }
}
