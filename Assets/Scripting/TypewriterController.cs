using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypewriterController : MonoBehaviour
{
    [SerializeField] Typewriter typewriter = null;

    public void StartTyping()
    {
        typewriter.StartTyping();
    }
}
