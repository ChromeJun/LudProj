
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Typewriter : MonoBehaviour
{
	[SerializeField] TMP_Text _tmpProText;
	string writer;

	[SerializeField] float delayBeforeStart = 0f;
	[SerializeField] float timeBtwChars = 0.1f;
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;

	[Header("Card Variations")]
	[SerializeField] TMP_Text[] cargoDeadCards = null;
	[SerializeField] TMP_Text[] lavaDeathCards = null;
	[SerializeField] TMP_Text[] debrisCrushCards = null;

	WaitForSeconds startDelay = null;
	WaitForSeconds intervalDelay = null;

	Coroutine typeWriterTMP = null;

	public DeathType DisplayDeathType { get; private set; } = DeathType.Cargo;

	// Use this for initialization
	void Start()
	{
		startDelay = new WaitForSeconds(delayBeforeStart);
		intervalDelay = new WaitForSeconds(timeBtwChars);
	}

	public void StartTyping()
	{
		switch (DisplayDeathType)
		{
			case DeathType.Cargo: writer = cargoDeadCards[Random.Range(0, cargoDeadCards.Length)].text; break;
			case DeathType.Lava: writer = lavaDeathCards[Random.Range(0, lavaDeathCards.Length)].text; break;
			case DeathType.Debris: writer = debrisCrushCards[Random.Range(0, debrisCrushCards.Length)].text; break;
		}

		writer = _tmpProText.text;

		_tmpProText.text = "";

		if (typeWriterTMP != null) StopCoroutine(typeWriterTMP);
		typeWriterTMP = StartCoroutine(TypeWriterTMP());
	}

	public void SetDeathType(DeathType deathType)
	{
		DisplayDeathType = deathType;
	}

	IEnumerator TypeWriterTMP()
	{
		_tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";

		yield return startDelay;

		foreach (char c in writer)
		{
			if (_tmpProText.text.Length > 0)
			{
				_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
			}
			_tmpProText.text += c;
			_tmpProText.text += leadingChar;
			yield return intervalDelay;
		}

		if (leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
		}
	}
}
