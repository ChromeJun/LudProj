
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleCard : MonoBehaviour
{
	[SerializeField] TMP_Text _tmpProText;
	string writer;

	[SerializeField] float delayBeforeStart = 0f;
	[SerializeField] float delayBeforeErasure = 0f;
	[SerializeField] float timeBtwChars = 0.1f;
	[SerializeField] float eraseInterval = 0.1f;
	[SerializeField] string leadingChar = "";
	[SerializeField] bool leadingCharBeforeDelay = false;

	WaitForSeconds startDelay = null;
	WaitForSeconds eraseDelay = null;

	Coroutine typeWriterTMP = null;

	public bool IsTyping { get; private set; } = false;
	public bool IsDoneTyping { get; private set; } = false;

	WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

	// Use this for initialization
	void Start()
	{
		startDelay = new WaitForSeconds(delayBeforeStart);
		eraseDelay = new WaitForSeconds(delayBeforeErasure);

		IsTyping = false;
		IsDoneTyping = false;

		StartTyping();
	}

	public void StartTyping()
	{
		IsDoneTyping = false;

		writer = _tmpProText.text;
		_tmpProText.text = "";

		IsTyping = true;

		if (typeWriterTMP != null) StopCoroutine(typeWriterTMP);
		typeWriterTMP = StartCoroutine(TypeWriterTMP());
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

			yield return endOfFrame;

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

		yield return eraseDelay;

		stringTimer = 0.0f;

		while (_tmpProText.text.Length > 0)
		{
			stringTimer += Time.deltaTime;

			yield return endOfFrame;

			if (stringTimer >= eraseInterval)
			{
				if (_tmpProText.text.Length > 0)
				{
					_tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - 1);
				}

				stringTimer = 0.0f;
			}
		}
	}
}
