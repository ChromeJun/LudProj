
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

	Coroutine typeWriterTMP = null;

	public bool IsTyping { get; private set; } = false;
	public bool IsDoneTyping { get; private set; } = false;

	public DeathType DisplayDeathType { get; private set; } = DeathType.Cargo;

	WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

	// Use this for initialization
	void Start()
	{
		startDelay = new WaitForSeconds(delayBeforeStart);

		IsTyping = false;
		IsDoneTyping = false;
	}

	public void StartTyping()
	{
		IsDoneTyping = false;

		switch (DisplayDeathType)
		{
			case DeathType.Cargo: writer = cargoDeadCards[Random.Range(0, cargoDeadCards.Length)].text; break;
			case DeathType.Lava: writer = lavaDeathCards[Random.Range(0, lavaDeathCards.Length)].text; break;
			case DeathType.Debris: writer = debrisCrushCards[Random.Range(0, debrisCrushCards.Length)].text; break;
		}

		_tmpProText.text = "";

		IsTyping = true;

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

		int stringCounter = 0;
		float stringTimer = 0.0f;

		while (true)
        {
			stringTimer += Time.deltaTime;

			if (Input.anyKeyDown)
            {
				stringTimer = timeBtwChars;
			}

			yield return null;

			if (stringTimer >= timeBtwChars)
            {
				if (_tmpProText.text.Length > 0)
				{
					_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
				}
				_tmpProText.text += writer[stringCounter];
				_tmpProText.text += leadingChar;

				stringTimer = 0.0f;
				stringCounter += 1;

				if (stringCounter >= writer.Length) break;
			}
        }

		IsDoneTyping = true;

		if (leadingChar != "")
		{
			_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
		}
	}
}
